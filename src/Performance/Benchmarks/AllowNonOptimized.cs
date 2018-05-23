namespace Benchmarks
{
    using System.Linq;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Validators;

    public class AllowNonOptimized
        : ManualConfig
    {
        public AllowNonOptimized()
        {
            this.Add(JitOptimizationsValidator.DontFailOnError);

            this.Add(DefaultConfig.Instance.GetLoggers().ToArray());
            this.Add(DefaultConfig.Instance.GetExporters().ToArray());
            this.Add(DefaultConfig.Instance.GetColumnProviders().ToArray());
        }
    }
}