using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// service provider for any required global access
// also provided global variables
// singleton

public class ServiceProvider : MonoBehaviour
{
    public static ServiceProvider instance;
    [SerializeField]PlayerManager player;
    [SerializeField]UIManager userInterface;
    [SerializeField]float minPlayerX;
    [SerializeField]float maxPlayerX;
    [SerializeField]CropManager cropCorn;
    [SerializeField]CropManager cropTomato;
    [SerializeField]CropManager cropCarrot;
    [SerializeField]CropManager cropPurple;
    [SerializeField]CropManager cropCornRuin;
    [SerializeField]CropManager cropTomatoRuin;
    [SerializeField]CropManager cropCarrotRuin;
    [SerializeField]CropManager cropPurpleRuin;
    [SerializeField]HomesteadManager homestead;
    [SerializeField]CropSlotHolder cropHolder;
    [SerializeField]DayNightManager dayNightManager;
    [SerializeField]AmbienceManager ambienceManager;
    [SerializeField]QuestManager questManager;
    int karma; // player karma is used to make sure bad luck is balanced out - giving a sense of unpredictability while maintaining balance

    void Awake()
    {
        if (instance)
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            instance = this;
        }
        karma = 0;
    }

    public CropManager GetCrop(CropType crop)
    {
        switch (crop)
        {
            case CropType.Corn:
            {
                return cropCorn;
            }
            case CropType.Tomato:
            {
                return cropTomato;
            }
            case CropType.Carrot:
            {
                return cropCarrot;
            }
            case CropType.YuckyPurpleThing: // because nobody likes eggplant!!!
            {
                return cropPurple;
            }
            case CropType.CornRuin:
            {
                return cropCornRuin;
            }
            case CropType.TomatoRuin:
            {
                return cropTomatoRuin;
            }
            case CropType.CarrotRuin:
            {
                return cropCarrotRuin;
            }
            case CropType.YuckyRuin: // because nobody likes eggplant!!!
            {
                return cropPurpleRuin;
            }
        }

        Debug.Log("<color=red>ERROR</color> ServiceProvider GetCrop failed to match the CropType " + crop);
        return cropCorn;
    }

    // used to produce random numbers but avoid consistent runs of good or bad luck
    // it always treats high numbers as good and low numbers as bad, keep that in mind with what you do with the number
    public int KarmicInt(int min, int maxExclusive)
    {
        int value = Random.Range(min, maxExclusive);
        float mean = ((float)min + (float)(maxExclusive - 1)) / 2f; // remember it's maxExclusive, so the actual maximum is one less

        if (karma > 0)
        {
            if (value <= mean)
            {
                // we got a low result, spend karma
                karma--;
                value = Random.Range(min, maxExclusive); // get a re-roll and reduce our good karma
            }
        }
        else if (karma < 0)
        {
            if (value >= mean)
            {
                // we got a high result, spend negative karma
                karma++;
                value = Random.Range(min, maxExclusive); // get a re-roll and reduce our bad karma
            }
        }

        if (value < mean)
        {
            karma++;
        }
        else if (value > mean)
        {
            karma--;
        }
        return value;
    }

    public PlayerManager Player()
    {
        return player;
    }
    public HomesteadManager Homestead()
    {
        return homestead;
    }
    public CropSlotHolder CropSlots()
    {
        return cropHolder;
    }
    public UIManager GetUI()
    {
        return userInterface;
    }
    public DayNightManager GetDayNightManager()
    {
        return dayNightManager;
    }
    public AmbienceManager GetAmbienceManager()
    {
        return ambienceManager;
    }
    public QuestManager GetQuestManager()
    {
        return questManager;
    }
    public float MinPlayerX()
    {
        return minPlayerX;
    }
    public float MaxPlayerX()
    {
        return maxPlayerX;
    }
}
