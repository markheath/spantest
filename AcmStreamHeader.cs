using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace SpanTest
{
    class AcmStreamHeader : IDisposable
    {
        private readonly AcmStreamHeaderStruct streamHeader;
        private Memory<byte> sourceBuffer;
        private readonly MemoryHandle hSourceBuffer;
        private Memory<byte> destBuffer;
        private readonly MemoryHandle hDestBuffer;
        private readonly IntPtr streamHandle;
        private bool firstTime;

        public AcmStreamHeader(IntPtr streamHandle, int sourceBufferLength, int destBufferLength)
        {
            streamHeader = new AcmStreamHeaderStruct();
            sourceBuffer = new Memory<byte>(new byte[sourceBufferLength]);
            hSourceBuffer = sourceBuffer.Pin();

            destBuffer = new Memory<byte>(new byte[destBufferLength]);
            hDestBuffer = destBuffer.Pin();

            this.streamHandle = streamHandle;
            firstTime = true;
            //Prepare();
        }

        private unsafe void Prepare()
        {
            streamHeader.cbStruct = Marshal.SizeOf(streamHeader);
            streamHeader.sourceBufferLength = sourceBuffer.Length;
            streamHeader.sourceBufferPointer = (IntPtr)hSourceBuffer.Pointer;
            streamHeader.destBufferLength = destBuffer.Length;
            streamHeader.destBufferPointer = (IntPtr)hDestBuffer.Pointer;
            MmException.Try(AcmInterop.acmStreamPrepareHeader(streamHandle, streamHeader, 0), "acmStreamPrepareHeader");
        }

        private unsafe void Unprepare()
        {
            streamHeader.sourceBufferLength = sourceBuffer.Length;
            streamHeader.sourceBufferPointer = (IntPtr)hSourceBuffer.Pointer;
            streamHeader.destBufferLength = destBuffer.Length;
            streamHeader.destBufferPointer = (IntPtr)hDestBuffer.Pointer;

            MmResult result = AcmInterop.acmStreamUnprepareHeader(streamHandle, streamHeader, 0);
            if (result != MmResult.NoError)
            {
                //if (result == MmResult.AcmHeaderUnprepared)
                throw new MmException(result, "acmStreamUnprepareHeader");
            }
        }

        public void Reposition()
        {
            firstTime = true;
        }

        public int Convert(int bytesToConvert, out int sourceBytesConverted)
        {
            Prepare();
            try
            {
                streamHeader.sourceBufferLength = bytesToConvert;
                streamHeader.sourceBufferLengthUsed = bytesToConvert;
                AcmStreamConvertFlags flags = firstTime ? (AcmStreamConvertFlags.Start | AcmStreamConvertFlags.BlockAlign) : AcmStreamConvertFlags.BlockAlign;
                MmException.Try(AcmInterop.acmStreamConvert(streamHandle, streamHeader, flags), "acmStreamConvert");
                firstTime = false;
                System.Diagnostics.Debug.Assert(streamHeader.destBufferLength == destBuffer.Length, "Codecs should not change dest buffer length");
                sourceBytesConverted = streamHeader.sourceBufferLengthUsed;
            }
            finally
            {
                Unprepare();
            }

            return streamHeader.destBufferLengthUsed;
        }

        public Memory<byte> SourceBuffer => sourceBuffer;

        public Memory<byte> DestBuffer => destBuffer;

        #region IDisposable Members

        bool disposed = false;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                //Unprepare();
                sourceBuffer = null;
                destBuffer = null;
                hSourceBuffer.Dispose();
                hDestBuffer.Dispose();
            }
            disposed = true;
        }

        ~AcmStreamHeader()
        {
            System.Diagnostics.Debug.Assert(false, "AcmStreamHeader dispose was not called");
            Dispose(false);
        }
        #endregion
    }
}