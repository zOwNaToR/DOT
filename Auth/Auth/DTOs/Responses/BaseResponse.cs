namespace Auth.DTOs.Responses
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public BaseResponse() { }
        public BaseResponse(bool success)
        {
            Success = success;
        }
        public BaseResponse(bool success, List<string> errors)
        {
            Success = success;
            Errors = errors;
        }
    }
}
