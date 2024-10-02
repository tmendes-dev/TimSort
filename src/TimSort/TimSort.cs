
namespace TimSort;

/// <summary>
/// Represents an implementation of the TimSort sorting algorithm.
/// </summary>
/// <typeparam name="T">The type of elements in the array to be sorted.</typeparam>
public sealed class TimSort<T>
{
    private const int DEFAULT_MIN_MERGE = 32;
    private const int DEFAULT_TMP_STORAGE_LENGTH = 256;

    private readonly T[] array;
    private readonly Comparison<T> compare;
    private T[] tmp;
    private int stackSize = 0;
    private readonly int[] runStart;
    private readonly int[] runLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimSort{T}"/> class.
    /// </summary>
    /// <param name="array">The array to be sorted.</param>
    /// <param name="compare">The comparison delegate used for sorting.</param>
    public TimSort(T[] array, Comparison<T> compare)
    {
        this.array = array;
        this.compare = compare;
        int len = array.Length;
        this.tmp = new T[Math.Min(len >> 1, DEFAULT_TMP_STORAGE_LENGTH)];
        this.runStart = new int[40]; // Stack size
        this.runLength = new int[40];
    }

    /// <summary>
    /// Sorts the specified array using the TimSort algorithm.
    /// </summary>
    public void Sort()
    {
        int remaining = array.Length;
        if (remaining < 2) return;

        if (remaining < DEFAULT_MIN_MERGE)
        {
            int runLen = MakeAscendingRun(0, remaining);
            BinaryInsertionSort(array, 0, remaining, runLen, compare);
            return;
        }

        int minRun = MinRunLength(remaining);
        int lo = 0;

        do
        {
            int runLen = MakeAscendingRun(lo, remaining);
            if (runLen < minRun)
            {
                int force = Math.Min(remaining, minRun);
                BinaryInsertionSort(array, lo, lo + force, lo + runLen, compare);
                runLen = force;
            }

            PushRun(lo, runLen);
            MergeRuns();

            remaining -= runLen;
            lo += runLen;
        } while (remaining > 0);

        ForceMergeRuns();
    }

    /// <summary>
    /// Determines the minimum run length for the specified number of elements.
    /// </summary>
    /// <param name="n">The number of elements.</param>
    /// <returns>The minimum run length.</returns>
    private static int MinRunLength(int n)
    {
        int r = 0;
        while (n >= DEFAULT_MIN_MERGE)
        {
            r |= (n & 1);
            n >>= 1;
        }
        return n + r;
    }

    /// <summary>
    /// Performs a binary insertion sort on the specified sub-array.
    /// </summary>
    /// <param name="array">The array to sort.</param>
    /// <param name="lo">The starting index of the sub-array.</param>
    /// <param name="hi">The ending index of the sub-array.</param>
    /// <param name="start">The starting index for insertion.</param>
    /// <param name="compare">The comparison delegate used for sorting.</param>
    private static void BinaryInsertionSort(T[] array, int lo, int hi, int start, Comparison<T> compare)
    {
        if (start == lo) start++;
        for (; start < hi; start++)
        {
            T pivot = array[start];
            int left = lo;
            int right = start;

            while (left < right)
            {
                int mid = (left + right) >> 1;
                if (compare(pivot, array[mid]) < 0)
                    right = mid;
                else
                    left = mid + 1;
            }

            Array.Copy(array, left, array, left + 1, start - left);
            array[left] = pivot;
        }
    }

    /// <summary>
    /// Creates an ascending run in the specified sub-array.
    /// </summary>
    /// <param name="lo">The starting index of the sub-array.</param>
    /// <param name="hi">The ending index of the sub-array.</param>
    /// <returns>The length of the run.</returns>
    private int MakeAscendingRun(int lo, int hi)
    {
        int runHi = lo + 1;
        if (runHi == hi) return 1;

        if (compare(array[runHi++], array[lo]) < 0)
        {
            while (runHi < hi && compare(array[runHi], array[runHi - 1]) < 0) runHi++;
            Array.Reverse(array, lo, runHi - lo);
        }
        else
        {
            while (runHi < hi && compare(array[runHi], array[runHi - 1]) >= 0) runHi++;
        }
        return runHi - lo;
    }

    /// <summary>
    /// Pushes the specified run onto the run stack.
    /// </summary>
    /// <param name="runStart">The start index of the run.</param>
    /// <param name="runLength">The length of the run.</param>
    private void PushRun(int runStart, int runLength)
    {
        this.runStart[stackSize] = runStart;
        this.runLength[stackSize] = runLength;
        stackSize++;
    }

    /// <summary>
    /// Merges runs in the run stack.
    /// </summary>
    private void MergeRuns()
    {
        while (stackSize > 1)
        {
            int n = stackSize - 2;
            if (n >= 1 && runLength[n - 1] <= runLength[n] + runLength[n + 1])
            {
                if (runLength[n - 1] < runLength[n + 1]) n--;
            }
            else if (runLength[n] > runLength[n + 1]) break;
            MergeAt(n);
        }
    }

    /// <summary>
    /// Forces the merging of runs in the run stack until only one remains.
    /// </summary>
    private void ForceMergeRuns()
    {
        while (stackSize > 1)
        {
            int n = stackSize - 2;
            if (n > 0 && runLength[n - 1] < runLength[n + 1]) n--;
            MergeAt(n);
        }
    }

    /// <summary>
    /// Merges the two runs at the specified index.
    /// </summary>
    /// <param name="i">The index of the runs to merge.</param>
    private void MergeAt(int i)
    {
        int start1 = runStart[i];
        int length1 = runLength[i];
        int start2 = runStart[i + 1];
        int length2 = runLength[i + 1];

        runLength[i] = length1 + length2;

        if (i == stackSize - 3)
        {
            runStart[i + 1] = runStart[i + 2];
            runLength[i + 1] = runLength[i + 2];
        }
        stackSize--;

        // Merge the runs
        if (length1 == 0 || length2 == 0) return;

        if (compare(array[start1 + length1 - 1], array[start2]) <= 0)
        {
            return; // Already in order
        }

        // Choose whether to merge low or high based on the first elements
        if (compare(array[start1], array[start2 + length2 - 1]) <= 0)
        {
            MergeLow(start1, length1, start2, length2);
        }
        else
        {
            MergeHigh(start1, length1, start2, length2);
        }
    }

    /// <summary>
    /// Merges the two runs where the first run is in the original array and the second run is in a temporary array.
    /// </summary>
    /// <param name="start1">The starting index of the first run.</param>
    /// <param name="length1">The length of the first run.</param>
    /// <param name="start2">The starting index of the second run.</param>
    /// <param name="length2">The length of the second run.</param>
    private void MergeLow(int start1, int length1, int start2, int length2)
    {
        if (length1 <= 0) return; // No elements to merge

        if (tmp.Length < length1)
            Array.Resize(ref tmp, length1);

        Array.Copy(array, start1, tmp, 0, length1);

        int cursor1 = 0; // First run (copied to tmp)
        int cursor2 = start2; // Second run (in original array)
        int dest = start1; // Destination in the original array


        while (cursor1 < length1 && cursor2 < start2 + length2)
        {
            if (compare(tmp[cursor1], array[cursor2]) <= 0)
            {
                array[dest++] = tmp[cursor1++];
            }
            else
            {
                array[dest++] = array[cursor2++];
            }
        }

        // Copy any remaining elements from tmp (first run) back to the original array
        if (cursor1 < length1)
            Array.Copy(tmp, cursor1, array, dest, length1 - cursor1);
    }

    /// <summary>
    /// Merges the two runs where the second run is in a temporary array and the first run is in the original array.
    /// </summary>
    /// <param name="start1">The starting index of the first run.</param>
    /// <param name="length1">The length of the first run.</param>
    /// <param name="start2">The starting index of the second run.</param>
    /// <param name="length2">The length of the second run.</param>
    private void MergeHigh(int start1, int length1, int start2, int length2)
    {
        if (length2 <= 0) return; // No elements to merge

        if (tmp.Length < length2)
            Array.Resize(ref tmp, length2);

        // Copy the second run to the temporary array
        Array.Copy(array, start2, tmp, 0, length2);

        int cursor1 = start1 + length1 - 1;  // Last element in the first run
        int cursor2 = length2 - 1;           // Last element in the second run (in tmp)
        int dest = start2 + length2 - 1;     // Position to place in the original array

        // Merge the two runs back into the main array from back to front
        while (cursor1 >= start1 && cursor2 >= 0)
        {
            if (compare(array[cursor1], tmp[cursor2]) > 0)
            {
                array[dest--] = array[cursor1--];  // Take from the first run
            }
            else
            {
                array[dest--] = tmp[cursor2--];    // Take from the second run (tmp)
            }
        }

        // Copy remaining elements from the second run (tmp) if cursor2 has not reached zero
        if (cursor2 >= 0)
            Array.Copy(tmp, 0, array, start1, cursor2 + 1);
    }
}
