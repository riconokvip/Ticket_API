namespace Ticket.API.Models
{
    public class BaseResponse
    {
        /// <summary>
        /// Mã HttpStatusCode
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// Có lỗi hay không
        /// </summary>
        public bool isError => code != 200;

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public int error { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Mô tả lỗi
        /// </summary>
        public string errorMessage { get; set; }
    }

    public class BaseResponseWithNumber : BaseResponse
    {
        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public int? data { get; set; } = null;
    }

    public class BaseResponse<TEntity> : BaseResponse where TEntity : class
    {
        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public TEntity data { get; set; } = null;
    }

    public class BaseResponseWithPagination<TEntity> : BaseResponse where TEntity : class
    {
        /// <summary>
        /// Dữ liệu phân trang
        /// </summary>
        public PaginationResponse pagination { get; set; } = null;

        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public TEntity data { get; set; } = null;
    }
}
