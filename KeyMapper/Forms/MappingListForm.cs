using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace KeyMapper
{
	public partial class MappingListForm : KMBaseForm
	{

		private List<int> _clearedKeys = new List<int>();
		private List<Key> _keylist = new List<Key>();

		public MappingListForm(Point callingFormLocation, Size callingFormSize)
		{
			InitializeComponent();
			LoadUserSettings(callingFormLocation, callingFormSize);
			Populate();
			MappingsManager.MappingsChanged += HandleMappingsChanged;
		}

		private void LoadUserSettings(Point callingFormLocation, Size callingFormSize)
		{

			Properties.Settings userSettings = new Properties.Settings();

			Point savedLocation = userSettings.MappingListFormLocation;
			int savedHeight = userSettings.MappingListFormHeight;
			int savedWidth = userSettings.MappingListWidth;

			// Position ourselves under the calling form.

			if (savedLocation.IsEmpty)
			{
				this.Location = new Point(callingFormLocation.X + callingFormSize.Width - this.Width, callingFormLocation.Y + callingFormSize.Height + 1);
			}
			else
			{
				this.Location = savedLocation;
			}

			if (savedHeight > 0)
				this.Height = savedHeight;

			if (savedWidth > 0)
				this.Width = savedWidth;
		}

		private void MappingListFormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				this.Hide();
			}
			SaveUserSettings();
		}

		private void SaveUserSettings()
		{
			Properties.Settings userSettings = new Properties.Settings();
			userSettings.MappingListFormLocation = this.Location;
			userSettings.MappingListFormHeight = this.Height;
			userSettings.Save();
		}



		void HandleMappingsChanged(object sender, EventArgs e)
		{
			Populate();
		}

		private void Populate()
		{

			grdMappings.Rows.Clear();
			_clearedKeys.Clear();
			_keylist.Clear();

			AddRowsToGrid();

			// Resize according to number of mappings
			int height = grdMappings.ColumnHeadersHeight;

			foreach (DataGridViewRow row in grdMappings.Rows)
				height += row.Height + row.DividerHeight;

			this.SetClientSizeCore(this.ClientSize.Width, height);

			// ResizeControls();
		}

		private void AddRowsToGrid()
		{
			AddRowsToGrid(MappingFilter.User);
			AddRowsToGrid(MappingFilter.Boot);
			AddRowsToGrid(MappingFilter.ClearedUser);
			AddRowsToGrid(MappingFilter.ClearedBoot);

			if (grdMappings.RowCount == 0)
			{
				// No mappings.
				int index = grdMappings.Rows.Add("You haven't created any mappings yet");
				_clearedKeys.Add(index); // Stops Delete key being shown. 
			}

		}

		private void AddRowsToGrid(MappingFilter filter)
		{
			Collection<KeyMapping> maps = MappingsManager.GetMappings(filter);

			foreach (KeyMapping map in maps)
			{
				if (filter == MappingFilter.ClearedUser || filter == MappingFilter.ClearedBoot)
				{
					if (_keylist.Contains(map.From))
						return; // Don't add an entry for a cleared key which has been remapped.
				}
				else
				{
					_keylist.Add(map.From);
				}

				int index = grdMappings.Rows.Add(map.ToString());
				grdMappings.Rows[index].Tag = map;

				string cellvalue = string.Empty;

				switch (filter)
				{
					case MappingFilter.Boot:
						cellvalue = "Boot";
						break;

					case MappingFilter.User:
						cellvalue = "User";
						break;

					case MappingFilter.ClearedUser:
					case MappingFilter.ClearedBoot:
						cellvalue = "Cleared";

						// Need to store the row to a little array as
						// don't want to have to access each cell to decide whether 
						// to show the delete button for it or not.
						_clearedKeys.Add(index);
						break;
				}

				grdMappings.Rows[index].Cells[1].Value = cellvalue;

				if (MappingsManager.IsMappingPending(map, filter))
					grdMappings.Rows[index].Cells[2].Value = "Pending";
				else
					grdMappings.Rows[index].Cells[2].Value = "Mapped";
			}

		}


		private void grdMappingsCellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != 3)
				return;

			int row = e.RowIndex;

			if (_clearedKeys.Contains(row))
				return; // Shouldn't happen anyway

			if (row >= 0)
			{
				DataGridViewRow currentRow = grdMappings.Rows[row];

				if (currentRow.Tag != null)
				{
					// Is this a user or a boot mapping?
					MappingFilter filter;
					if (currentRow.Cells[1].Value.ToString() == "User")
						filter = MappingFilter.User;
					else if (currentRow.Cells[1].Value.ToString() == "Boot")
						filter = MappingFilter.Boot;
					else
						return;

					MappingsManager.DeleteMapping((KeyMapping)currentRow.Tag, filter);
				}

				Populate();

			}

		}

		private void grdMappingsCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{

			// Don't display Delete button for cleared rows (as it has no effect)
			// Unfortunately, delete looks CRAP on Windows 2000
			// as it takes on the background colour ..??. TODO.

			if (e.ColumnIndex == 3 && e.RowIndex >= 0)
			{
				if (_clearedKeys.Contains(e.RowIndex))
				{
					e.PaintBackground(e.CellBounds, true);
					e.Handled = true;
				}

			}

		}


	}

}
