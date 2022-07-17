namespace BlazorVanillaServer.Core
{
    public class Starbase : Actor
    {
        public Starbase(int x, int y, string name, int speed) : base(name, speed)
        {
            X = x;
            Y = y;
            Symbol = '!';
        }

        public override string ToString()
        {
            return Symbol.ToString();
        }
    }
}