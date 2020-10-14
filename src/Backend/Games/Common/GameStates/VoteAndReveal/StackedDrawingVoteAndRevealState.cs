﻿using Backend.GameInfrastructure.DataModels.Users;
using Backend.Games.Common.DataModels;
using Common.DataModels.Requests;
using Common.DataModels.Responses;
using Backend.APIs.DataModels.UnityObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Common.DataModels.Enums;
using Backend.GameInfrastructure;

namespace Backend.Games.Common.GameStates.VoteAndReveal
{
    public class StackedDrawingVoteAndRevealState : VoteAndRevealState<List<UserDrawing>>
    {
        private Action<Dictionary<User, int>> VoteCountingManager { get; set; }
        public Func<User, int, string> PromptAnswerAddOnGenerator { get; set; } = (User user, int answer) => "";

        public StackedDrawingVoteAndRevealState(
            Lobby lobby,
            List<List<UserDrawing>> stackedDrawings,
            Action<Dictionary<User, int>> voteCountManager,
            List<User> votingUsers = null,
            TimeSpan? votingTime = null) : base(lobby, stackedDrawings, votingUsers, votingTime)
        {
            this.VoteCountingManager = voteCountManager;
        }

        public override UserPrompt VotingPromptGenerator(User user)
        {
            return new UserPrompt()
            {
                UserPromptId = UserPromptId.Voting,
                Title = VotingTitle,
                SubPrompts = new SubPrompt[]
                {
                    new SubPrompt()
                    {
                        Prompt = VotingPromptTexts?[0] ?? null,
                        Answers = Enumerable.Range(1, this.Objects.Count).Select(num => num.ToString() + PromptAnswerAddOnGenerator(user, num -1)).ToArray(),
                    }
                },
                SubmitButton = true
            };
        }
        public override UnityImage VotingUnityObjectGenerator(int objectIndex)
        {
            return new UnityImage()
            {
                Base64Pngs = new StaticAccessor<IReadOnlyList<string>> { Value = Objects[objectIndex].Select(userDrawing => userDrawing.Drawing).ToList() },
                ImageIdentifier = new StaticAccessor<string> { Value = (objectIndex + 1).ToString() },
                Title = new StaticAccessor<string> { Value = this.ShowObjectTitlesForVoting ? this.ObjectTitles?[objectIndex] : null},
                Header = new StaticAccessor<string> { Value = this.ShowObjectHeadersForVoting ? this.ObjectHeaders?[objectIndex] : null },
            };
        }
        public override UnityImage RevealUnityObjectGenerator(int objectIndex)
        {
            return new UnityImage()
            {
                Base64Pngs = new StaticAccessor<IReadOnlyList<string>> { Value = this.Objects[objectIndex].Select(userDrawing => userDrawing.Drawing).ToList() },
                ImageIdentifier = new StaticAccessor<string> { Value = (objectIndex + 1).ToString() },
                Title = new StaticAccessor<string> { Value = this.ObjectTitles?[objectIndex] },
                Header = new StaticAccessor<string> { Value = this.ObjectTitles?[objectIndex] },
                ImageOwnerId = new StaticAccessor<Guid?> { Value = this.Objects[objectIndex][0].Owner?.UserId },
                VoteRevealOptions = new StaticAccessor<UnityImageVoteRevealOptions>
                {
                    Value = new UnityImageVoteRevealOptions()
                    {
                        RelevantUsers = new StaticAccessor<IReadOnlyList<User>> { Value = AnswersToUsersWhoVoted.ContainsKey(objectIndex) ? AnswersToUsersWhoVoted[objectIndex] : new List<User>() },
                        RevealThisImage = new StaticAccessor<bool?> { Value = IndexesOfObjectsToReveal?.Contains(objectIndex) }
                    }
                },
            };
        }
        public override List<int> VotingFormSubmitManager(User user, UserFormSubmission submission)
        {
            return new List<int>() { submission.SubForms[0].RadioAnswer.Value };
        }
        public override List<int> VotingTimeoutManager(User user, UserFormSubmission submission)
        {
            if (submission.SubForms.Count > 0
               && submission.SubForms[0].RadioAnswer != null)
            {
                return new List<int>() { submission.SubForms[0].RadioAnswer.Value };
            }
            else
            {
                return new List<int>();
            }
        }
        public override void VoteCountManager(Dictionary<User, (List<int>, double)> usersToVotes)
        {
            Dictionary<User, int> singleAnswerDict = new Dictionary<User, int>();
            foreach (User user in usersToVotes.Keys)
            {
                if (usersToVotes[user].Item1.Count > 0)
                {
                    singleAnswerDict.Add(user, usersToVotes[user].Item1[0]);
                }
            }
            VoteCountingManager(singleAnswerDict);
        }
    }
}