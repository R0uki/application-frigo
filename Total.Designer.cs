namespace projet_SEILER_TRAN_VOLTZ
{
    partial class Total
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
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnlRecapIngrUsten = new System.Windows.Forms.Panel();
            this.pnlIngr = new System.Windows.Forms.Panel();
            this.pnlUsten = new System.Windows.Forms.Panel();
            this.pnlEtape = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlRecapIngrUsten.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::projet_SEILER_TRAN_VOLTZ.Properties.Resources.logo1;
            this.pictureBox3.Location = new System.Drawing.Point(1044, 26);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(288, 123);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 2;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::projet_SEILER_TRAN_VOLTZ.Properties.Resources.imgDefaut;
            this.pictureBox2.Location = new System.Drawing.Point(160, 13);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(205, 170);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = global::projet_SEILER_TRAN_VOLTZ.Properties.Resources.btnRetour4;
            this.pictureBox1.Location = new System.Drawing.Point(19, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(80, 80);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.sortirPage);
            // 
            // pnlRecapIngrUsten
            // 
            this.pnlRecapIngrUsten.AutoScroll = true;
            this.pnlRecapIngrUsten.BackgroundImage = global::projet_SEILER_TRAN_VOLTZ.Properties.Resources.fond6;
            this.pnlRecapIngrUsten.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlRecapIngrUsten.Controls.Add(this.pnlIngr);
            this.pnlRecapIngrUsten.Controls.Add(this.pnlUsten);
            this.pnlRecapIngrUsten.Location = new System.Drawing.Point(19, 209);
            this.pnlRecapIngrUsten.Name = "pnlRecapIngrUsten";
            this.pnlRecapIngrUsten.Size = new System.Drawing.Size(330, 520);
            this.pnlRecapIngrUsten.TabIndex = 3;
            // 
            // pnlIngr
            // 
            this.pnlIngr.AutoScroll = true;
            this.pnlIngr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(114)))), ((int)(((byte)(22)))));
            this.pnlIngr.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlIngr.Location = new System.Drawing.Point(18, 62);
            this.pnlIngr.Name = "pnlIngr";
            this.pnlIngr.Size = new System.Drawing.Size(293, 192);
            this.pnlIngr.TabIndex = 5;
            // 
            // pnlUsten
            // 
            this.pnlUsten.AutoScroll = true;
            this.pnlUsten.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(114)))), ((int)(((byte)(22)))));
            this.pnlUsten.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlUsten.Location = new System.Drawing.Point(18, 317);
            this.pnlUsten.Name = "pnlUsten";
            this.pnlUsten.Size = new System.Drawing.Size(293, 191);
            this.pnlUsten.TabIndex = 4;
            // 
            // pnlEtape
            // 
            this.pnlEtape.AutoScroll = true;
            this.pnlEtape.Location = new System.Drawing.Point(382, 225);
            this.pnlEtape.Name = "pnlEtape";
            this.pnlEtape.Size = new System.Drawing.Size(969, 517);
            this.pnlEtape.TabIndex = 5;
            // 
            // Total
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1344, 741);
            this.Controls.Add(this.pnlEtape);
            this.Controls.Add(this.pnlRecapIngrUsten);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Total";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Total";
            this.Load += new System.EventHandler(this.Total_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlRecapIngrUsten.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Panel pnlRecapIngrUsten;
        private System.Windows.Forms.Panel pnlIngr;
        private System.Windows.Forms.Panel pnlUsten;
        private System.Windows.Forms.Panel pnlEtape;
    }
}