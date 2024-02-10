using Microsoft.Xna.Framework;

namespace Asteroid_Survival.Source
{
    internal class Animation
    {
        private int _Counter = 0;
        private int _CurrentFrame = 0;
        private int _CurrentRowPosition = 0;
        private int _CurrentColumnPosition = 0;

        private readonly int _Padding = 0;
        private readonly int _OffsetX = 0;
        private readonly int _OffsetY = 0;
        private readonly int _NumberOfFrames;
        private readonly int _NumberOfColumns;
        private readonly int _SizeOfTextures;
        private readonly int _Interval;

        internal Animation(int numberOfFrames, int numberOfColumns, int sizeOfTextures, int interval, int padding)
        {
            _NumberOfFrames = numberOfFrames;
            _NumberOfColumns = numberOfColumns;
            _SizeOfTextures = sizeOfTextures;
            _Interval = interval;
            _Padding = padding;
        }

        internal void Update()
        {
            _Counter++;
            if (_Counter >= _Interval)
            {
                _Counter = 0;
                NextFrame();
            }
        }

        private void NextFrame()
        {
            _CurrentFrame++;
            _CurrentColumnPosition++;
            if (_CurrentFrame >= _NumberOfFrames)
            {
                Reset();
            }

            if (_CurrentColumnPosition >= _NumberOfColumns)
            {
                _CurrentColumnPosition = 0;
                _CurrentRowPosition++;
            }
        }

        internal void Reset()
        {
            _CurrentFrame = 0;
            _CurrentColumnPosition = 0;
            _CurrentRowPosition = 0;
        }

        internal Rectangle? CurrentFrame => new Rectangle((_CurrentColumnPosition * (_SizeOfTextures + _Padding)) + _OffsetX, (_CurrentRowPosition * (_SizeOfTextures + _Padding)) + _OffsetY, _SizeOfTextures, _SizeOfTextures);
    }
}
