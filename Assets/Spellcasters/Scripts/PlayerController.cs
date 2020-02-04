using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// TODO 
// Write action simulator: At any given time, character will either:
// face target, move to target, attack target or move freely
//
// Write a system that simulates the execution of following actions
// necessary to perform the actions that are dependent on others. For example,
// if player wants to attack enemy character, and it is not facing it and is out of range, 
// first it needs to face the target, then get in range and then attack.

public abstract class Character : MonoBehaviour
{
    public void FaceTarget(Vector3 target)
    {
        // TODO
    }

    public void RangeAttack(Vector3 target, GameObject projectile)
    {
        // TODO
    }

    public void MeleeAttack(Vector3 target)
    {
        // TODO
    }
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : Character
{
    [Header("References")]
    [SerializeField]
    private GameObject _spellDefault;
    [SerializeField]
    private Transform _spellCastPoint;

    private NavMeshAgent _agent;
    private Animator _animator;

    // Start is called before the first frame update
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = true;

        _animator = GetComponent<Animator>();
    }

    private bool _isMoving;
    private Vector3 _targetDest;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, 1 << LayerMask.NameToLayer("Walkable")))
            {
                // If agent was idle
                if(!_isMoving)
                {
                    _animator.SetTrigger("move");
                    _isMoving = true;
                }

                _agent.SetDestination(hit.point);
                _targetDest = hit.point;

                //transform.rotation = Quaternion.LookRotation(hit.point - transform.position);
            }
        }

        // Shoot
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                SpellCast(hit.point - transform.position);
            }
        }

        float dist = Vector3.Distance(transform.position, _targetDest);
        Debug.Log(dist);

        if (_isMoving && dist < _agent.stoppingDistance)
        {
            _animator.SetTrigger("stop");
            _isMoving = false;
        }

        //if (!_agent.pathPending)
        //{
        //    // Check distance
        //    if (_agent.remainingDistance != Mathf.Infinity && _agent.remainingDistance <= _agent.stoppingDistance) //_agent.stoppingDistance)
        //    {
        //        if (_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
        //        {
        //            // Reached, stop animating
        //            _animator.SetTrigger("stop");
        //            _isMoving = false;
        //        }
        //    }
        //}

    }

    private void SpellCast(Vector3 targetDir)
    {
        GameObject instance = Instantiate(_spellDefault, _spellCastPoint.position, Quaternion.identity);
        instance.GetComponent<Rigidbody>().AddForce(targetDir.normalized * 10, ForceMode.Impulse);
    }
}
