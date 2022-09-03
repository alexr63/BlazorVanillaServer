using System;
using System.Drawing;
using BlazorVanillaServer.Interfaces;
using RogueSharp;

namespace BlazorVanillaServer.Core
{
    public class Actor : IActor, IDrawable
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public char Symbol { get; set; }

        public Actor(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
        }
     }
}