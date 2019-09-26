﻿using PeNet.Utilities;
using System;
using System.Collections.Generic;

namespace PeNet.Structures
{
    /// <summary>
    ///     The IMAGE_BASE_RELOCATION structure holds information needed to relocate
    ///     the image to another virtual address.
    /// </summary>
    public class IMAGE_BASE_RELOCATION : AbstractStructure
    {
        /// <summary>
        ///     Create a new IMAGE_BASE_RELOCATION object.
        /// </summary>
        /// <param name="buff">PE binary as byte array.</param>
        /// <param name="offset">Offset to the relocation struct in the binary.</param>
        /// <param name="relocSize">Size of the complete relocation directory.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     If the SizeOfBlock is bigger than the size
        ///     of the Relocation Directory.
        /// </exception>
        public IMAGE_BASE_RELOCATION(byte[] buff, uint offset, uint relocSize)
            : base(buff, offset)
        {
            if (SizeOfBlock > relocSize)
            {
                throw new ArgumentOutOfRangeException(nameof(relocSize),
                    "SizeOfBlock cannot be bigger than size of the Relocation Directory.");
            }

            if (SizeOfBlock < 8)
            {
                throw new Exception("SizeOfBlock cannot be smaller than 8.");
            }

            ParseTypeOffsets();
        }

        /// <summary>
        ///     RVA of the relocation block.
        /// </summary>
        public uint VirtualAddress
        {
            get => Buff.BytesToUInt32(Offset);
            set => Buff.SetUInt32(Offset, value);
        }

        /// <summary>
        ///     SizeOfBlock-8 indicates how many TypeOffsets follow the SizeOfBlock.
        /// </summary>
        public uint SizeOfBlock
        {
            get => Buff.BytesToUInt32(Offset + 0x4);
            set => Buff.SetUInt32(Offset + 0x4, value);
        }

        /// <summary>
        ///     Array with the TypeOffsets for the relocation block.
        /// </summary>
        public TypeOffset[] TypeOffsets { get; private set; }

        private void ParseTypeOffsets()
        {
            List<TypeOffset> list = new List<TypeOffset>();
            for (uint i = 0; i < (SizeOfBlock - 8) / 2; i++)
            {
                list.Add(new TypeOffset(Buff, Offset + 8 + i * 2));
            }
            TypeOffsets = list.ToArray();
        }

        /// <summary>
        ///     Represents the type and offset in an
        ///     IMAGE_BASE_RELOCATION structure.
        /// </summary>
        public class TypeOffset
        {
            private readonly byte[] _buff;
            private readonly uint _offset;

            /// <summary>
            ///     Create a new TypeOffset object.
            /// </summary>
            /// <param name="buff">PE binary as byte array.</param>
            /// <param name="offset">Offset of the TypeOffset in the array.</param>
            public TypeOffset(byte[] buff, uint offset)
            {
                _buff = buff;
                _offset = offset;
            }

            /// <summary>
            ///     The type is described in the 4 lower bits of the
            ///     TypeOffset word.
            /// </summary>
            public byte Type
            {
                get
                {
                    ushort to = _buff.BytesToUInt16(_offset);
                    return (byte)(to >> 12);
                }
            }

            /// <summary>
            ///     The offset is described in the 12 higher bits of the
            ///     TypeOffset word.
            /// </summary>
            public ushort Offset
            {
                get
                {
                    ushort to = _buff.BytesToUInt16(_offset);
                    return (ushort)(to & 0xFFF);
                }
            }
        }
    }
}