using EventManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventManager.Models
{
    public class Event
    {
        public string EventId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EventTypeId { get; set; } = string.Empty;
        public Attendance Attendance { get; set; }
        public string IconPath { get; set; } = string.Empty;
        public bool IsHumanitarian { get; set; }
        public decimal AveragePrice { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public DateTime CurrentYearDate { get; set; }
        public List<DateTime> PreviousDates { get; set; } = new List<DateTime>();
        public List<string> TagIds { get; set; } = new List<string>();

        // Map position
        public double MapX { get; set; }
        public double MapY { get; set; }
        public bool IsOnMap { get; set; }

        // Navigation properties (will be populated by service)
        public EventType? EventType { get; set; }
        public List<Tag> Tags { get; set; } = new List<Tag>();

        // Helper properties
        public string AttendanceDisplay => Attendance.ToDisplayString();
        public string PreviousDatesDisplay => string.Join(", ", PreviousDates.Select(d => d.ToString("yyyy-MM-dd")));
        public string TagsDisplay => string.Join(", ", Tags.Select(t => t.Name));
    }
}