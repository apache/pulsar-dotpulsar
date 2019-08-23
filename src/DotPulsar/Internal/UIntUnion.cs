using System.Runtime.InteropServices;

namespace DotPulsar.Internal
{
    [StructLayout(LayoutKind.Explicit)]
    public struct UIntUnion
    {
        public UIntUnion(byte b0, byte b1, byte b2, byte b3)
        {
            UInt = 0;
            B0 = b0;
            B1 = b1;
            B2 = b2;
            B3 = b3;
        }

        public UIntUnion(uint value)
        {
            B0 = 0;
            B1 = 0;
            B2 = 0;
            B3 = 0;
            UInt = value;
        }

        [FieldOffset(0)]
        public byte B0;
        [FieldOffset(1)]
        public byte B1;
        [FieldOffset(2)]
        public byte B2;
        [FieldOffset(3)]
        public byte B3;

        [FieldOffset(0)]
        public uint UInt;
    }
}
