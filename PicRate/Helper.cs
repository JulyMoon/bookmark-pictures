﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace PicRate
{
    static class ListHelper
    {
        public static List<T> RepeatedDefault<T>(int count) => Repeated(default(T), count);

        public static List<T> Repeated<T>(T value, int count)
        {
            var list = new List<T>(count);
            list.AddRange(Enumerable.Repeat(value, count));
            return list;
        }
    }

    static class CacheHelper
    {
        public static string ToBase64(string s) => Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
        public static string FromBase64(string s) => Encoding.UTF8.GetString(Convert.FromBase64String(s));

        public static byte[] Compress(byte[] toCompress)
        {
            using (var ms = new MemoryStream())
            {
                using (var gzip = new GZipStream(ms, CompressionMode.Compress, true))
                    gzip.Write(toCompress, 0, toCompress.Length);

                return ms.ToArray();
            }
        }

        public static byte[] Decompress(byte[] toDecompress)
        {
            using (var gzip = new GZipStream(new MemoryStream(toDecompress), CompressionMode.Decompress))
            {
                const int size = 4096;
                var buffer = new byte[size];
                using (var ms = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = gzip.Read(buffer, 0, size);
                        if (count > 0)
                            ms.Write(buffer, 0, count);
                    }
                    while (count > 0);
                    return ms.ToArray();
                }
            }
        }

        public static byte[] Serialize<T>(T toSerialize)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, toSerialize);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] toDeserialize)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                ms.Write(toDeserialize, 0, toDeserialize.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)bf.Deserialize(ms);
            }
        }
    }
}
