using ShippingContainer.Helpers;
using ShippingContainer.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace ShippingContainer.Tests
{
    public class SpoilageHelperTests
    {
        /// <summary>
        /// Happy path test to demonstrate spoilage (spoilage is a positive temperature)
        /// </summary>
        [Fact]
        public void ConsecutiveDataSpoiledPositive()
        {
            var now = DateTime.UtcNow;

            var data = new List<TemperatureRecord>()
            {
                new TemperatureRecord(now, 1),
                new TemperatureRecord(now.AddSeconds(1), 1),
                new TemperatureRecord(now.AddSeconds(2), 1),
                new TemperatureRecord(now.AddSeconds(3), 5),
                new TemperatureRecord(now.AddSeconds(4), 5),
                new TemperatureRecord(now.AddSeconds(5), 5),
                new TemperatureRecord(now.AddSeconds(6), 5),
                new TemperatureRecord(now.AddSeconds(7), 5),
                new TemperatureRecord(now.AddSeconds(8), 1),
                new TemperatureRecord(now.AddSeconds(9), 1),
            };

            var isSpoiled = SpoilageHelpers.IsSpoiled(data, 5, 5);
            Assert.True(isSpoiled);
        }


        /// <summary>
        /// Prove ordering of spoiled TemperatureRecord doesn't matter (spoilage is a positive temperature)
        /// </summary>
        [Fact]
        public void OutOfOrderSpoiledPositive()
        {
            var now = DateTime.UtcNow;

            var data = new List<TemperatureRecord>()
            {
                new TemperatureRecord(now.AddSeconds(3), 5),
                new TemperatureRecord(now.AddSeconds(6), 5),
                new TemperatureRecord(now, 1),
                new TemperatureRecord(now.AddSeconds(8), 1),
                new TemperatureRecord(now.AddSeconds(1), 1),
                new TemperatureRecord(now.AddSeconds(2), 1),
                new TemperatureRecord(now.AddSeconds(7), 5),
                new TemperatureRecord(now.AddSeconds(9), 1),
                new TemperatureRecord(now.AddSeconds(4), 5),
                new TemperatureRecord(now.AddSeconds(5), 5),
            };

            var isSpoiled = SpoilageHelpers.IsSpoiled(data, 5, 5);
            Assert.True(isSpoiled);
        }


        /// <summary>
        /// Demonstrate that gaps in data can be tolerated (spoilage is a positive temperature)
        /// </summary>
        [Fact]
        public void PartialDataSpoiledPositive()
        {
            var now = DateTime.UtcNow;

            var data = new List<TemperatureRecord>()
            {
                new TemperatureRecord(now, 1),
                new TemperatureRecord(now.AddSeconds(1), 1),
                new TemperatureRecord(now.AddSeconds(2), 1),
                new TemperatureRecord(now.AddSeconds(3), 5),
                new TemperatureRecord(now.AddSeconds(7), 5),
                new TemperatureRecord(now.AddSeconds(8), 1),
                new TemperatureRecord(now.AddSeconds(9), 1),
            };

            var isSpoiled = SpoilageHelpers.IsSpoiled(data, 5, 5);
            Assert.True(isSpoiled);
        }


        /// <summary>
        /// Happy path test to demonstrate no spoilage (spoilage is a positive temperature)
        /// </summary>
        [Fact]
        public void ConsecutiveDataNotSpoiledPositive()
        {
            var now = DateTime.UtcNow;

            var data = new List<TemperatureRecord>()
            {
                new TemperatureRecord(now, 1),
                new TemperatureRecord(now.AddSeconds(1), 1),
                new TemperatureRecord(now.AddSeconds(2), 1),
                new TemperatureRecord(now.AddSeconds(3), 1),
                new TemperatureRecord(now.AddSeconds(4), 1),
                new TemperatureRecord(now.AddSeconds(5), 1),
                new TemperatureRecord(now.AddSeconds(6), 1),
                new TemperatureRecord(now.AddSeconds(7), 1),
                new TemperatureRecord(now.AddSeconds(8), 1),
                new TemperatureRecord(now.AddSeconds(9), 1),
            };

            var isSpoiled = SpoilageHelpers.IsSpoiled(data, 5, 5);
            Assert.False(isSpoiled);
        }


        /// <summary>
        /// Prove ordering of spoiled TemperatureRecord doesn't matter (spoilage is a positive temperature)
        /// </summary>
        [Fact]
        public void OutOfOrderNotSpoiledPositive()
        {
            var now = DateTime.UtcNow;

            var data = new List<TemperatureRecord>()
            {
                new TemperatureRecord(now.AddSeconds(2), 1),
                new TemperatureRecord(now.AddSeconds(7), 1),
                new TemperatureRecord(now.AddSeconds(8), 1),
                new TemperatureRecord(now.AddSeconds(3), 1),
                new TemperatureRecord(now.AddSeconds(4), 1),
                new TemperatureRecord(now.AddSeconds(1), 1),
                new TemperatureRecord(now.AddSeconds(5), 1),
                new TemperatureRecord(now.AddSeconds(6), 1),
                new TemperatureRecord(now.AddSeconds(9), 1),
                new TemperatureRecord(now, 1),
            };

            var isSpoiled = SpoilageHelpers.IsSpoiled(data, 5, 5);
            Assert.False(isSpoiled);
        }


        /// <summary>
        /// Demonstrate that gaps in data can be tolerated (spoilage is a positive temperature)
        /// </summary>
        [Fact]
        public void PartialDataNotSpoiledPositive()
        {
            var now = DateTime.UtcNow;

            var data = new List<TemperatureRecord>()
            {
                new TemperatureRecord(now, 1),
                new TemperatureRecord(now.AddSeconds(2), 1),
                new TemperatureRecord(now.AddSeconds(3), 1),
                new TemperatureRecord(now.AddSeconds(4), 2),
                new TemperatureRecord(now.AddSeconds(5), 1),
                new TemperatureRecord(now.AddSeconds(7), 2),
                new TemperatureRecord(now.AddSeconds(9), 1),
            };

            var isSpoiled = SpoilageHelpers.IsSpoiled(data, 5, 5);
            Assert.False(isSpoiled);
        }


        /// <summary>
        /// Happy path test to demonstrate spoilage (spoilage is a negative temperature)
        /// </summary>
        [Fact]
        public void ConsecutiveDataSpoiledNegative()
        {
            var now = DateTime.UtcNow;

            var data = new List<TemperatureRecord>()
            {
                new TemperatureRecord(now, 1),
                new TemperatureRecord(now.AddSeconds(1), 1),
                new TemperatureRecord(now.AddSeconds(2), 1),
                new TemperatureRecord(now.AddSeconds(3), -5),
                new TemperatureRecord(now.AddSeconds(4), -5),
                new TemperatureRecord(now.AddSeconds(5), -6),
                new TemperatureRecord(now.AddSeconds(6), -5),
                new TemperatureRecord(now.AddSeconds(7), -5),
                new TemperatureRecord(now.AddSeconds(8), 1),
                new TemperatureRecord(now.AddSeconds(9), 1),
            };

            var isSpoiled = SpoilageHelpers.IsSpoiled(data, -4, 5);
            Assert.True(isSpoiled);
        }


        /// <summary>
        /// Prove ordering of spoiled TemperatureRecord doesn't matter (spoilage is a negative temperature)
        /// </summary>
        [Fact]
        public void OutOfOrderSpoiledNegative()
        {
            var now = DateTime.UtcNow;

            var data = new List<TemperatureRecord>()
            {
                new TemperatureRecord(now.AddSeconds(3), -5),
                new TemperatureRecord(now.AddSeconds(6), -5),
                new TemperatureRecord(now, 1),
                new TemperatureRecord(now.AddSeconds(8), 1),
                new TemperatureRecord(now.AddSeconds(1), 1),
                new TemperatureRecord(now.AddSeconds(2), 1),
                new TemperatureRecord(now.AddSeconds(7), -5),
                new TemperatureRecord(now.AddSeconds(9), 1),
                new TemperatureRecord(now.AddSeconds(4), -5),
                new TemperatureRecord(now.AddSeconds(5), -5),
            };

            var isSpoiled = SpoilageHelpers.IsSpoiled(data, -5, 5);
            Assert.True(isSpoiled);
        }


        /// <summary>
        /// Demonstrate that gaps in data can be tolerated (spoilage is a negative temperature)
        /// </summary>
        [Fact]
        public void PartialDataSpoiledNegative()
        {
            var now = DateTime.UtcNow;

            var data = new List<TemperatureRecord>()
            {
                new TemperatureRecord(now, 1),
                new TemperatureRecord(now.AddSeconds(1), 1),
                new TemperatureRecord(now.AddSeconds(2), 1),
                new TemperatureRecord(now.AddSeconds(3), -5),
                new TemperatureRecord(now.AddSeconds(7), -10),
                new TemperatureRecord(now.AddSeconds(8), 1),
                new TemperatureRecord(now.AddSeconds(9), 1),
            };

            var isSpoiled = SpoilageHelpers.IsSpoiled(data, -4, 5);
            Assert.True(isSpoiled);
        }


        /// <summary>
        /// Happy path test to demonstrate no spoilage (spoilage is a negative temperature)
        /// </summary>
        [Fact]
        public void ConsecutiveDataNotSpoiledNegative()
        {
            var now = DateTime.UtcNow;

            var data = new List<TemperatureRecord>()
            {
                new TemperatureRecord(now, 1),
                new TemperatureRecord(now.AddSeconds(1), -1),
                new TemperatureRecord(now.AddSeconds(2), -1),
                new TemperatureRecord(now.AddSeconds(3), -2),
                new TemperatureRecord(now.AddSeconds(4), -3),
                new TemperatureRecord(now.AddSeconds(5), -1),
                new TemperatureRecord(now.AddSeconds(6), 10),
                new TemperatureRecord(now.AddSeconds(7), 0),
                new TemperatureRecord(now.AddSeconds(8), 1),
                new TemperatureRecord(now.AddSeconds(9), -1),
            };

            var isSpoiled = SpoilageHelpers.IsSpoiled(data, -16, 5);
            Assert.False(isSpoiled);
        }


        /// <summary>
        /// Prove ordering of spoiled TemperatureRecord doesn't matter (spoilage is a negative temperature)
        /// </summary>
        [Fact]
        public void OutOfOrderNotSpoiledNegative()
        {
            var now = DateTime.UtcNow;

            var data = new List<TemperatureRecord>()
            {
                new TemperatureRecord(now.AddSeconds(2), -1),
                new TemperatureRecord(now.AddSeconds(7), -3),
                new TemperatureRecord(now.AddSeconds(8), -1),
                new TemperatureRecord(now.AddSeconds(3), 1),
                new TemperatureRecord(now.AddSeconds(4), 4),
                new TemperatureRecord(now.AddSeconds(1), 16),
                new TemperatureRecord(now.AddSeconds(5), 10),
                new TemperatureRecord(now.AddSeconds(6), -1),
                new TemperatureRecord(now.AddSeconds(9), -1),
                new TemperatureRecord(now, 1),
            };

            var isSpoiled = SpoilageHelpers.IsSpoiled(data, -5, 5);
            Assert.False(isSpoiled);
        }


        /// <summary>
        /// Demonstrate that gaps in data can be tolerated (spoilage is a negative temperature)
        /// </summary>
        [Fact]
        public void PartialDataNotSpoiledNegative()
        {
            var now = DateTime.UtcNow;

            var data = new List<TemperatureRecord>()
            {
                new TemperatureRecord(now, 1),
                new TemperatureRecord(now.AddSeconds(2), -1),
                new TemperatureRecord(now.AddSeconds(3), -1),
                new TemperatureRecord(now.AddSeconds(4), -2),
                new TemperatureRecord(now.AddSeconds(5), -1),
                new TemperatureRecord(now.AddSeconds(7), -2),
                new TemperatureRecord(now.AddSeconds(9), -1),
            };

            var isSpoiled = SpoilageHelpers.IsSpoiled(data, -2.5, 5);
            Assert.False(isSpoiled);
        }
    }
}
