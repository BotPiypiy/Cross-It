using UnityEngine;

public static class VectorHelper
{
    public static Vector3 VectorXYToVectorXZ(Vector2 swipeDirection)
    {

        Vector3 worldDirection = Vector3.zero;

        if (Mathf.Abs(swipeDirection.x) > 0.1f)
        {
            worldDirection.x = Mathf.Sign(swipeDirection.x);
        }

        if (Mathf.Abs(swipeDirection.y) > 0.1f)
        {
            worldDirection.z = Mathf.Sign(swipeDirection.y);
        }

        if (worldDirection.x != 0 && worldDirection.z != 0)
        {
            worldDirection.Normalize();
        }

        return worldDirection;
    }

    public static Vector2 ApplyAxisBias(Vector2 direction, float axisBias)
    {
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        if (absX > absY * axisBias)
        {
            return new Vector2(Mathf.Sign(direction.x), 0);
        }
        else if (absY > absX * axisBias)
        {
            return new Vector2(0, Mathf.Sign(direction.y));
        }

        return direction.normalized;
    }
}
