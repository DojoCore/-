using System;

namespace pages 
{
    public class MemoryListFactory
    {
        public static MemoryList Create(int size, DispatchMode mode)
        {
            switch(mode)
            {
                case DispatchMode.FIFO: return new FIFOMemoryList(size);
                case DispatchMode.LRU: return new LRUMemoryList(size);
                case DispatchMode.OPT: return new OPTMemoryList(size);
                default: throw new ArgumentException();
            }
        }
    }
}