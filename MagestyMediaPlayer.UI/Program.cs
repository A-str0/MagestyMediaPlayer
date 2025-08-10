using System;
using System.Reflection.PortableExecutable;
using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using MagestyMediaPlayer.Infrastructure;
using MagestyMediaPlayer.Infrastructure.Data;
using MagestyMediaPlayer.UI.ViewModels;
using MagestyMediaPlayer.Core.Interfaces;
using MagestyMediaPlayer.Infrastructure.Services;

namespace MagestyMediaPlayer.UI
{
    internal sealed class Program
    {
        public static IServiceProvider Services { get; private set; }

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            ConfigureServices();

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static void ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddDbContextFactory<AppDbContext>();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<LibraryViewModel>();
            services.AddSingleton<PlaybackControlViewModel>();

            services.AddSingleton<IMediaRepository, LocalMediaRepository>();
            services.AddSingleton<IPlaylistRepository, PlaylistRepository>();
            services.AddSingleton<MediaPlaybackService>();

            Services = services.BuildServiceProvider();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
                .UseReactiveUI();
    }
}
