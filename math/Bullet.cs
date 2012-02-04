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
        Enemy lockOn;
        bool found = false;
        public bool tracking;
        Vector2 inertia = new Vector2(float.NaN, float.NaN);
        bool initi = true;
        int inertiafact = 1;
        public void update()
        {
            if (!Program.theGame.Window.ClientBounds.Contains((int)pos.X + Program.theGame.Window.ClientBounds.X, (int)pos.Y + Program.theGame.Window.ClientBounds.Y))
            {
                Bullet.toremove.Add(this);
            }
            Vector2 vec = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            if (lockOn == null && !found && tracking)
            {
                float smallest = float.MaxValue;
                Enemy temp = null;
                foreach (Enemy e in Enemy.enemies)
                {

                    float theDist = Vector2.Distance(e.pos, pos);
                    if (smallest > theDist)
                    {
                        smallest = theDist;
                        temp = e;
                    }

                }
                lockOn = temp;
                found = true;
            }
            if (lockOn != null && tracking)
            {
                if (lockOn.health > 0)
                {
                    Vector2 tocalc = new Vector2(pos.X - lockOn.pos.X, pos.Y - lockOn.pos.Y);
                      
                    tocalc.Normalize();
                    tocalc *= 5;
                    
                    if (!initi)
                    {
                        tocalc = Vector2.Lerp(inertia, tocalc, .03f);
                    }
                    else
                    {
                        inertia = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                        inertia *= (int)speed;
                        tocalc = Vector2.Lerp(inertia, tocalc, .01f);
                        initi = false;
                    }
                    angle = Math.Atan2(tocalc.Y, tocalc.X);
                 /*   inertia += tocalc;
                    if (inertia.X > inertiafact)
                    {
                        inertia.X = inertiafact;
                    }
                    if (inertia.X < -inertiafact)
                    {
                        inertia.X = -inertiafact;
                    }
                    if (inertia.Y > inertiafact)
                    {
                        inertia.Y = inertiafact;
                    }
                    if (inertia.Y < -inertiafact)
                    {
                        inertia.Y = -inertiafact;
                    }*/
                    pos -= tocalc;
                    //pos -= inertia;
                    inertia = tocalc;
                    
                 //   double angle2 = Math.Atan2(pos.Y, pos.X);
                  //  Console.WriteLine();
                //    Console.WriteLine(angle);
                }
                else
                {
                    lockOn = null;
                    found = false;
                }
            }
            else
            {
                pos -= (vec * (int)speed);
            }
        }
    }
}
