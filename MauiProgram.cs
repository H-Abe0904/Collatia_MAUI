using System;
using Microsoft.Extensions.DependencyInjection;
using Collatia_MAUI.Data.Repositories.Auth;
using Collatia_MAUI.AuthServices;
using System.Reflection;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Collatia_MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("Collatia_MAUI.env");
            using var reader = new StreamReader(stream ?? throw new InvalidOperationException("Resource not found."));

            if (stream != null)
            {
                DotNetEnv.Env.Load(stream);
            }

            string? apiBaseUrl = DotNetEnv.Env.GetString("API_BASE_URL");

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // CookieContainerとHttpClientの登録
            builder.Services.AddSingleton<CookieContainer>();
            builder.Services.AddSingleton<HttpClient>(sp =>
                {
                var CookieContainer = sp.GetRequiredService<CookieContainer>();
                var handler = new HttpClientHandler
                {
                    CookieContainer = CookieContainer,
                    UseCookies = true
                };

                return new HttpClient(handler)
                {
                    BaseAddress = new Uri(apiBaseUrl ?? throw new InvalidOperationException("API_BASE_URL is not set in the environment variables."))
                };

            });

            // 認証サービスの登録
            builder.Services.AddHttpClient<IAuthRepository, AuthRepository>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl ?? throw new InvalidOperationException("API_BASE_URL is not set in the environment variables."));
            });

            builder.Services.AddScoped<IAuthService, AuthService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
