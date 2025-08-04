using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena
{
    class AutoPlay
    {
        private LevelForm levelForm;
        private System.Windows.Forms.Timer autoPlayTimer;
        private int interval = 1500; // co 1.5 sekundy

        public AutoPlay(LevelForm form)
        {
            levelForm = form;

            autoPlayTimer = new System.Windows.Forms.Timer();
            autoPlayTimer.Interval = interval;
            autoPlayTimer.Tick += AutoPlayTick;
        }

        public void Start()
        {
            autoPlayTimer.Start();
        }

        public void Stop()
        {
            autoPlayTimer.Stop();
        }

        private void AutoPlayTick(object sender, EventArgs e, IGameElement[,] grid)
        { 
            var move = AIHelper.SuggestBestMove(grid);
            if (move.startX == -1) return;

            PictureBox startBox = levelForm.Controls.OfType<PictureBox>()
                .FirstOrDefault(p => p.Tag is Point pt && pt == new Point(move.startX, move.startY));
            PictureBox endBox = levelForm.Controls.OfType<PictureBox>()
                .FirstOrDefault(p => p.Tag is Point pt && pt == new Point(move.endX, move.endY));

            if (startBox != null && endBox != null)
            {
                levelForm.HandleSwap(
                    new Point(move.startX, move.startY),
                    new Point(move.endX, move.endY),
                    startBox,
                    endBox
                );
            }
        }
    }
}
