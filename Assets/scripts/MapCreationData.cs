using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "MapCreationData")]
public class MapCreationData : ScriptableObject
{
    [SerializeField] Theme defaultTheme;
    [SerializeField] Theme[] themes;
    [SerializeField] Color[] colors;
    [SerializeField] int fontSize = 20;


    public Theme GetTheme(MapData.Themes theme)
    {
        Theme newTheme = themes.FirstOrDefault(i => i.IsOfTheme(theme));
        if(newTheme == null) { newTheme = defaultTheme; }
        return newTheme;
    }

    public Theme GetDefaultTheme() 
    {
        return defaultTheme;
    }

    internal Color[] Colors()
    {
        return colors;
    }

    internal int FontSize()
    {
        return fontSize;
    }
}
