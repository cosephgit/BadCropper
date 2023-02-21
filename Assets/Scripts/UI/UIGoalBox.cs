using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIGoalBox : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI goalWater;
    [SerializeField]TextMeshProUGUI goalFood;
    [SerializeField]TextMeshProUGUI goalHealth;
    [SerializeField]Color goalTextDone = Color.green;
    [SerializeField]Color goalTextPending = Color.white;

    public void GoalsUpdate(bool water, bool food, bool health)
    {
        if (water) goalWater.color = goalTextDone;
        else goalWater.color = goalTextPending;
        if (food) goalFood.color = goalTextDone;
        else goalFood.color = goalTextPending;
        if (health) goalHealth.color = goalTextDone;
        else goalHealth.color = goalTextPending;
    }
}
