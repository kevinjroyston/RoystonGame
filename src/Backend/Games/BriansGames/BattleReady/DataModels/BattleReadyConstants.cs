﻿using Common.DataModels.Enums;
using System;
using System.Collections.Generic;

namespace Backend.Games.BriansGames.BattleReady.DataModels
{
    public static class BattleReadyConstants
    {
        public const int PointsForVote = 100;
        public const int PointsForPartUsed = 50;
        public const int NumDrawingsInHand = 3;

        public static IReadOnlyDictionary<GameDuration, TimeSpan> SetupPerDrawingTimer = new Dictionary<GameDuration, TimeSpan>()
        {
            { GameDuration.Short, TimeSpan.FromSeconds(60)},
            { GameDuration.Normal, TimeSpan.FromSeconds(90)},
            { GameDuration.Extended, TimeSpan.FromSeconds(120)},
        };
        public static IReadOnlyDictionary<GameDuration, TimeSpan> SetupPerPromptTimer = new Dictionary<GameDuration, TimeSpan>()
        {
            { GameDuration.Short, TimeSpan.FromSeconds(60)},
            { GameDuration.Normal, TimeSpan.FromSeconds(90)},
            { GameDuration.Extended, TimeSpan.FromSeconds(120)},
        };
        public static IReadOnlyDictionary<GameDuration, TimeSpan> PerCreationTimer = new Dictionary<GameDuration, TimeSpan>()
        {
            { GameDuration.Short, TimeSpan.FromSeconds(50)},
            { GameDuration.Normal, TimeSpan.FromSeconds(60)},
            { GameDuration.Extended, TimeSpan.FromSeconds(90)},
        };
        public static IReadOnlyDictionary<GameDuration, TimeSpan> VotingTimer = new Dictionary<GameDuration, TimeSpan>()
        {
            { GameDuration.Short, TimeSpan.FromSeconds(45)},
            { GameDuration.Normal, TimeSpan.FromSeconds(60)},
            { GameDuration.Extended, TimeSpan.FromSeconds(90)},
        };

        // Number of rounds (round = create contestants & vote)
        public static IReadOnlyDictionary<GameDuration, int> NumRounds = new Dictionary<GameDuration, int>()
        {
            { GameDuration.Short, 1},
            { GameDuration.Normal, 2},
            { GameDuration.Extended, 2},
        };

        // SubRound = 1 prompt. By default 1 per player.
        public static IReadOnlyDictionary<GameDuration, int> MaxNumSubRounds = new Dictionary<GameDuration, int>()
        {
            { GameDuration.Short, 4},
            { GameDuration.Normal, 4},
            { GameDuration.Extended, 5},
        };

        public static IReadOnlyDictionary<GameDuration, int> NumDrawingsPerPlayer = new Dictionary<GameDuration, int>()
        {
            { GameDuration.Short, 2},
            { GameDuration.Normal, 3},
            { GameDuration.Extended, 4},
        };
    }
}
