using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the actual crop object

public enum CropType
{
    Corn,
    Tomato,
    Carrot,
    YuckyPurpleThing, // because nobody likes eggplant!!!
    CornRuin,
    TomatoRuin,
    CarrotRuin,
    YuckyRuin
}

public class CropManager : MonoBehaviour
{
    const int MAXSTAGE = 3;
    [SerializeField]CropType type;
    [SerializeField]Sprite bud;
    [SerializeField]Sprite spriteStage0;
    [SerializeField]Sprite spriteStage1;
    [SerializeField]Sprite spriteStage2;
    CropSlotManager cropManager;
    SpriteRenderer cropSprite;
    int growth;

    // initialise crop after instantiation
    public void Planted(bool fertile = false)
    {
        cropSprite = GetComponent<SpriteRenderer>();
        if (fertile)
        {
            cropSprite.sprite = spriteStage2;
            growth = MAXSTAGE;
        }
        else
        {
            growth = 0;
            cropSprite.sprite = bud;
        }
    }

    // advance the crop to the next growth stage
    // returns true if fully grown
    public bool Grow()
    {
        if (growth < MAXSTAGE)
        {
            growth++;
            if (growth == 1)
            {
                cropSprite.sprite = spriteStage0;
            }
            else if (growth == 2)
            {
                cropSprite.sprite = spriteStage1;
            }
            else
            {
                cropSprite.sprite = spriteStage2;
            }
        }
        return (growth == MAXSTAGE);
    }

    public bool Harvest()
    {
        if (growth < MAXSTAGE)
        {
            return false;
        }
        else if (type == CropType.CornRuin || type == CropType.TomatoRuin || type == CropType.CarrotRuin || type == CropType.YuckyRuin)
        {
            ServiceProvider.instance.Homestead().AddFertiliser(1);
            growth = 1;
            cropSprite.sprite = spriteStage0;
            return false; // this means that, despite being harvested, the crop will not be destroyed and an action point will not be spent
            // the above lines mean it can't be harvested more than once
            // boy this is janky af, if this wasn't a game jam game made in 5 days I'd be ashamed of myself
        }
        else
        {
            switch (ServiceProvider.instance.KarmicInt(0, 4))
            {
                case 0:
                {
                    // minimum "unlucky" result, just one of each (enough to barely survive)
                    ServiceProvider.instance.Homestead().AddFood(1);
                    ServiceProvider.instance.Homestead().AddSeed(type, 1);
                    break;
                }
                case 1:
                default:
                {
                    // average result, a bonus food but no more seeds
                    ServiceProvider.instance.Homestead().AddFood(2);
                    ServiceProvider.instance.Homestead().AddSeed(type, 1);
                    break;
                }
                case 2:
                {
                    // good result, normal food but more seeds give better long-term chances
                    ServiceProvider.instance.Homestead().AddFood(1);
                    ServiceProvider.instance.Homestead().AddSeed(type, 2);
                    break;
                }
                case 3:
                {
                    // average result, a bonus food but no more seeds
                    ServiceProvider.instance.Homestead().AddFood(2);
                    ServiceProvider.instance.Homestead().AddSeed(type, 1);
                    break;
                }
            }
            return true;
        }
    }

    public bool IsGrown()
    {
        return (growth == MAXSTAGE);
    }

    public CropType PlantedCropType()
    {
        return type;
    }
}
