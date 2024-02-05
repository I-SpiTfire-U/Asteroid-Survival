using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Asteroid_Survival.Source.Entities
{
    internal class Bullet : Sprite
    {
        private float _Speed = 5f;

        internal Bullet(Texture2D texture, float rotation, int size, Vector2 position) : base(texture, rotation, size, position)
        {
            
        }

        internal Bullet(Texture2D texture, float rotation, Vector2 position) : base(texture, rotation, position)
        {
            
        }

        internal override void Update()
        {
            // TODO: Add your update code here
            Position += new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation)) * _Speed;

            base.Update();
        }

        internal override void Draw(Rectangle? source = null)
        {
            // TODO: Add your draw code here

            base.Draw(source);
        }
    }
}
