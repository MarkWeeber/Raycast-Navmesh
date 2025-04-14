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
    [SerializeField] private int _ammoCount = 50;
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
    private bool _focusWasLost;

    private void Start()
    {
        _fireCooldown.DropTime = Time.time + _fireCooldownRate;
        UIManager.Instance.Ammo = _ammoCount;
        GameManager.Instance.OnGameWin += OnGameWinOrLose;
        GameManager.Instance.OnGameLose += OnGameWinOrLose;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameWin -= OnGameWinOrLose;
        GameManager.Instance.OnGameLose -= OnGameWinOrLose;
    }

    private void Update()
    {
        ManageFire();
        ManageFocus();
    }

    private void ManageFire()
    {
        if (
                Mouse.current.leftButton.wasPressedThisFrame            // Input System check
                && _fireCooldown.IsReady(Time.time, _fireCooldownRate)  // Cooldown check
                && _ammoCount > 0                                       // Available ammo check
            )
        {
            if (_focusWasLost) // if focus was lost and user clicked on game screen then user needs to click once again
            {
                _focusWasLost = false;
                return;
            }
            Fire();
            _ammoCount--;
            UIManager.Instance.Ammo = _ammoCount;
        }
    }

    private void ManageFocus() // necessary for switching fps controller on and off in editor mode for convenience
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HideMouseCursorEnableFPSControls();
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            RevealMouseCursorDisableFPSControls();
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
            HideMouseCursorEnableFPSControls();
        }
        else // lose focus - disable aim and movements
        {
            RevealMouseCursorDisableFPSControls();
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

    private void HideMouseCursorEnableFPSControls()
    {
        if (!enabled)
        {
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
        _FPS_Controller.enabled = true;
    }

    private void RevealMouseCursorDisableFPSControls()
    {
        Cursor.lockState = CursorLockMode.None;
        _FPS_Controller.enabled = false;
        _focusWasLost = true;
    }

    private void OnGameWinOrLose()
    {
        RevealMouseCursorDisableFPSControls();
        enabled = false;
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
