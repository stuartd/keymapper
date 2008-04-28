namespace KeyMapper.Forms
{
	partial class Help
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
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(15, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(213, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Drag a key off the form to disable it";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(15, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(428, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "To remap a key, double-click it (or drop the key you want it to be onto it)";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(15, 67);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(356, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "To delete a mapping or re-enable a key, drag it off the form.";
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(18, 186);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(171, 17);
			this.checkBox1.TabIndex = 3;
			this.checkBox1.Text = "Show this form at startup";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// Help
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(502, 228);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "Help";
			this.Text = "Help";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkBox1;
	}
}