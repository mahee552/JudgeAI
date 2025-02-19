// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested model is not supported by the system.
    /// </summary>
    public class ModelNotSupportedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelNotSupportedException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public ModelNotSupportedException(string message)
            : base(message)
        {
        }
    }
}
