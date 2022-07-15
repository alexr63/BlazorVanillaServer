﻿using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlazorVanillaServer.Core;
using RogueSharp;
using Timer = System.Timers.Timer;

namespace BlazorVanillaServer.Pages
{
    public partial class Index
    {
        private Timer _timer;
        private Random _random = new Random();

        private string _condition = "GREEN";
        private int _torpedoes = 10;
        private int _energy = 1815;
        private int _shieldLevel = 1000;
        private int _klingons = 17;

        const int ExpectedWidth = 7;
        const int ExpectedHeight = 8;

        private QuadrantMap _map;

        protected override Task OnInitializedAsync()
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

            _timer = new Timer();
            _timer.Interval = 1000 * 10;
            _timer.Elapsed += TimerElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            var cells = _map.GetAllCells().ToList();
            var index = _random.Next(0, cells.Count);
            _map.Destination = (Cell)cells.Skip(index).First();

            return base.OnInitializedAsync();
        }

        private void TimerElapsed(Object source, System.Timers.ElapsedEventArgs e)
        {
            Update();
            Draw();
        }

        private void Update()
        {
            if (_energy > 100)
            {
                

                var pathFinder = new PathFinder(_map, 1.41);
                var source = _map.EnterpriseCell;
                try
                {
                    var path = pathFinder.ShortestPath(source, _map.Destination);
                    if (_map.SetEnterprisePosition(path.StepForward()))
                    {
                        _energy -= _random.Next(1, 10);
                    }
                }
                catch
                {
                    var cells = _map.GetAllCells().ToList();
                    var index = _random.Next(0, cells.Count);
                    _map.Destination = (Cell)cells.Skip(index).First();
                    _energy = 1815;
                }

                this.InvokeAsync(() => this.StateHasChanged());
                Thread.Sleep(2000);
            }
        }

        private void Draw()
        {
            //this.StateHasChanged();
            this.InvokeAsync(() => this.StateHasChanged());
        }

        public string MapDisplay =>
            _map.ToString()
                .Replace(".", "&nbsp;.&nbsp;")
                .Replace("*", "&nbsp;*&nbsp;")
                .Replace("!", ">!<")
                .Replace("E", "-E-");
    }
}
