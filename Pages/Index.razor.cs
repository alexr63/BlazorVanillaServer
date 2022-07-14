using System.Text;
using System.Threading.Tasks;

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

        protected override Task OnInitializedAsync()
        {
            _quadrant = new Quadrant("3/1");

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    _quadrant.Cells[i, j] = new Empty();
                }
            }

            _quadrant.Cells[7, 2] = new Enterprise();
            _quadrant.Cells[5, 0] = new Star();
            _quadrant.Cells[7, 4] = new Star();
            _quadrant.Cells[4, 5] = new Star();
            _quadrant.Cells[5, 6] = new Star();

            return base.OnInitializedAsync();
        }

        public class Empty
        {
            public override string ToString()
            {
                return "&nbsp;.&nbsp;";
            }
        }

        public class Star : Empty
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
