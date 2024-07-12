namespace Atlantic.Data.Models.Dtos
{
    public class UpdatePinDto
    {
        public string OldPin { get; set; }
        public string? NewPin { get; set; } = string.Empty;

    }
}
