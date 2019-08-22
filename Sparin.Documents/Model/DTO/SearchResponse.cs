using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparin.Documents.Model.DTO
{
    public class SearchResponse<T>
    {
        public long TotalItems { get; set; }
        public long TotalPages { get; set; }
        public long CurrentPage { get; set; }
        public IEnumerable<T> Items { get; set; }

        public SearchResponse(long totalItems, int page, int limit, IEnumerable<T> items)
        {
            this.TotalItems = totalItems;
            this.TotalPages = totalItems / limit;
            this.TotalPages += totalItems % limit > 0 ? 1 : 0;
            this.CurrentPage = page;
            this.Items = items;
        }
    }
}
