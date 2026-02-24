using CryptoExchange.Net;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Interfaces.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
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
        /// Add services such as the IWhiteBitRestClient and IWhiteBitSocketClient. Configures the services based on the provided configuration.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration(section) containing the options</param>
        /// <returns></returns>
        public static IServiceCollection AddWhiteBit(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var options = new WhiteBitOptions();
            // Reset environment so we know if they're overridden
            options.Rest.Environment = null!;
            options.Socket.Environment = null!;
            configuration.Bind(options);

            if (options.Rest == null || options.Socket == null)
                throw new ArgumentException("Options null");

            var restEnvName = options.Rest.Environment?.Name ?? options.Environment?.Name ?? WhiteBitEnvironment.Live.Name;
            var socketEnvName = options.Socket.Environment?.Name ?? options.Environment?.Name ?? WhiteBitEnvironment.Live.Name;
            options.Rest.Environment = WhiteBitEnvironment.GetEnvironmentByName(restEnvName) ?? options.Rest.Environment!;
            options.Rest.ApiCredentials = options.Rest.ApiCredentials ?? options.ApiCredentials;
            options.Socket.Environment = WhiteBitEnvironment.GetEnvironmentByName(socketEnvName) ?? options.Socket.Environment!;
            options.Socket.ApiCredentials = options.Socket.ApiCredentials ?? options.ApiCredentials;


            services.AddSingleton(x => Options.Options.Create(options.Rest));
            services.AddSingleton(x => Options.Options.Create(options.Socket));

            return AddWhiteBitCore(services, options.SocketClientLifeTime);
        }

        /// <summary>
        /// Add services such as the IWhiteBitRestClient and IWhiteBitSocketClient. Services will be configured based on the provided options.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="optionsDelegate">Set options for the WhiteBit services</param>
        /// <returns></returns>
        public static IServiceCollection AddWhiteBit(
            this IServiceCollection services,
            Action<WhiteBitOptions>? optionsDelegate = null)
        {
            var options = new WhiteBitOptions();
            // Reset environment so we know if they're overridden
            options.Rest.Environment = null!;
            options.Socket.Environment = null!;
            optionsDelegate?.Invoke(options);
            if (options.Rest == null || options.Socket == null)
                throw new ArgumentException("Options null");

            options.Rest.Environment = options.Rest.Environment ?? options.Environment ?? WhiteBitEnvironment.Live;
            options.Rest.ApiCredentials = options.Rest.ApiCredentials ?? options.ApiCredentials;
            options.Socket.Environment = options.Socket.Environment ?? options.Environment ?? WhiteBitEnvironment.Live;
            options.Socket.ApiCredentials = options.Socket.ApiCredentials ?? options.ApiCredentials;

            services.AddSingleton(x => Options.Options.Create(options.Rest));
            services.AddSingleton(x => Options.Options.Create(options.Socket));

            return AddWhiteBitCore(services, options.SocketClientLifeTime);
        }

        private static IServiceCollection AddWhiteBitCore(
            this IServiceCollection services,
            ServiceLifetime? socketClientLifeTime = null)
        {

            services.AddHttpClient<IWhiteBitRestClient, WhiteBitRestClient>((client, serviceProvider) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<WhiteBitRestOptions>>().Value;
                client.Timeout = options.RequestTimeout;
                return new WhiteBitRestClient(client, serviceProvider.GetRequiredService<ILoggerFactory>(), serviceProvider.GetRequiredService<IOptions<WhiteBitRestOptions>>());
            }).ConfigurePrimaryHttpMessageHandler((serviceProvider) => {
                var options = serviceProvider.GetRequiredService<IOptions<WhiteBitRestOptions>>().Value;
                return LibraryHelpers.CreateHttpClientMessageHandler(options);
            }).SetHandlerLifetime(Timeout.InfiniteTimeSpan);
            services.Add(new ServiceDescriptor(typeof(IWhiteBitSocketClient), x => { return new WhiteBitSocketClient(x.GetRequiredService<IOptions<WhiteBitSocketOptions>>(), x.GetRequiredService<ILoggerFactory>()); }, socketClientLifeTime ?? ServiceLifetime.Singleton));

            services.AddTransient<ICryptoRestClient, CryptoRestClient>();
            services.AddSingleton<ICryptoSocketClient, CryptoSocketClient>();
            services.AddTransient<IWhiteBitOrderBookFactory, WhiteBitOrderBookFactory>();
            services.AddTransient<IWhiteBitTrackerFactory, WhiteBitTrackerFactory>();
            services.AddTransient<ITrackerFactory, WhiteBitTrackerFactory>();
            services.AddSingleton<IWhiteBitUserClientProvider, WhiteBitUserClientProvider>(x =>
            new WhiteBitUserClientProvider(
                x.GetRequiredService<IHttpClientFactory>().CreateClient(typeof(IWhiteBitRestClient).Name),
                x.GetRequiredService<ILoggerFactory>(),
                x.GetRequiredService<IOptions<WhiteBitRestOptions>>(),
                x.GetRequiredService<IOptions<WhiteBitSocketOptions>>()));

            services.RegisterSharedRestInterfaces(x => x.GetRequiredService<IWhiteBitRestClient>().V4Api.SharedClient);
            services.RegisterSharedSocketInterfaces(x => x.GetRequiredService<IWhiteBitSocketClient>().V4Api.SharedClient);
            return services;
        }
    }
}
