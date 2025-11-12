using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Response
{

    public static class ApiErrorResponses
    {
        public static ErrorResponse InvalidInput { get; } = new()
        {
            ErrorCode = "LMS401",
            Message = "Missing/invalid input"
        };

        public static ErrorResponse Unauthorized { get; } = new()
        {
            ErrorCode = "LMS401",
            Message = "Thiếu hoặc token thông báo không hợp lệ"
        };

        public static ErrorResponse Forbidden { get; } = new()
        {
            ErrorCode = "LMS403",
            Message = "Quyền truy cập bị từ chối"
        };

        public static ErrorResponse ResourceNotFound { get; } = new()
        {
            ErrorCode = "LMS404",
            Message = "Không tìm thấy tài nguyên"
        };

        public static ErrorResponse InternalError { get; } = new()
        {
            ErrorCode = "LMS501",
            Message = "Lỗi máy chủ nội bộ"
        };

        public static ErrorResponse ValidationError(string message) => new()
        {
            ErrorCode = "LMS401",
            Message = message
        };
    }

}
