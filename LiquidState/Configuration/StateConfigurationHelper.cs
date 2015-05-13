// Author: Prasanna V. Loganathar
// Created: 3:36 PM 07-12-2014
// Project: LiquidState
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using LiquidState.Common;
using LiquidState.Representations;

namespace LiquidState.Configuration
{
    public class StateConfigurationHelper<TState, TTrigger>
    {
        private readonly Dictionary<TState, StateRepresentation<TState, TTrigger>> config;
        private readonly StateRepresentation<TState, TTrigger> currentStateRepresentation;

        internal StateConfigurationHelper(Dictionary<TState, StateRepresentation<TState, TTrigger>> config,
                                          TState currentState)
        {
            //Contract.Requires(config != null);
            //Contract.Requires(currentState != null);

            Contract.Ensures(this.config != null);
            Contract.Ensures(currentStateRepresentation != null);

            this.config = config;
            currentStateRepresentation = FindOrCreateStateRepresentation(currentState, config);
        }

        public StateConfigurationHelper<TState, TTrigger> OnEntry(Action action)
        {
            currentStateRepresentation.OnEntryAction = action;
            return this;
        }

        public StateConfigurationHelper<TState, TTrigger> OnExit(Action action)
        {
            currentStateRepresentation.OnExitAction = action;
            return this;
        }

        public StateConfigurationHelper<TState, TTrigger> PermitReentry(TTrigger trigger)
        {
            //Contract.Requires(trigger != null);
            Contract.Assume(currentStateRepresentation.State != null);

            return PermitInternal(null, trigger, currentStateRepresentation.State, null);
        }

        public StateConfigurationHelper<TState, TTrigger> PermitReentry(TTrigger trigger, Action onEntryAction)
        {
            //Contract.Requires(trigger != null);
            Contract.Assume(currentStateRepresentation.State != null);

            return PermitInternal(null, trigger, currentStateRepresentation.State, onEntryAction);
        }

        public StateConfigurationHelper<TState, TTrigger> PermitReentry<TArgument>(
            ParameterizedTrigger<TTrigger, TArgument> trigger, Action<TArgument> onEntryAction)
        {
            //Contract.Requires(trigger != null);
            Contract.Assume(currentStateRepresentation.State != null);

            return PermitInternal(null, trigger, currentStateRepresentation.State, onEntryAction);
        }

        public StateConfigurationHelper<TState, TTrigger> PermitReentryIf(Func<bool> predicate, TTrigger trigger)
        {
            //Contract.Requires(trigger != null);
            Contract.Assume(currentStateRepresentation.State != null);

            return PermitInternal(predicate, trigger, currentStateRepresentation.State, null);
        }

        public StateConfigurationHelper<TState, TTrigger> PermitReentryIf(Func<bool> predicate, TTrigger trigger,
                                                                          Action onEntryAction)
        {
            //Contract.Requires(trigger != null);
            Contract.Assume(currentStateRepresentation.State != null);

            return PermitInternal(predicate, trigger, currentStateRepresentation.State, onEntryAction);
        }

        public StateConfigurationHelper<TState, TTrigger> PermitReentryIf<TArgument>(Func<bool> predicate,
                                                                                     ParameterizedTrigger<TTrigger, TArgument> trigger,
                                                                                     Action<TArgument> onEntryAction)
        {
            //Contract.Requires(trigger != null);
            Contract.Assume(currentStateRepresentation.State != null);

            return PermitInternal(predicate, trigger, currentStateRepresentation.State, onEntryAction);
        }

        public StateConfigurationHelper<TState, TTrigger> Permit(TTrigger trigger, TState resultingState)
        {
            //Contract.Requires(trigger != null);
            //Contract.Requires(resultingState != null);

            return PermitInternal(null, trigger, resultingState, null);
        }

        public StateConfigurationHelper<TState, TTrigger> Ignore(TTrigger trigger)
        {
            //Contract.Requires(trigger != null);

            return IgnoreInternal(null, trigger);
        }

        public StateConfigurationHelper<TState, TTrigger> IgnoreIf(Func<bool> predicate, TTrigger trigger)
        {
            //Contract.Requires(trigger != null);

            return IgnoreInternal(predicate, trigger);
        }

        public StateConfigurationHelper<TState, TTrigger> Permit(TTrigger trigger, TState resultingState,
                                                                 Action onEntryAction)
        {
            //Contract.Requires(trigger != null);
            //Contract.Requires(resultingState != null);

            return PermitInternal(null, trigger, resultingState, onEntryAction);
        }

        public StateConfigurationHelper<TState, TTrigger> Permit<TArgument>(
            ParameterizedTrigger<TTrigger, TArgument> trigger, TState resultingState,
            Action<TArgument> onEntryAction)
        {
            //Contract.Requires(trigger != null);
            //Contract.Requires(resultingState != null);

            return PermitInternal(null, trigger, resultingState, onEntryAction);
        }

        public StateConfigurationHelper<TState, TTrigger> PermitIf(Func<bool> predicate, TTrigger trigger,
                                                                   TState resultingState)
        {
            //Contract.Requires(trigger != null);
            //Contract.Requires(resultingState != null);

            return PermitInternal(predicate, trigger, resultingState, null);
        }

        public StateConfigurationHelper<TState, TTrigger> PermitIf(Func<bool> predicate, TTrigger trigger,
                                                                   TState resultingState, Action onEntryAction)
        {
            //Contract.Requires(trigger != null);
            //Contract.Requires(resultingState != null);

            return PermitInternal(predicate, trigger, resultingState, onEntryAction);
        }

        public StateConfigurationHelper<TState, TTrigger> PermitIf<TArgument>(Func<bool> predicate,
                                                                              ParameterizedTrigger<TTrigger, TArgument> trigger,
                                                                              TState resultingState, Action<TArgument> onEntryAction)
        {
            //Contract.Requires(trigger != null);
            //Contract.Requires(resultingState != null);

            return PermitInternal(predicate, trigger, resultingState, onEntryAction);
        }

        internal static StateRepresentation<TState, TTrigger> FindOrCreateStateRepresentation(TState state,
                                                                                              Dictionary<TState, StateRepresentation<TState, TTrigger>> config)
        {
            //Contract.Requires(state != null);
            //Contract.Requires(config != null);

            Contract.Ensures(Contract.Result<StateRepresentation<TState, TTrigger>>() != null);

            StateRepresentation<TState, TTrigger> rep;
            if (config.TryGetValue(state, out rep))
            {
                return rep;
            }

            rep = new StateRepresentation<TState, TTrigger>(state);
            config[state] = rep;

            return rep;
        }

        internal static TriggerRepresentation<TTrigger, TState> FindOrCreateTriggerRepresentation(TTrigger trigger,
                                                                                                  Func<bool> predicate,
                                                                                                  StateRepresentation<TState, TTrigger> stateRepresentation)
        {
            //Contract.Requires(stateRepresentation != null);
            //Contract.Requires(trigger != null);

            Contract.Ensures(Contract.Result<TriggerRepresentation<TTrigger, TState>>() != null);

            var repList = FindTriggerRepresentation(trigger, stateRepresentation);
            if (repList != null && repList.Count == 1)
            {
                var rep = repList[0];
                if (rep.ConditionalTriggerPredicate == predicate)
                {
                    Contract.Assume(rep.Trigger != null);
                    return rep;
                }
            }
                
            foreach (var rep in repList)
            {
                if (rep.Trigger.Equals(trigger) &&
                    rep.ConditionalTriggerPredicate == predicate)
                {
                    Contract.Assume(rep.Trigger != null);
                    return rep;
                }
            }

            var newRep = new TriggerRepresentation<TTrigger, TState>(trigger);
            stateRepresentation.Triggers.Add(newRep, null);
            return newRep;
        }

        internal static List<TriggerRepresentation<TTrigger, TState>> FindTriggerRepresentation(TTrigger trigger,
                                                                                                StateRepresentation<TState, TTrigger> stateRepresentation)
        {
            var list = from state in stateRepresentation.Triggers
                                where state.Key.Trigger.Equals(trigger)
                                select state.Key;
            return list.ToList();
        }

        private StateConfigurationHelper<TState, TTrigger> PermitInternal(Func<bool> predicate, TTrigger trigger,
                                                                          TState resultingState, Action onEntryAction)
        {
            //Contract.Requires<ArgumentNullException>(trigger != null);
            //Contract.Requires<ArgumentNullException>(resultingState != null);

            var rep = FindOrCreateTriggerRepresentation(trigger, predicate, currentStateRepresentation);

            rep.NextStateRepresentation = FindOrCreateStateRepresentation(resultingState, config);
            rep.OnTriggerAction = onEntryAction;
            rep.ConditionalTriggerPredicate = predicate;

            return this;
        }

        private StateConfigurationHelper<TState, TTrigger> PermitInternal<TArgument>(Func<bool> predicate,
                                                                                     ParameterizedTrigger<TTrigger, TArgument> trigger,
                                                                                     TState resultingState, Action<TArgument> onEntryAction)
        {
            //Contract.Requires<ArgumentNullException>(trigger != null);
            //Contract.Requires<ArgumentNullException>(resultingState != null);

            Contract.Assume(trigger.Trigger != null);

            var rep = FindOrCreateTriggerRepresentation(trigger.Trigger, predicate, currentStateRepresentation);

            rep.NextStateRepresentation = FindOrCreateStateRepresentation(resultingState, config);
            rep.OnTriggerAction = onEntryAction;
            rep.ConditionalTriggerPredicate = predicate;

            return this;
        }

        private StateConfigurationHelper<TState, TTrigger> IgnoreInternal(Func<bool> predicate, TTrigger trigger)
        {
            //Contract.Requires<ArgumentNullException>(trigger != null);

            var rep = FindOrCreateTriggerRepresentation(trigger, predicate, currentStateRepresentation);

            rep.NextStateRepresentation = null;
            rep.ConditionalTriggerPredicate = predicate;

            return this;
        }
    }
}
