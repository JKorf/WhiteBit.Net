using CryptoExchange.Net;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            // Reset environment so we know if theyre overriden
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
            // Reset environment so we know if theyre overriden
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

        /// <summary>
        /// DEPRECATED; use <see cref="AddWhiteBit(IServiceCollection, Action{WhiteBitOptions}?)" /> instead
        /// </summary>
        public static IServiceCollection AddWhiteBit(
            this IServiceCollection services,
            Action<WhiteBitRestOptions> restDelegate,
            Action<WhiteBitSocketOptions>? socketDelegate = null,
            ServiceLifetime? socketClientLifeTime = null)
        {
            services.Configure<WhiteBitRestOptions>((x) => { restDelegate?.Invoke(x); });
            services.Configure<WhiteBitSocketOptions>((x) => { socketDelegate?.Invoke(x); });

            return AddWhiteBitCore(services, socketClientLifeTime);
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
                var handler = new HttpClientHandler();
                try
                {
                    handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                }
                catch (PlatformNotSupportedException)
                { }

                var options = serviceProvider.GetRequiredService<IOptions<WhiteBitRestOptions>>().Value;
                if (options.Proxy != null)
                {
                    handler.Proxy = new WebProxy
                    {
                        Address = new Uri($"{options.Proxy.Host}:{options.Proxy.Port}"),
                        Credentials = options.Proxy.Password == null ? null : new NetworkCredential(options.Proxy.Login, options.Proxy.Password)
                    };
                }
                return handler;
            });
            services.Add(new ServiceDescriptor(typeof(IWhiteBitSocketClient), x => { return new WhiteBitSocketClient(x.GetRequiredService<IOptions<WhiteBitSocketOptions>>(), x.GetRequiredService<ILoggerFactory>()); }, socketClientLifeTime ?? ServiceLifetime.Singleton));

            services.AddTransient<ICryptoRestClient, CryptoRestClient>();
            services.AddSingleton<ICryptoSocketClient, CryptoSocketClient>();
            services.AddTransient<IWhiteBitOrderBookFactory, WhiteBitOrderBookFactory>();
            services.AddTransient<IWhiteBitTrackerFactory, WhiteBitTrackerFactory>();

            services.RegisterSharedRestInterfaces(x => x.GetRequiredService<IWhiteBitRestClient>().V4Api.SharedClient);
            services.RegisterSharedSocketInterfaces(x => x.GetRequiredService<IWhiteBitSocketClient>().V4Api.SharedClient);
            return services;
        }
    }
}
