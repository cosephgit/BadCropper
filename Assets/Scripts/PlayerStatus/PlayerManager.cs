using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]float speed = 3f;
    [SerializeField]float dialogueLineDuration = 2f; // the time a dialogue line displays for
    [SerializeField]float dialogueLineDelay = 2.5f; // the time before the next dialogue line shows
    [SerializeField]SpriteRenderer dialogueBackground;
    [SerializeField]int healthMax = 5;
    [SerializeField]int healthStart = 2;
    [SerializeField]float fastDialogueRate = 2f;
    [SerializeField]float slowDialogueRate = 0.5f;
    CropType cropSelected;
    float moveInput;
    bool facingR = true;
    Animator animator;
    SpriteRenderer sprite;
    TextMeshPro dialogue;
    List<string> dialogueQue;
    float dialogueLineEnd;
    float dialogueLineNext;
    Vector2 posStart; // stores the player's initial position to reset each day
    bool controlActive;
    bool dialogueDoneHarvest; // true when the player first gets the "space to harvest" message
    int health;
    int actionPoints;
    float currentLineLength;

    void Awake()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        dialogue = GetComponentInChildren<TextMeshPro>();
        dialogue.text = "";
        currentLineLength = 0;
        dialogueQue = new List<string>();
        dialogueLineEnd = 0;
        dialogueLineNext = 0;
        dialogue.sortingLayerID = dialogueBackground.sortingLayerID; // set sorting layer for TMP asset
        dialogueBackground.enabled = false;
        posStart = transform.position;
        controlActive = true;
        dialogueDoneHarvest = false;
        health = healthStart;
        actionPoints = 3;
        cropSelected = CropType.Corn;
    }

    void Start()
    {
        ServiceProvider.instance.GetUI().InitHealth(health, healthMax);
        ServiceProvider.instance.GetUI().UpdateActionPoints(actionPoints);
        ServiceProvider.instance.GetUI().SelectCrop(cropSelected);
    }

    // handles the player's movement input
    void PlayerMovement()
    {
        float move;
        Vector2 pos;

        if (controlActive)
            moveInput = Input.GetAxis("Horizontal");
        else
            moveInput = 0;

        move = moveInput * speed * Time.deltaTime;

        pos = transform.position;
        if (pos.x + move > ServiceProvider.instance.MaxPlayerX())
        {
            move = ServiceProvider.instance.MaxPlayerX() - pos.x;
        }
        else if (pos.x + move < ServiceProvider.instance.MinPlayerX())
        {
            move = ServiceProvider.instance.MinPlayerX() - pos.x;
        }
        pos.x += move;
        transform.position = pos;

        if (Mathf.Approximately(move, 0))
        {}
        else if (move > 0)
        {
            if (!facingR)
            {
                facingR = true;
                sprite.flipX = false;
            }
        }
        else
        {
            if (facingR)
            {
                facingR = false;
                sprite.flipX = true;
            }
        }

        animator.SetBool("walking", !Mathf.Approximately(move, 0));
    }

    void PlayerOtherControls()
    {
        if (controlActive)
        {
            // these are keyed to the 1, 2, 3, 4 keys!
            // make sure the crops they are assigned to are in the same order on the UI
            if (Input.GetButtonDown("Crop1") && (cropSelected != CropType.Corn) && (ServiceProvider.instance.Homestead().HasSeed(CropType.Corn)))
            {
                cropSelected = CropType.Corn;
                ServiceProvider.instance.GetUI().SelectCrop(cropSelected);
            }
            else if (Input.GetButtonDown("Crop2") && (cropSelected != CropType.Carrot) && (ServiceProvider.instance.Homestead().HasSeed(CropType.Carrot)))
            {
                cropSelected = CropType.Carrot;
                ServiceProvider.instance.GetUI().SelectCrop(cropSelected);
            }
            else if (Input.GetButtonDown("Crop3") && (cropSelected != CropType.YuckyPurpleThing) && (ServiceProvider.instance.Homestead().HasSeed(CropType.YuckyPurpleThing)))
            {
                cropSelected = CropType.YuckyPurpleThing;
                ServiceProvider.instance.GetUI().SelectCrop(cropSelected);
            }
/*            else if (Input.GetButtonDown("Crop4") && (cropSelected != CropType.Corn) && (ServiceProvider.instance.Homestead().HasSeed(CropType.Corn)))
            {

            }
*/

        }
    }

    // should only be called if there is dialogue in the dialogue que
    // shows the next line of dialogue, sets timers and removes the line from the que
    void ShowNextDialogue()
    {
        dialogue.text = dialogueQue[0];
        dialogueBackground.enabled = !System.String.IsNullOrEmpty(dialogueQue[0]);
        currentLineLength = dialogueQue[0].Length;
        dialogueLineNext = dialogueLineDelay;
        dialogueLineEnd = dialogueLineDuration;
        dialogueQue.RemoveAt(0);
    }

    // checks the player's dialogue que, shows and progresses it as required
    void PlayerDialogue()
    {
        if (dialogueLineNext > 0) // then it is not time to show the next dialogue line yet
        {
            float rate = 80f / (70f + currentLineLength); // play slower if the string is longer - the default time value is for 10 characters
            if (Input.GetAxis("Vertical") > 0.1f)
            {
                // playerg is pressing up - make dialogue move faster
                rate *= fastDialogueRate;
            }
            else if (Input.GetAxis("Vertical") < -0.1f)
            {
                // player is pressing down - make dialogue move slower
                rate *= slowDialogueRate;
            }
            if (dialogueQue.Count > 3) rate *= ((2f + (float)dialogueQue.Count) / 5); // make dialogue go 20% faster for each quede line above 3

            dialogueLineNext -= Time.deltaTime * rate;
            if (dialogueLineEnd > 0) // then a dialogue line is displayed and we need to handle it
            {
                dialogueLineEnd -= Time.deltaTime * rate;
                if (dialogueLineEnd <= 0)
                {
                    dialogue.text = "";
                    currentLineLength = 0;
                    dialogueBackground.enabled = false;
                }
            }
        }
        else if (dialogueQue.Count > 0)
        {
            ShowNextDialogue();
        }
/*
PSEUDO

IF playing
    update playing timer and nextline timer
    if the playing timer runs out
        remove the current line from que
        clear dialogue text
        next line check

next line check:
if there is a line remaining in the dialogue
    display dialogue text
    start playing timer
else
    stop playing

*/

    }

    void Update()
    {
        PlayerMovement();
        PlayerOtherControls();
        PlayerDialogue();
    }

    // adds the passed dialogue line to the dialogue que, unless it's already in the que (to avoid spamming the keyboard and getting 1000 entries to play back)
    // returns true if the line is accepted
    public bool AddDialogue(string line, bool lowPriority = false)
    {
        if (lowPriority && dialogueQue.Count > 1) return false;
        if (dialogueQue.Contains(line)) return false;
        dialogueQue.Add(line);
        return true;
    }
    // string array handler
    public bool AddDialogue(string[] lines, bool lowPriority = false)
    {
        if (lowPriority && dialogueQue.Count > 1) return false;

        for (int i = 0; i < lines.Length; i++)
            AddDialogue(lines[i]);
        return true;
    }
    // forces the dialogue que to accept this line at the beginning of the que
    // if there is a que, it enters it in index 1 (next line) so it will come up immediately after the current line
    // otherwise, just add it to the que normally
    // PlayerDialogue() will detect the presence of lines
    public void ForceDialogue(string line)
    {
        if (dialogueQue.Count > 1)
            dialogueQue.Insert(1, line);
        else
            AddDialogue(line);
    }

    public void NightFalls()
    {
        string quote;
        switch (10) //Random.Range(0, 10))
        {
            default:
/*            case 0:
            {
                quote = "“Happiness consists of getting enough sleep. Just that, nothing more.” — Robert A. Heinlein, Starship Troopers";
                break;
            }
            case 1:
            {
                quote = "“The best bridge between despair and hope is a good night's sleep.” — E. Joseph Cossman";
                break;
            }
            case 2:
            {
                quote = "“A well-spent day brings happy sleep.” — Leonardo da Vinci";
                break;
            }
            case 3:
            {
                quote = "“Even a soul submerged in sleep is hard at work and helps make something of the world.” — Heraclitus";
                break;
            }
            case 4:
            {
                quote = "“We are such stuff as dreams are made on.” — William Shakespeare";
                break;
            }
            case 5:
            {
                quote = "“True silence is the rest of the mind, and is to the spirit what sleep is to the body, nourishment and refreshment.” — William Penn";
                break;
            }
            case 6:
            {
                quote = "“Whoever thinks of going to bed before twelve o'clock is a scoundrel.” —  Samuel Johnson";
                break;
            }
            case 7:
            {
                quote = "“Early to bed and early to rise makes a man healthy, wealthy, and wise.” —  Benjamin Franklin";
                break;
            }
            case 8:
            {
                quote = "“Never go to sleep without a request to your subconscious.” —  Thomas Edison";
                break;
            }
            case 9:
            {
                quote = "“O dearest charm of sleep, ally against sickness.” —  Euripides";
                break;
            }*/
            case 10:
            {
                quote = "The sun's going down. Time to sleep.";
                break;
            }
        }
        ForceDialogue(quote);
        controlActive = false;
    }
    // sets new day parameters and dialogue
    // this is called by DayNightManager() in the night cycle when the screen is blacked out
    public void NewDay(int day)
    {
        switch (day)
        {
            case 0:
            default:
            {
                // this shouldn't happen as this doesn't get called until day 1
                break;
            }
            case 1:
            {
                break;
            }
            case 2:
            {
                break;
            }
            case 3:
            {
                break;
            }
            case 4:
            {
                break;
            }
            case 5:
            {
                break;
            }
            case 6:
            {
                break;
            }
        }
        transform.position = posStart; // reset the player position
        controlActive = true;
    }

    // called when starving during night transition
    public void NightHunger()
    {
        if (health > 0)
            health--;
        actionPoints = 2; // lose an action point
        ServiceProvider.instance.GetUI().UpdateHealth(health);
        // if health is 0, time to die!
    }

    // called when able to eat well during night transition
    public void NightFeed()
    {
        if (health < healthMax)
            health++;
        else
        {
            actionPoints = 4;
            // TODO gain an extra action point if at full health AND well fed
            // kek, like I'll make it that easy
        }
        ServiceProvider.instance.GetUI().UpdateHealth(health);
    }

    public void NightFood(int feed, bool water)
    {
        if (!water) NightHunger();

        actionPoints = 3;

        switch (feed)
        {
            case 1:
            default:
            {
                break;
            }
            case 0:
            {
                NightHunger();
                break;
            }
            case 2:
            {
                NightFeed();
                break;
            }
        }

        ServiceProvider.instance.GetUI().UpdateActionPoints(actionPoints);
    }

    // special dialogue event for when you first touch a cropslot
    public void DialogueHarvest()
    {
        if (!dialogueDoneHarvest)
        {
            dialogueDoneHarvest = true;
            AddDialogue("[SPACE to HARVEST]");
        }
    }

    // returns true if the player currently has control of the character
    // called before an interacts with the character to avoid conflicts
    public bool IsControlled()
    {
        return controlActive;
    }

    public bool CanAct()
    {
        if (!controlActive) return false;

        if (actionPoints > 0)
            return true;

        AddDialogue("I'm too tired, I should sleep.", true);

        return false;
    }

    // checked when an interactable is checking for player interaction/
    // note: the "Fire1" button should have already been checked here, this is the final "commit or abort" check before actually doing something as it spends action points
    public bool TakeAction()
    {
        if (!controlActive) return false;

        if (actionPoints > 0)
        {
            actionPoints--;
            ServiceProvider.instance.GetUI().UpdateActionPoints(actionPoints);
            if (actionPoints == 0) ServiceProvider.instance.GetDayNightManager().OutOfAP();
            return true;
        }

        //AddDialogue("I'm too tired, I should sleep.");
        // this should have already been caught by CanAct() so something has gone wrong to get here

        return false;
    }

    // returns true if the player is at maximum health
    public bool MaxHealth()
    {
        return (health == healthMax);
    }

    // returns true if the player is out of health
    public int GotHealth()
    {
        return health;
    }

    // returns true if the player is out of health
    public bool NoHealth()
    {
        return (health == 0);
    }

    public CropType CropSelected()
    {
        return cropSelected;
    }

    public bool TravelGoal()
    {
        if (MaxHealth() && (ServiceProvider.instance.Homestead().GotFood() >= 5) && (ServiceProvider.instance.Homestead().GotWater() >= 5)) return true;
        return false;
    }
}
