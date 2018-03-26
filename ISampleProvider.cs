using System;

namespace SpanTest
{
    public interface ISampleProvider
    {
        WaveFormat WaveFormat { get; }
        int Read(Span<float> buffer);
    }
}
