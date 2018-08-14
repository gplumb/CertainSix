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
        /// Happy path test to demonstrate spoilage
        /// </summary>
        [Fact]
        public void ConsecutiveDataSpoiled()
        {
            var now = DateTime.Now;

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
        /// Prove ordering of spoiled TemperatureRecord doesn't matter
        /// </summary>
        [Fact]
        public void OutOfOrderSpoiled()
        {
            var now = DateTime.Now;

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
        /// Demonstrate that gaps in data can be tolerated
        /// </summary>
        [Fact]
        public void PartialDataSpoiled()
        {
            var now = DateTime.Now;

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
        /// Happy path test to demonstrate no spoilage
        /// </summary>
        [Fact]
        public void ConsecutiveDataNotSpoiled()
        {
            var now = DateTime.Now;

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
        /// Prove ordering of spoiled TemperatureRecord doesn't matter
        /// </summary>
        [Fact]
        public void OutOfOrderNotSpoiled()
        {
            var now = DateTime.Now;

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
        /// Demonstrate that gaps in data can be tolerated
        /// </summary>
        [Fact]
        public void PartialDataNotSpoiled()
        {
            var now = DateTime.Now;

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
    }
}
