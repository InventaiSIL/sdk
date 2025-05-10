#if UNITY_EDITOR || UNITY_STANDALONE
using UnityEngine;
#else
using System;
#endif

namespace Inventai
{
    public static class Logger
    {
        public static void WriteLine(string message)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
                        Debug.Log(message);
#else
            Console.WriteLine(message);
#endif
        }

        public static void WriteWarning(string message)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
                        Debug.LogWarning(message);
#else
            Console.WriteLine($"WARNING: {message}");
#endif
        }

        public static void WriteError(string message)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
                        Debug.LogError(message);
#else
            Console.WriteLine($"ERROR: {message}");
#endif
        }
    }
}