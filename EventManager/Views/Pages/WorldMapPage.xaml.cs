// EventManager/Views/Pages/WorldMapPage.xaml.cs
// DYNAMIC VERSION - Works with any number of events

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
        private List<Event> _filteredAvailableEvents = new(); // For filter support

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
                _filteredAvailableEvents = _availableEvents.ToList();

                RefreshAvailableEventsDisplay();
                RefreshEventsOnMap();
                RefreshMapVisual();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading events: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // DYNAMIC: Refresh available events display
        private void RefreshAvailableEventsDisplay()
        {
            if (AvailableEventsPanel != null)
            {
                // Populate events with proper icons
                foreach (var evt in _filteredAvailableEvents)
                {
                    evt.EventType = _dataService.GetEventTypeById(evt.EventTypeId); // Ensure EventType is loaded
                }
                AvailableEventsPanel.ItemsSource = _filteredAvailableEvents;
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

        // DYNAMIC: GetEventIcon now properly uses event's actual icon
        private string GetEventIcon(Event evt)
        {
            // Priority 1: Use event's specific icon if available
            if (!string.IsNullOrEmpty(evt.IconPath) && !evt.IconPath.Contains("/") && !evt.IconPath.Contains("\\"))
            {
                return evt.IconPath;
            }

            // Priority 2: Use event type's icon if available
            if (evt.EventType != null && !string.IsNullOrEmpty(evt.EventType.IconPath))
            {
                if (!evt.EventType.IconPath.Contains("/") && !evt.EventType.IconPath.Contains("\\"))
                {
                    return evt.EventType.IconPath;
                }
                else
                {
                    return ConvertFilePathToEmoji(evt.EventType.IconPath);
                }
            }

            // Priority 3: Fallback to type-based icons
            if (evt.EventType?.Name != null)
            {
                var typeName = evt.EventType.Name.ToLower();
                if (typeName.Contains("music")) return "🎵";
                if (typeName.Contains("film")) return "🎬";
                if (typeName.Contains("sports")) return "🏀";
                if (typeName.Contains("conference")) return "💼";
                if (typeName.Contains("cultural")) return "🎭";
            }

            // Priority 4: Default fallback
            return "🎪";
        }

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
            if (sender is Button button && button.Tag is Event evt)
            {
                _draggedEvent = evt;
                _isDragging = true;
                _draggedButton = button;
                _dragStartPoint = e.GetPosition(button);
                button.CaptureMouse();
                e.Handled = true;
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

        // DYNAMIC: Filter now works with any events
        private void MapFilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MapFilterTextBox == null) return;

            var filterText = MapFilterTextBox.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(filterText))
            {
                // Show all available events
                _filteredAvailableEvents = _availableEvents.ToList();
            }
            else
            {
                // Filter events based on name, description, city, or country
                _filteredAvailableEvents = _availableEvents.Where(evt =>
                    evt.Name.ToLower().Contains(filterText) ||
                    evt.Description.ToLower().Contains(filterText) ||
                    evt.City.ToLower().Contains(filterText) ||
                    evt.Country.ToLower().Contains(filterText)
                ).ToList();
            }

            RefreshAvailableEventsDisplay();
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