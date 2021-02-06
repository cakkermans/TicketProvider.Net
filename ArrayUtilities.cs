using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net
{
    public class ArrayUtilities
    {

        public static int IndexOf<T>(T[] haystack, T[] pattern, int start)
        {
            return IndexOf<T>(haystack, pattern, start, haystack.Length);
        }

        public static int IndexOf<T>(T[] haystack, T[] pattern, int start, int end)
        {

            int haystackLength;

            haystackLength = end - pattern.Length;
            for (int i = start; i < haystackLength; i++)
            {
                if (IsMatch(haystack, i, pattern))
                    return i;
            }

            return -1;
        }

        public static bool IsMatch<T>(T[] haystack, int index, T[] pattern)
        {

            int patternLength;

            patternLength = pattern.Length;
            for (int j = 0; j < patternLength; j++)
            {
                if (!haystack[index + j].Equals(pattern[j]))
                    return false;
            }
            return true;
        }
    }
}
