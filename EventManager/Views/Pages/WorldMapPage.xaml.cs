using EventManager.Models;
using EventManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EventManager.Views.Pages
{
    public partial class WorldMapPage : Page
    {
        private readonly DataService _dataService;
        private List<Event> _allEvents = new();
        private List<Event> _availableEvents = new();
        private List<Event> _eventsOnMap = new();

        // Drag & Drop variables
        private bool _isDragging = false;
        private Event? _draggedEvent = null;
        private Point _dragStartPoint;
        private Border? _draggedEventOnMap = null;
        private Button? _draggedButton = null;

        public WorldMapPage()
        {
            InitializeComponent();
            _dataService = DataService.Instance;
            LoadEvents();
            SetupMapEvents();
            InitializeFilter();
        }

        private void LoadEvents()
        {
            try
            {
                _allEvents = _dataService.GetAllEvents();
                _availableEvents = _allEvents.Where(e => !e.IsOnMap).ToList();
                _eventsOnMap = _allEvents.Where(e => e.IsOnMap).ToList();

                RefreshEventButtons();
                RefreshEventsOnMap();
                RefreshMapVisual();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading events: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshEventButtons()
        {
            // Update button visibility based on available events
            var availableEventNames = _availableEvents.Select(e => e.Name.ToLower()).ToList();

            if (KustendorfButton != null)
                KustendorfButton.Visibility = availableEventNames.Any(n => n.Contains("küstendorf") || n.Contains("kustendorf"))
                    ? Visibility.Visible : Visibility.Collapsed;

            if (CoachellaButton != null)
                CoachellaButton.Visibility = availableEventNames.Any(n => n.Contains("coachella"))
                    ? Visibility.Visible : Visibility.Collapsed;

            if (ExitFestivalButton != null)
                ExitFestivalButton.Visibility = availableEventNames.Any(n => n.Contains("exit"))
                    ? Visibility.Visible : Visibility.Collapsed;

            if (NBAButton != null)
                NBAButton.Visibility = availableEventNames.Any(n => n.Contains("nba") || n.Contains("world cup") || n.Contains("wimbledon"))
                    ? Visibility.Visible : Visibility.Collapsed;
        }

        private void RefreshEventsOnMap()
        {
            if (EventsOnMapListView != null)
            {
                EventsOnMapListView.ItemsSource = _eventsOnMap;
            }

            if (NoEventsOnMapText != null)
            {
                NoEventsOnMapText.Visibility = _eventsOnMap.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            if (EventsOnMapSection != null)
            {
                EventsOnMapSection.Visibility = _eventsOnMap.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void RefreshMapVisual()
        {
            if (MapCanvas == null) return;

            // Remove existing event markers from map
            var eventMarkers = MapCanvas.Children.OfType<Border>()
                .Where(b => b.Tag is Event).ToList();

            foreach (var marker in eventMarkers)
            {
                MapCanvas.Children.Remove(marker);
            }

            // Add event markers for events on map
            foreach (var evt in _eventsOnMap)
            {
                AddEventToMapVisual(evt);
            }
        }

        private void AddEventToMapVisual(Event evt)
        {
            if (MapCanvas == null) return;

            var eventMarker = new Border
            {
                Width = 30,
                Height = 30,
                Background = new SolidColorBrush(Colors.LightYellow),
                BorderBrush = new SolidColorBrush(Colors.DarkBlue),
                BorderThickness = new Thickness(2),
                Tag = evt,
                Cursor = Cursors.Hand
            };

            var textBlock = new TextBlock
            {
                Text = GetEventIcon(evt),
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            eventMarker.Child = textBlock;

            // Set position
            Canvas.SetLeft(eventMarker, evt.MapX);
            Canvas.SetTop(eventMarker, evt.MapY);

            // Add event handlers for dragging events on map
            eventMarker.MouseLeftButtonDown += EventOnMap_MouseLeftButtonDown;
            eventMarker.MouseMove += EventOnMap_MouseMove;
            eventMarker.MouseLeftButtonUp += EventOnMap_MouseLeftButtonUp;

            // Add tooltip
            eventMarker.ToolTip = $"{evt.Name}\n{evt.City}, {evt.Country}";

            MapCanvas.Children.Add(eventMarker);
        }

        private string GetEventIcon(Event evt)
        {
            if (evt.EventType?.Name?.ToLower().Contains("music") == true) return "🎵";
            if (evt.EventType?.Name?.ToLower().Contains("film") == true) return "🎬";
            if (evt.EventType?.Name?.ToLower().Contains("sports") == true) return "🏀";
            if (evt.EventType?.Name?.ToLower().Contains("conference") == true) return "💼";
            if (evt.EventType?.Name?.ToLower().Contains("cultural") == true) return "🎭";
            return "🎪";
        }

        private void SetupMapEvents()
        {
            if (MapCanvas != null)
            {
                MapCanvas.AllowDrop = true;
            }
        }

        private void InitializeFilter()
        {
            if (MapFilterTextBox != null)
            {
                MapFilterTextBox.Text = "";
                MapFilterTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }

            if (SearchMapTextBox != null)
            {
                SearchMapTextBox.GotFocus += SearchMapTextBox_GotFocus;
                SearchMapTextBox.LostFocus += SearchMapTextBox_LostFocus;
                SearchMapTextBox.TextChanged += SearchMapTextBox_TextChanged;
            }
        }

        #region Button Drag & Drop Events

        private void EventButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button button)
            {
                _draggedEvent = GetEventForButton(button);
                if (_draggedEvent != null)
                {
                    _isDragging = true;
                    _draggedButton = button;
                    _dragStartPoint = e.GetPosition(button);
                    button.CaptureMouse();
                    e.Handled = true;
                }
            }
        }

        private void EventButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _draggedEvent != null && e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is Button button)
                {
                    var currentPoint = e.GetPosition(button);
                    var diff = _dragStartPoint - currentPoint;

                    if (Math.Abs(diff.X) > 5 || Math.Abs(diff.Y) > 5)
                    {
                        try
                        {
                            var dragData = new DataObject("Event", _draggedEvent);
                            var result = DragDrop.DoDragDrop(button, dragData, DragDropEffects.Move);

                            button.ReleaseMouseCapture();
                            _isDragging = false;
                            _draggedEvent = null;
                            _draggedButton = null;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Drag operation failed: {ex.Message}", "Error");
                            button.ReleaseMouseCapture();
                            _isDragging = false;
                            _draggedEvent = null;
                            _draggedButton = null;
                        }
                    }
                }
            }
        }

        private void EventButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button button)
            {
                button.ReleaseMouseCapture();
                _isDragging = false;
                _draggedEvent = null;
                _draggedButton = null;
            }
        }

        private Event? GetEventForButton(Button button)
        {
            var buttonName = button.Name;
            switch (buttonName)
            {
                case "KustendorfButton":
                    return _availableEvents.FirstOrDefault(e => e.Name.ToLower().Contains("küstendorf") || e.Name.ToLower().Contains("kustendorf"));
                case "CoachellaButton":
                    return _availableEvents.FirstOrDefault(e => e.Name.ToLower().Contains("coachella"));
                case "ExitFestivalButton":
                    return _availableEvents.FirstOrDefault(e => e.Name.ToLower().Contains("exit"));
                case "NBAButton":
                    return _availableEvents.FirstOrDefault(e =>
                        e.Name.ToLower().Contains("nba") ||
                        e.Name.ToLower().Contains("world cup") ||
                        e.Name.ToLower().Contains("wimbledon") ||
                        e.Name.ToLower().Contains("fifa"));
                default:
                    return null;
            }
        }

        #endregion

        #region Map Events Drag & Drop

        private void EventOnMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is Event evt)
            {
                _isDragging = true;
                _draggedEvent = evt;
                _draggedEventOnMap = border;
                _dragStartPoint = e.GetPosition(MapCanvas);
                border.CaptureMouse();
                e.Handled = true;
            }
        }

        private void EventOnMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _draggedEventOnMap != null && e.LeftButton == MouseButtonState.Pressed && MapCanvas != null)
            {
                var currentPoint = e.GetPosition(MapCanvas);

                var newX = Math.Max(0, Math.Min(MapCanvas.ActualWidth - _draggedEventOnMap.Width, currentPoint.X - _draggedEventOnMap.Width / 2));
                var newY = Math.Max(0, Math.Min(MapCanvas.ActualHeight - _draggedEventOnMap.Height, currentPoint.Y - _draggedEventOnMap.Height / 2));

                if (!IsPositionOccupied(newX, newY, _draggedEvent))
                {
                    Canvas.SetLeft(_draggedEventOnMap, newX);
                    Canvas.SetTop(_draggedEventOnMap, newY);
                }
            }
        }

        private void EventOnMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging && _draggedEventOnMap != null && _draggedEvent != null)
            {
                _draggedEvent.MapX = Canvas.GetLeft(_draggedEventOnMap);
                _draggedEvent.MapY = Canvas.GetTop(_draggedEventOnMap);

                _dataService.UpdateEvent(_draggedEvent);

                _draggedEventOnMap.ReleaseMouseCapture();
                _isDragging = false;
                _draggedEvent = null;
                _draggedEventOnMap = null;
            }
        }

        #endregion

        #region Map Canvas Drop Operations

        private void MapCanvas_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Event") && MapCanvas != null && DropIndicator != null)
            {
                e.Effects = DragDropEffects.Move;

                var position = e.GetPosition(MapCanvas);
                var indicatorX = Math.Max(0, Math.Min(MapCanvas.ActualWidth - DropIndicator.Width, position.X - DropIndicator.Width / 2));
                var indicatorY = Math.Max(0, Math.Min(MapCanvas.ActualHeight - DropIndicator.Height, position.Y - DropIndicator.Height / 2));

                Canvas.SetLeft(DropIndicator, indicatorX);
                Canvas.SetTop(DropIndicator, indicatorY);
                DropIndicator.Visibility = Visibility.Visible;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                if (DropIndicator != null)
                {
                    DropIndicator.Visibility = Visibility.Collapsed;
                }
            }
            e.Handled = true;
        }

        private void MapCanvas_Drop(object sender, DragEventArgs e)
        {
            if (DropIndicator != null)
            {
                DropIndicator.Visibility = Visibility.Collapsed;
            }

            if (e.Data.GetDataPresent("Event") && e.Data.GetData("Event") is Event droppedEvent && MapCanvas != null)
            {
                var dropPosition = e.GetPosition(MapCanvas);
                var newX = Math.Max(0, Math.Min(MapCanvas.ActualWidth - 30, dropPosition.X - 15));
                var newY = Math.Max(0, Math.Min(MapCanvas.ActualHeight - 30, dropPosition.Y - 15));

                if (IsPositionOccupied(newX, newY, droppedEvent))
                {
                    MessageBox.Show("This position is already occupied.", "Position Occupied",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                droppedEvent.MapX = newX;
                droppedEvent.MapY = newY;
                droppedEvent.IsOnMap = true;

                _dataService.UpdateEvent(droppedEvent);
                LoadEvents();

                e.Handled = true;
            }
        }

        #endregion

        #region Helper Methods

        private bool IsPositionOccupied(double x, double y, Event? excludeEvent = null)
        {
            const double minDistance = 35;

            foreach (var evt in _eventsOnMap)
            {
                if (excludeEvent != null && evt.EventId == excludeEvent.EventId)
                    continue;

                var distance = Math.Sqrt(Math.Pow(evt.MapX - x, 2) + Math.Pow(evt.MapY - y, 2));
                if (distance < minDistance)
                    return true;
            }

            return false;
        }

        private void RemoveFromMap_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Event eventToRemove)
            {
                eventToRemove.IsOnMap = false;
                eventToRemove.MapX = 0;
                eventToRemove.MapY = 0;

                _dataService.UpdateEvent(eventToRemove);
                LoadEvents();
            }
        }

        #endregion

        #region Filter Methods

        private void MapFilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MapFilterTextBox == null) return;

            var filterText = MapFilterTextBox.Text.ToLower().Trim();

            // Show/hide buttons based on filter
            if (string.IsNullOrWhiteSpace(filterText))
            {
                // Show all available event buttons
                RefreshEventButtons();
            }
            else
            {
                // Filter buttons
                if (KustendorfButton != null)
                    KustendorfButton.Visibility = "kustendorf".Contains(filterText) && _availableEvents.Any(e => e.Name.ToLower().Contains("kustendorf"))
                        ? Visibility.Visible : Visibility.Collapsed;

                if (CoachellaButton != null)
                    CoachellaButton.Visibility = "coachella".Contains(filterText) && _availableEvents.Any(e => e.Name.ToLower().Contains("coachella"))
                        ? Visibility.Visible : Visibility.Collapsed;

                if (ExitFestivalButton != null)
                    ExitFestivalButton.Visibility = "exit festival".Contains(filterText) && _availableEvents.Any(e => e.Name.ToLower().Contains("exit"))
                        ? Visibility.Visible : Visibility.Collapsed;

                if (NBAButton != null)
                    NBAButton.Visibility = "nba".Contains(filterText) && _availableEvents.Any(e => e.Name.ToLower().Contains("nba") || e.Name.ToLower().Contains("world cup") || e.Name.ToLower().Contains("wimbledon"))
                        ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void MapFilterTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // No placeholder handling needed
        }

        private void MapFilterTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // No placeholder handling needed
        }

        private void SearchMapTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchMapTextBox != null)
            {
                SearchMapTextBox.Text = "";
                SearchMapTextBox.Foreground = new SolidColorBrush(Colors.Black);
                SearchMapTextBox.FontStyle = FontStyles.Normal;
            }
        }

        private void SearchMapTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchMapTextBox != null && string.IsNullOrWhiteSpace(SearchMapTextBox.Text))
            {
                
                SearchMapTextBox.Foreground = new SolidColorBrush(Colors.Gray);
                SearchMapTextBox.FontStyle = FontStyles.Italic;
            }
        }

        private void SearchMapTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchMapTextBox == null)
                return;

            var searchText = SearchMapTextBox.Text.ToLower().Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                // Filter events on map based on search
                var matchingEvents = _eventsOnMap.Where(evt =>
                    evt.Name.ToLower().Contains(searchText) ||
                    evt.City.ToLower().Contains(searchText) ||
                    evt.Country.ToLower().Contains(searchText) ||
                    evt.Description.ToLower().Contains(searchText)).ToList();

                // Highlight matching events on map
                HighlightEventsOnMap(matchingEvents);
            }
            else
            {
                // Reset all highlights
                ResetMapHighlights();
            }
        }

        private void HighlightEventsOnMap(List<Event> eventsToHighlight)
        {
            if (MapCanvas == null) return;

            foreach (var child in MapCanvas.Children.OfType<Border>().Where(b => b.Tag is Event))
            {
                var evt = (Event)child.Tag;
                if (eventsToHighlight.Any(e => e.EventId == evt.EventId))
                {
                    // Highlight matching events
                    child.Background = new SolidColorBrush(Colors.Yellow);
                    child.BorderBrush = new SolidColorBrush(Colors.Red);
                    child.BorderThickness = new Thickness(3);
                }
                else
                {
                    // Dim non-matching events
                    child.Opacity = 0.5;
                }
            }
        }

        private void ResetMapHighlights()
        {
            if (MapCanvas == null) return;

            foreach (var child in MapCanvas.Children.OfType<Border>().Where(b => b.Tag is Event))
            {
                // Reset to original style
                child.Background = new SolidColorBrush(Colors.LightYellow);
                child.BorderBrush = new SolidColorBrush(Colors.DarkBlue);
                child.BorderThickness = new Thickness(2);
                child.Opacity = 1.0;
            }
        }

        #endregion

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("Home");
        }
    }
}