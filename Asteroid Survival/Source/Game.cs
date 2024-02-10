using Asteroid_Survival.Source.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace Asteroid_Survival.Source
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static int PlayerScore;
        public static int ScreenWidth = 600;
        public static int ScreenHeight = 600;

        private readonly GraphicsDeviceManager _Graphics;
        private readonly ScreenManager _ScreenManager;
        public SpriteBatch SpriteBatch;

        public Game()
        {
            _Graphics = new GraphicsDeviceManager(this);
            _ScreenManager = new ScreenManager();
            Components.Add(_ScreenManager);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _Graphics.PreferredBackBufferWidth = ScreenWidth;
            _Graphics.PreferredBackBufferHeight = ScreenHeight;
            _Graphics.ApplyChanges();

            _ScreenManager.LoadScreen(new MenuScreen(this), new FadeTransition(GraphicsDevice, Color.Black));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
