using System.IO;
using System.Threading.Tasks;

namespace CDC.Messaging.Core.Interfaces
{
    public interface ISerializer
    {
        T Deserialize<T>(byte[] array, int length);

        T Deserialize<T>(Stream stream, int length);

        T Deserialize<T>(byte[] array);

        T Deserialize<T>(Stream stream);

        byte[] SerializeToArray<T>(T value);

        Task<byte[]> SerializeToArrayAsync<T>(T value);

        string SerializeToString<T>(T value);

        T DeserializeFromString<T>(string value);

        void SerializeToStream<T>(Stream stream, T value);
    }
}
