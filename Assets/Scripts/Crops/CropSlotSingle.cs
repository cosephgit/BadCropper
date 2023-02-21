using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is used for single-used harvest points, it automatically spawns the indicated crop and when harvested it is disabled

public class CropSlotSingle : HarvestSlotParent
{
    [SerializeField]CropType cropSet;
    [SerializeField]float cropYoffset = -0.4f; // how far the crop should be positioned offset on the transform
    CropManager crop; // the actual planted crop

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        // start harvestable, after the service provider singleton is set up (during Awake())
        PrePlant();
    }

    void PrePlant()
    {
        Vector2 pos = transform.position;
        CropManager cropPrefab = ServiceProvider.instance.GetCrop(cropSet);
        pos.y += cropYoffset;

        crop = Instantiate(cropPrefab, pos, cropPrefab.transform.rotation, transform);

        crop.Planted(true); // initialise the new crop fully grown
        grown = true;

        Unselect();
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

    // single use harvesting
    public override void HarvestCrop()
    {
        if (crop.Harvest())
        {
            if (ServiceProvider.instance.Player().TakeAction())
            {
                switch (cropSet)
                {
                    case CropType.Tomato:
                    {
                        ServiceProvider.instance.Player().AddDialogue("");
                        break;
                    }
                    case CropType.Carrot:
                    {
                        ServiceProvider.instance.Player().AddDialogue("The corn was ruined by the hurricane, maybe these will survive better.");
                        break;
                    }
                    case CropType.YuckyPurpleThing:
                    {
                        ServiceProvider.instance.Player().AddDialogue("Interesting, the vermin don't seem to like this. Hopefully I'll disagree.");
                        break;
                    }
                }
                Destroy(crop.gameObject);
                // tell the quest manager that the special event has been completed
                ServiceProvider.instance.GetQuestManager().EventIndividualComplete();
                gameObject.SetActive(false);
            }
        }
    }
}
