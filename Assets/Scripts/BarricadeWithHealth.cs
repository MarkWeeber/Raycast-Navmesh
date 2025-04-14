using System.Collections;
using UnityEngine;

/// <summary>
/// A simple class for barricades
/// when hit multiple times disables the barricade for some given time
/// </summary>
public class BarricadeWithHealth : MonoBehaviour
{
    [SerializeField]
    private int _hitHealth = 3;
    [SerializeField]
    private float _reviveTime = 5f;
    [SerializeField]
    private GameObject _barricade;
    [SerializeField]
    private Collider _collider;

    private int _startingHitHealth;

    private void Start()
    {
        _startingHitHealth = _hitHealth;
    }

    public void Hit()
    {
        if (_hitHealth > 0)
        {
            _hitHealth--;
            if (_hitHealth == 0)
            {
                HideBarricade();
            }
        }
    }

    private void HideBarricade()
    {
        _barricade.gameObject.SetActive(false);
        StartCoroutine(ReviveCoroutine());
        _collider.enabled = false;

    }

    IEnumerator ReviveCoroutine()
    {
        yield return new WaitForSeconds(_reviveTime);
        _barricade.gameObject.SetActive(true);
        _hitHealth = _startingHitHealth;
        _collider.enabled = true;
    }
}
