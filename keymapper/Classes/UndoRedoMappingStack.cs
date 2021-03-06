﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KeyMapper.Classes
{
    public class UndoRedoMappingStack
    {
        public UndoRedoMappingStack()
        {
            Mappings = new Stack<Collection<KeyMapping>>();
        }

        public Stack<Collection<KeyMapping>> Mappings { get; }

        public void Push(Collection<KeyMapping> mappings)
        {
            Mappings.Push(mappings);
        }

        public int Count => Mappings.Count;
    }
}
