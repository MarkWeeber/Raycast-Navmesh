using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private Vector3 _destination;
    [SerializeField]
    public Vector3 Destination { get => _destination; set => SetNewDestination(value); }
    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private Animator _animator;

    public Action<EnemyAI> OnKilled;

    private void SetNewDestination(Vector3 destination)
    {
        if (_agent != null)
        {
            _destination = destination;
            _agent.destination = _destination;
            _animator.Play("Alien_run_forward_anim");
        }
    }

    private void OnDestroy()
    {
        foreach (Action<EnemyAI> action in OnKilled.GetInvocationList()) // manually clear delegates
        {
            OnKilled -= action;
        }
    }
}
