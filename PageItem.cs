namespace pages
{
    /// <summary>
    /// 页表项
    /// </summary>
    public class PageItem
    {
        public int? Page { get; set; } = null;  //页码
        
        public int Visit { get; set; } = 0;     //访问次数
        
        public bool InMemory { get; set; } = false; //是否已在内存中
    }
}
