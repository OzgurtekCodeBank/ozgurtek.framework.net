namespace ozgurtek.framework.test.winforms
{
    partial class TestForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.fileTextBox = new System.Windows.Forms.TextBox();
            this.minZoomLevelText = new System.Windows.Forms.TextBox();
            this.maxZoomLevelText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.minXTextBox = new System.Windows.Forms.TextBox();
            this.minYTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.maxXTextBox = new System.Windows.Forms.TextBox();
            this.maxYTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tileSizelabel = new System.Windows.Forms.Label();
            this.calcTileButton = new System.Windows.Forms.Button();
            this.densityResultLabel = new System.Windows.Forms.Label();
            this.densityCalcButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(52, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(467, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "for unit testing, see the unit test folder";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(120, 86);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(14, 177);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "TILE!!";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // fileTextBox
            // 
            this.fileTextBox.Location = new System.Drawing.Point(23, 40);
            this.fileTextBox.Name = "fileTextBox";
            this.fileTextBox.Size = new System.Drawing.Size(268, 20);
            this.fileTextBox.TabIndex = 2;
            this.fileTextBox.Text = "C:\\TURKEY_DTED\\turkey_dted_30m_3857.tif";
            // 
            // minZoomLevelText
            // 
            this.minZoomLevelText.Location = new System.Drawing.Point(22, 101);
            this.minZoomLevelText.Name = "minZoomLevelText";
            this.minZoomLevelText.Size = new System.Drawing.Size(100, 20);
            this.minZoomLevelText.TabIndex = 3;
            this.minZoomLevelText.Text = "20";
            // 
            // maxZoomLevelText
            // 
            this.maxZoomLevelText.Location = new System.Drawing.Point(177, 101);
            this.maxZoomLevelText.Name = "maxZoomLevelText";
            this.maxZoomLevelText.Size = new System.Drawing.Size(100, 20);
            this.maxZoomLevelText.TabIndex = 4;
            this.maxZoomLevelText.Text = "20";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "min zoom level";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(183, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "max zoom level";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(214, 164);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "minY";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(39, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "minX";
            // 
            // minXTextBox
            // 
            this.minXTextBox.Location = new System.Drawing.Point(15, 180);
            this.minXTextBox.Name = "minXTextBox";
            this.minXTextBox.Size = new System.Drawing.Size(100, 20);
            this.minXTextBox.TabIndex = 8;
            this.minXTextBox.Text = "34.4646353216893";
            // 
            // minYTextBox
            // 
            this.minYTextBox.Location = new System.Drawing.Point(176, 180);
            this.minYTextBox.Name = "minYTextBox";
            this.minYTextBox.Size = new System.Drawing.Size(100, 20);
            this.minYTextBox.TabIndex = 7;
            this.minYTextBox.Text = "38.73258080727801";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(214, 215);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "maxY";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 215);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "maxX";
            // 
            // maxXTextBox
            // 
            this.maxXTextBox.Location = new System.Drawing.Point(15, 231);
            this.maxXTextBox.Name = "maxXTextBox";
            this.maxXTextBox.Size = new System.Drawing.Size(100, 20);
            this.maxXTextBox.TabIndex = 12;
            this.maxXTextBox.Text = "34.62385274471592";
            // 
            // maxYTextBox
            // 
            this.maxYTextBox.Location = new System.Drawing.Point(176, 231);
            this.maxYTextBox.Name = "maxYTextBox";
            this.maxYTextBox.Size = new System.Drawing.Size(100, 20);
            this.maxYTextBox.TabIndex = 11;
            this.maxYTextBox.Text = "38.812034524942696";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(116, 152);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "envelope";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(23, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "File";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(304, 86);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 17;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(404, 86);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 18;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.maxXTextBox);
            this.panel1.Controls.Add(this.maxYTextBox);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.minXTextBox);
            this.panel1.Controls.Add(this.minYTextBox);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.maxZoomLevelText);
            this.panel1.Controls.Add(this.minZoomLevelText);
            this.panel1.Controls.Add(this.fileTextBox);
            this.panel1.Location = new System.Drawing.Point(35, 205);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(322, 299);
            this.panel1.TabIndex = 23;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tileSizelabel);
            this.panel2.Controls.Add(this.calcTileButton);
            this.panel2.Controls.Add(this.densityResultLabel);
            this.panel2.Controls.Add(this.densityCalcButton);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Location = new System.Drawing.Point(363, 205);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(127, 299);
            this.panel2.TabIndex = 24;
            // 
            // tileSizelabel
            // 
            this.tileSizelabel.AutoSize = true;
            this.tileSizelabel.Location = new System.Drawing.Point(11, 127);
            this.tileSizelabel.Name = "tileSizelabel";
            this.tileSizelabel.Size = new System.Drawing.Size(25, 13);
            this.tileSizelabel.TabIndex = 26;
            this.tileSizelabel.Text = "size";
            // 
            // calcTileButton
            // 
            this.calcTileButton.Location = new System.Drawing.Point(14, 101);
            this.calcTileButton.Name = "calcTileButton";
            this.calcTileButton.Size = new System.Drawing.Size(98, 23);
            this.calcTileButton.TabIndex = 25;
            this.calcTileButton.Text = "Calc Tile Size";
            this.calcTileButton.UseVisualStyleBackColor = true;
            this.calcTileButton.Click += new System.EventHandler(this.calcTileButton_Click);
            // 
            // densityResultLabel
            // 
            this.densityResultLabel.AutoSize = true;
            this.densityResultLabel.Location = new System.Drawing.Point(11, 64);
            this.densityResultLabel.Name = "densityResultLabel";
            this.densityResultLabel.Size = new System.Drawing.Size(40, 13);
            this.densityResultLabel.TabIndex = 24;
            this.densityResultLabel.Text = "density";
            // 
            // densityCalcButton
            // 
            this.densityCalcButton.Location = new System.Drawing.Point(14, 38);
            this.densityCalcButton.Name = "densityCalcButton";
            this.densityCalcButton.Size = new System.Drawing.Size(98, 23);
            this.densityCalcButton.TabIndex = 23;
            this.densityCalcButton.Text = "Calc Density";
            this.densityCalcButton.UseVisualStyleBackColor = true;
            this.densityCalcButton.Click += new System.EventHandler(this.densityCalcButton_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 526);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TestForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Samples";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox fileTextBox;
        private System.Windows.Forms.TextBox minZoomLevelText;
        private System.Windows.Forms.TextBox maxZoomLevelText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox minXTextBox;
        private System.Windows.Forms.TextBox minYTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox maxXTextBox;
        private System.Windows.Forms.TextBox maxYTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label tileSizelabel;
        private System.Windows.Forms.Button calcTileButton;
        private System.Windows.Forms.Label densityResultLabel;
        private System.Windows.Forms.Button densityCalcButton;
    }
}

