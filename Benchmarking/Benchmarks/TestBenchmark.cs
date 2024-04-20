using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

namespace Receiver.Benchmarks;

[HtmlExporter]
[MarkdownExporterAttribute.GitHub]
public class TestBenchmark
{
    private readonly SHA256 sha256 = SHA256.Create();
    private readonly MD5 md5 = MD5.Create();

    [Benchmark]
    public byte[] Sha256() => sha256.ComputeHash("Hello Igor!"u8.ToArray());

    [Benchmark]
    public byte[] Md5() => md5.ComputeHash("Hello Igor!"u8.ToArray());
}