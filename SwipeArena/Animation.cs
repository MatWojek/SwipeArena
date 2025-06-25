using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO:
// Animacja wykonuje się gdy połączymy 3 elementy 
// Animacja z Bitmapy

namespace SwipeArena
{
    public class Animation
    {
        List<Bitmap> animFrames;
        int currentFrame;

        public Animation(Bitmap spriteSheet, int frameCount)
        {
            animFrames = new List<Bitmap>();
            int frameWidth = spriteSheet.Width / frameCount;

            for (int i = 0; i < frameCount; i++)
            {
                Rectangle frameRect = new Rectangle(i * frameWidth, 0, frameWidth, spriteSheet.Height);
                animFrames.Add(spriteSheet.Clone(frameRect, spriteSheet.PixelFormat));
            }

            currentFrame = 0;
        }

        /// <summary>
        /// Pobranie kolejnej klatki
        /// </summary>
        /// <returns></returns>
        public Image GetNextFrame()
        {
            Bitmap frame = animFrames[currentFrame];
            currentFrame = (currentFrame + 1) % animFrames.Count;
            return frame;
        }

    }
}
