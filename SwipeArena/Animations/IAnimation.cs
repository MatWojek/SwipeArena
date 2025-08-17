using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.Animations
{
    public interface IAnimation
    { 
       void Animation(AnimationContext context, Action onComplete = null);
    }
}
