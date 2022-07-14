using System;
using System.Drawing;
using BlazorVanillaServer.Interfaces;
using RogueSharp;

namespace BlazorVanillaServer.Core
{
    public class Star : IStar, IDrawable
    {
        public Star(int x, int y)
        {
            X = x;
            Y = Y;
            Symbol = '*';
        }

        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString()
        {
            return Symbol.ToString();
        }
    }
}