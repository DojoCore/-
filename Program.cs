using System;
using System.Collections.Generic;

namespace pages
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] cmd = Generate(320);              //指令序列
            double[,] results = new double[3, 29];  //存储结果
            
            Console.Write("======指令序列======");
            for (int i = 0; i < cmd.Length; i++)
            {
                if(i % 8 == 0) { Console.WriteLine(); }
                Console.Write("{0, 5}", cmd[i]);
            }
            Console.WriteLine();

            //Console.WriteLine("2153434 乔磊");
            for (int k = 4; k <= 32; k++)
            {
                results[0, k - 4] = RunTest(k, cmd, DispatchMode.FIFO);  //FIFO模式调度
                results[1, k - 4] = RunTest(k, cmd, DispatchMode.LRU);   //LRU模式调度
                results[2, k - 4] = RunTest(k, cmd, DispatchMode.OPT);   //OPT模式调度
            }

            Console.WriteLine("\n缺页率统计结果：");
            Console.WriteLine("{0,-4}{1,8}{2,8}{3,8}", "内存块数", "FIFO", "LRU", "OPT");
            for(int i = 0; i < 29; i++)
            {
                Console.WriteLine("{0,4}\t   {1:####.00}%  {2:####.00}%  {3:####.00}%", 
                    i + 4, results[0, i], results[1, i], results[2, i]); //格式化输出
            }
            Console.ReadKey();
        }

        /// <summary>
        /// 生成目标数量的指令序列
        /// </summary>
        /// <param name="v">指令数量</param>
        /// <returns>指令序列</returns>
        private static int[] Generate(int v)
        {
            var r = new Random();
            var cmd = new int[v];
            var m = v;
            for(int i = 0; i < v; i++)
            {
                switch (i % 4)
                {   //按 顺序，后半，顺序，前半 序列生成
                    case 0: m = r.Next(0, m); break;
                    case 1:
                    case 3: m++; break;
                    case 2: m = r.Next(m + 1, v); break;
                }
                if (m >= v) m = v;
                cmd[i] = m;
            }
            return cmd;
        }

        /// <summary>
        /// 使用给定指令序列和内存块数进行制定模式的测试
        /// </summary>
        /// <param name="mc">内存块数</param>
        /// <param name="cmd">指令序列</param>
        /// <param name="mode">调度模式</param>
        /// <returns>缺页率</returns>
        public static double RunTest(int mc, int[] cmd, DispatchMode mode)
        {
            List<PageItem> table = new List<PageItem>();    //页表
            MemoryList memory = MemoryListFactory.Create(mc, mode);   //初始化内存和调度模式
            for(int i = 0; i < cmd.Length/10;i++)
            {
                table.Add(new PageItem() { Page = i });
            }

            var accCount = 0;   //命中次数
            var failCount = 0;  //缺页次数
            for(int i = 0; i < cmd.Length; i++)
            {
                var next = table[cmd[i] / 10];
                if(next.InMemory)
                {   //指令在内存中，命中
                    accCount++;
                    if(mode == DispatchMode.LRU)
                    {   //若使用LRU调度模式，清空内存驻留时间
                        var mem = memory.GetMemByPage(next.Page);
                        mem.Time = 0;
                    }
                }
                else
                {   //指令不在内存中，缺页中断，调页
                    failCount++;
                    MemBlock mem = null;    //可用内存单元
                    int? dis = null;        //移出页页号
                    mem = memory.GetMem(out dis, cmd, i);
                    next.InMemory = true;
                    mem.Page = next.Page;   //目标页进入内存
                    mem.Time = 0;           //清空内存驻留时间
                    if (dis != null)
                    {   //若有页被调出，将其在页表中的标志位设置false
                        table[(int)dis].InMemory = false;
                    }
                }
                memory.Refresh();   //刷新内存状态

#if DEBUG
                Console.WriteLine("当前指令 -> {0}", cmd[i]);
                memory.Show();
#endif
            }
            return failCount * 100.0 / (failCount + accCount);
        }
    } 
}
