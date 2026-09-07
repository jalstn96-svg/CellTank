using UnityEngine;

public interface ITurret
{
    void Aim(Vector2 targetPosition);
    void TryFire();
}
