﻿using Backend.GameInfrastructure.ControlFlows.Exit;
using Backend.GameInfrastructure.DataModels.States.GameStates;
using Backend.GameInfrastructure.DataModels.States.UserStates;
using Backend.GameInfrastructure.DataModels.Users;
using Backend.GameInfrastructure.Extensions;
using Backend.Games.BriansGames.ImposterDrawing.DataModels;
using Common.DataModels.Requests;
using Common.DataModels.Responses;
using Backend.APIs.DataModels.UnityObjects;
using Common.Code.WordLists;
using System;
using System.Collections.Generic;
using static System.FormattableString;
using Backend.GameInfrastructure;
using Common.DataModels.Enums;
using Backend.GameInfrastructure.DataModels;
using Common.DataModels.Interfaces;
using Common.Code.Helpers;
using System.Linq;
using Backend.GameInfrastructure.DataModels.States.StateGroups;
using Common.Code.Extensions;
using Backend.Games.Common.DataModels;
using Backend.GameInfrastructure.DataModels.Enums;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Backend.Games.BriansGames.ImposterDrawing.GameStates
{
    public class Setup_GS : GameState
    {
        private UserState GetChallengesUserState()
        {
            return new SimplePromptUserState(
                promptGenerator: (User user) => new UserPrompt()
                {
                    UserPromptId = UserPromptId.ImposterSyndrome_CreatePrompt,
                    Title = "Game setup",
                    Description = "In the boxes below, enter two drawing prompts such that only you will be able to tell the drawings apart.",
                    SubPrompts = new SubPrompt[]
                    {
                        new SubPrompt
                        {
                            //Prompt = Invariant($"The drawing prompt to show all users. Suggestions: '{string.Join("', '",RandomLineFromFile.GetRandomLines(FileNames.Nouns, 5))}'"),
                            Prompt = Invariant($"The drawing prompt to show all users. Suggestions: '{string.Join("', '",RandomLineFromFile.GetRandomLines(FileNames.Nouns, 5))}'"),
                            ShortAnswer = true,
                        },
                        new SubPrompt
                        {
                            Prompt = "The drawing prompt to show to imposter",
                            ShortAnswer = true,
                        }
                    },
                    SubmitButton = true
                },
                formSubmitHandler: (User user, UserFormSubmission input) =>
                {
                    PromptsToPopulate.Add(new Prompt
                    {
                        Owner = user,
                        RealPrompt = input.SubForms[0].ShortAnswer,
                        FakePrompt = input.SubForms[1].ShortAnswer,
                        MaxMemberCount = this.MaxPlayersPerPrompt,
                        BannedMemberIds = new List<Guid>{ user.Id }.ToImmutableHashSet(),
                        AllowDuplicateIds = false,
                    });
                    return (true, string.Empty);
                },
                exit: new WaitForUsers_StateExit(lobby: this.Lobby),
                maxPromptDuration: WritingTimeDuration);
        }
        private Random Rand { get; set; } = new Random();

        /// <summary>
        /// Returns a chain of user states which will prompt for the proper drawings, assumes this.SubChallenges is fully set up.
        /// </summary>
        /// <param name="user">The user to build a chain for.</param>
        /// <returns>A list of user states designed for a given user.</returns>
        private List<State> GetDrawingsUserStateChain(User user)
        {
            List<State> stateChain = new List<State>();
            List<Prompt> challenges = this.PromptsToPopulate.OrderBy(_ => Rand.Next()).ToList();
            int index = 0;
            foreach (Prompt promptToDraw in challenges)
            {
                if (!promptToDraw.UsersToDrawings.ContainsKey(user))
                {
                    continue;
                }

                var lambdaSafeIndex = index;
                stateChain.Add(new SimplePromptUserState(
                    promptGenerator: (User user) => new UserPrompt()
                    {
                        UserPromptId = UserPromptId.ImposterSyndrome_Draw,
                        Title = "Draw the prompt below",
                        Description = "Careful, if you aren't the odd one out and people think you are, you will lose points for being a terrible artist.",
                        SubPrompts = new SubPrompt[]
                         {
                            new SubPrompt
                            {
                                Prompt = Invariant($"Your prompt:\"{(promptToDraw.Imposter == user ? promptToDraw.FakePrompt : promptToDraw.RealPrompt)}\""),
                                Drawing = new DrawingPromptMetadata(){
                                    GalleryOptions = null
                                },
                            },
                         },
                        SubmitButton = true
                    },
                    formSubmitHandler: (User user, UserFormSubmission input) =>
                    {
                        promptToDraw.UsersToDrawings.AddOrReplace(
                            user,
                            new UserDrawing()
                            {
                                Drawing = input.SubForms[0].Drawing,
                                Owner = user,
                                ShouldHighlightReveal = promptToDraw.Imposter == user,
                                UnityImageRevealOverrides = new UnityObjectOverrides
                                {
                                    Title = user.DisplayName,
                                }
                            });
                        return (true, string.Empty);
                    },
                    userTimeoutHandler: (User user, UserFormSubmission input)=> { return UserTimeoutAction.None; }));
                index++;
            }

            return stateChain;
        }
        private List<Prompt> PromptsToPopulate { get; }
        private TimeSpan? WritingTimeDuration { get; }
        private TimeSpan? DrawingTimeDuration { get; }
        private int NumRounds { get; }
        private int MaxPlayersPerPrompt { get; }
        private int NumDrawingsPerUser { get; }
        public Setup_GS(Lobby lobby, List<Prompt> promptsToPopulate, TimeSpan? writingTimeDuration, TimeSpan? drawingTimeDuration, int numRounds, int maxPlayersPerPrompt, int numDrawingsPerUser)
        : base(
            lobby: lobby,
            exit: new WaitForUsers_StateExit(lobby))
        {
            this.PromptsToPopulate = promptsToPopulate;
            this.WritingTimeDuration = writingTimeDuration;
            this.DrawingTimeDuration = drawingTimeDuration.MultipliedBy(numDrawingsPerUser); // TODO, this is incorrect in edge case where we have too many users and maxPlayersPerPrompt is exceeded.
            this.NumRounds = numRounds;
            this.MaxPlayersPerPrompt = maxPlayersPerPrompt;
            this.NumDrawingsPerUser = numDrawingsPerUser;
            State getChallenges = GetChallengesUserState();

            this.Entrance.Transition(getChallenges);
            getChallenges.AddExitListener(() => this.AssignPrompts());
            getChallenges.Transition(() =>
            {
                StateExit waitForDrawings = new WaitForUsers_StateExit(
                    lobby: this.Lobby,
                    waitingPromptGenerator: (User user) =>
                    {
                        return Prompts.DisplayText("Waiting for others to draw.")(user);
                    });
                var getDrawings = new MultiStateChain(GetDrawingsUserStateChain, exit: waitForDrawings, stateDuration: DrawingTimeDuration);
                getDrawings.Transition(this.Exit);
                return getDrawings;
            });

            this.UnityView = new UnityView(this.Lobby)
            {
                ScreenId = TVScreenId.WaitForUserInputs,
                Instructions = new UnityField<string> { Value = "Complete all the prompts on your devices." },
            };
        }


        private void AssignPrompts()
        {
            IReadOnlyList<User> users = this.Lobby.GetAllUsers();
            List<Prompt> randomizedOrderChallenges = this.PromptsToPopulate.OrderBy(_ => Rand.Next()).ToList();
            List<Prompt> excessChallenges = randomizedOrderChallenges.Skip(this.NumRounds).ToList();
            randomizedOrderChallenges = randomizedOrderChallenges.Take(this.NumRounds).ToList();

            foreach (Prompt extra in excessChallenges)
            {
                this.PromptsToPopulate.Remove(extra);
            }

            if (randomizedOrderChallenges.Count == 0)
            {
                throw new Exception("Can't play the game if there are no prompts");
            }

            List<IGroup<User>> groups = MemberHelpers<User>.Assign(
                randomizedOrderChallenges.Cast<IConstraints<User>>().ToList(),
                users,
                this.NumDrawingsPerUser);

            var assignments = groups.Zip(randomizedOrderChallenges);

            foreach ((IGroup<User> groupedUsers, Prompt tracker) in assignments)
            {
                tracker.UsersToDrawings = new ConcurrentDictionary<User,UserDrawing>(
                    groupedUsers.Members.ToDictionary<User,User,UserDrawing>(
                        keySelector:(user) => user,
                        elementSelector:(user) => null));
                tracker.Imposter = tracker.UsersToDrawings.Keys.ElementAt(Rand.Next(tracker.UsersToDrawings.Keys.Count));
            }
        }
    }
}
