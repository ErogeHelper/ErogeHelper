﻿using System;
using System.Threading.Tasks;
using System.Windows;
using ErogeHelper.Common.Functions;
using ErogeHelper.Model.DataServices;
using ErogeHelper.Model.DataServices.Interface;
using ErogeHelper.Model.Services;
using ErogeHelper.Model.Services.Interface;
using ErogeHelper.View.Windows;
using ErogeHelper.ViewModel.Windows;
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
            Locator.CurrentMutable.Register(() => new SelectProcessWindow(), typeof(IViewFor<SelectProcessViewModel>));
            // ViewModel
            Locator.CurrentMutable.RegisterLazySingleton(() => new MainGameViewModel());
            Locator.CurrentMutable.Register(() => new SelectProcessViewModel());
            // DataService
            Locator.CurrentMutable.RegisterLazySingleton(() => new GameDataService(), typeof(IGameDataService));
            // Service
            Locator.CurrentMutable.Register(() => new StartupService(), typeof(IStartupService));
            Locator.CurrentMutable.Register(() => new FilterProcessService(), typeof(IFilterProcessService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new FakeGameWindowHooker(), typeof(IGameWindowHooker));

            // https://stackoverflow.com/questions/30352447/using-reactiveuis-bindto-to-update-a-xaml-property-generates-a-warning/#31464255
            Locator.CurrentMutable.Register(() => new CustomPropertyResolver(), typeof(ICreatesObservableForProperty));
        }

        public static T GetService<T>() => Locator.Current.GetService<T>() ??
                                           throw new InvalidOperationException(
                                               $"No service for type {typeof(T)} has been registered.");

        public static void ShowView<T>() where T : ReactiveObject
        {
            var view = GetService<IViewFor<T>>();
            if (view is not Window window)
                throw new TypeAccessException("View not implement IViewFor");
            window.Show();
        }
    }
}