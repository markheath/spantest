using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SpanTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            SpeedTest();
            using (var wo = new WaveOutEvent())
            {
                var sg = new SignalGenerator() { Gain = 0.2f};
                sg.SweepLengthSecs = 5;
                sg.Frequency = 200;
                sg.FrequencyEnd = 2000;
                sg.Type = SignalGeneratorType.Sweep;

                var mp3 = new Mp3FileReader("test.mp3");
                

                await wo.InitAsync(mp3.ToSampleProvider());
                wo.Play();
                wo.PlaybackStopped += (s,e)=> Console.WriteLine($"Stopped {e.Exception}");
                Console.WriteLine("playing...");
                while(wo.PlaybackState != PlaybackState.Stopped)
                {
                    Console.Write(".");
                    await Task.Delay(500);
                }
                Console.WriteLine("Finished");
            }
        }

        static void SpeedTest()
        {
            var ms = new MemoryStream(File.ReadAllBytes("test.mp3"));
            var sw = Stopwatch.StartNew();
            var totalRead = 0;
            using (var reader = new Mp3FileReader(ms))
            {
                var b = new Span<byte>(new byte[reader.WaveFormat.AverageBytesPerSecond / 10]);
                var read = 0;
                while ((read = reader.Read(b)) > 0)
                {
                    totalRead += read;
                }
            }
            Console.WriteLine($"Decompressed {ms.Length} bytes of MP3 into {totalRead} bytes of audio in {sw.ElapsedMilliseconds}ms");
        }


        void Test()
        {
                        var arr = new byte[10];
            Span<byte> bytes = arr; // Implicit cast from T[] to Span<T>

            void AssertEqual(int a, int b) { if (!a.Equals(b)) { Console.WriteLine($"Expected {a} got {b}"); } }
            Span<byte> slicedBytes = bytes.Slice(start: 5, length: 2);
            slicedBytes[0] = 42;
            slicedBytes[1] = 43;
            AssertEqual(42, slicedBytes[0]);
            AssertEqual(43, slicedBytes[1]);
            AssertEqual(arr[5], slicedBytes[0]);
            AssertEqual(arr[6], slicedBytes[1]);
            //slicedBytes[2] = 44; // Throws IndexOutOfRangeException
            bytes[2] = 45; // OK
            AssertEqual(arr[2], bytes[2]);
            AssertEqual(45, arr[2]);

            var floats = MemoryMarshal.Cast<byte, float>(bytes); //bytes.NonPortableCast<byte,float>();
            floats[0] = 1.0f;
            floats[1] = 2.0f;
            AssertEqual(42, slicedBytes[0]);
            Console.WriteLine("Hello World!");
        }
    }
}
