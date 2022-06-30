namespace LoanCalculator.API.Models
{
    public class CommonResponse<T>
    {
        public CommonResponse(T data)  
        {
            Data = data;
        }
        public CommonResponse(string error)
        {
            Error = error;
            Success = false;
        }
        public bool Success { get; set; } = true;
        public string Error { get; set; }
        public T Data { get; set; }
    }
}
