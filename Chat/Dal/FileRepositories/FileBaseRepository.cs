using System.IO;

namespace Chat.Repositories
{
    public abstract class FileBaseRepository<T>
    {
        public void SaveToFile(T item, string path)
        {
            using var writer = new BinaryWriter(File.Open(path, FileMode.Create));
            Write(writer, item);
        }

        protected abstract void Write(BinaryWriter writer, T item);

        public T GetFromFile(string path)
        {
            using var reader = new BinaryReader(File.OpenRead(path));
            return Read(reader);
        }

        protected abstract T Read(BinaryReader reader);
    }
}