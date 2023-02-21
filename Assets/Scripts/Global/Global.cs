using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    public const int GOALFOOD = 5;
    public const int GOALWATER = 5;
    
    public static string ListToString(List<string> strings)
    {
        string output = "";
        for (int i = 0; i < strings.Count; i++)
        {
            if (i > 0) output += ", ";
            output += strings[i];
        }
        return output;
    }
}
