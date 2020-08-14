using System;

namespace RevitJournal.Duplicate.Comparer
{
    public static class LevenstheinHelper
    {
        public static string LevenstheinAsString(int distance)
        {
            if (distance == 0) { return string.Empty; }
            return distance.ToString();
        }

        public static int ComputeLevensthein(string original, string other)
        {
            if (IsBooleanValue(original) && IsBooleanValue(other))
            {
                var originalBool = bool.Parse(original);
                var otherBool = bool.Parse(other);
                if (originalBool != otherBool) { return 1; }

                return 0;
            }

            var originalLength = original.Length;
            var otherLength = other.Length;
            var twoDArray = new int[originalLength + 1, otherLength + 1];

            // Step 1
            if (originalLength == 0) { return otherLength; }
            if (otherLength == 0) { return originalLength; }

            // Step 2
            for (var origIdx = 0; origIdx <= originalLength; twoDArray[origIdx, 0] = origIdx++) { }
            for (var otherIdx = 0; otherIdx <= otherLength; twoDArray[0, otherIdx] = otherIdx++) { }

            // Step 3
            for (var origIdx = 1; origIdx <= originalLength; origIdx++)
            {
                //Step 4
                for (var otherIdx = 1; otherIdx <= otherLength; otherIdx++)
                {
                    // Step 5
                    var cost = (other[otherIdx - 1] == original[origIdx - 1]) ? 0 : 1;

                    // Step 6
                    twoDArray[origIdx, otherIdx] = Math.Min(
                        Math.Min(twoDArray[origIdx - 1, otherIdx] + 1, twoDArray[origIdx, otherIdx - 1] + 1),
                        twoDArray[origIdx - 1, otherIdx - 1] + cost);
                }
            }
            // Step 7
            return twoDArray[originalLength, otherLength];
        }

        private static bool IsBooleanValue(string value)
        {
            return string.IsNullOrEmpty(value) == false
                && (value.Equals(bool.TrueString, StringComparison.CurrentCultureIgnoreCase)
                || value.Equals(bool.FalseString, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
