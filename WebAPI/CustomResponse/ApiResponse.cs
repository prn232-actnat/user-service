namespace WebAPI.CustomResponse
{
    public class ApiResponse<T>
    {
        public ResponseCode Code { get; set; }
        public string Status => "success";
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T data, ResponseMessage msg = ResponseMessage.RequestSuccessful)
        {
            return new ApiResponse<T>
            {
                Code = ResponseCode.Success,
                Message = msg.GetMessage(),
                Data = data
            };
        }
    }
}
