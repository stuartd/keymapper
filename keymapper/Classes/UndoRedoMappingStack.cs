using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KeyMapper.Classes
{
    public class UndoRedoMappingStack
    {
        public UndoRedoMappingStack()
        {
            BootStack = new Stack<Collection<KeyMapping>>();
            UserStack = new Stack<Collection<KeyMapping>>();
        }

        public Stack<Collection<KeyMapping>> UserStack { get; }

        public Stack<Collection<KeyMapping>> BootStack { get; }

        public void Push(Collection<KeyMapping> usermaps, Collection<KeyMapping> bootmaps)
        {
            UserStack.Push(usermaps);
            BootStack.Push(bootmaps);
        }

        public int Count => UserStack.Count;

		public void Clear()
        {
            UserStack.Clear();
            BootStack.Clear();
        }
    }
}


