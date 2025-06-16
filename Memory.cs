namespace Chipoito
{
    public class Memory
    {
        public byte[] RAM { get; private set; }
        
        public Memory()
        {
            RAM = new byte[4096];
        }

        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= RAM.Length)
                    throw new IndexOutOfRangeException($"Acesso inválido à memória: {index}");
                return RAM[index];
            }
            set
            {
                if (index < 0 || index >= RAM.Length)
                    throw new IndexOutOfRangeException($"Escrita inválida na memória: {index}");
                RAM[index] = value;
            }
        }
    }

}
