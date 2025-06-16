
using SFML.System;
using System;
using System.IO;

namespace Chipoito
{
    internal class Chip8
    {
        public Memory memory;
        public CPU cpu;
        public Display display;
        public Chip8Keyboard keyboard;
        public Buzzer buzzer;
        public Chip8()
        {
            memory = new Memory();
            cpu = new CPU(memory);
            display = new Display();
            keyboard = new Chip8Keyboard();
            buzzer = new Buzzer();
            keyboard.SetCPU(cpu);
            cpu.SetDisplay(display);
            display.SetKeyboard(keyboard);
            cpu.SetKeyboard(keyboard);
            cpu.SetBuzzer(buzzer);
        }
        internal void Cycle()
        {
            cpu.Cycle();
        }

        internal void LoadProgram(string path)
        {
            byte[] program = File.ReadAllBytes(path);
            for (int i = 0; i < program.Length; i++)
            {
                memory[0x200 + i] = program[i];
            }
        }

        internal void UpdateDisplay()
        {
            display.Render();
        }
    }
}