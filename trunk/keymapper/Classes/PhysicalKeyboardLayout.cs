using System;
using System.Collections.Generic;
using System.Linq;

namespace KeyMapper.Classes
{
    public class PhysicalKeyboardLayout
    {
        // There are two main layouts - 'US' and 'European' (though many Europeans use the US layout..)

        // There is an extra one: the Punjabi keyboard has a European style enter key
        // and a US style left shift key 

        // Instance cache. Don't want to run through all that code every time.
        static readonly List<PhysicalKeyboardLayout> cachedInstances = new List<PhysicalKeyboardLayout>(0);

        // As we know the number of rows required, use them for initialisation.
        readonly List<KeyboardRow> functionKeys = new List<KeyboardRow>(1);
        readonly List<KeyboardRow> typewriterKeys = new List<KeyboardRow>(5);
        readonly List<KeyboardRow> numberpadKeys = new List<KeyboardRow>(5);
        readonly List<KeyboardRow> utilityKeys = new List<KeyboardRow>(1);
        readonly List<KeyboardRow> navigationKeys = new List<KeyboardRow>(2);
        readonly List<KeyboardRow> arrowKeys = new List<KeyboardRow>(2);

        KeyboardLayoutType layout;

        Boolean isMacKeyboard;

        public IEnumerable<KeyboardRow> FunctionKeys
        { get { return this.functionKeys; } }

        public IEnumerable<KeyboardRow> TypewriterKeys
        { get { return this.typewriterKeys; } }

        public IEnumerable<KeyboardRow> NumberPadKeys
        { get { return this.numberpadKeys; } }

        public IEnumerable<KeyboardRow> UtilityKeys
        { get { return this.utilityKeys; } }

        public IEnumerable<KeyboardRow> NavigationKeys
        { get { return this.navigationKeys; } }

        public IEnumerable<KeyboardRow> ArrowKeys
        { get { return this.arrowKeys; } }

        private PhysicalKeyboardLayout() { }

        public static PhysicalKeyboardLayout GetPhysicalLayout(KeyboardLayoutType layout, bool isMacKeyboard)
        {
            var existingInstance =
                cachedInstances.SingleOrDefault(l => l.layout == layout && l.isMacKeyboard == isMacKeyboard);

            if (existingInstance != null)
            {
                return existingInstance;
            }

            PhysicalKeyboardLayout nl = new PhysicalKeyboardLayout
                {
                    layout = layout, isMacKeyboard = isMacKeyboard
                };

           nl.Populate();

            cachedInstances.Add(nl);

            return nl;
        }

        public static int[] GetRowTerminators(KeyboardLayoutType layout)
        {
            // First five rows are the hashes of the terminator keys
            // (Backspace, Enter, R Shift, R Ctrl for Euro layout)

            switch (layout)
            {
                default: // includes case KeyboardLayoutType.US:
                    return new [] { 
                        KeyHasher.GetHashFromKeyData(14, 0), 
                        KeyHasher.GetHashFromKeyData(43, 0), 
                        KeyHasher.GetHashFromKeyData(28, 0), 
                        KeyHasher.GetHashFromKeyData(54, 224), 
                        KeyHasher.GetHashFromKeyData(29, 224) };

                case KeyboardLayoutType.Punjabi:
                case KeyboardLayoutType.European:

                    return new [] {
                        KeyHasher.GetHashFromKeyData(14, 0), 
                        KeyHasher.GetHashFromKeyData(28, 0), 
                        99999,  
                        KeyHasher.GetHashFromKeyData(54, 224), 
                        KeyHasher.GetHashFromKeyData(29, 224) };
                        
                       

            }

        }

        private void Populate()
        {
            PopulateFunctionKeys();
            PopulateArrowKeys();
            PopulateNavigationKeys();
            PopulateNumberpad();
            PopulateUtilityKeys();
            PopulateTypewriterKeys();
        }

        private void PopulateFunctionKeys()
        {
            // Structure: 
            // KeyboardLayoutElement(scancode, extended, button, horizontalstretch, verticalstretch, rightpadding)

            if (this.isMacKeyboard)
            {
                this.functionKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                           new[]{
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
                this.functionKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                           new[]{
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

            if (this.isMacKeyboard)
                this.utilityKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(new KeyboardLayoutElement[] { null })));
            else
            {
                // PrtSc, Scroll Lock, Pause/Break
                this.utilityKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                          new[]{
                                                              new KeyboardLayoutElement(55, 224, BlankButton.Blank, 0, 0, 0), 
                                                              new KeyboardLayoutElement(70, 0, BlankButton.Blank, 0, 0, 0), 
                                                              new KeyboardLayoutElement(29, 225, BlankButton.Blank, 0, 0, 0)})));
            }
        }

        private void PopulateArrowKeys()
        {
            // Up
            this.arrowKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                            new[]{
                                                null, 
                                                new KeyboardLayoutElement(72, 224, BlankButton.Blank, 0, 0, 0)})));

            // Left, down, right
            this.arrowKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                            new[]{
                                                new KeyboardLayoutElement(75, 224, BlankButton.Blank, 0, 0, 0),
                                                new KeyboardLayoutElement(80, 224, BlankButton.Blank, 0, 0, 0), 
                                                new KeyboardLayoutElement(77, 224, BlankButton.Blank, 0, 0, 0)})));
        }

        private void PopulateNavigationKeys()
        {
            // Insert, Home, Page Up..
            this.navigationKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                         new[]{
                                                             new KeyboardLayoutElement(82, 224, BlankButton.Blank, 0, 0, 0), 
                                                             new KeyboardLayoutElement(71, 224, BlankButton.Blank, 0, 0, 0), 
                                                             new KeyboardLayoutElement(73, 224, BlankButton.Blank, 0, 0, 0)})));
            // .. Delete, End, Page Down
            this.navigationKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                         new[]{
                                                             new KeyboardLayoutElement(83, 224, BlankButton.Blank, 0, 0, 0), 
                                                             new KeyboardLayoutElement(79, 224, BlankButton.Blank, 0, 0, 0), 
                                                             new KeyboardLayoutElement(81, 224, BlankButton.Blank, 0, 0, 0)})));

        }

        private void PopulateTypewriterKeys()
        {
            // Initialise rows.

            // The first row is common to both layouts.
            // Top left key (OEM3), 1! to =+, and Backspace.

            this.typewriterKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                         new[]{
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

            if (this.isMacKeyboard)
            {
                this.typewriterKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                             new[]{
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
                this.typewriterKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                             new[]{
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
            if (this.layout == KeyboardLayoutType.US)
            {
                // Tab, Q to ]}
                this.typewriterKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                             new[]{
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
                this.typewriterKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                             new[]{
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
            if (this.layout == KeyboardLayoutType.US)
            {
                // Caps Lock, gap, A to '", double-wide enter.
                this.typewriterKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                             new[]{
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
                this.typewriterKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                             new[]{
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
            if ((this.layout == KeyboardLayoutType.US) | (this.layout == KeyboardLayoutType.Punjabi))
            {

                // Left Shift, Z to /?, right shift
                this.typewriterKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                             new[]{
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
                this.typewriterKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                             new[]{
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
            this.numberpadKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                        new[]{
                                                            new KeyboardLayoutElement(69, 0, BlankButton.Blank, 0, 0, 0),
                                                            new KeyboardLayoutElement(53, 224, BlankButton.Blank, 0, 0, 0), 
                                                            new KeyboardLayoutElement(55, 0, BlankButton.Blank, 0, 0, 0), 
                                                            new KeyboardLayoutElement(74, 0, BlankButton.Blank, 0, 0, 0)})));

            // Second Row: 7 8 9 +
            this.numberpadKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                        new[]{
                                                            new KeyboardLayoutElement(71, 0, BlankButton.Blank, 0, 0, 0),
                                                            new KeyboardLayoutElement(72, 0, BlankButton.Blank, 0, 0, 0), 
                                                            new KeyboardLayoutElement(73, 0, BlankButton.Blank, 0, 0, 0), 
                                                            new KeyboardLayoutElement(78, 0, BlankButton.TallBlank, 0, 1, 0)})));

            // Third Row: 4 5 6
            this.numberpadKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                        new[]{
                                                            new KeyboardLayoutElement(75, 0, BlankButton.Blank, 0, 0, 0),
                                                            new KeyboardLayoutElement(76, 0, BlankButton.Blank, 0, 0, 0), 
                                                            new KeyboardLayoutElement(77, 0, BlankButton.Blank, 0, 0, 0)})));

            // Fourth Row: 1 2 3 Enter
            this.numberpadKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                        new[]{
                                                            new KeyboardLayoutElement(79, 0, BlankButton.Blank, 0, 0, 0),
                                                            new KeyboardLayoutElement(80, 0, BlankButton.Blank, 0, 0, 0), 
                                                            new KeyboardLayoutElement(81, 0, BlankButton.Blank, 0, 0, 0), 
                                                            new KeyboardLayoutElement(28, 224, BlankButton.TallBlank, 0, 1, 0)})));

            // Finally, 0 .
            this.numberpadKeys.Add(new KeyboardRow(new List<KeyboardLayoutElement>(
                                                        new[]{
                                                            new KeyboardLayoutElement(82, 0, BlankButton.DoubleWideBlank, 0, 0, 0),
                                                            new KeyboardLayoutElement(83, 0, BlankButton.Blank, 0, 0, 0)})));
        }


    }
}