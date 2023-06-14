using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KuberDemo.HealthChecks
{
    public class RandomHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var result = Random.Shared.Next(5) != 0
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("Failed random");

            return Task.FromResult(result);
        }
    }
}
