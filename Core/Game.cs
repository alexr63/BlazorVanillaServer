namespace BlazorVanillaServer.Core
{
    public class Game
    {
        const int ExpectedWidth = 7;
        const int ExpectedHeight = 8;
        private readonly QuadrantMap _map;

        private readonly Enterprise _enterprise;

        public Game()
        {
            _map = new QuadrantMap(ExpectedWidth, ExpectedHeight);
            _map.Clear(true, true);

            _map.Add(new Star(0, 2));
            _map.Add(new Star(1, 6));
            _map.Add(new Star(2, 1));
            _map.Add(new Star(3, 5));
            _map.Add(new Star(3, 7));
            _map.Add(new Star(4, 5));
            _map.Add(new Star(5, 0));
            _map.Add(new Star(6, 4));
            _map.Add(new Star(5, 6));

            _enterprise = new Enterprise(2, 7, "Enterprise", 10);
        }
    }
}
