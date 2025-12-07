using System;

namespace RehabilitationSystem.EngineAPI.Models
{
    public class CommandEnvelope
    {
        public string? Status { get; set; }
        public string? Command { get; set; }
        public object? Data { get; set; }
        public TherapySnapshot? Therapy { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
