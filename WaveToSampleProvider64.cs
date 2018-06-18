using System;
using System.Runtime.InteropServices;

namespace SpanTest
{
    /// <summary>
    /// Helper class turning an already 64 bit floating point IWaveProvider
    /// into an ISampleProvider - hopefully not needed for most applications
    /// </summary>
    public class WaveToSampleProvider64 : SampleProviderConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the WaveToSampleProvider class
        /// </summary>
        /// <param name="source">Source wave provider, must be IEEE float</param>
        public WaveToSampleProvider64(IWaveProvider source)
            : base(source)
        {
            if (source.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                throw new ArgumentException("Must be already floating point");
            }
        }

        /// <summary>
        /// Reads from this provider
        /// </summary>
        public override int Read(Span<float> buffer)
        {
            int bytesNeeded = buffer.Length * 8;
            EnsureSourceBuffer(bytesNeeded);
            var b = new Span<byte>(sourceBuffer, 0, bytesNeeded);
            int bytesRead = source.Read(b);
            int samplesRead = bytesRead / 8;
            var b64 = MemoryMarshal.Cast<byte, double>(b); // b.NonPortableCast<byte,double>();
            for (int n = 0; n < samplesRead; n ++)
            {
                buffer[n] = (float)b64[n];
            }
            return samplesRead;
        }
    }
}
