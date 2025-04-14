using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A simple class that would hold some events and clears self
/// </summary>
public class RayCastZone : MonoBehaviour
{
    [SerializeField]
    private float _destroySelfAfterHit = 3f;

    public UnityEvent HitEvent;

    private bool _activated;

    public void Hit()
    {
        if (!_activated)
        {
            HitEvent?.Invoke();
            Destroy(gameObject, _destroySelfAfterHit);
        }
        _activated = true;
    }
}
