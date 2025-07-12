using System;

namespace EventManager.Models
{
    public enum Attendance
    {
        UpTo1000,
        From1000To5000,
        From5000To10000,
        Over10000
    }

    public static class AttendanceExtensions
    {
        public static string ToDisplayString(this Attendance attendance)
        {
            return attendance switch
            {
                Attendance.UpTo1000 => "Up to 1000",
                Attendance.From1000To5000 => "1000-5000",
                Attendance.From5000To10000 => "5000-10000",
                Attendance.Over10000 => "Over 10000",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}