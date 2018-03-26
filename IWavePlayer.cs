using System;
using System.Threading.Tasks;

namespace SpanTest
{
    public interface IWavePlayer : IDisposable
    {
        Task InitAsync(IWaveProvider provider);
        void Play();
        void Stop();
    }
}