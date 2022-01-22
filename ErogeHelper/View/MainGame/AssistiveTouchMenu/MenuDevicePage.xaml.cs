﻿using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ErogeHelper.Shared.Contracts;
using ErogeHelper.View.MainGame.AssistiveMenu;
using WindowsInput.Events;

namespace ErogeHelper.View.MainGame;

public partial class MenuDevicePage : Page
{
    private readonly Subject<string> _pageSubject = new();
    public IObservable<string> PageChanged => _pageSubject;

    public MenuDevicePage()
    {
        InitializeComponent();
        ApplyTransistionInAnimation();
    }

    public void TransistIn()
    {
        SetCurrentValue(VisibilityProperty, Visibility.Visible);

        GridPanel.Children.Cast<MenuItemControl>().FillBackground(false);

        VolumeDown.SetCurrentValue(RenderTransformProperty, _volumeDownTransform);
        Back.SetCurrentValue(RenderTransformProperty, _backTransform);

        _volumeDownMoveAnimation.SetCurrentValue(DoubleAnimation.FromProperty, _volumeDownTransform.X);
        _backMoveAnimation.SetCurrentValue(DoubleAnimation.FromProperty, _backTransform.Y);

        _transitionInStoryboard.Begin();
    }

    public void TransistOut()
    {
        GridPanel.Children.Cast<MenuItemControl>().FillBackground(false);
        _transitionInStoryboard.SetCurrentValue(Timeline.AutoReverseProperty, true);
        _transitionInStoryboard.Begin();
        _transitionInStoryboard.Seek(TimeSpan.FromMilliseconds(AssistiveTouch.TouchTransformDuration));
    }

    private void BackOnClickEvent(object sender, EventArgs e) => _pageSubject.OnNext(PageTag.DeviceBack);

    private readonly TranslateTransform _volumeDownTransform = AnimationTool.LeftOneTransform;
    private readonly TranslateTransform _backTransform = AnimationTool.BottomOneTransform;

    private readonly Storyboard _transitionInStoryboard = new();
    private readonly DoubleAnimation _volumeDownMoveAnimation = AnimationTool.TransformMoveToZeroAnimation;
    private readonly DoubleAnimation _backMoveAnimation = AnimationTool.TransformMoveToZeroAnimation;

    private void ApplyTransistionInAnimation()
    {
        var pageOpacityAnimation = AnimationTool.FadeInAnimation;
        Storyboard.SetTarget(pageOpacityAnimation, this);
        Storyboard.SetTargetProperty(pageOpacityAnimation, new PropertyPath(OpacityProperty));
        _transitionInStoryboard.Children.Add(pageOpacityAnimation);

        Storyboard.SetTarget(_volumeDownMoveAnimation, VolumeDown);
        Storyboard.SetTargetProperty(_volumeDownMoveAnimation, new PropertyPath(AnimationTool.XProperty));
        _transitionInStoryboard.Children.Add(_volumeDownMoveAnimation);

        Storyboard.SetTarget(_backMoveAnimation, Back);
        Storyboard.SetTargetProperty(_backMoveAnimation, new PropertyPath(AnimationTool.YProperty));
        _transitionInStoryboard.Children.Add(_backMoveAnimation);

        _transitionInStoryboard.Completed += (_, _) =>
        {
            VolumeDown.SetCurrentValue(RenderTransformProperty, new TranslateTransform(0, 0));
            Back.SetCurrentValue(RenderTransformProperty, new TranslateTransform(0, 0));
            GridPanel.Children.Cast<MenuItemControl>().FillBackground(true);

            if (_transitionInStoryboard.AutoReverse == true)
            {
                _transitionInStoryboard.SetCurrentValue(Timeline.AutoReverseProperty, false);
                SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
            }
        };
    }

    private async void VolumeDownOnClickEvent(object sender, EventArgs e) =>
         await WindowsInput.Simulate.Events()
            .Click(KeyCode.VolumeDown)
            .Invoke().ConfigureAwait(false);

    private async void VolumeUpOnClickEvent(object sender, EventArgs e) =>
        await WindowsInput.Simulate.Events()
            .Click(KeyCode.VolumeUp)
            .Invoke().ConfigureAwait(false);
}
