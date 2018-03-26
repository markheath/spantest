using System;

namespace SpanTest
{
    interface IAudioCapture
    {
        void Start();
        void Stop();
        //void OnDataAvailable(Func<Span<byte>> callback);
        void OnDataAvailable(AudioCallback callback);
        
        //event EventHandler<AudioCaptureEventArgs> DataAvailable;
        event EventHandler<StoppedEventArgs> RecordingStopped;
    }

    delegate void AudioCallback(Span<byte> x);

    /*
    public class AudioCaptureEventArgs : EventArgs
    {
        public AudioCaptureEventArgs(Span<byte> audio)
        {
            Buffer = audio;
        }

        public Span<byte> Buffer { get; }
    } */
}