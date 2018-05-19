﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using CsInjection.Core.Native;

namespace CsInjection.Core.Utilities
{
    public class SigScanner
    {
        /// <summary>
        ///     Contains all the bytes of the specified module.
        /// </summary>
        private byte[] _moduleBytes { get; }

        /// <summary>
        ///     The <see cref="ProcessModule"/> that has been selected to scan for patterns.
        /// </summary>
        private ProcessModule _selectedModule { get; }

        /// <summary>
        ///     The base address of the module.
        /// </summary>
        private IntPtr _moduleBase { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SigScanner" /> class.
        /// </summary>
        /// <param name="module"><see cref="ProcessModule"/> which we are going to scan.</param>
        public SigScanner(ProcessModule module)
        {
            _selectedModule = module;
            _moduleBase = module.BaseAddress;
            _moduleBytes = Kernel32.ReadProcessMemory<byte[]>(module.BaseAddress, module.ModuleMemorySize);
        }

        /// <summary>
        ///     Find a pattern that matches the string.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public IntPtr FindPattern(string pattern, int offset = 0)
        {
            byte[] arrPattern = ParsePatternString(pattern);

            for (int index = 0; index < _moduleBytes.Length; index++)
            {
                if (_moduleBytes[index] != arrPattern[0])
                    continue;

                if (PatternCheck(index, arrPattern))
                {
                    return _moduleBase + index + int.Parse(offset.ToString("X"), System.Globalization.NumberStyles.HexNumber);
                }
            }

            return IntPtr.Zero;
        }

        /// <summary>
        ///     Parses the pattern and changes the values to byte array understandable logic.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private byte[] ParsePatternString(string pattern)
        {
            List<byte> patternbytes = new List<byte>();

            foreach (var curByte in pattern.Split(' '))
            {
                // when we have a ? it's a variable, otherwise convert it to a byte.
                patternbytes.Add(curByte == "?" ? (byte)0x0 : Convert.ToByte(curByte, 16));
            }
                
            return patternbytes.ToArray();
        }

        /// <summary>
        ///     Checks the values if they match with the provided pattern.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private bool PatternCheck(int index, byte[] pattern)
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                // Skip this byte in case its a variable.
                if (pattern[i] == 0x0)
                    continue;

                if (pattern[i] != _moduleBytes[index + i])
                {
                    // Increase the index with the i we stopped at so we don't repeat scanning those bytes again.
                    index += i;
                    return false;
                }
            }

            return true;
        }
    }
}