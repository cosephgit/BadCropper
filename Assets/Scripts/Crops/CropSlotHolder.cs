using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropSlotHolder : MonoBehaviour
{
    [SerializeField]CropSlotManager[] cropSlots; // array containing all crop slots, used to manage their growth overnight and destruction from events
    [SerializeField]WaterSlot waterSlot;
    [SerializeField]FertiliserSlot fertiliserSlot;
    [SerializeField]CropSlotHolder eventCropSlots;

    public void DayTransition()
    {
        for (int i = 0; i < cropSlots.Length; i++)
        {
            cropSlots[i].DayTransition();
        }
        waterSlot.DayTransition();
        fertiliserSlot.DayTransition();
    }


    // this is called during events to destroy the crops
    public void DestroyAll(int tier)
    {
        for (int i = 0; i < cropSlots.Length; i++)
        {
            CropType typeDestroyed = cropSlots[i].DestroyCrops(tier);
            switch (typeDestroyed)
            {
                case CropType.Corn:
                {
                    eventCropSlots.EventPlantCrop(CropType.CornRuin, i);
                    break;
                }
                case CropType.Tomato:
                {
                    eventCropSlots.EventPlantCrop(CropType.TomatoRuin, i);
                    break;
                }
                case CropType.Carrot:
                {
                    eventCropSlots.EventPlantCrop(CropType.CarrotRuin, i);
                    break;
                }
                case CropType.YuckyPurpleThing:
                {
                    eventCropSlots.EventPlantCrop(CropType.YuckyRuin, i);
                    break;
                }
                default:
                {
                    eventCropSlots.EventPlantCrop(CropType.CornRuin, i, false); // place anything in the unpopulated slots, this means the user will not be able to interact
                    break;
                }
            }
        }
    }

    // this is received by the event crop slots to place ruined crops in place
    public void EventPlantCrop(CropType cropNew, int index, bool fertile = true)
    {
        //cropSlots[index].DestroyCrops(); // there shouldn't be anything there though
        cropSlots[index].PlantCrop(cropNew, fertile, true);
    }

    public bool HasRuinedCrops()
    {
        bool result = false;

        for (int i = 0; i < cropSlots.Length; i++)
        {
            if (cropSlots[i].CropGrown()) result = true;
        }

        return result;
    }

    // returns true if any of the crop slots contain the indicated type of crop
    public bool HasAnyCrops(CropType type)
    {
        bool result = false;

        for (int i = 0; i < cropSlots.Length; i++)
        {
            if (cropSlots[i].PlantedType() == type) result = true;
        }

        return result;
    }
}
