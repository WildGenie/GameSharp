﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CsInjection.Core.Native;

namespace CsInjection.Core.Helpers
{
    public class ThreadContextHelper
    {
        public Structs.Context GetThreadContext()
        {
            // Get the first thread id.
            var curThread = Process.GetCurrentProcess().Threads[0]; 

            Structs.Context Context = new Structs.Context();
            Context.ContextFlags = (uint)Enums.ContextFlags.CONTEXT_CONTROL;
            IntPtr hThread = Kernel32.OpenThread(Enums.ThreadAccessFlags.GET_CONTEXT, false, (uint)curThread.Id);

            if (hThread == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            Kernel32.SuspendThread(hThread);

            if (!Kernel32.GetThreadContext(hThread, ref Context))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            if (!Kernel32.CloseHandle(hThread))
                throw new Win32Exception("Cannot close thread handle.");

            Kernel32.ResumeThread(hThread);

            return Context;
        }
    }
}