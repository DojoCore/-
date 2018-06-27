using System;
using System.Collections.Generic;
using System.Linq;

namespace pages
{
    /// <summary>
    /// 内存描述
    /// </summary>
    public class Memory
    {
        private List<MemBlock> _blocks;              //内存单元表
        private int _numBlock;                      //内存单元量
        private DispatchMode _mode;                 //当前调度模式
        public bool IsFull { get; private set; } = false;
                                                    //内存是否已满

        public Memory(int n, DispatchMode dispatch = DispatchMode.FIFO)
        {
            _blocks = new List<MemBlock>(n);
            _numBlock = n;
            _mode = dispatch;
        }

        /// <summary>
        /// 搜索装有目标页的内存单元，如果页表标记已在内存中，该过程
        /// 必然正常执行
        /// </summary>
        /// <param name="page">目标页号</param>
        /// <returns>装有目标页的内存单元</returns>
        public MemBlock GetMemByPage(int? page)
        {
            if (page != null)
            {
                return _blocks.Find(b => b.Page == page);
            }
            throw new Exception("no such mem");
        }

        /// <summary>
        /// 取内存单元用于调页
        /// </summary>
        /// <param name="dispage">被调出页页号</param>
        /// <param name="cmd">指令序列</param>
        /// <param name="current">当前指令指针</param>
        /// <returns>可用内存单元</returns>
        public MemBlock GetMem(out int? dispage, int[] cmd = null, int current = 0)
        {
            if (IsFull)
            {   //内存已满情况下要求调度
                if (_mode == DispatchMode.OPT) return GetFirstMem(out dispage, cmd, current);
                return GetFirstMem(out dispage);
            }
            else
            {   //内存未满，直接取用空闲内存
                dispage = null;
                return GetEmptyMem();
            }
        }

        /// <summary>
        /// 返回空闲的内存单元
        /// </summary>
        /// <returns></returns>
        private MemBlock GetEmptyMem()
        {
            var newBlock = new MemBlock();
            _blocks.Add(newBlock);
            if (_blocks.Count >= _numBlock)
            {   //装入新内存后如果内存已满，将标记标志位true
                IsFull = true;
            }
            return newBlock;
        }

        /// <summary>
        /// ！只能用于OPT模式，返回可用内存单元
        /// </summary>
        /// <param name="dispage">被调出页页号</param>
        /// <param name="cmd">指令序列</param>
        /// <param name="current">当前指令指针</param>
        /// <returns>可用内存单元</returns>
        private MemBlock GetFirstMem(out int? dispage, int[] cmd, int current)
        {
            if (_mode != DispatchMode.OPT) throw new Exception("该方法只能用于OPT模式！");
            List<int> position = new List<int>(new int[_numBlock]);
            for(int j = 0; j < _numBlock; j++)
            {
                for (int i = current + 1; i < cmd.Length; i++)
                {   //用一个游标变量，探测内存单元中的页第一次出现在后续代码的位置
                    if (_blocks[j].Page == cmd[i] / 10)
                    {
                        position[j] = i;
                        break;
                    }
                }
            }
            //取得最晚被调入的页所在的内存单元，调出，并返回内存块
            var index = position.FindIndex(p => p == position.Max());
            dispage = index;
            return _blocks[index];
        }

        /// <summary>
        /// 用于FIFO和LRU模式，返回可用内存单元
        /// </summary>
        /// <param name="dispage">被调出页的页号</param>
        /// <returns>可用内存单元</returns>
        private MemBlock GetFirstMem(out int? dispage)
        {
            MemBlock block = null;
            switch(_mode)
            {
                case DispatchMode.FIFO:
                    //FIFO状态下，新装入的内存永远在最后一位，最早装入的永远在最前位
                    block = _blocks[0];
                    dispage = block.Page;
                    _blocks.RemoveAt(0);
                    _blocks.Add(block);
                    return block;
                case DispatchMode.LRU:
                    //LRU模式下，取驻留时间最久的内存单元返回
                    block = _blocks.Max();
                    dispage = block.Page;
                    return block;
                default:
                    throw new Exception("无法确定的调度方法");
            }
        }

        /// <summary>
        /// 只用于LRU模式，刷新内存单元的驻留时间
        /// </summary>
        public void Refresh()
        {
            if (_mode != DispatchMode.LRU) return;
            foreach (var b in _blocks) { b.Time++; }
        }

#if DEBUG
        /// <summary>
        /// 打印当前的内存状态
        /// </summary>
        public void PrintState()
        {
            Console.WriteLine("------------Memory");
            for (int i = 0; i < _numBlock; i++)
            {
                try
                {
                    var b = _blocks[i];
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
#endif
    }
}
