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
        private int _klingons = 17;

        private Quadrant _quadrant;

        const int ExpectedWidth = 7;
        const int ExpectedHeight = 8;

        private QuadrantMap _map;

        private Cell _destination;

        protected override Task OnInitializedAsync()
        {
            _map = new QuadrantMap(ExpectedWidth, ExpectedHeight);
            _map.Clear(true, true);

            _map.Add(new Star(5, 0));
            _map.Add(new Star(6, 4));
            _map.Add(new Star(4, 5));
            _map.Add(new Star(5, 6));

            _quadrant = new Quadrant("3/1");

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    _quadrant.Cells[i, j] = new Empty();
                }
            }

            _quadrant.Cells[7, 2] = new Enterprise2();
            _quadrant.Cells[5, 0] = new Star2();
            _quadrant.Cells[7, 4] = new Star2();
            _quadrant.Cells[4, 5] = new Star2();
            _quadrant.Cells[5, 6] = new Star2();

            _timer = new Timer();
            _timer.Interval = 1000 * 1;
            _timer.Elapsed += TimerElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            var cells = _map.GetAllCells();
            var index = _random.Next(0, cells.Count());
            _destination = (Cell)cells.Skip(index).First();

            return base.OnInitializedAsync();
        }

        private void TimerElapsed(Object source, System.Timers.ElapsedEventArgs e)
        {
            Update();
            Draw();
        }

        private void Update()
        {
            if (_energy > 100)
            {
                _energy -= _random.Next(-10, 10);

                var pathFinder = new PathFinder(_map, 1.41);
                var source = _map.EnterpriseCell;
                Path path = null;
                try
                {
                    path = pathFinder.ShortestPath(source, _destination);
                    _map.SetEnterprisePosition(path.StepForward());
                }
                catch
                {
                    var cells = _map.GetAllCells();
                    var index = _random.Next(0, cells.Count());
                    _destination = (Cell)cells.Skip(index).First();
                }

                this.InvokeAsync(() => this.StateHasChanged());
                Thread.Sleep(2000);
            }
        }

        private void Draw()
        {
            //this.StateHasChanged();
            this.InvokeAsync(() => this.StateHasChanged());
        }

        public string MapDisplay =>
            _map.ToString()
                .Replace(".", "&nbsp;.&nbsp;")
                .Replace("*", "&nbsp;*&nbsp;")
                .Replace("E", "-E-");

        public class Empty
        {
            public override string ToString()
            {
                return "&nbsp;.&nbsp;";
            }
        }

        public class Star2 : Empty
        {
            public override string ToString()
            {
                return "&nbsp;*&nbsp;";
            }
        }

        public class Enterprise2 : Empty
        {
            public override string ToString()
            {
                return "-E-";
            }
        }

        public class Quadrant
        {
            public string Title { get; set; }
            public Empty[,] Cells = new Empty[8, 7];

            public Quadrant(string title)
            {
                Title = title;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        sb.Append(Cells[i, j]);
                    }
                    sb.AppendLine("<br />");
                }

                return sb.ToString();
            }
        }
    }
}
