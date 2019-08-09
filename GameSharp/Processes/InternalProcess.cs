﻿using GameSharp.Memory.Internal;
using GameSharp.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GameSharp.Processes
{
    public sealed class InternalProcess : IProcess
    {
        static InternalProcess _instance;

        public static InternalProcess GetInstance => _instance ?? (_instance = new InternalProcess());

        private Process Process { get; } = Process.GetCurrentProcess();

        public ProcessModule LoadLibrary(string libraryPath, bool resolveReferences = true)
        {
            if (!File.Exists(libraryPath))
                throw new FileNotFoundException(libraryPath);

            bool failed = resolveReferences
                ? Kernel32.LoadLibrary(libraryPath) == IntPtr.Zero
                : Kernel32.LoadLibraryExW(libraryPath, IntPtr.Zero, Enums.LoadLibraryFlags.DontResolveDllReferences) == IntPtr.Zero;

            if (failed)
                throw new Win32Exception($"Couldn't load the library {libraryPath}.");

            return GetProcessModule(Path.GetFileName(libraryPath));
        }

        public ProcessModule GetProcessModule(string moduleName)
        {
            Process.Refresh();

            IEnumerable<ProcessModule> moduleList = Process.Modules.Cast<ProcessModule>();

            ProcessModule module = moduleList.SingleOrDefault(m => string.Equals(m.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase));

            return module;
        }

        public T CallFunction<T>(SafeFunction safeFunction, params object[] parameters) => safeFunction.Call<T>(parameters);
    }
}
