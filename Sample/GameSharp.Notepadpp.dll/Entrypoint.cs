﻿using GameSharp.Core.Memory;
using GameSharp.Core.Native.Enums;
using GameSharp.Core.Services;
using GameSharp.Internal;
using GameSharp.Internal.Memory;
using RGiesecke.DllExport;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace GameSharp.Notepadpp
{
    public class Entrypoint
    {
        static GameSharpProcess Process = GameSharpProcess.Instance;
        

        [DllExport]
        public static void Main()
        {
            LoggingService.Info("I have been injected!");

            LoggingService.Info("Calling MessageBoxW!");
            if (Functions.SafeMessageBoxFunction.Call<int>(IntPtr.Zero, "Through a SafeFunctionCall method", "Caption", (uint)0) == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            LoggingService.Info("Enabling hook on MessageBoxW!");
            HookMessageBoxW messageBoxHook = new HookMessageBoxW();
            messageBoxHook.Enable();

            TestForDebugger();
        }

        private static void TestForDebugger()
        {
            while(true)
            {
                IsDebuggerPresent();
                NtQueryInformationProcess(ProcessInformationClass.ProcessDebugPort);
                NtQueryInformationProcess(ProcessInformationClass.ProcessDebugObjectHandle);
                NtQueryInformationProcess(ProcessInformationClass.ProcessDebugFlags);

                Thread.Sleep(1000);
            }
        }

        private static void IsDebuggerPresent()
        {
            if (Functions.IsDebuggerPresent.Call<bool>(null))
            {
                LoggingService.Info("IsDebuggerPresent() => We're being debugged!");
            }
        }

        // https://docs.microsoft.com/en-us/windows/win32/api/winternl/nf-winternl-ntqueryinformationprocess
        // https://github.com/processhacker/processhacker/blob/master/phnt/include/ntpsapi.h#L98 #bc35992
        private static void NtQueryInformationProcess(ProcessInformationClass flag)
        {
            using (IMemoryAddress result = Process.AllocateManagedMemory(IntPtr.Size))
            {
                if (Functions.NtQueryInformationProcess.Call<int>(Process.Handle, (int)flag, result.Address, (uint)4, null) == 0)
                    LoggingService.Error($"Couldn't query NtQueryInformationProcess, Error code: {Marshal.GetLastWin32Error()}");

                LoggingService.Info($"{flag.ToString()} => Result {result.Read<IntPtr>().ToString("X")}");
            }
        }
    }
}
