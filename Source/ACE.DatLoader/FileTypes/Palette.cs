using System.Collections.Generic;
using System.IO;

namespace ACE.DatLoader.FileTypes
{
    /// <summary>
    /// These are client_portal.dat files starting with 0x04. 
    /// </summary>
    [DatFileType(DatFileType.Palette)]
    public class Palette : IUnpackable
    {
        public uint Id { get; private set; }

        /// <summary>
        /// Color data is stored in ARGB format (Alpha, Red, Green, Blue--each are two bytes long)
        /// </summary>
        public List<uint> Colors { get; } = new List<uint>();

        public void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();

            Colors.Unpack(reader);
        }

        public static Palette ReadFromDat(uint fileId)
        {
            // Check the FileCache so we don't need to hit the FileSystem repeatedly
            if (DatManager.PortalDat.FileCache.TryGetValue(fileId, out var result))
                return (Palette)result;

            DatReader datReader = DatManager.PortalDat.GetReaderForFile(fileId);

            var obj = new Palette();

            using (var memoryStream = new MemoryStream(datReader.Buffer))
            using (var reader = new BinaryReader(memoryStream))
                obj.Unpack(reader);

            // Store this object in the FileCache
            DatManager.PortalDat.FileCache[fileId] = obj;

            return obj;
        }
    }
}
