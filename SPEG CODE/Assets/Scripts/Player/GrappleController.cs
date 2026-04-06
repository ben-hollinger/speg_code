using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleController : MonoBehaviour
{
    [Header("Grapple Settings")]
    [SerializeField] private float _grappleRange = 12f;
    [SerializeField] private float _pullSpeed = 10f;
    [SerializeField] private float _arrivalDistance = 1.5f;
    [SerializeField] private float _grappleTimeoutSeconds = 3f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private LayerMask _grappleObjectLayer;
    [SerializeField] private float _objectPullSpeed = 10f;

    [Header("References")]
    [SerializeField] private Transform _grappleOrigin;
    [SerializeField] private LineRenderer _lineRenderer;

    public bool IsGrappling => _state != GrappleState.Idle;

    private enum GrappleState { Idle, Shooting, PullingToWall, PullingEnemyToPlayer, PullingObjectToPlayer }

    private GrappleState _state = GrappleState.Idle;
    private Vector3 _grappleTargetPoint;
    private EnemyController _grappledEnemy;
    private Rigidbody _grappledBody;
    private float _grappleStartTime;

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
        if (!HatSelector.IsActiveAbility(HatSelector.AbilityType.Grapple))
        {
            UpdateLineRenderer();
            return;
        }

        if (_state == GrappleState.Idle && Mouse.current.rightButton.wasPressedThisFrame)
            BeginGrappleShoot();

        UpdateLineRenderer();
    }

    private void FixedUpdate()
    {
        UpdatePull();
    }

    private void BeginGrappleShoot()
    {
        _state = GrappleState.Shooting;
        _grappleStartTime = Time.time;
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
        LayerMask combined = _groundLayer | _enemyLayer | _grappleObjectLayer;

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
            bool hitGrappleLayer = (_grappleObjectLayer.value & (1 << hit.collider.gameObject.layer)) != 0;
            Rigidbody propBody = hit.collider.GetComponentInParent<Rigidbody>();
            if (hitGrappleLayer && propBody != null && !propBody.isKinematic)
            {
                _grappledBody = propBody;
                _grappleTargetPoint = propBody.worldCenterOfMass;
                _state = GrappleState.PullingObjectToPlayer;
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
    }

    private void UpdatePull()
    {
        if (_state != GrappleState.Idle && Time.time - _grappleStartTime >= _grappleTimeoutSeconds)
        {
            FinishGrapple();
            return;
        }

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

            _grappledEnemy.transform.position += dir.normalized * _pullSpeed * Time.fixedDeltaTime;
            _grappleTargetPoint = _grappledEnemy.transform.position + Vector3.up;
        }
        else if (_state == GrappleState.PullingObjectToPlayer)
        {
            if (_grappledBody == null)
            {
                FinishGrapple();
                return;
            }

            Vector3 flat = transform.position - _grappledBody.position;
            flat.y = 0f;
            if (flat.sqrMagnitude <= _arrivalDistance * _arrivalDistance)
            {
                FinishGrapple();
                return;
            }

            Vector3 dir = flat.normalized;
            Vector3 v = _grappledBody.linearVelocity;
            v.x = dir.x * _objectPullSpeed;
            v.z = dir.z * _objectPullSpeed;
            _grappledBody.linearVelocity = v;
            _grappleTargetPoint = _grappledBody.worldCenterOfMass;
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

        _grappledBody = null;

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
