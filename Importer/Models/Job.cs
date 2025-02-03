namespace Importer.Models
{
    internal class Job
    {
        public Guid Id { get; set; }
        public int Progress { get; set; }
        public bool IsCompleted { get; set; }
        public string? Result { get; set; }
    }
}
