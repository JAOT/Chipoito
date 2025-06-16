
using SFML.Window;

namespace Chipoito
{
    public class CPU
    {
        private Memory memory;
        private Stack<ushort> stack;
        private Display? display; // Referência passada ou atribuída

        private ushort PC;  // Program Counter
        private ushort I;   //registo de endereço
        private byte[] V;   //Registos V0 a VF
        private byte delayTimer; //Temporizador de atraso
        private byte soundTimer; //Temporizador de atraso

        private bool waitingForKey = false;
        private byte waitingRegister;
        
        private Buzzer? buzzer;

        public CPU(Memory memory)
        {
            this.memory = memory;
            PC = 0x200; // Início do programa
            this.stack = new Stack<ushort>();

            this.memory = memory;
            this.stack = new Stack<ushort>();
            this.V = new byte[16];
            this.PC = 0x200; // Início do programa
            this.I = 0;
            this.delayTimer = 0;
            this.soundTimer = 0;
        }

        public void SetBuzzer(Buzzer buzzer)
        {
            this.buzzer = buzzer;
        }
        
        public void SetDisplay(Display display)
        {
            this.display = display;
        }

        internal void Cycle()
        {

            if (waitingForKey)
                return;

            if (soundTimer > 0)
                buzzer?.Start();
            else
                buzzer?.Stop();

            if (PC + 1 >= memory.RAM.Length)
            {
                //Console.WriteLine($"Erro: tentativa de leitura fora da memória em PC={PC:X4}");
                return;
            }

            //obter opcode
            ushort opcode = (ushort)((memory[PC] << 8) | memory[PC + 1]);

            //Console.WriteLine($"[CICLO] PC={PC:X4} | Opcode={opcode:X4}");

            PC += 2;
            //executar instrução
            DecodeAndExecute(opcode);

            //atualizar temporizadores
            if (delayTimer > 0) delayTimer--;
            if (soundTimer > 0) soundTimer--;

        }
        
        private Chip8Keyboard? keyboard;

        public void SetKeyboard(Chip8Keyboard keyboard)
        {
            this.keyboard = keyboard;
        }

        private void DecodeAndExecute(ushort opcode)
        {
            // Decodificar e executar instruções

            ushort nnn = (ushort)(opcode & 0x0FFF);
            byte nn = (byte)(opcode & 0x00FF);
            byte n = (byte)(opcode & 0x000F);
            byte x = (byte)((opcode & 0x0F00) >> 8);
            byte y = (byte)((opcode & 0x00F0) >> 4);

            var o = (byte)(opcode & 0xF000);

            switch (opcode & 0xF000)
            {
                case 0x0000:
                    if (opcode == 0x00E0)
                    {
                        // 00E0 - CLS: Limpa o ecrã
                        Console.WriteLine("Instrução: CLS");
                        display.Clear();
                    }
                    else if (opcode == 0x00EE)
                    {
                        // 00EE - RET: Retorna de sub-rotina
                        Console.WriteLine("Instrução: RET");
                        PC = stack.Pop();
                    }
                    break;

                case 0x1000:
                    // 1NNN - JP addr: Salta para o endereço NNN
                    //Console.WriteLine($"Instrução: JP {nnn:X3}");
                    PC = nnn;
                    break;

                case 0x2000:
                    // 2NNN - CALL addr: Chama sub-rotina no endereço NNN
                    //Console.WriteLine($"Instrução: CALL {nnn:X3}");
                    stack.Push(PC);
                    PC = nnn;
                    break;

                case 0x3000:
                    // SE Vx, NN
                    if (V[x] == nn)
                        PC += 2;
                    break;

                case 0x4000:
                    // SNE Vx, NN
                    if (V[x] != nn)
                        PC += 2;
                    break;

                case 0x5000:
                    if ((opcode & 0x000F) == 0x0)
                    {
                        if (V[x] == V[y])
                            PC += 2;
                    }
                    else
                    {
                        //Console.WriteLine($"Opcode 5XY_ desconhecido: {opcode:X4}");
                    }
                    break;

                case 0x6000:
                    // LD Vx, NN
                    V[x] = nn;
                    break;

                case 0x7000:
                    // ADD Vx, NN
                    V[x] += nn;
                    break;

                case 0x8000:
                    switch (opcode & 0x000F)
                    {
                        case 0x0:
                            V[x] = V[y];
                            break;
                        case 0x1:
                            V[x] |= V[y];
                            break;
                        case 0x2:
                            V[x] &= V[y];
                            break;
                        case 0x3:
                            V[x] ^= V[y];
                            break;
                        case 0x4:
                            {
                                int sum = V[x] + V[y];
                                V[0xF] = (byte)(sum > 255 ? 1 : 0);
                                V[x] = (byte)(sum & 0xFF);
                            }
                            break;
                        case 0x5:
                            V[0xF] = (byte)(V[x] >= V[y] ? 1 : 0);
                            V[x] = (byte)(V[x] - V[y]);
                            break;
                        case 0x6:
                            V[0xF] = (byte)(V[x] & 0x1);
                            V[x] >>= 1;
                            break;
                        case 0x7:
                            V[0xF] = (byte)(V[y] >= V[x] ? 1 : 0);
                            V[x] = (byte)(V[y] - V[x]);
                            break;
                        case 0xE:
                            V[0xF] = (byte)((V[x] & 0x80) >> 7);
                            V[x] <<= 1;
                            break;
                        default:
                            //Console.WriteLine($"Opcode 8XY_ desconhecido: {opcode:X4}");
                            break;
                        }
                break;
                    
                case 0x9000:
                    if ((opcode & 0x000F) == 0x0)
                    {
                        if (V[x] != V[y])
                            PC += 2;
                    }
                    else
                    {
                        //Console.WriteLine($"Opcode 9XY_ desconhecido: {opcode:X4}");
                    }
                    break;

                case 0xA000:
                    I = nnn;
                    //Console.WriteLine($"LD I, {nnn:X3}");
                    break;

                case 0xB000:
                    PC = (ushort)(nnn + V[0]);
                    //Console.WriteLine($"Instrução: JP V0, {nnn:X3} → Salta para {PC:X4}");
                    break;

                case 0xC000:
                    Random rand = new Random();
                    V[x] = (byte)(rand.Next(0, 256) & nn);
                    break;

                case 0xD000:
                    {
                        byte xPos = V[x];
                        byte yPos = V[y];
                        byte height = n;
                        V[0xF] = 0;
                        //Console.WriteLine($"Desenhar sprite em ({xPos}, {yPos}) com altura {height}");

                        for (int row = 0; row < height; row++)
                        {
                            byte spriteByte = memory[I + row];

                            //Console.WriteLine($"Sprite linha {row}: {Convert.ToString(spriteByte, 2).PadLeft(8, '0')}");

                            for (int col = 0; col < 8; col++)
                            {
                                byte spritePixel = (byte)((spriteByte >> (7 - col)) & 0x1);
                                if (spritePixel == 1)
                                {
                                    if (display.TogglePixel(xPos + col, yPos + row))
                                    {
                                        V[0xF] = 1; // colisão
                                    }
                                }
                            }
                        }
                    }
                    break;

                case 0xE000:
                    switch (nn)
                    {
                        case 0x9E:
                            if (keyboard != null && keyboard.IsKeyPressed(V[x]))
                                PC += 2;
                            break;
                        case 0xA1:
                            if (keyboard != null && !keyboard.IsKeyPressed(V[x]))
                                PC += 2;
                            break;
                    }
                    break;

                case 0xF000:
                    switch (nn)
                    {
                        case 0x07:
                            V[x] = delayTimer;
                            break;
                        case 0x15:
                            delayTimer = V[x];
                            break;
                        case 0x18:
                            soundTimer = V[x];
                            break;
                        case 0x1E:
                            I += V[x];
                            break;
                        case 0x0A:
                            waitingForKey = true;
                            waitingRegister = x;
                            break;

                        case 0x29:
                            I = (ushort)(V[x] * 5); // endereço do sprite da fonte
                            break;
                        case 0x33:
                            memory[I] = (byte)(V[x] / 100);
                            memory[I + 1] = (byte)((V[x] / 10) % 10);
                            memory[I + 2] = (byte)(V[x] % 10);
                            break;
                        case 0x55:
                            for (int i = 0; i <= x; i++)
                                memory[I + i] = V[i];
                            // I += (ushort)(x + 1); // Descomenta se quiseres compatibilidade com algumas ROMs
                            break;

                        case 0x65:
                            for (int i = 0; i <= x; i++)
                                V[i] = memory[I + i];
                            break;
                    }
                    break;

                default:
                    //Console.WriteLine($"Opcode desconhecido: {opcode:X4}");
                break;
            }
        }
        
        public void OnKeyPressed(byte key)
        {
            if (waitingForKey)
            {
                V[waitingRegister] = key;
                waitingForKey = false;
            }
        }

    }
}