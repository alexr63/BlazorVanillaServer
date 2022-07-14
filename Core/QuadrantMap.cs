using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueSharp;

namespace BlazorVanillaServer.Core;

public class QuadrantMap : Map
{
    public Cell Destination;
    private readonly Enterprise _enterprise;
    private readonly List<Star> _stars;

    public QuadrantMap(int width, int height) : base(width, height)
    {
        _enterprise = new Enterprise(2, 7, "Enterprise", 10);
        _stars = new List<Star>();
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        int num = 0;
        foreach (Cell allCell in this.GetAllCells())
        {
            if (allCell.Y != num)
            {
                num = allCell.Y;
                stringBuilder.Append("<br />");
            }

            var star = _stars.SingleOrDefault(e => e.X == allCell.X && e.Y == allCell.Y);
            if (star != null)
            {
                stringBuilder.Append(star.ToString());
            }
            else if (_enterprise.X == allCell.X && _enterprise.Y == allCell.Y)
            {
                stringBuilder.Append(_enterprise.ToString());
            }
            else if (Destination.X == allCell.X && Destination.Y == allCell.Y)
            {
                stringBuilder.Append("+");
            }
            else
            {
                stringBuilder.Append(allCell.ToString());
            }
        }
        return stringBuilder.ToString().TrimEnd('\r', '\n');
    }

    public void Add(Star star)
    {
        _stars.Add(star);
        SetIsWalkable(star.X, star.Y, false);
    }

    public ICell EnterpriseCell => GetCell(_enterprise.X, _enterprise.Y);

    public bool SetEnterprisePosition(ICell cell)
    {
        if (cell.IsWalkable)
        {
            _enterprise.X = cell.X;
            _enterprise.Y = cell.Y;
            return true;
        }
        return false;
    }

    public bool SetActorPosition(Actor actor, int x, int y)
    {
        if (GetCell(x, y).IsWalkable)
        {
            SetIsWalkable(actor.X, actor.Y, true);
            actor.X = x;
            actor.Y = y;
            SetIsWalkable(actor.X, actor.Y, false);
            return true;
        }
        return false;
    }
    
    public void SetIsWalkable(int x, int y, bool isWalkable)
    {
        ICell cell = GetCell(x, y);
        SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
    }
}
