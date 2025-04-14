using UnityEngine;

/// <summary>
/// A simple class to sphere cast and set Enemy states
/// once object is enabled
/// casts once
/// </summary>
public class EnemyArreaSetter : MonoBehaviour
{
    [SerializeField]
    private float _radius = 5f;
    [SerializeField]
    private LayerMask _targetMask;
    [SerializeField]
    private EnemyAI.EnemyState _targetState = EnemyAI.EnemyState.Dead;

    private EnemyAI _enemyAI;

    private void OnEnable()
    {
        SetOff();
    }

    private void SetOff()
    {
        var collisions = Physics.OverlapSphere(transform.position, _radius, _targetMask);
        foreach (var collider in collisions)
        {
            if (collider.TryGetComponent<EnemyAI>(out _enemyAI))
            {
                _enemyAI.State = _targetState;
            }
        }
    }
}
