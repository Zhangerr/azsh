using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace math
{
    class Enemy
    {
        public static List<Enemy> enemies = new List<Enemy>();
        public static List<Enemy> toRemove = new List<Enemy>();
        public double angle;
        public Color col;
        public Vector2 pos;
        public int health;
        public int speed;
        public int size;
        public Rectangle bounds;
        public Enemy(double ang, Color c, Vector2 p, int h, int sp, int sze)
        {
            angle = ang;
            col = c;
            pos = p;
            health = h;
            speed = sp;
            size = sze;
            bounds = new Rectangle((int)this.pos.X, (int)this.pos.Y, this.size, this.size);
        }
        public static void spawnWave()
        {
           
            Rectangle bounds = Program.theGame.Window.ClientBounds;
            Random r = new Random();//need at the beginning for diff seed value ^_^
            for (int i = 0; i < 360; i += r.Next(13, 20))
            {
                if (enemies.Count > 10)
                {
                    return;
                }
                int multiply = new Random().Next(20, 100);
               /* if (bounds.Width > bounds.Height)
                {
                    multiply = bounds.Width;
                }
                else
                {
                    multiply = bounds.Height;
                }*/
                double radians = MathHelper.ToRadians(i);
                Vector2 unit = new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians));
                unit *= multiply;
            //    Console.WriteLine(new Random().Next(bounds.Width));
                unit += new Vector2(r.Next(bounds.Width), r.Next(bounds.Height));
           //     Console.WriteLine(unit.X);
                Enemy e = new Enemy(radians, Color.Red, unit, 10, r.Next(1,2), r.Next(8,15));
                enemies.Add(e);
                
            }
        }
        public void update()
        {
            Vector2 dest = Game1.mainPlayer.location;
            Vector2 result = (dest - pos);
            this.bounds = new Rectangle((int)this.pos.X, (int)this.pos.Y, this.size, this.size);
            result.Normalize();
            Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            direction *= -speed;
            result *= speed;
            pos += result;
            Rectangle bounds = Program.theGame.Window.ClientBounds;
            Vector2 center = new Vector2(bounds.Width/2f,bounds.Height/2f);
            foreach (Bullet b in Bullet.bullets)
            {
                if (new Rectangle((int)this.pos.X, (int)this.pos.Y, this.size, this.size).Contains(new Point((int)b.pos.X, (int)b.pos.Y)))
                {
                    this.health -= 1;
                    col = new Color(255 - ((health % 10) * 10), ((health % 10) * 10), 255 - ((health % 10) * 10));
                    //   toRemove.Add(this);
                    //   Console.WriteLine("hit");
                    Bullet.toremove.Add(b);
                }
             /*   if (new Rectangle((int)b.pos.X - 10, (int)b.pos.Y - 10, 20, 20).Contains((int)this.pos.X, (int)this.pos.Y))
                {

                }*/
            }
            foreach (Bullet b in Bullet.toremove)
            {
                Bullet.bullets.Remove(b);
            }
            Bullet.toremove.Clear();
            if (health < 1)
            {
                Particle.burst(this.pos);
                toRemove.Add(this);
                Game1.score++;
            }
            if (!Program.theGame.Window.ClientBounds.Contains((int)pos.X, (int)pos.Y))
            {
                
            }
        }
        public void draw(SpriteBatch sb,Texture2D t2d)
        {
            sb.Draw(t2d, new Rectangle((int)pos.X,(int)pos.Y,size,size), col);
        }
    }
}
