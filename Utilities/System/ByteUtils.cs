using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Utilities.System
{
    public static class ByteUtils
    {
        /// <summary>
        /// [Serializable()] and [NonSerialized()] to class and properties
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ToArray(object obj)
        {
            if (obj == null) { return null; }

            var binForm = new BinaryFormatter();
            using (var memStream = new MemoryStream())
            {
                binForm.Serialize(memStream, obj);
                return memStream.ToArray();
            }
        }

        public static TObject ToObject<TObject>(byte[] arrBytes)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = (TObject)binForm.Deserialize(memStream);
            return obj;
        }
    }
}
