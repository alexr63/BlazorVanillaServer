using System;
using System.Drawing;
using BlazorVanillaServer.Interfaces;
using RogueSharp;

namespace BlazorVanillaServer.Core
{
    public class Actor : IActor, IDrawable
    {
        public Actor(string name, int speed)
        {
            Name = name;
            Speed = speed;
        }

        // IActor

        // IDrawable
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        
        public string Name { get; set; }

        public int Speed { get; set; }
    }
}