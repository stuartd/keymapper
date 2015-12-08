using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using KeyMapper.Classes;

namespace KeyMapper.Forms
{
    public partial class MappingListForm : KMBaseForm
    {
        private readonly List<int> _clearedKeys = new List<int>();
        private readonly List<Key> _keylist = new List<Key>();
        private const int _minimumWidth = 300;

        /// <remarks>Leaving the Type column in even though there are only Boot mappings now, </remarks>
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
                Width = savedWidth;
        }

        private void MappingListFormClosing(object sender, FormClosingEventArgs e)
        {
            SaveUserSettings();
        }

        private void SaveUserSettings()
        {
            Properties.Settings userSettings = new Properties.Settings();
            userSettings.MappingListFormLocation = Location;
            userSettings.MappingListFormWidth = Width;
            userSettings.Save();
        }

        private void HandleMappingsChanged(object sender, EventArgs e)
        {
            Populate();
        }

        private void Populate()
        {
            // Form grabs focus from main form when repopulating. Check if we have focus now..
            bool hasFocus = grdMappings.ContainsFocus;

            // Using grdMappings.Rows.Clear() sometimes results in 
            // "Can't add rows where there are no columns" error,
            // resulting in an InvalidOperationException.

            for (int i = grdMappings.Rows.Count - 1; i >= 0; i--)
            {
                grdMappings.Rows.Remove(grdMappings.Rows[i]);
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
                return;
            }

            // Resize according to number of mappings
            int height = grdMappings.ColumnHeadersHeight
                + grdMappings.Rows.Cast<DataGridViewRow>().Sum(row => row.Height + row.DividerHeight);

            MinimumSize = new Size(0, 0);
            MaximumSize = new Size(0, 0);
            SetClientSizeCore(ClientSize.Width, height);
            MinimumSize = new Size(_minimumWidth, Size.Height);

            // If we didn't have form to start with, set focus back to main form.
            if (hasFocus == false)
                FormsManager.ActivateMainForm();


        }

        private void AddRowsToGrid()
        {
            AddRowsToGrid(MappingFilter.Boot);
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
                if (filter == MappingFilter.ClearedBoot)
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
                    MappingsManager.DeleteMapping((KeyMapping)currentRow.Tag);
                }
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
