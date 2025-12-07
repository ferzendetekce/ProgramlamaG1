using System;

namespace RehabilitationSystem.EngineAPI.Models
{
    public class TherapySnapshot
    {
        public string? PatientName { get; set; }
        public bool IsRunning { get; set; }
        public bool IsPaused { get; set; }
        public bool IsEmergency { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int TargetDurationMinutes { get; set; }
        public double WeightSupport { get; set; }
        public int ShoeSize { get; set; }
        public double SupportBarHeight { get; set; }
        public string? LastCommand { get; set; }
        public string? StatusText { get; set; }
    }
}
