using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Asteroid_Survival.Source.Entities
{
    internal class Player : Sprite
    {
        private float _CurrentSpeed = 0f;
        private readonly float _MaxSpeed;

        internal Player(Texture2D texture, float maxSpeed, float rotation, int size, Vector2 position) : base(texture, rotation, size, position) => _MaxSpeed = maxSpeed;

        internal Player(Texture2D texture, float maxSpeed, float rotation, Vector2 position) : base(texture, rotation, position) => _MaxSpeed = maxSpeed;

        internal override void Update()
        {
            Position += new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation)) * _CurrentSpeed;
            base.Update();
        }

        internal void IncreaseCurrentSpeed(float speed)
        {
            if (_CurrentSpeed + speed < _MaxSpeed)
            {
                _CurrentSpeed += speed;
                return;
            }
            _CurrentSpeed = _MaxSpeed;
        }

        internal void DecreaseCurrentSpeed(float speed)
        {
            if (_CurrentSpeed - speed > 0f)
            {
                _CurrentSpeed -= speed;
                return;
            }
            _CurrentSpeed = 0f;
        }
    }
}
