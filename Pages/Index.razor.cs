using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlazorVanillaServer.Core;
using RogueSharp;
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
        private bool _shieldsUp = false;
        private int _klingons = 17;

        private int starDate = 76045;
        private string starDateString;

        const int ExpectedWidth = 7;
        const int ExpectedHeight = 8;

        private QuadrantMap _map;

        protected override Task OnInitializedAsync()
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

            return base.OnInitializedAsync();
        }

        private void TimerElapsed(Object source, System.Timers.ElapsedEventArgs e)
        {
            Update();
            Draw();
        }

        private void Update()
        {
            starDate = StarDate();

            if (_energy > 100)
            {
                

                var pathFinder = new PathFinder(_map, 1.41);
                var source = _map.EnterpriseCell;
                try
                {
                    var path = pathFinder.ShortestPath(source, _map.Destination);
                    if (_map.SetEnterprisePosition(path.StepForward()))
                    {
                        var energySpent = _random.Next(1, 10);
                        if (_shieldsUp)
                        {
                            energySpent *= 10;
                        }
                        _energy -= energySpent;
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
            _shieldsUp = !_shieldsUp;
        }
    }
}
