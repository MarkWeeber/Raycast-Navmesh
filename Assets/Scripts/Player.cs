using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float _fireCooldownRate = 1f;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField ]private Camera _camera;

    private Ray _ray;
    private RaycastHit _hit;
    private SimpleCooldown _fireCooldown;
    private EnemyAI _enemyAI;

    private void Start()
    {
        _fireCooldown.DropTime = Time.time + _fireCooldownRate;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && _fireCooldown.IsReady(Time.time, _fireCooldownRate))
        {
            Fire();
        }
    }

    private void Fire()
    {
        _ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(_ray, out _hit, 500f, _targetMask))
        {
            if (_hit.collider.TryGetComponent<EnemyAI>(out _enemyAI))
            {
                if (_enemyAI.State != EnemyAI.EnemyState.Dead)
                {
                    _enemyAI.State = EnemyAI.EnemyState.Dead;
                }
            }
        }
    }

}

public struct SimpleCooldown
{
    public float DropTime;
    private float _timeToDrop;

    public bool IsReady(float time, float newRate)
    {
        _timeToDrop = DropTime - time;
        if (_timeToDrop <= 0f)
        {
            DropTime = time + newRate;
            return true;
        }
        else
        {
            return false;
        }
    }
}
