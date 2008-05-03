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
		private int _minimumWidth = 300;

		public MappingListForm()
		{
			//TODO: Look into changing the column header colours as they are much too dark on XP without themes or w2k

			InitializeComponent();
			Populate();
			MappingsManager.MappingsChanged += HandleMappingsChanged;
		}

		public void LoadUserSettings()
		{

			Properties.Settings userSettings = new Properties.Settings();

			int savedWidth = userSettings.MappingListFormWidth;

			if (savedWidth > _minimumWidth)
				this.Width = savedWidth;
		}

		private void MappingListFormClosing(object sender, FormClosingEventArgs e)
		{
			SaveUserSettings();
		}

		private void SaveUserSettings()
		{
			Properties.Settings userSettings = new Properties.Settings();
			userSettings.MappingListFormLocation = this.Location;
			userSettings.MappingListFormWidth = this.Width;
			userSettings.Save();
		}

		void HandleMappingsChanged(object sender, EventArgs e)
		{
			Populate();
		}

		private void Populate()
		{

		
			// Using grdMappings.Rows.Clear() I sometimes get 
			// "Can't add rows where there are no columns" error.
			// resulting in an InvalidOperationException.

			for (int i = grdMappings.Rows.Count - 1; i >= 0 ; i--)
			{
				grdMappings.Rows.Remove(grdMappings.Rows[i]) ;
			}
			
			_clearedKeys.Clear();
			_keylist.Clear();
			
			try
			{
				AddRowsToGrid();
			}
			catch (InvalidOperationException)
			{
				Console.WriteLine("Unexpected return of the AddRowsToGrid bug!");
			}

			// Resize according to number of mappings
			int height = grdMappings.ColumnHeadersHeight;

			foreach (DataGridViewRow row in grdMappings.Rows)
				height += row.Height + row.DividerHeight;

			this.MinimumSize = new Size(0, 0);
			this.MaximumSize = new Size(0, 0);
			this.SetClientSizeCore(this.ClientSize.Width, height);
			this.MinimumSize = new Size(_minimumWidth, this.Size.Height);
		// 	this.MaximumSize = new Size(0, this.Size.Height);

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
					{
						// Don't add an entry for a cleared key which has been remapped.
						break;
					}
						
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
