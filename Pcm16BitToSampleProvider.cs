using System;
using System.Runtime.InteropServices;

namespace SpanTest
{
    /// <summary>
    /// Converts an IWaveProvider containing 16 bit PCM to an
    /// ISampleProvider
    /// </summary>
    public class Pcm16BitToSampleProvider : SampleProviderConverterBase
    {
        /// <summary>
        /// Initialises a new instance of Pcm16BitToSampleProvider
        /// </summary>
        /// <param name="source">Source wave provider</param>
        public Pcm16BitToSampleProvider(IWaveProvider source)
            : base(source)
        {
        }

        /// <summary>
        /// Reads samples from this sample provider
        /// </summary>
        /// <param name="buffer">Sample buffer</param>
        /// <returns>Number of samples read</returns>
        public override int Read(Span<float> buffer)
        {
            int sourceBytesRequired = buffer.Length * 2;
            EnsureSourceBuffer(sourceBytesRequired);
            var sbuf = new Span<byte>(sourceBuffer,0,sourceBytesRequired);
            int bytesRead = source.Read(sbuf);
            int outIndex = 0;
            var buf16 = MemoryMarshal.Cast<byte, short>(sbuf);   //sbuf.NonPortableCast<byte,short>();
            for(int n = 0; n < bytesRead / 2; n++)
            {
                buffer[outIndex++] = buf16[n] / 32768f;
            }
            return bytesRead / 2;
        }
    }
}
