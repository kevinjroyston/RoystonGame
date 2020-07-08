﻿using RoystonGame.TV.DataModels.States.GameStates;
using RoystonGame.TV.DataModels.States.UserStates;
using RoystonGame.TV.DataModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoystonGame.TV.GameModes.TimsGames.FriendQuiz.GameStates
{
    public class Setup_GS: GameState
    {
        private Random Rand { get; } = new Random();
        public Setup_GS(Lobby lobby, string prompt, RoundTracker roundTracker, TimeSpan? writingDurration = null) : base(lobby)
        {
            SimplePromptUserState writingUserState = new SimplePromptUserState(
                promptGenerator: (User user) => new UserPrompt()
                {
                    Title = "Time To Write",
                    Description = Invariant($"Write the first sentence for a \"{prompt}\""),
                    SubPrompts = new SubPrompt[]
                    {
                        new SubPrompt
                        {
                            ShortAnswer = true
                        }
                    },
                    SubmitButton = true
                },
                formSubmitHandler: (User user, UserFormSubmission input) =>
                {
                    string text = input.SubForms[0].ShortAnswer;
                    UserWriting writing = new UserWriting(user, text, WritingDisplayPosition.None);
                    roundTracker.UsersToUserWriting.AddOrUpdate(user, writing, (User user, UserWriting oldWriting) => writing);
                    return (true, string.Empty);
                },
                exit: new WaitForUsers_StateExit(
                    lobby: lobby,
                    usersToWaitFor: WaitForUsersType.All),
                maxPromptDuration: writingDurration);
            this.Entrance.Transition(writingUserState);
            writingUserState.Transition(this.Exit);

            this.UnityView = new UnityView(this.Lobby)
            {
                ScreenId = new StaticAccessor<TVScreenId> { Value = TVScreenId.WaitForUserInputs },
                Title = new StaticAccessor<string> { Value = "Time To Write" },
                Instructions = new StaticAccessor<string> { Value = Invariant($"Write the first sentence for a \"{prompt}\"") },
            };
        }
    }
}
