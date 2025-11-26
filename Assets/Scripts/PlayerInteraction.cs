using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteraction : MonoBehaviour
{
    private Camera _mainCamera;

    [Tooltip("RayCast max distance")]
    public float RayDistance = 2f;
    public float RotateSpeed = 200f;

    public UnityEvent OnView;
    public UnityEvent OnFinishView;

    private Interactable _currentInteractable;
    private bool isViewing;
    private bool canFinish;

    public Transform ObjectViewer;

    private Vector3 _originPosition;
    private Quaternion _originRotation;

    private AudioPlayer _audioPlayer;

    private void Awake()
    {
        _audioPlayer = GetComponent<AudioPlayer>();
    }

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        CheckInteractables();
    }

    void CheckInteractables()
    {
        if (isViewing)
        {
            if (_currentInteractable.item.grabbable && Input.GetMouseButton(0))
            {
                RotateObject();
            }

            if (canFinish && Input.GetMouseButton(1))
            {
                FinishView();
            }

            return;
        }

        RaycastHit hit;
        Vector3 rayOrigin = _mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f));

        if (Physics.Raycast(rayOrigin, _mainCamera.transform.forward, out hit, RayDistance))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                UIManager.Instance.SetHandCursor(true);

                if (Input.GetMouseButtonDown(0))
                {
                    if (interactable.isMoving)
                    {
                        return;
                    }

                    _currentInteractable = interactable;

                    _currentInteractable.OnInteract.Invoke();

                    if (_currentInteractable.item != null)
                    {
                        OnView.Invoke();

                        isViewing = true;

                        Interact(_currentInteractable.item);

                        if (_currentInteractable.item.grabbable)
                        {
                            _originPosition = _currentInteractable.transform.position;
                            _originRotation = _currentInteractable.transform.rotation;

                            StartCoroutine(MovingObject(_currentInteractable, ObjectViewer.position));
                        }
                    }
                }
            }
            else
            {
                UIManager.Instance.SetHandCursor(false);
            }
        }
        else
        {
            UIManager.Instance.SetHandCursor(false);
        }
    }

    void Interact(Item item)
    {
        if (item.Image != null)
        {
            UIManager.Instance.SetImage(item.Image);
        }

        _audioPlayer.PlayAudio(item.AudioClip);

        UIManager.Instance.SetCaptionText(item.Text);

        Invoke(nameof(CanFinish), item.AudioClip.length + 0.5f);
    }

    void CanFinish()
    {
        canFinish = true;

        if (_currentInteractable.item.Image == null && !_currentInteractable.item.grabbable)
        {
            FinishView();
        }
        else
        {
            UIManager.Instance.SetBackImg(true);
        }

        UIManager.Instance.SetCaptionText("");
    }

    void FinishView()
    {
        canFinish = false;
        isViewing = false;

        UIManager.Instance.SetBackImg(false);

        if (_currentInteractable.item.grabbable)
        {
            _currentInteractable.transform.rotation = _originRotation;

            StartCoroutine(MovingObject(_currentInteractable, _originPosition));
        }

        OnFinishView.Invoke();
    }

    IEnumerator MovingObject(Interactable obj, Vector3 position)
    {
        obj.isMoving = true;

        float timer = 0f;

        while (timer < 1)
        {
            obj.transform.position = Vector3.Lerp(obj.transform.position, position, Time.deltaTime * 5);
            timer += Time.deltaTime;

            yield return null;
        }

        obj.transform.position = position;
        obj.isMoving = false;
    }

    void RotateObject()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        _currentInteractable.transform.Rotate(_mainCamera.transform.right, -Mathf.Deg2Rad * y * RotateSpeed, Space.World);
        _currentInteractable.transform.Rotate(_mainCamera.transform.up, -Mathf.Deg2Rad * x * RotateSpeed, Space.World);
    }
}
