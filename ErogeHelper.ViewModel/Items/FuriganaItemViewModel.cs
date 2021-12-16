﻿using System.Drawing;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ErogeHelper.ViewModel.Items;

public class FuriganaItemViewModel : ReactiveObject
{
    [Reactive]
    public string Text { get; set; } = string.Empty;

    [Reactive]
    public string Kana { get; set; } = string.Empty;

    [Reactive]
    public double FontSize { get; set; } = 1;

    [Reactive]
    public Color BackgroundColor { get; set; }

    public ReactiveCommand<Unit, Unit> LayoutUpdated { get; set; } = ReactiveCommand.Create(() => { });
}
