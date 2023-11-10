using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

/// <summary>
/// An interface that defines a binary operation.
/// The operation can be any binary operation (sum, difference, multiply, min, max, etc...).
/// </summary>
/// <typeparam name="T">The type of the values to perform the operation on.</typeparam>
public interface IBinaryOperator<T> where T : struct
{
    public T Operator(T a, T b);
}

[BurstCompile(CompileSynchronously = true)]
public struct ParallelReduceJob<T, U> : IJobParallelForBatch
    where T : struct
    where U : struct, IBinaryOperator<T>
{
    // The step rate of the source array.
    public int Step;
    [ReadOnly]
    public NativeSlice<T> Src;
    [WriteOnly]
    public NativeSlice<T> Dst;

    /// <summary>
    /// The operation to perform on the values of the array.
    /// </summary>
    public U Operator;

    /// <summary>
    /// Serial reduction.
    /// </summary>
    /// <param name="src">The source array to reduce.</param>
    /// <returns>The reduced value.</returns>
    public static T1 Reduce<T1, U1>(in NativeSlice<T1> src, int step, U1 op)
        where T1 : struct
        where U1 : struct, IBinaryOperator<T1>
    {
        T1 val = src[0];
        for (int i = step; i < src.Length; i += step)
        {
            val = op.Operator(val, src[i]);
        }
        return val;
    }

    public void Execute(int startIndex, int count)
    {
        Dst[startIndex] = Reduce(Src.Slice(startIndex, count), Step, Operator);
    }
}

public static class ParallelReduce
{
    /// <summary>
    /// Swap the arrays. This is very efficient for NativeArray as only the internal
    /// memory pointer is swapped, not the values of the array.
    /// </summary>
    /// <typeparam name="T">The value type of the arrays being swapped.</typeparam>
    /// <param name="a">The first array to swap with the second.</param>
    /// <param name="b">The second array to swap with the first.</param>
    private static void Swap<T>(ref NativeArray<T> a, ref NativeArray<T> b)
        where T : struct
    {
        (a, b) = (b, a);
    }

    /// <summary>
    /// Perform a parallel reduction on the elements of the array.
    /// </summary>
    /// <typeparam name="T">The type of the elements to be reduced.</typeparam>
    /// <typeparam name="U">The type of the binary operation to perform on each element of the array.</typeparam>
    /// <param name="values">The values to be reduced.</param>
    /// <param name="op">The operation to perform on the elements of the array.</param>
    /// <returns>The result of the reduction.</returns>
    public static T Reduce<T, U>(in NativeArray<T> values, U op)
        where T : struct
        where U : struct, IBinaryOperator<T>
    {
        // The number of values to reduce per thread batch.
        const int BATCH_SIZE = 1024;

        // The step rate for the reduction.
        // On the first iteration, this is every value of the source array.
        int stepRate = 1;

        // How many values to reduce in the current batch.
        int batchSize = BATCH_SIZE;

        JobHandle job = default;

        var src = new NativeArray<T>(values.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        var dst = new NativeArray<T>(values, Allocator.TempJob);

        while (stepRate < values.Length)
        {
            Swap(ref src, ref dst);

            job = new ParallelReduceJob<T, U>
            {
                Src = src,
                Dst = dst,
                Step = stepRate,
                Operator = op,
            }.ScheduleBatch(values.Length, batchSize, job);

            // Increment the step rate and batch size.
            stepRate = batchSize;
            batchSize *= 2;
        }

        job.Complete();

        T res = dst[0];

        src.Dispose();
        dst.Dispose();

        return res;
    }
}