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
            this.btnRequestHighPrioUpdate = new System.Windows.Forms.Button();
            this.linkLabelGitHub = new System.Windows.Forms.LinkLabel();
            this.btnMessagesClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.Location = new System.Drawing.Point(13, 42);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(322, 180);
            this.treeView.TabIndex = 0;
            this.treeView.DoubleClick += new System.EventHandler(this.treeView_DoubleClick);
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(341, 13);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
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
            this.txtConnect.Location = new System.Drawing.Point(13, 15);
            this.txtConnect.Name = "txtConnect";
            this.txtConnect.Size = new System.Drawing.Size(322, 20);
            this.txtConnect.TabIndex = 1;
            this.txtConnect.Text = "Enter URL to HomeMatic CCU 2 with XmlApi Add-On installed...";
            this.txtConnect.Enter += new System.EventHandler(this.txtConnect_Enter);
            this.txtConnect.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtConnect_KeyUp);
            // 
            // btnRequestHighPrioUpdate
            // 
            this.btnRequestHighPrioUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRequestHighPrioUpdate.Location = new System.Drawing.Point(341, 42);
            this.btnRequestHighPrioUpdate.Name = "btnRequestHighPrioUpdate";
            this.btnRequestHighPrioUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnRequestHighPrioUpdate.TabIndex = 3;
            this.btnRequestHighPrioUpdate.Text = "Update";
            this.btnRequestHighPrioUpdate.UseVisualStyleBackColor = true;
            this.btnRequestHighPrioUpdate.Click += new System.EventHandler(this.btnRequestHighPrioUpdate_Click);
            // 
            // linkLabelGitHub
            // 
            this.linkLabelGitHub.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabelGitHub.AutoSize = true;
            this.linkLabelGitHub.Location = new System.Drawing.Point(12, 233);
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
            this.btnMessagesClear.Location = new System.Drawing.Point(341, 71);
            this.btnMessagesClear.Name = "btnMessagesClear";
            this.btnMessagesClear.Size = new System.Drawing.Size(75, 39);
            this.btnMessagesClear.TabIndex = 3;
            this.btnMessagesClear.Text = "Clear messages";
            this.btnMessagesClear.UseVisualStyleBackColor = true;
            this.btnMessagesClear.Click += new System.EventHandler(this.btnMessagesClear_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 255);
            this.Controls.Add(this.linkLabelGitHub);
            this.Controls.Add(this.btnMessagesClear);
            this.Controls.Add(this.btnRequestHighPrioUpdate);
            this.Controls.Add(this.txtConnect);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.treeView);
            this.Name = "FormMain";
            this.Text = "LIB_HomeMaticXmlApi - Tester";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtConnect;
        private System.Windows.Forms.Button btnRequestHighPrioUpdate;
        private System.Windows.Forms.LinkLabel linkLabelGitHub;
        private System.Windows.Forms.Button btnMessagesClear;
    }
}

