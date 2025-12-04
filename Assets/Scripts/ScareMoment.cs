using UnityEngine;
using UnityEngine.Events;

public class ScareMoment : MonoBehaviour
{
    public bool IsActivated;
    public UnityEvent OnScare;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsActivated && other.CompareTag("Player"))
        {
            IsActivated = true;
            OnScare.Invoke();
        }
    }
}
