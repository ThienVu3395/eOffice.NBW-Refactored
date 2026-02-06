using System.Collections.Generic;

namespace Clone_KySoDienTu.Models.Dtos.QuanLyChamCong
{
    public sealed class PagedResult<T>
    {
        public IList<T> Items { get; set; }
        public int Total { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}