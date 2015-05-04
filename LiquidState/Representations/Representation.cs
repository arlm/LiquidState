﻿// Author: Prasanna V. Loganathar
// Created: 2:12 AM 27-11-2014
// Project: LiquidState
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LiquidState.Representations
{
    internal class StateRepresentation<TState, TTrigger>
    {
        public readonly TState State;
        public readonly Dictionary<TriggerRepresentation<TTrigger, TState>, Func<bool>> Triggers;
        public Action OnEntryAction;
        public Action OnExitAction;

        internal StateRepresentation(TState state)
        {
            Contract.Requires(state != null);
            Contract.Ensures(State != null);

            State = state;
            // Allocate with capacity as 1 to avoid wastage of memory.
            Triggers = new Dictionary<TriggerRepresentation<TTrigger, TState>, Func<bool>>(1);
        }
    }

    internal class TriggerRepresentation<TTrigger, TState>
    {
        public readonly TTrigger Trigger;
        public Func<bool> ConditionalTriggerPredicate;
        public StateRepresentation<TState, TTrigger> NextStateRepresentation;
        public object OnTriggerAction;

        internal TriggerRepresentation(TTrigger trigger)
        {
            Contract.Requires(trigger != null);
            Contract.Ensures(Trigger != null);

            Trigger = trigger;
        }
    }
}
