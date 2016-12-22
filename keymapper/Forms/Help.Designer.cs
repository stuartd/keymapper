namespace KeyMapper.Forms
{
	partial class HelpForm
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
			if (disposing && (this.components != null))
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chkShowHelpAtStartup = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelFAQ = new System.Windows.Forms.LinkLabel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(282, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Drag a key off the keyboard to disable it.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(15, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(219, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "To remap a key, double-click it.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(471, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "To delete a key mapping or re-enable a key, drag it off the keyboard.";
            // 
            // chkShowHelpAtStartup
            // 
            this.chkShowHelpAtStartup.AutoSize = true;
            this.chkShowHelpAtStartup.Checked = true;
            this.chkShowHelpAtStartup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowHelpAtStartup.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkShowHelpAtStartup.Location = new System.Drawing.Point(18, 3);
            this.chkShowHelpAtStartup.Name = "chkShowHelpAtStartup";
            this.chkShowHelpAtStartup.Size = new System.Drawing.Size(167, 20);
            this.chkShowHelpAtStartup.TabIndex = 3;
            this.chkShowHelpAtStartup.Text = "Show help at startup";
            this.chkShowHelpAtStartup.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(15, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(446, 32);
            this.label4.TabIndex = 4;
            this.label4.Text = "You can capture the key you want to map or choose it from a list\r\nby choosing Cre" +
                "ate New Mapping from the Mappings menu.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 161);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(453, 32);
            this.label5.TabIndex = 1;
            this.label5.Text = "If the key you want to map and the one you want to map it to are\r\nboth visible, d" +
                "rag and drop the action key onto the target key.\r\n";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.labelFAQ);
            this.panel1.Controls.Add(this.chkShowHelpAtStartup);
            this.panel1.Location = new System.Drawing.Point(-1, 206);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(500, 26);
            this.panel1.TabIndex = 5;
            // 
            // labelFAQ
            // 
            this.labelFAQ.AutoSize = true;
            this.labelFAQ.Font = new System.Drawing.Font("Verdana", 9.75F);
            this.labelFAQ.Location = new System.Drawing.Point(410, 4);
            this.labelFAQ.Name = "labelFAQ";
            this.labelFAQ.Size = new System.Drawing.Size(80, 16);
            this.labelFAQ.TabIndex = 4;
            this.labelFAQ.TabStop = true;
            this.labelFAQ.Text = "Online FAQ";
            this.labelFAQ.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelFAQClick);
            // 
            // HelpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 231);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HelpForm";
            this.ShowInTaskbar = false;
            this.Text = "KeyMapper Help";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HelpFormFormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox chkShowHelpAtStartup;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel labelFAQ;
	}
}