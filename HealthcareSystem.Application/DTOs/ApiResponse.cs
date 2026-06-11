namespace HealthcareSystem.Application.DTOs
{
    // Standardized API response format for all endpoints
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        // Success Response Factory
        public static ApiResponse<T> Ok(T data, string message = "Success")
            => new ApiResponse<T> { Success = true, Data = data, Message = message };

        // Error Response Factory
        public static ApiResponse<T> Fail(string message, List<string>? errors = null)
            => new ApiResponse<T> { Success = false, Message = message, Errors = errors };
    }
}