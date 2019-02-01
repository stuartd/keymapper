using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace KeyMapper.Classes
{
    public class UndoRedoMappingStack
    {
        public UndoRedoMappingStack()
        {
            BootStack = new Stack<Collection<KeyMapping>>();
        }

        public Stack<Collection<KeyMapping>> BootStack { get; }

        public void Push(Collection<KeyMapping> bootmaps)
        {
            BootStack.Push(bootmaps);
        }

        public int Count => BootStack.Count;

        public void Clear()
        {
            BootStack.Clear();
        }
    }
}
