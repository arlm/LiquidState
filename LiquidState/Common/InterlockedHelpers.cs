﻿// Author: Prasanna V. Loganathar
// Created: 10:58 PM 28-01-2015
// Project: LiquidState
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Threading;

namespace LiquidState.Common
{
    internal static class InterlockedHelpers
    {
        public static void SpinWaitUntilCompareExchangeSucceeds(ref int location, int value, int comparand)
        {
            // Take fast path if possible.
            if (Interlocked.CompareExchange(ref location, value, comparand) == comparand) return;

            var spinWait = new SpinWait();
            do
            {
                spinWait.SpinOnce();
            } while (Interlocked.CompareExchange(ref location, value, comparand) != comparand);
        }
    }

    /// <summary>
    ///     A struct is used for lesser over-head. But be very cautious about where its used.
    ///     And it should never be marked readonly, since the compiler will start reacting by creating copies
    ///     of mutation.
    /// </summary>
    internal struct InterlockedBlockingMonitor
    {
        private int busy;

        public bool IsBusy
        {
            get { return Interlocked.CompareExchange(ref busy, -1, -1) > 0; }
        }

        /// <summary>
        ///     WARNING: This method has to be not be used with awaits in-between Entry and Exit.
        ///     Task continuations expected to be run on the same thread (eg: UI context) will result in a deadlock.
        ///     This is also the reason why this functionality is not a part of the InterlockedMonitor itself, and is
        ///     isolated.
        /// </summary>
        public void Enter()
        {
            InterlockedHelpers.SpinWaitUntilCompareExchangeSucceeds(ref busy, 1, 0);
        }

        public void Exit()
        {
            Interlocked.Exchange(ref busy, 0);
        }
    }

    /// <summary>
    ///     A struct is used for lesser over-head. But be very cautious about where its used.
    ///     And it should never be marked readonly, since the compiler will start reacting by creating copies
    ///     of mutation.
    /// </summary>
    internal struct InterlockedMonitor
    {
        private int busy;

        public bool IsBusy
        {
            get { return Interlocked.CompareExchange(ref busy, -1, -1) > 0; }
        }

        public bool TryEnter()
        {
            return Interlocked.CompareExchange(ref busy, 1, 0) == 0;
        }

        public void Exit()
        {
            Interlocked.Exchange(ref busy, 0);
        }
    }
}
