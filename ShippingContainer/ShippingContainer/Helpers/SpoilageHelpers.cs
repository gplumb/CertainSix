using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShippingContainer.Helpers
{
    /// <summary>
    /// Helper for calculating spoilage
    /// </summary>
    public static class SpoilageHelpers
    {
        /// <summary>
        /// Determines whether spoilage has occured for the given temperature thresholds and measurements
        /// </summary>
        public static bool IsSpoiled(List<Models.TemperatureRecord> records, double spoilageTemperature, double spoilageDurationInSeconds)
        {
            // Sort by time first so we can use a greedy algorithm O(n)
            var data = records.OrderBy(y => y.Time).ToList();

            // Assume the worst, since we don't actually know
            if (data.Count == 0)
                return true;

            if (data.Count == 1)
            {
                // Not enough data points, so be super pessimistic
                return (spoilageTemperature <= 0) ? data[0].Value <= spoilageTemperature : data[0].Value >= spoilageTemperature;
            }

            // Leave as double to avoid boxing when looking at DateTime differences
            var largestDurationAtThreshold = 0d;
            var last = data[0];
            var item = data[0];
            int x = 1;

            do
            {
                // The difference between the last known data point and this one
                var difference = data[x].Time.Subtract(last.Time).TotalSeconds;

                // Do we have a gap in the data?
                if (difference > 0)
                {
                    // Yes - then find the worst data point (pessimistic) to use
                    if (spoilageTemperature <= 0)
                    {
                        // Use the colder of the two if the threshold is negative
                        item = (last.Value < data[x].Value) ? last : data[x];
                    }
                    else
                    {
                        // Use the higher of the two if the threshold is positive
                        item = (last.Value > data[x].Value) ? last : data[x];
                    }
                }
                else
                {
                    // No
                    item = data[x];
                }

                // Is the temperature beyond the spoilage threshold?
                var isThresholdMet = (spoilageTemperature <= 0) ? item.Value <= spoilageTemperature : item.Value >= spoilageTemperature;

                if (isThresholdMet)
                {
                    // Yes - then pessimistically store the duration at this temperature
                    largestDurationAtThreshold += difference;
                }
                else
                {
                    largestDurationAtThreshold = 0;
                }

                last = data[x];
                x++;
            }
            while (x < data.Count && largestDurationAtThreshold < spoilageDurationInSeconds);

            // We've iterated over everything, do we have spoilage?
            return (largestDurationAtThreshold >= spoilageDurationInSeconds) ? true : false;
        }
    }
}
