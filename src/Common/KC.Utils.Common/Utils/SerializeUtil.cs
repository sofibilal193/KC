using System;
using System.IO;
using ProtoBuf;

namespace KC.Utils.Common
{
    public static class SerializeUtil
    {
        public static byte[]? ToByteArray(this object obj)
        {
            if (obj == null)
                return null;

            using MemoryStream memoryStream = new();
            Serializer.Serialize(memoryStream, obj);
            return memoryStream.ToArray();
        }

        public static T? FromByteArray<T>(this byte[] byteArray)
        {
            if (byteArray == null)
            {
                return default;
            }

            using MemoryStream memoryStream = new();
            // Ensure that our stream is at the beginning.
            memoryStream.Write(byteArray, 0, byteArray.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var val = Serializer.Deserialize<T>(memoryStream);

            if (val is T t)
            {
                return t;
            }
            try
            {
                return (T?)Convert.ChangeType(val, typeof(T));
            }
            catch (InvalidCastException)
            {
                return default;
            }
        }
    }
}
