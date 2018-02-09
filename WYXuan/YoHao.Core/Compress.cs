using System;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace YoHao.Core
{
    public class Compress
    {
        public static byte[] EnCompress(byte[] input)
        {
            if (input != null)
            {
                var ms = new MemoryStream();
                var stream = new GZipStream(ms, CompressionMode.Compress);
                stream.Write(input, 0, input.Length);
                stream.Close();
                return ms.ToArray();
            }
            else
            {
                return input;
            }
        }

        public static byte[] DeCompress(byte[] input)
        {
            if (input != null)
            {
                var ms = new MemoryStream();
                var outms = new MemoryStream(input);
                var stream = new GZipStream(outms, CompressionMode.Decompress);
                var buffer = new byte[100];
                int length = 0;
                while ((length = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    ms.Write(buffer, 0, length);
                }
                stream.Close();
                var outdata = ms.ToArray();
                outms.Close();
                ms.Close();
                return outdata;
            }
            else
            {
                return input;
            }

        }

        public static string EnCompress(string input)
        {
            if (input != null)
            {
                var mStream = new MemoryStream();
                var gStream = new GZipStream(mStream, CompressionMode.Compress);

                var bw = new BinaryWriter(gStream);
                bw.Write(Encoding.UTF8.GetBytes(input));
                bw.Close();

                gStream.Close();
                var outs = Convert.ToBase64String(mStream.ToArray());
                mStream.Close();
                return outs;
            }
            else
            {
                return input;
            }
        }

        public static string DeCompress(string input)
        {
            if (input != null)
            {
                var data = Convert.FromBase64String(input);
                var mStream = new MemoryStream(data);
                var gStream = new GZipStream(mStream, CompressionMode.Decompress);
                var streamR = new StreamReader(gStream);
                var outs = streamR.ReadToEnd();
                mStream.Close();
                gStream.Close();
                streamR.Close();
                return outs;
            }
            else
            {
                return input;
            }
        }
    }
}
