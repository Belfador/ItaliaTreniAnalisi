﻿namespace API.Models.Domain
{
    public class Job
    {
        public Guid Id { get; set; }
        public int Progress { get; set; }
        public bool IsCompleted { get; set; }
        public object? Result { get; set; }
    }
}
