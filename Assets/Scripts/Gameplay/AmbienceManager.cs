using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

// this manages the ambient wind sounds and other ambient noise
// it has a number of specific tracks and can be called to transition between them
// it will start automatically with default settings
// as well as looping and transitioning, it will also randomly vary the ambient sound to add an extra layer of variation within set parameters

public enum Ambience
{
    Wind = 0,
    Storm = 1,
    Rats = 2
}

public class AmbienceManager : MonoBehaviour
{
    [SerializeField]float defaultVolMin = 0.1f; // the minimum volume the ambient sound can reach
    [SerializeField]float defaultVolMax = 0.5f; // the maximum volume the ambient sound can reach
    [Header("Audio events must be in order of: Wind, Storm, Rats")]
    [SerializeField]StudioEventEmitter[] ambientEvents;
    //[SerializeField]AudioSource[] ambientSource; // the default ambience
    [SerializeField]string[] ambientEventParamName;
    [Header("Volume variation setting")]
    [SerializeField]float variationVolMinTime = 10f; // the minimum time for the next volume variation to complete
    [SerializeField]float variationVolMaxTime = 15f; // the maximum time for the next volume variation to complete
    Ambience ambientType;
    float volMin; // the min variation for the currently primary audio source
    float volMax; // the max variation for the currently primary audio source
    float[] volCurrent;
    float[] volTarget; // the current volume target
    float[] volTargetTime; // the time to take to reach the current volume target linearly
    int trackCount;

    void Awake()
    {
        ambientType = Ambience.Wind;
        trackCount = ambientEvents.Length;
        volCurrent = new float[trackCount];
        volTarget = new float[trackCount];
        volTargetTime = new float[trackCount];
        for (int i = 0; i < trackCount; i++)
        {
            // initialise each ambient track to 0 volume
            volCurrent[i] = 0;
            volTarget[i] = 0;
            volTargetTime[i] = 0;
            ambientEvents[i].SetParameter(ambientEventParamName[i], 0);
            //ambientSource[i].volume = 0;
        }
        AmbienceDefault();
        SelectTargetVolume(); // AmbienceDefault will detect that there is no ambience change so wont set this
    }

    // select a new target volume in the desired range
    void SelectTargetVolume(float setTime = 0)
    {
        volTarget[(int)ambientType] = Random.Range(volMin, volMax);
        if (setTime > 0)
            volTargetTime[(int)ambientType] = setTime;
        else
            volTargetTime[(int)ambientType] = Random.Range(variationVolMinTime, variationVolMaxTime);
    }

    // sets the ambience to the default settings
    public void AmbienceDefault()
    {
        AmbienceCustom(Ambience.Wind, defaultVolMin, defaultVolMax, 1f);
    }

    // set a custom ambience, which includes a transition time for making the transition quickly or slowly
    public void AmbienceCustom(Ambience type, float customVolMin, float customVolMax, float transitionTime)
    {
        volMin = customVolMin;
        volMax = customVolMax;
        if (ambientType == type)
        {
            // no need for transition types, just update the current values
        }
        else
        {
            // tell the current ambient to transition to 0
            volTarget[(int)ambientType] = 0;
            volTargetTime[(int)ambientType] = transitionTime;

            // start the new ambient sound transitioning
            ambientType = type;
            SelectTargetVolume();
        }
    }

    void Update()
    {
        for (int i = 0; i < trackCount; i++)
        {
            float difference = volTarget[i] - volCurrent[i]; // the actual difference from the target volume

            if (!Mathf.Approximately(difference, 0))
            {
                float tickChange = difference / Mathf.Max(volTargetTime[i], 0.01f) * Time.deltaTime; // the difference to apply this frame

                if ((difference > 0 && tickChange >= difference)
                    || (difference < 0 && tickChange <= difference))
                {
                    // the target volume has been reached, complete the transition and 
                    tickChange = difference;
                    volTargetTime[i] = 0;

                    if (i == (int)ambientType)
                    {
                        // this is the currently active audio, so tell it to start a new gradual transition
                        SelectTargetVolume();
                    }
                }
                else 
                {
                    volTargetTime[i] -= Time.deltaTime;
                }

                volCurrent[i] += tickChange;
                ambientEvents[i].SetParameter(ambientEventParamName[i], Global.ApparentToDecibel(volCurrent[i]));
                //ambientSource[i].volume = volCurrent[i];
            }
        }
    }
}
