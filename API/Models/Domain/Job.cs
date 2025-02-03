namespace API.Models.Domain
{
    public class Job
    {
        public Guid Id { get; set; }
        public int Progress { get; set; }
        public bool IsCompleted { get; set; }
        public string? Result { get; set; }
    }
}
