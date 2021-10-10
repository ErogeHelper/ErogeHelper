﻿using ReactiveUI;
using System.Reactive.Disposables;

namespace ErogeHelper.View.Pages
{
    /// <summary>
    /// GeneralPage.xaml 的交互逻辑
    /// </summary>
    public partial class GeneralPage
    {
        public GeneralPage()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.Bind(ViewModel,
                    vm => vm.UseBigSizeAssistiveTouch,
                    v => v.BigSizeAssistiveTouch.IsOn).DisposeWith(d);

                this.Bind(ViewModel,
                    vm => vm.UseDPIDpiCompatibility,
                    v => v.DpiCompatibility.IsChecked).DisposeWith(d);

                this.Bind(ViewModel,
                    vm => vm.UseEdgeTouchMask,
                    v => v.EdgeTouchMask.IsOn).DisposeWith(d);
            });
        }
    }
}
