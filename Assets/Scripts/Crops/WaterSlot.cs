using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class for the water harvest slot
// this can be harvested like other harvest slots to gain 1-2 water
// it refreshes each night

public class WaterSlot : HarvestSlotParent
{
    bool newHarvest;

    protected override void Awake()
    {
        base.Awake();
        newHarvest = true;
        // start harvestable
        DayTransition();
    }

    // can never have anything planted
    protected override bool Selectable()
    {
        return false;
    }

    // TODO
    protected override void FirstHarvest()
    {
    }

    // gain 1-2 water once per day
    public override void HarvestCrop()
    {
        if (ServiceProvider.instance.Player().TakeAction())
        {
            int waterCount = ServiceProvider.instance.KarmicInt(1, 4);
            if (newHarvest)
            {
                ServiceProvider.instance.Player().AddDialogue("“When the well's dry, we know the worth of water.” — Benjamin Franklin"); // – Benjamin Franklin
                newHarvest = false;
            }

            ServiceProvider.instance.Homestead().AddWater(waterCount);
            grown = false;
            Unselect();
        }
    }

    // make overnight changes
    public override void DayTransition()
    {
        grown = true;
        Unselect(); // force change in tint
    }
}
