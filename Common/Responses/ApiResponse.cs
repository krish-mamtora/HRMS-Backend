namespace HRMS_Backend.Common.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }

        public int Code { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public List<string>? Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(
            T data,
            string message,
            int code)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Code = code,
                Data = data
            };
        }

        public static ApiResponse<T> FailResponse(
            string message,
            int code,
            List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Code = code,
                Errors = errors
            };
        }
    }
}