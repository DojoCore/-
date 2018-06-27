using System;
using System.Collections.Generic;
using System.Linq;

namespace pages 
{
    public class LRUMemoryList : MemoryList
    {
        public LRUMemoryList(int size)
        {
            base.blocks = new List<MemBlock>();
            base.count = size;
        }

        public override MemBlock GetMem(out int? dispage, int[] cmds = null, int current = -1)
        {
            dispage = null;
            return IsFull ? GetFirstMem(out dispage, cmds, current) : GetEmptyMem();
        }

        protected override MemBlock GetFirstMem(out int? dispage, int[] cmd = null, int current = -1)
        {
            MemBlock block = blocks.Max();
            dispage = block.Page;
            return block;
        }
    }
}