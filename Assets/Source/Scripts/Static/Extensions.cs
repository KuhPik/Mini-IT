using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Extensions
{
    public static void Push<T>(this List<T> list, T item, int maxCount)
    {
        if (list.Count < maxCount) list.Add(item);
        else
        {
            for (int i = 1; i < maxCount; i++)
            {
                var element = list[i];
                list[i - 1] = element;
            }

            list[maxCount - 1] = item;
        }
    }

    public static T GetRandom<T>(this T[] collection)
    {
        var rng = Random.Range(0, collection.Length);
        return collection[rng];
    }
}
