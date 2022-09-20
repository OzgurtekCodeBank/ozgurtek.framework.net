
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
            this.label1 = new System.Windows.Forms.Label();
            this.ExportButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.outputUserControl = new ozgurtek.framework.converter.winforms.OutputUserControl();
            this.DatabaseFrame.SuspendLayout();
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
            this.ConnectionStringText.Size = new System.Drawing.Size(657, 43);
            this.ConnectionStringText.TabIndex = 0;
            // 
            // QueryTextBox
            // 
            this.QueryTextBox.Location = new System.Drawing.Point(31, 107);
            this.QueryTextBox.Multiline = true;
            this.QueryTextBox.Name = "QueryTextBox";
            this.QueryTextBox.Size = new System.Drawing.Size(657, 116);
            this.QueryTextBox.TabIndex = 1;
            // 
            // DatabaseFrame
            // 
            this.DatabaseFrame.Controls.Add(this.ConnectionStringText);
            this.DatabaseFrame.Controls.Add(this.QueryTextBox);
            this.DatabaseFrame.Controls.Add(this.label1);
            this.DatabaseFrame.Controls.Add(this.ConnectionStringLabel);
            this.DatabaseFrame.Location = new System.Drawing.Point(25, 13);
            this.DatabaseFrame.Name = "DatabaseFrame";
            this.DatabaseFrame.Size = new System.Drawing.Size(707, 245);
            this.DatabaseFrame.TabIndex = 5;
            this.DatabaseFrame.TabStop = false;
            this.DatabaseFrame.Text = "Database";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Query";
            // 
            // ExportButton
            // 
            this.ExportButton.Location = new System.Drawing.Point(305, 531);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(158, 25);
            this.ExportButton.TabIndex = 1;
            this.ExportButton.Text = "Export";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(17, 567);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(739, 23);
            this.progressBar.TabIndex = 8;
            // 
            // outputUserControl
            // 
            this.outputUserControl.Location = new System.Drawing.Point(16, 258);
            this.outputUserControl.Name = "outputUserControl";
            this.outputUserControl.RegisteryPrefix = null;
            this.outputUserControl.Size = new System.Drawing.Size(732, 262);
            this.outputUserControl.TabIndex = 0;
            // 
            // DbUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.outputUserControl);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.DatabaseFrame);
            this.Name = "DbUserControl";
            this.Size = new System.Drawing.Size(770, 600);
            this.DatabaseFrame.ResumeLayout(false);
            this.DatabaseFrame.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label ConnectionStringLabel;
        private System.Windows.Forms.TextBox ConnectionStringText;
        private System.Windows.Forms.TextBox QueryTextBox;
        private System.Windows.Forms.GroupBox DatabaseFrame;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar;
        private OutputUserControl outputUserControl;
    }
}
