using EventManager.Services;
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
                new("Welcome to Event Manager Demo!", async () => {
                    _navigationService.NavigateTo("Home");
                    await Task.Delay(2000, cancellationToken);
                }),

                new("Viewing all events in the system", async () => {
                    _navigationService.NavigateTo("EventList");
                    await Task.Delay(3000, cancellationToken);
                }),

                new("Let's look at the world map view", async () => {
                    _navigationService.NavigateTo("WorldMap");
                    await Task.Delay(3000, cancellationToken);
                }),

                new("Now we'll create a new event", async () => {
                    _navigationService.NavigateTo("AddEvent");
                    await Task.Delay(2000, cancellationToken);
                }),

                new("Filling out event details automatically", async () => {
                    // This would simulate filling out the form
                    await SimulateEventCreation(cancellationToken);
                }),

                new("Checking event types management", async () => {
                    _navigationService.NavigateTo("EventTypes");
                    await Task.Delay(2500, cancellationToken);
                }),

                new("Looking at tags for categorization", async () => {
                    _navigationService.NavigateTo("Tags");
                    await Task.Delay(2500, cancellationToken);
                }),

                new("Returning to home screen", async () => {
                    _navigationService.NavigateTo("Home");
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
                    await step.Action();
                }

                // Wait before restarting the cycle
                await Task.Delay(3000, cancellationToken);
                OnDemoAction("Restarting demo cycle...");
            }
        }

        private async Task SimulateEventCreation(CancellationToken cancellationToken)
        {
            // This would simulate user input in the form
            await Task.Delay(1000, cancellationToken);
            OnDemoAction("Entering event name: 'Demo Music Festival'");

            await Task.Delay(1000, cancellationToken);
            OnDemoAction("Setting event type to 'Music Festival'");

            await Task.Delay(1000, cancellationToken);
            OnDemoAction("Adding location: Belgrade, Serbia");

            await Task.Delay(1500, cancellationToken);
            OnDemoAction("Event creation simulated - going to next step");
        }

        private void OnDemoAction(string action)
        {
            Application.Current.Dispatcher.Invoke(() =>
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