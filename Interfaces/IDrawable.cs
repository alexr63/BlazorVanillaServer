namespace BlazorVanillaServer.Interfaces
{
    public interface IDrawable
    {
        char Symbol { get; set; }
        int X { get; set; }
        int Y { get; set; }
    }
}