namespace Ticket.API.Controllers
{
    public class BaseController : Controller
    {
        protected BaseResponse Success()
        {
            return new BaseResponse()
            {
                code = (int)HttpCodes.OK,
                error = (int)ErrorCodes.SUCCESS,
                message = ErrorCodes.SUCCESS.GetEnumMemberValue()
            };
        }

        protected BaseResponseWithNumber SuccessWithNumber(int data)
        {
            return new BaseResponseWithNumber()
            {
                code = (int)HttpCodes.OK,
                error = (int)ErrorCodes.SUCCESS,
                message = ErrorCodes.SUCCESS.GetEnumMemberValue(),
                data = data
            };
        }

        protected BaseResponse<T> Success<T>(T data) where T : class
        {
            return new BaseResponse<T>()
            {
                code = (int)HttpCodes.OK,
                error = (int)ErrorCodes.SUCCESS,
                message = ErrorCodes.SUCCESS.GetEnumMemberValue(),
                data = data
            };
        }

        protected BaseResponseWithPagination<T> SuccessWithPagination<T>(PaginationResponse pagination, T data) where T : class
        {
            return new BaseResponseWithPagination<T>()
            {
                code = (int)HttpCodes.OK,
                error = (int)ErrorCodes.SUCCESS,
                message = ErrorCodes.SUCCESS.GetEnumMemberValue(),
                data = data,
                pagination = pagination
            };
        }
    }
}
