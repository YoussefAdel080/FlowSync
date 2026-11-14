using FlowSync.Contracts.Enums;

namespace FlowSync.Contracts.Requests
{
    public class PaginationRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public SortEnum Sort { get; set; } = SortEnum.Ascending;
    }
}
