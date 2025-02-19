// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Utilities.Formatters
{
    using System;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// Provides utility methods for formatting elapsed time from a Stopwatch into human-readable strings.
    /// </summary>
    public static class ElapsedTimeFormatter
    {
        /// <summary>
        /// Formats the elapsed time from a Stopwatch into a human-readable string.
        /// </summary>
        /// <param name="stopwatch">The Stopwatch instance containing the elapsed time to format.</param>
        /// <returns>
        /// A formatted string representing the elapsed time in the following format:
        ///  For ≥1 hour: displays hours and minutes (e.g., "2 hrs 30 mins").
        ///  For ≥1 minute: displays minutes and seconds (e.g., "45 mins 20 secs").
        ///  For ≥1 second: displays seconds (e.g., "30 secs").
        ///  For less than 1 second: displays milliseconds (e.g., "500 ms").
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when stopwatch is null.</exception>
        public static string FormatElapsedTime(Stopwatch stopwatch)
        {
            if (stopwatch == null)
            {
                throw new ArgumentNullException(nameof(stopwatch), "Stopwatch cannot be null.");
            }

            TimeSpan elapsed = stopwatch.Elapsed;
            var result = new StringBuilder();

            // Hours and minutes (≥1 hour)
            if (elapsed.TotalHours >= 1)
            {
                int hours = (int)Math.Floor(elapsed.TotalHours);
                int minutes = elapsed.Minutes;

                result.Append(hours == 1 ? "1 hr" : $"{hours} hrs");

                if (minutes > 0)
                {
                    result.Append(minutes == 1 ? " 1 min" : $" {minutes} mins");
                }

                return result.ToString();
            }

            // Minutes and seconds (≥1 minute)
            else if (elapsed.TotalMinutes >= 1)
            {
                int minutes = (int)Math.Floor(elapsed.TotalMinutes);
                int seconds = elapsed.Seconds;

                result.Append(minutes == 1 ? "1 min" : $"{minutes} mins");

                if (seconds > 0)
                {
                    result.Append(seconds == 1 ? " 1 sec" : $" {seconds} secs");
                }

                return result.ToString();
            }

            // Seconds (≥1 second)
            else if (elapsed.TotalSeconds >= 1)
            {
                int seconds = (int)Math.Floor(elapsed.TotalSeconds);
                return seconds == 1 ? "1 sec" : $"{seconds} secs";
            }

            // Milliseconds (<1 second)
            else
            {
                int ms = (int)Math.Floor(elapsed.TotalMilliseconds);
                return $"{ms} ms";
            }
        }
    }
}
