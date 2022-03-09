using Microsoft.Extensions.DependencyInjection;

namespace HttpHelper.Core
{
    public static class ServiceRegistrationLayer
    {
        // register all http services
        public static IServiceCollection RegisterHttpHelper(this IServiceCollection service)
        {
            service.AddHttpClient();

            // for set locator if Ihttpclient factory not set
            ServiceLocatorHelper.SetLocatorProvider(service.BuildServiceProvider());
            return service;
        }
    }
}
