using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActionPoint : MonoBehaviour
{
    [SerializeField]Image[] actionPoints;
    [SerializeField]Color actionPointFull;
    [SerializeField]Color actionPointSpent;
    int maxAP;
    int currentAP;

    void Awake()
    {
        maxAP = actionPoints.Length;
    }

    public void UpdateAP(int points)
    {
        currentAP = points;
        if (currentAP > maxAP) currentAP = maxAP;

        for (int i = 0; i < maxAP; i++)
        {
            if (i < currentAP)
                actionPoints[i].color = actionPointFull;
            else
                actionPoints[i].color = actionPointSpent;
        }
    }

}
