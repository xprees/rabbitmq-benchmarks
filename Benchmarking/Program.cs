using BenchmarkDotNet.Running;
using Benchmarking.Benchmarks;

BenchmarkRunner.Run<RabbitMqAndHttpBenchmark>();
BenchmarkRunner.Run<MassTransitBenchmark>();