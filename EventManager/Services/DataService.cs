using EventManager.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.Services
{
    public class DataService
    {
        private static DataService? _instance;
        private readonly string _dataPath = "Data";
        private readonly string _eventsFile = "events.json";
        private readonly string _eventTypesFile = "event_types.json";
        private readonly string _tagsFile = "tags.json";

        public static DataService Instance => _instance ??= new DataService();

        private List<Event> _events = new();
        private List<EventType> _eventTypes = new();
        private List<Tag> _tags = new();

        public DataService()
        {
            InitializeDataDirectory();
            LoadSampleData();
        }

        private void InitializeDataDirectory()
        {
            if (!Directory.Exists(_dataPath))
            {
                Directory.CreateDirectory(_dataPath);
            }
        }

        // Events
        public List<Event> GetAllEvents()
        {
            PopulateEventNavigationProperties();
            return _events;
        }

        public Event? GetEventById(string id) =>
            _events.FirstOrDefault(e => e.EventId == id);

        public void AddEvent(Event eventItem)
        {
            _events.Add(eventItem);
            SaveEventsToFile();
        }

        public void UpdateEvent(Event eventItem)
        {
            var index = _events.FindIndex(e => e.EventId == eventItem.EventId);
            if (index >= 0)
            {
                _events[index] = eventItem;
                SaveEventsToFile();
            }
        }

        public void DeleteEvent(string id)
        {
            _events.RemoveAll(e => e.EventId == id);
            SaveEventsToFile();
        }

        // Event Types
        public List<EventType> GetAllEventTypes() => _eventTypes;

        public EventType? GetEventTypeById(string id) =>
            _eventTypes.FirstOrDefault(et => et.EventTypeId == id);

        public void AddEventType(EventType eventType)
        {
            _eventTypes.Add(eventType);
            SaveEventTypesToFile();
        }

        public void UpdateEventType(EventType eventType)
        {
            var index = _eventTypes.FindIndex(et => et.EventTypeId == eventType.EventTypeId);
            if (index >= 0)
            {
                _eventTypes[index] = eventType;
                SaveEventTypesToFile();
            }
        }

        public void DeleteEventType(string id)
        {
            _eventTypes.RemoveAll(et => et.EventTypeId == id);
            SaveEventTypesToFile();
        }

        // Tags
        public List<Tag> GetAllTags() => _tags;

        public Tag? GetTagById(string id) =>
            _tags.FirstOrDefault(t => t.TagId == id);

        public void AddTag(Tag tag)
        {
            _tags.Add(tag);
            SaveTagsToFile();
        }

        public void UpdateTag(Tag tag)
        {
            var index = _tags.FindIndex(t => t.TagId == tag.TagId);
            if (index >= 0)
            {
                _tags[index] = tag;
                SaveTagsToFile();
            }
        }

        public void DeleteTag(string id)
        {
            _tags.RemoveAll(t => t.TagId == id);
            SaveTagsToFile();
        }

        // Helper methods
        private void PopulateEventNavigationProperties()
        {
            foreach (var evt in _events)
            {
                evt.EventType = GetEventTypeById(evt.EventTypeId);
                evt.Tags = _tags.Where(t => evt.TagIds.Contains(t.TagId)).ToList();
            }
        }

        // File operations
        private void SaveEventsToFile()
        {
            var json = JsonConvert.SerializeObject(_events, Formatting.Indented);
            File.WriteAllText(Path.Combine(_dataPath, _eventsFile), json);
        }

        private void SaveEventTypesToFile()
        {
            var json = JsonConvert.SerializeObject(_eventTypes, Formatting.Indented);
            File.WriteAllText(Path.Combine(_dataPath, _eventTypesFile), json);
        }

        private void SaveTagsToFile()
        {
            var json = JsonConvert.SerializeObject(_tags, Formatting.Indented);
            File.WriteAllText(Path.Combine(_dataPath, _tagsFile), json);
        }

        private void LoadSampleData()
        {
            // Load sample data if files don't exist
            var eventsPath = Path.Combine(_dataPath, _eventsFile);
            var eventTypesPath = Path.Combine(_dataPath, _eventTypesFile);
            var tagsPath = Path.Combine(_dataPath, _tagsFile);

            if (!File.Exists(eventTypesPath))
            {
                CreateSampleEventTypes();
                SaveEventTypesToFile();
            }
            else
            {
                var json = File.ReadAllText(eventTypesPath);
                _eventTypes = JsonConvert.DeserializeObject<List<EventType>>(json) ?? new();
            }

            if (!File.Exists(tagsPath))
            {
                CreateSampleTags();
                SaveTagsToFile();
            }
            else
            {
                var json = File.ReadAllText(tagsPath);
                _tags = JsonConvert.DeserializeObject<List<Tag>>(json) ?? new();
            }

            if (!File.Exists(eventsPath))
            {
                CreateSampleEvents();
                SaveEventsToFile();
            }
            else
            {
                var json = File.ReadAllText(eventsPath);
                _events = JsonConvert.DeserializeObject<List<Event>>(json) ?? new();
            }
        }

        private void CreateSampleEventTypes()
        {
            _eventTypes = new List<EventType>
            {
                new() { EventTypeId = "music-festival", Name = "Music Festival", Description = "Large music events", IconPath = "/Assets/Icons/music.png" },
                new() { EventTypeId = "film-festival", Name = "Film Festival", Description = "Cinema and movie events", IconPath = "/Assets/Icons/film.png" },
                new() { EventTypeId = "sports-event", Name = "Sports Event", Description = "Athletic competitions", IconPath = "/Assets/Icons/sports.png" },
                new() { EventTypeId = "conference", Name = "Conference", Description = "Professional gatherings", IconPath = "/Assets/Icons/conference.png" },
                new() { EventTypeId = "cultural", Name = "Cultural Event", Description = "Art and cultural exhibitions", IconPath = "/Assets/Icons/culture.png" }
            };
        }

        private void CreateSampleTags()
        {
            _tags = new List<Tag>
            {
                new() { TagId = "outdoor", Name = "Outdoor", Description = "Events held outdoors", Color = "#28A745" },
                new() { TagId = "indoor", Name = "Indoor", Description = "Events held indoors", Color = "#0066CC" },
                new() { TagId = "vip-access", Name = "VIP Access", Description = "Premium access available", Color = "#FFD700" },
                new() { TagId = "family-friendly", Name = "Family Friendly", Description = "Suitable for all ages", Color = "#FF6B6B" },
                new() { TagId = "international", Name = "International", Description = "Global event", Color = "#9B59B6" }
            };
        }

        private void CreateSampleEvents()
        {
            _events = new List<Event>
            {
                new()
                {
                    EventId = "exit-festival",
                    Name = "Exit Festival",
                    Description = "One of Europe's biggest music festivals featuring international artists",
                    EventTypeId = "music-festival",
                    Attendance = Attendance.Over10000,
                    IsHumanitarian = false,
                    AveragePrice = 120,
                    Country = "Serbia",
                    City = "Novi Sad",
                    CurrentYearDate = new DateTime(2024, 7, 8),
                    PreviousDates = new List<DateTime> { new(2023, 7, 6), new(2022, 7, 7), new(2021, 7, 8) },
                    TagIds = new List<string> { "outdoor", "international" },
                    MapX = 200,
                    MapY = 150,
                    IsOnMap = true
                },
                new()
                {
                    EventId = "kustendorf",
                    Name = "Küstendorf Film Festival",
                    Description = "International film festival in traditional village setting directed by Emir Kusturica",
                    EventTypeId = "film-festival",
                    Attendance = Attendance.From1000To5000,
                    IsHumanitarian = true,
                    AveragePrice = 80,
                    Country = "Serbia",
                    City = "Drvengrad",
                    CurrentYearDate = new DateTime(2024, 1, 15),
                    PreviousDates = new List<DateTime> { new(2023, 1, 10), new(2022, 1, 12), new(2021, 1, 14) },
                    TagIds = new List<string> { "indoor", "international", "family-friendly" },
                    MapX = 180,
                    MapY = 160,
                    IsOnMap = true
                },
                new()
                {
                    EventId = "coachella",
                    Name = "Coachella Valley Music Festival",
                    Description = "Annual music and arts festival in the Colorado Desert",
                    EventTypeId = "music-festival",
                    Attendance = Attendance.Over10000,
                    IsHumanitarian = false,
                    AveragePrice = 400,
                    Country = "United States",
                    City = "Indio, California",
                    CurrentYearDate = new DateTime(2024, 4, 12),
                    PreviousDates = new List<DateTime> { new(2023, 4, 14), new(2022, 4, 15), new(2019, 4, 12) },
                    TagIds = new List<string> { "outdoor", "international", "vip-access" },
                    MapX = 0,
                    MapY = 0,
                    IsOnMap = false
                },
                new()
                {
                    EventId = "tomorrowland",
                    Name = "Tomorrowland",
                    Description = "Electronic dance music festival held in Belgium",
                    EventTypeId = "music-festival",
                    Attendance = Attendance.Over10000,
                    IsHumanitarian = false,
                    AveragePrice = 350,
                    Country = "Belgium",
                    City = "Boom",
                    CurrentYearDate = new DateTime(2024, 7, 19),
                    PreviousDates = new List<DateTime> { new(2023, 7, 21), new(2022, 7, 22), new(2019, 7, 19) },
                    TagIds = new List<string> { "outdoor", "international" },
                    MapX = 0,
                    MapY = 0,
                    IsOnMap = false
                },
                new()
                {
                    EventId = "cannes-film",
                    Name = "Cannes Film Festival",
                    Description = "Prestigious international film festival and market",
                    EventTypeId = "film-festival",
                    Attendance = Attendance.From5000To10000,
                    IsHumanitarian = false,
                    AveragePrice = 200,
                    Country = "France",
                    City = "Cannes",
                    CurrentYearDate = new DateTime(2024, 5, 14),
                    PreviousDates = new List<DateTime> { new(2023, 5, 16), new(2022, 5, 17), new(2021, 7, 6) },
                    TagIds = new List<string> { "indoor", "international", "vip-access" },
                    MapX = 0,
                    MapY = 0,
                    IsOnMap = false
                },
                new()
                {
                    EventId = "sxsw",
                    Name = "South by Southwest (SXSW)",
                    Description = "Annual conglomerate of parallel film, interactive media, and music festivals",
                    EventTypeId = "conference",
                    Attendance = Attendance.Over10000,
                    IsHumanitarian = false,
                    AveragePrice = 250,
                    Country = "United States",
                    City = "Austin, Texas",
                    CurrentYearDate = new DateTime(2024, 3, 8),
                    PreviousDates = new List<DateTime> { new(2023, 3, 10), new(2022, 3, 11), new(2019, 3, 8) },
                    TagIds = new List<string> { "indoor", "international" },
                    MapX = 0,
                    MapY = 0,
                    IsOnMap = false
                },
                new()
                {
                    EventId = "oktoberfest",
                    Name = "Oktoberfest",
                    Description = "World's largest Volksfest (beer festival and travelling funfair)",
                    EventTypeId = "cultural",
                    Attendance = Attendance.Over10000,
                    IsHumanitarian = false,
                    AveragePrice = 100,
                    Country = "Germany",
                    City = "Munich",
                    CurrentYearDate = new DateTime(2024, 9, 16),
                    PreviousDates = new List<DateTime> { new(2023, 9, 16), new(2022, 9, 17), new(2019, 9, 21) },
                    TagIds = new List<string> { "outdoor", "family-friendly" },
                    MapX = 0,
                    MapY = 0,
                    IsOnMap = false
                },
                new()
                {
                    EventId = "wimbledon",
                    Name = "Wimbledon Championships",
                    Description = "The oldest tennis tournament in the world",
                    EventTypeId = "sports-event",
                    Attendance = Attendance.From5000To10000,
                    IsHumanitarian = false,
                    AveragePrice = 300,
                    Country = "United Kingdom",
                    City = "London",
                    CurrentYearDate = new DateTime(2024, 7, 1),
                    PreviousDates = new List<DateTime> { new(2023, 7, 3), new(2022, 6, 27), new(2021, 6, 28) },
                    TagIds = new List<string> { "outdoor", "international", "vip-access" },
                    MapX = 0,
                    MapY = 0,
                    IsOnMap = false
                },
                new()
                {
                    EventId = "burning-man",
                    Name = "Burning Man",
                    Description = "Annual event focused on community, art, self-expression, and self-reliance",
                    EventTypeId = "cultural",
                    Attendance = Attendance.Over10000,
                    IsHumanitarian = false,
                    AveragePrice = 500,
                    Country = "United States",
                    City = "Black Rock City, Nevada",
                    CurrentYearDate = new DateTime(2024, 8, 25),
                    PreviousDates = new List<DateTime> { new(2023, 8, 27), new(2022, 8, 28), new(2019, 8, 25) },
                    TagIds = new List<string> { "outdoor", "international" },
                    MapX = 0,
                    MapY = 0,
                    IsOnMap = false
                },
                new()
                {
                    EventId = "ted-vancouver",
                    Name = "TED Conference Vancouver",
                    Description = "Annual flagship TED event featuring innovative speakers",
                    EventTypeId = "conference",
                    Attendance = Attendance.From1000To5000,
                    IsHumanitarian = false,
                    AveragePrice = 8500,
                    Country = "Canada",
                    City = "Vancouver",
                    CurrentYearDate = new DateTime(2024, 4, 15),
                    PreviousDates = new List<DateTime> { new(2023, 4, 17), new(2022, 4, 10), new(2021, 4, 12) },
                    TagIds = new List<string> { "indoor", "international", "vip-access" },
                    MapX = 0,
                    MapY = 0,
                    IsOnMap = false
                },
                new()
                {
                    EventId = "world-cup-qatar",
                    Name = "FIFA World Cup 2022",
                    Description = "The most prestigious football tournament in the world",
                    EventTypeId = "sports-event",
                    Attendance = Attendance.Over10000,
                    IsHumanitarian = false,
                    AveragePrice = 600,
                    Country = "Qatar",
                    City = "Doha",
                    CurrentYearDate = new DateTime(2022, 11, 20),
                    PreviousDates = new List<DateTime> { new(2018, 6, 14), new(2014, 6, 12), new(2010, 6, 11) },
                    TagIds = new List<string> { "outdoor", "international" },
                    MapX = 0,
                    MapY = 0,
                    IsOnMap = false
                }
            };
        }
    }
}