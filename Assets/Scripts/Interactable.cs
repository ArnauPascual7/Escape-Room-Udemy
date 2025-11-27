using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PreviousItem
{
    public Item RequiredItem;
    public Item InteractionItem;

    public UnityEvent OnInteract;
}

public class Interactable : MonoBehaviour
{
    public Item item;

    public PreviousItem[] PreviousItems;

    public UnityEvent OnInteract;
    public UnityEvent OnCollectItem;

    [HideInInspector]
    public bool isMoving;
}
