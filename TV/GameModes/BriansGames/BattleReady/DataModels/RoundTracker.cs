﻿using RoystonGame.TV.DataModels;
using System.Collections.Generic;


namespace RoystonGame.TV.GameModes.BriansGames.BattleReady.DataModels
{
    public class RoundTracker
    {
        public List<Gameplay_Person> UnassignedPeople { get; set; } = new List<Gameplay_Person>();
        public Dictionary<User, Gameplay_Person> AssignedPeople { get; set; } = new Dictionary<User, Gameplay_Person>();
        public Dictionary<User, int> UsersToSeatNumber { get; set; } = new Dictionary<User, int>();
        public List<User> OrderedUsers { get; set; } = new List<User>();

        public void ResetRoundVariables()
        {
            UnassignedPeople = new List<Gameplay_Person>();
            AssignedPeople = new Dictionary<User, Gameplay_Person>();
            UsersToSeatNumber = new Dictionary<User, int>();
            OrderedUsers = new List<User>();
        }
    }
}
