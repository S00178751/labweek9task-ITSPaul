using Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using Sprites;
using Microsoft.Xna.Framework.Audio;

namespace LabStarter
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        string Message = "ppowell Paul Powell";
        SpriteFont messageFont;
        private Texture2D BackgroundTx;
        private Player player;
        RandomEnemy renemy;

        public ActiveScreenState current { get; private set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            current = ActiveScreenState.PLAY;
            base.Initialize();
        }
        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Helper.graphicsDevice = GraphicsDevice;
            // Also Load and Add font reference to a spritefont in the helper static class

            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(spriteBatch);
            messageFont = Content.Load<SpriteFont>("Message");

            SoundEffect[] _PlayerSounds = new SoundEffect[5];
            for (int i = 0; i < _PlayerSounds.Length; i++)
                _PlayerSounds[i] =
                    Content.Load<SoundEffect>(@"Audio/PlayerDirection/" + i.ToString());

            BackgroundTx = Content.Load<Texture2D>("background");
            player = new Player(new Texture2D[] {Content.Load<Texture2D>(@"Images/left"),
                                                Content.Load<Texture2D>(@"Images/right"),
                                                Content.Load<Texture2D>(@"Images/up"),
                                                Content.Load<Texture2D>(@"Images/down"),
                                                Content.Load<Texture2D>(@"Images/stand")},
                _PlayerSounds,
                    new Vector2(200, 200), 8, 0, 5.0f);
            renemy = new RandomEnemy(this,
                Content.Load<Texture2D>(@"Images/player"),
                new Vector2(200, 200), 14
                );
            // Load all the assets and create your objects here


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            switch (current)
            {
                case ActiveScreenState.OPENING:
                    break;
                case ActiveScreenState.PLAY:
                    player.Update(gameTime);
                    renemy.Update(gameTime);
                    break;
                case ActiveScreenState.ENDING:
                    break;
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (current)
            {
                case ActiveScreenState.OPENING:
                    break;
                case ActiveScreenState.PLAY:
                    draw_play_screen(spriteBatch);
                    
                    break;
                case ActiveScreenState.ENDING:
                    break;
            }


            spriteBatch.End();
            // TODO: Add your drawing code here
            base.Draw(gameTime);
        }

        private void draw_play_screen(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BackgroundTx, GraphicsDevice.Viewport.Bounds, Color.White);
            spriteBatch.DrawString(messageFont,
                Message, new Vector2(20, 20), Color.White);
            player.Draw(spriteBatch);
            renemy.Draw(spriteBatch);
        }
    }
}
