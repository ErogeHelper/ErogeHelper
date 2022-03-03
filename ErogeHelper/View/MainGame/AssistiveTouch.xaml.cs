﻿using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using ErogeHelper.Platform.XamlTool;
using ErogeHelper.Shared;
using ErogeHelper.Shared.Entities;
using ErogeHelper.ViewModel.MainGame;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace ErogeHelper.View.MainGame;

public partial class AssistiveTouch : IViewFor<AssistiveTouchViewModel>
{
    #region ViewModel DependencyProperty
    /// <summary>Identifies the <see cref="ViewModel"/> dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(AssistiveTouchViewModel),
        typeof(AssistiveTouch));

    public AssistiveTouchViewModel? ViewModel
    {
        get => (AssistiveTouchViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => SetValue(ViewModelProperty, (AssistiveTouchViewModel?)value);
    }
    #endregion

    public const double TouchTransformDuration = 200;
    private const double TouchReleaseToEdgeDuration = 300;
    private const double OpacityChangeDuration = 4000;

    private const double OpacityHalf = 0.4;
    private const double OpacityFull = 1;
    private const double ButtonSpace = 2;

    // The diameter of button use for mouse releasing
    private double _buttonSize;
    private double _distance;
    private double _halfDistance;
    private double _oneThirdDistance;
    private double _twoThirdDistance;

    private bool _isMoving = false;

    public Action<double>? UpdateMenuStatusDelegate;

    public AssistiveTouch()
    {
        InitializeComponent();

        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            return;

        ViewModel = DependencyResolver.GetService<AssistiveTouchViewModel>();

        var disposables = new CompositeDisposable();
        Loaded += (_, _) =>
        {
            var parent = Parent as FrameworkElement
                ?? throw new InvalidOperationException("Control's parent must be FrameworkElement type");

            UpdateButtonDiameterFields(ViewModel.UseBigSize.Take(1).Wait(), TouchPosition, parent);
            SetCurrentValue(VisibilityProperty, Visibility.Visible);

            parent.Events().SizeChanged
                .Subscribe(_ => SetCurrentValue(MarginProperty, GetTouchMargin(_buttonSize, TouchPosition, parent)))
                .DisposeWith(disposables);

            #region Opacity Adjust
            FadeOpacityAnimation.Freeze();
            var mainGameWindow = (MainGameWindow)Application.Current.MainWindow;
            Point lastPos;
            BehaviorSubject<bool> tryTransparentizeSubj = new(true);
            tryTransparentizeSubj
                .Throttle(TimeSpan.FromMilliseconds(OpacityChangeDuration))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(on => on && _isMoving == false && mainGameWindow.TouchMenu.IsOpen == false)
                .Subscribe(_ => BeginAnimation(OpacityProperty, FadeOpacityAnimation));

            mainGameWindow.TouchMenu.Events().Closed
                .Subscribe(_ => tryTransparentizeSubj.OnNext(true))
                .DisposeWith(disposables);
            #endregion

            #region Core Logic of Moving

            var updateStatusWhenMouseDown = this.Events().PreviewMouseLeftButtonDown;
            var mouseMoveAndCheckEdge = parent.Events().PreviewMouseMove;
            var mouseRelease = parent.Events().PreviewMouseUp;

            updateStatusWhenMouseDown.Subscribe(evt =>
            {
                SetCurrentValue(OpacityProperty, OpacityFull);
                _isMoving = true;
                tryTransparentizeSubj.OnNext(true);
                lastPos = evt.GetPosition(parent);
                _oldPos = lastPos;
            });

            mouseMoveAndCheckEdge.Subscribe(evt =>
            {
                if (!_isMoving)
                    return;

                var newPos = evt.GetPosition(parent);
                // new position equal current margin plus position delta
                var left = Margin.Left + newPos.X - lastPos.X;
                var top = Margin.Top + newPos.Y - lastPos.Y;

                SetCurrentValue(MarginProperty, new Thickness(left, top, 0, 0));

                lastPos = newPos;

                // if mouse go out of the edge
                if (left < -_oneThirdDistance || top < -_oneThirdDistance ||
                left > parent.ActualWidth - _twoThirdDistance || top > parent.ActualHeight - _twoThirdDistance)
                {
                    RaiseMouseReleaseEventInCode(this, parent);
                }
            });

            mouseRelease.Subscribe(evt =>
            {
                if (!_isMoving)
                    return;

                var pos = evt.GetPosition(parent);
                _newPos = pos;
                var parentActualHeight = parent.ActualHeight;
                var parentActualWidth = parent.ActualWidth;
                var curMarginLeft = Margin.Left;
                var curMarginTop = Margin.Top;

                var left = curMarginLeft + _newPos.X - lastPos.X;
                var top = curMarginTop + _newPos.Y - lastPos.Y;
                // button 距离右边缘距离
                var right = parentActualWidth - left - _buttonSize;
                // button 距离下边缘距离
                var bottom = parentActualHeight - top - _buttonSize;
                var verticalMiddleLine = parentActualWidth / 2;

                // 根据button所处屏幕位置来确定button之后应该动画移动到的位置
                if (left < _halfDistance && top < _twoThirdDistance) // button 距离左上角边距同时小于 distance
                {
                    left = ButtonSpace;
                    top = ButtonSpace;
                    TouchPosition = AssistiveTouchPosition.UpperLeft;
                }
                else if (left < _halfDistance && bottom < _twoThirdDistance) // bottom-left
                {
                    left = ButtonSpace;
                    top = parentActualHeight - _buttonSize - ButtonSpace;
                    TouchPosition = AssistiveTouchPosition.LowerLeft;
                }
                else if (right < _halfDistance && top < _twoThirdDistance) // top-right
                {
                    left = parentActualWidth - _buttonSize - ButtonSpace;
                    top = ButtonSpace;
                    TouchPosition = AssistiveTouchPosition.UpperRight;
                }
                else if (right < _halfDistance && bottom < _twoThirdDistance) // bottom-right
                {
                    left = parentActualWidth - _buttonSize - ButtonSpace;
                    top = parentActualHeight - _buttonSize - ButtonSpace;
                    TouchPosition = AssistiveTouchPosition.LowerRight;
                }
                else if (top < _twoThirdDistance) // top
                {
                    left = curMarginLeft;
                    top = ButtonSpace;
                    var scale = (curMarginLeft + ButtonSpace + (_buttonSize / 2)) / parentActualWidth;
                    TouchPosition = new(TouchButtonCorner.Top, scale);
                }
                else if (bottom < _twoThirdDistance) // bottom
                {
                    left = curMarginLeft;
                    top = parentActualHeight - _buttonSize - ButtonSpace;
                    var scale = (curMarginLeft + ButtonSpace + (_buttonSize / 2)) / parentActualWidth;
                    TouchPosition = new(TouchButtonCorner.Bottom, scale);
                }
                else if (left + (_buttonSize / 2) < verticalMiddleLine) // left
                {
                    left = ButtonSpace;
                    top = curMarginTop;
                    var scale = (curMarginTop + ButtonSpace + (_buttonSize / 2)) / parentActualHeight;
                    TouchPosition = new(TouchButtonCorner.Left, scale);
                }
                else // right
                {
                    left = parentActualWidth - _buttonSize - ButtonSpace;
                    top = curMarginTop;
                    var scale = (curMarginTop + ButtonSpace + _buttonSize / 2) / parentActualHeight;
                    TouchPosition = new(TouchButtonCorner.Right, scale);
                }

                SmoothMoveAnimation(this, left, top);
                _isMoving = false;
                tryTransparentizeSubj.OnNext(true);
            });

            #endregion Core Logic of Moving
        };

        this.WhenActivated(d =>
        {
            disposables.DisposeWith(d);
            this.WhenAnyObservable(x => x.ViewModel!.UseBigSize)
                .Skip(1)
                .Subscribe(isBigSize =>
                    UpdateButtonDiameterFields(isBigSize, TouchPosition, (FrameworkElement)Parent)).DisposeWith(d);
        });
    }

    public double Size => ViewModel!.UseBigSize.Take(1).Wait() ? XamlResource.AssistiveTouchBigSize
                                                               : XamlResource.AssistiveTouchSize;

    public void Show() => SetCurrentValue(VisibilityProperty, Visibility.Visible);

    public void Hide() => SetCurrentValue(VisibilityProperty, Visibility.Collapsed);

    private static readonly DoubleAnimation FadeOpacityAnimation = new()
    {
        From = OpacityFull,
        To = OpacityHalf,
        Duration = TimeSpan.FromMilliseconds(TouchTransformDuration)
    };

    /// <summary>
    /// Set whole button layout values
    /// </summary>
    private void UpdateButtonDiameterFields
        (bool isBigSize, AssistiveTouchPosition touchPos, FrameworkElement parent)
    {
        XamlResource.AssistiveTouchStyle = GetAssistiveTouchStyle(isBigSize);
        _buttonSize = isBigSize ? XamlResource.AssistiveTouchBigSize : XamlResource.AssistiveTouchSize;
        _distance = _buttonSize;
        _halfDistance = _distance / 2;
        _oneThirdDistance = _distance / 3;
        _twoThirdDistance = _oneThirdDistance * 2;

        // This would effect Margin value when first time idk why
        var newMargin = GetTouchMargin(_buttonSize, touchPos, parent);
        Margin = newMargin;
        SetCurrentValue(MarginProperty, newMargin);

        // Update assistivetouch menu dynamic values
        XamlResource.AssistiveTouchMenuMaxSize = isBigSize ? XamlResource.AssistiveTouchMenuBiggerSize
                                                           : XamlResource.AssistiveTouchMenuNormalSize;
        XamlResource.AssistiveTouchItemSize = isBigSize ? XamlResource.AssistiveTouchItemBiggerSize
                                                        : XamlResource.AssistiveTouchItemNormalSize;
        UpdateMenuStatusDelegate!(parent.ActualHeight);
    }

    private static ControlTemplate GetAssistiveTouchStyle(bool useBigSize) =>
        useBigSize ? XamlResource.BigAssistiveTouchTemplate : XamlResource.NormalAssistiveTouchTemplate;

    private static void RaiseMouseReleaseEventInCode(Button touch, UIElement father)
    {
        var timestamp = new TimeSpan(DateTime.Now.Ticks).Milliseconds;

        var mouseUpEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, timestamp, MouseButton.Left)
        {
            RoutedEvent = PreviewMouseUpEvent,
            Source = touch,
        };

        father.RaiseEvent(mouseUpEvent);
    }

    private readonly static ThicknessAnimation MoveAnimation = new() 
    { 
        Duration = TimeSpan.FromMilliseconds(TouchReleaseToEdgeDuration)
    };

    private static void SmoothMoveAnimation(Button touch, double left, double top)
    {
        MoveAnimation.SetCurrentValue(ThicknessAnimation.FromProperty, touch.Margin);
        MoveAnimation.SetCurrentValue(ThicknessAnimation.ToProperty, new Thickness(left, top, 0, 0));
        touch.BeginAnimation(MarginProperty, MoveAnimation);
    }

    private static Thickness GetTouchMargin(
        double buttonSize, AssistiveTouchPosition touchPos, FrameworkElement parent)
    {
        var rightLineMargin = parent.ActualWidth - buttonSize - ButtonSpace;
        var bottomLineMargin = parent.ActualHeight - buttonSize - ButtonSpace;
        var verticalScaleMargin = (touchPos.Scale * parent.ActualHeight) - (buttonSize / 2) - ButtonSpace;
        var horizontalScaleMargin = (touchPos.Scale * parent.ActualWidth) - buttonSize / 2 - ButtonSpace;
        return touchPos.Corner switch
        {
            TouchButtonCorner.UpperLeft => new Thickness(ButtonSpace, ButtonSpace, 0, 0),
            TouchButtonCorner.UpperRight => new Thickness(rightLineMargin, ButtonSpace, 0, 0),
            TouchButtonCorner.LowerLeft => new Thickness(ButtonSpace, bottomLineMargin, 0, 0),
            TouchButtonCorner.LowerRight => new Thickness(rightLineMargin, bottomLineMargin, 0, 0),
            TouchButtonCorner.Left => new Thickness(ButtonSpace, verticalScaleMargin, 0, 0),
            TouchButtonCorner.Top => new Thickness(horizontalScaleMargin, ButtonSpace, 0, 0),
            TouchButtonCorner.Right => new Thickness(rightLineMargin, verticalScaleMargin, 0, 0),
            TouchButtonCorner.Bottom => new Thickness(horizontalScaleMargin, bottomLineMargin, 0, 0),
            _ => throw new InvalidOperationException(),
        };
    }

    public AssistiveTouchPosition TouchPosition
    {
        get => ViewModel!.AssistiveTouchPosition;
        set => ViewModel!.AssistiveTouchPosition = value;
    }

    private new event EventHandler? ClickEvent;

    private Point _newPos; // The position of the button after the left mouse button is released
    private Point _oldPos; // The position of the button

    private void AssistiveTouchOnClick(object sender, RoutedEventArgs e)
    {
        if (_newPos == _oldPos && !_isMoving)
        {
            ClickEvent?.Invoke(sender, e);
        }
        else
        {
            e.Handled = true;
        }
    }
}
