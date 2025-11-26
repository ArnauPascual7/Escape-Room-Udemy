using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public Item item;

    [HideInInspector]
    public bool isMoving;

    public UnityEvent OnInteract;
}
