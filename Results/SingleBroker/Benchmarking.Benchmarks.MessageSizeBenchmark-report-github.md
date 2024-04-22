```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3447/23H2/2023Update/SunValley3)
13th Gen Intel Core i7-13700H, 1 CPU, 20 logical and 14 physical cores
.NET SDK 8.0.203
  [Host]     : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2
  Job-AXCGAS : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

```

| Method                  | MessageSize   |           Mean |          Error |         StdDev |         Median |
|-------------------------|---------------|---------------:|---------------:|---------------:|---------------:|
| **MessageWaitForReply** | **512**       |  **48.365 ms** |  **0.9442 ms** |  **1.5246 ms** |  **48.535 ms** |
| **MessageWaitForReply** | **1024**      |  **48.001 ms** |  **1.7431 ms** |  **4.9730 ms** |  **48.688 ms** |
| **MessageWaitForReply** | **524288**    |   **5.110 ms** |  **0.1322 ms** |  **0.3837 ms** |   **5.033 ms** |
| **MessageWaitForReply** | **1048576**   |   **8.691 ms** |  **0.1726 ms** |  **0.3895 ms** |   **8.672 ms** |
| **MessageWaitForReply** | **8388608**   |  **66.970 ms** |  **2.3279 ms** |  **6.7905 ms** |  **66.693 ms** |
| **MessageWaitForReply** | **33554432**  | **275.684 ms** | **14.7459 ms** | **43.2472 ms** | **259.644 ms** |
| **MessageWaitForReply** | **67108864**  | **496.370 ms** |  **9.7704 ms** | **22.6444 ms** | **497.373 ms** |
| **MessageWaitForReply** | **134217728** | **994.215 ms** | **19.7508 ms** | **41.2272 ms** | **993.790 ms** |
