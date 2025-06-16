
using SFML.Window;
using System.Collections.Generic;
using static SFML.Window.Keyboard;

namespace Chipoito
{
    public class Chip8Keyboard
    {
        private readonly Dictionary<Key, byte> keyMap;
        private readonly bool[] keys;

        public Chip8Keyboard()
        {
            keys = new bool[16];

            keyMap = new Dictionary<Key, byte>
            {
                { Key.Num1, 0x1 },
                { Key.Num2, 0x2 },
                { Key.Num3, 0x3 },
                { Key.Num4, 0xC },
                { Key.Q,    0x4 },
                { Key.W,    0x5 },
                { Key.E,    0x6 },
                { Key.R,    0xD },
                { Key.A,    0x7 },
                { Key.S,    0x8 },
                { Key.D,    0x9 },
                { Key.F,    0xE },
                { Key.Z,    0xA },
                { Key.X,    0x0 },
                { Key.C,    0xB },
                { Key.V,    0xF },
            };
        }

        public void OnKeyPressed(object? sender, KeyEventArgs e)
        {
            if (keyMap.TryGetValue(e.Code, out byte chip8Key))
            {
                Console.WriteLine($"Tecla {chip8Key} pressionada!!!");

                keys[chip8Key] = true;
                cpu?.OnKeyPressed(chip8Key);
            }
        }

        public void OnKeyReleased(object? sender, KeyEventArgs e)
        {
            Console.WriteLine($"Tecla livre!!!");
            if (keyMap.TryGetValue(e.Code, out byte chip8Key))
                keys[chip8Key] = false;
        }

        public bool IsKeyPressed(byte key)
        {
            //Console.WriteLine($"Verificar tecla {key:X}: {keys[key]}");
            return keys[key];
        }

        public byte WaitForKeyPress()
        {
            while (true)
            {
                for (byte i = 0; i < 16; i++)
                {
                    if (keys[i])
                        return i;
                }
                System.Threading.Thread.Sleep(1);
            }
        }

        private CPU? cpu;

        public void SetCPU(CPU cpu)
        {
            this.cpu = cpu;
        }

    }
}
