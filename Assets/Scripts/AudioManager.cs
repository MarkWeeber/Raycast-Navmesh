using UnityEngine;

/// <summary>
/// Simple Audio Managing class, a singleton
/// in order to sound correctly spawns a gameobject in game scene space, so they are spatially blent
/// </summary>
public class AudioManager : SingletonBehaviour<AudioManager>
{
    [SerializeField]
    private GameObject _laserFire;
    [SerializeField]
    private GameObject _laserHitBarricade;
    [SerializeField]
    private GameObject _enemyKilled;
    [SerializeField]
    private GameObject _enemyEscaped;

    private GameObject _soundObject;
    private AudioSource _audioSource;

    public void PlayLaserFireSound(Vector3 position)
    {
        PlayAudioSourceAtPosition(_laserFire, position);
    }

    public void PlayLaserHitBarricadeSound(Vector3 position)
    {
        PlayAudioSourceAtPosition(_laserHitBarricade, position);
    }
    public void PlayEnemyKilledSound(Vector3 position)
    {
        PlayAudioSourceAtPosition(_enemyKilled, position);
    }
    public void PlayEnemyEscapedSound(Vector3 position)
    {
        PlayAudioSourceAtPosition(_enemyEscaped, position);
    }

    private void PlayAudioSourceAtPosition(GameObject prefab, Vector3 position)
    {
        _soundObject = Instantiate(prefab, position, Quaternion.identity);
        _audioSource = _soundObject.GetComponent<AudioSource>();
        _audioSource.Play();
        Destroy(_soundObject, 2f);
    }
}
