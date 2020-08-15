﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RoystonGame.TV.GameModes.BriansGames.TwoToneDrawing.DataModels
{
    public static class TwoToneDrawingConstants
    {
        public const int PointsForMakingWinningDrawing = 500;
        public const int PointsForVotingForWinningDrawing = 100;
        public const int PointsToLoseForBadSelfVote= -100;

        public const double SetupTimerMin = 45;
        public const double SetupTimerAve = 120;
        public const double SetupTimerMax = 240;

        public const double DrawingTimerMin = 45;
        public const double DrawingTimerAve = 120;
        public const double DrawingTimerMax = 240;

        public const double VotingTimerMin = 30;
        public const double VotingTimerAve = 60;
        public const double VotingTimerMax = 240;
    }
}
