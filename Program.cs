namespace Chipoito
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Chip8 chip8 = new Chip8();
            chip8.LoadProgram("roms/Animal Race (fix)(Brian Astle)(1977).ch8");

            while (true)
            {
                chip8.Cycle();
                chip8.UpdateDisplay();
                Thread.Sleep(2); // Aproximadamente 500 Hz
            }
        }
    }
}
