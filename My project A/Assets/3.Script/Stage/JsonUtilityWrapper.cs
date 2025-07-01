using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// JsonUtilityWrapper.cs
public static class JsonUtilityWrapper
{
    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> items;
    }

    public static List<T> FromJsonList<T>(string json)
    {
        string newJson = "{\"items\":" + json + "}";
        return JsonUtility.FromJson<Wrapper<T>>(newJson).items;
    }
}
