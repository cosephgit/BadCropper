using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HomeSteadManager
// keeps track of the player's inventory of seeds, fertiliser and food
// stored in the ServiceProvider
// has methods for checking and updating resources
// when resources are changed it passes the change to the UI

public class HomesteadManager : MonoBehaviour
{
    [SerializeField]int seedsCorn;
    [SerializeField]int seedsTomato;
    [SerializeField]int seedsCarrots;
    [SerializeField]int seedsYucky; // because nobody likes eggplant!!!
    [SerializeField]int food;
    [SerializeField]int water;
    [SerializeField]int fertiliser;

    // sets up the UI with the initial values
    // the UI is initialised in Awake()
    void Start()
    {
        if (seedsCorn > 0)
        {
            ServiceProvider.instance.GetUI().UpdateSeeds(CropType.Corn, seedsCorn);
        }
        if (seedsTomato > 0)
        {
            ServiceProvider.instance.GetUI().UpdateSeeds(CropType.Tomato, seedsTomato);
        }
        if (seedsCarrots > 0)
        {
            ServiceProvider.instance.GetUI().UpdateSeeds(CropType.Carrot, seedsCarrots);
        }
        if (seedsYucky > 0)
        {
            ServiceProvider.instance.GetUI().UpdateSeeds(CropType.YuckyPurpleThing, seedsYucky);
        }
        if (food > 0)
        {
            ServiceProvider.instance.GetUI().UpdateFood(food);
        }
        if (water > 0)
        {
            ServiceProvider.instance.GetUI().UpdateWater(water);
        }
        if (fertiliser > 0)
        {
            ServiceProvider.instance.GetUI().UpdateFertiliser(fertiliser);
        }
    }

    // UseSeed with a CropType of seed
    // subtracts the requested type of seed from the inventory if present and returns true
    // returns false if there is no seed of the required type
    public bool UseSeed(CropType seed)
    {
        switch (seed)
        {
            case CropType.Corn:
            {
                if (seedsCorn > 0)
                {
                    seedsCorn--;
                    ServiceProvider.instance.GetUI().UpdateSeeds(seed, seedsCorn);
                    return true;
                }
                else return false;
            }
            case CropType.Tomato:
            {
                if (seedsTomato > 0)
                {
                    seedsTomato--;
                    ServiceProvider.instance.GetUI().UpdateSeeds(seed, seedsTomato);
                    return true;
                }
                else return false;
            }
            case CropType.Carrot:
            {
                if (seedsCarrots > 0)
                {
                    seedsCarrots--;
                    ServiceProvider.instance.GetUI().UpdateSeeds(seed, seedsCarrots);
                    return true;
                }
                else return false;
            }
            case CropType.YuckyPurpleThing:
            {
                if (seedsYucky > 0)
                {
                    seedsYucky--;
                    ServiceProvider.instance.GetUI().UpdateSeeds(seed, seedsYucky);
                    return true;
                }
                else return false;
            }
        }
        return false;
    }

    public bool HasSeed(CropType seed)
    {
        switch (seed)
        {
            case CropType.Corn:
            {
                if (seedsCorn > 0)
                    return true;
                else return false;
            }
            case CropType.Tomato:
            {
                if (seedsTomato > 0)
                    return true;
                else return false;
            }
            case CropType.Carrot:
            {
                if (seedsCarrots > 0)
                    return true;
                else return false;
            }
            case CropType.YuckyPurpleThing:
            {
                if (seedsYucky > 0)
                    return true;
                else return false;
            }
        }
        return false;
    }

    public int GetSeed(CropType seed)
    {
        switch (seed)
        {
            case CropType.Corn:
            {
                return seedsCorn;
            }
            case CropType.Tomato:
            {
                return seedsTomato;
            }
            case CropType.Carrot:
            {
                return seedsCarrots;
            }
            case CropType.YuckyPurpleThing:
            {
                return seedsYucky;
            }
        }
        return 0;
    }

    // adds seeds and updates UI
    public void AddSeed(CropType seed, int count)
    {
        switch (seed)
        {
            case CropType.Corn:
            {
                seedsCorn += count;
                ServiceProvider.instance.GetUI().UpdateSeeds(seed, seedsCorn);
                break;
            }
            case CropType.Tomato:
            {
                seedsTomato += count;
                ServiceProvider.instance.GetUI().UpdateSeeds(seed, seedsTomato);
                break;
            }
            case CropType.Carrot:
            {
                seedsCarrots += count;
                ServiceProvider.instance.GetUI().UpdateSeeds(seed, seedsCarrots);
                break;
            }
            case CropType.YuckyPurpleThing:
            {
                seedsYucky += count;
                ServiceProvider.instance.GetUI().UpdateSeeds(seed, seedsYucky);
                break;
            }
        }
    }


    public bool HasFertiliser()
    {
        return (fertiliser > 0);
    }
    // if fertiliser > 0, use fertiliser and update UI and return true
    // else return false
    public bool UseFertiliser()
    {
        if (fertiliser > 0)
        {
            fertiliser--;
            ServiceProvider.instance.GetUI().UpdateFertiliser(fertiliser);
            return true;
        }
        return false;
    }
    // adds fertiliser and updates UI
    public void AddFertiliser(int count)
    {
        fertiliser += count;
        ServiceProvider.instance.GetUI().UpdateFertiliser(fertiliser);
    }
    // if water > 0, use fertiliser and update UI and return true
    // else return false
    public bool UseWater()
    {
        if (water > 0)
        {
            water--;
            ServiceProvider.instance.GetUI().UpdateWater(water);
            return true;
        }
        return false;
    }
    // adds water and updates UI
    public void AddWater(int count)
    {
        water += count;
        ServiceProvider.instance.GetUI().UpdateWater(water);
    }

    // if food > 0, use food and update UI and return true
    // else return false
    public bool UseFood(int count = 1)
    {
        if (food >= count)
        {
            food -= count;
            ServiceProvider.instance.GetUI().UpdateFood(food);
            return true;
        }
        return false;
    }
    // adds food and updates UI
    public void AddFood(int count)
    {
        food += count;
        ServiceProvider.instance.GetUI().UpdateFood(food);
    }
    // returns the current food amount
    public int GotFood()
    {
        return food;
    }
    // returns the current water amount
    public int GotWater()
    {
        return water;
    }

    // called by the daynightmanager on the night transition
    public void DayTransition()
    {
        AddFertiliser(1); // get one fertiliser per day, you know why
        if (UseFood())
            ServiceProvider.instance.Player().NightFeed();
        else
            ServiceProvider.instance.Player().NightHunger();
    }
}
