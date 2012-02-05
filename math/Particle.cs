using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace math
{
    class Particle
    {
        public double angle;
        public Color col;
        public Vector2 pos;
        public int speed = 8;
        public int life = 1000; //ms
        public int maxlife = 1000;
        public int size = 2;
        public int alpha = 255;
        public static List<Particle> particles = new List<Particle>();
        public static List<Particle> dead = new List<Particle>();
        public static void burst(Vector2 initial)
        {
            Random r = new Random();
            for (int i = 0; i < 360; i+= r.Next(10,15))
            {
                if (particles.Count > 400)
                {
                    return;
                }
                double angle = MathHelper.ToRadians(i);
                Particle p = new Particle();
                p.size = r.Next(1, 4);
                p.speed = r.Next(2, 11);
                p.pos = initial;
                p.angle = angle;
                int l = r.Next(500, 1500);
                p.life = l;
                p.maxlife = l;
                p.col = new Color(r.Next(255), r.Next(255), r.Next(255));
                particles.Add(p);
             
            }
           
        }
        public void update(GameTime gt)
        {
            life -= gt.ElapsedGameTime.Milliseconds;

            if (life < 1)
            {
                dead.Add(this);
                return;
            }
            alpha = (int)(((float)life / (float)maxlife) * 255);
            col = Color.FromNonPremultiplied(col.R, col.G, col.B, alpha);
            Vector2 v2 = new Vector2((float)Math.Cos(angle), (float) Math.Sin(angle));
            v2 *= speed;
            pos += v2;
        }
        public void draw(SpriteBatch sb, Texture2D t2d)
        {
            sb.Draw(t2d, new Rectangle((int)pos.X,(int)pos.Y,size,size), col);
        }
    }
}
