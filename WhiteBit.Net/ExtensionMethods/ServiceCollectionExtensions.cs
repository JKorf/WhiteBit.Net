using CryptoExchange.Net;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Interfaces;
using System;
using System.Net;
using System.Net.Http;
using WhiteBit.Net;
using WhiteBit.Net.Clients;
using WhiteBit.Net.Interfaces;
using WhiteBit.Net.Interfaces.Clients;
using WhiteBit.Net.Objects.Options;
using WhiteBit.Net.SymbolOrderBooks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for DI
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the IWhiteBitRestClient and IWhiteBitSocketClient to the sevice collection so they can be injected
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="defaultRestOptionsDelegate">Set default options for the rest client</param>
        /// <param name="defaultSocketOptionsDelegate">Set default options for the socket client</param>
        /// <param name="socketClientLifeTime">The lifetime of the IWhiteBitSocketClient for the service collection. Defaults to Singleton.</param>
        /// <returns></returns>
        public static IServiceCollection AddWhiteBit(
            this IServiceCollection services,
            Action<WhiteBitRestOptions>? defaultRestOptionsDelegate = null,
            Action<WhiteBitSocketOptions>? defaultSocketOptionsDelegate = null,
            ServiceLifetime? socketClientLifeTime = null)
        {
            var restOptions = WhiteBitRestOptions.Default.Copy();

            if (defaultRestOptionsDelegate != null)
            {
                defaultRestOptionsDelegate(restOptions);
                WhiteBitRestClient.SetDefaultOptions(defaultRestOptionsDelegate);
            }

            if (defaultSocketOptionsDelegate != null)
                WhiteBitSocketClient.SetDefaultOptions(defaultSocketOptionsDelegate);

            services.AddHttpClient<IWhiteBitRestClient, WhiteBitRestClient>(options =>
            {
                options.Timeout = restOptions.RequestTimeout;
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                if (restOptions.Proxy != null)
                {
                    handler.Proxy = new WebProxy
                    {
                        Address = new Uri($"{restOptions.Proxy.Host}:{restOptions.Proxy.Port}"),
                        Credentials = restOptions.Proxy.Password == null ? null : new NetworkCredential(restOptions.Proxy.Login, restOptions.Proxy.Password)
                    };
                }
                return handler;
            });

            services.AddTransient<ICryptoRestClient, CryptoRestClient>();
            services.AddSingleton<ICryptoSocketClient, CryptoSocketClient>();
            services.AddTransient<IWhiteBitOrderBookFactory, WhiteBitOrderBookFactory>();
            services.AddTransient<IWhiteBitTrackerFactory, WhiteBitTrackerFactory>();

            services.RegisterSharedRestInterfaces(x => x.GetRequiredService<IWhiteBitRestClient>().V4Api.SharedClient);
            services.RegisterSharedSocketInterfaces(x => x.GetRequiredService<IWhiteBitSocketClient>().V4Api.SharedClient);

            if (socketClientLifeTime == null)
                services.AddSingleton<IWhiteBitSocketClient, WhiteBitSocketClient>();
            else
                services.Add(new ServiceDescriptor(typeof(IWhiteBitSocketClient), typeof(WhiteBitSocketClient), socketClientLifeTime.Value));
            return services;
        }
    }
}
