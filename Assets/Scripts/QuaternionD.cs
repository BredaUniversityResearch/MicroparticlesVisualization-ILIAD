using Unity.Mathematics;
using static Unity.Mathematics.math;

/// <summary>
/// Unity doesn't have a quaternion that can operate on double-precision values.
/// This struct implements a double-precision quaternion.
/// </summary>
public struct QuaternionD
{
    /// <summary>
    /// Vector part.
    /// </summary>
    public double3 v;

    /// <summary>
    /// Scalar part.
    /// </summary>
    public double w;

    public double x
    {
        get => v.x;
        set => v.x = value;
    }

    public double y
    {
        get => v.y;
        set => v.y = value;
    }

    public double z
    {
        get => v.z;
        set => v.z = value;
    }

    public static readonly QuaternionD identity = new QuaternionD(0.0, 0.0, 0.0, 1.0);
    public static readonly QuaternionD zero = new QuaternionD(0, 0, 0, 0);

    /// <summary>
    /// Construct a quaternion from an axis of rotation and an angle (in radians).
    /// </summary>
    /// <param name="axis">The axis of rotation.</param>
    /// <param name="angle">The rotation angle (in radians).</param>
    /// <returns>A quaternion that represents a rotation about an axis.</returns>
    public static QuaternionD AxisAngle(double3 axis, double angle)
    {
        sincos(0.5 * angle, out var s, out var c);
        return new QuaternionD(double4(axis * s, c));
    }

    /// <summary>
    /// Construct a quaternion formed by two vectors.
    /// Source: https://github.com/g-truc/glm/blob/ab913bbdd0bd10462114a17bcb65cf5a368c1f32/glm/gtx/quaternion.inl#L121-L158
    /// </summary>
    /// <param name="a">The first vector (normalized).</param>
    /// <param name="b">The second vector (normalized).</param>
    public static QuaternionD FromVectors(double3 a, double3 b)
    {
        var cosTheta = dot(a, b);
        double3 rotationAxis;

        if (cosTheta > 1.0 - EPSILON_DBL)
        {
            // The angle between the two vectors is almost 0
            return identity;
        }

        if (cosTheta < -1.0 + EPSILON_DBL)
        {
            // Vectors are pointing in opposite directions
            // In this case, we choose an (arbitrary) axis of rotation
            // that is perpendicular to a and the Y-axis.
            rotationAxis = cross(double3(0, 1, 0), a);
            if (lengthsq(rotationAxis) < EPSILON_DBL)
            {
                // a is parallel to Y... Choose another (arbitrary) axis.
                rotationAxis = cross(double3(1, 0, 0), a);
            }

            rotationAxis = normalize(rotationAxis);
            return AxisAngle(float3(rotationAxis), PI);
        }

        // Implementation from Stan Melax's Game Programming Gems 1 article
        rotationAxis = cross(a, b);

        var s = sqrt((1.0 + cosTheta) * 2.0);
        var invs = 1.0 / s;

        return new QuaternionD(double4(rotationAxis * invs, s * 0.5));
    }

    public static explicit operator QuaternionD(double4 v)
    {
        return new QuaternionD(v);
    }

    public static double3 operator *(QuaternionD q, double3 v)
    {
        double3 t = 2.0 * cross(q.v, v);
        return v + q.w * t + cross(q.v, t);
    }

    /// <summary>
    /// Rotate a vector by a quaternion.
    /// </summary>
    /// <param name="q">The rotation quaternion.</param>
    /// <param name="v">The vector to rotate.</param>
    /// <returns></returns>
    public static double3 rotate(QuaternionD q, double3 v)
    {
        return q * v;
    }

    /// <summary>
    /// Construct a quaternion from (x, y, z, w) components.
    /// </summary>
    /// <param name="x">The x value of the vector part.</param>
    /// <param name="y">The y value of the vector part.</param>
    /// <param name="z">The z value of the vector part.</param>
    /// <param name="w">The scalar part.</param>
    public QuaternionD(double x, double y, double z, double w)
    {
        this.v = double3(x, y, z);
        this.w = w;
    }

    /// <summary>
    /// Construct a quaternion from (x, y, z, w) components.
    /// </summary>
    /// <param name="v">The vector to use to construct this quaternion.</param>
    public QuaternionD(double4 v)
    {
        this.v = v.xyz;
        w = v.w;
    }

    /// <summary>
    /// Construct a quaternion formed by two vectors.
    /// Source: https://github.com/g-truc/glm/blob/ab913bbdd0bd10462114a17bcb65cf5a368c1f32/glm/gtx/quaternion.inl#L121-L158
    /// </summary>
    /// <param name="a">The first vector (normalized)</param>
    /// <param name="b">The second vector (normalized)</param>
    public QuaternionD(double3 a, double3 b)
    {
        this = FromVectors(a, b);
    }


}
