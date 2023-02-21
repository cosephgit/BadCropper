using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField]float relativeXMin = -2; // x minimum for player to trigger this dialogue
    [SerializeField]float relativeXMax = 2; // x minimum for player to trigger this dialogue
    [SerializeField]float repeatCooldown; // if > 0 then this dialogue repeats only after this time delay (make it at least PlayerManager() line delay * line count)
    [SerializeField]string[] dialogue; // lines of dialogue to play when triggered
    [SerializeField]bool lowPriority;
    float repeatTime; // time after which it can repeat
    bool dialogueNew; // is this new unplayed dialogue?
    float actualXMin;
    float actualXMax;

    void Awake()
    {
        repeatTime = 0;
        dialogueNew = true;
        actualXMin = transform.position.x + relativeXMin;
        actualXMax = transform.position.x + relativeXMax;
        if (dialogue.Length == 0)
        {
            Debug.Log("<color=red>ERROR</color> DialogueTrigger with no dialogue");
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (repeatCooldown == 0 && !dialogueNew) return; // this dialogue has played it's one time (object could arguably be deleted here)

        if (repeatTime > 0)
        {
            repeatTime -= Time.deltaTime;
        }
        else
        {
            // check if the player is in the trigger area
            if (ServiceProvider.instance.Player().transform.position.x > actualXMin
                && ServiceProvider.instance.Player().transform.position.x < actualXMax)
            {
                ServiceProvider.instance.Player().AddDialogue(dialogue, lowPriority);
                repeatTime = repeatCooldown; // set it up to be repeated if it has a cooldown
                dialogueNew = false; // this is no longer new dialogue
            }
        }
    }
}
