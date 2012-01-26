using System;

namespace NContrib {

    /// <summary>Byte-swaps unsigned numerics for endian changing</summary>
    [CLSCompliant(false)]
    public static class Endian {

        /// <summary>Byte-swaps a <see cref="UInt16"/></summary>
        /// <param name="word"><see cref="UInt16"/> to swap the byte order of.</param>
        /// <returns>Byte order-swapped <see cref="UInt16"/></returns>
        public static ushort Swap(this ushort word) {
            return (ushort)((0x00FF) & (word >> 8) | (0xFF00) & (word << 8));
        }

        /// <summary>Byte-swaps a <see cref="UInt32"/></summary>
        /// <param name="word"><see cref="UInt32"/> to swap the byte-order of.</param>
        /// <returns>Byte order-swapped <see cref="UInt32"/></returns>
        public static uint Swap(this uint word) {
            return (  (0x000000FF) & (word >> 24)
                    | (0x0000FF00) & (word >> 8)
                    | (0x00FF0000) & (word << 8)
                    | (0xFF000000) & (word << 24));
        }

        /// <summary>Swaps the byte order of a <see cref="UInt64"/></summary>
        /// <param name="word"><see cref="UInt64"/> to swap the byte order of.</param>
        /// <returns>Byte order-swapped <see cref="UInt64"/></returns>
        public static ulong Swap(this ulong word) {
            return (  (0x00000000000000FF) & (word >> 56)
                    | (0x000000000000FF00) & (word >> 40)
                    | (0x0000000000FF0000) & (word >> 24)
                    | (0x00000000FF000000) & (word >> 8)
                    | (0x000000FF00000000) & (word << 8)
                    | (0x0000FF0000000000) & (word << 24)
                    | (0x00FF000000000000) & (word << 40)
                    | (0xFF00000000000000) & (word << 56));
        }
    }
}
