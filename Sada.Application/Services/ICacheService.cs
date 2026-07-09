namespace Sada.Application.Services
{
    public interface ICacheService
    {
        Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> factory, CachePolicy policy);
        void Set<T>(string cacheKey, T value, DateTimeOffset absoluteExpiration);
    }
}
