using System;

namespace SpanTest
{
    public interface IWaveProvider
    {
        WaveFormat WaveFormat { get; }
        int Read(Span<byte> buffer);
    }
}