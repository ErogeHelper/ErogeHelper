﻿using System.Reactive.Disposables;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ErogeHelper.Shared;
using ErogeHelper.ViewModel.HookConfig;
using ReactiveUI;
using Splat;

namespace ErogeHelper.View.HookConfig;

public partial class HookWindow : IEnableLogger
{
    public HookWindow()
    {
        InitializeComponent();
        SetHookTipLink.NavigateUri = new Uri(Shared.Languages.Strings.HookPage_LinkSetHook);
        WhatIsHookTipLink.NavigateUri = new Uri(Shared.Languages.Strings.HookPage_LinkWhatIsHook);

        ViewModel ??= DependencyResolver.GetService<HookViewModel>();

        this.WhenActivated(d =>
        {
            ViewModel.DisposeWith(d);

            this.OneWayBind(ViewModel,
                vm => vm.CurrentInUseHookName,
                v => v.CurrentInUseHook.Content).DisposeWith(d);
            this.OneWayBind(ViewModel,
                vm => vm.CurrentInUseHookColor,
                v => v.CurrentInUseHook.Foreground,
                color => color.ToNativeBrush()).DisposeWith(d);

            this.OneWayBind(ViewModel,
                vm => vm.ConsoleInfo,
                v => v.ConsoleInfo.Text).DisposeWith(d);

            this.BindCommand(ViewModel,
                vm => vm.ReInject,
                v => v.ReInject).DisposeWith(d);

            this.OneWayBind(ViewModel,
                vm => vm.CurrentSelectedText,
                v => v.SelectedText.Content,
                TextEvaluateWrapper).DisposeWith(d);

            this.BindCommand(ViewModel,
                vm => vm.Refresh,
                v => v.RefreshButton).DisposeWith(d);

            this.BindCommand(ViewModel,
                vm => vm.OpenHCodeDialog,
                v => v.HCodeButton).DisposeWith(d);

            this.BindCommand(ViewModel,
                vm => vm.OpenRCodeDialog,
                v => v.RCodeButton).DisposeWith(d);

            this.Bind(ViewModel,
                vm => vm.ClipboardStatus,
                v => v.ClipboardButton.IsChecked).DisposeWith(d);

            this.OneWayBind(ViewModel,
                vm => vm.HookEngineNames,
                v => v.HookThreadsComboBox.ItemsSource).DisposeWith(d);
            this.Bind(ViewModel,
                vm => vm.SelectedHookEngine,
                v => v.HookThreadsComboBox.SelectedItem).DisposeWith(d);
            this.BindCommand(ViewModel,
                vm => vm.RemoveHook,
                v => v.RemoveHookButton).DisposeWith(d);

            this.OneWayBind(ViewModel,
                vm => vm.HookThreadItems,
                v => v.HookThreadItems.ItemsSource).DisposeWith(d);

            this.BindCommand(ViewModel,
                vm => vm.Submit,
                v => v.SubmitButton).DisposeWith(d);

            this.OneWayBind(ViewModel,
                vm => vm.HCodeViewModel,
                v => v.HCodeDialogHost.ViewModel).DisposeWith(d);

            this.OneWayBind(ViewModel,
                vm => vm.RCodeViewModel,
                v => v.RCodeDialogHost.ViewModel).DisposeWith(d);
        });
    }

    private object TextEvaluateWrapper(string input)
    {
        var textBlock = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap
        };

        var escapedXml = SecurityElement.Escape(input);

        while (escapedXml?.IndexOf("|~S~|") != -1)
        {
            //up to |~S~| is normal
            textBlock.Inlines.Add(new Run(escapedXml?[..escapedXml.IndexOf("|~S~|", StringComparison.Ordinal)]));

            //between |~S~| and |~E~| is highlighted
            textBlock.Inlines.Add(new Run(escapedXml?[
                (escapedXml.IndexOf("|~S~|", StringComparison.Ordinal) + 5)..escapedXml.IndexOf("|~E~|", StringComparison.Ordinal)])
            {
                TextDecorations = TextDecorations.Strikethrough,
                Background = Brushes.Red
            });

            //the rest of the string (after the |~E~|)
            escapedXml = escapedXml?[(escapedXml.IndexOf("|~E~|", StringComparison.Ordinal) + 5)..];
        }

        if (escapedXml.Length > 0)
            textBlock.Inlines.Add(new Run(escapedXml));
        return textBlock;
    }
}
