using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace math
{
    class Bullet
    {
        public static List<Bullet> toremove = new List<Bullet>();
        public static List<Bullet> bullets = new List<Bullet>();
            public double angle;
            public Color col;
            public Vector2 pos;
            public double speed = 8;
        
    }
}
