using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UINightMenu : MonoBehaviour
{
    [SerializeField]Button buttonRationStarve;
    [SerializeField]Button buttonRationRation;
    [SerializeField]Button buttonRationFeast;
    [SerializeField]TextMeshProUGUI buttonRationFeastImpact;
    [SerializeField]GameObject tileWater;
    [SerializeField]GameObject tileWaterNot;
    [SerializeField]Button buttonSleep;
    int feed;
    bool water;

    public void Open()
    {
        int food = ServiceProvider.instance.Homestead().GotFood();

        // open the night menu and initialise the buttons
        if (food >= 2)
        {
            buttonRationFeast.interactable = true;
            buttonRationRation.interactable = true;
        }
        else if (food == 1)
        {
            buttonRationFeast.interactable = false;
            buttonRationRation.interactable = true;
        }
        else
        {
            buttonRationFeast.interactable = false;
            buttonRationRation.interactable = false;
        }
        buttonSleep.interactable = false;
        feed = 0;

        water = (ServiceProvider.instance.Homestead().GotWater() > 0);
        if (water && ServiceProvider.instance.Player().MaxHealth())
        {
            // if the player is at max health, has water, and feasts, they get a bonus action point instead of health
            buttonRationFeastImpact.text = "(+1 Action)";
        }
        else
        {
            buttonRationFeastImpact.text = "(+1 Health)";
        }

        tileWater.SetActive(water);
        tileWaterNot.SetActive(!water);
    }

    public void ButtonStarvePressed()
    {
        feed = 0;
        buttonSleep.interactable = true;
    }
    public void ButtonRationPressed()
    {
        feed = 1;
        buttonSleep.interactable = true;
    }
    public void ButtonFeastPressed()
    {
        feed = 2;
        buttonSleep.interactable = true;
    }

    public void ButtonSleepPressed()
    {
        ServiceProvider.instance.Player().NightFood(feed, water); // affects player health
        ServiceProvider.instance.Homestead().UseFood(feed);
        ServiceProvider.instance.Homestead().AddFertiliser(1);
        if (water) ServiceProvider.instance.Homestead().UseWater();
        ServiceProvider.instance.GetDayNightManager().NightFinish();
        gameObject.SetActive(false);
    }
}
