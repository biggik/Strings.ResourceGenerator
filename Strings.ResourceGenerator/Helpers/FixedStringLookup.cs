using System.Globalization;

// Heavily copied from https://referencesource.microsoft.com/#System/compmod/system/collections/specialized/fixedstringlookup.cs
namespace Strings.ResourceGenerator.Helpers;

internal static class FixedStringLookup
{
    // Returns whether the match is found in the lookup table
    internal static bool Contains(string[][] lookupTable, string value, bool ignoreCase)
    {
        int length = value.Length;
        if (length <= 0 || length - 1 >= lookupTable.Length)
        {
            return false;
        }

        string[] subArray = lookupTable[length - 1];
        return subArray == null ? false : Contains(subArray, value, ignoreCase);
    }

    // This routine finds a hit within a single sorted array, with the assumption that the
    // value and all the strings are of the same length.
    private static bool Contains(string[] array, string value, bool ignoreCase)
    {
        int min = 0;
        int max = array.Length;
        int pos = 0;
        char searchChar;
        while (pos < value.Length)
        {
            searchChar = ignoreCase ? char.ToLower(value[pos], CultureInfo.InvariantCulture) : value[pos];
            if ((max - min) <= 1)
            {
                // we are down to a single item, so we can stay on this row until the end.
                if (searchChar != array[min][pos])
                {
                    return false;
                }
                pos++;
                continue;
            }

            // There are multiple items to search, use binary search to find one of the hits
            if (!FindCharacter(array, searchChar, pos, ref min, ref max))
            {
                return false;
            }
            // and move to next char
            pos++;
        }
        return true;
    }

    // Do a binary search on the character array at the specific position and constrict the ranges appropriately.
    private static bool FindCharacter(string[] array, char value, int pos, ref int min, ref int max)
    {
        int index = min;
        while (min < max)
        {
            index = (min + max) / 2;
            char comp = array[index][pos];
            if (value == comp)
            {
                // We have a match. Now adjust to any adjacent matches
                int newMin = index;
                while (newMin > min && array[newMin - 1][pos] == value)
                {
                    newMin--;
                }
                min = newMin;

                int newMax = index + 1;
                while (newMax < max && array[newMax][pos] == value)
                {
                    newMax++;
                }
                max = newMax;
                return true;
            }
            if (value < comp)
            {
                max = index;
            }
            else
            {
                min = index + 1;
            }
        }
        return false;
    }
}