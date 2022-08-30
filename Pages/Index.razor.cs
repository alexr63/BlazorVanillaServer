using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlazorVanillaServer.Core;
using BlazorVanillaServer.Models;
using BlazorVanillaServer.Services;
using Microsoft.AspNetCore.Components;
using Orleans.Streams;
using RogueSharp;
using Enterprise = BlazorVanillaServer.Models.Enterprise;
using Timer = System.Timers.Timer;

namespace BlazorVanillaServer.Pages
{
    public partial class Index
    {
        private Timer _timer;
        private Random _random = new Random();

        private string _condition = "GREEN";
        private int _torpedoes = 10;
        private int _energy = 1815;
        private int _shieldLevel = 1000;
        private int _klingons = 17;

        private int starDate = 76045;
        private string starDateString;

        const int ExpectedWidth = 7;
        const int ExpectedHeight = 8;

        private QuadrantMap _map;

        [Inject]
        private SectorService SectorService { get; set; }
        
        private Guid ownerKey = Guid.Empty;
        private KlingonKeyedCollection klingons = new();
        private EnterpriseKeyedCollection enterprises = new();
        private StreamSubscriptionHandle<KlingonNotification>? klingonSubscription;
        private StreamSubscriptionHandle<EnterpriseNotification>? enterpriseSubscription;
        private string map;

        protected override async Task OnInitializedAsync()
        {
            starDate = StarDate();
            starDateString = DecimalToArbitrarySystem(starDate, 20);
            
            _map = new QuadrantMap(ExpectedWidth, ExpectedHeight);
            _map.Clear(true, true);

            _map.Add(new Star(0, 2));
            _map.Add(new Star(1, 6));
            _map.Add(new Star(2, 1));
            _map.Add(new Star(3, 5));
            _map.Add(new Star(3, 7));
            _map.Add(new Star(4, 5));
            _map.Add(new Star(5, 0));
            _map.Add(new Star(6, 4));
            
            _map.Add(new Star(5, 6));

            _timer = new Timer();
            _timer.Interval = 1000 * 10;
            _timer.Elapsed += TimerElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            var cells = _map.GetAllCells().ToList();
            var index = _random.Next(0, cells.Count);
            _map.Destination = (Cell)cells.Skip(index).First();

            // subscribe to updates for the current list
            // note that the blazor task scheduler is reentrant
            // therefore notifications can and will come
            // // through when the code is stuck at an await
            klingonSubscription = await SectorService.SubscribeKlingonAsync(
                ownerKey,
                notification => InvokeAsync(
                    () => HandleKlingonNotificationAsync(notification)));
            enterpriseSubscription = await SectorService.SubscribeEnterpriseAsync(
                ownerKey,
                notification => InvokeAsync(
                    () => HandleEnterpriseNotificationAsync(notification)));

            // get all items from the cluster
            KlingonKeyedCollection klingons2 = new();
            foreach (var item in await SectorService.GetAllAsync(ownerKey))
            {
                klingons2.Add(item);
            }
            EnterpriseKeyedCollection enterprises2 = new();
            foreach (var item in await SectorService.GetAllAsync(ownerKey))
            {
                klingons2.Add(item);
            }

            map = await SectorService.GetMapAsync(ownerKey, klingons2);

            await AddKlingonAsync();
            await AddEnterpriseAsync();

            await base.OnInitializedAsync();
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (klingonSubscription is not null)
                {
                    // unsubscribe from the orleans stream - best effort
                    await klingonSubscription.UnsubscribeAsync();
                }
                
                if (enterpriseSubscription is not null)
                {
                    // unsubscribe from the orleans stream - best effort
                    await enterpriseSubscription.UnsubscribeAsync();
                }
            }
            catch
            {
                // noop
            }
        }

        private async Task AddKlingonAsync()
        {
            var rnd = new Random();
            // create a new klingon
            var klingon = new Klingon(Guid.NewGuid(), $"Ogen K'gandlaj {DateTime.UtcNow.Second}",
                new Location(rnd.Next(0, 7), rnd.Next(0, 8)),
                new Location(rnd.Next(0, 7), rnd.Next(0, 8)),
                ownerKey, DateTime.UtcNow);

            // add it to the cluste
            await SectorService.SetAsync(klingon);

            // the above this will generate a stream notification that may or may not have come through while we were awaiting the call
            // therefore only add it to the interface if it is not there yet
            if (klingons.TryGetValue(klingon.Key, out var current))
            {
                // latest one wins
                if (klingon.Timestamp > current.Timestamp)
                {
                    klingons[klingons.IndexOf(current)] = klingon;
                }
            }
            else
            {
                klingons.Add(klingon);
            }
        }

        private async Task AddEnterpriseAsync()
        {
            var rnd = new Random();
            // create a new enterprise
            var enterprise = new Enterprise(Guid.NewGuid(), $"Enterprise {DateTime.UtcNow.Second}",
                new Location(rnd.Next(0, 7), rnd.Next(0, 8)),
                new Location(rnd.Next(0, 7), rnd.Next(0, 8)),
                ownerKey, DateTime.UtcNow);

            // add it to the cluste
            await SectorService.SetAsync(enterprise);

            // the above this will generate a stream notification that may or may not have come through while we were awaiting the call
            // therefore only add it to the interface if it is not there yet
            if (enterprises.TryGetValue(enterprise.Key, out var current))
            {
                // latest one wins
                if (enterprise.Timestamp > current.Timestamp)
                {
                    enterprises[enterprises.IndexOf(current)] = enterprise;
                }
            }
            else
            {
                enterprises.Add(enterprise);
            }
        }

        private async Task HandleKlingonNotificationAsync(KlingonNotification notification)
        {
            // was the item removed
            if (notification.Item is null)
            {
                // attempt to remove it from the user interface
                if (klingons.Remove(notification.ItemKey))
                {
                    StateHasChanged();
                }
                return;
            }

            if (klingons.TryGetValue(notification.Item.Key, out var current))
            {
                // latest one wins
                if (notification.Item.Timestamp > current.Timestamp)
                {
                    klingons[klingons.IndexOf(current)] = notification.Item;
                    map = await SectorService.GetMapAsync(ownerKey, klingons);
                    StateHasChanged();
                }
                map = await SectorService.GetMapAsync(ownerKey, klingons);
                StateHasChanged();
                return;
            }

            klingons.Add(notification.Item);
            map = await SectorService.GetMapAsync(ownerKey, klingons);
            StateHasChanged();
            return;
        }

        private async Task HandleEnterpriseNotificationAsync(EnterpriseNotification notification)
        {
            // was the item removed
            if (notification.Item is null)
            {
                // attempt to remove it from the user interface
                if (klingons.Remove(notification.ItemKey))
                {
                    StateHasChanged();
                }
                return;
            }

            if (enterprises.TryGetValue(notification.Item.Key, out var current))
            {
                // latest one wins
                if (notification.Item.Timestamp > current.Timestamp)
                {
                    enterprises[enterprises.IndexOf(current)] = notification.Item;
                    map = await SectorService.GetMapAsync(ownerKey, klingons);
                    StateHasChanged();
                }
                map = await SectorService.GetMapAsync(ownerKey, klingons);
                StateHasChanged();
                return;
            }

            enterprises.Add(notification.Item);
            map = await SectorService.GetMapAsync(ownerKey, klingons);
            StateHasChanged();
            return;
        }

        private void TryUpdateKlingonCollection(Klingon item)
        {
            // we need to cater for reentrancy allowing a stream notification during the previous await
            // the notification may have even have deleted the item - if so then deletion wins
            if (klingons.TryGetValue(item.Key, out var current))
            {
                // latest one wins
                if (item.Timestamp > current.Timestamp)
                {
                    klingons[klingons.IndexOf(current)] = item;
                }
            }
        }

        private void TryUpdateEnterpriseCollection(Enterprise item)
        {
            // we need to cater for reentrancy allowing a stream notification during the previous await
            // the notification may have even have deleted the item - if so then deletion wins
            if (enterprises.TryGetValue(item.Key, out var current))
            {
                // latest one wins
                if (item.Timestamp > current.Timestamp)
                {
                    enterprises[enterprises.IndexOf(current)] = item;
                }
            }
        }

        private void TimerElapsed(Object source, System.Timers.ElapsedEventArgs e)
        {
            Update();
            Draw();
        }

        private void Update()
        {
            starDate += 1;

            if (_energy > 100)
            {
                

                var pathFinder = new PathFinder(_map, 1.41);
                var source = _map.EnterpriseCell;
                try
                {
                    var path = pathFinder.ShortestPath(source, _map.Destination);
                    if (_map.SetEnterprisePosition(path.StepForward()))
                    {
                        _energy -= _random.Next(1, 10);
                    }
                }
                catch
                {
                    var cells = _map.GetAllCells().ToList();
                    var index = _random.Next(0, cells.Count);
                    _map.Destination = (Cell)cells.Skip(index).First();
                    _energy = 1815;
                }

                this.InvokeAsync(() => this.StateHasChanged());
                Thread.Sleep(2000);
            }
        }

        private void Draw()
        {
            starDateString = DecimalToArbitrarySystem(starDate, 20);

            //this.StateHasChanged();
            this.InvokeAsync(() => this.StateHasChanged());
        }

        public string MapDisplay =>
            _map.ToString()
                .Replace(".", "&nbsp;.&nbsp;")
                .Replace("*", "&nbsp;*&nbsp;")
                .Replace("!", ">!<")
                .Replace("E", "-E-");

        /// <summary>
        /// Converts the given decimal number to the numeral system with the
        /// specified radix (in the range [2, 36]).
        /// </summary>
        /// <param name="decimalNumber">The number to convert.</param>
        /// <param name="radix">The radix of the destination numeral system (in the range [2, 36]).</param>
        /// <returns></returns>
        public static string DecimalToArbitrarySystem(long decimalNumber, int radix)
        {
            const int BitsInLong = 64;
            const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (radix < 2 || radix > Digits.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

            if (decimalNumber == 0)
                return "0";

            int index = BitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[BitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = Digits[remainder];
                currentNumber = currentNumber / radix;
            }

            string result = new String(charArray, index + 1, BitsInLong - index - 1);
            if (decimalNumber < 0)
            {
                result = "-" + result;
            }

            return result;
        }

        private int StarDate()
        {
            var now = DateTime.UtcNow;
            var then = new DateTime(1987, 7, 15);
            var stardate = (now - then).TotalMilliseconds;
            stardate = stardate / (1000 * 60 * 60 * 24 * 0.036525);
            stardate = Math.Floor(stardate + 410000);
            stardate = stardate / 10;
            return (int)stardate;
        }

        private void SwitchShields()
        {
            _shieldLevel = _shieldLevel == 1000 ? 0 : 1000;
        }
    }
}
