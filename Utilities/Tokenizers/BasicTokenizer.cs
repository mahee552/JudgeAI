// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Utilities.Tokenizers
{
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A simple tokenizer that splits text into words based on spaces, newlines, and tabs.
    /// </summary>
    public static class BasicTokenizer
    {
        /// <summary>
        /// Tokenizes the given text into words and punctuation while handling whitespace correctly.
        /// </summary>
        /// <param name="text">The input text to tokenize.</param>
        /// <returns>A list of tokens extracted from the input text.</returns>
        public static List<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<string>();
            }

            // Regex pattern explanation:
            // - (\w+): Captures words (letters, numbers, underscores)
            // - ([^\w\s]): Captures punctuation marks
            // - (\s+): Captures whitespace (spaces, tabs, newlines)
            string pattern = @"(\w+)|([^\w\s])|(\s+)";
            var matches = Regex.Matches(text, pattern);

            List<string> tokens = matches
                .Where(m => !string.IsNullOrWhiteSpace(m.Value))
                .Select(m => m.Value)
                .ToList();

            return tokens;
        }
    }
}
