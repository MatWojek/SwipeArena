using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO:
// Animacja wykonuje się gdy połączymy 3 elementy 
// Animacja z Bitmapy

namespace SwipeArena
{
    public class Animation
    {
        private List<Bitmap> animFrames; // Lista klatek animacji
        private int currentFrame; // Aktualna klatka
        private PictureBox pictureBox; // PictureBox, w którym animacja jest wyświetlana
        private System.Windows.Forms.Timer animationTimer; // Timer do obsługi animacji
        private Action onComplete; // Callback po zakończeniu animacji
        public bool IsCompleted { get; private set; } // Czy animacja została zakończona


        /// <summary>
        /// Konstruktor dzielący obrazek na klatki.
        /// </summary>
        /// <param name="spriteSheet">Obrazek zawierający wszystkie klatki animacji.</param>
        /// <param name="frameCount">Liczba klatek w obrazku.</param>
        public Animation(Bitmap spriteSheet, int frameCount)
        {
            animFrames = new List<Bitmap>();
            int frameWidth = spriteSheet.Width / frameCount;

            for (int i = 0; i < frameCount; i++)
            {
                Rectangle frameRect = new Rectangle(i * frameWidth, 0, frameWidth, spriteSheet.Height);
                animFrames.Add(spriteSheet.Clone(frameRect, spriteSheet.PixelFormat));
            }

            currentFrame = 0;
            IsCompleted = false;
        }

        /// <summary>
        /// Rozpocznij animację w podanym PictureBox.
        /// </summary>
        /// <param name="pictureBox">Kontrolka PictureBox, w której animacja będzie wyświetlana.</param>
        /// <param name="frameInterval">Interwał między klatkami w milisekundach.</param>
        /// <param name="onComplete">Callback wywoływany po zakończeniu animacji.</param>
        public void Start(PictureBox pictureBox, int frameInterval, Action onComplete = null)
        {
            this.pictureBox = pictureBox;
            this.onComplete = onComplete;

            animationTimer = new System.Windows.Forms.Timer { Interval = frameInterval };
            animationTimer.Tick += (s, e) =>
            {
                if (currentFrame < animFrames.Count)
                {
                    pictureBox.Image = animFrames[currentFrame]; // Ustaw aktualną klatkę
                    currentFrame++;
                }
                else
                {
                    animationTimer.Stop();
                    IsCompleted = true;
                    onComplete?.Invoke(); // Wywołaj callback po zakończeniu animacji
                }
            };
            animationTimer.Start();
        }

        /// <summary>
        /// Pobranie kolejnej klatki (jeśli potrzebujesz ręcznego sterowania animacją).
        /// </summary>
        /// <returns>Obrazek z kolejną klatką animacji.</returns>
        public Image GetNextFrame()
        {
            Bitmap frame = animFrames[currentFrame];
            currentFrame = (currentFrame + 1) % animFrames.Count;
            return frame;
        }
    }
}
