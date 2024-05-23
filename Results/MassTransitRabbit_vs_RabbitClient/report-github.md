# MassTransitRabbit vs RabbitMQClient in .NET

<details>

<summary>Testing Environment Details</summary>

```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
13th Gen Intel Core i7-13700H, 1 CPU, 20 logical and 14 physical cores
.NET SDK 8.0.300
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  Job-GJHEED : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

```

</details>
| Method       | MessageSize | Mean        | Error       | StdDev      | Median      | Min         | Max         | Ratio | RatioSD |
|------------- |------------ |------------:|------------:|------------:|------------:|------------:|------------:|------:|--------:|
| RabbitClient | 102400      |    158.9 μs |     8.29 μs |    24.31 μs |    157.1 μs |    119.8 μs |    213.8 μs |  1.00 |    0.00 |
| MassTransit  | 102400      |  1,331.2 μs |    48.29 μs |   126.37 μs |  1,319.1 μs |  1,125.7 μs |  1,707.5 μs |  8.57 |    1.62 |
|              |             |             |             |             |             |             |             |       |         |
| RabbitClient | 524288      |    453.6 μs |    10.95 μs |    31.77 μs |    456.1 μs |    379.5 μs |    534.2 μs |  1.00 |    0.00 |
| MassTransit  | 524288      |  5,562.7 μs |   426.58 μs | 1,189.13 μs |  5,200.8 μs |  4,050.7 μs |  8,879.7 μs | 12.31 |    2.84 |
|              |             |             |             |             |             |             |             |       |         |
| RabbitClient | 1048576     |    836.8 μs |    17.11 μs |    50.17 μs |    833.4 μs |    724.1 μs |    952.0 μs |  1.00 |    0.00 |
| MassTransit  | 1048576     | 11,138.9 μs |   508.28 μs | 1,450.15 μs | 10,820.5 μs |  9,188.2 μs | 14,992.9 μs | 13.35 |    1.91 |
|              |             |             |             |             |             |             |             |       |         |
| RabbitClient | 4194304     |  3,982.1 μs |   158.14 μs |   463.79 μs |  3,841.2 μs |  3,224.1 μs |  5,211.6 μs |  1.00 |    0.00 |
| MassTransit  | 4194304     | 40,796.1 μs | 1,004.85 μs | 2,817.71 μs | 40,829.7 μs | 34,972.9 μs | 46,008.6 μs | 10.42 |    1.46 |
