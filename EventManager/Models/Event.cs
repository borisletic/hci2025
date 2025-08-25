// EventManager/Models/Event.cs - UPDATED with EventTypeIcon property

using EventManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EventManager.Models
{
    public class Event : INotifyPropertyChanged
    {
        private string _eventId = string.Empty;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private string _eventTypeId = string.Empty;
        private Attendance _attendance;
        private string _iconPath = string.Empty;
        private bool _isHumanitarian;
        private decimal _averagePrice;
        private string _country = string.Empty;
        private string _city = string.Empty;
        private DateTime _currentYearDate;
        private List<DateTime> _previousDates = new List<DateTime>();
        private List<string> _tagIds = new List<string>();
        private double _mapX;
        private double _mapY;
        private bool _isOnMap;

        public string EventId
        {
            get => _eventId;
            set { _eventId = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public string EventTypeId
        {
            get => _eventTypeId;
            set { _eventTypeId = value; OnPropertyChanged(); }
        }

        public Attendance Attendance
        {
            get => _attendance;
            set { _attendance = value; OnPropertyChanged(); OnPropertyChanged(nameof(AttendanceDisplay)); }
        }

        public string IconPath
        {
            get => _iconPath;
            set { _iconPath = value; OnPropertyChanged(); OnPropertyChanged(nameof(EventTypeIcon)); }
        }

        public bool IsHumanitarian
        {
            get => _isHumanitarian;
            set { _isHumanitarian = value; OnPropertyChanged(); }
        }

        public decimal AveragePrice
        {
            get => _averagePrice;
            set { _averagePrice = value; OnPropertyChanged(); }
        }

        public string Country
        {
            get => _country;
            set { _country = value; OnPropertyChanged(); }
        }

        public string City
        {
            get => _city;
            set { _city = value; OnPropertyChanged(); }
        }

        public DateTime CurrentYearDate
        {
            get => _currentYearDate;
            set { _currentYearDate = value; OnPropertyChanged(); }
        }

        public List<DateTime> PreviousDates
        {
            get => _previousDates;
            set { _previousDates = value; OnPropertyChanged(); OnPropertyChanged(nameof(PreviousDatesDisplay)); }
        }

        public List<string> TagIds
        {
            get => _tagIds;
            set { _tagIds = value; OnPropertyChanged(); }
        }

        // Map position properties
        public double MapX
        {
            get => _mapX;
            set { _mapX = value; OnPropertyChanged(); }
        }

        public double MapY
        {
            get => _mapY;
            set { _mapY = value; OnPropertyChanged(); }
        }

        public bool IsOnMap
        {
            get => _isOnMap;
            set { _isOnMap = value; OnPropertyChanged(); }
        }

        // Navigation properties (will be populated by service)
        public EventType? EventType { get; set; }
        public List<Tag> Tags { get; set; } = new List<Tag>();

        // Helper properties
        public string AttendanceDisplay => Attendance.ToDisplayString();
        public string PreviousDatesDisplay => string.Join(", ", PreviousDates.Select(d => d.ToString("yyyy-MM-dd")));
        public string TagsDisplay => string.Join(", ", Tags.Select(t => t.Name));

        // Map-specific helper properties
        public string LocationDisplay => $"{City}, {Country}";

        // UPDATED: EventTypeIcon now uses proper priority system
        public string EventTypeIcon
        {
            get
            {
                // Priority 1: Use event's specific icon if available
                if (!string.IsNullOrEmpty(IconPath) && !IconPath.Contains("/") && !IconPath.Contains("\\"))
                {
                    return IconPath;
                }

                // Priority 2: Use event type's icon if available
                if (EventType != null && !string.IsNullOrEmpty(EventType.IconPath))
                {
                    if (!EventType.IconPath.Contains("/") && !EventType.IconPath.Contains("\\"))
                    {
                        return EventType.IconPath;
                    }
                    else
                    {
                        return ConvertFilePathToEmoji(EventType.IconPath);
                    }
                }

                // Priority 3: Fallback to type-based icons
                if (EventType?.Name != null)
                {
                    var typeName = EventType.Name.ToLower();
                    if (typeName.Contains("music")) return "🎵";
                    if (typeName.Contains("film")) return "🎬";
                    if (typeName.Contains("sports")) return "🏀";
                    if (typeName.Contains("conference")) return "💼";
                    if (typeName.Contains("cultural")) return "🎭";
                }

                // Priority 4: Default fallback
                return "🎪";
            }
        }

        // Helper method for converting file paths to emojis
        private string ConvertFilePathToEmoji(string filePath)
        {
            var path = filePath.ToLower();
            if (path.Contains("music")) return "🎵";
            if (path.Contains("film") || path.Contains("movie")) return "🎬";
            if (path.Contains("sports")) return "🏀";
            if (path.Contains("conference") || path.Contains("business")) return "💼";
            if (path.Contains("cultural") || path.Contains("culture")) return "🎭";
            if (path.Contains("art")) return "🎨";
            return "🎪";
        }

        public string MapTooltip => $"{Name}\n{City}, {Country}\n{AttendanceDisplay} attendees\n${AveragePrice}";

        // Position validation
        public bool IsValidMapPosition(double canvasWidth, double canvasHeight)
        {
            const double markerSize = 40;
            return MapX >= 0 && MapY >= 0 &&
                   MapX <= canvasWidth - markerSize &&
                   MapY <= canvasHeight - markerSize;
        }

        public double DistanceTo(Event other)
        {
            return Math.Sqrt(Math.Pow(MapX - other.MapX, 2) + Math.Pow(MapY - other.MapY, 2));
        }

        public bool OverlapsWith(Event other, double minDistance = 45)
        {
            return other != null && DistanceTo(other) < minDistance;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}