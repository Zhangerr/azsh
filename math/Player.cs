using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace math
{
    public enum PlayerState
    {
        NORMAL = 1,
        BLINKING = 2,
        DEAD = 3
    }
    public class Player
    {
        public Vector2 location;
        Vector2 force = new Vector2(0, 0);
        double angleofrot;
        int charge = 0;
        PlayerState state = PlayerState.NORMAL;
        public int Charge
        {
            get { return charge; }
            set { charge = value; }
        }
        const int multiplier = 5;
        const int upperbound = 5;
        float lastangle;
        private int hp = 5;

        public int Hp
        {
            get { return hp; }
            set {
                Game1.beginHpTrans(hp, value, 500);
                hp = value; 
            }
        }
        Vector2 dest = new Vector2(500, 500);
        bool canpress2 = false;
        bool pressed;
        int blinktimer;
        byte opacity = 255;
        public Player(Vector2 loc)
        {
            location = loc;
        }
        public void draw(SpriteBatch sb, Texture2D arrow)
        {
            Color col = Color.White;
            if (state == PlayerState.BLINKING)
            {
                col = Color.FromNonPremultiplied(255, 255, 255, opacity);
                opacity-=10;
            }
            sb.Draw(arrow, location, null, col, (float)(angleofrot + Math.PI / 2), new Vector2(arrow.Width / 2, arrow.Height), 1.0f, SpriteEffects.None, 0);
        }
        public void update(GameTime gameTime)
        {
            dest = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            if (state == PlayerState.BLINKING)
            {
                blinktimer -= gameTime.ElapsedGameTime.Milliseconds;
                if (blinktimer <= 0)
                {
                    state = PlayerState.NORMAL;
                    opacity = 255;
                }    
            }
            if (state == PlayerState.NORMAL)
            {
                foreach (Enemy e in Enemy.enemies)
                {
                    if (e.bounds.Contains((int)location.X, (int)location.Y))
                    {
                        Hp--;
                      //  Game1.beginHpTrans(Hp, --Hp, 500);
                        state = PlayerState.BLINKING;
                        blinktimer = 2000;
                        if (Hp <= 0)
                        {
                           // state = PlayerState.DEAD;
                            state = PlayerState.BLINKING;
                            Hp = 5;
                            location = new Vector2(100, 100);
                            Game1.score = 0;
                        }
                        break;
                    }
                }
            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (!pressed)
                {

                    pressed = true;
                }
                else
                {
                    if (Charge < 20)
                    {
                        Charge++;
                    }
                }
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (pressed)
                {
                    for (int i = 0; i < 10 + Charge; i++)
                    {
                        Bullet toadd = new Bullet();
                        toadd.angle = angleofrot;
                        Random r = new Random();
                        toadd.col = new Color(r.Next(255), 255, 255);
                        toadd.pos = location - (new Vector2((float)Math.Cos(angleofrot), (float)Math.Sin(angleofrot)) * i);
                        Bullet.bullets.Add(toadd);
                    }
                    pressed = false;
                    Charge = 0;
                }
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed && canpress2)
            {
                // createBurst(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
                Bullet toadd = new Bullet();
                toadd.angle = angleofrot;
                Random r = new Random();
                toadd.col = new Color(r.Next(255), 255, 255);
                toadd.pos = location;// -(new Vector2((float)Math.Cos(angleofrot), (float)Math.Sin(angleofrot)) * i);
                toadd.tracking = true;
                Bullet.bullets.Add(toadd);
                // Enemy.spawnWave();
                canpress2 = false;
            }

            else if (Mouse.GetState().RightButton == ButtonState.Released)
            {
                canpress2 = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                //apply upwards thrust

                force.Y += (gameTime.ElapsedGameTime.Milliseconds / 1000f) * multiplier;
                if (force.Y > upperbound)
                {
                    force.Y = upperbound;
                }
            }
            else
            {
                if (force.Y > 0)
                {
                    force.Y -= (gameTime.ElapsedGameTime.Milliseconds / 1000f) * (multiplier / 2f);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                force.Y -= (gameTime.ElapsedGameTime.Milliseconds / 1000f) * multiplier;
                if (force.Y < -upperbound)
                {
                    force.Y = -upperbound;
                }
            }
            else
            {
                if (force.Y < 0)
                {
                    force.Y += (gameTime.ElapsedGameTime.Milliseconds / 1000f) * (multiplier / 2f);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                force.X -= (gameTime.ElapsedGameTime.Milliseconds / 1000f) * multiplier;
                if (force.X < -upperbound)
                {
                    force.X = -upperbound;
                }
                //  angleofrot -= (gameTime.ElapsedGameTime.Milliseconds / 1000f);
            }
            else
            {
                if (force.X < 0)
                {
                    force.X += (gameTime.ElapsedGameTime.Milliseconds / 1000f) * (multiplier / 2f);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                force.X += (gameTime.ElapsedGameTime.Milliseconds / 1000f) * multiplier;
                //Console.WriteLine(gameTime.ElapsedGameTime.Milliseconds / 1000f);
                if (force.X > upperbound)
                {
                    force.X = upperbound;
                }
            }
            else
            {
                if (force.X > 0)
                {
                    force.X -= (gameTime.ElapsedGameTime.Milliseconds / 1000f) * (multiplier / 2f);
                }
            }
            if (dest != new Vector2(float.NaN, float.NaN) && Config.mouseplay)
            {
                if (Math.Abs(location.X - dest.X) > 3 || Math.Abs(location.Y - dest.Y) > 3)
                {
                    //new Vector4(location.X,location.Y,dest.X,dest.Y)
                    Vector2 tocalc = new Vector2(location.X - dest.X, location.Y - dest.Y);
                    tocalc.Normalize();
                    angleofrot = Math.Atan2(tocalc.Y, tocalc.X);
                    /*     if (MathHelper.ToDegrees((float)Math.Abs(angleofrot - lastangle)) > 2)
                          {
                              if (angleofrot - lastangle > 0)
                              {
                                  tocalc = new Vector2((float)Math.Cos(lastangle + 2), (float)Math.Sin(lastangle + 2));
                              }
                              else
                              {
                                  tocalc = new Vector2((float)Math.Cos(lastangle - 2), (float)Math.Sin(lastangle - 2));

                              }
                          } */
                    tocalc *= 6;
                    int calcx = (int)((location.X - tocalc.X));
                    // Console.WriteLine("{0} {1} {2}", tocalc.X, tocalc.Y, calcx);
                    location -= tocalc;
                    //(calcx, (int)((location.Y - tocalc.Y)), arrow.Width, arrow.Height);
                    lastangle = (float)angleofrot;
                }
                else
                {
                    dest = new Vector2(float.NaN, float.NaN);
                }

            }
            else
            {
                Vector2 tocalc = new Vector2(location.X - Mouse.GetState().X, location.Y - Mouse.GetState().Y);
                tocalc.Normalize();
                angleofrot = Math.Atan2(tocalc.Y, tocalc.X);
                Point p = Program.theGame.Window.ClientBounds.Location;
                if (Program.theGame.Window.ClientBounds.Contains(new Point((int)(location.X + force.X + p.X), (int)(location.Y + force.Y + p.Y))))
                {
                    location.X += force.X;
                    location.Y += force.Y;
                }
                else
                {
                    force.X = 0;
                    force.Y = 0;
                }
            }
        }
    }
}
