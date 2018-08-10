﻿using CsInjection.Core.Hooks;
using Injectable.Helpers;
using System;
using System.Runtime.InteropServices;

namespace Injectable.Hooks
{
    internal class HookOnAfk : HookBase
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, SetLastError = true)]
        public delegate void OnAfkDelegate(IntPtr thisPtr);

        public override Delegate GetHookDelegate()
        {
            return Marshal.GetDelegateForFunctionPointer<OnAfkDelegate>(Offsets.OnAfk);
        }

        public override Delegate GetDetourDelegate()
        {
            return new OnAfkDelegate(DetourMethod);
        }

        private void DetourMethod(IntPtr thisPtr)
        {
            Console.WriteLine($"Event::ONAFK has been triggered");
            CallOriginal(thisPtr);
        }
    }
}