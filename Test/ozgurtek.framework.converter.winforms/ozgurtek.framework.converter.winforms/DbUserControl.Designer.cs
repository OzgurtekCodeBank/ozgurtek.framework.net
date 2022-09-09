
namespace ozgurtek.framework.converter.winforms
{
    partial class DbUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ConnectionStringLabel = new System.Windows.Forms.Label();
            this.ConnectionStringText = new System.Windows.Forms.TextBox();
            this.QueryTextBox = new System.Windows.Forms.TextBox();
            this.DatabaseFrame = new System.Windows.Forms.GroupBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.OutputGroupBox = new System.Windows.Forms.GroupBox();
            this.folderButton = new System.Windows.Forms.Button();
            this.OutPutFolderTextBox = new System.Windows.Forms.TextBox();
            this.EpsgTextBox = new System.Windows.Forms.TextBox();
            this.EntityPerPageTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.OutputFolderLabel = new System.Windows.Forms.Label();
            this.EntityPerPageLabel = new System.Windows.Forms.Label();
            this.ExportButton = new System.Windows.Forms.Button();
            this.DatabaseFrame.SuspendLayout();
            this.OutputGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConnectionStringLabel
            // 
            this.ConnectionStringLabel.AutoSize = true;
            this.ConnectionStringLabel.Location = new System.Drawing.Point(33, 23);
            this.ConnectionStringLabel.Name = "ConnectionStringLabel";
            this.ConnectionStringLabel.Size = new System.Drawing.Size(88, 13);
            this.ConnectionStringLabel.TabIndex = 0;
            this.ConnectionStringLabel.Text = "ConnectionString";
            // 
            // ConnectionStringText
            // 
            this.ConnectionStringText.Location = new System.Drawing.Point(32, 44);
            this.ConnectionStringText.Multiline = true;
            this.ConnectionStringText.Name = "ConnectionStringText";
            this.ConnectionStringText.Size = new System.Drawing.Size(686, 84);
            this.ConnectionStringText.TabIndex = 1;
            // 
            // QueryTextBox
            // 
            this.QueryTextBox.Location = new System.Drawing.Point(32, 161);
            this.QueryTextBox.Multiline = true;
            this.QueryTextBox.Name = "QueryTextBox";
            this.QueryTextBox.Size = new System.Drawing.Size(686, 116);
            this.QueryTextBox.TabIndex = 1;
            // 
            // DatabaseFrame
            // 
            this.DatabaseFrame.Controls.Add(this.ConnectButton);
            this.DatabaseFrame.Controls.Add(this.ConnectionStringText);
            this.DatabaseFrame.Controls.Add(this.QueryTextBox);
            this.DatabaseFrame.Controls.Add(this.label1);
            this.DatabaseFrame.Controls.Add(this.ConnectionStringLabel);
            this.DatabaseFrame.Location = new System.Drawing.Point(16, 13);
            this.DatabaseFrame.Name = "DatabaseFrame";
            this.DatabaseFrame.Size = new System.Drawing.Size(740, 352);
            this.DatabaseFrame.TabIndex = 5;
            this.DatabaseFrame.TabStop = false;
            this.DatabaseFrame.Text = "Database";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(291, 302);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(158, 25);
            this.ConnectButton.TabIndex = 2;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 145);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Query";
            // 
            // OutputGroupBox
            // 
            this.OutputGroupBox.Controls.Add(this.folderButton);
            this.OutputGroupBox.Controls.Add(this.OutPutFolderTextBox);
            this.OutputGroupBox.Controls.Add(this.EpsgTextBox);
            this.OutputGroupBox.Controls.Add(this.EntityPerPageTextBox);
            this.OutputGroupBox.Controls.Add(this.label2);
            this.OutputGroupBox.Controls.Add(this.OutputFolderLabel);
            this.OutputGroupBox.Controls.Add(this.EntityPerPageLabel);
            this.OutputGroupBox.Location = new System.Drawing.Point(16, 371);
            this.OutputGroupBox.Name = "OutputGroupBox";
            this.OutputGroupBox.Size = new System.Drawing.Size(739, 184);
            this.OutputGroupBox.TabIndex = 7;
            this.OutputGroupBox.TabStop = false;
            this.OutputGroupBox.Text = "Output";
            // 
            // folderButton
            // 
            this.folderButton.Location = new System.Drawing.Point(76, 116);
            this.folderButton.Name = "folderButton";
            this.folderButton.Size = new System.Drawing.Size(23, 23);
            this.folderButton.TabIndex = 2;
            this.folderButton.Text = "...";
            this.folderButton.UseVisualStyleBackColor = true;
            this.folderButton.Click += new System.EventHandler(this.folderButton_Click);
            // 
            // OutPutFolderTextBox
            // 
            this.OutPutFolderTextBox.Location = new System.Drawing.Point(101, 116);
            this.OutPutFolderTextBox.Multiline = true;
            this.OutPutFolderTextBox.Name = "OutPutFolderTextBox";
            this.OutPutFolderTextBox.Size = new System.Drawing.Size(617, 50);
            this.OutPutFolderTextBox.TabIndex = 1;
            // 
            // EpsgTextBox
            // 
            this.EpsgTextBox.Location = new System.Drawing.Point(101, 71);
            this.EpsgTextBox.Name = "EpsgTextBox";
            this.EpsgTextBox.Size = new System.Drawing.Size(617, 20);
            this.EpsgTextBox.TabIndex = 1;
            // 
            // EntityPerPageTextBox
            // 
            this.EntityPerPageTextBox.Location = new System.Drawing.Point(100, 29);
            this.EntityPerPageTextBox.Name = "EntityPerPageTextBox";
            this.EntityPerPageTextBox.Size = new System.Drawing.Size(618, 20);
            this.EntityPerPageTextBox.TabIndex = 1;
            this.EntityPerPageTextBox.Text = "10";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Source EPSG";
            // 
            // OutputFolderLabel
            // 
            this.OutputFolderLabel.AutoSize = true;
            this.OutputFolderLabel.Location = new System.Drawing.Point(10, 119);
            this.OutputFolderLabel.Name = "OutputFolderLabel";
            this.OutputFolderLabel.Size = new System.Drawing.Size(68, 13);
            this.OutputFolderLabel.TabIndex = 0;
            this.OutputFolderLabel.Text = "Ouput Folder";
            // 
            // EntityPerPageLabel
            // 
            this.EntityPerPageLabel.AutoSize = true;
            this.EntityPerPageLabel.Location = new System.Drawing.Point(10, 29);
            this.EntityPerPageLabel.Name = "EntityPerPageLabel";
            this.EntityPerPageLabel.Size = new System.Drawing.Size(80, 13);
            this.EntityPerPageLabel.TabIndex = 0;
            this.EntityPerPageLabel.Text = "Entity Per Page";
            // 
            // ExportButton
            // 
            this.ExportButton.Location = new System.Drawing.Point(307, 593);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(158, 25);
            this.ExportButton.TabIndex = 2;
            this.ExportButton.Text = "Export";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // DbUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.OutputGroupBox);
            this.Controls.Add(this.DatabaseFrame);
            this.Name = "DbUserControl";
            this.Size = new System.Drawing.Size(772, 648);
            this.DatabaseFrame.ResumeLayout(false);
            this.DatabaseFrame.PerformLayout();
            this.OutputGroupBox.ResumeLayout(false);
            this.OutputGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label ConnectionStringLabel;
        private System.Windows.Forms.TextBox ConnectionStringText;
        private System.Windows.Forms.TextBox QueryTextBox;
        private System.Windows.Forms.GroupBox DatabaseFrame;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.GroupBox OutputGroupBox;
        private System.Windows.Forms.Label EntityPerPageLabel;
        private System.Windows.Forms.TextBox EntityPerPageTextBox;
        private System.Windows.Forms.Label OutputFolderLabel;
        private System.Windows.Forms.TextBox OutPutFolderTextBox;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button folderButton;
        private System.Windows.Forms.TextBox EpsgTextBox;
        private System.Windows.Forms.Label label2;
    }
}
