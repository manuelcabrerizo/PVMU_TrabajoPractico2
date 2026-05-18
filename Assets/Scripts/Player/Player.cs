using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private GameManager GameManager => ServiceProvider.Instance.GetService<GameManager>();
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

    [Networked] public bool CanMove { get; set; } = false;
    [Networked] public int Score { get; set; } = 0;

    [Networked] private bool isReviving { get; set; } = false;
    [Networked] private bool isDying { get; set; } = false;
    [Networked] private Vector3 spawnPosition { get; set; } = Vector3.zero;
    [Networked] private TickTimer shootTimer { get; set; }

    [SerializeField] private Transform weaponTransform = null;
    [SerializeField] private Uzi weapon = null;
    [SerializeField] private float jumpForce = 8.0f;

    private SimpleKCC KCC;
    private Health health;

    public override void Spawned()
    {
        health = GetComponent<Health>();
        KCC = GetComponent<SimpleKCC>();
        KCC.SetGravity(Physics.gravity.y * 2.0f);
        if (HasInputAuthority)
        {
            GameManager.LocalPlayer = this;
        }
    }

    public override void FixedUpdateNetwork() 
    {
        if (isReviving)
        {
            KCC.SetPosition(spawnPosition);
            isReviving = false;
        }

        if (isDying)
        {
            KCC.SetPosition(new Vector3(0.0f, -200.0f, 0.0f));
            isDying = false;
        }

        if (!CanMove || !health.IsAlive)
        {
            KCC.Move(Vector3.zero, 0.0f);
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

    private void LateUpdate()
    {
        if (!HasInputAuthority)
            return;
        Camera.main.transform.SetPositionAndRotation(weaponTransform.position, weaponTransform.rotation);
    }

    private void Fire()
    {
        List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
        float rayLength = float.MaxValue;
        PlayerRef playerRef = Object.InputAuthority;
        Runner.LagCompensation.RaycastAll(weaponTransform.position, weaponTransform.forward, rayLength, playerRef, hits);
        for (int i = 0; i < hits.Count; i++)
        {
            if (hits[i].Hitbox && hits[i].Hitbox.gameObject.layer == LayerMask.NameToLayer("PlayerHitbox"))
            {
                if (hits[i].Hitbox.Root.Object == Object) continue;
                Health health = hits[i].Hitbox.gameObject.GetComponentInParent<Health>();
                if (health.IsAlive && health.TakeDamage(5))
                {
                    health.GetComponent<Player>().isDying = true;
                    Score++;
                    if (HasStateAuthority)
                    {
                        Rpc_RaiseOnScoreChangeEvent(Score);
                    }
                }
                break;
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void Rpc_RevivePlayer()
    {
        spawnPosition = GameManager.GetRandomSpawnPosition();
        health.Cure();
        isReviving = true;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority, HostMode = RpcHostMode.SourceIsServer)]
    public void Rpc_RaiseOnScoreChangeEvent(int score)
    {
        EventBus.Raise<OnScoreChangeEvent>(score);
    }
}