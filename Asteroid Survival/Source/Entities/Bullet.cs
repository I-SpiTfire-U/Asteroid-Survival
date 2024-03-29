﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Asteroid_Survival.Source.Entities
{
    internal class Bullet : Sprite
    {
        private readonly float _Speed = 5f;

        internal Bullet(Texture2D texture, float rotation, int size, Vector2 position) : base(texture, rotation, size, position) { }

        internal Bullet(Texture2D texture, float rotation, Vector2 position) : base(texture, rotation, position) { }

        internal override void Update()
        {
            Position += new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation)) * _Speed;
            base.Update();
        }
    }
}
