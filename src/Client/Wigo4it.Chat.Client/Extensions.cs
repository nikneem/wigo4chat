using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Wigo4it.Chat.Client
{
    /// <summary>
    /// Extension methods for configuring the Wigo4it Chat Client in dependency injection
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Adds the Wigo4it Chat Client to the service collection with configuration from IConfiguration
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration containing the chat client options</param>
        /// <param name="configSectionPath">The configuration section path. Default is "Wigo4itChat"</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddWigo4itChatClient(
            this IServiceCollection services,
            IConfiguration configuration,
            string configSectionPath = "Wigo4itChat")
        {
            services.Configure<ChatClientOptions>(configuration.GetSection(configSectionPath));
            return AddWigo4itChatClient(services);
        }

        /// <summary>
        /// Adds the Wigo4it Chat Client to the service collection with options configured by an action
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configureOptions">The action to configure options</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddWigo4itChatClient(
            this IServiceCollection services,
            Action<ChatClientOptions> configureOptions)
        {
            services.Configure(configureOptions);
            return AddWigo4itChatClient(services);
        }

        /// <summary>
        /// Adds the Wigo4it Chat Client to the service collection with options already configured
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddWigo4itChatClient(this IServiceCollection services)
        {
            services.AddHttpClient<IChatClient, ChatClient>().AddStandardResilienceHandler();
            services.AddSingleton<IChatHubClient, ChatHubClient>();
            return services;
        }
    }
}