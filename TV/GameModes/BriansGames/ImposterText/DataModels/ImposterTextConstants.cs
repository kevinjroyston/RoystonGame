﻿using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoystonGame.TV.GameModes.BriansGames.ImposterText.DataModels
{
    public static class ImposterTextConstants
    {
        public const int TotalPointsToAwardPerVote = 100;
        public const int PointsToLooseForWrongVote = 50;

        public const double SetupTimerMin = 45;
        public const double SetupTimerMax = 420;
        public const double AnsweringTimerMin = 30;
        public const double AnsweringTimerMax = 420;
        public const double VotingTimerMin = 30;
        public const double VotingTimerMax = 420;
    }
}
