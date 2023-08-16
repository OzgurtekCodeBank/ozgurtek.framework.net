
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.fileTabPage = new System.Windows.Forms.TabPage();
            this.fileUserControl = new ozgurtek.framework.converter.winforms.FileUserControl();
            this.dbTabPage = new System.Windows.Forms.TabPage();
            this.dbUserControl = new ozgurtek.framework.converter.winforms.DbUserControl();
            this.appSpecTabPage = new System.Windows.Forms.TabPage();
            this.appSpesific1 = new ozgurtek.framework.converter.winforms.AppSpesific();
            this.aboutTabPage = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.fileTabPage.SuspendLayout();
            this.dbTabPage.SuspendLayout();
            this.appSpecTabPage.SuspendLayout();
            this.aboutTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.fileTabPage);
            this.tabControl.Controls.Add(this.dbTabPage);
            this.tabControl.Controls.Add(this.appSpecTabPage);
            this.tabControl.Controls.Add(this.aboutTabPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(777, 638);
            this.tabControl.TabIndex = 0;
            // 
            // fileTabPage
            // 
            this.fileTabPage.Controls.Add(this.button1);
            this.fileTabPage.Controls.Add(this.fileUserControl);
            this.fileTabPage.Location = new System.Drawing.Point(4, 22);
            this.fileTabPage.Name = "fileTabPage";
            this.fileTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.fileTabPage.Size = new System.Drawing.Size(769, 612);
            this.fileTabPage.TabIndex = 0;
            this.fileTabPage.Text = "File";
            this.fileTabPage.UseVisualStyleBackColor = true;
            // 
            // fileUserControl
            // 
            this.fileUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileUserControl.Location = new System.Drawing.Point(3, 3);
            this.fileUserControl.Name = "fileUserControl";
            this.fileUserControl.Size = new System.Drawing.Size(763, 606);
            this.fileUserControl.TabIndex = 0;
            // 
            // dbTabPage
            // 
            this.dbTabPage.Controls.Add(this.dbUserControl);
            this.dbTabPage.Location = new System.Drawing.Point(4, 22);
            this.dbTabPage.Name = "dbTabPage";
            this.dbTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.dbTabPage.Size = new System.Drawing.Size(769, 612);
            this.dbTabPage.TabIndex = 1;
            this.dbTabPage.Text = "Database";
            this.dbTabPage.UseVisualStyleBackColor = true;
            // 
            // dbUserControl
            // 
            this.dbUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dbUserControl.Location = new System.Drawing.Point(3, 3);
            this.dbUserControl.Name = "dbUserControl";
            this.dbUserControl.Size = new System.Drawing.Size(763, 606);
            this.dbUserControl.TabIndex = 0;
            // 
            // appSpecTabPage
            // 
            this.appSpecTabPage.Controls.Add(this.appSpesific1);
            this.appSpecTabPage.Location = new System.Drawing.Point(4, 22);
            this.appSpecTabPage.Name = "appSpecTabPage";
            this.appSpecTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.appSpecTabPage.Size = new System.Drawing.Size(769, 612);
            this.appSpecTabPage.TabIndex = 3;
            this.appSpecTabPage.Text = "App";
            this.appSpecTabPage.UseVisualStyleBackColor = true;
            // 
            // appSpesific1
            // 
            this.appSpesific1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appSpesific1.Location = new System.Drawing.Point(3, 3);
            this.appSpesific1.Name = "appSpesific1";
            this.appSpesific1.Size = new System.Drawing.Size(763, 606);
            this.appSpesific1.TabIndex = 0;
            // 
            // aboutTabPage
            // 
            this.aboutTabPage.Controls.Add(this.label1);
            this.aboutTabPage.Location = new System.Drawing.Point(4, 22);
            this.aboutTabPage.Name = "aboutTabPage";
            this.aboutTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.aboutTabPage.Size = new System.Drawing.Size(769, 612);
            this.aboutTabPage.TabIndex = 2;
            this.aboutTabPage.Text = "About";
            this.aboutTabPage.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Model Creator 15.09.2022 17:50";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(109, 453);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 638);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Otek 3D Model Creator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl.ResumeLayout(false);
            this.fileTabPage.ResumeLayout(false);
            this.dbTabPage.ResumeLayout(false);
            this.appSpecTabPage.ResumeLayout(false);
            this.aboutTabPage.ResumeLayout(false);
            this.aboutTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage fileTabPage;
        private System.Windows.Forms.TabPage dbTabPage;
        private FileUserControl fileUserControl;
        private DbUserControl dbUserControl;
        private System.Windows.Forms.TabPage aboutTabPage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage appSpecTabPage;
        private AppSpesific appSpesific1;
        private System.Windows.Forms.Button button1;
    }
}

