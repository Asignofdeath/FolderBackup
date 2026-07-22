namespace FolderBackup
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ─── 控件声明 ───────────────────────────────────────────
            var panelHeader        = new Panel();
            var lblTitle           = new Label();
            var lblSubtitle        = new Label();

            var panelPaths         = new Panel();
            var lblSourceTitle     = new Label();
            txtSource              = new TextBox();
            btnBrowseSource        = new Button();
            var lblBackupTitle     = new Label();
            txtBackup              = new TextBox();
            btnBrowseBackup        = new Button();

            var panelActions       = new Panel();
            btnBackup              = new Button();
            btnRestore             = new Button();
            lblLastRestore         = new Label();

            var panelList          = new Panel();
            var lblListTitle       = new Label();
            lblBackupCount         = new Label();
            listBackups            = new ListView();
            var colName            = new ColumnHeader();
            var colDate            = new ColumnHeader();
            var colFullPath        = new ColumnHeader();
            var colSize            = new ColumnHeader();

            var panelListActions   = new Panel();
            btnRefresh             = new Button();
            btnDelete              = new Button();
            btnOpenBackupFolder    = new Button();

            var panelStatus        = new Panel();
            progressBar            = new ProgressBar();
            lblStatus              = new Label();

            // ─── 颜色常量 ──────────────────────────────────────────
            var clrBg        = System.Drawing.Color.FromArgb(30, 30, 46);
            var clrSurface   = System.Drawing.Color.FromArgb(49, 50, 68);
            var clrMantle    = System.Drawing.Color.FromArgb(24, 24, 37);
            var clrText      = System.Drawing.Color.FromArgb(205, 214, 244);
            var clrSubtext   = System.Drawing.Color.FromArgb(166, 173, 200);
            var clrBlue      = System.Drawing.Color.FromArgb(137, 180, 250);
            var clrGreen     = System.Drawing.Color.FromArgb(166, 227, 161);
            var clrRed       = System.Drawing.Color.FromArgb(243, 139, 168);
            var clrOverlay   = System.Drawing.Color.FromArgb(108, 112, 134);
            var fontMain     = new System.Drawing.Font("微软雅黑", 9.5f);
            var fontBold     = new System.Drawing.Font("微软雅黑", 9.5f, System.Drawing.FontStyle.Bold);
            var fontSmall    = new System.Drawing.Font("微软雅黑", 8.5f);
            var fontLarge    = new System.Drawing.Font("微软雅黑", 13f, System.Drawing.FontStyle.Bold);

            // ════════════════════════════════════════════════════════
            //  窗体
            // ════════════════════════════════════════════════════════
            this.SuspendLayout();
            this.Text            = "📦 文件夹备份工具";
            this.Size            = new System.Drawing.Size(780, 660);
            this.MinimumSize     = new System.Drawing.Size(780, 620);
            this.BackColor       = clrBg;
            this.Font            = fontMain;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;

            // ════════════════════════════════════════════════════════
            //  Header Panel
            // ════════════════════════════════════════════════════════
            panelHeader.Dock      = System.Windows.Forms.DockStyle.Top;
            panelHeader.Height    = 72;
            panelHeader.BackColor = clrMantle;
            panelHeader.Padding   = new Padding(20, 0, 20, 0);

            lblTitle.Text      = "📦 文件夹备份工具";
            lblTitle.Font      = fontLarge;
            lblTitle.ForeColor = clrBlue;
            lblTitle.AutoSize  = true;
            lblTitle.Location  = new System.Drawing.Point(20, 12);

            lblSubtitle.Text      = "压缩备份 · 一键还原 · 文件安全";
            lblSubtitle.Font      = fontSmall;
            lblSubtitle.ForeColor = clrSubtext;
            lblSubtitle.AutoSize  = true;
            lblSubtitle.Location  = new System.Drawing.Point(23, 42);

            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(lblSubtitle);

            // ════════════════════════════════════════════════════════
            //  Paths Panel
            // ════════════════════════════════════════════════════════
            panelPaths.Dock      = System.Windows.Forms.DockStyle.Top;
            panelPaths.Height    = 120;
            panelPaths.BackColor = clrSurface;
            panelPaths.Padding   = new Padding(20, 10, 20, 10);

            // 源文件夹
            lblSourceTitle.Text      = "源文件夹";
            lblSourceTitle.Font      = fontBold;
            lblSourceTitle.ForeColor = clrText;
            lblSourceTitle.AutoSize  = true;
            lblSourceTitle.Location  = new System.Drawing.Point(20, 12);

            txtSource.Location    = new System.Drawing.Point(20, 34);
            txtSource.Width       = 580;
            txtSource.BackColor   = clrBg;
            txtSource.ForeColor   = clrText;
            txtSource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtSource.Font        = fontMain;
            txtSource.PlaceholderText = "请输入或选择源文件夹路径…";
            txtSource.Leave      += txtSource_Leave;

            btnBrowseSource.Text      = "浏览…";
            btnBrowseSource.Location  = new System.Drawing.Point(608, 32);
            btnBrowseSource.Size      = new System.Drawing.Size(80, 26);
            btnBrowseSource.BackColor = clrOverlay;
            btnBrowseSource.ForeColor = clrText;
            btnBrowseSource.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnBrowseSource.FlatAppearance.BorderColor = clrOverlay;
            btnBrowseSource.Font      = fontSmall;
            btnBrowseSource.Click    += btnBrowseSource_Click;

            // 备份文件夹
            lblBackupTitle.Text      = "备份存储文件夹";
            lblBackupTitle.Font      = fontBold;
            lblBackupTitle.ForeColor = clrText;
            lblBackupTitle.AutoSize  = true;
            lblBackupTitle.Location  = new System.Drawing.Point(20, 72);

            txtBackup.Location    = new System.Drawing.Point(20, 94);
            txtBackup.Width       = 580;
            txtBackup.BackColor   = clrBg;
            txtBackup.ForeColor   = clrText;
            txtBackup.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtBackup.Font        = fontMain;
            txtBackup.PlaceholderText = "请输入或选择备份存储文件夹路径…";
            txtBackup.Leave      += txtBackup_Leave;

            btnBrowseBackup.Text      = "浏览…";
            btnBrowseBackup.Location  = new System.Drawing.Point(608, 92);
            btnBrowseBackup.Size      = new System.Drawing.Size(80, 26);
            btnBrowseBackup.BackColor = clrOverlay;
            btnBrowseBackup.ForeColor = clrText;
            btnBrowseBackup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnBrowseBackup.FlatAppearance.BorderColor = clrOverlay;
            btnBrowseBackup.Font      = fontSmall;
            btnBrowseBackup.Click    += btnBrowseBackup_Click;

            panelPaths.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                lblSourceTitle, txtSource, btnBrowseSource,
                lblBackupTitle, txtBackup, btnBrowseBackup
            });

            // ════════════════════════════════════════════════════════
            //  Actions Panel
            // ════════════════════════════════════════════════════════
            panelActions.Dock      = System.Windows.Forms.DockStyle.Top;
            panelActions.Height    = 60;
            panelActions.BackColor = clrMantle;
            panelActions.Padding   = new Padding(20, 10, 20, 10);

            btnBackup.Text      = "💾  立即备份";
            btnBackup.Size      = new System.Drawing.Size(140, 38);
            btnBackup.Location  = new System.Drawing.Point(20, 10);
            btnBackup.BackColor = clrBlue;
            btnBackup.ForeColor = clrMantle;
            btnBackup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnBackup.FlatAppearance.BorderSize = 0;
            btnBackup.Font      = fontBold;
            btnBackup.Click    += btnBackup_Click;

            btnRestore.Text      = "🔄  还原备份";
            btnRestore.Size      = new System.Drawing.Size(140, 38);
            btnRestore.Location  = new System.Drawing.Point(172, 10);
            btnRestore.BackColor = clrGreen;
            btnRestore.ForeColor = clrMantle;
            btnRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnRestore.FlatAppearance.BorderSize = 0;
            btnRestore.Font      = fontBold;
            btnRestore.Click    += btnRestore_Click;

            // 上次还原时间标签
            lblLastRestore.Text      = "上次还原：—";
            lblLastRestore.Font      = fontSmall;
            lblLastRestore.ForeColor = clrSubtext;
            lblLastRestore.AutoSize  = true;
            lblLastRestore.Location  = new System.Drawing.Point(326, 20);

            panelActions.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                btnBackup, btnRestore, lblLastRestore
            });

            // ════════════════════════════════════════════════════════
            //  List Panel  (填满剩余空间)
            // ════════════════════════════════════════════════════════
            panelList.Dock      = System.Windows.Forms.DockStyle.Fill;
            panelList.BackColor = clrBg;
            panelList.Padding   = new Padding(20, 8, 20, 8);

            lblListTitle.Text      = "备份文件列表";
            lblListTitle.Font      = fontBold;
            lblListTitle.ForeColor = clrText;
            lblListTitle.AutoSize  = true;
            lblListTitle.Location  = new System.Drawing.Point(20, 10);

            lblBackupCount.Text      = "共 0 个备份";
            lblBackupCount.Font      = fontSmall;
            lblBackupCount.ForeColor = clrSubtext;
            lblBackupCount.AutoSize  = true;
            lblBackupCount.Location  = new System.Drawing.Point(160, 13);

            // ListView
            listBackups.Location         = new System.Drawing.Point(20, 36);
            listBackups.Anchor           = System.Windows.Forms.AnchorStyles.Top
                                         | System.Windows.Forms.AnchorStyles.Bottom
                                         | System.Windows.Forms.AnchorStyles.Left
                                         | System.Windows.Forms.AnchorStyles.Right;
            listBackups.Size             = new System.Drawing.Size(720, 340);
            listBackups.View             = System.Windows.Forms.View.Details;
            listBackups.FullRowSelect    = true;
            listBackups.GridLines        = false;
            listBackups.BackColor        = clrSurface;
            listBackups.ForeColor        = clrText;
            listBackups.Font             = fontMain;
            listBackups.BorderStyle      = System.Windows.Forms.BorderStyle.None;
            listBackups.MultiSelect      = false;
            listBackups.DoubleClick     += listBackups_DoubleClick;

            colName.Text  = "备份文件名";
            colName.Width = 360;
            colDate.Text  = "备份时间";
            colDate.Width = 140;
            colFullPath.Text  = "路径";
            colFullPath.Width = 0;    // 隐藏
            colSize.Text  = "大小";
            colSize.Width = 80;

            listBackups.Columns.AddRange(new[] { colName, colDate, colFullPath, colSize });

            // 列表操作按钮区
            panelListActions.Location  = new System.Drawing.Point(20, 384);
            panelListActions.Anchor    = System.Windows.Forms.AnchorStyles.Bottom
                                       | System.Windows.Forms.AnchorStyles.Left;
            panelListActions.Size      = new System.Drawing.Size(720, 40);
            panelListActions.BackColor = clrBg;

            btnRefresh.Text      = "🔃 刷新列表";
            btnRefresh.Size      = new System.Drawing.Size(110, 32);
            btnRefresh.Location  = new System.Drawing.Point(0, 4);
            btnRefresh.BackColor = clrOverlay;
            btnRefresh.ForeColor = clrText;
            btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Font      = fontSmall;
            btnRefresh.Click    += btnRefresh_Click;

            btnDelete.Text      = "🗑 删除备份";
            btnDelete.Size      = new System.Drawing.Size(110, 32);
            btnDelete.Location  = new System.Drawing.Point(118, 4);
            btnDelete.BackColor = clrRed;
            btnDelete.ForeColor = clrMantle;
            btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Font      = fontSmall;
            btnDelete.Click    += btnDelete_Click;

            btnOpenBackupFolder.Text      = "📂 打开备份文件夹";
            btnOpenBackupFolder.Size      = new System.Drawing.Size(140, 32);
            btnOpenBackupFolder.Location  = new System.Drawing.Point(236, 4);
            btnOpenBackupFolder.BackColor = clrOverlay;
            btnOpenBackupFolder.ForeColor = clrText;
            btnOpenBackupFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnOpenBackupFolder.FlatAppearance.BorderSize = 0;
            btnOpenBackupFolder.Font      = fontSmall;
            btnOpenBackupFolder.Click    += btnOpenBackupFolder_Click;

            panelListActions.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                btnRefresh, btnDelete, btnOpenBackupFolder
            });

            panelList.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                lblListTitle, lblBackupCount, listBackups, panelListActions
            });

            // ════════════════════════════════════════════════════════
            //  Status Bar
            // ════════════════════════════════════════════════════════
            panelStatus.Dock      = System.Windows.Forms.DockStyle.Bottom;
            panelStatus.Height    = 36;
            panelStatus.BackColor = clrMantle;

            progressBar.Location  = new System.Drawing.Point(16, 10);
            progressBar.Size      = new System.Drawing.Size(200, 16);
            progressBar.Style     = System.Windows.Forms.ProgressBarStyle.Blocks;
            progressBar.Maximum   = 100;

            lblStatus.Text      = "就绪";
            lblStatus.Font      = fontSmall;
            lblStatus.ForeColor = clrSubtext;
            lblStatus.AutoSize  = true;
            lblStatus.Location  = new System.Drawing.Point(228, 10);

            panelStatus.Controls.Add(progressBar);
            panelStatus.Controls.Add(lblStatus);

            // ════════════════════════════════════════════════════════
            //  Add to Form (注意顺序: Bottom → Fill → Top panels)
            // ════════════════════════════════════════════════════════
            this.Controls.Add(panelList);      // Fill
            this.Controls.Add(panelActions);   // Top 3
            this.Controls.Add(panelPaths);     // Top 2
            this.Controls.Add(panelHeader);    // Top 1
            this.Controls.Add(panelStatus);    // Bottom

            this.ResumeLayout(false);
        }

        #endregion

        // ─── 控件字段 ─────────────────────────────────────────────
        private TextBox    txtSource          = null!;
        private TextBox    txtBackup          = null!;
        private Button     btnBrowseSource    = null!;
        private Button     btnBrowseBackup    = null!;
        private Button     btnBackup          = null!;
        private Button     btnRestore         = null!;
        private Label      lblLastRestore     = null!;
        private ListView   listBackups        = null!;
        private Button     btnRefresh         = null!;
        private Button     btnDelete          = null!;
        private Button     btnOpenBackupFolder = null!;
        private ProgressBar progressBar       = null!;
        private Label      lblStatus          = null!;
        private Label      lblBackupCount     = null!;
    }
}
