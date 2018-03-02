using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libtcod;
using Archmage.Engine.DataStructures;

namespace Archmage.Engine
{
    class AnimatedImage
    {
        TCODImage _imageSheet;


        public IntVector2 ImageSize { get; protected set; }

        public int Frames { get; protected set; }

        public float ImageIndex { get; set; }

        public bool Reverse { get; set; }


        public AnimatedImage(string path, IntVector2 imageSize, int frames)
        {
            _imageSheet = new TCODImage(path);
            ImageSize = imageSize;
            Frames = frames;

            ImageIndex = 0F;
            Reverse = false;
        }

        public void AnimationStep(float dT)
        {
            if (!Reverse)
            {
                ImageIndex += dT;

                if (ImageIndex >= Frames)
                {
                    ImageIndex = Frames-1;
                    Reverse = true;
                }
            }
            else
            {
                ImageIndex -= dT;

                if (ImageIndex <= 0)
                {
                    ImageIndex = 1;
                    Reverse = false;
                }
            }
        }

        public void Draw(IntVector2 position)
        {
            int frameIndex = ((int)Math.Floor(ImageIndex)) % Frames;
            Draw(position, frameIndex);
        }

        public void Draw(IntVector2 position, int frame)
        {
            //Check if we are being asked to draw a frame out of range
            if (frame > Frames)
                return;

            IntVector2 subImagePosition = new IntVector2(ImageSize.X * frame,0);

            for (int x = 0; x < ImageSize.X; x++)
            {
                for (int y = 0; y < ImageSize.Y; y++)
                {
                    TCODConsole.root.setCharBackground(
                        position.X + x, position.Y + y,
                        _imageSheet.getPixel(subImagePosition.X + x, subImagePosition.Y + y));
                }
            }
        }
    }
}
