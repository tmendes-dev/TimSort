# TimSort in C#

![TimSort Logo](https://via.placeholder.com/150) *(Optional: Add your own logo here)*

## Table of Contents
1. [Introduction](#introduction)
2. [What is TimSort?](#what-is-timsort)
3. [Algorithm Overview](#algorithm-overview)
4. [Why Use TimSort?](#why-use-timsort)
5. [Features of this C# Version](#features-of-this-c-version)
6. [Credits](#credits)
7. [Contributing](#contributing)
8. [License](#license)

---

## Introduction

Welcome to the **C# implementation of TimSort**, one of the fastest and most efficient sorting algorithms used in the real world. This implementation is based on the original TimSort algorithm, which is a hybrid sorting algorithm derived from **Merge Sort** and **Insertion Sort**. It is highly optimized for real-world data and is used in Java’s `Arrays.sort()` and Python’s `sorted()` functions.

This repository provides a **robust and reliable version** of TimSort in C# with features like **logging** for tracking performance and sorting details. The project is designed to work efficiently with both small and large datasets, handling edge cases and providing detailed performance metrics using **Serilog** for logging.

---

## What is TimSort?

TimSort was invented by **Tim Peters** in 2002. It was specifically created to efficiently sort real-world data that often contain ordered subsequences, or "runs," which TimSort takes advantage of. The algorithm is a **stable**, **adaptive** sort that merges sorted runs and utilizes binary insertion sort for smaller chunks, achieving high performance.

TimSort is a **hybrid sorting algorithm** that combines:
- **Merge Sort**: Efficient for large datasets and ensures stable sorting.
- **Insertion Sort**: Optimal for small datasets, providing quick sorting for small subsequences.

TimSort is widely used in high-performance applications, including Python’s `sorted()` and Java's `Arrays.sort()`.

---

## Algorithm Overview

### TimSort Steps:
1. **Identify Runs**: The algorithm identifies naturally ordered subsequences (ascending or descending) called **runs**. If a run is descending, it reverses it into ascending order.
2. **Insertion Sort on Small Runs**: For smaller runs, it applies an optimized **Binary Insertion Sort**, which reduces comparisons.
3. **Merge Runs**: When runs are larger than a predefined minimum size, TimSort merges them in a way that minimizes comparisons and moves, using techniques like **galloping** to speed up merging.
4. **Galloping Mode**: This mode is activated when one run is significantly larger than another. It uses a binary search-like strategy to skip multiple elements and avoid unnecessary comparisons during the merge.

---

## Why Use TimSort?

### Advantages of TimSort:
- **Stable**: It retains the relative order of equal elements, making it perfect for sorting non-primitive types where equality may not imply the same object.
- **Adaptive**: TimSort takes advantage of partially ordered data, making it more efficient than traditional algorithms on real-world datasets.
- **Proven Performance**: It's used in major systems like Python and Java due to its **O(n log n)** worst-case time complexity and its ability to perform closer to **O(n)** for nearly sorted data.

---

## Features of this C# Version

This C# implementation of TimSort includes the following features:
- **Efficient Sorting**: Designed to sort arrays with minimal comparisons and memory usage, handling both small and large datasets.
- **Real-Time Logging**: Using **Serilog**, you can track sorting performance in real-time, logging both to the console and to a file.
- **Random or Custom Array Input**: Generate random arrays or input your own data for sorting.
- **Error Handling**: Robust error handling ensures that any issues are logged and handled gracefully.
- **Performance Tracking**: Logs the time taken to sort the array and verifies if the array is sorted correctly after execution.

---


## Credits
Tim Peters: The original inventor of the TimSort algorithm, which this project is based on. TimSort was introduced in Python 2.3 in 2002.

---


## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvements, please feel free to open an issue or submit a pull request.

- Fork the repository.
- Create a new branch for your feature or bugfix.
- Open a pull request once your changes are ready.
Please ensure all tests pass before submitting a pull request.

---


## License
This project is licensed under the MIT License. See the LICENSE file for more details.