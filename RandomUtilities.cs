using System;
using UnityEngine;

public static class RandomUtilities
{
    /// <summary>
    /// Returns a random integer number between min [inclusive] and max [exclusive]. Never returns something equal to excluded.
    /// </summary>
    public static int ExclusiveRandomRange(int min, int max, int excluded)
    {
        if (excluded < min || excluded >= max)
            return UnityEngine.Random.Range(min, max);
        int result = UnityEngine.Random.Range(min, max - 1);
        if (result >= excluded)
            result++;
        return result;
    }

    public static int[] RandomIndexesRange(int min, int max, int count, bool allowDuplicates = true)
    {
        if (count < 1)
            return null;

        if (count == 1)
            return new int[] { UnityEngine.Random.Range(min, max) };

        if (!allowDuplicates && count > (max - min))
        {
            Debug.LogError("Random indexes: count is too high compared to min-max to allow not to have duplicates.");
            count = max - min;
        }

        int[] result = new int[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = UnityEngine.Random.Range(min, max);
            if (!allowDuplicates && i > 0)
            {
                bool isDuplicate = false;
                do
                {
                    isDuplicate = false;
                    for (int j = 0; j < i; j++)
                        if (result[j] == result[i])
                        {
                            isDuplicate = true;
                            result[i] = UnityEngine.Random.Range(min, max);
                            j = i;
                        }
                } while (isDuplicate);
            }
        }
        return result;
    }

    public static T RandomItem<T>(this T[] array)
    {
        return array[UnityEngine.Random.Range(0, array.Length)];
    }
}

public static class RandomEnum<T>
{
    static T[] values;
    static RandomEnum()
    {
        var values = Enum.GetValues(typeof(T));
        RandomEnum<T>.values = new T[values.Length];
        for (int i = 0; i < RandomEnum<T>.values.Length; i++)
            RandomEnum<T>.values[i] = (T)values.GetValue(i);
    }

    /// <summary>
    /// Get a random value.
    /// </summary>
    /// <returns></returns>
    public static T Get()
    {
        return values[UnityEngine.Random.Range(0, values.Length)];
    }
}
