namespace RoseHillSolutions.KeyMapper
{
	partial class MappingListForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param _name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			this.grdMappings = new System.Windows.Forms.DataGridView();
			this.KeyMapping = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Delete = new System.Windows.Forms.DataGridViewButtonColumn();
			((System.ComponentModel.ISupportInitialize)(this.grdMappings)).BeginInit();
			this.SuspendLayout();
			// 
			// grdMappings
			// 
			this.grdMappings.AllowUserToAddRows = false;
			this.grdMappings.AllowUserToDeleteRows = false;
			this.grdMappings.AllowUserToResizeColumns = false;
			this.grdMappings.AllowUserToResizeRows = false;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.ControlLight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
			this.grdMappings.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
			this.grdMappings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.grdMappings.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.grdMappings.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.grdMappings.CausesValidation = false;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ControlDark;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 8.25F);
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ControlLightLight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.grdMappings.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.grdMappings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grdMappings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.KeyMapping,
            this.Type,
            this.Status,
            this.Delete});
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Verdana", 8.25F);
			dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.ControlLight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.grdMappings.DefaultCellStyle = dataGridViewCellStyle3;
			this.grdMappings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grdMappings.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.grdMappings.Location = new System.Drawing.Point(0, 0);
			this.grdMappings.MultiSelect = false;
			this.grdMappings.Name = "grdMappings";
			this.grdMappings.ReadOnly = true;
			this.grdMappings.RowHeadersVisible = false;
			this.grdMappings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.grdMappings.Size = new System.Drawing.Size(499, 156);
			this.grdMappings.TabIndex = 4;
			this.grdMappings.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.grdMappingsCellPainting);
			this.grdMappings.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdMappingsCellContentClick);
			// 
			// KeyMapping
			// 
			this.KeyMapping.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.KeyMapping.FillWeight = 61F;
			this.KeyMapping.HeaderText = "Key Mappings";
			this.KeyMapping.Name = "KeyMapping";
			this.KeyMapping.ReadOnly = true;
			this.KeyMapping.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// Type
			// 
			this.Type.FillWeight = 13F;
			this.Type.HeaderText = "Type";
			this.Type.Name = "Type";
			this.Type.ReadOnly = true;
			this.Type.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// Status
			// 
			this.Status.FillWeight = 13F;
			this.Status.HeaderText = "Status";
			this.Status.Name = "Status";
			this.Status.ReadOnly = true;
			this.Status.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// Delete
			// 
			this.Delete.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Delete.FillWeight = 13F;
			this.Delete.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.Delete.HeaderText = "Delete";
			this.Delete.MinimumWidth = 60;
			this.Delete.Name = "Delete";
			this.Delete.ReadOnly = true;
			this.Delete.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Delete.Text = "Delete";
			this.Delete.ToolTipText = "Delete this mapping";
			this.Delete.UseColumnTextForButtonValue = true;
			this.Delete.Width = 60;
			// 
			// MappingListForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(499, 156);
			this.Controls.Add(this.grdMappings);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MappingListForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Key Mapping List";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MappingListFormClosing);
			((System.ComponentModel.ISupportInitialize)(this.grdMappings)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView grdMappings;
		private System.Windows.Forms.DataGridViewTextBoxColumn KeyMapping;
		private System.Windows.Forms.DataGridViewTextBoxColumn Type;
		private System.Windows.Forms.DataGridViewTextBoxColumn Status;
		private System.Windows.Forms.DataGridViewButtonColumn Delete;
	}
}

