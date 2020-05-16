﻿using RoystonGame.TV.DataModels.Enums;
using RoystonGame.TV.DataModels.States.UserStates;
using RoystonGame.TV.DataModels.Users;
using RoystonGame.TV.Extensions;
using RoystonGame.Web.DataModels.Requests;
using RoystonGame.Web.DataModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoystonGame.TV.ControlFlows.Exit
{
    public class WaitForPartyLeader_StateExit : WaitForUsers_StateExit
    {

        private UserState PartyLeaderUserState { get; set; }

        /// <summary>
        /// Initializes a new <see cref="WaitForTrigger"/>.
        /// </summary>
        /// <param name="waitingPromptGenerator">The waiting prompt generator to use while waiting for the trigger.</param>
        public WaitForPartyLeader_StateExit(Lobby lobby, Func<User, UserPrompt> partyLeaderPromptGenerator = null, Func<User, UserPrompt> waitingPromptGenerator = null, Func<User, UserFormSubmission, (bool, string)> partyLeaderFormSubmitListener = null)
            : base(
                  usersToWaitFor:()=>lobby.GetAllUsers().ToList(),
                  waitingPromptGenerator: waitingPromptGenerator)
        {
            this.PartyLeaderUserState = new SimplePromptUserState(partyLeaderPromptGenerator ?? SimplePromptUserState.YouHaveThePowerPrompt, formSubmitListener: partyLeaderFormSubmitListener);
            this.PartyLeaderUserState.Transition(this.WaitingState);
        }

        /// <summary>
        /// The inlet to the transition.
        /// </summary>
        /// <param name="user">The user to move into the transition.</param>
        /// <param name="stateResult">The state result of the last node (this transition doesnt care).</param>
        /// <param name="formSubmission">The user input of the last node (this transition doesnt care).</param>
        public override void Inlet(User user, UserStateResult stateResult, UserFormSubmission formSubmission)
        {
            if (user.IsPartyLeader)
            {
                this.PartyLeaderUserState.Inlet(user, stateResult, formSubmission);
            }
            else
            {
                base.Inlet(user, stateResult, formSubmission);
            }
        }
    }
}
