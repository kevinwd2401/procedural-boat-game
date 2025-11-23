
using UnityEngine;
using System.Linq;
using Random = System.Random;

public static class Math
{
    public static Vector3 CalculateElevation(Vector3 origin, Vector3 destination, float gravity, float initialSpeed) {
        Vector3 toTarget = destination - origin;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        float xz = toTargetXZ.magnitude;    // horizontal distance
        float y = toTarget.y;                // vertical displacement

        // Guard: zero speed
        if (initialSpeed <= 0f)
            return Vector3.zero;

        float g = gravity;

        float v2 = initialSpeed * initialSpeed;
        float v4 = v2 * v2;

        // Discriminant of quadratic for tan(theta) formula
        float insideSqrt = v4 - g * (g * xz * xz + 2f * y * v2);

        if (insideSqrt < 0f)
        {
            // No solution at this speed
            return Vector3.zero;
        }

        float sqrt = Mathf.Sqrt(insideSqrt);

        // Low trajectory
        float tanTheta = (v2 - sqrt) / (g * xz);
        float angle = Mathf.Atan(tanTheta);

        // Build direction: horizontal * cos(angle) + up * sin(angle)
        Vector3 horizontalDir = toTargetXZ.normalized;
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        Vector3 launchDir = horizontalDir * cos + Vector3.up * sin;
        return launchDir.normalized;
    }

    public static int[] randomizeSpecialArray() {
        Random rnd = new Random();
        int[] numbers = new[] { 0, 1, 2, 3, 4, 5 };
        return numbers.OrderBy(x => rnd.Next()).ToArray();
    }
}
