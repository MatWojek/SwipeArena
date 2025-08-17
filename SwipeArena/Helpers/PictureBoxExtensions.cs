using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.Helpers
{
    public static class PictureBoxExtensions
    {
        public static PictureBox WithEventHandlers(this PictureBox pic, LevelForm form)
        {
            pic.MouseDown += form.Pic_MouseDown;
            pic.DragEnter += form.Pic_DragEnter;
            pic.DragDrop += form.Pic_DragDrop;
            pic.MouseEnter += form.Pic_MouseEnter;
            pic.MouseLeave += form.Pic_MouseLeave;
            pic.MouseMove += form.Pic_MouseMove;
            pic.Click += form.Pic_Click;
            return pic;
        }
    }
}
