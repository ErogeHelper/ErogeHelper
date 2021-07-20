﻿using System;
using System.Threading.Tasks;
using System.Windows;
using ErogeHelper.Common.Function;
using ErogeHelper.Model.DataService;
using ErogeHelper.Model.DataService.Interface;
using ErogeHelper.Model.Service;
using ErogeHelper.Model.Service.Interface;
using ErogeHelper.View.Window;
using ErogeHelper.ViewModel.Window;
using ReactiveUI;
using Splat;

namespace ErogeHelper.Common
{
    public static class DependencyInject
    {
        public static void Register()
        {
            // Locator.CurrentMutable.InitializeSplat();
            // Locator.CurrentMutable.InitializeReactiveUI(RegistrationNamespace.Wpf);
            
            // View
            Locator.CurrentMutable.Register(() => new MainGameWindow(), typeof(IViewFor<MainGameViewModel>));
            // ViewModel
            Locator.CurrentMutable.RegisterLazySingleton(() => new MainGameViewModel());
            // DataService
            Locator.CurrentMutable.RegisterLazySingleton(() => new GameDataService(), typeof(IGameDataService));
            // Service
            Locator.CurrentMutable.Register(() => new StartupService(), typeof(IStartupService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new FakeGameWindowHooker(), typeof(IGameWindowHooker));

            // https://stackoverflow.com/a/31464255
            Locator.CurrentMutable.Register(() => new CustomPropertyResolver(), typeof(ICreatesObservableForProperty));
        }

        public static T GetService<T>() => Locator.Current.GetService<T>() ??
                                           throw new InvalidOperationException(
                                               $"No service for type {typeof(T)} has been registered.");

        public static async Task ShowViewAsync<T>() where T : ReactiveObject =>
            await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    var view = GetService<IViewFor<T>>();
                    if (view is not Window window)
                        throw new TypeAccessException("View not implement IViewFor");
                    window.Show();
                });

        public static void ShowView<T>() where T : ReactiveObject =>
            Application.Current.Dispatcher.Invoke(() =>
            {
                var view = GetService<IViewFor<T>>();
                if (view is not Window window)
                    throw new TypeAccessException("View not implement IViewFor");
                window.Show();
            });
    }
}