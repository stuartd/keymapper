using System.Drawing;
using System.Windows.Forms;

namespace KeyMapper.Classes
{
	/// <summary>
	///  <remarks>This decorator class owes a great deal to http://blogs.msdn.com/jfoscoding/articles/456968.aspx</remarks>
	/// </summary>
	public static class ComboItemSeparator
	{
	    static ComboItemSeparator()
	    {
	        VerticalItemPadding = 4;
	        SeparatorHeight = 3;
	    }

	    public static int SeparatorHeight { get; private set; }

	    public static int VerticalItemPadding { get; private set; }

	    internal class SeparatorItem
		{
			private readonly string _name;
			
            public SeparatorItem(string name)
			{
			    this._name = name;
			}

			public override string ToString()
			{
				if (this._name != null)
				{
					return this._name;
				}
				return base.ToString();
			}

		}

		internal static void MeasureComboItem(object sender, MeasureItemEventArgs e)
		{
			if (e.Index == -1)
			{
			    return;
			}

			ComboBox combo = sender as ComboBox;
			if (combo != null)
			{
				object comboBoxItem = combo.Items[e.Index];

				Size textSize = TextRenderer.MeasureText(comboBoxItem.ToString(), combo.Font);

				e.ItemHeight = textSize.Height + VerticalItemPadding;
				e.ItemWidth = textSize.Width;

				if (comboBoxItem is SeparatorItem)
				{
					// one white line, one dark, one white.
					e.ItemHeight += SeparatorHeight;
				}
			}
		}

		internal static void DrawComboItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index == -1)
			{ return; }

			ComboBox combo = sender as ComboBox;
			if (combo != null)
			{
				object comboBoxItem = combo.Items[e.Index];

				e.DrawBackground();
				e.DrawFocusRectangle();

				bool isSeparatorItem = (comboBoxItem is SeparatorItem);

				Rectangle bounds = e.Bounds;
				// adjust the bounds so that the text is centered properly.
				// if we're a separator, remove the separator height

				if (isSeparatorItem && (e.State & DrawItemState.ComboBoxEdit) != DrawItemState.ComboBoxEdit)
				{
					bounds.Height -= SeparatorHeight;
				}

				TextRenderer.DrawText(e.Graphics, comboBoxItem.ToString(), combo.Font,
					bounds, e.ForeColor, TextFormatFlags.Left & TextFormatFlags.VerticalCenter);

				// draw the separator line
				if (isSeparatorItem && ((e.State & DrawItemState.ComboBoxEdit) != DrawItemState.ComboBoxEdit))
				{
					Rectangle separatorRect = new Rectangle(e.Bounds.Left, e.Bounds.Bottom - SeparatorHeight, e.Bounds.Width, SeparatorHeight);

					// fill the background behind the separator
					using (Brush br = new SolidBrush(combo.BackColor))
					{
						e.Graphics.FillRectangle(br, separatorRect);
					}
					e.Graphics.DrawLine(SystemPens.ControlText, separatorRect.Left + 2, separatorRect.Top + 1,
						separatorRect.Right - 2, separatorRect.Top + 1);

				}
			}

		}
	}
}

