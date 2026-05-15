using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Networked] public bool CanMove { get; set; } = false;
    [SerializeField] private Transform weaponTransform = null;
    // TODO: Make this a scriptable object? pickables?
    [SerializeField] private Uzi weapon = null;
    [SerializeField] private LayerMask hitMask;

    private SimpleKCC KCC;
    private float jumpForce = 8.0f;
    [Networked] private TickTimer shootTimer { get; set; }

    public override void Spawned()
    {
        KCC = GetComponent<SimpleKCC>();
        KCC.SetGravity(Physics.gravity.y * 2.0f);
    }

    private void Update()
    {
        if (!HasInputAuthority)
            return;
    }

    private void LateUpdate()
    {
        if (!HasInputAuthority)
            return;
        Camera.main.transform.SetPositionAndRotation(weaponTransform.position, weaponTransform.rotation);
    }

    public override void FixedUpdateNetwork() 
    {
        if (!CanMove)
        {
            return;
        }
        if (GetInput(out NetworkInputData data))
        {
            Vector2 lookRotation = data.GetLookRotation();
            Quaternion yawRotation = Quaternion.Euler(0.0f, lookRotation.x, 0.0f);
            Quaternion pitchRotation = Quaternion.Euler(lookRotation.y, 0.0f, 0.0f);
            Vector3 forward = yawRotation * Vector3.forward;
            Vector3 right = yawRotation * Vector3.right;
            Vector3 direction = Vector3.zero;

            if (data.IsAction(InputAction.Shoot) && shootTimer.ExpiredOrNotRunning(Runner))
            {
                Fire();
                shootTimer = TickTimer.CreateFromSeconds(Runner, weapon.RateOfFire);
            }
            if (data.IsAction(InputAction.MoveForward))
            {
                direction += forward;
            }
            if (data.IsAction(InputAction.MoveBackward))
            {
                direction -= forward;
            }
            if (data.IsAction(InputAction.MoveLeft))
            {
                direction -= right;
            }
            if (data.IsAction(InputAction.MoveRight))
            {
                direction += right;
            }
            if (direction.sqrMagnitude > 0.0f)
            {
                direction.Normalize();
            }
            float currentJumpForce = 0.0f;
            if (KCC.IsGrounded && data.IsAction(InputAction.Jump))
            {
                currentJumpForce = jumpForce;
            }
            KCC.Move(direction * 6.0f, currentJumpForce);
            KCC.SetLookRotation(0.0f, lookRotation.x);
            weaponTransform.localRotation = pitchRotation;
        }
        else
        {
            KCC.Move(Vector3.zero, 0.0f);
        }
        //float velocity = characterController.Velocity.magnitude / characterController.maxSpeed;
        //mecanimAnimator.Animator.SetFloat("Velocity", velocity);
    }

    private void Fire()
    {
        List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
        float rayLength = float.MaxValue;
        PlayerRef playerRef = Object.InputAuthority;
        Runner.LagCompensation.RaycastAll(weaponTransform.position, weaponTransform.forward, rayLength, playerRef, hits);
        for (int i = 0; i < hits.Count; i++)
        {
            if (hits[i].Hitbox && hits[i].Hitbox.gameObject.layer == hitMask.value)
            {
                Health health = hits[i].Hitbox.gameObject.GetComponentInParent<Health>();
                health.TakeDamage(5);
                break;
            }
        }
    }
}