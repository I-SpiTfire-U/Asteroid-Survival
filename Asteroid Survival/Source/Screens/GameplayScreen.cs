using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using Microsoft.Xna.Framework.Audio;
using Asteroid_Survival.Source.Entities;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens.Transitions;

namespace Asteroid_Survival.Source.Screens
{
    internal class GameplayScreen : GameScreen
    {
        private int _MaxSpawnedAsteroids = 5;
        private bool _HasFired = false;
        private bool _IsDead = false;

        private SpriteFont _HyperspaceFont;

        private Texture2D _KeysTexture;
        private Texture2D _PlayerTexture;
        private Texture2D _BulletTexture;
        private Texture2D[] _AsteroidTextures;

        private SoundEffect _GameLoseSoundEffect;
        private SoundEffect _GameStartSoundEffect;
        private SoundEffect _PlayerShootSoundEffect;
        private SoundEffect[] _ExplosionSoundEffects;

        private Animation _PlayerMoveAnimation;
        private Player _Player;
        private readonly List<Bullet> _SpawnedBullets = [];
        private readonly List<Asteroid> _SpawnedAsteroids = [];
        private readonly Random _Random = new Random();

        public GameplayScreen(Game Game) : base(Game) { }

        private new Game Game => (Game)base.Game;

        public override void LoadContent()
        {
            base.LoadContent();

            _HyperspaceFont = Content.Load<SpriteFont>("Fonts/Hyperspace");

            _KeysTexture = Content.Load<Texture2D>("Graphics/Keys");
            _PlayerTexture = Content.Load<Texture2D>("Graphics/Player");
            _BulletTexture = Content.Load<Texture2D>("Graphics/Bullet");
            _AsteroidTextures =
            [
                Content.Load<Texture2D>("Graphics/AsteroidSmall"),
                Content.Load<Texture2D>("Graphics/AsteroidMedium"),
                Content.Load<Texture2D>("Graphics/AsteroidLarge"),
            ];

            _GameLoseSoundEffect = Content.Load<SoundEffect>("Audio/GameLose");
            _GameStartSoundEffect = Content.Load<SoundEffect>("Audio/GameStart");
            _PlayerShootSoundEffect = Content.Load<SoundEffect>("Audio/PlayerShoot");
            _ExplosionSoundEffects =
            [
                Content.Load<SoundEffect>("Audio/SmallExplosion"),
                Content.Load<SoundEffect>("Audio/MediumExplosion"),
                Content.Load<SoundEffect>("Audio/LargeExplosion")
            ];

            _Player = new Player(_PlayerTexture, 4f, -90f, 16, new Vector2(Game.ScreenWidth / 2, Game.ScreenHeight / 2));
            _PlayerMoveAnimation = new Animation(2, 2, 16, 1, 1);

            Start();
        }

        public void Start()
        {
            Game.PlayerScore = 0;
            _GameStartSoundEffect.Play();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardInput();
            UpdateBullets();
            UpdateAsteroids();

            if (_SpawnedAsteroids.Count < _MaxSpawnedAsteroids)
            {
                CreateAsteroid(_Random.Next(1, 3), new(-64, -64));
            }

            if (Game.PlayerScore > (1000 * _MaxSpawnedAsteroids) + (100 * _MaxSpawnedAsteroids))
            {
                _MaxSpawnedAsteroids += 5;
            }

            _Player.Update();
            _Player.ScreenWrap(-32, -32, Game.ScreenWidth + 32, Game.ScreenHeight + 32);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Black);
            Game.SpriteBatch.Begin();

            Game.SpriteBatch.DrawString(_HyperspaceFont, Game.PlayerScore.ToString(), new Vector2(10, 6), Color.White);
            Game.SpriteBatch.Draw(_KeysTexture, new Rectangle(10, 570, 96, 16), Color.White);

            foreach (Bullet bullet in _SpawnedBullets)
            {
                bullet.Draw(Game.SpriteBatch);
            }

            foreach (Asteroid asteroid in _SpawnedAsteroids)
            {
                asteroid.Draw(Game.SpriteBatch);
            }

            if (!_IsDead)
            {
                _Player.Draw(Game.SpriteBatch, _PlayerMoveAnimation.CurrentFrame);
            }

            Game.SpriteBatch.End();
        }

        private void KeyboardInput()
        {
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
                    _ = _GameLoseSoundEffect.Play();
                    ScreenManager.LoadScreen(new MenuScreen(Game), new FadeTransition(GraphicsDevice, Color.Black));
                    return;
                }

                if (!_HasFired)
                {
                    _SpawnedBullets.Add(new Bullet(_BulletTexture, _Player.GetRotation, _Player.GetPosition));
                    _HasFired = true;
                    _ = _PlayerShootSoundEffect.Play();
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
                if (_SpawnedBullets[i].GetPosition.X < -64 || _SpawnedBullets[i].GetPosition.X > Game.ScreenWidth + 64 || _SpawnedBullets[i].GetPosition.Y < -64 || _SpawnedBullets[i].GetPosition.Y > Game.ScreenHeight + 64)
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
                    _ = _ExplosionSoundEffects[_SpawnedAsteroids[i].Size].Play();
                    SplitAsteroid(_SpawnedAsteroids[i]);
                    _IsDead = true;
                    continue;
                }

                _SpawnedAsteroids[i].ScreenWrap(-65, -65, Game.ScreenWidth + 65, Game.ScreenHeight + 65);

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
            _ = _SpawnedAsteroids.Remove(a);
            Game.PlayerScore += 140 * (a.Size + 1);
            _ = _ExplosionSoundEffects[a.Size].Play();
        }

        private void CreateAsteroid(int size, Vector2 position) => _SpawnedAsteroids.Add(new Asteroid(_AsteroidTextures[size], size, _Random.Next(360), position));
    }
}
