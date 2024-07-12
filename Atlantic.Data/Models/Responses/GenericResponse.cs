namespace Atlantic.Data.Models.Responses
{
    public class GenericResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public object Data { get; set; }
    }
}
