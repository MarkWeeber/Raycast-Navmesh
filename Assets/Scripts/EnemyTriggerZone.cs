using UnityEngine;

/// <summary>
/// a special behaviour only for triggers
/// intended to work with EnemyAI only
/// sets their state machine for given state for a given duration only
/// has a probability chance to set, otherwise a set won't happen
/// requires a trigger collider
/// </summary>
public class EnemyTriggerZone : MonoBehaviour
{
    [SerializeField]
    private string _targetTag = "Enemy";
    [SerializeField]
    [Range(0f, 1f)]
    private float _setChance = 0.4f;
    [SerializeField]
    private bool _instantSetState;
    [SerializeField]
    private float _setStateDuration = 3f;
    [SerializeField]
    private EnemyAI.EnemyState _setState = EnemyAI.EnemyState.Hiding;

    private EnemyAI _enemyAI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_targetTag) && _setChance >= Random.Range(0f, 1f))
        {
            if (other.gameObject.TryGetComponent<EnemyAI>(out _enemyAI))
            {
                if (_instantSetState)
                {
                    _enemyAI.State = _setState;
                }
                else
                {
                    _enemyAI.SetStateForDuration(_setState, _setStateDuration);
                }
            }
        }
    }

}
