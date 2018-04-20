using System.Collections.Generic;

namespace ITOrm.Core.PetaPoco
{
    // Results from paged request
    public class Page<T> where T : new()
    {
        public long CurrentPage { get; set; }
        public long TotalPages { get; set; }
        public long TotalItems { get; set; }
        public long ItemsPerPage { get; set; }
        public List<T> Items { get; set; }
    }
}
