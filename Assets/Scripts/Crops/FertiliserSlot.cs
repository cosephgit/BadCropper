using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class for the water harvest slot
// this can be harvested like other harvest slots to gain 0-1 fertiliser
// it refreshes each night

public class FertiliserSlot : HarvestSlotParent
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

    // gain 0-1 fertiliser once per day
    public override void HarvestCrop()
    {
        if (ServiceProvider.instance.Player().TakeAction())
        {
            int fertiliserCount = 1; //ServiceProvider.instance.KarmicInt(1, 3);

            if (newHarvest)
            {
                ServiceProvider.instance.Player().AddDialogue("“The life of the dead is placed in the heart of the living.” ― Cicero"); //
                newHarvest = false;
                //ServiceProvider.instance.Player().AddDialogue("Even in the grave, all is not lost."); // ― Edgar Allan Poe.
            }

            ServiceProvider.instance.Homestead().AddFertiliser(fertiliserCount);
            //grown = false;
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
