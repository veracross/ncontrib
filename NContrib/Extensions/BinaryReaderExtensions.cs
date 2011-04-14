using System.IO;

namespace NContrib.Extensions {

    public static class BinaryReaderExtensions {

        /// <summary>Reads a ushort from Big Endian data</summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public static ushort ReadUInt16BE(this BinaryReader br) {
            return br.ReadUInt16().Swap();
        }

        /// <summary>Reads a uint from Big Endian data</summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public static uint ReadUInt32BE(this BinaryReader br) {
            return br.ReadUInt32().Swap();
        }

        /// <summary>Reads a ulong from Big Endian data</summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public static ulong ReadUInt64BE(this BinaryReader br) {
            return br.ReadUInt64().Swap();
        }

    }
}
