
using UnityEngine;

public static class ResetHelper
{
    /// <summary>
    /// Resets the rigid body velocity
    /// </summary>
    /// <param name="rigidBody">Rigid body to reset</param>
    public static void ResetRigidBody(Rigidbody2D rigidBody)
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = 0.0f;
    }
}
