using System;

namespace EventManager.Models
{
    public class EventType
    {
        public string EventTypeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
    }
}