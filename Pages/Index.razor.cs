using System.Text;
using System.Threading.Tasks;
using BlazorVanillaServer.Core;
using RogueSharp;

namespace BlazorVanillaServer.Pages
{
    public partial class Index
    {
        private string _condition = "GREEN";
        private int _torpedoes = 10;
        private int _energy = 1815;
        private int _shieldLevel = 1000;
        private int _klingons = 17;

        private Quadrant _quadrant;

        const int ExpectedWidth = 8;
        const int ExpectedHeight = 7;

        private QuadrantMap _map;

        protected override Task OnInitializedAsync()
        {
            _map = new QuadrantMap(ExpectedWidth, ExpectedHeight);
            _map.Clear(true, true);

            _map.Add(new Star(5, 0));
            
            _map.SetCellProperties(5, 0, false, false);
            _map.SetCellProperties(7, 4, false, false);
            _map.SetCellProperties(4, 5, false, false);
            _map.SetCellProperties(5, 6, false, false);
            var q = _map.ToString();

            var pathFinder = new PathFinder(_map, 1.41);
            var source = _map.GetCell(7, 2);
            var destination = _map.GetCell(3, 4);
            Path shortestPath = pathFinder.ShortestPath(source, destination);
            
            _quadrant = new Quadrant("3/1");

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    _quadrant.Cells[i, j] = new Empty();
                }
            }

            _quadrant.Cells[7, 2] = new Enterprise();
            _quadrant.Cells[5, 0] = new Star2();
            _quadrant.Cells[7, 4] = new Star2();
            _quadrant.Cells[4, 5] = new Star2();
            _quadrant.Cells[5, 6] = new Star2();

            return base.OnInitializedAsync();
        }

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

        public class Enterprise : Empty
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
