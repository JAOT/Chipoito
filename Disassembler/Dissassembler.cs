using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace Chipoito.Disassembler
{
    internal class Dissassembler
    {
        private string romPath;

        public Dissassembler(string romPath)
        {
            this.romPath = romPath;
            DissassembleRom(romPath);
        }

        private void DissassembleRom(string romPath)
        {
            byte[] rom = File.ReadAllBytes(romPath);
            int pc = 0x200; // Endereço inicial típico da Chip-8
            int i = 0;
            string output = "";
            while (i + 1 < rom.Length) // garante que temos 2 bytes disponíveis
            {
                ushort opcode = (ushort)(rom[i] << 8 | rom[i + 1]);
                string instruction = DecodeOpcode(opcode);
                Console.WriteLine($"{pc:X4}: {opcode:X4}    {instruction}");
                output += instruction + "\n";
                i += 2;
                pc += 2;
            }
            if (rom.Length % 2 != 0)
            {
                int lastByte = rom[^1];
                Console.WriteLine($"{pc:X4}: {lastByte:X2}");
                Console.WriteLine($"AVISO: byte isolado no fim da ROM em 0x{pc:X4}: {lastByte:X2}");
                output += $"DESCONHECIDO 0x{lastByte:X2}" + "\n";
            }
            var nome = Path.GetFileNameWithoutExtension(romPath);
            File.WriteAllText(nome+".asm", output);
        }

        private string DecodeOpcode(ushort opcode)
        {
                int x = (opcode & 0x0F00) >> 8;
                int y = (opcode & 0x00F0) >> 4;
                int n = opcode & 0x000F;
                int nn = opcode & 0x00FF;
                int nnn = opcode & 0x0FFF;

                switch (opcode & 0xF000)
                {
                    case 0x0000:
                        if (opcode == 0x00E0) return "CLS";
                        if (opcode == 0x00EE) return "RET";
                        return $"SYS 0x{nnn:X3}";
                    case 0x1000: return $"JP  0x{nnn:X3}";
                    case 0x2000: return $"CALL 0x{nnn:X3}";
                    case 0x3000: return $"SE V{x:X}, 0x{nn:X2}";
                    case 0x4000: return $"SNE V{x:X}, 0x{nn:X2}";
                    case 0x5000:
                        if (n == 0x0) return $"SE V{x:X}, V{y:X}";
                        break;
                    case 0x6000: return $"LD V{x:X}, 0x{nn:X2}";
                    case 0x7000: return $"ADD V{x:X}, 0x{nn:X2}";
                    case 0x8000:
                        return n switch
                        {
                            0x0 => $"LD V{x:X}, V{y:X}",
                            0x1 => $"OR V{x:X}, V{y:X}",
                            0x2 => $"AND V{x:X}, V{y:X}",
                            0x3 => $"XOR V{x:X}, V{y:X}",
                            0x4 => $"ADD V{x:X}, V{y:X}",
                            0x5 => $"SUB V{x:X}, V{y:X}",
                            0x6 => $"SHR V{x:X}",
                            0x7 => $"SUBN V{x:X}, V{y:X}",
                            0xE => $"SHL V{x:X}",
                            _ => $"DESCONHECIDO 8xy{n:X}"
                        };
                    case 0x9000:
                        if (n == 0x0) return $"SNE V{x:X}, V{y:X}";
                        break;

                    case 0xA000: return $"LD I, 0x{nnn:X3}";
                    case 0xB000: return $"JP V0, 0x{nnn:X3}";
                    case 0xC000: return $"RND V{x:X}, 0x{nn:X2}";
                    case 0xD000: return $"DRW V{x:X}, V{y:X}, 0x{n:X1}";

                    case 0xE000:
                        return nn switch
                        {
                            0x9E => $"SKP V{x:X}",
                            0xA1 => $"SKNP V{x:X}",
                            _ => $"DESCONHECIDO Ex{nn:X2}"
                        };
                    case 0xF000:
                        return nn switch
                        {
                            0x07 => $"LD V{x:X}, DT",
                            0x0A => $"LD V{x:X}, K",
                            0x15 => $"LD DT, V{x:X}",
                            0x18 => $"LD ST, V{x:X}",
                            0x1E => $"ADD I, V{x:X}",
                            0x29 => $"LD F, V{x:X}",
                            0x33 => $"LD BCD, V{x:X}",
                            0x55 => $"LD [I], V{x:X}",
                            0x65 => $"LD V{x:X}, [I]",
                            _ => $"DESCONHECIDO Fx{nn:X2}"
                        };
                }
                return $"DESCONHECIDO 0x{opcode:X4}";
            }
    }
}
