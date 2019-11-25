﻿using RoystonGame.TV.ControlFlows;
using RoystonGame.TV.DataModels;
using RoystonGame.TV.DataModels.Enums;
using RoystonGame.TV.DataModels.GameStates;
using RoystonGame.Web.DataModels.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoystonGame.TV.Extensions
{
    /// <summary>
    /// Class containing extensions for the UserState object.
    /// </summary>
    public static class StateExtensions
    {
        /// <summary>
        /// Links a <paramref name="A"/> to a target <paramref name="C"/> via Transition <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The transition to use, the transition MUST define constructor(UserState transitionTo).</typeparam>
        /// <param name="A">The state to transition out of.</param>
        /// <param name="C">The state to transition into.</param>
        /// <returns><paramref name="C"/> unmodified.</returns>
        /// <remarks>Don't assign the result of this operation to anything! The response is purely meant for chaining.</remarks>
        public static State Transition<T>(this StateOutlet A, State C) where T : UserStateTransition, new() => A.Transition<T>(C, out _);
        public static State Transition<T>(this StateOutlet A, State C, out T B) where T : UserStateTransition, new()
        {
            // Existence of constructor not checked at compile time.
            B = new T();

            B.SetOutlet(C.Inlet);
            A.SetOutlet(B.Inlet);

            // Return C for easy chaining. Careful with references.
            return C;
        }

        public static State Transition(this StateOutlet A, State B)
        {
            A.SetOutlet(B.Inlet);
            return B;
        }
    }
}
