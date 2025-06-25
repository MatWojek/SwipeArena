using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena
{
    /// <summary>
    ///  Interfejs do tworzenia elementów w grze
    /// </summary>
    internal interface IGameElement
    {
        public string Name { get; set; }
        public Image Icon { get; set; }
        public Point Position { get; set; }

        public void Draw(Graphics graphics);

    }
}
