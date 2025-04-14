using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// base enemy AI management class
/// has a simple state machine
/// controls animator
/// controls navmesh agent
/// once killed calls all delegates on action OnKilled
/// once reached destination calls all delegates on action OnDestinationReached
/// for visuals spawns particle effects when killed or reached destination
/// </summary>
public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Running,
        Hiding,
        Dead,
        DestinationReached,
        Idle
    }
    private EnemyState _state;
    public EnemyState State { get => _state; set => SetState(value); }
    private Vector3 _destination;
    public Vector3 Destination { get => _destination; set => SetNewDestination(value); } // everytime state is changed outside, the FSM function will be executed
    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private ParticleSystem _particlesWhenKilled;
    [SerializeField]
    private ParticleSystem _particlesWhenReachedDestination;
    [SerializeField]
    private Vector3 _particleSystemSpawnOffset = Vector3.up;
    public Action<EnemyAI> OnKilled;
    public Action<EnemyAI> OnDestinationReached;

    private GameObject _instantiatedObject;
    private IEnumerator _resetStateRoutine;

    private void Start()
    {
        _resetStateRoutine = ResetStateRoutine(EnemyState.Running, 0f);
        GameManager.Instance.OnGameLose += OnGameEnd;
        GameManager.Instance.OnGameWin += OnGameEnd;
    }
    private void OnDestroy()
    {
        GameManager.Instance.OnGameLose -= OnGameEnd;
        GameManager.Instance.OnGameWin -= OnGameEnd;
        // manually clearing delegates
        if (OnKilled != null)
        {
            foreach (var action in OnKilled.GetInvocationList())
            {
                OnKilled -= (Action<EnemyAI>)action;
            }
        }
        if (OnDestinationReached != null)
        {
            foreach (var action in OnDestinationReached.GetInvocationList())
            {
                OnDestinationReached -= (Action<EnemyAI>)action;
            }
        }
    }

    private void SetNewDestination(Vector3 destination)
    {
        _destination = destination;
        if (_agent != null)
        {
            _agent.destination = _destination;
            SetState(EnemyState.Running);
        }
    }

    private void SetState(EnemyState newState) // main switch for FSM
    {
        _state = newState;
        if (!enabled)
        {
            _agent.isStopped = true;
            return;
        }
        switch (newState)
        {
            case EnemyState.Running:
                _agent.isStopped = false;
                _animator.Play("Alien_run_forward_anim");
                break;
            case EnemyState.Hiding:
                _agent.isStopped = true;
                _animator.Play("Alien_crouch_idle_anim");
                break;
            case EnemyState.Dead:
                _agent.isStopped = true;
                _animator.Play("Alien_death_anim");
                SpawnParticlesAtPosition(_particlesWhenKilled);
                AudioManager.Instance.PlayEnemyKilledSound(transform.position);
                StopCoroutine(_resetStateRoutine);
                GameManager.Instance.Score++;
                OnKilled?.Invoke(this);
                break;
            case EnemyState.DestinationReached:
                _particlesWhenReachedDestination.Play();
                SpawnParticlesAtPosition(_particlesWhenReachedDestination);
                _agent.isStopped = true;
                AudioManager.Instance.PlayEnemyEscapedSound(transform.position);
                GameManager.Instance.EnemiesEscaped++;
                StopCoroutine(_resetStateRoutine);
                OnDestinationReached?.Invoke(this);
                break;
            case EnemyState.Idle:
                _agent.isStopped = true;
                _animator.Play("Alien_idle_anim");
                break;
            default:
                break;
        }
    }

    public void SetStateForDuration(EnemyState newState, float duration)
    {
        
        StopCoroutine(_resetStateRoutine);
        _resetStateRoutine = ResetStateRoutine(_state, duration);
        SetState(newState);
        StartCoroutine(_resetStateRoutine);
    }

    public void WarpAgent(Vector3 position)
    {
        _agent.Warp(position);
    }

    IEnumerator ResetStateRoutine(EnemyState initialState, float duration)
    {
        yield return new WaitForSeconds(duration);
        SetState(initialState);
    }
    
    private void SpawnParticlesAtPosition(ParticleSystem particleSystem)
    {
        _instantiatedObject = Instantiate(particleSystem.gameObject, transform.position + _particleSystemSpawnOffset, Quaternion.identity);
        Destroy(_instantiatedObject, 2f);
    }

    private void OnGameEnd()
    {
        SetState(EnemyState.Idle);
        _agent.isStopped = true;
        enabled = false;
    }
}
