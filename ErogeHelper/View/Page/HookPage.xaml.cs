﻿using Caliburn.Micro;
using ErogeHelper.Common.Enum;
using ErogeHelper.Common.Messenger;
using ModernWpf.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ErogeHelper.View.Page
{
    /// <summary>
    /// HookPage.xaml 的交互逻辑
    /// </summary>
    public partial class HookPage : IHandle<ViewActionMessage>
    {
        public HookPage()
        {
            InitializeComponent();

            IoC.Get<IEventAggregator>().SubscribeOnUIThread(this);
            DataContext = IoC.Get<ViewModel.Page.HookViewModel>();
        }

        public async Task HandleAsync(ViewActionMessage message, CancellationToken cancellationToken)
        {
            if (message.WindowType == GetType())
            {
                switch (message.Action)
                {
                    case ViewAction.OpenDialog:
                        if (message.DialogType == ModernDialog.HookCode)
                            await HCodeDialog.ShowAsync().ConfigureAwait(false);
                        else if (message.DialogType == ModernDialog.HookSettingUpdatedTip)
                            // UNDONE: 拿出来
                            await new ContentDialog
                            {
                                Content = $"Update succeed",
                                CloseButtonText = "OK"
                            }.ShowAsync().ConfigureAwait(false);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private bool _contentIsExpanded;

        private void OnContentRootSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 在设置Grid布局后，ContentColumn的真实宽度瞬间减小了InfoColumn的宽度左右
            var frameWidth = ContentColumn.ActualWidth;
            if (_contentIsExpanded)
            {
                // HACK: With many magic number this 100, frameWidth=700, Text.MaxWidth=430, InfoColumn width=160
                frameWidth += InfoColumn.Width + 100;
            }

            if (frameWidth > 700)
            {
                if (_contentIsExpanded == false)
                {
                    InfoColumn.SetValue(Grid.RowProperty, 0);
                    InfoColumn.SetValue(Grid.ColumnProperty, 1);
                    _contentIsExpanded = true;
                }
            }
            else
            {
                if (_contentIsExpanded)
                {
                    InfoColumn.SetValue(Grid.RowProperty, 1);
                    InfoColumn.SetValue(Grid.ColumnProperty, 0);
                    _contentIsExpanded = false;
                }
            }
        }
    }
}
