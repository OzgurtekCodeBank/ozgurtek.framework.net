
namespace ozgurtek.framework.converter.winforms
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.fileTabPage = new System.Windows.Forms.TabPage();
            this.fileUserControl = new ozgurtek.framework.converter.winforms.FileUserControl();
            this.dbTabPage = new System.Windows.Forms.TabPage();
            this.dbUserControl = new ozgurtek.framework.converter.winforms.DbUserControl();
            this.tabControl.SuspendLayout();
            this.fileTabPage.SuspendLayout();
            this.dbTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.fileTabPage);
            this.tabControl.Controls.Add(this.dbTabPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(777, 657);
            this.tabControl.TabIndex = 0;
            // 
            // fileTabPage
            // 
            this.fileTabPage.Controls.Add(this.fileUserControl);
            this.fileTabPage.Location = new System.Drawing.Point(4, 22);
            this.fileTabPage.Name = "fileTabPage";
            this.fileTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.fileTabPage.Size = new System.Drawing.Size(769, 569);
            this.fileTabPage.TabIndex = 0;
            this.fileTabPage.Text = "File";
            this.fileTabPage.UseVisualStyleBackColor = true;
            // 
            // fileUserControl
            // 
            this.fileUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileUserControl.Location = new System.Drawing.Point(3, 3);
            this.fileUserControl.Name = "fileUserControl";
            this.fileUserControl.Size = new System.Drawing.Size(763, 563);
            this.fileUserControl.TabIndex = 0;
            // 
            // dbTabPage
            // 
            this.dbTabPage.Controls.Add(this.dbUserControl);
            this.dbTabPage.Location = new System.Drawing.Point(4, 22);
            this.dbTabPage.Name = "dbTabPage";
            this.dbTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.dbTabPage.Size = new System.Drawing.Size(769, 631);
            this.dbTabPage.TabIndex = 1;
            this.dbTabPage.Text = "Database";
            this.dbTabPage.UseVisualStyleBackColor = true;
            // 
            // dbUserControl
            // 
            this.dbUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dbUserControl.Location = new System.Drawing.Point(3, 3);
            this.dbUserControl.Name = "dbUserControl";
            this.dbUserControl.Size = new System.Drawing.Size(763, 625);
            this.dbUserControl.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 657);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Otek Model Creator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl.ResumeLayout(false);
            this.fileTabPage.ResumeLayout(false);
            this.dbTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage fileTabPage;
        private System.Windows.Forms.TabPage dbTabPage;
        private FileUserControl fileUserControl;
        private DbUserControl dbUserControl;
    }
}

