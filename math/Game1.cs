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
        public static Queue<string> messages = new Queue<string>();
        Texture2D arrow;
        //   Rectangle bounds;
        Texture2D prim;
        public static Player mainPlayer;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
              Content.RootDirectory = "Content1";
            Console.WriteLine(Content.RootDirectory);       
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
            mainPlayer = new Player(new Vector2(100, 100));
            mainPlayer.Hp = 5;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            arrow = Content.Load<Texture2D>("arrow_down");
            sf = Content.Load<SpriteFont>("SpriteFont1");
            //    location = new Vector2(100, 100);
            prim = new Texture2D(this.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Int32[] pixel = { 0xFFFFFF }; // White. 0xFF is Red, 0xFF0000 is Blue
            prim.SetData<Int32>(pixel, 0, prim.Width * prim.Height);
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
        void createBurst(Vector2 initial)
        {
            Random r = new Random();
            for (int i = 0; i < 360; i += r.Next(1, 7))
            {
                Bullet b = new Bullet();
                b.angle = MathHelper.ToRadians(i);
                b.pos = initial;
                b.speed = r.Next(7, 15);
                b.col = Color.White;
                Bullet.bullets.Add(b);
            }
        }
        bool canspawn = false;
        int spawntimer;

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            mainPlayer.update(gameTime);
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
            if (hpTransitioning)
            {
                progressSoFar += gameTime.ElapsedGameTime.Milliseconds;
                hpVal = MathHelper.Lerp(oldHP, newHP, (float)progressSoFar / transTime);
                if (progressSoFar >= transTime)
                {
                    hpTransitioning = false;
                }
            }
            base.Update(gameTime);
        }
        public static float hpVal;
        static bool hpTransitioning;
        static int oldHP, newHP, transTime,progressSoFar;
        public static void beginHpTrans(int oldhp, int newHp, int time)
        {
            hpTransitioning = true;
            oldHP = oldhp;
            newHP = newHp;
            transTime = time;
            progressSoFar = 0;
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            Vector2 mouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            spriteBatch.DrawString(sf, "Score: " + score + " Ms until next wave: " + (5000 - spawntimer) + " FPS:" + 1f / (gameTime.ElapsedGameTime.Milliseconds / 1000f), new Vector2(50, 50), Color.White);
            spriteBatch.Draw(prim, new Rectangle(50, 10, (int)(200 * (mainPlayer.Charge / 20f)), 30), Color.SteelBlue);
            spriteBatch.Draw(prim, new Rectangle(270, 10, (int)(200 * (hpVal / 5f)), 30), Color.Red);
            mainPlayer.draw(spriteBatch, arrow);
            foreach (Bullet b in Bullet.bullets)
            {
                spriteBatch.Draw(prim, new Rectangle((int)b.pos.X, (int)b.pos.Y, 2, 2), b.col);
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
