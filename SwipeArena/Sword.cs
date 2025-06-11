using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena
{
    /// <summary>
    /// Miecz, jako objekt pojawiający się w grze 
    /// </summary>
    internal class Sword : IFigure
    {
        [Description("Size")]
        public int Size { get; set; }

        [Description("Position")]
        public Point Position { get; set; }

        private Bitmap swordBitmap;

        public Sword()
        {
            // Wczytanie bitmapy z zasobów
            swordBitmap = new Bitmap("objectsPixelArt.webp");
        }

        public void Draw(Graphics graphics)
        {
            // Sprawdzenie, czy bitmapa została poprawnie wczytana
            if (swordBitmap != null)
            {
                // Określenie obszaru źródłowego (część obrazu, np. miecz)
                Rectangle sourceRect = new Rectangle(0, 0, 32, 32); // Współrzędne i rozmiar miecza w obrazie

                // Określenie obszaru docelowego (gdzie narysować na ekranie)
                Rectangle destRect = new Rectangle(Position.X, Position.Y, Size, Size);

                // Rysowanie wybranego fragmentu obrazu
                graphics.DrawImage(swordBitmap, destRect, sourceRect, GraphicsUnit.Pixel);
            }
        }

    }
}
