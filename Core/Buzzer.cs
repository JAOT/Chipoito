using SFML.Audio;

namespace Chipoito.Core
{
    public class Buzzer
    {
        private Sound sound;
        private SoundBuffer buffer;

        public Buzzer()
        {
            // Gera um tom simples (ex: 440Hz, 100ms)
            var samples = GenerateSineWave(440, 44100, 0.1f);
            buffer = new SoundBuffer(samples, 1, 44100);
            sound = new Sound(buffer);
            sound.Loop = true;
        }

        public void Start()
        {
            if (sound.Status != SoundStatus.Playing)
                sound.Play();
        }

        public void Stop()
        {
            if (sound.Status == SoundStatus.Playing)
                sound.Stop();
        }

        private short[] GenerateSineWave(float frequency, uint sampleRate, float durationSeconds)
        {
            int sampleCount = (int)(sampleRate * durationSeconds);
            short[] samples = new short[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                samples[i] = (short)(MathF.Sin(2 * MathF.PI * frequency * t) * short.MaxValue * 0.25f);
            }
            return samples;
        }
    }
}
