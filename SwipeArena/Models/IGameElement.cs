using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.Models
{
    /// <summary>
    ///  Interfejs do tworzenia elementów w grze
    /// </summary>
    public interface IGameElement
    {
        string Name { get; set; }
        Image Icon { get; set; }
        Point Position { get; set; }

        void Draw(Graphics graphics);

        IGameElement Clone(); 
    }
}
