using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Sada.Application.Configuration;

namespace Sada.Application.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly CacheSettings _settings;

        public MemoryCacheService(IMemoryCache memoryCache, IOptions<CacheSettings> settings)
        {
            _memoryCache = memoryCache;
            _settings = settings.Value;
        }

        public async Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> factory, CachePolicy policy)
        {
            if (_memoryCache.TryGetValue(cacheKey, out T? cachedValue) && cachedValue is not null)
                return cachedValue;

            var result = await factory();

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(ResolveExpirationMinutes(policy))
            };

            _memoryCache.Set(cacheKey, result, options);
            return result;
        }

        public void Set<T>(string cacheKey, T value, DateTimeOffset absoluteExpiration)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration
            };

            _memoryCache.Set(cacheKey, value, options);
        }

        private int ResolveExpirationMinutes(CachePolicy policy) => policy switch
        {
            CachePolicy.Proposals => _settings.ProposalsAbsoluteExpirationMinutes,
            CachePolicy.Installment => _settings.InstallmentAbsoluteExpirationMinutes,
            CachePolicy.ProjectDetails => _settings.ProjectDetailsAbsoluteExpirationMinutes,
            CachePolicy.HistoryStatus => _settings.HistoryStatusAbsoluteExpirationMinutes,
            CachePolicy.ListAllProjects => _settings.ListAllProjectsAbsoluteExpirationMinutes,
            CachePolicy.Home => _settings.HomeAbsoluteExpirationMinutes,
            CachePolicy.MonitoringDashboard => _settings.MonitoringDashboardAbsoluteExpirationMinutes,
            _ => throw new ArgumentOutOfRangeException(nameof(policy), policy, "CachePolicy não mapeada")
        };
    }
}
