using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages crops
// crops are planted into slots (instantiated as child objects)

public class CropSlotManager : HarvestSlotParent
{
    [SerializeField]float cropYoffset = -0.4f; // how far the crop should be positioned offset on the transform
    CropManager crop; // the actual planted crop


    // PlantCrop
    // takes CropType parameter cropNew
    // checks if there is a crop already
    // if no crop, instantiates a crop of the type passed as cropNew
    public void PlantCrop(CropType cropNew, bool fertile = false, bool varyHeight = false)
    {
        if (cropNew == CropType.CornRuin || cropNew == CropType.TomatoRuin || cropNew == CropType.CarrotRuin || cropNew == CropType.YuckyRuin)
        {
            Vector2 pos = transform.position;
            CropManager cropPrefab = ServiceProvider.instance.GetCrop(cropNew);
            pos.y += cropYoffset;

            crop = Instantiate(cropPrefab, pos, cropPrefab.transform.rotation);

            crop.Planted(fertile); // initialise the new crop

            if (fertile)
            {
                boxSelection.color = boxColorGrown;
                grown = true;
            }
            else
            {
                // this is just to clear the selection box so the event action spaces are clear
                boxSelection.color = Color.clear;
                grown = false;
            }
        }
        // plant
        else if (ServiceProvider.instance.Homestead().HasFertiliser())
        {
            if (ServiceProvider.instance.Homestead().UseSeed(cropNew))
            {
                // this has already been checked in the HarvestSlotParent PlayerManager.CanAct call, so should always succeed, so it's ok to already spend resources (UseSeed)
                if (ServiceProvider.instance.Player().TakeAction())
                {
                    ServiceProvider.instance.Homestead().UseFertiliser();

                    if (crop)
                    {
                        Debug.Log("<color=red>ERROR</color> PlantCrop called on CropSlotManager which has a crop");
                    }
                    else
                    {
                        Vector2 pos = transform.position;
                        CropManager cropPrefab = ServiceProvider.instance.GetCrop(cropNew);
                        pos.y += cropYoffset;

                        crop = Instantiate(cropPrefab, pos, cropPrefab.transform.rotation);

                        crop.Planted(fertile); // initialise the new crop

                        Unselect();
                    }
                }
            }
            else
            {
                switch (cropNew)
                {
                    case CropType.Corn:
                    {
                        ServiceProvider.instance.Player().AddDialogue("I have no corn seeds to plant.", true);
                        break;
                    }
                    case CropType.Tomato:
                    {
                        ServiceProvider.instance.Player().AddDialogue("I have no tomato seeds to plant.", true);
                        break;
                    }
                    case CropType.Carrot:
                    {
                        ServiceProvider.instance.Player().AddDialogue("I have no carrot seeds to plant.", true);
                        break;
                    }
                    case CropType.YuckyPurpleThing:
                    {
                        ServiceProvider.instance.Player().AddDialogue("I have no aubergine seeds to plant.", true);
                        break;
                    }
                }
            }
        }
        else
            ServiceProvider.instance.Player().AddDialogue("I have no fertiliser to grow with.");

        if (crop)
        {
            Vector2 scale = crop.transform.localScale;
            if (varyHeight) // used for ruined crops to make them more varied
                scale.y = Random.Range(0.5f, 1f);
            else
                scale.y = Random.Range(0.9f, 1f);
            crop.transform.localScale = scale;
        }
    }

    public override void Plant(CropType cropPlant)
    {
        PlantCrop(cropPlant);
    }

    // returns true if the empty slot can be selected
    // for crops, true if there is no existing crop
    protected override bool Selectable()
    {
        return !crop;
    }

    protected override void FirstHarvest()
    {
        // this is handled in the player as it can only happen once and there are 5 slot crops
        ServiceProvider.instance.Player().DialogueHarvest();// first time interaction on grown crops
    }

    public override void HarvestCrop()
    {
        if (crop.Harvest())
        {
            if (ServiceProvider.instance.Player().TakeAction())
            {
                Destroy(crop.gameObject);
                grown = false;
                Unselect();
            }
        }
        else if (crop)
        {
            // jank
            CropType checkForDead = crop.PlantedCropType();

            if (checkForDead == CropType.CornRuin || checkForDead == CropType.TomatoRuin || checkForDead == CropType.CarrotRuin || checkForDead == CropType.YuckyRuin)
            {
                grown = false;
                Unselect();
            }
        }
    }

    public override void DayTransition()
    {
        if (crop)
        {
            // grow!
            if (crop.Grow())
            {
                // it's fully grown
                boxSelection.color = boxColorGrown;
                grown = true;
            }
        }
    }

    public CropType DestroyCrops(int tier)
    {
        CropType destroyedType = CropType.YuckyRuin; // the returned value is only used if it's one of the four healthy types
        if (crop)
        {
            if (crop.PlantedCropType() == CropType.YuckyPurpleThing && tier < 2)
            {
                Unselect();
                return destroyedType;
            }
            if (crop.PlantedCropType() == CropType.Carrot && tier < 1)
            {
                Unselect();
                return destroyedType;
            }
            destroyedType = crop.PlantedCropType();
            Destroy(crop.gameObject);
            grown = false;
            Unselect();
            return destroyedType;
        }
        return destroyedType;
    }

    public bool CropGrown()
    {
        if (crop) return crop.IsGrown();
        return false;
    }

    // returned the type of crop
    // null crop is returned  as CropType.YuckyRuin, use this only for checking for healthy crop types
    public CropType PlantedType()
    {
        if (crop)
        {
            return crop.PlantedCropType();
        }
        return CropType.YuckyRuin;
    }
}
