using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Singleton instance of UIManager, allowing easy access to its methods and variables from other scripts
    public static UIManager Instance;


    // UI elements for different interaction feedbacks and the inventory system
    public GameObject HandCursor; // UI element representing a hand cursor to indicate interactable objects
    public GameObject BackImg; // UI element to display a "back" button or image when viewing items
    public TextMeshProUGUI CaptionText; // Text UI for displaying captions or descriptions
    public Image InteractionImage; // Image UI for showing item icons or interaction hints
    public GameObject InventoryImg; // The entire inventory UI panel
    public Image[] InventoryIcons; // Array of UI Images to display item icons in the inventory
    public TextMeshProUGUI[] InventoryItems; // Array of TextMeshProUGUI for showing item names in inventory slots
    public TextMeshProUGUI InfoText; // Text UI for providing information or feedback to the player


    // Called when the script instance is being loaded
    private void Awake()
    {

        Instance = this; // Assigns this instance of UIManager to the static "instance" variable (Singleton pattern)
    }


    // Called once per frame
    private void Update()
    {
        // Toggles the inventory UI on or off when the "I" key is pressed
        if (Input.GetKeyDown(KeyCode.I))
        {
            InventoryImg.SetActive(!InventoryImg.activeInHierarchy); // Switch the active state of the inventory UI
        }
    }


    // Toggles the visibility of the hand cursor UI element
    public void SetHandCursor(bool state)
    {
        HandCursor.SetActive(state); // Activates or deactivates the hand cursor based on the "state" parameter
    }


    // Toggles the visibility of the backImage UI element and manages interaction image behavior
    public void SetBackImg(bool state)
    {
        BackImg.SetActive(state); // Activates or deactivates the backImage UI based on the "state" parameter

        // If backImage is turned off, also disable the interaction image
        if (!state)
        {
            InteractionImage.enabled = false; // Hides the interaction image when the back button is not active
        }
    }


    // Sets the caption text displayed on the UI
    public void SetCaptionText(string text)
    {
        CaptionText.text = text; // Updates the captionText UI element with the provided text
    }


    // Updates the interaction image to display a specific sprite and enables it
    public void SetImage(Sprite spr)
    {
        InteractionImage.sprite = spr; // Sets the interaction image to the provided sprite
        InteractionImage.enabled = true; // Enables the interaction image so it becomes visible
    }


    // Updates the inventory slot with the given item's information at the specified index
    public void SetItems(Item item, int index)
    {
        InventoryItems[index].text = item.CollectMsg; // Sets the inventory slot text to the item's collection message
        InfoText.text = item.CollectMsg; // Displays the same message as temporary feedback to the player

        InventoryIcons[index].enabled = true; // Enables the item image at the given index
        InventoryIcons[index].sprite = item.ItemIcon; // Sets the image sprite to the item's icon

        StartCoroutine(FadingText()); // Starts the coroutine to fade in and out the infoText
    }


    // Coroutine that gradually fades the infoText in and then fades it out after a delay
    IEnumerator FadingText()
    {
        Color newColor = InfoText.color; // Store the current color of the infoText

        // Fade in: Increase the alpha value of the text color from 0 to 1 over time
        while (newColor.a < 1)
        {
            newColor.a += Time.deltaTime; // Gradually increase alpha based on the frame time
            InfoText.color = newColor; // Update the text color with the new alpha value
            yield return null; // Wait until the next frame
        }

        yield return new WaitForSeconds(2f); // Wait for 2 seconds with the text fully visible

        // Fade out: Decrease the alpha value of the text color from 1 to 0 over time
        while (newColor.a > 0)
        {
            newColor.a -= Time.deltaTime; // Gradually decrease alpha based on the frame time
            InfoText.color = newColor; // Update the text color with the new alpha value
            yield return null; // Wait until the next frame
        }

    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= InventoryItems.Length)
        {
            return;
        }

        InventoryItems[index].text = "";
        InventoryIcons[index].enabled = false;
        InventoryIcons[index].sprite = null;
    }

    public void RefreshItems(List<Item> items)
    {
        for (int i = 0; i < InventoryItems.Length; i++)
        {
            if (i < items.Count)
            {
                InventoryItems[i].text = items[i].CollectMsg;
                InventoryIcons[i].enabled = true;
                InventoryIcons[i].sprite = items[i].ItemIcon;
            }
            else
            {
                InventoryItems[i].text = "";
                InventoryIcons[i].enabled = false;
                InventoryIcons[i].sprite = null;
            }
        }
    }
}
