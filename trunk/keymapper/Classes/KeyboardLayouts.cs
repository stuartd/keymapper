using System;
using System.Collections.Generic;

namespace RoseHillSolutions.KeyMapper
{

	public enum KeyboardLayoutType
	{
		// These are used in keyboards.xml as the layout IDs
		// US = 0, European = 1, Punjabi = 2

		US = 0, European = 1, Punjabi = 2
	}

	public class PhysicalKeyboardLayout
	{

		// There are two main layouts - 'US' and 'European' (though many Europeans use the US layout..)

		// There is an extra one: the Punjabi keyboard has a European style enter key
		// and a US style left shift key 

		#region Fields and Properties

		// Instance cache. Don't want to run through all that code every time.
		static List<PhysicalKeyboardLayout> _instances = new List<PhysicalKeyboardLayout>(0);

		List<KeyboardRow> _functions = new List<KeyboardRow>(1);
		List<KeyboardRow> _typewriter = new List<KeyboardRow>(5);
		List<KeyboardRow> _numberpad = new List<KeyboardRow>(5);
		List<KeyboardRow> _utilities = new List<KeyboardRow>(1);
		List<KeyboardRow> _navigation = new List<KeyboardRow>(2);
		List<KeyboardRow> _arrows = new List<KeyboardRow>(2);

		KeyboardLayoutType _layout;

		Boolean _isMacKeyboard = false;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public List<KeyboardRow> Functions
		{ get { return _functions; } }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public List<KeyboardRow> Typewriter
		{ get { return _typewriter; } }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public List<KeyboardRow> NumberPad
		{ get { return _numberpad; } }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public List<KeyboardRow> UtilityKeys
		{ get { return _utilities; } }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public List<KeyboardRow> Navigation
		{ get { return _navigation; } }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public List<KeyboardRow> Arrows
		{ get { return _arrows; } }

		#endregion

		private PhysicalKeyboardLayout() { }

		public static PhysicalKeyboardLayout GetPhysicalLayout(KeyboardLayoutType layout, bool isMacKeyboard)
		{

			// Look for an instance, if any. 
			foreach (PhysicalKeyboardLayout cachedLayouts in _instances)
			{
				if (cachedLayouts._layout == layout && cachedLayouts._isMacKeyboard == isMacKeyboard)
					return cachedLayouts;
			}

			PhysicalKeyboardLayout nl = new PhysicalKeyboardLayout();

			// Assign params to new instance.
			nl._layout = layout;
			nl._isMacKeyboard = isMacKeyboard;

			nl.PopulateFunctionKeys();
			nl.PopulateArrowKeys();
			nl.PopulateNavigationKeys();
			nl.PopulateNumberpad();
			nl.PopulateUtilityKeys();

			nl.PopulateTypewriterKeys();

			_instances.Add(nl);

			return nl;
		}

		public static int[] GetRowTerminators(KeyboardLayoutType layout)
		{
			// First five rows are the hashes of the terminator keys
			// (Backspace, Enter, R Shift, R Ctrl for Euro layout)

			switch (layout)
			{
				case KeyboardLayoutType.US:
					return new int[] { 28, 86, 56, 109, 59 };

				case KeyboardLayoutType.Punjabi:
				case KeyboardLayoutType.European:
					return new int[] { 28, 56, 999, 109, 59 }; // Nothing will match 999

			}

			return null;
		}

		private void PopulateFunctionKeys()
		{
			// Structure: 
			// KeyboardLayoutElement(scancode, extended, button, horizontalstretch, verticalstretch, rightpadding)

			if (_isMacKeyboard)
			{
				_functions.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
				new KeyboardLayoutElement[]{
					new KeyboardLayoutElement(1, 0, BlankButton.Blank, 0, 0, 4),  
					new KeyboardLayoutElement(59, 0, BlankButton.Blank, 0, 0, 0), 
					new KeyboardLayoutElement(60, 0, BlankButton.Blank, 0, 0, 0),
					new KeyboardLayoutElement(61, 0, BlankButton.Blank, 0, 0, 0),
					new KeyboardLayoutElement(62, 0, BlankButton.Blank, 0, 0, 1),
					new KeyboardLayoutElement(63, 0, BlankButton.Blank, 0, 0, 0),
					new KeyboardLayoutElement(64, 0, BlankButton.Blank, 0, 0, 0),
					new KeyboardLayoutElement(65, 0, BlankButton.Blank, 0, 0, 0),
					new KeyboardLayoutElement(66, 0, BlankButton.Blank, 0, 0, 1),
					new KeyboardLayoutElement(67, 0, BlankButton.Blank, 0, 0, 0),
					new KeyboardLayoutElement(68, 0, BlankButton.Blank, 0, 0, 0),
					new KeyboardLayoutElement(87, 0, BlankButton.Blank, 0, 0, 0),
					new KeyboardLayoutElement(88, 0, BlankButton.Blank, 0, 0, 0)})));


			}


			else
			{
				_functions.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
				new KeyboardLayoutElement[]{
					new KeyboardLayoutElement(1, 0, BlankButton.Blank, 0, 0, 4),  
					new KeyboardLayoutElement(59, 0, BlankButton.Blank, 0, 0, 0), 
					new KeyboardLayoutElement(60, 0, BlankButton.Blank, 0, 0, 0),
					new KeyboardLayoutElement(61, 0, BlankButton.Blank, 0, 0, 0),
					new KeyboardLayoutElement(62, 0, BlankButton.Blank, 0, 0, 1),
					new KeyboardLayoutElement(63, 0, BlankButton.Blank, 0, 0, 0),
					new KeyboardLayoutElement(64, 0, BlankButton.Blank, 0, 0, 0),
					new KeyboardLayoutElement(65, 0, BlankButton.Blank, 0, 0, 0),
					new KeyboardLayoutElement(66, 0, BlankButton.Blank, 0, 0, 1),
					new KeyboardLayoutElement(67, 0, BlankButton.Blank, 0, 0, 0),
					new KeyboardLayoutElement(68, 0, BlankButton.Blank, 0, 0, 0),
					new KeyboardLayoutElement(87, 0, BlankButton.Blank, 0, 0, 0),
					new KeyboardLayoutElement(88, 0, BlankButton.Blank, 0, 0, 0)})));
			}
		}

		private void PopulateUtilityKeys()
		{

			if (_isMacKeyboard)
				_utilities.Add(new KeyboardRow(new List<KeyboardLayoutElement>(new KeyboardLayoutElement[] { null })));
			else
			{
				// PrtSc, Scroll Lock, Pause/Break
				_utilities.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
				new KeyboardLayoutElement[]{
				new KeyboardLayoutElement(55, 224, BlankButton.Blank, 0, 0, 0), 
				new KeyboardLayoutElement(70, 0, BlankButton.Blank, 0, 0, 0), 
				new KeyboardLayoutElement(69, 224, BlankButton.Blank, 0, 0, 0)})));
			}
		}

		private void PopulateArrowKeys()
		{
			// Up
			_arrows.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
				new KeyboardLayoutElement[]{
				null, 
			 	new KeyboardLayoutElement(72, 224, BlankButton.Blank, 0, 0, 0)})));

			// Left, down, right
			_arrows.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
				new KeyboardLayoutElement[]{
				new KeyboardLayoutElement(75, 224, BlankButton.Blank, 0, 0, 0),
				new KeyboardLayoutElement(80, 224, BlankButton.Blank, 0, 0, 0), 
 				new KeyboardLayoutElement(77, 224, BlankButton.Blank, 0, 0, 0)})));
		}

		private void PopulateNavigationKeys()
		{
			// Insert, Home, Page Up..
			Navigation.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
				new KeyboardLayoutElement[]{
				new KeyboardLayoutElement(82, 224, BlankButton.Blank, 0, 0, 0), 
				new KeyboardLayoutElement(71, 224, BlankButton.Blank, 0, 0, 0), 
				new KeyboardLayoutElement(73, 224, BlankButton.Blank, 0, 0, 0)})));
			// .. Delete, End, Page Down
			Navigation.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
				new KeyboardLayoutElement[]{
				new KeyboardLayoutElement(83, 224, BlankButton.Blank, 0, 0, 0), 
				new KeyboardLayoutElement(79, 224, BlankButton.Blank, 0, 0, 0), 
				new KeyboardLayoutElement(81, 224, BlankButton.Blank, 0, 0, 0)})));

		}

		private void PopulateTypewriterKeys()
		{
			// Initialise rows.

			// The first row is common to both layouts.
			// Top left key (OEM3), 1! to =+, and Backspace.

			Typewriter.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
				new KeyboardLayoutElement[]{
                new KeyboardLayoutElement(41, 0, BlankButton.Blank, 0, 0, 0),  
                new KeyboardLayoutElement(2, 0, BlankButton.Blank, 0, 0, 0), 
                new KeyboardLayoutElement(3, 0, BlankButton.Blank, 0, 0, 0),
                new KeyboardLayoutElement(4, 0, BlankButton.Blank, 0, 0, 0),
                new KeyboardLayoutElement(5, 0, BlankButton.Blank, 0, 0, 0),
                new KeyboardLayoutElement(6, 0, BlankButton.Blank, 0, 0, 0),
                new KeyboardLayoutElement(7, 0, BlankButton.Blank, 0, 0, 0),
                new KeyboardLayoutElement(8, 0, BlankButton.Blank, 0, 0, 0),
                new KeyboardLayoutElement(9, 0, BlankButton.Blank, 0, 0, 0),
                new KeyboardLayoutElement(10, 0, BlankButton.Blank, 0, 0, 0),
                new KeyboardLayoutElement(11, 0, BlankButton.Blank, 0, 0, 0),
                new KeyboardLayoutElement(12, 0, BlankButton.Blank, 0, 0, 0),
                new KeyboardLayoutElement(13, 0, BlankButton.Blank, 0, 0, 0),
                new KeyboardLayoutElement(14, 0, BlankButton.MediumWideBlank, 0, 0, 0)})));

			GetSecondRow();
			GetThirdRow();
			GetFourthRow();


			// Final row is same for all layouts except Macs

			if (_isMacKeyboard)
			{
				Typewriter.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
				new KeyboardLayoutElement[]{
                    new KeyboardLayoutElement(29, 0, BlankButton.MediumWideBlank, 0, 0, 0), 
                    new KeyboardLayoutElement(56, 0, BlankButton.MediumWideBlank, 0, 0, 0), 
					new KeyboardLayoutElement(91, 224, BlankButton.MediumWideBlank, 5, 0, 1), 
                    new KeyboardLayoutElement(57, 0, BlankButton.QuadrupleWideBlank, 20, 0, 0), 
                    new KeyboardLayoutElement(92, 224, BlankButton.MediumWideBlank, 5, 0, 0), 
					new KeyboardLayoutElement(56, 224, BlankButton.MediumWideBlank, 0, 0, 1), 
                    new KeyboardLayoutElement(29, 224, BlankButton.MediumWideBlank, 0, 0, 0)})));

			}
			else
			{
				Typewriter.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
	new KeyboardLayoutElement[]{
                    new KeyboardLayoutElement(29, 0, BlankButton.MediumWideBlank, 0, 0, 1),
                    new KeyboardLayoutElement(91, 224, BlankButton.MediumWideBlank, 0, 0, 1), 
                    new KeyboardLayoutElement(56, 0, BlankButton.MediumWideBlank, 0, 0, 0), 
                    new KeyboardLayoutElement(57, 0, BlankButton.QuadrupleWideBlank, 3, 0, 0), 
                    new KeyboardLayoutElement(56, 224, BlankButton.MediumWideBlank, 0, 0, 1), 
                    new KeyboardLayoutElement(92, 224, BlankButton.MediumWideBlank, 0, 0, 1), 
                    new KeyboardLayoutElement(93, 224, BlankButton.MediumWideBlank, 0, 0, 1), 
                    new KeyboardLayoutElement(29, 224, BlankButton.MediumWideBlank, 0, 0, 0)})));
			}
		}


		private void GetSecondRow()
		{
			if (_layout == KeyboardLayoutType.US)
			{
				// Tab, Q to ]}
				Typewriter.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
					new KeyboardLayoutElement[]{
                    new KeyboardLayoutElement(15, 0, BlankButton.MediumWideBlank, 0, 0, 0), 
                    new KeyboardLayoutElement(16, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(17, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(18, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(19, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(20, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(21, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(22, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(23, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(24, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(25, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(26, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(27, 0, BlankButton.Blank, 0, 0, 2), 
                    new KeyboardLayoutElement(43, 0, BlankButton.Blank, 0, 0, 0)})));

			}
			else
			{
				// Tab, Q to ]}, Enter - includes Punjabi layout.
				Typewriter.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
					new KeyboardLayoutElement[]{
                    new KeyboardLayoutElement(15, 0, BlankButton.MediumWideBlank, 0, 0, 0), 
                    new KeyboardLayoutElement(16, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(17, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(18, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(19, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(20, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(21, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(22, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(23, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(24, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(25, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(26, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(27, 0, BlankButton.Blank, 0, 0, 2), 
                    new KeyboardLayoutElement(28, 0, BlankButton.TallBlank, 0, 1, 0)})));

			}
		}

		private void GetThirdRow()
		{
			if (_layout == KeyboardLayoutType.US)
			{
				// Caps Lock, gap, A to '", double-wide enter.
				Typewriter.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
				new KeyboardLayoutElement[]{
                    new KeyboardLayoutElement(58, 0, BlankButton.MediumWideBlank, 1, 0, 1), 
                    new KeyboardLayoutElement(30, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(31, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(32, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(33, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(34, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(35, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(36, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(37, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(38, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(39, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(40, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(28, 0, BlankButton.DoubleWideBlank, 0, 0, 0)})));
			}
			else
			{
				// Caps Lock, gap, A to '@, key 43.
				Typewriter.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
					new KeyboardLayoutElement[]{
                    new KeyboardLayoutElement(58, 0, BlankButton.MediumWideBlank, 1, 0, 1), 
                    new KeyboardLayoutElement(30, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(31, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(32, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(33, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(34, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(35, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(36, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(37, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(38, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(39, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(40, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(43, 0, BlankButton.Blank, 0, 0, 0)})));
			}

		}

		private void GetFourthRow()
		{
			if ((_layout == KeyboardLayoutType.US) | (_layout == KeyboardLayoutType.Punjabi))
			{

				// Left Shift, Z to /?, right shift
				Typewriter.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
					new KeyboardLayoutElement[]{
                    new KeyboardLayoutElement(42, 0, BlankButton.DoubleWideBlank, 1, 0, 0), 
                    new KeyboardLayoutElement(44, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(45, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(46, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(47, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(48, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(49, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(50, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(51, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(52, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(53, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(54, 224, BlankButton.TripleWideBlank, -6, 0, 0)})));
			}
			else
			{
				// Left Shift, key 43, Z to /?, right shift
				Typewriter.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
					new KeyboardLayoutElement[]{
                    new KeyboardLayoutElement(42, 0, BlankButton.Blank, 0, 0, 0),
                    new KeyboardLayoutElement(86, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(44, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(45, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(46, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(47, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(48, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(49, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(50, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(51, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(52, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(53, 0, BlankButton.Blank, 0, 0, 0), 
                    new KeyboardLayoutElement(54, 224, BlankButton.TripleWideBlank, -6, 0, 0)})));
			}

		}


		private void PopulateNumberpad()
		{

			// Now for the Numberpad: 
			// First row: NumLock / * and -
			NumberPad.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
				new KeyboardLayoutElement[]{
				new KeyboardLayoutElement(69, 0, BlankButton.Blank, 0, 0, 0),
				new KeyboardLayoutElement(53, 224, BlankButton.Blank, 0, 0, 0), 
				new KeyboardLayoutElement(55, 0, BlankButton.Blank, 0, 0, 0), 
				new KeyboardLayoutElement(74, 0, BlankButton.Blank, 0, 0, 0)})));

			// Second Row: 7 8 9 +
			NumberPad.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
				new KeyboardLayoutElement[]{
				new KeyboardLayoutElement(71, 0, BlankButton.Blank, 0, 0, 0),
				new KeyboardLayoutElement(72, 0, BlankButton.Blank, 0, 0, 0), 
				new KeyboardLayoutElement(73, 0, BlankButton.Blank, 0, 0, 0), 
				new KeyboardLayoutElement(78, 0, BlankButton.TallBlank, 0, 1, 0)})));

			// Third Row: 4 5 6
			NumberPad.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
				new KeyboardLayoutElement[]{
				new KeyboardLayoutElement(75, 0, BlankButton.Blank, 0, 0, 0),
				new KeyboardLayoutElement(76, 0, BlankButton.Blank, 0, 0, 0), 
				new KeyboardLayoutElement(77, 0, BlankButton.Blank, 0, 0, 0)})));

			// Fourth Row: 1 2 3 Enter
			NumberPad.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
				new KeyboardLayoutElement[]{
				new KeyboardLayoutElement(79, 0, BlankButton.Blank, 0, 0, 0),
				new KeyboardLayoutElement(80, 0, BlankButton.Blank, 0, 0, 0), 
				new KeyboardLayoutElement(81, 0, BlankButton.Blank, 0, 0, 0), 
				new KeyboardLayoutElement(28, 224, BlankButton.TallBlank, 0, 1, 0)})));

			// Finally, 0 .
			NumberPad.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
				new KeyboardLayoutElement[]{
				new KeyboardLayoutElement(82, 0, BlankButton.DoubleWideBlank, 0, 0, 0),
				new KeyboardLayoutElement(83, 0, BlankButton.Blank, 0, 0, 0)})));
		}


	}

	public class KeyboardLayoutElement
	{
		int _scancode;
		int _extended;
		BlankButton _button;
		int _horizontalstretch;
		int _verticalstretch;
		int _rightpadding;

		public int Scancode
		{
			get { return _scancode; }
		}

		public int Extended
		{
			get { return _extended; }
		}

		public BlankButton Button
		{
			get { return _button; }
		}

		public int HorizontalStretch
		{
			get { return _horizontalstretch; }
		}

		public int VerticalStretch
		{
			get { return _verticalstretch; }
		}

		public int RightPadding
		{
			get { return _rightpadding; }
		}


		public KeyboardLayoutElement(int scancode, int extended, BlankButton button,
			int horizontalStretch, int verticalStretch, int rightPadding)
		{
			_scancode = scancode;
			_extended = extended;
			_button = button;
			_horizontalstretch = horizontalStretch;
			_verticalstretch = verticalStretch;
			_rightpadding = rightPadding;
		}

	}

	public class KeyboardRow
	{
		List<KeyboardLayoutElement> _keys;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public List<KeyboardLayoutElement> Keys
		{
			get { return _keys; }
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public KeyboardRow(List<KeyboardLayoutElement> keys)
		{
			_keys = keys;
		}

	}

}
