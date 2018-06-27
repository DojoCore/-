using System;

namespace pages
{
    /// <summary>
    /// 内存单元
    /// </summary>
    public class MemBlock : IComparable<MemBlock>
    {
        public int Time { get; set; } = 0;      //在内存中存在的时间

        public int? Page { get; set; } = null;  //在内存中的页

        public int CompareTo(MemBlock other)
        {
            return Time.CompareTo(other.Time);
        }
    }
}
