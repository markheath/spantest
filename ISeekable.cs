namespace SpanTest
{
    interface ISeekable
    {
        long Position { get; }
        void Seek(long position);
    }
}