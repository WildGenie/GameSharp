﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsInjection.Core.Utilities
{
    public static class Logger
    {
        public static void Info(string message) => Write(message, ConsoleColor.White);
        public static void Warning(string message) => Write(message, ConsoleColor.Yellow);
        public static void Error(string message) => Write(message, ConsoleColor.Red);

        private static void Write(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"[CsInjection] :: {message}");
        }
    }
}
