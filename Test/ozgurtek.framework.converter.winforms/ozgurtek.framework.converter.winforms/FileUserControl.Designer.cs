
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.ExportButton = new System.Windows.Forms.Button();
            this.outputUserControl = new ozgurtek.framework.converter.winforms.OutputUserControl();
            this.DatabaseFrame.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConnectionStringText
            // 
            this.ConnectionStringText.Location = new System.Drawing.Point(45, 44);
            this.ConnectionStringText.Multiline = true;
            this.ConnectionStringText.Name = "ConnectionStringText";
            this.ConnectionStringText.Size = new System.Drawing.Size(666, 26);
            this.ConnectionStringText.TabIndex = 0;
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
            this.DatabaseFrame.Location = new System.Drawing.Point(14, -1);
            this.DatabaseFrame.Name = "DatabaseFrame";
            this.DatabaseFrame.Size = new System.Drawing.Size(718, 106);
            this.DatabaseFrame.TabIndex = 10;
            this.DatabaseFrame.TabStop = false;
            // 
            // FileButton
            // 
            this.FileButton.Location = new System.Drawing.Point(15, 47);
            this.FileButton.Name = "FileButton";
            this.FileButton.Size = new System.Drawing.Size(23, 23);
            this.FileButton.TabIndex = 2;
            this.FileButton.Text = "...";
            this.FileButton.UseVisualStyleBackColor = true;
            this.FileButton.Click += new System.EventHandler(this.FileButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(8, 498);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(739, 23);
            this.progressBar.TabIndex = 12;
            // 
            // ExportButton
            // 
            this.ExportButton.Location = new System.Drawing.Point(292, 467);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(158, 25);
            this.ExportButton.TabIndex = 1;
            this.ExportButton.Text = "Export";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // outputUserControl
            // 
            this.outputUserControl.Location = new System.Drawing.Point(8, 117);
            this.outputUserControl.Name = "outputUserControl";
            this.outputUserControl.RegisteryPrefix = null;
            this.outputUserControl.Size = new System.Drawing.Size(732, 269);
            this.outputUserControl.TabIndex = 0;
            // 
            // FileUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.outputUserControl);
            this.Controls.Add(this.DatabaseFrame);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.ExportButton);
            this.Name = "FileUserControl";
            this.Size = new System.Drawing.Size(765, 539);
            this.DatabaseFrame.ResumeLayout(false);
            this.DatabaseFrame.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox ConnectionStringText;
        private System.Windows.Forms.Label ConnectionStringLabel;
        private System.Windows.Forms.GroupBox DatabaseFrame;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.Button FileButton;
        private OutputUserControl outputUserControl;
    }
}
