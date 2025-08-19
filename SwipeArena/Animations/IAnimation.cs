using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.Animations
{
    /// <summary>
    /// Interfejs ogólny do tworzenia animacji w grze
    /// </summary>
    public interface IAnimation
    { 
       void Animation(AnimationContext context, Action onComplete = null);
    }
}
