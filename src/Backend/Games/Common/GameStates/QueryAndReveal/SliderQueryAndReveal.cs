﻿using Backend.APIs.DataModels.UnityObjects;
using Backend.GameInfrastructure;
using Backend.GameInfrastructure.DataModels.Users;
using Backend.Games.TimsGames.FriendQuiz.DataModels;
using Common.DataModels.Enums;
using Common.DataModels.Requests;
using Common.DataModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Games.Common.GameStates.QueryAndReveal
{
    public class SliderQueryAndReveal : QueryAndRevealState<Question, (int, int)>
    {
        public override Func<User, List<Question>, UserPrompt> QueryPromptGenerator { get; set; }
        public override Action<List<Question>> QueryExitListener { get; set; }
        public string QueryPromptTitle { get; set; } = "Answer these questions";
        public string QueryPromptDescription { get; set; }
        public int SliderMin { get; set; } = 0;
        public int SliderMax { get; set; } = 100;

        public SliderQueryAndReveal(
            Lobby lobby,
            List<Question> objectsToQuery,
            List<User> usersToQuery = null,
            TimeSpan? queryTime = null) : base(lobby, objectsToQuery, usersToQuery, queryTime)
        {
            QueryPromptGenerator ??= DefaultQueryPromptGenerator;
        }
        public override (int, int) AnswerExtractor(UserSubForm subForm)
        {
            if (subForm?.Slider?.Count == 2)
            {
                return (subForm.Slider[0], subForm.Slider[1]);
            }
            return (SliderMin, SliderMax);
        }

        public override UnityView QueryUnityViewGenerator()
        {
            UnityView unityView = base.QueryUnityViewGenerator();
            unityView.ScreenId = TVScreenId.WaitForUserInputs;
            unityView.UnityObjects = null;
            return unityView;
        }

        private UserPrompt DefaultQueryPromptGenerator(User user, List<Question> questions)
        {
            return new UserPrompt()
            {
                UserPromptId = UserPromptId.FriendQuiz_Query,
                Title = this.QueryPromptTitle,
                Description = this.QueryPromptDescription,
                SubPrompts = questions.Select(question => new SubPrompt()
                {
                    Slider = new SliderPromptMetadata()
                    {
                        Min = SliderMin,
                        Max = SliderMax,
                        Range = true,
                        TicksLabels = question.TickLabels.ToArray(),
                        Ticks = question.TickValues.ToArray(),
                        Value = new int[] { (int) ((SliderMin + SliderMax) * 0.25), (int)((SliderMin + SliderMax) * 0.75) },
                    }
                }).ToArray(),
                SubmitButton = true
            };     
        }
    }
}