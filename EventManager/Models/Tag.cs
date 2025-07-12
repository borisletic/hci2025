using System;
using System.Windows.Media;

namespace EventManager.Models
{
    public class Tag
    {
        public string TagId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = "#0066CC"; // Default blue color

        public SolidColorBrush ColorBrush => new SolidColorBrush((Color)ColorConverter.ConvertFromString(Color));
    }
}