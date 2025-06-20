
namespace Chipoito
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Chip8 chip8 = new Chip8();
            string romPath = "C:\\Users\\joseo\\Downloads\\Chip8-Database\\Chip8-Games\\C-Zero (Ethan Pini)(2019).ch8";
            chip8.LoadProgram(romPath);
            //Dissassembler dissassembler = new Dissassembler(romPath);

            while (true)
            {
                chip8.Cycle();
                chip8.UpdateDisplay();
                Thread.Sleep(2); // Aproximadamente 500 Hz
            }
        }


    }
}
