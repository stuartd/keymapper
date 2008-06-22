namespace KeyMapper
{
    partial class IdentifyNumLockKey
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
                if (_sniffer != null)
                    _sniffer.Dispose();
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
            this.KeyPictureBox = new KeyMapper.KMPictureBox();
            this.lblExplanation = new System.Windows.Forms.Label();
            this.OKButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.KeyPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // KeyPictureBox
            // 
            this.KeyPictureBox.Location = new System.Drawing.Point(12, 144);
            this.KeyPictureBox.Name = "KeyPictureBox";
            this.KeyPictureBox.Size = new System.Drawing.Size(128, 128);
            this.KeyPictureBox.TabIndex = 0;
            this.KeyPictureBox.TabStop = false;
            // 
            // lblExplanation
            // 
            this.lblExplanation.AutoSize = true;
            this.lblExplanation.Font = new System.Drawing.Font("Verdana", 10F);
            this.lblExplanation.Location = new System.Drawing.Point(9, 9);
            this.lblExplanation.Name = "lblExplanation";
            this.lblExplanation.Size = new System.Drawing.Size(48, 17);
            this.lblExplanation.TabIndex = 3;
            this.lblExplanation.Text = "[why]";
            // 
            // OKButton
            // 
            this.OKButton.Enabled = false;
            this.OKButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OKButton.Location = new System.Drawing.Point(260, 238);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(84, 34);
            this.OKButton.TabIndex = 5;
            this.OKButton.Text = "&OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // IdentifyNumLockKey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 284);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.lblExplanation);
            this.Controls.Add(this.KeyPictureBox);
            this.Name = "IdentifyNumLockKey";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Identify Num Lock";
            ((System.ComponentModel.ISupportInitialize)(this.KeyPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private KMPictureBox KeyPictureBox;
        private System.Windows.Forms.Label lblExplanation;
        private System.Windows.Forms.Button OKButton;
    }
}