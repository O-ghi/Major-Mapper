using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingTrigger : EntityBase
{
    bool playerIsNear = false;
    FishingPanel fishingPanel;

    public FishingTrigger(GameObject _obj) : base(_obj)
    {
        GameEventManager.Singleton.onInteract += InteractPressed;

    }

    public void InteractPressed()
    {
        if (!playerIsNear)
        {
            return;
        }
        if (fishingPanel == null)
            fishingPanel = PanelManager.SetPanel("FishingPanel") as FishingPanel;
        else
        {
            fishingPanel.gameObject.SetActive(true);
        }
    }

    protected void CheckCollisions()
    {
        // Perform your collision checks here
        // Example: Check for collisions with all colliders in the scene
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.name.Equals("Player"))
            {
                playerIsNear = true;
                return;
            }
        }
        playerIsNear = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            playerIsNear = true;
        }
    }
    private void OnDestroy()
    {
        GameEventManager.Singleton.onInteract -= InteractPressed;

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            playerIsNear = false;
            fishingPanel.gameObject.SetActive(false);

        }
    }
}
