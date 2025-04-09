using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy AI spawn manager
// Has object pool to stire enemies
// once enemy killed respawns it after some time
// no enemy object is destroyed
public class EnemyAISpawnManager : SingletonBehaviour<EnemyAISpawnManager>
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private int _enemyAmount = 20;
    [SerializeField] private float _enemySpawnTimeAfterkilled = 5f;
    [SerializeField] private float _startSpawnDelay = 3f;
    [SerializeField] private Transform _targetDestination;

    private List<EnemyAI> _pool;
    private int _currentPoolIndex;
    private EnemyAI _enemyAI;
    private GameObject _spawnedObject;

    private void Start()
    {
        _pool = new List<EnemyAI>();
        InstantiateEnemyPool();
        Invoke(nameof(SpawnEnemiesOnStart), _startSpawnDelay);
    }

    private void InstantiateEnemyPool() // fill the pool
    {
        for (int i = 0; i < _enemyAmount; i++)
        {
            _spawnedObject = Instantiate(_enemyPrefab, transform.position, Quaternion.identity, transform);
            if (_spawnedObject.TryGetComponent<EnemyAI>(out _enemyAI))
            {
                _enemyAI.OnKilled += OnEnemyKilled; // assign function to on killed action
                _enemyAI.gameObject.SetActive(false);
                _pool.Add(_enemyAI);
            }
        }
    }

    private void SpawnEnemiesOnStart()
    {
        if (_currentPoolIndex < _pool.Count) // yet to spawn from the pool
        {
            Spawn(_pool[_currentPoolIndex]);
            if (_currentPoolIndex < _pool.Count - 1) // still yet to spawn from the pool
            {
                Invoke(nameof(SpawnEnemiesOnStart), _startSpawnDelay); // repeat self after given time    
            }
            _currentPoolIndex++;
        }
    }

    private void OnEnemyKilled(EnemyAI enemyAI)
    {
        StartCoroutine(SpawnDelayedRoutine(enemyAI, _enemySpawnTimeAfterkilled));
    }

    IEnumerator SpawnDelayedRoutine(EnemyAI enemyAI, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Spawn(enemyAI);
    }

    private void Spawn(EnemyAI enemyAI)
    {
        enemyAI.gameObject.SetActive(true);
        enemyAI.transform.position = transform.position;
        enemyAI.Destination = _targetDestination.position;
    }    
}
