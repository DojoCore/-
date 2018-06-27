using System;
using System.Collections.Generic;
using System.Linq;

namespace pages 
{
    public class OPTMemoryList : MemoryList 
    {
        public OPTMemoryList(int size)
        {
            base.blocks = new List<MemBlock>();
            base.count = size;
        }

        public override MemBlock GetMem(out int? dispage, int[] cmds = null, int current = -1)
        {
            if(cmds == null || current < 0) throw new ArgumentException();

            dispage = null;
            return IsFull ? GetFirstMem(out dispage, cmds, current) : GetEmptyMem();
        }

        protected override MemBlock GetFirstMem(out int? dispage, int[] cmd = null, int current = -1)
        {
            List<int> position = new List<int>(new int[count]);
            for(int j = 0; j < count; j++)
            {
                for (int i = current + 1; i < cmd.Length; i++)
                {   //用一个游标变量，探测内存单元中的页第一次出现在后续代码的位置
                    if (blocks[j].Page == cmd[i] / 10)
                    {
                        position[j] = i;
                        break;
                    }
                }
            }
            //取得最晚被调入的页所在的内存单元，调出，并返回内存块
            var index = position.FindIndex(p => p == position.Max());
            dispage = index;
            return blocks[index];
        }
    }
}