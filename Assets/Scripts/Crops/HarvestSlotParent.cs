using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestSlotParent : MonoBehaviour
{
    [SerializeField]protected SpriteRenderer boxSelection; // the highlight box showing the selection
    [SerializeField]protected Color boxColorActive;
    [SerializeField]protected Color boxColorHighlight;
    [SerializeField]protected Color boxColorGrown; // for when the crop is fully grown
    [SerializeField]protected Color boxColorGrownHighlight; // for when the crop is grown and highlighted
    [SerializeField]protected float selectionOffset = 0.3f; // how close (in X units) the player needs to be to select this box
    [SerializeField]bool hideIfEmpty = false;
    protected bool grown;
    protected bool selected; // is this box currently lit up?

    protected virtual void Awake()
    {
        grown = false;
        selected = false;
    }


    // done in lateupdate so player movement is completed in Update() first
    void LateUpdate()
    {
        if (selected)
        {
            float offset = Mathf.Abs(ServiceProvider.instance.Player().transform.position.x - transform.position.x);

            if (offset > selectionOffset)
            {
                Unselect();
            }
            else if (Input.GetButtonDown("Fire1")
                && ServiceProvider.instance.Player().CanAct())
            {
                if (grown)
                    HarvestCrop();
                else
                    Plant(ServiceProvider.instance.Player().CropSelected());
            }
        }
        else
        {
            float offset = Mathf.Abs(ServiceProvider.instance.Player().transform.position.x - transform.position.x);

            if (offset < selectionOffset)
            {
                if (Selectable()) // select to plant crops
                    Select();
                else if (grown) // select to harvest crops
                {
                    Select();
                    FirstHarvest();
                }
            }
        }
    }

    // change sprite to lit state
    protected void Select()
    {
        selected = true;
        if (grown)
            boxSelection.color = boxColorGrownHighlight;
        else if (hideIfEmpty)
            boxSelection.color = Color.clear;
        else
            boxSelection.color = boxColorHighlight;
    }
    // change sprite to unlit state
    protected void Unselect()
    {
        selected = false;
        if (grown)
            boxSelection.color = boxColorGrown;
        else if (hideIfEmpty)
            boxSelection.color = Color.clear;
        else
            boxSelection.color = boxColorActive;
    }

    // returns whether the empty slot can be selected (used for planting crops)
    protected virtual bool Selectable()
    {
        return true;
    }

    // reports first interaction at the player
    protected virtual void FirstHarvest()
    {

    }

    // plant crops
    public virtual void Plant(CropType cropPlant)
    {
    }

    // harvest whatever this produces
    public virtual void HarvestCrop()
    {
    }

    // make overnight changes
    public virtual void DayTransition()
    {
    }
}
