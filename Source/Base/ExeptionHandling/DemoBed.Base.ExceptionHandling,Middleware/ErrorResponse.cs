namespace DemoBed.Base.ExceptionHandling_Middleware
{
    public class ErrorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
    }
}