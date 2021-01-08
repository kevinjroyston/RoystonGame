﻿using Backend.GameInfrastructure.DataModels.Users;
using Common.DataModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Games.TimsGames.FriendQuiz.DataModels
{
    public class UserQuestionsHolder : Constraints<Question>
    { 
        public UserQuestionsHolder (User user)
        {
            this.QuestionedUser = user;
        }
        public User QuestionedUser { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }
}
