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

    // this converts a 0...1 volume value in Unity (which is scaled to apparent volume) to a 0...1 volume value for FMOD (which is scaled to decibels)
    // working on the metric that +-10 decibels is twice (or half) as loud
    public static float ApparentToDecibel(float apparent)
    {
        float decibel;

        //if (apparent < 0.01) decibel = 0; // no need to care about any smaller value and we don't want to tangle with infinity
        //else decibel = (2 + Mathf.Log(apparent)) * 0.5f; // log(0.01) = -2 and log(1) = 0 so adjust this to 0...1 scale

        // so we need a value of 1 to change to 1
        // a value of 0.5 needs to change to 0.875 (i.e. 7/8s, as the decibel scale is -80...0 so this is equivalent to -10)
        // a value of 0.25 needs to change to 0.75 (i.e. 6/8s)

        // math breakdown
        //apparent = Mathf.Log(apparent, 2); // so each time apparent halves, the output is 1 less, this gives a range of -inf to 0
        //apparent += 8; // the lowest relevant value is -8 (8 halvings or -80 decibels, approximately silent)
        //apparent *= 0.125f; // aka divided by 8 to adjust to a scale of 0...1
        if (apparent < 0.004) decibel = 0; // 0.004 is approximately 2^-8 - i.e. 8 halvings so anything less than this is less than -80 decibels and can be treated as silent
        else decibel = (Mathf.Log(apparent, 2) + 8) * 0.125f;

        return decibel;
    }
    // this reverses the above
    public static float DecibelToApparent(float decibel)
    {
        float apparent = Mathf.Pow(10, (2 * decibel) - 2);

        return apparent;
    }
}
