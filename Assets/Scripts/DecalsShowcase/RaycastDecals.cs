using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastDecals : MonoBehaviour
{
    [SerializeField]
    private GameObject _decalPrefab;

    private Ray _ray;
    private RaycastHit _hit;
    private Camera _camera;
    private GameObject _instantiatedDecal;
    private Quaternion _decalRotation;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(_ray, out _hit))
            {
                //_decalRotation = Quaternion.LookRotation(_hit.normal * -1f); // always allign with hitting surface
                _decalRotation = Quaternion.LookRotation(_ray.direction); // align with look direction
                _instantiatedDecal = Instantiate(_decalPrefab, _hit.point, _decalRotation); 
                _instantiatedDecal.transform.parent = _hit.collider.transform; // make sure that decal stays with hit object, even when object is moving
            }
        }
    }
}
