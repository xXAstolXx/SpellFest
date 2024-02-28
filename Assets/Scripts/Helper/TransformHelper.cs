using UnityEngine;

public static class TransformHelper
{
    public static float DirectionToAngle(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        angle *= -1;
        if (angle < 0)
        {
            angle = 360 + angle;
        }
        angle += 90;
        if (angle > 360)
        {
            angle -= 360;
        }
        return angle;
    }
}
