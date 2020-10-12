using ErogeHelper.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Windows;

namespace ErogeHelper.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class GameViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public GameViewModel()
        {
            DisplayTextCollection = new ObservableCollection<SingleTextItem>();

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                ClientAreaMargin = new Thickness(10, 30, 10, 10);
                TextAreaVisibility = Visibility.Visible;

                // ����(�楦��)�������(������㥯)����У�������(����)�����ʤ�ۤɤʤ���
                #region Render Model
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "�楦��",
                    Text = "����",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "���~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "����",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "���~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "��",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "���~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "������㥯",
                    Text = "����",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "���~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "����",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "���~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "��",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "���~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "��",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "���~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "��",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "���~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "��",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "���~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "����",
                    Text = "��",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "���~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "��",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "���~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "��",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "ӛ��"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "�ʤ�ۤ�",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "�Є��~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "�ʤ�",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "���~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "��",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "ӛ��"
                });
                #endregion
            }
            else
            {
                // Code runs "for real"
                TextAreaVisibility = Visibility.Collapsed;
                Topmost = true;
            }
        }

        public bool Topmost { get; set; }
        public Thickness ClientAreaMargin { get; set; }
        public Visibility TextAreaVisibility { get; set; }

        public ObservableCollection<SingleTextItem> DisplayTextCollection { get; set; }
        public TextTemplateType TextTemplateConfig { get; set; } = TextTemplateType.Default;

    }
}