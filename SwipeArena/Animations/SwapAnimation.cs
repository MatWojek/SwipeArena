using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.Animations
{
    internal class SwapAnimation : IAnimation
    {
        /// <summary>
        /// Animacja przesunięcia elementów
        /// </summary>
        /// <param name="onComplete"></param>
        public void Animation(AnimationContext context, Action onComplete = null)
        {
            var pos1 = context.Pos1;
            var pos2 = context.Pos2;
            var box1 = context.Box1;
            var box2 = context.Box2;

            var originalLocation1 = box1.Location;
            var originalLocation2 = box2.Location;

            var animationTimer = new System.Windows.Forms.Timer { Interval = 2 };
            int step = 0;
            int totalSteps = 4;

            animationTimer.Tick += (s, e) =>
            {
                step++;
                box1.Location = new Point(
                    originalLocation1.X + (originalLocation2.X - originalLocation1.X) * step / totalSteps,
                    originalLocation1.Y + (originalLocation2.Y - originalLocation1.Y) * step / totalSteps
                );
                box2.Location = new Point(
                    originalLocation2.X + (originalLocation1.X - originalLocation2.X) * step / totalSteps,
                    originalLocation2.Y + (originalLocation1.Y - originalLocation2.Y) * step / totalSteps
                );

                if (step >= totalSteps)
                {
                    animationTimer.Stop();
                    animationTimer.Dispose();

                    SwapImages(box1, box2);

                    onComplete?.Invoke();
                }
            };

            animationTimer.Start();
        }

        /// <summary>
        /// Zamienia obrazki PictureBoxów
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        void SwapImages(PictureBox a, PictureBox b)
        {
            // Zamiana obrazów między dwoma PictureBox
            var tempImage = a.Image;
            a.Image = b.Image;
            b.Image = tempImage;
        }
    }
}
