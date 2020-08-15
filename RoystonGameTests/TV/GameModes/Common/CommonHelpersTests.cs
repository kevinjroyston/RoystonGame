﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoystonGame.TV.GameModes.Common;
using System;

namespace RoystonGameTests.TV.GameModes.Common
{
    [TestClass]
    public class CommonHelpersTests
    {
        [DataRow(10, 5, 0.0, 1.0, 0.0, 10)]
        [DataRow(10, 5, 1.0, 2.0, 0.5, 10)]
        [DataRow(10, 5, 1.0, 2.0, 2.0, 5)]
        [DataRow(200, 10, 1.0, 2.0, 2.1, 10)]
        [DataRow(100, 50, 1.0, 3.0, 2.0, 75)]
        [DataRow(100, 1, 0.0, 1.0, 0.2, 80)]
        [DataRow(0, 20, 5.0, 1.0, 0.0, null)]
        [DataRow(20, 40, 5.0, 1.0, 0.0, null)]
        [DataRow(0, 20, -1.0, 0.0, 0.0, null)]
        [DataRow(-20, 0, 0.0, 2.0, 1.0, null)]
        [DataRow(-20, 0, 0.0, 2.0, 1.0, 1999)]
        [DataTestMethod]
        public void PointsForSpeedTest(int maxPoints, int minPoints, double startTime, double endTime, double secondsTaken, int? expectedValue)
        {
            if (expectedValue.HasValue)
            {
                Assert.AreEqual(expectedValue, CommonHelpers.PointsForSpeed(maxPoints, minPoints, startTime, endTime, secondsTaken));
            }
            else
            {
                // Testing it does not error, do not care about return value.
                CommonHelpers.PointsForSpeed(maxPoints, minPoints, startTime, endTime, secondsTaken);
            }
        }
    }
}