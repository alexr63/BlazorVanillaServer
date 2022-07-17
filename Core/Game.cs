﻿using System;
using System.Linq;
using RogueSharp;

namespace BlazorVanillaServer.Core
{
    public class Game
    {
        const int ExpectedWidth = 7;
        const int ExpectedHeight = 8;
        private readonly QuadrantMap _map;

        private readonly Enterprise _enterprise;
        private readonly Starbase _starbase;

        private Random _random = new Random();

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

            var cells = _map.GetAllCells().ToList();
            var index = _random.Next(0, cells.Count);
            var cell = (Cell)cells.Skip(index).First();
            _starbase = new Starbase(cell.X, cell.Y, "Starbase", 0);
        }

        public override string ToString()
        {
            var rc = _map.ToString();
            return rc;
        }
    }
}
