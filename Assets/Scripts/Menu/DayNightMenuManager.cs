using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightMenuManager : MonoBehaviour
{
    [SerializeField]Transform sun;
    [SerializeField]SpriteRenderer tint;
    [SerializeField]float dayLength = 300f; // seconds duration
    [SerializeField]float sunPosXMin = -5f;
    [SerializeField]float sunPosXMax = 5f;
    [SerializeField]float sunPosYApex = 1f;
    [SerializeField]Color tintColorSunMin;
    [SerializeField]Color tintColorSunMax;
    float sunPosYMin; // the minimum of the sun is taken as the start Y pos, this is added to it
    float dayTimeLeft;
    float dayTimeRush; // when the player wants to end the day this is adjusted to end it quickly
    const float FADETOBLACKTIME = 4f;

    void Awake()
    {
        dayTimeLeft = dayLength;
        dayTimeRush = 1;
        sunPosYMin = sun.position.y;
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

        // fade to black cycle updated each frame
        while (fadeToBlack < FADETOBLACKTIME)
        {
            Color fade = Color.Lerp(tintColorSunMin, Color.black, fadeToBlack / FADETOBLACKTIME); // fade from orangest shade to black
            tint.color = fade;
            yield return new WaitForEndOfFrame();
            fadeToBlack += Time.deltaTime;
        }

        // put the sun in the right position as we fade back in
        sun.localPosition = new Vector2(sunPosXMin, sunPosYMin);

        // fade back in cycle updated each frame
        float fadeBack = 0;
        while (fadeBack < FADETOBLACKTIME)
        {
            Color fade = Color.Lerp(Color.black, tintColorSunMin, fadeBack / FADETOBLACKTIME); // fade from orangest shade to black
            tint.color = fade;
            yield return new WaitForEndOfFrame();
            fadeBack += Time.deltaTime;
        }

        dayTimeLeft = dayLength; // this is set here as it starts up the daylight cycle again
    }
}
