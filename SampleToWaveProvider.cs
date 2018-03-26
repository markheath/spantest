using System;
using System.Diagnostics;

namespace SpanTest
{
    /// <summary>
    /// Helper class for when you need to convert back to an IWaveProvider from
    /// an ISampleProvider. Keeps it as IEEE float
    /// </summary>
    public class SampleToWaveProvider : IWaveProvider
    {
        private readonly ISampleProvider source;

        /// <summary>
        /// Initializes a new instance of the WaveProviderFloatToWaveProvider class
        /// </summary>
        /// <param name="source">Source wave provider</param>
        public SampleToWaveProvider(ISampleProvider source)
        {
            if (source.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                throw new ArgumentException("Must be already floating point");
            }
            this.source = source;
        }

        /// <summary>
        /// Reads from this provider
        /// </summary>
        public int Read(Span<byte> buffer)
        {
            var f = buffer.NonPortableCast<byte,float>();
            Debug.Assert(f.Length == buffer.Length / 4);
            int samplesRead = source.Read(f);
            return samplesRead * 4;
        }

        /// <summary>
        /// The waveformat of this WaveProvider (same as the source)
        /// </summary>
        public WaveFormat WaveFormat => source.WaveFormat;
    }
}
