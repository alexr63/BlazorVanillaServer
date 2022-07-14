namespace BlazorVanillaServer.Core
{
    public class Enterprise : Actor
    {
        public Enterprise(int x, int y, string name, int speed) : base(name, speed)
        {
            X = x;
            Y = y;
            Symbol = 'E';
        }

        public override string ToString()
        {
            return Symbol.ToString();
        }
    }
}