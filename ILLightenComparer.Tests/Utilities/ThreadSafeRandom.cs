﻿using System;

namespace ILLightenComparer.Tests.Utilities
{
    internal sealed class ThreadSafeRandom
    {
        private static readonly Random Global = new Random();
        [ThreadStatic] private static Random _local;

        public static double NextDouble()
        {
            return GetInstance().NextDouble();
        }

        public static int Next(int minValue, int maxValue)
        {
            return GetInstance().Next(minValue, maxValue);
        }

        private static Random GetInstance()
        {
            if (_local != null)
            {
                return _local;
            }

            return _local = new Random(GetSeed());
        }

        private static int GetSeed()
        {
            lock (Global)
            {
                return Global.Next();
            }
        }
    }
}
