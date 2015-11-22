namespace TRoschinsky.Lib.HomeMaticXmlApi
{
    partial class FormMain
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.treeView = new System.Windows.Forms.TreeView();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtConnect = new System.Windows.Forms.TextBox();
            this.btnUpdateRequest = new System.Windows.Forms.Button();
            this.btnMessagesClear = new System.Windows.Forms.Button();
            this.checkBoxFastUpdateDevices = new System.Windows.Forms.CheckBox();
            this.listBoxFastUpdateDevices = new System.Windows.Forms.ListBox();
            this.groupBoxConnect = new System.Windows.Forms.GroupBox();
            this.groupBoxOps = new System.Windows.Forms.GroupBox();
            this.btnAddFastUpdateDevice = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusSpacer = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.linkLabelGitHub = new System.Windows.Forms.LinkLabel();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
            this.groupBoxConnect.SuspendLayout();
            this.groupBoxOps.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(238, 181);
            this.treeView.TabIndex = 0;
            this.treeView.DoubleClick += new System.EventHandler(this.treeView_DoubleClick);
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Enabled = false;
            this.btnConnect.Location = new System.Drawing.Point(409, 17);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(144, 23);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtConnect
            // 
            this.txtConnect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConnect.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtConnect.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txtConnect.Location = new System.Drawing.Point(6, 19);
            this.txtConnect.Name = "txtConnect";
            this.txtConnect.Size = new System.Drawing.Size(387, 20);
            this.txtConnect.TabIndex = 2;
            this.txtConnect.Enter += new System.EventHandler(this.txtConnect_Enter);
            this.txtConnect.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtConnect_KeyUp);
            this.txtConnect.Leave += new System.EventHandler(this.txtConnect_Leave);
            // 
            // btnUpdateRequest
            // 
            this.btnUpdateRequest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateRequest.Enabled = false;
            this.btnUpdateRequest.Location = new System.Drawing.Point(10, 117);
            this.btnUpdateRequest.Name = "btnUpdateRequest";
            this.btnUpdateRequest.Size = new System.Drawing.Size(144, 23);
            this.btnUpdateRequest.TabIndex = 3;
            this.btnUpdateRequest.Text = "Update";
            this.btnUpdateRequest.UseVisualStyleBackColor = true;
            this.btnUpdateRequest.Click += new System.EventHandler(this.btnUpdateRequest_Click);
            // 
            // btnMessagesClear
            // 
            this.btnMessagesClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMessagesClear.Enabled = false;
            this.btnMessagesClear.Location = new System.Drawing.Point(10, 146);
            this.btnMessagesClear.Name = "btnMessagesClear";
            this.btnMessagesClear.Size = new System.Drawing.Size(144, 23);
            this.btnMessagesClear.TabIndex = 3;
            this.btnMessagesClear.Text = "Clear messages";
            this.btnMessagesClear.UseVisualStyleBackColor = true;
            this.btnMessagesClear.Click += new System.EventHandler(this.btnMessagesClear_Click);
            // 
            // checkBoxFastUpdateDevices
            // 
            this.checkBoxFastUpdateDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxFastUpdateDevices.AutoSize = true;
            this.checkBoxFastUpdateDevices.Enabled = false;
            this.checkBoxFastUpdateDevices.Location = new System.Drawing.Point(10, 19);
            this.checkBoxFastUpdateDevices.Name = "checkBoxFastUpdateDevices";
            this.checkBoxFastUpdateDevices.Size = new System.Drawing.Size(144, 17);
            this.checkBoxFastUpdateDevices.TabIndex = 6;
            this.checkBoxFastUpdateDevices.Text = "Only FastUpdateDevices";
            this.checkBoxFastUpdateDevices.UseVisualStyleBackColor = true;
            // 
            // listBoxFastUpdateDevices
            // 
            this.listBoxFastUpdateDevices.Enabled = false;
            this.listBoxFastUpdateDevices.FormattingEnabled = true;
            this.listBoxFastUpdateDevices.Location = new System.Drawing.Point(10, 42);
            this.listBoxFastUpdateDevices.Name = "listBoxFastUpdateDevices";
            this.listBoxFastUpdateDevices.Size = new System.Drawing.Size(93, 69);
            this.listBoxFastUpdateDevices.TabIndex = 7;
            // 
            // groupBoxConnect
            // 
            this.groupBoxConnect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxConnect.Controls.Add(this.txtConnect);
            this.groupBoxConnect.Controls.Add(this.btnConnect);
            this.groupBoxConnect.Location = new System.Drawing.Point(13, 13);
            this.groupBoxConnect.Name = "groupBoxConnect";
            this.groupBoxConnect.Size = new System.Drawing.Size(559, 52);
            this.groupBoxConnect.TabIndex = 8;
            this.groupBoxConnect.TabStop = false;
            this.groupBoxConnect.Text = "Connect to Homematic CCU2 (http://yourHmHost)";
            // 
            // groupBoxOps
            // 
            this.groupBoxOps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxOps.Controls.Add(this.btnAddFastUpdateDevice);
            this.groupBoxOps.Controls.Add(this.checkBoxFastUpdateDevices);
            this.groupBoxOps.Controls.Add(this.listBoxFastUpdateDevices);
            this.groupBoxOps.Controls.Add(this.btnMessagesClear);
            this.groupBoxOps.Controls.Add(this.btnUpdateRequest);
            this.groupBoxOps.Location = new System.Drawing.Point(412, 71);
            this.groupBoxOps.Name = "groupBoxOps";
            this.groupBoxOps.Size = new System.Drawing.Size(160, 182);
            this.groupBoxOps.TabIndex = 9;
            this.groupBoxOps.TabStop = false;
            this.groupBoxOps.Text = "Operations";
            // 
            // btnAddFastUpdateDevice
            // 
            this.btnAddFastUpdateDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddFastUpdateDevice.Enabled = false;
            this.btnAddFastUpdateDevice.Location = new System.Drawing.Point(110, 43);
            this.btnAddFastUpdateDevice.Name = "btnAddFastUpdateDevice";
            this.btnAddFastUpdateDevice.Size = new System.Drawing.Size(44, 68);
            this.btnAddFastUpdateDevice.TabIndex = 8;
            this.btnAddFastUpdateDevice.Text = "Add";
            this.btnAddFastUpdateDevice.UseVisualStyleBackColor = true;
            this.btnAddFastUpdateDevice.Click += new System.EventHandler(this.btnAddFastUpdateDevice_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusSpacer,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 279);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(584, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(48, 17);
            this.toolStripStatusLabel1.Text = "Ready...";
            // 
            // toolStripStatusSpacer
            // 
            this.toolStripStatusSpacer.AutoSize = false;
            this.toolStripStatusSpacer.Name = "toolStripStatusSpacer";
            this.toolStripStatusSpacer.Size = new System.Drawing.Size(17, 17);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // linkLabelGitHub
            // 
            this.linkLabelGitHub.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabelGitHub.AutoSize = true;
            this.linkLabelGitHub.Location = new System.Drawing.Point(9, 256);
            this.linkLabelGitHub.Name = "linkLabelGitHub";
            this.linkLabelGitHub.Size = new System.Drawing.Size(261, 13);
            this.linkLabelGitHub.TabIndex = 1;
            this.linkLabelGitHub.TabStop = true;
            this.linkLabelGitHub.Text = "https://github.com/roschinsky/HomeMatic-XmlApi-Lib";
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerMain.Location = new System.Drawing.Point(13, 72);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.treeView);
            this.splitContainerMain.Panel1MinSize = 100;
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.richTextBoxLog);
            this.splitContainerMain.Panel2MinSize = 0;
            this.splitContainerMain.Size = new System.Drawing.Size(393, 181);
            this.splitContainerMain.SplitterDistance = 238;
            this.splitContainerMain.TabIndex = 12;
            // 
            // richTextBoxLog
            // 
            this.richTextBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxLog.Font = new System.Drawing.Font("Courier New", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxLog.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxLog.Name = "richTextBoxLog";
            this.richTextBoxLog.ReadOnly = true;
            this.richTextBoxLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxLog.Size = new System.Drawing.Size(151, 181);
            this.richTextBoxLog.TabIndex = 0;
            this.richTextBoxLog.Text = "";
            this.richTextBoxLog.WordWrap = false;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 301);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.linkLabelGitHub);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBoxOps);
            this.Controls.Add(this.groupBoxConnect);
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "FormMain";
            this.Text = "LIB_HomeMaticXmlApi - Tester";
            this.groupBoxConnect.ResumeLayout(false);
            this.groupBoxConnect.PerformLayout();
            this.groupBoxOps.ResumeLayout(false);
            this.groupBoxOps.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtConnect;
        private System.Windows.Forms.Button btnUpdateRequest;
        private System.Windows.Forms.Button btnMessagesClear;
        private System.Windows.Forms.CheckBox checkBoxFastUpdateDevices;
        private System.Windows.Forms.ListBox listBoxFastUpdateDevices;
        private System.Windows.Forms.GroupBox groupBoxConnect;
        private System.Windows.Forms.GroupBox groupBoxOps;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.LinkLabel linkLabelGitHub;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.RichTextBox richTextBoxLog;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusSpacer;
        private System.Windows.Forms.Button btnAddFastUpdateDevice;
    }
}

