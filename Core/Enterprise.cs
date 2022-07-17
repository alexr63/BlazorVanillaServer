using System;
using System.Timers;

namespace BlazorVanillaServer.Core
{
    public class Enterprise : Actor
    {
        private Timer _timer;
        
        public Enterprise(int x, int y, string name, int speed) : base(name, speed)
        {
            X = x;
            Y = y;
            Symbol = 'E';

            _timer = new Timer();
            _timer.Interval = 1000 * 20;
            _timer.Elapsed += TimerElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void TimerElapsed(Object source, System.Timers.ElapsedEventArgs e)
        {
            Symbol = 'N';
        }
        
        public override string ToString()
        {
            return Symbol.ToString();
        }
    }
}