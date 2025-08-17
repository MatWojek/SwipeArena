using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.Animations
{
    public class AnimationContext
    {

        // Dla MatchAnimation
        public Control ParentControl { get; set; } 
        public List<Point> Matches { get; set; }

        // Dla SwapAnimation
        public Point Pos1 { get; set; }
        public Point Pos2 { get; set; }
        public PictureBox Box1 { get; set; }
        public PictureBox Box2 { get; set; }
    }
}
