using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// manages the quest status and events

public class QuestManager : MonoBehaviour
{
    [SerializeField]int questSoftProgressDaysDefault = 5;
    [SerializeField]int questHardProgressDaysDefault = 10;
    [SerializeField]GameObject interactableNormal;
    [SerializeField]GameObject interactableEvent;
    [SerializeField]GameObject[] interactableEventIndividual;
    [SerializeField]CropSlotHolder ruinedCrops;
    int questState; // tracks the players progress
    int questSoftProgressDays; // days until the next "soft progress" quest step (advance if other criteria are met)
    int questHardProgressDays; // days until the quest progresses regardless of other criteria
    bool eventState; // is a major event currently playing out
    bool eventIndividualState; // is the special completion condition of the current event unfinished
    int questLevels; // the total number of quest levels before the ending

    void Awake()
    {
        questState = 0;
        eventState = false;
        InitQuestLevel();
        interactableNormal.SetActive(true);
        interactableEvent.SetActive(false);
        questLevels = interactableEventIndividual.Length;
        for (int i = 0; i < questLevels; i++) interactableEventIndividual[i].SetActive(false);
    }

    void InitQuestLevel()
    {
        questSoftProgressDays = questSoftProgressDaysDefault;
        questHardProgressDays = questHardProgressDaysDefault;
    }

    // checks quest-level dependent conditions for a soft continuation
    bool SoftConditions()
    {
        switch (questState)
        {
            case 0:
            {
                // first event: storm
                // player just loses crops, so fairly easy conditions
                // player just needs at least (health + food) of 3 and 1 seed
                int healthFood = ServiceProvider.instance.Homestead().GotFood() + ServiceProvider.instance.Player().GotHealth();
                bool seed = ServiceProvider.instance.Homestead().HasSeed(CropType.Corn);
                if (seed && (healthFood >= 4)) return true;
                return false;
            }
            case 1:
            {
                // second event: rats
                // player loses all crops plus half of their seed and food
                // player still just needs at least (health + food) of 3 and 1 seed, but this time AFTER LOSSES
                int healthFood = Mathf.FloorToInt((float)ServiceProvider.instance.Homestead().GotFood() * 0.5f) + ServiceProvider.instance.Player().GotHealth();
                bool seed = (ServiceProvider.instance.Homestead().GetSeed(CropType.Corn) > 0) || (ServiceProvider.instance.Homestead().GetSeed(CropType.Carrot) > 1);
                if (seed && (healthFood >= 4)) return true;
                return false;
            }
            case 2:
            {
                // first event: storm
                // player just loses crops, so fairly easy conditions
                // player just needs at least (health + food) of 3 and 1 seed
                int healthFood = ServiceProvider.instance.Homestead().GotFood() + ServiceProvider.instance.Player().GotHealth();
                bool seed = ServiceProvider.instance.Homestead().HasSeed(CropType.Corn);
                if (seed && (healthFood >= 4)) return true;
                return false;
            }
            case 3:
            {
                // second event: rats
                // player loses all crops plus half of their seed and food
                // player still just needs at least (health + food) of 3 and 1 seed, but this time AFTER LOSSES
                int healthFood = Mathf.FloorToInt((float)ServiceProvider.instance.Homestead().GotFood() * 0.5f) + ServiceProvider.instance.Player().GotHealth();
                bool seed = (ServiceProvider.instance.Homestead().GetSeed(CropType.Corn) > 0) || (ServiceProvider.instance.Homestead().GetSeed(CropType.Carrot) > 1);
                if (seed && (healthFood >= 4)) return true;
                return false;
            }
        }
        return true;
    }

    // this quest is called each night to advance the quest timers
    // it returns true if there is a quest event, otherwise it returns false
    public bool NightAdvance()
    {
        questSoftProgressDays--;
        questHardProgressDays--;

        if (questState < questLevels)
        {
            if (questHardProgressDays <= 0
                || (SoftConditions() && questSoftProgressDays <= 0))
            {
                // do quest stuff
                StartEvent();

                return true;
            }
        }

        return false;
    }

    void StartEvent()
    {
        switch (questState)
        {
            case 0:
            {
                // first event: storm
                ServiceProvider.instance.GetAmbienceManager().AmbienceCustom(Ambience.Storm, 0.95f, 1f, 0.1f); // transition in quickly

                ServiceProvider.instance.GetUI().EventMenuOpen("A hurricane beats at the shack causing it to shake and creak. Outside debris can be heard banging around. It's too dangerous to go out and check, I have to wait until it ends.");
                break;
            }
            case 1:
            {
                // second event: rats
                ServiceProvider.instance.GetAmbienceManager().AmbienceCustom(Ambience.Rats, 0.9f, 1f, 0.1f); // transition in quickly

                ServiceProvider.instance.GetUI().EventMenuOpen("Rats! Rats are everywhere! They're swarming and climbing and chewing on everything in sight! I don't think I can stop them, they're mad with hunger!");

                // lose half of seed and food
                int corn = Mathf.FloorToInt((float)ServiceProvider.instance.Homestead().GetSeed(CropType.Corn) * 0.5f);
                int carrot = Mathf.CeilToInt((float)ServiceProvider.instance.Homestead().GetSeed(CropType.Carrot) * 0.5f);
                int food = Mathf.CeilToInt((float)ServiceProvider.instance.Homestead().GotFood() * 0.5f);

                ServiceProvider.instance.Homestead().AddSeed(CropType.Corn, -corn);
                ServiceProvider.instance.Homestead().AddSeed(CropType.Carrot, -carrot);
                ServiceProvider.instance.Homestead().UseFood(food);
                break;
            }
            case 2:
            {
                // first event: storm
                ServiceProvider.instance.GetAmbienceManager().AmbienceCustom(Ambience.Storm, 0.95f, 1f, 0.1f); // transition in quickly

                ServiceProvider.instance.GetUI().EventMenuOpen("Another hurricane! I hope my crops fare better than last time. It could be worse, the shack could collapse and bury me alive...");
                break;
            }
            case 3:
            {
                // second event: rats
                ServiceProvider.instance.GetAmbienceManager().AmbienceCustom(Ambience.Rats, 0.9f, 1f, 0.1f); // transition in quickly

                ServiceProvider.instance.GetUI().EventMenuOpen("Are these the same rats as before? ");

                // lose half of seed and food
                int corn = Mathf.FloorToInt((float)ServiceProvider.instance.Homestead().GetSeed(CropType.Corn) * 0.5f);
                int carrot = Mathf.CeilToInt((float)ServiceProvider.instance.Homestead().GetSeed(CropType.Carrot) * 0.5f);
                int food = Mathf.CeilToInt((float)ServiceProvider.instance.Homestead().GotFood() * 0.5f);

                ServiceProvider.instance.Homestead().AddSeed(CropType.Corn, -corn);
                ServiceProvider.instance.Homestead().AddSeed(CropType.Carrot, -carrot);
                ServiceProvider.instance.Homestead().UseFood(food);
                break;
            }
        }
    }

    // this is called by the event menu
    public void EndEvent()
    {
        // complete the event
        switch (questState)
        {
            case 0:
            {
                // first event: storm ends
                ServiceProvider.instance.GetAmbienceManager().AmbienceCustom(Ambience.Wind, 0.1f, 0.5f, 5); // allow it to trail off slowly so even if the player quick skips they'll hear it
                interactableEvent.SetActive(true);
                ServiceProvider.instance.CropSlots().DestroyAll(0); // this destroys all current live crops and places ruined crops in the event crops layer
                interactableNormal.SetActive(false);
                interactableEventIndividual[0].SetActive(true);
                eventState = true; // turn on event management
                eventIndividualState = true;
                break;
            }
            case 1:
            {
                // first event: rats leave
                ServiceProvider.instance.GetAmbienceManager().AmbienceCustom(Ambience.Wind, 0.1f, 0.5f, 5); // allow it to trail off slowly so even if the player quick skips they'll hear it
                interactableEvent.SetActive(true);
                ServiceProvider.instance.CropSlots().DestroyAll(1); // this destroys all current live crops and places ruined crops in the event crops layer
                interactableNormal.SetActive(false);
                interactableEventIndividual[1].SetActive(true);
                eventState = true; // turn on event management
                eventIndividualState = true;
                break;
            }
            case 2:
            {
                // first event: storm ends
                ServiceProvider.instance.GetAmbienceManager().AmbienceCustom(Ambience.Wind, 0.1f, 0.5f, 5); // allow it to trail off slowly so even if the player quick skips they'll hear it
                interactableEvent.SetActive(true);
                ServiceProvider.instance.CropSlots().DestroyAll(0); // this destroys all current live crops and places ruined crops in the event crops layer
                interactableNormal.SetActive(false);
                interactableEventIndividual[2].SetActive(true);
                eventState = true; // turn on event management
                //eventIndividualState = true; - only do this the first time
                break;
            }
            case 3:
            {
                // first event: rats leave
                ServiceProvider.instance.GetAmbienceManager().AmbienceCustom(Ambience.Wind, 0.1f, 0.5f, 5); // allow it to trail off slowly so even if the player quick skips they'll hear it
                interactableEvent.SetActive(true);
                ServiceProvider.instance.CropSlots().DestroyAll(1); // this destroys all current live crops and places ruined crops in the event crops layer
                interactableNormal.SetActive(false);
                interactableEventIndividual[3].SetActive(true);
                eventState = true; // turn on event management
                //eventIndividualState = true;
                break;
            }
        }

        ServiceProvider.instance.GetUI().EventMenuClose(); // close the menu
        ServiceProvider.instance.GetDayNightManager().EventFinish(); // allow the night to end
    }

    // update is only active while there as an ongoing event, to detect the event completing and allowing normal gameplay to continue
    void Update()
    {
        if (eventState)
        {
            if (!ruinedCrops.HasRuinedCrops() && !eventIndividualState)
            {
                // all ruined crops are gathered, and the special item is gathered, allow everything to go back to normal
                eventState = false;
                interactableNormal.SetActive(true);
                interactableEvent.SetActive(false);
                // turn off the event-specific interactables, and add player dialogue
                switch (questState)
                {
                    case 0:
                    {
                        interactableEventIndividual[0].SetActive(false);
                        ServiceProvider.instance.Player().AddDialogue("I think that's everything. Time to start anew.");
                        ServiceProvider.instance.Player().AddDialogue("[Press 2 to SELECT carrot seeds]");
                        break;
                    }
                    case 1:
                    {
                        interactableEventIndividual[1].SetActive(false);
                        ServiceProvider.instance.Player().AddDialogue("This place is trying to kill me. How often can I lose everything?");
                        ServiceProvider.instance.Player().AddDialogue("[Press 3 to SELECT aubergine seeds]");
                        break;
                    }
                    case 2:
                    {
                        interactableEventIndividual[2].SetActive(false);
                        ServiceProvider.instance.Player().AddDialogue("I have to build up my supplies so I can get out of here!");
                        break;
                    }
                    case 3:
                    {
                        interactableEventIndividual[3].SetActive(false);
                        ServiceProvider.instance.Player().AddDialogue("The rats might eat ME next time!");
                        break;
                    }
                }

                if (questState == 3) questState = 2; // start looping the last two quests
                else questState++;
                InitQuestLevel();
            }
        }
    }

    // returns true if the end game condition is met
    public bool VictoryCheck()
    {
        if (ServiceProvider.instance.Player().TravelGoal())
        {
            return true;
        }
        return false;
    }

    public void EventIndividualComplete()
    {
        eventIndividualState = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public int QuestStage()
    {
        return questState;
    }
}
