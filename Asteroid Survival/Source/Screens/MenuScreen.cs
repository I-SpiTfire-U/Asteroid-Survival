using Asteroid_Survival.Source.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using System;
using System.Collections.Generic;

namespace Asteroid_Survival.Source.Screens
{
    internal class MenuScreen : GameScreen
    {
        private Texture2D[] _AsteroidTextures;
        private SpriteFont _HyperspaceFont;
        private readonly List<Asteroid> _SpawnedAsteroids = [];
        private readonly Random _Random = new Random();

        public MenuScreen(Game Game) : base(Game) { }
        private new Game Game => (Game)base.Game;

        public override void LoadContent()
        {
            base.LoadContent();

            _AsteroidTextures =
            [
                Content.Load<Texture2D>("Graphics/AsteroidSmall"),
                Content.Load<Texture2D>("Graphics/AsteroidMedium"),
                Content.Load<Texture2D>("Graphics/AsteroidLarge"),
            ];

            _HyperspaceFont = Content.Load<SpriteFont>("Fonts/Hyperspace");

            Start();
        }

        public void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                int size = _Random.Next(0, 3);
                _SpawnedAsteroids.Add(new Asteroid(_AsteroidTextures[size], size, _Random.Next(360), new(-64, -64)));
            }
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = _SpawnedAsteroids.Count - 1; i >= 0; i--)
            {
                _SpawnedAsteroids[i].Update();
                _SpawnedAsteroids[i].ScreenWrap(-65, -65, Game.ScreenWidth + 65, Game.ScreenHeight + 65);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                ScreenManager.LoadScreen(new GameplayScreen(Game), new FadeTransition(GraphicsDevice, Color.Black));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Black);
            Game.SpriteBatch.Begin();

            if (Game.PlayerScore > 0)
            {
                Game.SpriteBatch.DrawString(_HyperspaceFont, "Score:", new Vector2(258, 200), Color.White);
                Game.SpriteBatch.DrawString(_HyperspaceFont, Game.PlayerScore.ToString(), new Vector2(246, 220), Color.White);
                Game.SpriteBatch.DrawString(_HyperspaceFont, "Game Over", new Vector2(235, 300), Color.White);
            }
            else
            {
                Game.SpriteBatch.DrawString(_HyperspaceFont, "Start Game", new Vector2(230, 300), Color.White);
            }
            Game.SpriteBatch.DrawString(_HyperspaceFont, "[Space]", new Vector2(252, 320), Color.White);

            foreach (Asteroid asteroid in _SpawnedAsteroids)
            {
                asteroid.Draw(Game.SpriteBatch);
            }

            Game.SpriteBatch.End();
        }
    }
}
