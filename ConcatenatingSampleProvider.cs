using System;
using System.Collections.Generic;
using System.Linq;

namespace SpanTest
{
    /// <summary>
    /// Sample Provider to concatenate multiple sample providers together
    /// </summary>
    public class ConcatenatingSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider[] providers;
        private int currentProviderIndex;

        /// <summary>
        /// Creates a new ConcatenatingSampleProvider
        /// </summary>
        /// <param name="providers">The source providers to play one after the other. Must all share the same sample rate and channel count</param>
        public ConcatenatingSampleProvider(IEnumerable<ISampleProvider> providers)
        {
            if (providers == null) throw new ArgumentNullException(nameof(providers));
            this.providers = providers.ToArray();
            if (this.providers.Length == 0) throw new ArgumentException("Must provide at least one input", nameof(providers));
            if (this.providers.Any(p => p.WaveFormat.Channels != WaveFormat.Channels)) throw new ArgumentException("All inputs must have the same channel count", nameof(providers));
            if (this.providers.Any(p => p.WaveFormat.SampleRate != WaveFormat.SampleRate)) throw new ArgumentException("All inputs must have the same sample rate", nameof(providers));
        }

        /// <summary>
        /// The WaveFormat of this Sample Provider
        /// </summary>
        public WaveFormat WaveFormat => providers[0].WaveFormat;

        /// <summary>
        /// Read Samples from this sample provider
        /// </summary>
        public int Read(Span<float> buffer)
        {
            var read = 0;
            while (read < buffer.Length && currentProviderIndex < providers.Length)
            {
                var needed = buffer.Length - read;
                var s2 = buffer.Slice(read,needed);
                var readThisTime = providers[currentProviderIndex].Read(s2);
                read += readThisTime;
                if (readThisTime == 0) currentProviderIndex++;
            }
            return read;
        }
    }
}