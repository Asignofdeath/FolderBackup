using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderBackup
{
    public partial class MainForm : Form
    {
        // ─── 配置持久化路径 ────────────────────────────────────────
        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FolderBackup", "settings.json");

        // ─── 状态 ─────────────────────────────────────────────────
        private bool _isBusy = false;

        /// <summary>上次还原操作时间；null 表示从未还原过</summary>
        private DateTime? _lastRestoreTime = null;

        /// <summary>上次还原的备份标签（【】内的内容）；null 或空表示无标签</summary>
        private string? _lastRestoreLabel = null;

        public MainForm()
        {
            InitializeComponent();
            LoadSettings();
            RefreshBackupList();
            ApplyTheme();
        }

        // ════════════════════════════════════════════════════════════
        //  设置读写
        // ════════════════════════════════════════════════════════════
        private void LoadSettings()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var json = File.ReadAllText(ConfigPath);
                    var doc  = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("sourcePath", out var sp))
                        txtSource.Text = sp.GetString() ?? "";
                    if (root.TryGetProperty("backupPath", out var bp))
                        txtBackup.Text = bp.GetString() ?? "";
                    // 读取上次还原时间（ISO 8601 字符串）
                    if (root.TryGetProperty("lastRestoreTime", out var lrt) &&
                        DateTime.TryParse(lrt.GetString(), out var dt))
                    {
                        _lastRestoreTime = dt;
                    }
                    // 读取上次还原标签
                    if (root.TryGetProperty("lastRestoreLabel", out var lrl))
                        _lastRestoreLabel = lrl.GetString();

                    if (_lastRestoreTime.HasValue)
                        UpdateLastRestoreLabel();
                }
            }
            catch { /* 首次运行时忽略 */ }
        }

        private void SaveSettings()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
                var obj = new
                {
                    sourcePath       = txtSource.Text.Trim(),
                    backupPath       = txtBackup.Text.Trim(),
                    // 保留上次还原时间（null 时写空字符串）
                    lastRestoreTime  = _lastRestoreTime?.ToString("O") ?? "",
                    lastRestoreLabel = _lastRestoreLabel ?? ""
                };
                File.WriteAllText(ConfigPath, JsonSerializer.Serialize(obj));
            }
            catch { }
        }

        // ════════════════════════════════════════════════════════════
        //  浏览按钮
        // ════════════════════════════════════════════════════════════
        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog
            {
                Description  = "选择源文件夹",
                SelectedPath = txtSource.Text.Trim()
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtSource.Text = dlg.SelectedPath;
                SaveSettings();
            }
        }

        private void btnBrowseBackup_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog
            {
                Description  = "选择备份存储文件夹",
                SelectedPath = txtBackup.Text.Trim()
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtBackup.Text = dlg.SelectedPath;
                SaveSettings();
            }
        }

        private void txtSource_Leave(object sender, EventArgs e) => SaveSettings();
        private void txtBackup_Leave(object sender, EventArgs e) => SaveSettings();

        // ════════════════════════════════════════════════════════════
        //  备份
        // ════════════════════════════════════════════════════════════
        private async void btnBackup_Click(object sender, EventArgs e)
        {
            if (_isBusy) return;

            var source = txtSource.Text.Trim();
            var backup = txtBackup.Text.Trim();

            if (!Directory.Exists(source))
            {
                ShowError("源文件夹不存在，请重新指定。");
                return;
            }
            if (string.IsNullOrEmpty(backup))
            {
                ShowError("请指定备份存储文件夹。");
                return;
            }

            // 弹出标签输入框
            var label = ShowLabelInputDialog();
            if (label == null) return; // 用户取消

            try
            {
                _isBusy = true;
                SetBusyState(true);
                SaveSettings();

                Directory.CreateDirectory(backup);

                // 构建文件名
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var folderName = Path.GetFileName(source.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                string fileName;
                if (!string.IsNullOrWhiteSpace(label))
                    fileName = $"【{label}】{folderName}_{timestamp}.zip";
                else
                    fileName = $"{folderName}_{timestamp}.zip";

                var zipPath = Path.Combine(backup, fileName);

                SetStatus("正在压缩，请稍候…");
                progressBar.Style = ProgressBarStyle.Marquee;

                await Task.Run(() => ZipDirectory(source, zipPath));

                progressBar.Style = ProgressBarStyle.Blocks;
                progressBar.Value = 100;
                SetStatus($"✅ 备份完成：{fileName}");
                RefreshBackupList();
            }
            catch (Exception ex)
            {
                progressBar.Style = ProgressBarStyle.Blocks;
                SetStatus("❌ 备份失败");
                ShowError($"备份时发生错误：\n{ex.Message}");
            }
            finally
            {
                _isBusy = false;
                SetBusyState(false);
            }
        }

        /// <summary>将目录压缩为 zip（支持嵌套）</summary>
        private static void ZipDirectory(string sourceDir, string zipPath)
        {
            if (File.Exists(zipPath)) File.Delete(zipPath);
            ZipFile.CreateFromDirectory(sourceDir, zipPath, CompressionLevel.Optimal, includeBaseDirectory: true);
        }

        // ════════════════════════════════════════════════════════════
        //  还原
        // ════════════════════════════════════════════════════════════
        private async void btnRestore_Click(object sender, EventArgs e)
        {
            if (_isBusy) return;

            if (listBackups.SelectedItems.Count == 0)
            {
                ShowError("请先在列表中选择一个备份文件。");
                return;
            }

            var source = txtSource.Text.Trim();
            if (string.IsNullOrEmpty(source))
            {
                ShowError("请指定源文件夹路径。");
                return;
            }

            var zipPath = listBackups.SelectedItems[0].SubItems[2].Text; // 第3列存完整路径
            if (!File.Exists(zipPath))
            {
                ShowError("备份文件不存在，可能已被移动或删除。");
                RefreshBackupList();
                return;
            }

            // 确认
            var ans = MessageBox.Show(
                $"即将还原备份：\n{Path.GetFileName(zipPath)}\n\n" +
                $"⚠ 目标文件夹内所有文件将被删除后替换：\n{source}\n\n确定要继续吗？",
                "确认还原", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (ans != DialogResult.Yes) return;

            // 检查占用（文件 + 文件夹）
            SetStatus("正在检查文件夹占用…");
            List<string> lockedItems;
            try
            {
                lockedItems = await Task.Run(() => GetLockedItems(source));
            }
            catch (Exception ex)
            {
                ShowError($"检查占用时发生错误：\n{ex.Message}");
                SetStatus("❌ 还原取消");
                return;
            }

            if (lockedItems.Count > 0)
            {
                var itemList = string.Join("\n  • ", lockedItems);
                var more     = lockedItems.Count >= 10 ? "\n  …（仍有更多被占用项）" : "";
                ShowOccupiedError(
                    $"检测到以下文件或文件夹正被其他程序占用，" +
                    $"删除操作将会失败。\n\n" +
                    $"请先关闭占用这些文件/文件夹的程序，再重试还原。\n\n" +
                    $"被占用的项目：\n  • {itemList}{more}");
                SetStatus("❌ 还原取消：检测到占用");
                return;
            }

            try
            {
                _isBusy = true;
                SetBusyState(true);
                progressBar.Style = ProgressBarStyle.Marquee;
                SetStatus("正在删除源文件夹内容…");

                await Task.Run(() =>
                {
                    // 删除源文件夹内所有内容（保留文件夹本身）
                    if (Directory.Exists(source))
                    {
                        foreach (var f in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
                            File.Delete(f);
                        foreach (var d in Directory.GetDirectories(source).OrderByDescending(x => x.Length))
                            Directory.Delete(d, recursive: true);
                    }
                    else
                    {
                        Directory.CreateDirectory(source);
                    }
                });

                SetStatus("正在解压备份文件…");

                await Task.Run(() =>
                {
                    // 解压 zip，zip 内有顶层文件夹（因为 includeBaseDirectory=true）
                    // 需要把 zip 内顶层目录下的内容解压到 source
                    using var archive = ZipFile.OpenRead(zipPath);
                    var firstEntry  = archive.Entries.FirstOrDefault();
                    if (firstEntry == null) return;

                    // 找公共前缀（zip 中的根目录名）
                    var prefix = firstEntry.FullName.Split('/')[0] + "/";

                    foreach (var entry in archive.Entries)
                    {
                        if (entry.FullName == prefix) continue; // 跳过根目录本身

                        // 去掉前缀，映射到 source
                        var relativePath = entry.FullName.StartsWith(prefix)
                            ? entry.FullName[prefix.Length..]
                            : entry.FullName;

                        if (string.IsNullOrEmpty(relativePath)) continue;

                        var destPath = Path.Combine(source, relativePath.Replace('/', Path.DirectorySeparatorChar));

                        if (entry.FullName.EndsWith("/"))
                        {
                            Directory.CreateDirectory(destPath);
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                            entry.ExtractToFile(destPath, overwrite: true);
                        }
                    }
                });

                progressBar.Style = ProgressBarStyle.Blocks;
                progressBar.Value = 100;
                SetStatus("✅ 还原完成！");

                // 从文件名提取【标签】（格式：【xxx】文件名_时间戳.zip）
                var zipFileName = Path.GetFileName(zipPath);
                string? extractedLabel = null;
                if (zipFileName.StartsWith("【"))
                {
                    var closeIdx = zipFileName.IndexOf('】');
                    if (closeIdx > 1)
                        extractedLabel = zipFileName.Substring(1, closeIdx - 1);
                }

                // 记录并展示上次还原时间 + 标签
                _lastRestoreTime  = DateTime.Now;
                _lastRestoreLabel = extractedLabel;
                UpdateLastRestoreLabel();
                SaveSettings();

                MessageBox.Show("还原成功！", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                progressBar.Style = ProgressBarStyle.Blocks;
                SetStatus("❌ 还原失败");
                ShowError($"还原时发生错误：\n{ex.Message}");
            }
            finally
            {
                _isBusy = false;
                SetBusyState(false);
            }
        }

        // ════════════════════════════════════════════════════════════
        //  检查文件夹是否被占用（文件 + 子目录均检测）
        // ════════════════════════════════════════════════════════════
        /// <summary>
        /// 检测文件夹内所有文件与子目录是否被其他进程占用。
        /// 返回所有被占用的路径列表；列表为空表示无占用。
        /// </summary>
        private static List<string> GetLockedItems(string folder)
        {
            var locked = new List<string>();
            if (!Directory.Exists(folder)) return locked;

            // 1. 检测文件是否被独占锁定
            foreach (var file in Directory.GetFiles(folder, "*", SearchOption.AllDirectories))
            {
                try
                {
                    using var fs = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (IOException)
                {
                    locked.Add(file);
                    if (locked.Count >= 10) return locked; // 最多列出 10 条，防止太多
                }
                catch { }
            }

            // 2. 检测子目录是否能被删除（资源管理器打开目录会阻止 Directory.Delete）
            //    方法：尝试 rename 目录再 rename 回来，若失败说明被锁
            foreach (var dir in Directory.GetDirectories(folder, "*", SearchOption.AllDirectories))
            {
                var testName = dir + "__locktest__";
                try
                {
                    Directory.Move(dir, testName);
                    Directory.Move(testName, dir); // 立即改回
                }
                catch (IOException)
                {
                    locked.Add(dir + Path.DirectorySeparatorChar + " [文件夹]");
                    if (locked.Count >= 10) return locked;
                }
                catch { }
            }

            return locked;
        }

        // ════════════════════════════════════════════════════════════
        //  刷新备份列表
        // ════════════════════════════════════════════════════════════
        private void btnRefresh_Click(object sender, EventArgs e) => RefreshBackupList();

        private void RefreshBackupList()
        {
            listBackups.Items.Clear();
            var backup = txtBackup.Text.Trim();
            if (string.IsNullOrEmpty(backup) || !Directory.Exists(backup)) return;

            var files = Directory.GetFiles(backup, "*.zip")
                                 .Select(f => new FileInfo(f))
                                 .OrderByDescending(f => f.LastWriteTime)
                                 .ToList();

            foreach (var fi in files)
            {
                var item = new ListViewItem(fi.Name);
                item.SubItems.Add(fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"));
                item.SubItems.Add(fi.FullName);                         // 隐藏完整路径
                item.SubItems.Add(FormatFileSize(fi.Length));
                listBackups.Items.Add(item);
            }

            lblBackupCount.Text = $"共 {files.Count} 个备份";
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes >= 1_073_741_824) return $"{bytes / 1_073_741_824.0:F2} GB";
            if (bytes >= 1_048_576)     return $"{bytes / 1_048_576.0:F1} MB";
            if (bytes >= 1_024)         return $"{bytes / 1_024.0:F1} KB";
            return $"{bytes} B";
        }

        // ════════════════════════════════════════════════════════════
        //  删除备份文件
        // ════════════════════════════════════════════════════════════
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listBackups.SelectedItems.Count == 0) return;

            var zipPath = listBackups.SelectedItems[0].SubItems[2].Text;
            var ans = MessageBox.Show(
                $"确定要删除备份文件吗？\n{Path.GetFileName(zipPath)}",
                "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans != DialogResult.Yes) return;

            try
            {
                if (File.Exists(zipPath)) File.Delete(zipPath);
                RefreshBackupList();
                SetStatus("已删除备份文件");
            }
            catch (Exception ex)
            {
                ShowError($"删除失败：{ex.Message}");
            }
        }

        // ════════════════════════════════════════════════════════════
        //  打开备份文件夹
        // ════════════════════════════════════════════════════════════
        private void btnOpenBackupFolder_Click(object sender, EventArgs e)
        {
            var backup = txtBackup.Text.Trim();
            if (Directory.Exists(backup))
                Process.Start("explorer.exe", backup);
            else
                ShowError("备份文件夹不存在。");
        }

        // ════════════════════════════════════════════════════════════
        //  标签输入弹窗
        // ════════════════════════════════════════════════════════════
        private string? ShowLabelInputDialog()
        {
            using var dlg   = new Form();
            dlg.Text        = "备份标签";
            dlg.Size        = new Size(380, 160);
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox = false;
            dlg.MinimizeBox = false;
            dlg.BackColor   = Color.FromArgb(30, 30, 46);

            var lbl         = new Label
            {
                Text        = "请输入备份标签（可为空，留空则不添加标签）：",
                ForeColor   = Color.FromArgb(205, 214, 244),
                Location    = new Point(16, 16),
                AutoSize    = true
            };

            var txt         = new TextBox
            {
                Location    = new Point(16, 44),
                Width       = 336,
                BackColor   = Color.FromArgb(49, 50, 68),
                ForeColor   = Color.FromArgb(205, 214, 244),
                BorderStyle = BorderStyle.FixedSingle,
                Font        = new Font("微软雅黑", 10)
            };

            var btnOk       = new Button
            {
                Text        = "确定",
                DialogResult = DialogResult.OK,
                Location    = new Point(185, 80),
                Width       = 80,
                BackColor   = Color.FromArgb(137, 180, 250),
                ForeColor   = Color.FromArgb(30, 30, 46),
                FlatStyle   = FlatStyle.Flat
            };
            btnOk.FlatAppearance.BorderSize = 0;

            var btnCancel   = new Button
            {
                Text        = "取消",
                DialogResult = DialogResult.Cancel,
                Location    = new Point(272, 80),
                Width       = 80,
                BackColor   = Color.FromArgb(69, 71, 90),
                ForeColor   = Color.FromArgb(205, 214, 244),
                FlatStyle   = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            dlg.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCancel });
            dlg.AcceptButton = btnOk;
            dlg.CancelButton = btnCancel;

            // 让 txt 获取焦点
            dlg.Shown += (s, ev) => txt.Focus();

            var result = dlg.ShowDialog(this);
            if (result == DialogResult.OK)
                return txt.Text.Trim();
            return null; // 取消
        }

        // ════════════════════════════════════════════════════════════
        //  UI 辅助
        // ════════════════════════════════════════════════════════════
        private void SetStatus(string msg)
        {
            if (InvokeRequired) { Invoke(() => SetStatus(msg)); return; }
            lblStatus.Text = msg;
        }

        /// <summary>刷新"上次还原"标签显示</summary>
        private void UpdateLastRestoreLabel()
        {
            if (InvokeRequired) { Invoke(UpdateLastRestoreLabel); return; }
            if (_lastRestoreTime.HasValue)
            {
                var t = _lastRestoreTime.Value;
                // 格式：M月d日 H时m分s秒  【标签】（无标签时不显示）
                var timeStr  = $"{t.Month}月{t.Day}日 {t.Hour}时{t.Minute}分{t.Second}秒";
                var labelStr = !string.IsNullOrWhiteSpace(_lastRestoreLabel)
                    ? $"  【{_lastRestoreLabel}】"
                    : "";
                lblLastRestore.Text      = $"上次还原：{timeStr}{labelStr}";
                lblLastRestore.ForeColor = System.Drawing.Color.FromArgb(166, 227, 161); // clrGreen
            }
            else
            {
                lblLastRestore.Text      = "上次还原：—";
                lblLastRestore.ForeColor = System.Drawing.Color.FromArgb(166, 173, 200); // clrSubtext
            }
        }

        private void SetBusyState(bool busy)
        {
            if (InvokeRequired) { Invoke(() => SetBusyState(busy)); return; }
            btnBackup.Enabled  = !busy;
            btnRestore.Enabled = !busy;
            btnDelete.Enabled  = !busy;
            if (!busy) progressBar.Style = ProgressBarStyle.Blocks;
        }

        private static void ShowError(string msg)
        {
            MessageBox.Show(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>专门用于展示"文件/文件夹被占用"的宽幅提示框</summary>
        private void ShowOccupiedError(string msg)
        {
            using var dlg = new Form();
            dlg.Text        = "⚠ 无法还原 — 检测到占用";
            dlg.Size        = new Size(560, 340);
            dlg.MinimumSize = new Size(480, 260);
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.FormBorderStyle = FormBorderStyle.Sizable;
            dlg.BackColor   = Color.FromArgb(30, 30, 46);

            var ico = new PictureBox
            {
                Location = new Point(16, 16),
                Size     = new Size(32, 32),
                Image    = SystemIcons.Warning.ToBitmap(),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            var lblMsg = new Label
            {
                Text      = "检测到文件或文件夹正被其他程序占用",
                ForeColor = Color.FromArgb(243, 139, 168),
                Font      = new Font("微软雅黑", 10f, FontStyle.Bold),
                Location  = new Point(56, 20),
                AutoSize  = true
            };

            var txt = new TextBox
            {
                Text       = msg,
                Multiline  = true,
                ReadOnly   = true,
                ScrollBars = ScrollBars.Vertical,
                Location   = new Point(16, 58),
                Anchor     = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Size       = new Size(520, 200),
                BackColor  = Color.FromArgb(49, 50, 68),
                ForeColor  = Color.FromArgb(205, 214, 244),
                BorderStyle = BorderStyle.None,
                Font       = new Font("微软雅黑", 9f)
            };

            var btnOk = new Button
            {
                Text        = "我知道了",
                DialogResult = DialogResult.OK,
                Anchor      = AnchorStyles.Bottom | AnchorStyles.Right,
                Size        = new Size(100, 32),
                BackColor   = Color.FromArgb(137, 180, 250),
                ForeColor   = Color.FromArgb(30, 30, 46),
                FlatStyle   = FlatStyle.Flat,
                Font        = new Font("微软雅黑", 9.5f, FontStyle.Bold)
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Location = new Point(dlg.ClientSize.Width - 116, dlg.ClientSize.Height - 44);
            dlg.Resize += (s, ev) =>
                btnOk.Location = new Point(dlg.ClientSize.Width - 116, dlg.ClientSize.Height - 44);

            dlg.Controls.AddRange(new Control[] { ico, lblMsg, txt, btnOk });
            dlg.AcceptButton = btnOk;
            dlg.ShowDialog(this);
        }

        private void ApplyTheme()
        {
            // 已在 Designer 中配置，此处可做动态调整
        }

        // 双击列表打开所在目录并选中文件
        private void listBackups_DoubleClick(object sender, EventArgs e)
        {
            if (listBackups.SelectedItems.Count == 0) return;
            var path = listBackups.SelectedItems[0].SubItems[2].Text;
            if (File.Exists(path))
                Process.Start("explorer.exe", $"/select,\"{path}\"");
        }
    }
}
