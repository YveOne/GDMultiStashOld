using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using GDMultiStash.Common;
using GDMultiStash.Common.Objects;

namespace GDMultiStash.Forms
{
    internal partial class MainForm : BaseForm
    {
        private Plexiglass.DockingPlexiglass _plexiglass = null;

        public MainWindow.DecorationCollection Decorations { get; } = new MainWindow.DecorationCollection();
        public MainWindow.StashesPage StashesPage { get; }
        public MainWindow.StashGroupsPage StashGroupsPage { get; }

        public EventHandler<EventArgs> SpaceClick;

        #region pages

        class PageHolder
        {
            public MainWindow.Page Page { get; }
            public Button Button { get; }

            public PageHolder(MainWindow.Page page, Button button)
            {
                Page = page;
                Button = button;
            }
        }

        private int currentPageIndex = -1;
        //private readonly EventHandler<EventArgs> PageChanged;
        private readonly List<PageHolder> pages = new List<PageHolder>();

        private void InitPage(MainWindow.Page page, Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseDownBackColor = C.PageButtonBackColorActive;
            button.FlatAppearance.MouseOverBackColor = C.PageButtonBackColorActive;
            button.BackColor = C.PageButtonBackColor;
            button.ForeColor = C.InteractiveForeColor;

            page.BackColor = C.PageBackColor;
            page.Dock = DockStyle.Fill;
            page.Visible = false;

            int pageIndex = pages.Count;
            pages.Add(new PageHolder(page, button));
            pagesPaddingPanel.Controls.Add(page);

            button.MouseEnter += delegate { button.ForeColor = C.InteractiveForeColorHighlight; };
            button.MouseLeave += delegate { if (pageIndex != currentPageIndex) button.ForeColor = C.InteractiveForeColor; };
            button.Click += delegate { ShowPage(pageIndex); };
            button.GotFocus += delegate { ClearFocus(); };
        }

        private void ShowPage(int pageIndex)
        {
            if (currentPageIndex != -1)
            {
                pages[currentPageIndex].Button.BackColor = C.PageButtonBackColor;
                pages[currentPageIndex].Button.ForeColor = C.InteractiveForeColor;
                pages[currentPageIndex].Page.Visible = false;
            }
            currentPageIndex = pageIndex;
            pages[currentPageIndex].Button.BackColor = C.PageButtonBackColorActive;
            pages[currentPageIndex].Button.ForeColor = C.InteractiveForeColorHighlight;
            pages[currentPageIndex].Page.Visible = true;

            pages[currentPageIndex].Page.Focus();
            //PageChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region WndProc

        Rectangle BorderTop { get { return new Rectangle(0, 0, ClientSize.Width, C.WindowResizeBorderSize); } }
        Rectangle BorderLeft { get { return new Rectangle(0, 0, C.WindowResizeBorderSize, ClientSize.Height); } }
        Rectangle BorderBottom { get { return new Rectangle(0, ClientSize.Height - C.WindowResizeBorderSize, ClientSize.Width, C.WindowResizeBorderSize); } }
        Rectangle BorderRight { get { return new Rectangle(ClientSize.Width - C.WindowResizeBorderSize, 0, C.WindowResizeBorderSize, ClientSize.Height); } }

        protected override CreateParams CreateParams
        {
            get
            {
                // this is required to be able
                // to minimize/restore the window by click
                // on taskbar item
                CreateParams cp = base.CreateParams;
                cp.Style |= Native.WS.MINIMIZEBOX;
                cp.ClassStyle |= Native.CS_DBLCLKS;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Native.WM_NCLBUTTONDBLCLK) return; // disable doubleclick on titlebar
            base.WndProc(ref m);
            if (m.Msg == C.WM_SHOWME)
            {
                if (!Visible)
                    Show();
                else
                    MessageBox.Show(G.L.AlreadyRunningMessage(), "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else if (m.Msg == 0x84)
            {
                var cursor = PointToClient(Cursor.Position);

                bool tHit = BorderTop.Contains(cursor);
                bool lHit = BorderLeft.Contains(cursor);
                bool rHit = BorderRight.Contains(cursor);
                bool bHit = BorderBottom.Contains(cursor);
                if (bHit && rHit) m.Result = (IntPtr)Native.HT.BOTTOMRIGHT;
                else if (bHit && lHit) m.Result = (IntPtr)Native.HT.BOTTOMLEFT;
                else if (tHit && rHit) m.Result = (IntPtr)Native.HT.TOPRIGHT;
                else if (tHit && lHit) m.Result = (IntPtr)Native.HT.TOPLEFT;
                else if (bHit) m.Result = (IntPtr)Native.HT.BOTTOM;
                else if (rHit) m.Result = (IntPtr)Native.HT.RIGHT;
                else if (lHit) m.Result = (IntPtr)Native.HT.LEFT;
                else if (tHit) m.Result = (IntPtr)Native.HT.TOP;
                else if (cursor.Y < C.WindowCaptionDragHeight)
                {
                    m.Result = (IntPtr)Native.HT.CAPTION;
                    return;
                }

            }

        }

        #endregion

        protected override bool ShowWithoutActivation => G.Configuration.Settings.AutoStartGame;

        #region buttons

        public void InitializeToolStripButton(ToolStripMenuItem item,
            Color backColor, Color backColorHover,
            Color foreColor, Color foreColorHover)
        {
            bool hover = false;
            bool opened = false;
            item.MouseEnter += delegate { hover = true; item.ForeColor = foreColorHover; item.BackColor = backColorHover; };
            item.MouseLeave += delegate { hover = false; if (!opened) item.ForeColor = foreColor; item.BackColor = backColor; };
            item.DropDownOpened += delegate { opened = true; item.ForeColor = foreColorHover; };
            item.DropDownClosed += delegate { opened = false; if (!hover) item.ForeColor = foreColor; };
            item.BackColor = backColor;
            item.ForeColor = foreColor;
        }

        #endregion

        //protected override bool ShowWithoutActivation => true;

        public MainForm() : base()
        {
            InitializeComponent();

            DoubleBuffered = true;
            Icon = Properties.Resources.icon32;
            FormBorderStyle = FormBorderStyle.None;
            SetStyle(ControlStyles.ResizeRedraw, true);
            TopMost = false;




            #region Init Buttons

            captionGameButton.Click += delegate {
                switch (G.Windows.StartGame())
                {
                    case Global.WindowsManager.GameStartResult.AlreadyRunning:
                        Console.AlertWarning(G.L.GameAlreadyRunningMessage());
                        break;
                    case Global.WindowsManager.GameStartResult.Success:
                        break;
                }
            };

            captionTrayButton.Click += delegate { CloseToTray(); };
            captionMinimizeButton.Click += delegate { WindowState = FormWindowState.Minimized; };
            captionCloseButton.Click += delegate { Close(); };








            InitializeToolStripButton(captionFileButton,
                C.CaptionButtonBackColor, C.CaptionButtonBackColorHover,
                C.InteractiveForeColor, C.InteractiveForeColorHighlight
                );

            InitializeToolStripButton(captionHelpButton,
                C.CaptionButtonBackColor, C.CaptionButtonBackColorHover,
                C.InteractiveForeColor, C.InteractiveForeColorHighlight
                );

            captionMenuStrip.Renderer = new Controls.FlatToolStripRenderer();


            #endregion

            BackColor = C.FormBackColor;
            formBackgroundPanel.BackColor = C.FormBackColor;
            BackgroundImage = Properties.Resources.border;
            BackgroundImageLayout = ImageLayout.Stretch;
            titlePanel.BackgroundImage = Properties.Resources.GDMSLogo;
            titlePanel.BackgroundImageLayout = ImageLayout.Zoom;

            formPaddingPanel.BackColor = C.FormBackColor;
            formPaddingPanel.Padding = C.FormPadding;

            pagesPaddingPanel.BackColor = C.PageBackColor;
            pagesPaddingPanel.Padding = C.PagesPadding;

            captionMenuStrip.BackColor = C.FormBackColor;

            AllowDrop = false; // used to drag transfer files into the window

            // unselect items by clicking on empty space
            EventHandler spaceClickhandler = new EventHandler((sender, e) => SpaceClick?.Invoke(sender, e));
            formBackgroundPanel.Click += spaceClickhandler;
            formPaddingPanel.Click += spaceClickhandler;
            pagesPaddingPanel.Click += spaceClickhandler;

            StashesPage = new MainWindow.StashesPage(this);
            InitPage(StashesPage, stashesPageButton);

            StashGroupsPage = new MainWindow.StashGroupsPage(this);
            InitPage(StashGroupsPage, stashGroupsPageButton);

            ShowPage(0);
        }

        protected override void Initialize()
        {
            base.Initialize();

            G.Windows.ExtendCaptionButton(captionGameButton, (Global.Windows.ExtendedButton exButton) => {
                exButton.BackColorHover = Color.FromArgb(0, 102, 77); // TODO: put me inside constants.cs (?)
                exButton.BackColorPressed = Color.FromArgb(0, 135, 102); // TODO: put me inside constants.cs (?)
            });

            G.Windows.ExtendCaptionButton(captionTrayButton, (Global.Windows.ExtendedButton exButton) => {
                exButton.Image = Properties.Resources.buttonTrayGray;
                exButton.ImageHover = Properties.Resources.buttonTrayWhite;
            });

            G.Windows.ExtendCaptionButton(captionMinimizeButton, (Global.Windows.ExtendedButton exButton) => {
                exButton.Image = Properties.Resources.buttonMinimizeGray;
                exButton.ImageHover = Properties.Resources.buttonMinimizeWhite;
            });

            G.Windows.ExtendCaptionCloseButton(captionCloseButton);

        }

        protected override void Localize(Global.LocalizationManager.StringsHolder L)
        {
            Text = C.AppName;

            // form
            captionGameButton.Text = L.StartGameButton();
            captionFileButton.Text = L.FileButton();
            captionHelpButton.Text = L.HelpButton();
            captionImportButton.Text = L.ImportButton();
            captionImportTransferFilesButton.Text = L.TransferFilesButton();
            captionExportButton.Text = L.ExportButton();
            captionExportTransferFilesButton.Text = L.TransferFilesButton();
            captionSettingsButton.Text = L.SettingsButton();
            captionChangelogButton.Text = L.ChangelogButton();
            captionAboutButton.Text = L.AboutButton();
            captionImportCraftingModeButton.Text = L.CraftingModeButton();

            // pages
            StashesPage.Localize();
            stashesPageButton.Text = L.StashesButton();
            StashGroupsPage.Localize();
            stashGroupsPageButton.Text = L.GroupsButton();
        }

        #region public methods

        public void ClearFocus()
        {
            titlePanel.Focus();
        }

        public void CloseToTray()
        {
            Hide();
            _plexiglass.Hide();
        }

        #endregion

        #region form events

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // fix: objectlistview is causing huge lags when form is in background
            _plexiglass = new Plexiglass.DockingPlexiglass(formPaddingPanel)
            {
                Color = C.FormBackColor,
                Opacity = 0.50,
                Margin = new Padding(0, 0, 0, 0)
            };
            // only after cfg has been initialized!
            Width = G.Configuration.Settings.WindowWidth;
            Height = G.Configuration.Settings.WindowHeight;
            StartPosition = FormStartPosition.Manual;
            Left = G.Configuration.Settings.WindowX;
            Top = G.Configuration.Settings.WindowY;

            switch (G.Configuration.Settings.StartPositionType)
            {
                case 0:
                    break;
                case 1:
                    WindowState = FormWindowState.Minimized;
                    break;
                case 2:
                    bool isInitialShowing = true;
                    Shown += (o, ee) => {
                        if (isInitialShowing)
                        {
                            isInitialShowing = false;
                            CloseToTray();
                        }
                    };
                    break;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (Program.IsQuitting)
            {
                return;
            }
            bool gameOpened = Native.FindWindow("Grim Dawn", null) != IntPtr.Zero;
            if (gameOpened && G.Configuration.Settings.ConfirmClosing)
            {
                DialogResult result = MessageBox.Show(G.L.ConfirmClosingMessage(), "", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                e.Cancel = (result == DialogResult.Cancel);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Program.Quit();
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            G.Configuration.Settings.WindowWidth = Width;
            G.Configuration.Settings.WindowHeight = Height;
            G.Configuration.Settings.WindowX = Left;
            G.Configuration.Settings.WindowY = Top;
            G.Configuration.Save();
        }

        #endregion

        #region controls events

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            G.Windows.ShowConfigurationWindow();
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            G.Windows.ShowAboutDialog();
        }

        private void ChangelogButton_Click(object sender, EventArgs e)
        {
            G.Windows.ShowChangelogWindow();
        }

        private void ImportTransferFilesButton_Click(object sender, EventArgs e)
        {
            G.Windows.ShowImportDialog();
        }

        private void ImportGDSCButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog()
            {
                Filter = $"gd_conf|gd_conf.xml",
                Multiselect = false,
            })
            {
                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.OK) return;

                var importedStashes = new List<StashObject>();
                string stashesPath = System.IO.Path.Combine(new System.IO.FileInfo(dialog.FileName).Directory.FullName, "Stashes");

                var reItem = new System.Text.RegularExpressions.Regex(@"<item\s+([^>]+)>");
                var reID = new System.Text.RegularExpressions.Regex(@"id=""(\d+)""", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                var reName = new System.Text.RegularExpressions.Regex(@"name=""([^""]+)""");
                var reColor = new System.Text.RegularExpressions.Regex(@"tcolor=""0x([a-f0-9]{6})""");
                var matches = reItem.Matches(System.IO.File.ReadAllText(dialog.FileName));
                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    var inner = match.Groups[1].Value;
                    var mID = reID.Match(inner);
                    var mName = reName.Match(inner);
                    var mColor = reColor.Match(inner);
                    if (!mID.Success) continue;
                    if (!mName.Success) continue;

                    var stashID = mID.Groups[1].Value;
                    string stashFile = System.IO.Directory.GetFiles(System.IO.Path.Combine(stashesPath, stashID))
                        .FirstOrDefault(name => true);
                    if (stashFile == null) continue;

                    if (!TransferFile.FromFile(stashFile, out TransferFile transferFile)) continue;
                    if (transferFile.Expansion == GrimDawnLib.GrimDawnGameExpansion.Unknown) continue;
                    if (transferFile.TotalUsage == 0) continue; // no items inside

                    var stashName = System.Web.HttpUtility.HtmlDecode(mName.Groups[1].Value.Trim());
                    StashObject stash = G.Stashes.CreateAndImportStash(stashFile, stashName, transferFile.Expansion, GrimDawnLib.GrimDawnGameMode.Both);
                    if (stash == null) continue;
                    importedStashes.Add(stash);

                    if (mColor.Success)
                        stash.TextColor = $"#{mColor.Groups[1].Value}";
                }

                if (importedStashes.Count != 0)
                {
                    var group = G.StashGroups.CreateGroup("GDSC", true);
                    foreach(var s in importedStashes)
                        s.GroupID = group.ID;
                    G.Configuration.Save();
                    G.Stashes.InvokeStashesAdded(importedStashes);
                    G.StashGroups.InvokeStashGroupsAdded(group);
                }
            }
        }

        private void CaptionImportCraftingModeButton_Click(object sender, EventArgs e)
        {
            G.Windows.ShowCraftingModeDialog();
        }

        private void TopMenuExportTransferFilesButton_Click(object sender, EventArgs e)
        {

            // TODO: redundant code in right click context -> export selected stashes

            StashesZipFile zipFile = new StashesZipFile();
            foreach (StashObject selStash in G.Stashes.GetAllStashes())
                zipFile.AddStash(selStash);

            using (var dialog = new SaveFileDialog()
            {
                Filter = $"{G.L.ZipArchive()}|*.zip",
                FileName = "TransferFiles.zip",
            })
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    zipFile.SaveTo(dialog.FileName);
                }
            }
        }

        #endregion

    }
}
