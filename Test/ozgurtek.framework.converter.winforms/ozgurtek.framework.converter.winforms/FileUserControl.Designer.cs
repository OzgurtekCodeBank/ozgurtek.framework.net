
namespace ozgurtek.framework.converter.winforms
{
    partial class FileUserControl
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
            this.ConnectionStringText = new System.Windows.Forms.TextBox();
            this.ConnectionStringLabel = new System.Windows.Forms.Label();
            this.DatabaseFrame = new System.Windows.Forms.GroupBox();
            this.FileButton = new System.Windows.Forms.Button();
            this.OutputGroupBox = new System.Windows.Forms.GroupBox();
            this.SuppressBlankTileCheck = new System.Windows.Forms.CheckBox();
            this.folderButton = new System.Windows.Forms.Button();
            this.OutPutFolderTextBox = new System.Windows.Forms.TextBox();
            this.EpsgTextBox = new System.Windows.Forms.TextBox();
            this.XyTileCountTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.OutputFolderLabel = new System.Windows.Forms.Label();
            this.EntityPerPageLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.ExportButton = new System.Windows.Forms.Button();
            this.DatabaseFrame.SuspendLayout();
            this.OutputGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConnectionStringText
            // 
            this.ConnectionStringText.Location = new System.Drawing.Point(32, 44);
            this.ConnectionStringText.Multiline = true;
            this.ConnectionStringText.Name = "ConnectionStringText";
            this.ConnectionStringText.Size = new System.Drawing.Size(686, 26);
            this.ConnectionStringText.TabIndex = 1;
            // 
            // ConnectionStringLabel
            // 
            this.ConnectionStringLabel.AutoSize = true;
            this.ConnectionStringLabel.Location = new System.Drawing.Point(33, 23);
            this.ConnectionStringLabel.Name = "ConnectionStringLabel";
            this.ConnectionStringLabel.Size = new System.Drawing.Size(23, 13);
            this.ConnectionStringLabel.TabIndex = 0;
            this.ConnectionStringLabel.Text = "File";
            // 
            // DatabaseFrame
            // 
            this.DatabaseFrame.Controls.Add(this.ConnectionStringText);
            this.DatabaseFrame.Controls.Add(this.FileButton);
            this.DatabaseFrame.Controls.Add(this.ConnectionStringLabel);
            this.DatabaseFrame.Location = new System.Drawing.Point(6, -1);
            this.DatabaseFrame.Name = "DatabaseFrame";
            this.DatabaseFrame.Size = new System.Drawing.Size(740, 106);
            this.DatabaseFrame.TabIndex = 10;
            this.DatabaseFrame.TabStop = false;
            // 
            // FileButton
            // 
            this.FileButton.Location = new System.Drawing.Point(4, 47);
            this.FileButton.Name = "FileButton";
            this.FileButton.Size = new System.Drawing.Size(23, 23);
            this.FileButton.TabIndex = 2;
            this.FileButton.Text = "...";
            this.FileButton.UseVisualStyleBackColor = true;
            this.FileButton.Click += new System.EventHandler(this.FileButton_Click);
            // 
            // OutputGroupBox
            // 
            this.OutputGroupBox.Controls.Add(this.SuppressBlankTileCheck);
            this.OutputGroupBox.Controls.Add(this.folderButton);
            this.OutputGroupBox.Controls.Add(this.OutPutFolderTextBox);
            this.OutputGroupBox.Controls.Add(this.EpsgTextBox);
            this.OutputGroupBox.Controls.Add(this.XyTileCountTextBox);
            this.OutputGroupBox.Controls.Add(this.label2);
            this.OutputGroupBox.Controls.Add(this.OutputFolderLabel);
            this.OutputGroupBox.Controls.Add(this.EntityPerPageLabel);
            this.OutputGroupBox.Location = new System.Drawing.Point(3, 114);
            this.OutputGroupBox.Name = "OutputGroupBox";
            this.OutputGroupBox.Size = new System.Drawing.Size(739, 184);
            this.OutputGroupBox.TabIndex = 11;
            this.OutputGroupBox.TabStop = false;
            this.OutputGroupBox.Text = "Output";
            // 
            // SuppressBlankTileCheck
            // 
            this.SuppressBlankTileCheck.AutoSize = true;
            this.SuppressBlankTileCheck.Checked = true;
            this.SuppressBlankTileCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SuppressBlankTileCheck.Location = new System.Drawing.Point(597, 161);
            this.SuppressBlankTileCheck.Name = "SuppressBlankTileCheck";
            this.SuppressBlankTileCheck.Size = new System.Drawing.Size(120, 17);
            this.SuppressBlankTileCheck.TabIndex = 3;
            this.SuppressBlankTileCheck.Text = "Suppress Blank Tile";
            this.SuppressBlankTileCheck.UseVisualStyleBackColor = true;
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
            this.OutPutFolderTextBox.Size = new System.Drawing.Size(617, 23);
            this.OutPutFolderTextBox.TabIndex = 1;
            // 
            // EpsgTextBox
            // 
            this.EpsgTextBox.Location = new System.Drawing.Point(101, 71);
            this.EpsgTextBox.Name = "EpsgTextBox";
            this.EpsgTextBox.Size = new System.Drawing.Size(617, 20);
            this.EpsgTextBox.TabIndex = 1;
            // 
            // XyTileCountTextBox
            // 
            this.XyTileCountTextBox.Location = new System.Drawing.Point(100, 29);
            this.XyTileCountTextBox.Name = "XyTileCountTextBox";
            this.XyTileCountTextBox.Size = new System.Drawing.Size(618, 20);
            this.XyTileCountTextBox.TabIndex = 1;
            this.XyTileCountTextBox.Text = "10";
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
            this.EntityPerPageLabel.Size = new System.Drawing.Size(72, 13);
            this.EntityPerPageLabel.TabIndex = 0;
            this.EntityPerPageLabel.Text = "XY Tile Count";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(15, 341);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(739, 23);
            this.progressBar.TabIndex = 12;
            // 
            // ExportButton
            // 
            this.ExportButton.Location = new System.Drawing.Point(299, 310);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(158, 25);
            this.ExportButton.TabIndex = 9;
            this.ExportButton.Text = "Export";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // FileUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.DatabaseFrame);
            this.Controls.Add(this.OutputGroupBox);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.ExportButton);
            this.Name = "FileUserControl";
            this.Size = new System.Drawing.Size(765, 377);
            this.DatabaseFrame.ResumeLayout(false);
            this.DatabaseFrame.PerformLayout();
            this.OutputGroupBox.ResumeLayout(false);
            this.OutputGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox ConnectionStringText;
        private System.Windows.Forms.Label ConnectionStringLabel;
        private System.Windows.Forms.GroupBox DatabaseFrame;
        private System.Windows.Forms.GroupBox OutputGroupBox;
        private System.Windows.Forms.CheckBox SuppressBlankTileCheck;
        private System.Windows.Forms.Button folderButton;
        private System.Windows.Forms.TextBox OutPutFolderTextBox;
        private System.Windows.Forms.TextBox EpsgTextBox;
        private System.Windows.Forms.TextBox XyTileCountTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label OutputFolderLabel;
        private System.Windows.Forms.Label EntityPerPageLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.Button FileButton;
    }
}
