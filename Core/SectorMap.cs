using System.Linq;
using System.Text;
using BlazorVanillaServer.Models;
using BlazorVanillaServer.Pages;
using RogueSharp;

namespace BlazorVanillaServer.Core
{
    public class SectorMap : RogueSharp.Map
    {
        public SectorMap(int width, int height) : base(width, height)
        {
        }

        public string FormatString(KlingonKeyedCollection klingons)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int num = 0;
            foreach (Cell cell in this.GetAllCells())
            {
                if (cell.Y != num)
                {
                    num = cell.Y;
                    stringBuilder.Append("<br />");
                }

                var klingon = klingons.SingleOrDefault(e => e.Position.X == cell.X && e.Position.Y == cell.Y);
                if (!cell.IsWalkable)
                {
                    stringBuilder.Append("*");
                }
                else if (klingon != null)
                {
                    stringBuilder.Append("+");
                }
                else
                {
                    stringBuilder.Append(cell.ToString());
                }
            }

            return stringBuilder.ToString().TrimEnd('\r', '\n')
                .Replace(".", "&nbsp;.&nbsp;")
                .Replace("*", "&nbsp;*&nbsp;")
                .Replace("+", "+++")
                .Replace("!", ">!<")
                .Replace("E", "-E-");
        }

    }
}
