
namespace ozgurtek.framework.converter.winforms
{
    partial class OutputUserControl
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
            this.OutputGroupBox = new System.Windows.Forms.GroupBox();
            this.TileTypeComboBox = new System.Windows.Forms.ComboBox();
            this.SuppressBlankTileCheck = new System.Windows.Forms.CheckBox();
            this.folderButton = new System.Windows.Forms.Button();
            this.OutPutFolderTextBox = new System.Windows.Forms.TextBox();
            this.EpsgTextBox = new System.Windows.Forms.TextBox();
            this.DescTextBox = new System.Windows.Forms.TextBox();
            this.StyleFieldTextBox = new System.Windows.Forms.TextBox();
            this.ExtFieldTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.GeomFieldTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.FidFieldTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.XyTileCountTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.OutputFolderLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.GroundHeightTextBox = new System.Windows.Forms.TextBox();
            this.OutputGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // OutputGroupBox
            // 
            this.OutputGroupBox.Controls.Add(this.TileTypeComboBox);
            this.OutputGroupBox.Controls.Add(this.SuppressBlankTileCheck);
            this.OutputGroupBox.Controls.Add(this.folderButton);
            this.OutputGroupBox.Controls.Add(this.OutPutFolderTextBox);
            this.OutputGroupBox.Controls.Add(this.GroundHeightTextBox);
            this.OutputGroupBox.Controls.Add(this.EpsgTextBox);
            this.OutputGroupBox.Controls.Add(this.DescTextBox);
            this.OutputGroupBox.Controls.Add(this.StyleFieldTextBox);
            this.OutputGroupBox.Controls.Add(this.ExtFieldTextBox);
            this.OutputGroupBox.Controls.Add(this.label6);
            this.OutputGroupBox.Controls.Add(this.GeomFieldTextBox);
            this.OutputGroupBox.Controls.Add(this.label5);
            this.OutputGroupBox.Controls.Add(this.FidFieldTextBox);
            this.OutputGroupBox.Controls.Add(this.label4);
            this.OutputGroupBox.Controls.Add(this.XyTileCountTextBox);
            this.OutputGroupBox.Controls.Add(this.label7);
            this.OutputGroupBox.Controls.Add(this.label3);
            this.OutputGroupBox.Controls.Add(this.label2);
            this.OutputGroupBox.Controls.Add(this.label1);
            this.OutputGroupBox.Controls.Add(this.OutputFolderLabel);
            this.OutputGroupBox.Location = new System.Drawing.Point(7, 10);
            this.OutputGroupBox.Name = "OutputGroupBox";
            this.OutputGroupBox.Size = new System.Drawing.Size(713, 249);
            this.OutputGroupBox.TabIndex = 8;
            this.OutputGroupBox.TabStop = false;
            this.OutputGroupBox.Text = "Output";
            // 
            // TileTypeComboBox
            // 
            this.TileTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TileTypeComboBox.FormattingEnabled = true;
            this.TileTypeComboBox.Items.AddRange(new object[] {
            "XY Tile Count",
            "Size In Meter"});
            this.TileTypeComboBox.Location = new System.Drawing.Point(14, 29);
            this.TileTypeComboBox.Name = "TileTypeComboBox";
            this.TileTypeComboBox.Size = new System.Drawing.Size(99, 21);
            this.TileTypeComboBox.TabIndex = 8;
            // 
            // SuppressBlankTileCheck
            // 
            this.SuppressBlankTileCheck.AutoSize = true;
            this.SuppressBlankTileCheck.Checked = true;
            this.SuppressBlankTileCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SuppressBlankTileCheck.Location = new System.Drawing.Point(549, 229);
            this.SuppressBlankTileCheck.Name = "SuppressBlankTileCheck";
            this.SuppressBlankTileCheck.Size = new System.Drawing.Size(120, 17);
            this.SuppressBlankTileCheck.TabIndex = 7;
            this.SuppressBlankTileCheck.Text = "Suppress Blank Tile";
            this.SuppressBlankTileCheck.UseVisualStyleBackColor = true;
            // 
            // folderButton
            // 
            this.folderButton.Location = new System.Drawing.Point(83, 116);
            this.folderButton.Name = "folderButton";
            this.folderButton.Size = new System.Drawing.Size(23, 23);
            this.folderButton.TabIndex = 2;
            this.folderButton.Text = "...";
            this.folderButton.UseVisualStyleBackColor = true;
            this.folderButton.Click += new System.EventHandler(this.folderButton_Click);
            // 
            // OutPutFolderTextBox
            // 
            this.OutPutFolderTextBox.Location = new System.Drawing.Point(119, 116);
            this.OutPutFolderTextBox.Multiline = true;
            this.OutPutFolderTextBox.Name = "OutPutFolderTextBox";
            this.OutPutFolderTextBox.Size = new System.Drawing.Size(208, 55);
            this.OutPutFolderTextBox.TabIndex = 2;
            // 
            // EpsgTextBox
            // 
            this.EpsgTextBox.Location = new System.Drawing.Point(119, 71);
            this.EpsgTextBox.Name = "EpsgTextBox";
            this.EpsgTextBox.Size = new System.Drawing.Size(208, 20);
            this.EpsgTextBox.TabIndex = 1;
            // 
            // DescTextBox
            // 
            this.DescTextBox.Location = new System.Drawing.Point(442, 203);
            this.DescTextBox.Name = "DescTextBox";
            this.DescTextBox.Size = new System.Drawing.Size(227, 20);
            this.DescTextBox.TabIndex = 6;
            // 
            // StyleFieldTextBox
            // 
            this.StyleFieldTextBox.Location = new System.Drawing.Point(442, 158);
            this.StyleFieldTextBox.Name = "StyleFieldTextBox";
            this.StyleFieldTextBox.Size = new System.Drawing.Size(227, 20);
            this.StyleFieldTextBox.TabIndex = 6;
            // 
            // ExtFieldTextBox
            // 
            this.ExtFieldTextBox.Location = new System.Drawing.Point(442, 118);
            this.ExtFieldTextBox.Name = "ExtFieldTextBox";
            this.ExtFieldTextBox.Size = new System.Drawing.Size(227, 20);
            this.ExtFieldTextBox.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(352, 203);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Desc Field";
            // 
            // GeomFieldTextBox
            // 
            this.GeomFieldTextBox.Location = new System.Drawing.Point(442, 71);
            this.GeomFieldTextBox.Name = "GeomFieldTextBox";
            this.GeomFieldTextBox.Size = new System.Drawing.Size(227, 20);
            this.GeomFieldTextBox.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(352, 158);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Style Field";
            // 
            // FidFieldTextBox
            // 
            this.FidFieldTextBox.Location = new System.Drawing.Point(442, 29);
            this.FidFieldTextBox.Name = "FidFieldTextBox";
            this.FidFieldTextBox.Size = new System.Drawing.Size(227, 20);
            this.FidFieldTextBox.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(352, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Ext Field";
            // 
            // XyTileCountTextBox
            // 
            this.XyTileCountTextBox.Location = new System.Drawing.Point(119, 29);
            this.XyTileCountTextBox.Name = "XyTileCountTextBox";
            this.XyTileCountTextBox.Size = new System.Drawing.Size(208, 20);
            this.XyTileCountTextBox.TabIndex = 0;
            this.XyTileCountTextBox.Text = "10";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(352, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Geom Field";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Source EPSG";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(352, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fid Field";
            // 
            // OutputFolderLabel
            // 
            this.OutputFolderLabel.AutoSize = true;
            this.OutputFolderLabel.Location = new System.Drawing.Point(14, 119);
            this.OutputFolderLabel.Name = "OutputFolderLabel";
            this.OutputFolderLabel.Size = new System.Drawing.Size(68, 13);
            this.OutputFolderLabel.TabIndex = 0;
            this.OutputFolderLabel.Text = "Ouput Folder";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 196);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(76, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Ground Height";
            // 
            // GroundHeightTextBox
            // 
            this.GroundHeightTextBox.Location = new System.Drawing.Point(119, 196);
            this.GroundHeightTextBox.Name = "GroundHeightTextBox";
            this.GroundHeightTextBox.Size = new System.Drawing.Size(208, 20);
            this.GroundHeightTextBox.TabIndex = 1;
            // 
            // OutputUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.OutputGroupBox);
            this.Name = "OutputUserControl";
            this.Size = new System.Drawing.Size(732, 269);
            this.OutputGroupBox.ResumeLayout(false);
            this.OutputGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox OutputGroupBox;
        private System.Windows.Forms.Button folderButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label OutputFolderLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        internal System.Windows.Forms.TextBox OutPutFolderTextBox;
        internal System.Windows.Forms.TextBox EpsgTextBox;
        internal System.Windows.Forms.TextBox XyTileCountTextBox;
        internal System.Windows.Forms.TextBox ExtFieldTextBox;
        internal System.Windows.Forms.TextBox GeomFieldTextBox;
        internal System.Windows.Forms.TextBox FidFieldTextBox;
        internal System.Windows.Forms.TextBox StyleFieldTextBox;
        internal System.Windows.Forms.CheckBox SuppressBlankTileCheck;
        internal System.Windows.Forms.TextBox DescTextBox;
        private System.Windows.Forms.Label label6;
        internal System.Windows.Forms.ComboBox TileTypeComboBox;
        internal System.Windows.Forms.TextBox GroundHeightTextBox;
        private System.Windows.Forms.Label label7;
    }
}
