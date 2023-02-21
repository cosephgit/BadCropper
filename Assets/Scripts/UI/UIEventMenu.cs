using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIEventMenu : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI eventTextBody;

    public void SetEvent(string eventText)
    {
        eventTextBody.text = eventText;
    }
}
