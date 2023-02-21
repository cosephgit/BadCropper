using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
PSEUDO

a timer runs slowly from start to finish with the sun visibly moving and the light level changing

player gets, say, 3 action points

when they run out (or at any time?) they can return to their camp and end the day
maybe only after 30 seconds into the day
when they end the day the timer runs much faster and the sun and colour shift quickly
*/

public class DayNightManager : MonoBehaviour
{
    [SerializeField]Transform sun;
    [SerializeField]SpriteRenderer tint;
    [SerializeField]float dayLength = 300f; // seconds duration
    [SerializeField]float sunPosXMin = -5f;
    [SerializeField]float sunPosXMax = 5f;
    [SerializeField]float sunPosYApex = 2f;
    [SerializeField]Color tintColorSunMin;
    [SerializeField]Color tintColorSunMax;
    [SerializeField]QuestManager questStatus;
    [SerializeField]float eventMult = 0.5f;
    [SerializeField]float outOfAPMult = 8;
    float sunPosYMin; // the minimum of the sun is taken as the start Y pos, this is added to it
    float dayTimeLeft;
    float dayTimeRush; // when the player wants to end the day this is adjusted to end it quickly
    const float FADETOBLACKTIME = 5f;
    int dayNumber;
    bool nightHold; // hold the night until the night menu is closed
    bool eventHold; // hold the night until the event menu is closed

    void Awake()
    {
        dayTimeLeft = dayLength;
        dayTimeRush = 1;
        sunPosYMin = sun.position.y;
        dayNumber = 0;
        nightHold = false;
        eventHold = false;
    }

    void LightAndSun()
    {
        float sunIncrement = (dayLength - dayTimeLeft) / dayLength;
        float sunPosX = sunPosXMin + (sunIncrement * (sunPosXMax - sunPosXMin));
        float sunHeightBase = Mathf.Sin(sunIncrement * Mathf.PI);
        float sunposY = sunPosYMin + (sunHeightBase * sunPosYApex); // sun pos follows a sine curve
        Vector2 sunPos = new Vector2(sunPosX, sunposY);
        Color tintColor = Color.Lerp(tintColorSunMin, tintColorSunMax, sunHeightBase);

        tint.color = tintColor;

        sun.localPosition = sunPos;

        dayTimeLeft -= Time.deltaTime * dayTimeRush;
        if (dayTimeLeft <= 0)
        {
            StartCoroutine(DayTransition());
        }
    }

    void Update()
    {
        if (dayTimeLeft > 0)
        {
            // manage the Light and Sun level while the timer is above 0
            LightAndSun();
        }
    }

    // this coroutine runs the fade to black/fade back transition, timers, updating player position, and notifies the homestead to update crops and player food
    // when it ends the dayTimeLeft timer is updated, which allows the LightAndSun() method to be called and resume the usual day cycle
    IEnumerator DayTransition()
    {
        float fadeToBlack = 0; // time to fade out

        ServiceProvider.instance.Player().NightFalls(); // stops play control and sends dialogue line

        // fade to black cycle updated each frame
        while (fadeToBlack < FADETOBLACKTIME)
        {
            Color fade = Color.Lerp(tintColorSunMin, Color.black, fadeToBlack / FADETOBLACKTIME); // fade from orangest shade to black
            tint.color = fade;
            yield return new WaitForEndOfFrame();
            fadeToBlack += Time.deltaTime;
        }

        dayNumber++;

        ServiceProvider.instance.CropSlots().DayTransition();

        // open the night menu
        ServiceProvider.instance.GetUI().NightMenuOpen();

        // wait for the night menu to be closed
        nightHold = true;
        while (nightHold)
        {
            yield return new WaitForEndOfFrame();
        }

        if (ServiceProvider.instance.Player().NoHealth())
        {
            // game over! pop up ending menu, they can close it to start over
            ServiceProvider.instance.GetUI().Defeat();
            yield break;
        }

        eventHold = questStatus.NightAdvance();
        if (eventHold)
        {
            dayTimeRush = eventMult; // the day will run slower until the player has completed all event actions
        }
        else
        {
            dayTimeRush = 1;
        }
        while(eventHold)
        {
            yield return new WaitForEndOfFrame();
        }

        // put the sun in the right position as we fade back in
        sun.localPosition = new Vector2(sunPosXMin, sunPosYMin);

        if (!ServiceProvider.instance.GetQuestManager().VictoryCheck())
        {
            // reactivate the player and play any new day dialogue
            // not if the player has won, then we just want the day to fade back in and show the win menu
            ServiceProvider.instance.Player().NewDay(dayNumber);
        }

        // only update after the event and rationing is complete
        ServiceProvider.instance.GetUI().SetGoals((ServiceProvider.instance.Homestead().GotWater() >= Global.GOALWATER), (ServiceProvider.instance.Homestead().GotFood() >= Global.GOALFOOD), ServiceProvider.instance.Player().MaxHealth());

        // fade back in cycle updated each frame
        float fadeBack = 0;
        while (fadeBack < FADETOBLACKTIME)
        {
            Color fade = Color.Lerp(Color.black, tintColorSunMin, fadeBack / FADETOBLACKTIME); // fade from orangest shade to black
            tint.color = fade;
            yield return new WaitForEndOfFrame();
            fadeBack += Time.deltaTime;
        }

        if (ServiceProvider.instance.GetQuestManager().VictoryCheck())
        {
            // game over! pop up ending menu, they can close it to start over
            ServiceProvider.instance.GetUI().Victory();
        }
        else
        {
            dayTimeLeft = dayLength; // this is set here as it starts up the daylight cycle again
        }
    }

    public void NightFinish()
    {
        // the player has selected their rationing, the night can now finish
        nightHold = false;
    }
    public void EventFinish()
    {
        // the player has read the evet and closed it, the night can now finish
        eventHold = false;
    }

    // called by the player when they run out of AP, to cause the day to end quickly
    // may also be called by interacting with the shack to just cycle days quickly
    public void OutOfAP()
    {
        dayTimeRush = outOfAPMult;
    }
}
