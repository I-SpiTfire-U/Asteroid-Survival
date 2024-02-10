using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroid_Survival.Source
{
    internal class Sprite
    {
        protected float Rotation;
        protected Vector2 Position;
        protected readonly Vector2 Origin;
        protected readonly Texture2D Texture;

        internal Sprite(Texture2D texture, float rotation, int size, Vector2 position)
        {
            Texture = texture;
            Rotation = MathHelper.ToRadians(rotation);
            Position = position;
            Origin = new Vector2(size / 2, size / 2);
        }

        internal Sprite(Texture2D texture, float rotation, Vector2 position)
        {
            Texture = texture;
            Rotation = MathHelper.ToRadians(rotation);
            Position = position;
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        internal virtual void Update() { }

        internal virtual void Draw(Rectangle? source = null) => Globals.SpriteBatch.Draw(Texture, Position, source, Color.White, Rotation, Origin, 1, SpriteEffects.None, 0);

        internal void ScreenWrap(int minWidth, int minHeight, int maxWidth, int maxHeight)
        {
            if (Position.X < minWidth)
            {
                Position.X = maxWidth - 1;
            }
            if (Position.X > maxWidth)
            {
                Position.X = minWidth + 1;
            }
            if (Position.Y < minHeight)
            {
                Position.Y = maxHeight - 1;
            }
            if (Position.Y > maxHeight)
            {
                Position.Y = minHeight + 1;
            }
        }

        internal void OffsetRotation(float degrees) => Rotation += MathHelper.ToRadians(degrees);

        internal float GetRotation => MathHelper.ToDegrees(Rotation);

        internal void SetRotation(float degrees) => Rotation = MathHelper.ToRadians(degrees);

        internal Vector2 GetPosition => Position;

        internal void SetPosition(Vector2 position) => Position = position;
    }
}
