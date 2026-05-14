using Fusion;
using UnityEngine;

public class Obstacle : NetworkBehaviour
{
    [Networked] public float Angle { get; private set; } = 0;
    [Networked] public Vector3 StartPosition { get; private set; } = Vector3.zero;

    public override void Spawned()
    {
        if (HasStateAuthority == false)
        {
            Runner.SetIsSimulated(Object, true);
            return;
        }
        Angle = Random.Range(0.0f, Mathf.PI * 2.0f);
        StartPosition = transform.position;
    }

    public override void FixedUpdateNetwork()
    {
        Angle += Runner.DeltaTime * 2.0f;
        if (Angle >= Mathf.PI * 2.0f)
        {
            Angle -= Mathf.PI * 2.0f;
        }
        Vector3 position = transform.position;
        transform.position = new Vector3(position.x, position.y, StartPosition.z + Mathf.Sin(Angle) * 4.0f);
    }
}