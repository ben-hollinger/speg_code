using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleController : MonoBehaviour
{
    [Header("Grapple Settings")]
    [SerializeField] private float _grappleRange = 12f;
    [SerializeField] private float _pullSpeed = 10f;
    [SerializeField] private float _arrivalDistance = 1.5f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _enemyLayer;

    [Header("References")]
    [SerializeField] private Transform _grappleOrigin;
    [SerializeField] private LineRenderer _lineRenderer;

    public bool IsGrappling => _state != GrappleState.Idle;

    private enum GrappleState { Idle, Shooting, PullingToWall, PullingEnemyToPlayer }

    private GrappleState _state = GrappleState.Idle;
    private Vector3 _grappleTargetPoint;
    private EnemyController _grappledEnemy;

    private PlayerMovement _movement;
    private Animator _animator;

    private static readonly int GrappleShootTrigger = Animator.StringToHash("GrappleShoot");
    private static readonly int GrapplePullTrigger = Animator.StringToHash("GrapplePull");
    private static readonly int GrappleEndTrigger = Animator.StringToHash("GrappleEnd");

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (_state == GrappleState.Idle && Mouse.current.rightButton.wasPressedThisFrame)
            BeginGrappleShoot();

        UpdatePull();
        UpdateLineRenderer();
    }

    private void BeginGrappleShoot()
    {
        _state = GrappleState.Shooting;
        _movement.SetFrozen(true);
        if (_animator != null)
        {
            _animator.ResetTrigger(GrappleEndTrigger);
            _animator.ResetTrigger(GrapplePullTrigger);
            _animator.SetTrigger(GrappleShootTrigger);
        }
    }

    // Called by animation event
    public void FireGrapple()
    {
        if (_state != GrappleState.Shooting) return;

        Vector3 origin = _grappleOrigin != null ? _grappleOrigin.position : transform.position + Vector3.up;
        LayerMask combined = _groundLayer | _enemyLayer;

        if (!Physics.Raycast(origin, transform.forward, out RaycastHit hit, _grappleRange, combined))
        {
            CancelGrapple();
            return;
        }

        _grappleTargetPoint = hit.point;
        bool hitEnemy = (_enemyLayer.value & (1 << hit.collider.gameObject.layer)) != 0;

        if (hitEnemy)
        {
            _grappledEnemy = hit.collider.GetComponentInParent<EnemyController>();
            if (_grappledEnemy == null || _grappledEnemy.IsDead)
            {
                CancelGrapple();
                return;
            }
            _grappledEnemy.SetGrappleFrozen(true);
            _state = GrappleState.PullingEnemyToPlayer;
            // Hold Grapple Shoot anim
        }
        else
        {
            _state = GrappleState.PullingToWall;
            if (_animator != null)
            {
                _animator.ResetTrigger(GrappleShootTrigger);
                _animator.SetTrigger(GrapplePullTrigger);
            }
        }
    }

    private void UpdatePull()
    {
        if (_state == GrappleState.PullingToWall)
        {
            Vector3 dir = _grappleTargetPoint - transform.position;
            if (dir.magnitude <= _arrivalDistance)
            {
                FinishGrapple();
                return;
            }
            _movement.SetGrappleMotion(dir.normalized * _pullSpeed);
        }
        else if (_state == GrappleState.PullingEnemyToPlayer)
        {
            if (_grappledEnemy == null || _grappledEnemy.IsDead)
            {
                FinishGrapple();
                return;
            }

            Vector3 dir = transform.position - _grappledEnemy.transform.position;
            if (dir.magnitude <= _arrivalDistance)
            {
                FinishGrapple();
                return;
            }

            _grappledEnemy.transform.position += dir.normalized * _pullSpeed * Time.deltaTime;
            _grappleTargetPoint = _grappledEnemy.transform.position + Vector3.up;
        }
    }

    private void UpdateLineRenderer()
    {
        if (_lineRenderer == null) return;

        if (_state == GrappleState.Idle || _state == GrappleState.Shooting)
        {
            _lineRenderer.enabled = false;
            return;
        }

        _lineRenderer.enabled = true;
        Vector3 start = _grappleOrigin.position;
        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, _grappleTargetPoint);
    }

    private void FinishGrapple()
    {
        if (_grappledEnemy != null)
        {
            _grappledEnemy.SetGrappleFrozen(false);
            _grappledEnemy = null;
        }

        _movement.SetGrappleMotion(Vector3.zero);
        _movement.SetFrozen(false);
        _state = GrappleState.Idle;

        if (_lineRenderer != null) _lineRenderer.enabled = false;

        _animator.ResetTrigger(GrappleShootTrigger);
        _animator.ResetTrigger(GrapplePullTrigger);
        _animator.SetTrigger(GrappleEndTrigger);
    }

    private void CancelGrapple()
    {
        _movement.SetFrozen(false);
        _state = GrappleState.Idle;
        if (_animator != null)
        {
            _animator.ResetTrigger(GrappleShootTrigger);
            _animator.ResetTrigger(GrapplePullTrigger);
            _animator.SetTrigger(GrappleEndTrigger);
        }
    }
}
