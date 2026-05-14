using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Networked] public bool CanMove { get; set; } = false;
    [SerializeField] private Transform weaponTransform = null;
    private SimpleKCC KCC;

    private float jumpForce = 8.0f;
    [Networked] public bool IsJumping { get; set; } = false;


    private void Awake()
    {
        KCC = GetComponent<SimpleKCC>();
    }

    public override void Spawned()
    {
        KCC.SetGravity(Physics.gravity.y * 2.0f);
    }

    private void Update()
    {
        if (!HasInputAuthority)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            Rpc_Shoot();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Rpc_Jump();
        }
    }

    private void LateUpdate()
    {
        if (!HasInputAuthority)
            return;
        Camera.main.transform.position = weaponTransform.position;
        Camera.main.transform.rotation = weaponTransform.rotation;
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
            Vector3 forward = yawRotation * Vector3.forward;
            Vector3 right = yawRotation * Vector3.right;
            Vector3 direction = Vector3.zero;
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
            if (KCC.IsGrounded && IsJumping)
            {
                currentJumpForce = jumpForce;
                IsJumping = false;
            }
            KCC.Move(direction * 6.0f, currentJumpForce);
            weaponTransform.rotation = Quaternion.Euler(lookRotation.y, lookRotation.x, 0.0f);
        }
        else
        {
            KCC.Move(Vector3.zero, 0.0f);
        }
        //float velocity = characterController.Velocity.magnitude / characterController.maxSpeed;
        //mecanimAnimator.Animator.SetFloat("Velocity", velocity);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    private void Rpc_Jump()
    {
        Rpc_RelayJump();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    private void Rpc_RelayJump()
    {
        IsJumping = true;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    private void Rpc_Shoot()
    {
        Rpc_RelayShoot();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    private void Rpc_RelayShoot()
    {
        // TODO: Shoot
    }
}