using System;
using System.Collections.Generic;
using System.Linq;

namespace pages
{
    public class FIFOMemoryList : MemoryList
    {
        public FIFOMemoryList(int size)
        {
            base.blocks = new List<MemBlock>();
            base.count = size;
        }

        public override MemBlock GetMem(out int? dispage, int[] cmds = null, int current = 0)
        {
            dispage = null;
            return IsFull ? GetFirstMem(out dispage, cmds, current) : GetEmptyMem();
        }

        protected override MemBlock GetFirstMem(out int? dispage, int[] cmds = null, int current = 0)
        {
            //FIFO状态下，新装入的内存永远在最后一位，最早装入的永远在最前位
            MemBlock block = blocks[0];
            dispage = block.Page;
            blocks.RemoveAt(0);
            blocks.Add(block);
            return block;
        }
    }
}