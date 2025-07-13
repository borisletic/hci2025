using EventManager.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EventManager.Services
{
    public class DemoService
    {
        private static DemoService? _instance;
        public static DemoService Instance => _instance ??= new DemoService();

        private readonly NavigationService _navigationService;
        private readonly DataService _dataService;
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isDemoRunning;

        public DemoService()
        {
            _navigationService = NavigationService.Instance;
            _dataService = DataService.Instance;
        }

        public bool IsDemoRunning => _isDemoRunning;

        public event EventHandler<string>? DemoActionOccurred;

        public async Task StartDemoAsync()
        {
            if (_isDemoRunning) return;

            _isDemoRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await RunDemoSequenceAsync(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Demo was cancelled - this is expected
                OnDemoAction("Demo was cancelled");
            }
            catch (Exception ex)
            {
                OnDemoAction($"Demo error: {ex.Message}");
            }
            finally
            {
                _isDemoRunning = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public void StopDemo()
        {
            if (_isDemoRunning && _cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                OnDemoAction("Demo stopped by user");
            }
        }

        private async Task RunDemoSequenceAsync(CancellationToken cancellationToken)
        {
            var demoSteps = new List<DemoStep>
            {
                new("🏠 Starting Demo - Welcome to Event Manager!", async () => {
                    _navigationService.NavigateTo("Home");
                    await Task.Delay(3000, cancellationToken);
                }),

                new("📅 Navigating to Event List to show all events", async () => {
                    _navigationService.NavigateTo("EventList");
                    await Task.Delay(4000, cancellationToken);
                }),

                new("🌍 Exploring the World Map feature", async () => {
                    _navigationService.NavigateTo("WorldMap");
                    await Task.Delay(4000, cancellationToken);
                }),

                new("➕ Demonstrating how to add a new event", async () => {
                    _navigationService.NavigateTo("AddEvent");
                    await Task.Delay(3000, cancellationToken);
                }),

                new("⚙️ Checking Event Types management", async () => {
                    _navigationService.NavigateTo("EventTypes");
                    await Task.Delay(3000, cancellationToken);
                }),

                new("🏷️ Reviewing Tags for event categorization", async () => {
                    _navigationService.NavigateTo("Tags");
                    await Task.Delay(3000, cancellationToken);
                }),

                new("🔙 Returning to Home screen", async () => {
                    _navigationService.NavigateTo("Home");
                    await Task.Delay(2000, cancellationToken);
                }),

                new("✅ Demo cycle completed! Starting again...", async () => {
                    await Task.Delay(2000, cancellationToken);
                })
            };

            // Run demo steps in a loop
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (var step in demoSteps)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    OnDemoAction(step.Description);

                    try
                    {
                        await step.Action();
                    }
                    catch (OperationCanceledException)
                    {
                        throw; // Re-throw cancellation
                    }
                    catch (Exception ex)
                    {
                        OnDemoAction($"Step error: {ex.Message}");
                    }
                }

                // Wait before restarting the cycle
                if (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(2000, cancellationToken);
                    OnDemoAction("🔄 Restarting demo cycle...");
                }
            }
        }

        private void OnDemoAction(string action)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                DemoActionOccurred?.Invoke(this, action);
            });
        }

        private class DemoStep
        {
            public string Description { get; }
            public Func<Task> Action { get; }

            public DemoStep(string description, Func<Task> action)
            {
                Description = description;
                Action = action;
            }
        }
    }
}