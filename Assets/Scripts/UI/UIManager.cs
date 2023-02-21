using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// UIManager keeps UI elements up to date

public class UIManager : MonoBehaviour
{
    [SerializeField]Image seedBoxCorn; // used to reveal the seed box only when they are first added
    [SerializeField]UIResourceBox seedCountCorn;
    [SerializeField]Image seedBoxTomato;
    [SerializeField]UIResourceBox seedCountTomato;
    [SerializeField]Image seedBoxCarrot;
    [SerializeField]UIResourceBox seedCountCarrot;
    [SerializeField]Image seedBoxYucky;
    [SerializeField]UIResourceBox seedCountYucky; // because nobody likes eggplant!!!
    [SerializeField]Color seedBoxNormal;
    [SerializeField]Color seedBoxSelected;
    [SerializeField]GameObject fertiliserBox;
    [SerializeField]UIResourceBox fertiliserCount;
    [SerializeField]GameObject waterBox;
    [SerializeField]UIResourceBox waterCount;
    [SerializeField]GameObject foodBox;
    [SerializeField]UIResourceBox foodCount;
    [SerializeField]Slider healthSlider;
    [SerializeField]Image healthSliderFill;
    [SerializeField]Color healthLowColor;
    [SerializeField]Color healthMaxColor;
    [SerializeField]UINightMenu nightMenu;
    [SerializeField]UIActionPoint actionPoints;
    [SerializeField]UIEventMenu eventMenu;
    [SerializeField]UIEventMenu winMenu;
    [SerializeField]UIEventMenu loseMenu;
    [SerializeField]UIGoalBox goalBox;

    void Awake()
    {
        seedBoxCorn.gameObject.SetActive(false);
        seedBoxTomato.gameObject.SetActive(false);
        seedBoxCarrot.gameObject.SetActive(false);
        seedBoxYucky.gameObject.SetActive(false);
        fertiliserBox.SetActive(false);
        waterBox.SetActive(false);
        foodBox.SetActive(false);
        nightMenu.gameObject.SetActive(false);
        eventMenu.gameObject.SetActive(false);
        winMenu.gameObject.SetActive(false);
        loseMenu.gameObject.SetActive(false);
    }

    // called by HomeSteadManager in Start() to set initial values

    // updates the UI element to show the number of seeds of the given type
    // enables the box, so the first time it's called the box becomes visible
    public void UpdateSeeds(CropType seed, int count)
    {
        switch (seed)
        {
            case CropType.Corn:
            {
                seedBoxCorn.gameObject.SetActive(true);
                seedCountCorn.UpdateStat(count);
                break;
            }
            case CropType.Tomato:
            {
                seedBoxTomato.gameObject.SetActive(true);
                seedCountTomato.UpdateStat(count);
                break;
            }
            case CropType.Carrot:
            {
                seedBoxCarrot.gameObject.SetActive(true);
                seedCountCarrot.UpdateStat(count);
                break;
            }
            case CropType.YuckyPurpleThing:
            {
                seedBoxYucky.gameObject.SetActive(true);
                seedCountYucky.UpdateStat(count);
                break;
            }
        }
    }

    public void UpdateFood(int count)
    {
        foodBox.SetActive(true);
        foodCount.UpdateStat(count);
    }

    public void UpdateWater(int count)
    {
        waterBox.SetActive(true);
        waterCount.UpdateStat(count);
    }

    public void UpdateFertiliser(int count)
    {
        fertiliserBox.SetActive(true);
        fertiliserCount.UpdateStat(count);
    }

    public void InitHealth(int value, int max)
    {
        healthSlider.maxValue = max;
        UpdateHealth(value);
    }
    public void UpdateHealth(int value)
    {
        Color barColor;
        float healthLevel = (float)value / (float)healthSlider.maxValue;

        if (healthLevel < 0.3f)
        {
            barColor = healthLowColor;
        }
        else if (healthLevel > 0.7f)
        {
            barColor = healthMaxColor;
        }
        else
        {
            barColor = Color.Lerp(healthLowColor, healthMaxColor, healthLevel);
        }

        healthSliderFill.color = barColor;
        healthSlider.value = value;
    }

    public void NightMenuOpen()
    {
        nightMenu.gameObject.SetActive(true);
        nightMenu.Open();
    }

    public void EventMenuOpen(string eventText)
    {
        eventMenu.gameObject.SetActive(true);
        eventMenu.SetEvent(eventText);
    }
    public void EventMenuClose()
    {
        eventMenu.gameObject.SetActive(false);
    }

    public void UpdateActionPoints(int points)
    {
        actionPoints.UpdateAP(points);
    }

    public void SelectCrop(CropType crop)
    {
        switch (crop)
        {
            case CropType.Corn:
            {
                seedBoxCorn.color = seedBoxSelected;
                seedBoxTomato.color = seedBoxNormal;
                seedBoxCarrot.color = seedBoxNormal;
                seedBoxYucky.color = seedBoxNormal;
                break;
            }
            case CropType.Tomato:
            {
                seedBoxCorn.color = seedBoxNormal;
                seedBoxTomato.color = seedBoxSelected;
                seedBoxCarrot.color = seedBoxNormal;
                seedBoxYucky.color = seedBoxNormal;
                break;
            }
            case CropType.Carrot:
            {
                seedBoxCorn.color = seedBoxNormal;
                seedBoxTomato.color = seedBoxNormal;
                seedBoxCarrot.color = seedBoxSelected;
                seedBoxYucky.color = seedBoxNormal;
                break;
            }
            case CropType.YuckyPurpleThing:
            {
                seedBoxCorn.color = seedBoxNormal;
                seedBoxTomato.color = seedBoxNormal;
                seedBoxCarrot.color = seedBoxNormal;
                seedBoxYucky.color = seedBoxSelected;
                break;
            }
        }
    }

    public void Victory()
    {
        string quote;

        switch (Random.Range(0,13))
        {
            case 0:
            default:
            {
                quote = "“I never dreamed about success. I worked for it.” — Estée Lauder";
                break;
            }
            case 1:
            {
                quote = "“I'm a greater believer in luck, and I find the harder I work the more I have of it.” — Thomas Jefferson";
                break;
            }
            case 2:
            {
                quote = "“Opportunity is missed by most people because it is dressed in overalls and looks like work.” — Thomas Edison";
                break;
            }
            case 3:
            {
                quote = "“He is a wise man who does not grieve for the things which he has not, but rejoices for those which he has.” — Epictetus";
                break;
            }
            case 4:
            {
                quote = "“The most difficult thing is the decision to act, the rest is merely tenacity.” — Amelia Earhart";
                break;
            }
            case 5:
            {
                quote = "“Opportunities don't happen, you create them.” — Chris Grosser";
                break;
            }
            case 6:
            {
                quote = "“I am not a product of my circumstances. I am a product of my decisions.” — Stephen R. Covey"; 
                break;
            }
            case 7:
            {
                quote = "“Do what you can, with what you have, where you are.” ― Theodore Roosevelt"; 
                break;
            }
            case 8:
            {
                quote = "“You cannot plow a field by turning it over in your mind. To begin, begin.” ―Gordon B. Hinckley"; 
                break;
            }
            case 9:
            {
                quote = "“When you arise in the morning think of what a privilege it is to be alive, to think, to enjoy, to love…” ― Marcus Aurelius"; 
                break;
            }
            case 10:
            {
                quote = "“He that can have patience can have what he will.” ― Benjamin Franklin"; 
                break;
            }
            case 11:
            {
                quote = "“Life is like riding a bicycle. To keep your balance you must keep moving.” — Albert Einstein"; 
                break;
            }
            case 12:
            {
                quote = "“The greater the difficulty, the more the glory in surmounting it.” ― Epicurus"; 
                break;
            }
        }

        quote = quote + "\nThanks for playing!";

        winMenu.gameObject.SetActive(true);
        winMenu.SetEvent(quote);
    }
    public void Defeat()
    {
        string quote;

        switch (Random.Range(0,14))
        {
            case 0:
            default:
            {
                quote = "“Failure isn't fatal, but failure to change might be.” - John Wooden"; // - John Wooden 5/10
                break;
            }
            case 1:
            {
                quote = "“Giving up is the only sure way to fail.” - Gena Showalter"; // “Giving up is the only sure way to fail.” - Gena Showalter
                break;
            }
            case 2:
            {
                quote = "“There is no failure except in no longer trying.” - Chris Bradford"; // There is no failure except in no longer trying.” - Chris Bradford
                break;
            }
            case 3:
            {
                quote = "“I have not failed. I've just found 10,000 ways that won't work.” - Thomas A. Edison"; // “I have not failed. I've just found 10,000 ways that won't work.” - Thomas A. Edison
                break;
            }
            case 4:
            {
                quote = "“Success is not final, failure is not fatal: it is the courage to continue that counts.” - Winston Churchill"; // “Success is not final, failure is not fatal: it is the courage to continue that counts.” - Winston Churchill
                break;
            }
            case 5:
            {
                quote = "“The only real mistake is the one from which we learn nothing.” - Henry Ford"; // “The only real mistake is the one from which we learn nothing.” - Henry Ford
                break;
            }
            case 6:
            {
                quote = "“Failures are finger posts on the road to achievement.” - C.S. Lewis"; 
                break;
            }
            case 7:
            {
                quote = "“Every adversity, every failure, every heartache carries with it the seed of an equal or greater benefit.” - Napoleon Hill"; 
                break;
            }
            case 8:
            {
                quote = "“It’s not how far you fall, but how high you bounce that counts.” - Zig Ziglar"; 
                break;
            }
            case 9:
            {
                quote = "“When we give ourselves permission to fail, we, at the same time, give ourselves permission to excel.” - Eloise Ristad"; 
                break;
            }
            case 10:
            {
                quote = "“Do not fear mistakes. You will know failure. Continue to reach out.” ― Benjamin Franklin"; 
                break;
            }
            case 11:
            {
                quote = "“The difference between average people and achieving people is their perception of and response to failure.” ― John C. Maxwell"; 
                break;
            }
            case 12:
            {
                quote = "“Develop success from failures. Discouragement and failure are two of the surest stepping stones to success.” ― Dale Carnegie"; 
                break;
            }
            case 13:
            {
                quote = "“Failure is success in progress.” ― Albert Einstein"; 
                break;
            }
            // "Do not fear mistakes. You will know failure. Continue to reach out." ― Benjamin Franklin
            // "The difference between average people and achieving people is their perception of and response to failure." ― John C. Maxwell
            // "Develop success from failures. Discouragement and failure are two of the surest stepping stones to success." ― Dale Carnegie
            // "Failure is success in progress." ― Albert Einstein
        }

        loseMenu.gameObject.SetActive(true);
        loseMenu.SetEvent(quote);
    }

    public void SetGoals(bool water, bool food, bool health)
    {
        goalBox.GoalsUpdate(water, food, health);
    }
}
