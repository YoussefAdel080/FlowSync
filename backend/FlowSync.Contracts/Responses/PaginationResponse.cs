using FlowSync.Contracts.Requests;

namespace FlowSync.Contracts.Responses
{
    public class PaginationResponse<T>: BaseResponse<List<T>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
