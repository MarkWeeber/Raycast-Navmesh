using GameDevHQ.FileBase.Plugins.FPS_Character_Controller;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Main player class
/// Shoots at enemies using Raycasts
/// for visual idenity, each time shooting happens a laser beam is spawned
/// also controls cursor visibility by overriding FPS_Controller class
/// </summary>
public class Player : MonoBehaviour
{
    [SerializeField] private float _fireCooldownRate = 0.5f;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private Camera _camera;
    [SerializeField] private FPS_Controller _FPS_Controller;
    [SerializeField] private GameObject _laserBeamPrefab;
    [SerializeField] private Transform _laserBeamOriginTransform;
    [SerializeField] private float _laserBeamLifeTime = 0.3f;
    [SerializeField] private float _rayCastLength = 50f;

    private Ray _ray;
    private RaycastHit _hit;
    private SimpleCooldown _fireCooldown;
    private EnemyAI _enemyAI;
    private LineRenderer _laserRenderer;
    private GameObject _laserSpawnedObject;

    private Vector3 pos1;
    private Vector3 pos2;

    private void Start()
    {
        _fireCooldown.DropTime = Time.time + _fireCooldownRate;
    }

    private void Update()
    {
        ManageFire();
        ManageFocus();
    }

    private void ManageFire()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && _fireCooldown.IsReady(Time.time, _fireCooldownRate))
        {
            Fire();
        }
    }

    private void ManageFocus() // necessary for switching fps controller on and off in editor mode for convenience
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _FPS_Controller.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            _FPS_Controller.enabled = false;
        }
    }

    private void Fire()
    {
        AudioManager.Instance.PlayLaserFireSound(transform.position);
        _ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(_ray, out _hit, _rayCastLength, _targetMask))
        {
            if (_hit.collider.TryGetComponent<EnemyAI>(out _enemyAI)) // enemy was hit
            {
                if (_enemyAI.State != EnemyAI.EnemyState.Dead)
                {
                    _enemyAI.State = EnemyAI.EnemyState.Dead;
                }
            }
            else // barricade was hit
            {
                AudioManager.Instance.PlayLaserHitBarricadeSound(_hit.point);
            }
            // if enemy or barrel was hit, then render between hit point and laser beam origin
            SpawnLaserBeam(_laserBeamOriginTransform.position, _hit.point, _laserBeamLifeTime);
        }
        else // if no enemy was hit, then just render forward laser beam
        {
            SpawnLaserBeam(_laserBeamOriginTransform.position, _laserBeamOriginTransform.position + _laserBeamOriginTransform.forward * _rayCastLength, _laserBeamLifeTime);
        }
        
    }


    // controlling cursor whenever application focus changes
    private void OnApplicationFocus(bool focus)
    {
        if (focus) // gain focus, hide cursor
        {
            Cursor.lockState = CursorLockMode.Locked;
            _FPS_Controller.enabled = true;
        }
        else // lose focus - disable aim and movements
        {
            Cursor.lockState = CursorLockMode.None;
            _FPS_Controller.enabled = false;
        }
    }

    private void SpawnLaserBeam(Vector3 origin, Vector3 end, float lifetTime)
    {
        _laserSpawnedObject = Instantiate(_laserBeamPrefab);
        if (_laserSpawnedObject.TryGetComponent<LineRenderer>(out _laserRenderer))
        {
            _laserRenderer.SetPosition(0, origin);
            _laserRenderer.SetPosition(1, end);
        }
        Destroy(_laserSpawnedObject, lifetTime);
    }    
}

/// <summary>
/// A struct for cooldown
/// Used in Player to limit firing rate
/// </summary>
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
