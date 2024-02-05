using Asteroid_Survival.Source.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Asteroid_Survival.Source
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        // General
        private string _Message = "Start Game\n  [space]";
        private int _Score;
        private int _WindowWidth;
        private int _WindowHeight;
        private int _MaxBigAsteroids = 5;
        private bool _HasFired = false;
        private bool _IsDead = true;
        private Random _Random = new Random();
        private GraphicsDeviceManager _Graphics;
        private Animation _PlayerMoveAnimation;

        // Content
        private Texture2D _Texture2D_Player;
        private Texture2D _Texture2D_Bullet;
        private Texture2D[] _Texture2D_Asteroid;
        private SpriteFont _SpriteFont_Hyperspace;
        private SoundEffect _SoundEffect_Shoot;
        private SoundEffect _SoundEffect_Start;
        private SoundEffect _SoundEffect_Explosion1;
        private SoundEffect _SoundEffect_Explosion2;

        // Entities
        private Player _Player;
        private List<Bullet> _SpawnedBullets = new List<Bullet>();
        private List<Asteroid> _SpawnedAsteroids = new List<Asteroid>();

        public Game()
        {
            _Graphics = new GraphicsDeviceManager(this);
            _Graphics.PreferredBackBufferWidth = 600;
            _Graphics.PreferredBackBufferHeight = 600;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _WindowWidth = _Graphics.PreferredBackBufferWidth;
            _WindowHeight = _Graphics.PreferredBackBufferHeight;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Globals.SpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _Texture2D_Asteroid =
            [
                Content.Load<Texture2D>("Graphics/AsteroidSmall"),
                Content.Load<Texture2D>("Graphics/AsteroidMedium"),
                Content.Load<Texture2D>("Graphics/AsteroidLarge"),
            ];
            _Texture2D_Player = Content.Load<Texture2D>("Graphics/Player");
            _Texture2D_Bullet = Content.Load<Texture2D>("Graphics/Bullet");
            _SpriteFont_Hyperspace = Content.Load<SpriteFont>("Fonts/Hyperspace");
            _SoundEffect_Shoot = Content.Load<SoundEffect>("Audio/Shoot");
            _SoundEffect_Start = Content.Load<SoundEffect>("Audio/Start");
            _SoundEffect_Explosion1 = Content.Load<SoundEffect>("Audio/Explosion1");
            _SoundEffect_Explosion2 = Content.Load<SoundEffect>("Audio/Explosion2");

            _Player = new Player(_Texture2D_Player, 4f, -90f, 16, new Vector2(_WindowWidth / 2, _WindowHeight / 2));
            _PlayerMoveAnimation = new Animation(2, 2, 16, 1, 1);
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            GetKeyboardInput();
            UpdateBullets();
            UpdateAsteroids();

            if (_SpawnedAsteroids.Count < _MaxBigAsteroids)
            {
                CreateAsteroid(_Random.Next(1, 3), new(-64, -64));
            }

            if (_Score > 1000 * _MaxBigAsteroids + 100 * _MaxBigAsteroids)
            {
                _MaxBigAsteroids += 5;
            }

            _Player.Update();
            _Player.ScreenWrap(-32, -32, _WindowWidth + 32, _WindowHeight + 32);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            Globals.SpriteBatch.Begin();

            // TODO: Add your drawing code here
            Globals.SpriteBatch.DrawString(_SpriteFont_Hyperspace, _Score.ToString(), new Vector2(10, 6), Color.White);
            Globals.SpriteBatch.DrawString(_SpriteFont_Hyperspace, _Message, new Vector2(230, 350), Color.White);

            foreach (Bullet bullet in _SpawnedBullets)
            {
                bullet.Draw();
            }

            foreach (Asteroid asteroid in _SpawnedAsteroids)
            {
                asteroid.Draw();
            }

            if (!_IsDead)
            {
                _Player.Draw(_PlayerMoveAnimation.CurrentFrame);
            }

            Globals.SpriteBatch.End();
            base.Draw(gameTime);
        }

        private void GetKeyboardInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _Player.OffsetRotation(-4f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _Player.OffsetRotation(4f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                _PlayerMoveAnimation.Update();
                _Player.IncreaseCurrentSpeed(0.25f);
            }
            else
            {
                _PlayerMoveAnimation.Reset();
                _Player.DecreaseCurrentSpeed(0.10f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (_IsDead)
                {
                    ResetGame();
                    _SoundEffect_Start.Play();
                }

                if (!_HasFired)
                {
                    _SpawnedBullets.Add(new Bullet(_Texture2D_Bullet, _Player.GetRotation, _Player.GetPosition));
                    _HasFired = true;
                    _SoundEffect_Shoot.Play();
                }
            }
            else
            {
                _HasFired = false;
            }
        }

        private void UpdateBullets()
        {
            for (int i = _SpawnedBullets.Count - 1; i >= 0; i--)
            {
                _SpawnedBullets[i].Update();
                if (_SpawnedBullets[i].GetPosition.X < -64 || _SpawnedBullets[i].GetPosition.X > _WindowWidth + 64 || _SpawnedBullets[i].GetPosition.Y < -64 || _SpawnedBullets[i].GetPosition.Y > _WindowHeight + 64)
                {
                    _SpawnedBullets.RemoveAt(i);
                }
            }
        }

        private void UpdateAsteroids()
        {
            for (int i = _SpawnedAsteroids.Count - 1; i >= 0; i--)
            {
                _SpawnedAsteroids[i].Update();

                if (_SpawnedAsteroids[i].IsCollidingWith(_Player.GetPosition) && !_IsDead)
                {
                    EndGame();
                    SplitAsteroid(_SpawnedAsteroids[i]);
                    _SoundEffect_Explosion2.Play();
                    continue;
                }

                _SpawnedAsteroids[i].ScreenWrap(-65, -65, _WindowWidth + 65, _WindowHeight + 65);

                for (int j = 0; j < _SpawnedBullets.Count; j++)
                {
                    if (_SpawnedAsteroids[i].IsCollidingWith(_SpawnedBullets[j].GetPosition))
                    {
                        SplitAsteroid(_SpawnedAsteroids[i]);
                        _SpawnedBullets.RemoveAt(j);
                        break;
                    }
                }
            }
        }

        private void SplitAsteroid(Asteroid a)
        {
            if (a.Size > 0)
            {
                CreateAsteroid(a.Size - 1, a.GetPosition);
                CreateAsteroid(a.Size - 1, a.GetPosition);
            }
            _SpawnedAsteroids.Remove(a);
            _Score += 140 * (a.Size + 1);
            _SoundEffect_Explosion1.Play();
        }

        private void CreateAsteroid(int size, Vector2 position)
        {
            _SpawnedAsteroids.Add(new Asteroid(_Texture2D_Asteroid[size], size, _Random.Next(360), position));
        }

        private void EndGame()
        {
            _IsDead = true;
            _Message = "Game Over\n [space]";
        }

        private void ResetGame()
        {
            _Message = "";
            _Score = 0;
            _IsDead = false;
            _HasFired = true;
            _Player.SetPosition(new Vector2(_WindowWidth / 2, _WindowHeight / 2));
            _Player.SetRotation(-90f);
            _SpawnedAsteroids.Clear();
            _SpawnedBullets.Clear();
        }
    }
}
