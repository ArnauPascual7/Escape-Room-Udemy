using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteraction : MonoBehaviour
{
    // Declare a private Camera object to reference the main camera in the scene.
    private Camera _mainCamera;


    // Public float to specify the maximum distance for raycasting interactions.
    [Tooltip("RayCast max distance")]
    public float RayDistance = 2f;

    // Public float to control the speed of rotating interactable objects.
    [Tooltip("Speed of the rotation of interactable objects")]
    public float RotateSpeed = 200f;


    // UnityEvent that is triggered when the player begins viewing an interactable object.
    public UnityEvent OnView;

    // UnityEvent that is triggered when the player finishes viewing an interactable object.
    public UnityEvent OnFinishView;


    // Private reference to the currently interacted object that has an Interactable component.
    private Interactable _currentInteractable;

    // Private reference to the currently interacted item that has been interacted with.
    private Item _currentItem;

    // Private boolean to check if the player is currently viewing an interactable object.
    private bool isViewing;

    // Private boolean to determine if the player can finish viewing an object.
    private bool canFinish;


    // Public Transform reference to the position where objects will be moved for viewing.
    public Transform ObjectViewer;

    // Private vector to store the original position of the interactable object.
    private Vector3 _originPosition;

    // Private quaternion to store the original rotation of the interactable object.
    private Quaternion _originRotation;


    // Private reference to the AudioPlayer component, used for playing sound effects.
    private AudioPlayer _audioPlayer;

    // Private reference to the PlayerInventory component, used for managing collected items.
    private PlayerInventory _inventory;

    // Public AudioClip to store the sound that plays when adding an item to the player's inventory.
    public AudioClip WritingSound;


    // Awake is called when the script instance is being loaded, before Start().
    private void Awake()
    {
        // Find and store the AudioPlayer component on the same GameObject.
        _audioPlayer = GetComponent<AudioPlayer>();

        // Find and store the PlayerInventory component on the same GameObject.
        _inventory = GetComponent<PlayerInventory>();
    }


    // Start is called before the first frame update.
    private void Start()
    {
        // Set the Camera reference to the main camera in the scene.
        _mainCamera = Camera.main;
    }


    // Update is called once per frame to check for interactions.
    private void Update()
    {
        // Call the function to check for interactable objects in front of the camera.
        CheckInteractables();
    }


    // Function to check for interactable objects using a raycast and handle interaction logic.
    void CheckInteractables()
    {
        // If the player is already viewing an interactable object:
        if (isViewing)
        {
            // Allow the player to rotate the object if it is marked as grabbable and the left mouse button is held.
            if (_currentInteractable.item.grabbable && Input.GetMouseButton(0))
            {
                RotateObject();
            }


            // Allow the player to finish viewing if the right mouse button is pressed and finishing is enabled.
            if (canFinish && Input.GetMouseButton(1))
            {
                FinishView();
            }

            // Exit the function to prevent further interaction checks while viewing.
            return;
        }

        // Declare a RaycastHit object to store information about the object hit by the ray.
        RaycastHit hit;

        // Calculate the origin point of the ray at the center of the camera’s viewport.
        Vector3 rayOrigin = _mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f));


        // Perform a raycast from the calculated origin in the camera's forward direction, up to the specified rayDistance.
        if (Physics.Raycast(rayOrigin, _mainCamera.transform.forward, out hit, RayDistance))
        {
            // Attempt to get an Interactable component from the object hit by the ray.
            Interactable interactable = hit.collider.GetComponent<Interactable>();


            // If the hit object has an Interactable component:
            if (interactable != null)
            {
                // Change the cursor to indicate that the object is interactable.
                UIManager.Instance.SetHandCursor(true);


                // Check if the player has clicked the left mouse button.
                if (Input.GetMouseButtonDown(0))
                {
                    // If the object is currently moving, prevent interaction.
                    if (interactable.isMoving)
                    {
                        return;
                    }

                    // Set the currentInteractable to the object and invoke its onInteract UnityEvent.
                    _currentInteractable = interactable;
                    _currentInteractable.OnInteract.Invoke();


                    // If the interactable has an associated item:
                    if (_currentInteractable.item != null)
                    {
                        // Invoke the onView UnityEvent and set the isViewing flag to true.
                        OnView.Invoke();
                        isViewing = true;


                        // Boolean to check if the player has a required previous item.
                        bool hasPreviousItem = false;

                        // Loop through the interactable's list of previous items.
                        for (int i = 0; i < _currentInteractable.PreviousItems.Length; i++)
                        {
                            // Check if the player's inventory contains the required previous item.
                            if (_inventory.Items.Contains(_currentInteractable.PreviousItems[i].RequiredItem))
                            {
                                // Interact with the item and invoke its interaction event.
                                Interact(_currentInteractable.PreviousItems[i].InteractionItem);
                                _currentInteractable.PreviousItems[i].OnInteract.Invoke();

                                _inventory.RemoveItem(_currentInteractable.PreviousItems[i].RequiredItem);
                                _currentInteractable.item = _currentInteractable.PreviousItems[i].InteractionItem;

                                hasPreviousItem = true; // Set flag to true if an interaction occurs.
                                break; // Exit the loop after finding a matching item.
                            }
                        }

                        // If the item is grabbable, store its original position and rotation, and move it to the objectViewer.
                        if (_currentInteractable.item.grabbable)
                        {
                            _originPosition = _currentInteractable.transform.position;
                            _originRotation = _currentInteractable.transform.rotation;

                            StartCoroutine(MovingObject(_currentInteractable, ObjectViewer.position));
                        }

                        // If a previous item interaction occurred, exit the function.
                        if (hasPreviousItem)
                        {
                            return;
                        }

                        // Interact with the current item if no previous item interaction occurred.
                        Interact(_currentInteractable.item);
                    }
                }
            }
            else // If no interactable object is hit, reset the cursor to the default state.
            {
                UIManager.Instance.SetHandCursor(false);
            }
        }
        else // If the raycast does not hit anything, reset the cursor to the default state.
        {
            UIManager.Instance.SetHandCursor(false);
        }
    }


    // Function to handle interaction with an Item.
    void Interact(Item item)
    {
        // Set the currentItem to the provided item.
        _currentItem = item;


        // Display the item's image in the UI if it has one.
        if (item.Image != null)
        {
            UIManager.Instance.SetImage(item.Image);
        }

        // Play the item's audio clip and set its associated text in the UI.
        _audioPlayer.PlayAudio(item.AudioClip);
        UIManager.Instance.SetCaptionText(item.Text);

        if (item.AudioClip != null)
        {
            // Invoke the CanFinish function after the audio clip finishes playing.
            Invoke(nameof(CanFinish), item.AudioClip.length + 0.5f);
        }
        else
        {
            Invoke(nameof(CanFinish), 1f);
        }
    }

    // Function to handle logic when the player is allowed to finish viewing an item.
    void CanFinish()
    {
        // Set canFinish to true, allowing the player to exit the viewing mode.
        canFinish = true;


        // Automatically finish viewing if the item has no image and is not grabbable.
        if (_currentItem.Image == null && !_currentItem.grabbable)
        {
            FinishView();
        }
        else
        {
            // Show the "Back" UI option to manually finish viewing.
            UIManager.Instance.SetBackImg(true);
        }

        // Clear the caption text from the UI.
        UIManager.Instance.SetCaptionText("");
    }


    // Function to finish viewing the current item and reset the viewing state.
    void FinishView()
    {
        // Reset the canFinish and isViewing flags.
        canFinish = false;
        isViewing = false;


        // Hide the "Back" UI option.
        UIManager.Instance.SetBackImg(false);


        // Add the item to the player's inventory if it is marked as collectible.
        if (_currentItem.InventoryItem)
        {
            _inventory.AddItem(_currentItem);

            _audioPlayer.PlayAudio(WritingSound);

            _currentInteractable.OnCollectItem.Invoke();
        }


        // If the item is grabbable, return it to its original position and rotation.
        if (_currentItem.grabbable)
        {
            _currentInteractable.transform.rotation = _originRotation;

            StartCoroutine(MovingObject(_currentInteractable, _originPosition));
        }

        // Invoke the onFinishView UnityEvent to signal that viewing is complete.
        OnFinishView.Invoke();
    }


    // Coroutine to move an interactable object smoothly to a specified position.
    IEnumerator MovingObject(Interactable obj, Vector3 position)
    {
        // Mark the object as moving.
        obj.isMoving = true;


        // Timer variable to control the movement interpolation.
        float timer = 0f;


        // Gradually move the object to the target position over time.
        while (timer < 1)
        {
            obj.transform.position = Vector3.Lerp(obj.transform.position, position, Time.deltaTime * 5);
            timer += Time.deltaTime;

            yield return null; // Wait for the next frame.
        }


        // Ensure the object's final position is set accurately.
        obj.transform.position = position;
        obj.isMoving = false; // Mark the object as no longer moving.
    }


    // Function to rotate the interactable object based on mouse input.
    void RotateObject()
    {
        // Get the mouse input along the X-axis and Y-axis.
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");


        // Rotate the object around the camera's right axis based on vertical mouse movement.
        _currentInteractable.transform.Rotate(_mainCamera.transform.right, -Mathf.Deg2Rad * y * RotateSpeed, Space.World);

        // Rotate the object around the camera's up axis based on horizontal mouse movement.
        _currentInteractable.transform.Rotate(_mainCamera.transform.up, -Mathf.Deg2Rad * x * RotateSpeed, Space.World);
    }
}
