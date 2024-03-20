using System;
using Unity.Mathematics;
using static Unity.Mathematics.math;

/// <summary>
/// Unity doesn't have a quaternion that can operate on double-precision values.
/// This struct implements a double-precision quaternion.
/// </summary>
[Serializable]
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
        var cosTheta = math.dot(a, b);
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

    /// <summary>
    /// Quaternion negation.
    /// </summary>
    /// <param name="a">The quaternion to negate.</param>
    /// <returns>The negated quaternion.</returns>
    public static QuaternionD operator -(QuaternionD a)
    {
        return new QuaternionD(-a.v, a.w);
    }

    /// <summary>
    /// Quaternion addition.
    /// </summary>
    /// <param name="a">The first quaternion.</param>
    /// <param name="b">The second quaternion.</param>
    /// <returns>The result of adding a and b.</returns>
    public static QuaternionD operator +(QuaternionD a, QuaternionD b)
    {
        return new QuaternionD(a.v + b.v, a.w + b.w);
    }

    /// <summary>
    /// Quaternion subtraction.
    /// </summary>
    /// <param name="a">The first quaternion.</param>
    /// <param name="b">The second quaternion.</param>
    /// <returns>The result of subtracting b from a.</returns>
    public static QuaternionD operator -(QuaternionD a, QuaternionD b)
    {
        return new QuaternionD(a.v - b.v, a.w - b.w);
    }

    /// <summary>
    /// Quaternion multiplication.
    /// </summary>
    /// <param name="a">The first quaternion.</param>
    /// <param name="b">The second quaternion.</param>
    /// <returns>The result of a * b.</returns>
    public static QuaternionD operator *(QuaternionD a, QuaternionD b)
    {
        return new QuaternionD(
            a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
            a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x,
            a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w,
            a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z
        );
    }

    /// <summary>
    /// Quaternion dot product. This is equivalent to vector dot product.
    /// </summary>
    /// <param name="a">The first quaternion.</param>
    /// <param name="b">The second quaternion.</param>
    /// <returns>The dot product between a and b.</returns>
    public static double dot(QuaternionD a, QuaternionD b)
    {
        return a.w * b.w + a.x * b.x + a.y * b.y + a.z * b.z;
    }

    /// <summary>
    /// Linear interpolation of two quaternions.
    /// </summary>
    /// <param name="a">The first quaternion.</param>
    /// <param name="b">The second quaternion.</param>
    /// <param name="t">The interpolation parameter.</param>
    /// <returns></returns>
    public static QuaternionD lerp(QuaternionD a, QuaternionD b, double t)
    {
        return a * (1.0 - t) + b * t;
    }

    /// <summary>
    /// Spherical linear interpolation.
    /// </summary>
    /// <param name="a">The starting quaternion.</param>
    /// <param name="b">The ending quaternion.</param>
    /// <param name="t">The interpolation parameter.</param>
    /// <returns>The interpolated quaternion.</returns>
    public static QuaternionD slerp(QuaternionD a, QuaternionD b, double t)
    {
        double c = dot(a, b); // Cosine angle between a and b.

        // If the cosine angle is close to 1, then the angle between a and b
        // is very close to 0. To avoid a division by 0 (when sin(a) = 0) then just
        // use linear interpolation.
        if (c > 1.0 - EPSILON_DBL)
        {
            return lerp(a, b, t);
        }

        // If the cosine angle between a and b is < 0, then negate a so that
        // the interpolation takes the shortest path from a to b.
        if (c < 0.0)
        {
            a = -a;
            c = -c;
        }

        double angle = acos(c); // The angle between a and b (in radians).
        return (a * sin((1.0 - t) * angle) + b * sin(t * angle)) / sin(angle);
    }

    /// <summary>
    /// Multiply a vector by a quaternion.
    /// </summary>
    /// <param name="q">The rotation quaternion.</param>
    /// <param name="v">The vector to rotate.</param>
    /// <returns>The rotated vector.</returns>
    public static double3 operator *(QuaternionD q, double3 v)
    {
        double3 t = 2.0 * cross(q.v, v);
        return v + q.w * t + cross(q.v, t);
    }

    /// <summary>
    /// Multiply a quaternion by a scalar.
    /// </summary>
    /// <param name="q">The quaternion.</param>
    /// <param name="s">The scalar.</param>
    /// <returns>The scaled quaternion.</returns>
    public static QuaternionD operator *(QuaternionD q, double s)
    {
        return new QuaternionD(q.v * s, q.w * s);
    }

    public static QuaternionD operator *(QuaternionD q, float s)
    {
        return q * (double)s;
    }

    /// <summary>
    /// Divide a quaternion by a scalar.
    /// </summary>
    /// <param name="q">The quaternion.</param>
    /// <param name="s">The scalar.</param>
    /// <returns>The scaled quaternion.</returns>
    public static QuaternionD operator /(QuaternionD q, double s)
    {
        return new QuaternionD(q.v / s, q.w / s);
    }

    public static QuaternionD operator /(QuaternionD q, float s)
    {
        return q / (double)s;
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
    /// Construct a quaternion from the vector part and scalar part.
    /// </summary>
    /// <param name="v">The vector part.</param>
    /// <param name="w">The scalar part.</param>
    public QuaternionD(double3 v, double w)
    {
        this.v = v;
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
