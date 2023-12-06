using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingTrigger : MonoBehaviour
{
    bool playerIsNear = false;
    FishingPanel fishingPanel;

    private void Start()
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
