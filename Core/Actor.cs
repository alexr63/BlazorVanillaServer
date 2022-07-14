using System;
using System.Drawing;
using BlazorVanillaServer.Interfaces;
using RogueSharp;

namespace BlazorVanillaServer.Core
{
    public class Actor : IActor, Interfaces.IDrawable, IScheduleable
    {
        private string _name;
        private int _speed;
        
        public Actor()
        {
        }

        // IActor

        // IDrawable
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        
        // IScheduleable
        public int Time
        {
            get
            {
                return Speed;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public int Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value;
            }
        }
    }
}