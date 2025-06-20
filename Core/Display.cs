using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Chipoito.Core
{
    public class Display
    {
        private bool[,] screen;

        private const int Width = 64;
        private const int Height = 32;

        private const int Scale = 10;

        private RenderWindow window;
        public bool IsOpen() => window.IsOpen;
        public Display()
        {
            screen = new bool[Width, Height];

            window = new RenderWindow(new VideoMode(Width * Scale, Height * Scale), "Chipoito");
            window.Closed += (_, __) => window.Close();

        }

        public bool TogglePixel(int x, int y)
        {
            x %= Width;
            y %= Height;

            bool previous = screen[x, y];
            screen[x, y] ^= true; // XOR
            return previous && !screen[x, y]; // colisão se passou de 1 para 0
        }

        internal void Render()
        {
            window.DispatchEvents();
            window.Clear(Color.Black);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (screen[x, y])
                    {
                        RectangleShape pixel = new RectangleShape(new Vector2f(Scale, Scale))
                        {
                            Position = new Vector2f(x * Scale, y * Scale),
                            FillColor = Color.White
                        };
                        window.Draw(pixel);
                    }
                }
            }

            window.Display();

        }
        public void Clear()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    screen[x, y] = false;
        }
        public void SetKeyboard(Chip8Keyboard keyboard)
        {
            window.KeyPressed += keyboard.OnKeyPressed;
            window.KeyReleased += keyboard.OnKeyReleased;
        }
    }
}