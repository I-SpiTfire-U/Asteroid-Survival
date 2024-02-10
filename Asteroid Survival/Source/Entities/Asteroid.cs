using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Asteroid_Survival.Source.Entities
{
    internal class Asteroid : Sprite
    {
        internal readonly int Size;
        private readonly int _Radius;

        internal Asteroid(Texture2D texture, int asteroidSize, float rotation, int size, Vector2 position) : base(texture, rotation, size, position)
        {
            Size = asteroidSize;
            _Radius = size / 2;
        }

        internal Asteroid(Texture2D texture, int asteroidSize, float rotation, Vector2 position) : base(texture, rotation, position)
        {
            Size = asteroidSize;
            _Radius = texture.Width / 2;
        }

        internal override void Update()
        {
            Position += new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation)) * (3 - Size);
            base.Update();
        }

        internal bool IsCollidingWith(Vector2 objectPosition) => Vector2.Distance(Position, objectPosition) < _Radius;
    }
}
