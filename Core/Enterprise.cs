namespace BlazorVanillaServer.Core
{
    public class Enterprise : Actor
    {
        public Enterprise(int x, int y)
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