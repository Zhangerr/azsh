using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
namespace math
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static int score;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferMultiSampling = true;
           // graphics.GraphicsDevice.PresentationParameters.MultiSampleType = MultiSampleType.SixteenSamples;
            graphics.ApplyChanges();
            
         //   this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 100.0f);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            base.Initialize();
          
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        Texture2D arrow;
        public static Vector2 location;
        Vector2 position;
     //   Rectangle bounds;
        Texture2D prim;
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            arrow = Content.Load<Texture2D>("arrow_down");
            sf = Content.Load<SpriteFont>("SpriteFont1");
            location = new Vector2(100, 100);
            prim = new Texture2D(this.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Int32[] pixel = {0xFFFFFF}; // White. 0xFF is Red, 0xFF0000 is Blue
            prim.SetData<Int32> (pixel, 0, prim.Width * prim.Height);
            //prim.SetData<Int32>()
            // TODO: use this.Content to load your game content here
        }
        SpriteFont sf;
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        bool pressed;
        Vector2 dest = new Vector2(500,500);
        float lastangle;
        
        void createBurst(Vector2 initial)
        {
            Random r = new Random();
            for(int i = 0; i < 360; i+= r.Next(1,7)) {
                Bullet b = new Bullet(); 
                b.angle = MathHelper.ToRadians(i);
                b.pos = initial;
                b.speed = r.Next(7,15);
                b.col = Color.White;
                Bullet.bullets.Add(b);
            }
                
            
        }
        bool canpress2 = false;
        int charge = 0;
        bool canspawn = false;
        int spawntimer;
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            dest = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            // TODO: Add your update logic here
            foreach (Bullet b in Bullet.bullets)
            {
                //find out what flipping cos/sin and negating them does
                b.update();
              
            }
            foreach (Particle p in Particle.particles)
            {
                p.update(gameTime);
            }
            foreach (Particle p in Particle.dead)
            {
                Particle.particles.Remove(p);
            }
            Particle.dead.Clear();
            foreach (Bullet b in Bullet.toremove)
            {
                Bullet.bullets.Remove(b);
            }
            Bullet.toremove.Clear();
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (!pressed)
                {

                    pressed = true;
                }
                else
                {
                    if (charge < 20)
                    {
                        charge++;
                    }
                }
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (pressed)
                {
                    for (int i = 0; i < 10 + charge; i++)
                    {
                        Bullet toadd = new Bullet();
                        toadd.angle = angleofrot;
                        Random r = new Random();
                        toadd.col = new Color(r.Next(255), 255, 255);
                        toadd.pos = location - (new Vector2((float)Math.Cos(angleofrot), (float)Math.Sin(angleofrot)) * i);
                        Bullet.bullets.Add(toadd);
                    }
                    pressed = false;
                    charge = 0;
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
            
            else if(Mouse.GetState().RightButton == ButtonState.Released)
            {
                canpress2 = true;
            }
            if (Enemy.enemies.Count <= 0)
            {
                spawntimer += gameTime.ElapsedGameTime.Milliseconds;
            }
            if (spawntimer > 5000)
            {
                spawntimer = 0;
                
                Enemy.spawnWave();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && canspawn)
            {
               // Enemy.spawnWave();
                canspawn = false;
            }
               
            else if (Keyboard.GetState().IsKeyUp(Keys.Space))
            {
                canspawn = true;
            }
            int multiplier = 5;
            int upperbound = 5;
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
                    force.Y -= (gameTime.ElapsedGameTime.Milliseconds / 1000f) * (multiplier/2f);
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
                    force.Y += (gameTime.ElapsedGameTime.Milliseconds / 1000f) * (multiplier/2f);
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
                    force.X += (gameTime.ElapsedGameTime.Milliseconds / 1000f) * (multiplier/2f);
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
                    force.X -= (gameTime.ElapsedGameTime.Milliseconds / 1000f) * (multiplier/2f);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
               /* Particle p = new Particle();
                p.pos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                p.angle = 5;
                p.size = 2;
                p.alpha = 255;
                p.col = Color.White;
                Particle.particles.Add(p);*/
            }
            foreach (Enemy e in Enemy.enemies)
            {
                e.update();
            }
            foreach (Enemy e in Enemy.toRemove)
            {
                Enemy.enemies.Remove(e);
            }
            if (dest != new Vector2(float.NaN, float.NaN) && mouseplay)
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
                Point p = this.Window.ClientBounds.Location;
                if (this.Window.ClientBounds.Contains(new Point((int)(location.X + force.X + p.X), (int)(location.Y + force.Y + p.Y))))
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
            base.Update(gameTime);
        }
        Vector2 force = new Vector2(0, 0);
        double angleofrot;
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        Boolean mouseplay = false;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            Vector2 mouse = new Vector2(Mouse.GetState().X,Mouse.GetState().Y);
            spriteBatch.DrawString(sf,"Score: " + score + " Ms until next wave: " + (5000 - spawntimer) + " FPS:" + 1f / (gameTime.ElapsedGameTime.Milliseconds / 1000f), new Vector2(50,50),Color.White);
         
            spriteBatch.Draw(arrow, location,null, Color.White,(float)(angleofrot + Math.PI/2),new Vector2(arrow.Width/2,arrow.Height), 1.0f, SpriteEffects.None,0);
            spriteBatch.Draw(prim, new Rectangle(50, 10, (int) (200 * (charge / 20f)), 30), Color.SteelBlue);
            foreach (Bullet b in Bullet.bullets)
            {
                spriteBatch.Draw(prim, new Rectangle((int)b.pos.X, (int)b.pos.Y,2,2), b.col);
            }
            foreach (Enemy e in Enemy.enemies)
            {
                e.draw(spriteBatch, prim);
            }
            foreach (Particle p in Particle.particles)
            {
                p.draw(spriteBatch, prim);
            }
              //  spriteBatch.Draw(prim, new Rectangle(0, 0, (int)mouse.Length(), 2), null, Color.White, (float)(Math.Atan2(mouse.Y, mouse.X)), Vector2.Zero, SpriteEffects.None, 0);
                spriteBatch.End();
         
            base.Draw(gameTime);
        }
    }


}
