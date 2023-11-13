
using UnityEngine;

public static class ResetHelper
{
    public static void ResetRigidBody(Rigidbody2D rigidBody)
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = 0.0f;
    }
}
