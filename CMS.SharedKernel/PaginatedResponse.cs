using System.Collections.Generic;

namespace CMS.SharedKernel
{
    public record PaginatedResponse<T>(IEnumerable<T> Data, long TotalSize, int Page = 1, int SizePerPage = 10)
    {
        public PaginatedResponse() : this(new List<T>(), 0, 0, 0) { }
    }
}
