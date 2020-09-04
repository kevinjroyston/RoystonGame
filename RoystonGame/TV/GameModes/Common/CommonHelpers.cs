﻿using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using RoystonGame.TV.DataModels.Users;
using RoystonGame.TV.GameModes.Common.DataModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.FormattableString;

namespace RoystonGame.TV.GameModes.Common
{
    public static class CommonHelpers
    {
        private static Random Rand = new Random();
        public static string HtmlImageWrapper(string image, int width = 240, int height = 240)
        {
            return Invariant($"<img width=\"{width}\" height=\"{height}\" src=\"{image}\"/>");
        }

        public static void ResetAllRevealDeltas(List<User> users)
        {
            foreach (User user in users)
            {
                user.ResetScoreDeltaReveal();
            }
        }

        public static T GetWeightedRandom<T>(IDictionary<T, int> valueToWeight)
        {
            int totalWeight = valueToWeight.Values.Sum();
            int random = Rand.Next(0, totalWeight);
            foreach (T value in valueToWeight.Keys)
            {
                if (random <= valueToWeight[value])
                {
                    return value;
                }
                else
                {
                    random -= valueToWeight[value];
                }
            }
            throw new Exception("Something went wrong getting weighted random");
        }
        public static IEnumerable<UserCreatedObject> TrimUserInputList(IEnumerable<UserCreatedObject> userInputs, int numInputsWanted)
        {
            Dictionary<User, List<UserCreatedObject>> usersToInputs = new Dictionary<User, List<UserCreatedObject>>();
            foreach (UserCreatedObject userInput in userInputs)
            {
                if (!usersToInputs.ContainsKey(userInput.Owner))
                {
                    usersToInputs.Add(userInput.Owner, new List<UserCreatedObject>());
                }
                usersToInputs[userInput.Owner].Add(userInput);
            }
            List<User> keys = usersToInputs.Keys.ToList();
            foreach (User user in keys)
            {
                usersToInputs[user] = usersToInputs[user].OrderBy(input => input.CreationTime).ToList();
            }
            List<UserCreatedObject> trimmedUserInputs = userInputs.ToList();
            while (trimmedUserInputs.Count > numInputsWanted)
            {
                User userWithMostInputs = GetUserWithMostInputs();
                trimmedUserInputs.Remove(usersToInputs[userWithMostInputs].Last());
                usersToInputs[userWithMostInputs].RemoveAt(usersToInputs[userWithMostInputs].Count - 1);
            }

            return trimmedUserInputs;

            User GetUserWithMostInputs()
            {
                User userWithMostInputs = usersToInputs.Keys.ToList()[0];
                foreach (User user in usersToInputs.Keys)
                {
                    if (usersToInputs[user].Count > usersToInputs[userWithMostInputs].Count)
                    {
                        userWithMostInputs = user;
                    }
                }
                return userWithMostInputs;
            }
        }

        public static int GetMaxInputsFromExpected(int numExpected, float multiplier = 1.3f, int minExtra = 2)
        {
            return Math.Max((int)Math.Ceiling(numExpected * multiplier), numExpected + minExtra);
        }
        public static int PointsForSpeed(int maxPoints, int minPoints, double startTime, double endTime, double secondsTaken)
        {
            Debug.Assert(maxPoints >= minPoints);
            Debug.Assert(endTime >= startTime);
            Debug.Assert(secondsTaken >= 0);

            if (secondsTaken <= startTime)
            {
                return maxPoints;
            }
            if(secondsTaken >= endTime)
            {
                return minPoints;
            }
            else
            {
                return (int)((endTime - secondsTaken)/(endTime - startTime) * (maxPoints - minPoints)) + minPoints;
            }
        }

        public static TimeSpan GetTimerFromLength(double length, double minTimerLength, double aveTimerLength, double maxTimerLength, double minLength = 0, double aveLength = 5, double maxLength = 10)
        {
            return TimeSpan.FromSeconds(ThreePointLerp(
                minX: minLength,
                aveX: aveLength,
                maxX: maxLength,
                x: length,
                minValue: minTimerLength,
                aveValue: aveTimerLength,
                maxValue: maxTimerLength));
        }
        public static double ThreePointLerp(double minX, double aveX, double maxX, double x, double minValue, double aveValue, double maxValue)
        {
            if (x < minX)
            {
                return minValue;
            }
            else if (x < aveX)
            {
                return Lerp(minValue, aveValue, (x - minX) / (aveX - minX));
            }
            else if (x < maxX)
            {
                return Lerp(aveValue, maxValue, (x - aveX) / (maxX - aveX));
            }
            else
            {
                return maxValue;
            }
        }
        public static double Lerp(double a, double b, double t)
        {
            return (b - a) * t + a;
        }
        public static double ClampedLerp(double a, double b, double t)
        {
            t = Math.Clamp(t, 0.0, 1.0);
            return (b - a) * t + a;
        }

        /// <summary>
        /// Evenly distributes the elements of toDistribute into the groups. Will duplicate toDistribute elements
        /// until the groups have reached maxGroupSize or all elements that pass the check have been assigned for each group
        /// </summary>
        public static Dictionary<TGroup, List<TToDistribute>> EvenlyDistribute<TGroup, TToDistribute>( // Todo fix
            List<TGroup> groups,
            List<TToDistribute> toDistribute,
            int maxGroupSize,
            Func<TGroup, TToDistribute, bool> validDistributeCheck)
        {
            if (groups.Count < 2)
            {
                throw new Exception("groups must have at least 2 elements");
            }

            Random Rand = new Random();

            List<TToDistribute> skipedDistributes = new List<TToDistribute>();
            int distributeIndex = 0;
            Dictionary<TGroup, List<TToDistribute>> distributedDict = new Dictionary<TGroup, List<TToDistribute>>();

            foreach (TGroup group in groups) 
            {
                distributedDict.Add(group, new List<TToDistribute>());
                for (int i = 0; i < toDistribute.Count; i++)
                {
                    if (distributedDict[group].Count >= maxGroupSize)
                    {
                        break;
                    }
                    bool addedFromSkipped = false;
                    foreach (TToDistribute skiped in skipedDistributes)
                    {
                        if (validDistributeCheck(group, skiped) && !distributedDict[group].Contains(skiped))
                        {
                            distributedDict[group].Add(skiped);
                            skipedDistributes.Remove(skiped);
                            addedFromSkipped = true;
                            break;
                        }
                    }
                    if (!addedFromSkipped)
                    {
                        if (validDistributeCheck(group, toDistribute[distributeIndex]) && !distributedDict[group].Contains(toDistribute[distributeIndex]))
                        {
                            distributedDict[group].Add(toDistribute[distributeIndex]);
                        }
                        else
                        {
                            skipedDistributes.Add(toDistribute[distributeIndex]);
                        }
                    }

                    distributeIndex++;
                    if (distributeIndex >= toDistribute.Count)
                    {
                        distributeIndex = 0;
                    }
                }
            }
            if (distributedDict.Count != groups.Count)
            {
                throw new Exception("Something went wrong assigning into groups");
            }
            // Make 100 attempts at valid swaps
            for (int i = 0; i < 100; i++)
            {
                TGroup randGroup1 = distributedDict.Keys.ToList()[Rand.Next(0, distributedDict.Count)];
                TGroup randGroup2 = distributedDict.Keys.Where((TGroup group) => !group.Equals(randGroup1)).ToList()[Rand.Next(0, distributedDict.Count - 1)];

                TToDistribute randDistribute1 = distributedDict[randGroup1][Rand.Next(0, distributedDict[randGroup1].Count)];
                TToDistribute randDistribute2 = distributedDict[randGroup2][Rand.Next(0, distributedDict[randGroup2].Count)];

                // Determines if a swap is valid
                if (
                    validDistributeCheck(randGroup1, randDistribute2) 
                    && validDistributeCheck(randGroup2, randDistribute1)
                    && !distributedDict[randGroup1].Contains(randDistribute2)
                    && !distributedDict[randGroup2].Contains(randDistribute1))
                {
                    distributedDict[randGroup1].Remove(randDistribute1);
                    distributedDict[randGroup1].Add(randDistribute2);

                    distributedDict[randGroup2].Remove(randDistribute2);
                    distributedDict[randGroup2].Add(randDistribute1);
                }
            }

            return distributedDict;
        }
    }
}
