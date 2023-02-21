using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// gives the player a small chance of gathering seeds with each search attempt
// a soak for excess actions, and a desperate last resort, ultimately

public class WeedsSearch : HarvestSlotParent
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

    // 1 in 4 chance of gaining seeds each try
    public override void HarvestCrop()
    {
        if (ServiceProvider.instance.Player().TakeAction())
        {
            int seedSearch = ServiceProvider.instance.KarmicInt(0, 3); // only succeeds on a 3, so a 1-in-4 chance of success
            bool getSeed = false;

            if (seedSearch == 1)
            {
                // get a seed only if the player doesn't have any
                if (!ServiceProvider.instance.Homestead().HasSeed(CropType.Corn)
                    && !ServiceProvider.instance.Homestead().HasSeed(CropType.Carrot)
                    && !ServiceProvider.instance.Homestead().HasSeed(CropType.YuckyPurpleThing))
                {
                    getSeed = true;
                }
            }
            else if (seedSearch == 2)
            {
                getSeed = true;
            }

            if (getSeed)
            {
                CropType selection = CropType.Corn;

                if (newHarvest)
                {
                    ServiceProvider.instance.Player().AddDialogue("“Diligence is the mother of good fortune.” ― Miguel de Cervantes Saavedra"); // ― Miguel de Cervantes Saavedra 
                    newHarvest = false;
                }

                switch (Random.Range(0, 2))
                {
                    default:
                    case 0:
                    {
                        break;
                    }
                    case 1:
                    {
                        if (!ServiceProvider.instance.Homestead().HasSeed(CropType.Carrot)
                            && ServiceProvider.instance.GetQuestManager().QuestStage() > 0)
                        {
                            selection = CropType.Carrot;
                        }
                        break;
                    }
                    case 2:
                    {
                        if (!ServiceProvider.instance.Homestead().HasSeed(CropType.YuckyPurpleThing)
                            && ServiceProvider.instance.GetQuestManager().QuestStage() > 1)
                        {
                            selection = CropType.YuckyPurpleThing;
                        }
                        break;
                    }
                }

                ServiceProvider.instance.Homestead().AddSeed(selection, 1);
            }

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
