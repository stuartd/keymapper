namespace KeyMapper
{
	partial class EditMappingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditMappingForm));
            this.FromKeyPictureBox = new System.Windows.Forms.PictureBox();
            this.ToKeyPictureBox = new System.Windows.Forms.PictureBox();
            this.MapButton = new System.Windows.Forms.Button();
            this.KeyListsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.KeysByGroupListbox = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.GroupsListbox = new System.Windows.Forms.ListBox();
            this.ListOptionsCombo = new System.Windows.Forms.ComboBox();
            this.MappingPanel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.CaptureAndCancelButton = new System.Windows.Forms.Button();
            this.DisableButton = new System.Windows.Forms.Button();
            this.EmptyPanel = new System.Windows.Forms.Panel();
            this.PanelFader = new KeyMapper.PanelFader();
            ((System.ComponentModel.ISupportInitialize)(this.FromKeyPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToKeyPictureBox)).BeginInit();
            this.KeyListsPanel.SuspendLayout();
            this.MappingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // FromKeyPictureBox
            // 
            this.FromKeyPictureBox.Location = new System.Drawing.Point(5, 28);
            this.FromKeyPictureBox.Name = "FromKeyPictureBox";
            this.FromKeyPictureBox.Size = new System.Drawing.Size(128, 128);
            this.FromKeyPictureBox.TabIndex = 0;
            this.FromKeyPictureBox.TabStop = false;
            // 
            // ToKeyPictureBox
            // 
            this.ToKeyPictureBox.Location = new System.Drawing.Point(87, 21);
            this.ToKeyPictureBox.Name = "ToKeyPictureBox";
            this.ToKeyPictureBox.Size = new System.Drawing.Size(128, 128);
            this.ToKeyPictureBox.TabIndex = 1;
            this.ToKeyPictureBox.TabStop = false;
            // 
            // MapButton
            // 
            this.MapButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MapButton.Location = new System.Drawing.Point(402, 30);
            this.MapButton.Name = "MapButton";
            this.MapButton.Size = new System.Drawing.Size(73, 34);
            this.MapButton.TabIndex = 4;
            this.MapButton.Text = "&Map";
            this.MapButton.UseVisualStyleBackColor = true;
            this.MapButton.Click += new System.EventHandler(this.MapButtonClick);
            // 
            // KeyListsPanel
            // 
            this.KeyListsPanel.ColumnCount = 2;
            this.KeyListsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47.54717F));
            this.KeyListsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52.45283F));
            this.KeyListsPanel.Controls.Add(this.KeysByGroupListbox, 1, 1);
            this.KeyListsPanel.Controls.Add(this.label2, 1, 0);
            this.KeyListsPanel.Controls.Add(this.label3, 0, 0);
            this.KeyListsPanel.Controls.Add(this.GroupsListbox, 0, 1);
            this.KeyListsPanel.Controls.Add(this.ListOptionsCombo, 0, 2);
            this.KeyListsPanel.Location = new System.Drawing.Point(135, 9);
            this.KeyListsPanel.Name = "KeyListsPanel";
            this.KeyListsPanel.RowCount = 3;
            this.KeyListsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.KeyListsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88F));
            this.KeyListsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.KeyListsPanel.Size = new System.Drawing.Size(265, 174);
            this.KeyListsPanel.TabIndex = 12;
            // 
            // KeysByGroupListbox
            // 
            this.KeysByGroupListbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.KeysByGroupListbox.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeysByGroupListbox.FormattingEnabled = true;
            this.KeysByGroupListbox.ItemHeight = 15;
            this.KeysByGroupListbox.Location = new System.Drawing.Point(128, 20);
            this.KeysByGroupListbox.Name = "KeysByGroupListbox";
            this.KeysByGroupListbox.Size = new System.Drawing.Size(134, 124);
            this.KeysByGroupListbox.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 10F);
            this.label2.Location = new System.Drawing.Point(128, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 17);
            this.label2.TabIndex = 13;
            this.label2.Text = "Key lists";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Verdana", 10F);
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Key groups";
            // 
            // GroupsListbox
            // 
            this.GroupsListbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupsListbox.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupsListbox.FormattingEnabled = true;
            this.GroupsListbox.ItemHeight = 15;
            this.GroupsListbox.Location = new System.Drawing.Point(3, 20);
            this.GroupsListbox.Name = "GroupsListbox";
            this.GroupsListbox.Size = new System.Drawing.Size(119, 124);
            this.GroupsListbox.TabIndex = 13;
            // 
            // ListOptionsCombo
            // 
            this.ListOptionsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ListOptionsCombo.FormattingEnabled = true;
            this.ListOptionsCombo.Items.AddRange(new object[] {
            "Useful keys only",
            "All working keys",
            "All keys"});
            this.ListOptionsCombo.Location = new System.Drawing.Point(3, 150);
            this.ListOptionsCombo.Name = "ListOptionsCombo";
            this.ListOptionsCombo.Size = new System.Drawing.Size(117, 21);
            this.ListOptionsCombo.TabIndex = 21;
            // 
            // MappingPanel
            // 
            this.MappingPanel.Controls.Add(this.pictureBox1);
            this.MappingPanel.Controls.Add(this.ToKeyPictureBox);
            this.MappingPanel.Location = new System.Drawing.Point(616, 38);
            this.MappingPanel.Name = "MappingPanel";
            this.MappingPanel.Size = new System.Drawing.Size(227, 174);
            this.MappingPanel.TabIndex = 13;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(14, 61);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(53, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 20;
            this.pictureBox1.TabStop = false;
            // 
            // CaptureAndCancelButton
            // 
            this.CaptureAndCancelButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CaptureAndCancelButton.Location = new System.Drawing.Point(402, 120);
            this.CaptureAndCancelButton.Name = "CaptureAndCancelButton";
            this.CaptureAndCancelButton.Size = new System.Drawing.Size(73, 34);
            this.CaptureAndCancelButton.TabIndex = 14;
            this.CaptureAndCancelButton.Text = "Cap&ture";
            this.CaptureAndCancelButton.UseVisualStyleBackColor = true;
            this.CaptureAndCancelButton.Click += new System.EventHandler(this.CaptureOrCancelButtonClick);
            // 
            // DisableButton
            // 
            this.DisableButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DisableButton.Location = new System.Drawing.Point(402, 75);
            this.DisableButton.Name = "DisableButton";
            this.DisableButton.Size = new System.Drawing.Size(73, 34);
            this.DisableButton.TabIndex = 16;
            this.DisableButton.Text = "Disa&ble";
            this.DisableButton.UseVisualStyleBackColor = true;
            this.DisableButton.Click += new System.EventHandler(this.DisableButtonClick);
            // 
            // EmptyPanel
            // 
            this.EmptyPanel.Location = new System.Drawing.Point(616, 218);
            this.EmptyPanel.Name = "EmptyPanel";
            this.EmptyPanel.Size = new System.Drawing.Size(227, 174);
            this.EmptyPanel.TabIndex = 19;
            // 
            // PanelFader
            // 
            this.PanelFader.Location = new System.Drawing.Point(562, 32);
            this.PanelFader.Name = "PanelFader";
            this.PanelFader.Size = new System.Drawing.Size(22, 18);
            this.PanelFader.TabIndex = 15;
            // 
            // EditMappingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 187);
            this.Controls.Add(this.EmptyPanel);
            this.Controls.Add(this.DisableButton);
            this.Controls.Add(this.PanelFader);
            this.Controls.Add(this.CaptureAndCancelButton);
            this.Controls.Add(this.MappingPanel);
            this.Controls.Add(this.KeyListsPanel);
            this.Controls.Add(this.MapButton);
            this.Controls.Add(this.FromKeyPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditMappingForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Mapping Editor";
            this.Deactivate += new System.EventHandler(this.FormDeactivate);
            this.Activated += new System.EventHandler(this.FormActivated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditMappingFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.FromKeyPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToKeyPictureBox)).EndInit();
            this.KeyListsPanel.ResumeLayout(false);
            this.KeyListsPanel.PerformLayout();
            this.MappingPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox FromKeyPictureBox;
		private System.Windows.Forms.PictureBox ToKeyPictureBox;
		private System.Windows.Forms.Button MapButton;
		private System.Windows.Forms.TableLayoutPanel KeyListsPanel;
		private System.Windows.Forms.ListBox GroupsListbox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ListBox KeysByGroupListbox;
		private System.Windows.Forms.Panel MappingPanel;
		private System.Windows.Forms.Button CaptureAndCancelButton;
		private PanelFader PanelFader;
		private System.Windows.Forms.Button DisableButton;
		private System.Windows.Forms.Panel EmptyPanel;
		private System.Windows.Forms.ComboBox ListOptionsCombo;
		private System.Windows.Forms.PictureBox pictureBox1;
	}
}