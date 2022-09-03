namespace BlazorVanillaServer.Core
{
    public class Enterprise : Actor
    {
        public Enterprise(string name, int x, int y) : base(name, x, y)
        {
            Symbol = 'E';
        }

        public override string ToString()
        {
            return Symbol.ToString();
        }
    }
}