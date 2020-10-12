using ErogeHelper.Common;
using ErogeHelper.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using log4net;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

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
        private static readonly ILog log = LogManager.GetLogger(typeof(GameViewModel));

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public GameViewModel()
        {
            log.Info("Initialize");

            DisplayTextCollection = new ObservableCollection<SingleTextItem>();
            TextTemplateConfig = TextTemplateType.KanaBottom;

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
                TextPanelPin = true;

                Textractor.SelectedDataEvent += SelectedDataEventHandler;
                _mecabHelper = new MecabHelper();
                WordSearchCommand = new RelayCommand<MouseButtonEventArgs>(WordSearch, CanWordSearch);
            }
        }

        private readonly MecabHelper _mecabHelper;

        private void SelectedDataEventHandler(object sender, HookParam hp)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                DisplayTextCollection.Clear();

                var mecabWordList = _mecabHelper.SentenceHandle(hp.Text);
                foreach (MecabWordInfo mecabWord in mecabWordList)
                {
                    DisplayTextCollection.Add(new SingleTextItem
                    {
                        Text = mecabWord.Word,
                        RubyText = mecabWord.Kana,
                        PartOfSpeed = mecabWord.PartOfSpeech,
                        TextTemplateType = TextTemplateConfig
                    });
                }
            });
        }

        public bool Topmost { get; set; }
        public Thickness ClientAreaMargin { get; set; }
        public Visibility TextAreaVisibility { get; set; }

        public ObservableCollection<SingleTextItem> DisplayTextCollection { get; set; }
        public TextTemplateType TextTemplateConfig { get; set; } = TextTemplateType.Default;

        private bool _textPanelPin;
        public bool TextPanelPin 
        {
            get => _textPanelPin;
            set
            {
                if (value == true)
                {
                    TextAreaVisibility = Visibility.Visible;
                    Messenger.Default.Send(new NotificationMessage("MakeTextPanelPin"));
                }
                else
                {
                    TextAreaVisibility = Visibility.Collapsed;
                    Messenger.Default.Send(new NotificationMessage("CancelTextPanelPin"));
                }
                _textPanelPin = value;
            }
        }

        public RelayCommand<MouseButtonEventArgs> WordSearchCommand { get; private set; }
        private bool CanWordSearch(MouseButtonEventArgs e) => true;

        private void WordSearch(MouseButtonEventArgs e)
        {
            log.Info(e.Source.ToString());
            log.Info("Click");
        }
    }
}