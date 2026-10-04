using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Senviok.Extensions;

/// <summary>
/// Extension methods for configuring the Senviok SDK in Microsoft.Extensions.DependencyInjection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="ISenviokClient"/> and <see cref="SenviokClient"/> with the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">The Senviok API key.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSenviok(this IServiceCollection services, string apiKey)
    {
        return services.AddSenviok(options => options.ApiKey = apiKey);
    }

    /// <summary>
    /// Registers <see cref="ISenviokClient"/> and <see cref="SenviokClient"/> with configured options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Delegate to configure <see cref="SenviokClientOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSenviok(this IServiceCollection services, Action<SenviokClientOptions> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        var options = new SenviokClientOptions();
        configure(options);

        services.TryAddSingleton(options);

        services.AddHttpClient<ISenviokClient, SenviokClient>((httpClient, sp) =>
        {
            var opt = sp.GetRequiredService<SenviokClientOptions>();
            return new SenviokClient(httpClient, opt.ApiKey);
        });

        services.TryAddTransient(sp => (SenviokClient)sp.GetRequiredService<ISenviokClient>());

        return services;
    }
}
