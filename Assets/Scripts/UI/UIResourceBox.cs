using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// keeps track of the values for the resource boxes, and shows a "pop" each time a resource changes

public class UIResourceBox : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI valueText;
    [SerializeField]float popMax = 1.5f; // how big the number gets when popping
    [SerializeField]float popTime = 1f; // how long the number pops for when popped
    [SerializeField]float popFrame = 0.05f;
    [SerializeField]bool redOnZero = false; // does the number turn red when it'z zero?
    int statValue = -1; // make sure everything is updated the first time it's set

    public void UpdateStat(int newValue)
    {
        if (newValue != statValue)
        {
            statValue = newValue;
            valueText.text = "" + newValue;
            StopAllCoroutines();
            StartCoroutine(StatPop());
        }
    }

    IEnumerator StatPop()
    {
        float scale = 1;
        float popFrameCount = popTime / popFrame;

        valueText.color = Color.yellow;

        for (int i = 0; i < popFrameCount; i++)
        {
            scale = ((popMax - 1) * i / popFrameCount) + 1;
            valueText.transform.localScale = new Vector3(scale,scale,scale);
            yield return new WaitForSeconds(popFrame);
        }

        // back to normal
        valueText.transform.localScale = new Vector3(1,1,1);
        if (statValue == 0 && redOnZero)
            valueText.color = Color.red;
        else
            valueText.color = Color.white;
    }
}
