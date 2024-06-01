# Dotnet RabbitMQ Benchmarking

This repository contains a simple benchmarking application that benchmarks RabbitMQ message broker using .NET.

## Benchmarks

### Http x RabbitMQ

[Results](Results/Http_vs_Rabbit/report-github.md)

This benchmark uses a simple Asp.Net WebAPI application as baseline and compares it with RabbitMQ.

### MassTransit x RabbitMQ Client

[Results](Results/MassTransitRabbit_vs_RabbitClient/report-github.md)

This benchmark compares overhead of [MassTransit](https://masstransit.io) against RabbitMQ.Client library.

## Results

- [Poster](Results/poster-small.png)
- [Report](Results/report.pdf)