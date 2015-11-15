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
            this.linkLabelGitHub = new System.Windows.Forms.LinkLabel();
            this.btnMessagesClear = new System.Windows.Forms.Button();
            this.checkBoxFastUpdateDevices = new System.Windows.Forms.CheckBox();
            this.listBoxFastUpdateDevices = new System.Windows.Forms.ListBox();
            this.groupBoxConnect = new System.Windows.Forms.GroupBox();
            this.groupBoxOps = new System.Windows.Forms.GroupBox();
            this.groupBoxConnect.SuspendLayout();
            this.groupBoxOps.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.Location = new System.Drawing.Point(12, 71);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(394, 206);
            this.treeView.TabIndex = 0;
            this.treeView.DoubleClick += new System.EventHandler(this.treeView_DoubleClick);
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(409, 17);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(144, 23);
            this.btnConnect.TabIndex = 2;
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
            this.txtConnect.TabIndex = 1;
            this.txtConnect.Text = "Enter URL to HomeMatic CCU 2 with XmlApi Add-On installed...";
            this.txtConnect.Enter += new System.EventHandler(this.txtConnect_Enter);
            this.txtConnect.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtConnect_KeyUp);
            // 
            // btnUpdateRequest
            // 
            this.btnUpdateRequest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateRequest.Location = new System.Drawing.Point(10, 143);
            this.btnUpdateRequest.Name = "btnUpdateRequest";
            this.btnUpdateRequest.Size = new System.Drawing.Size(144, 23);
            this.btnUpdateRequest.TabIndex = 3;
            this.btnUpdateRequest.Text = "Update";
            this.btnUpdateRequest.UseVisualStyleBackColor = true;
            this.btnUpdateRequest.Click += new System.EventHandler(this.btnRequestHighPrioUpdate_Click);
            // 
            // linkLabelGitHub
            // 
            this.linkLabelGitHub.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabelGitHub.AutoSize = true;
            this.linkLabelGitHub.Location = new System.Drawing.Point(12, 291);
            this.linkLabelGitHub.Name = "linkLabelGitHub";
            this.linkLabelGitHub.Size = new System.Drawing.Size(261, 13);
            this.linkLabelGitHub.TabIndex = 5;
            this.linkLabelGitHub.TabStop = true;
            this.linkLabelGitHub.Text = "https://github.com/roschinsky/HomeMatic-XmlApi-Lib";
            this.linkLabelGitHub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGitHub_LinkClicked);
            // 
            // btnMessagesClear
            // 
            this.btnMessagesClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMessagesClear.Location = new System.Drawing.Point(10, 172);
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
            this.checkBoxFastUpdateDevices.Location = new System.Drawing.Point(10, 19);
            this.checkBoxFastUpdateDevices.Name = "checkBoxFastUpdateDevices";
            this.checkBoxFastUpdateDevices.Size = new System.Drawing.Size(144, 17);
            this.checkBoxFastUpdateDevices.TabIndex = 6;
            this.checkBoxFastUpdateDevices.Text = "Only FastUpdateDevices";
            this.checkBoxFastUpdateDevices.UseVisualStyleBackColor = true;
            // 
            // listBoxFastUpdateDevices
            // 
            this.listBoxFastUpdateDevices.FormattingEnabled = true;
            this.listBoxFastUpdateDevices.Location = new System.Drawing.Point(10, 42);
            this.listBoxFastUpdateDevices.Name = "listBoxFastUpdateDevices";
            this.listBoxFastUpdateDevices.Size = new System.Drawing.Size(144, 95);
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
            this.groupBoxConnect.Text = "Connect to Homematic CCU2";
            // 
            // groupBoxOps
            // 
            this.groupBoxOps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxOps.Controls.Add(this.checkBoxFastUpdateDevices);
            this.groupBoxOps.Controls.Add(this.listBoxFastUpdateDevices);
            this.groupBoxOps.Controls.Add(this.btnMessagesClear);
            this.groupBoxOps.Controls.Add(this.btnUpdateRequest);
            this.groupBoxOps.Location = new System.Drawing.Point(412, 71);
            this.groupBoxOps.Name = "groupBoxOps";
            this.groupBoxOps.Size = new System.Drawing.Size(160, 206);
            this.groupBoxOps.TabIndex = 9;
            this.groupBoxOps.TabStop = false;
            this.groupBoxOps.Text = "Operations";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 313);
            this.Controls.Add(this.groupBoxOps);
            this.Controls.Add(this.groupBoxConnect);
            this.Controls.Add(this.linkLabelGitHub);
            this.Controls.Add(this.treeView);
            this.Name = "FormMain";
            this.Text = "LIB_HomeMaticXmlApi - Tester";
            this.groupBoxConnect.ResumeLayout(false);
            this.groupBoxConnect.PerformLayout();
            this.groupBoxOps.ResumeLayout(false);
            this.groupBoxOps.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtConnect;
        private System.Windows.Forms.Button btnUpdateRequest;
        private System.Windows.Forms.LinkLabel linkLabelGitHub;
        private System.Windows.Forms.Button btnMessagesClear;
        private System.Windows.Forms.CheckBox checkBoxFastUpdateDevices;
        private System.Windows.Forms.ListBox listBoxFastUpdateDevices;
        private System.Windows.Forms.GroupBox groupBoxConnect;
        private System.Windows.Forms.GroupBox groupBoxOps;
    }
}

