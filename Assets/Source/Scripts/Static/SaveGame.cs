using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveGame
{
    public static void Save<T>(ref T value, string id)
    {
        var @string = JsonUtility.ToJson(value);
        PlayerPrefs.SetString(id, @string);
    }

    public static void Load<T>(ref T field, string id, T defaultValue)
    {
        if (PlayerPrefs.HasKey(id))
        {
            var @string = PlayerPrefs.GetString(id);
            field = JsonUtility.FromJson<T>(@string);
        }

        else
        {
            field = defaultValue;
        }
    }
}
