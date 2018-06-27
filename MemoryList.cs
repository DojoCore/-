using System;
using System.Collections.Generic;
using System.Linq;

namespace pages
{
    public abstract class MemoryList 
    {
        protected List<MemBlock> blocks;
        protected int count;
        public bool IsFull { get; protected set; }

        public abstract MemBlock GetMem(out int? dispage, int[] cmds = null, int current = -1);
        protected abstract MemBlock GetFirstMem(out int? dispage, int[] cmd = null, int current = -1);

        protected virtual MemBlock GetEmptyMem()
        {
            if(IsFull) throw new Exception("Memory is full, no empty block");

            var newBlock = new MemBlock();
            blocks.Add(newBlock);
            IsFull = blocks.Count >= count;
            return newBlock;
        }

        public virtual MemBlock GetMemByPage(int? page)
        {
            if(page == null) return null;
            return blocks.Find(b => b.Page == page);
        }

        public virtual void Refresh() 
        {
            return;
        }

        public virtual void Show() 
        {
            Console.WriteLine("------------Memory");    
            for (int i = 0; i < count; i++)
            {
                try
                {
                    var b = blocks[i];
                    Console.WriteLine("Add:{0:0000}  Page:{1,-4}  Time:{2}", i, b.Page, b.Time);
                }
                catch
                {
                    Console.WriteLine("Add:{0:0000}  Page:{1,-4}  Time:{2}", i, "Null", "0");
                }
            }
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}