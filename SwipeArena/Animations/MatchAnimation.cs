using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SwipeArena.Animations
{
    public class MatchAnimation : IAnimation
    {
        /// <summary>
        /// Animacja po połączeniu 3 elementów (usuwanie elementów) 
        /// </summary>
        /// <param name="parentControl"></param>
        /// <param name="matches"></param>
        /// <param name="onAnimationComplete"></param>
        public void Animation(AnimationContext context, Action onComplete = null)
        {
            var matches = context.Matches;
            var parentControl = context.ParentControl;

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            int blinkCount = 0;
            timer.Interval = 150;

            timer.Tick += (s, e) =>
            {
                foreach (var match in matches)
                {
                    PictureBox pic = parentControl.Controls.OfType<PictureBox>()
                        .FirstOrDefault(p => p.Tag is Point position && position == match);
                    if (pic != null)
                    {
                        pic.Visible = !pic.Visible;
                    }
                }

                blinkCount++;
                if (blinkCount >= 6)
                {
                    timer.Stop();
                    foreach (var match in matches)
                    {
                        PictureBox pic = parentControl.Controls.OfType<PictureBox>()
                            .FirstOrDefault(p => p.Tag is Point position && position == match);
                        if (pic != null) pic.Visible = true;
                    }

                    onComplete?.Invoke();
                }
            };

            timer.Start();
        }
    }
}
