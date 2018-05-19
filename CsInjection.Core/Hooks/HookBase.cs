﻿using System;
using CsInjection.Core.Helpers;
using System.Diagnostics;

namespace CsInjection.Core.Hooks
{
    /// <summary>
    ///     Extend from <see cref="HookBase"/> with a hook you want to use.
    /// </summary>
    public class HookBase : IHook
    {
        protected Detour Detour;

        public virtual string GetName()
        {
            throw new NotImplementedException();
        }

        public void InstallHook()
        {
            Console.WriteLine($"Hooking {GetName()}");
            Detour = new Detour(GetToHookDelegate(), GetToDetourDelegate());
            Detour.Enable();
        }

        public virtual Delegate GetToHookDelegate()
        {
            throw new NotImplementedException();
        }

        public virtual Delegate GetToDetourDelegate()
        {
            throw new NotImplementedException();
        }
    }
}
