using Serilog;
using System.Diagnostics;

namespace TimSort
{
    /// <summary>
    /// The main program that demonstrates the TimSort algorithm.
    /// </summary>
    public static class Program
    {
        static void Main()
        {
            ConfigureLogging();
            try
            {
                Log.Information("Starting TimSort program.");
                DisplayWelcomeMessage();

                int arraySize = GetArraySize();
                int[] arrayToSort = GetArrayInput(arraySize);

                Log.Information("Array of size {ArraySize} prepared for sorting.", arrayToSort.Length);

                var stopwatch = Stopwatch.StartNew();

                TimSort<int> timSort = new(arrayToSort, Comparer<int>.Default.Compare);
                Log.Information("Starting the TimSort algorithm.");
                timSort.Sort();

                stopwatch.Stop();
                Log.Information("TimSort algorithm completed in {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);

                DisplaySortedResult(arrayToSort);

                Log.Information("Program ended.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred.");
                DisplayErrorMessage();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Configures the logging settings.
        /// </summary>
        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs\\timsort.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        /// <summary>
        /// Displays a welcome message to the user.
        /// </summary>
        private static void DisplayWelcomeMessage()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("********************************************");
            Console.WriteLine("**   Welcome to TimSort Sorting Program   **");
            Console.WriteLine("********************************************");
            Console.ResetColor();
        }

        /// <summary>
        /// Prompts the user for the size of the array.
        /// </summary>
        /// <returns>The array size inputted by the user.</returns>
        private static int GetArraySize()
        {
            int arraySize;
            Console.Write("Please enter the size of the array: ");
            while (!int.TryParse(Console.ReadLine(), out arraySize) || arraySize <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid input. Please enter a positive integer.");
                Console.ResetColor();
                Console.Write("Enter the array size: ");
            }
            return arraySize;
        }

        /// <summary>
        /// Prompts the user to choose between entering a custom array or generating a random one.
        /// </summary>
        /// <param name="arraySize">The size of the array.</param>
        /// <returns>An array inputted by the user or generated randomly.</returns>
        private static int[] GetArrayInput(int arraySize)
        {
            Console.Write("Would you like to enter a custom array or generate a random one? (Enter 'C' for custom, 'R' for random): ");
            var choice = Console.ReadLine()?.ToUpper();

            if (choice == "C")
            {
                return GetCustomArray(arraySize);
            }
            else
            {
                int lowerBound = GetIntegerInput("Enter the lower bound for random array values (-1000000 is default): ", -1000000);
                int upperBound = GetIntegerInput("Enter the upper bound for random array values (1000000 is default): ", 1000000);
                return GenerateRandomArray(arraySize, lowerBound, upperBound);
            }
        }

        /// <summary>
        /// Prompts the user to enter a custom array of the specified size.
        /// </summary>
        /// <param name="arraySize">The size of the array.</param>
        /// <returns>An array inputted by the user.</returns>
        private static int[] GetCustomArray(int arraySize)
        {
            var customArray = new int[arraySize];
            Console.WriteLine($"Please enter {arraySize} integers (separated by spaces or newlines):");
            for (int i = 0; i < arraySize; i++)
            {
                while (!int.TryParse(Console.ReadLine(), out customArray[i]))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input. Please enter an integer.");
                    Console.ResetColor();
                }
            }
            return customArray;
        }

        /// <summary>
        /// Generates a random array of the specified size with values between the given bounds.
        /// </summary>
        /// <param name="size">The size of the array.</param>
        /// <param name="minValue">The minimum value for random number generation.</param>
        /// <param name="maxValue">The maximum value for random number generation.</param>
        /// <returns>A randomly generated array.</returns>
        private static int[] GenerateRandomArray(int size, int minValue, int maxValue)
        {
            var random = new Random();
            return Enumerable.Range(0, size).Select(_ => random.Next(minValue, maxValue)).ToArray();
        }

        /// <summary>
        /// Displays the result of the sorting operation.
        /// </summary>
        /// <param name="arrayToSort">The sorted array.</param>
        private static void DisplaySortedResult(int[] arrayToSort)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Sorting completed successfully.");
            Console.WriteLine("First 100 elements of the sorted array:");
            Console.WriteLine(string.Join(", ", arrayToSort.Take(100)));

            bool sorted = IsSorted(arrayToSort);
            Console.WriteLine($"Array Sorted: {sorted}");
            Console.ResetColor();

            if (!sorted)
            {
                Log.Warning("The array was not sorted correctly.");
            }
        }

        /// <summary>
        /// Checks if the array is sorted in ascending order.
        /// </summary>
        /// <param name="array">The array to check.</param>
        /// <returns>True if the array is sorted, false otherwise.</returns>
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

        /// <summary>
        /// Prompts the user to enter an integer value, with optional default value.
        /// </summary>
        /// <param name="prompt">The prompt message for the user.</param>
        /// <param name="defaultValue">The default value if the user doesn't enter anything.</param>
        /// <returns>The integer entered by the user, or the default value.</returns>
        private static int GetIntegerInput(string prompt, int defaultValue)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int result))
            {
                return result;
            }
            Console.WriteLine($"Invalid input. Using default value: {defaultValue}");
            return defaultValue;
        }

        /// <summary>
        /// Displays an error message to the user if an exception occurs.
        /// </summary>
        private static void DisplayErrorMessage()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("An error occurred. Please check the log file for more details.");
            Console.ResetColor();
        }
    }
}
