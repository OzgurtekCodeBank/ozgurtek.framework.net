
namespace ozgurtek.framework.converter.winforms
{
    partial class AppSpesific
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
            this.TileBinaJsonButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TileBinaJsonButton
            // 
            this.TileBinaJsonButton.Location = new System.Drawing.Point(61, 27);
            this.TileBinaJsonButton.Name = "TileBinaJsonButton";
            this.TileBinaJsonButton.Size = new System.Drawing.Size(162, 23);
            this.TileBinaJsonButton.TabIndex = 0;
            this.TileBinaJsonButton.Text = "Tile Bina Json";
            this.TileBinaJsonButton.UseVisualStyleBackColor = true;
            this.TileBinaJsonButton.Click += new System.EventHandler(this.TileBinaJsonButton_Click);
            // 
            // AppSpesific
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TileBinaJsonButton);
            this.Name = "AppSpesific";
            this.Size = new System.Drawing.Size(649, 300);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button TileBinaJsonButton;
    }
}
