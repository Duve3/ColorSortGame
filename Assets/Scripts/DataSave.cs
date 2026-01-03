using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class DataSave : MonoBehaviour
{
    [NonSerialized] public readonly List<List<Color>> ListEmpty = new List<List<Color>>();

    public void SaveGameData(List<List<Color>> tubeData)
    {
        /* basically creating our own serializer and deserializer of our data!*/
        string final = "";

        foreach (List<Color> tube in tubeData)
        {
            string tubeString = string.Join(",", tube.Select(ColorUtility.ToHtmlStringRGBA));
            final += tubeString + ";";
        }

        Debug.Log("final: " + final);
        PlayerPrefs.SetString("Gamedata", final);
        PlayerPrefs.Save();
    }

    public List<List<Color>> ReadGameData()
    {
        String data = PlayerPrefs.GetString("Gamedata");
        Debug.Log("SAVE DATA: " + data);

        if (data == string.Empty)
        {
            return ListEmpty;
        }

        List<List<Color>> final = new List<List<Color>>();

        string[] tubes = data.Split(';');

        foreach (string tube in tubes)
        {
            if (string.IsNullOrEmpty(tube)) continue; // skip empty tubes

            List<Color> colors = new List<Color>();
            string[] colorStrings = tube.Split(',');

            foreach (string colorString in colorStrings)
            {
                if (ColorUtility.TryParseHtmlString("#" + colorString, out Color color))
                {
                    colors.Add(color);
                }
                else
                {
                    Debug.LogWarning("Failed to parse color: " + colorString);
                }
            }

            final.Add(colors);
        }

        return final;
    }
}