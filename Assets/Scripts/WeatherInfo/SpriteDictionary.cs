using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "SpriteDictionary", menuName = "GeoViz/Sprite Dictionary")]
public class SpriteDictionary : SerializedScriptableObject
{
    [SerializeField]
    private Dictionary<string, Sprite> dictionary = new Dictionary<string, Sprite>();

    public Sprite GetSprite(string key)
    {
        if (dictionary.TryGetValue(key, out Sprite value))
        {
            return value;
        }
        return null;
    }
}
