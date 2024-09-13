﻿namespace MyPlanner.Plannings.Shared.Infrastructure.Database
{
    public class AuditTable
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string TimeSpan { get; set; }
    }
}
