using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena
{
    /// <summary>
    /// Interfejs dla objektów pojawiających się w grze
    /// </summary>
    internal interface IFigure
    {
        /// <summary>
        /// Rysowanie objektów 
        /// </summary>
        /// <param name="graphics"></param>
        public void Draw(Graphics graphics); 
    }
}
