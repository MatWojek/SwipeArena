using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.Models
{
    /// <summary>
    /// Tworzenie nowych obiektów w grze
    /// </summary>
    public class GameElement : IGameElement
    {
        public string Name { get; set; }
        public Image Icon { get; set; }
        public Point Position { get; set; }

        public GameElement(string name, Image icon, Point position)
        {
            Name = name;
            Icon = icon;
            Position = position;
        }

        /// <summary>
        /// Tworzy kopię bieżącego obiektu GameElement
        /// </summary>
        /// <returns></returns>
        public IGameElement Clone()
        {
            return new GameElement(Name, Icon, Position);
        }

        /// <summary>
        /// Rysowanie elementu na podanym obiekcie Graphics
        /// </summary>
        /// <param name="graphics">Obiekt Graphics, na którym element zostanie narysowany</param>
        public void Draw(Graphics graphics)
        {
            graphics.DrawImage(Icon, Position.X, Position.Y, 64, 64); // Rysowanie obrazu w pozycji elementu
        }
    }
}
