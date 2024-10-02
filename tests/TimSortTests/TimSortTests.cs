using Serilog;
using TimSort;
using Xunit.Abstractions;

namespace TimSortTests;

public class TimSortTests
{
    private readonly ITestOutputHelper _output;

    public TimSortTests(ITestOutputHelper output)
    {
        _output = output;

        // Configure Serilog to use xUnit's output helper
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.TestOutput(_output)
            .CreateLogger();
    }

    [Fact]
    public void Sort_SortsRandomArrayCorrectly()
    {
        // Arrange
        int[] array = [5, 1, 4, 2, 8, 0, -1, 7];
        TimSort<int> timSort = new(array, Comparer<int>.Default.Compare);

        // Act
        timSort.Sort();

        // Assert
        Assert.True(IsSorted(array));
    }

    [Fact]
    public void Sort_SortsAlreadySortedArray()
    {
        // Arrange
        int[] array = [-5, 0, 3, 7, 9, 12, 14];
        TimSort<int> timSort = new(array, Comparer<int>.Default.Compare);

        // Act
        timSort.Sort();

        // Assert
        Assert.True(IsSorted(array));
    }

    [Fact]
    public void Sort_SortsReversedArray()
    {
        // Arrange
        int[] array = [9, 7, 5, 3, 1, -2];
        TimSort<int> timSort = new(array, Comparer<int>.Default.Compare);

        // Act
        timSort.Sort();

        // Assert
        Assert.True(IsSorted(array));
    }

    [Fact]
    public void Sort_SortsEmptyArray()
    {
        // Arrange
        int[] array = [];
        TimSort<int> timSort = new(array, Comparer<int>.Default.Compare);

        // Act
        timSort.Sort();

        // Assert
        Assert.True(IsSorted(array));
    }

    [Fact]
    public void Sort_SortsSingleElementArray()
    {
        // Arrange
        int[] array = [42];
        TimSort<int> timSort = new(array, Comparer<int>.Default.Compare);

        // Act
        timSort.Sort();

        // Assert
        Assert.True(IsSorted(array));
    }

    [Fact]
    public void Sort_LargeArrayPerformance()
    {
        // Arrange
        int arraySize = 100000;
        var random = new Random();
        var array = Enumerable.Range(0, arraySize).Select(_ => random.Next(-100000, 100000)).ToArray();
        TimSort<int> timSort = new(array, Comparer<int>.Default.Compare);

        // Log the unsorted array
        Log.Information("Unsorted Array: {Array}", string.Join(", ", array));

        // Ensure array is initially unsorted
        Assert.False(IsSorted(array));

        // Act
        timSort.Sort();

        // Log the sorted array
        Log.Information("Sorted Array: {Array}", string.Join(", ", array));

        // Assert the array is sorted
        Assert.True(IsSorted(array));

        // Additional checks (example: first 100 and last 100 elements)
        Assert.True(IsSorted(array.Take(100).ToArray()));
        Assert.True(IsSorted(array.Skip(array.Length - 100).ToArray()));
    }


    public static bool IsSorted(int[] array)
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            if (array[i] > array[i + 1])
            {
                Log.Error("Array is not sorted at index {Index}. {Value1} > {Value2}", i, array[i], array[i + 1]);
                return false;
            }
        }
        return true;
    }
}