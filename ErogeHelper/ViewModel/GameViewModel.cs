using ErogeHelper.Common;
using ErogeHelper.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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
            TextTemplateConfig = TextTemplateType.OutLineKanaBottom;

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

                CardInfo = new WordCardInfo()
                {
                    Word = "�I��",
                    Ruby = "����",
                    IsProcess = true, // fake
                    Hinshi = "���~",
                    Kaisetsu = new ObservableCollection<string>()
                    {
                        "1. �࣬��������ࡣ���������󡣶य�Τ�Ρ���",
                        "2. ��룬�󶼡����դĤ���һ��ˡ������Ƥ�����"
                    }
                };
            }
            else
            {
                // Code runs "for real"
                TextAreaVisibility = Visibility.Collapsed;
                Topmost = true;
                TextPanelPin = true;

                Textractor.SelectedDataEvent += SelectedDataEventHandler;
                _mecabHelper = new MecabHelper();
                _mojiHelper = new MojiDictApi();
                WordSearchCommand = new RelayCommand<SingleTextItem>(WordSearch, CanWordSearch);
                CardInfo = new WordCardInfo();
                PopupCloseCommand = new RelayCommand(() =>
                {
                    Messenger.Default.Send(new NotificationMessage("CloseCard"));
                });
            }
        }

        private readonly MecabHelper _mecabHelper;
        private readonly MojiDictApi _mojiHelper;

        private void SelectedDataEventHandler(object sender, HookParam hp)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                DisplayTextCollection.Clear();

                var pattern = SimpleIoc.Default.GetInstance<GameInfo>().Regexp;
                if (pattern != null)
                {
                    var list = Regex.Split(hp.Text, pattern);
                    hp.Text = string.Join("", list);
                }
                
                if (hp.Text.Length > 80)
                {
                    hp.Text = "String.Length > 80. Skip";
                }

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

        public RelayCommand<SingleTextItem> WordSearchCommand { get; private set; }
        private bool CanWordSearch(SingleTextItem item)
        {
            if (item.PartOfSpeed == "���~")
            {
                return false;
            }
            else if (item.PartOfSpeed == "ӛ��")
            {
                return false;
            }
            return true;
        }

        private async void WordSearch(SingleTextItem item)
        {
            log.Info($"Search \"{item.Text}\", partofspeech {item.PartOfSpeed} ");

            CardInfo = new WordCardInfo()
            {
                Word = item.Text,
                IsProcess = true
            };

            Messenger.Default.Send(new NotificationMessage("OpenCard"));

            var resp = await _mojiHelper.RequestAsync(item.Text);

            var result = resp.result;
            if (result != null)
            {
                log.Info($"Find explain <{result.word.excerpt}>");

                CardInfo.IsProcess = false;
                CardInfo.Word = result.word.spell;
                CardInfo.Hinshi = result.details[0].title;
                CardInfo.Ruby = result.word.pron;
                int count = 1;
                foreach (var subdetail in result.subdetails)
                {
                    CardInfo.Kaisetsu.Add($"{count++}. {subdetail.title}");
                }
            }
            else
            {
                CardInfo.IsProcess = false;
                CardInfo.Hinshi = "�տ�";
                CardInfo.Kaisetsu.Add("û���ҵ�ѽ!");
            }

        }

        private WordCardInfo cardInfo;
        public WordCardInfo CardInfo { get => cardInfo; set { cardInfo = value; RaisePropertyChanged(nameof(cardInfo)); } }
        public RelayCommand PopupCloseCommand { get; set; }
    }

    public class WordCardInfo : ViewModelBase
    {
        private bool isProcess; // procssing flag
        private string ruby;
        private string hinshi; // Ʒ�~
        private string word;
        private ObservableCollection<string> kaisetsu = new ObservableCollection<string>();

        public string Word { get => word; set { word = value; RaisePropertyChanged(nameof(word)); } }
        public string Hinshi { get => hinshi; set { hinshi = value; RaisePropertyChanged(nameof(hinshi)); } }
        public string Ruby { get => ruby; set { ruby = value; RaisePropertyChanged(nameof(ruby)); } }
        public ObservableCollection<string> Kaisetsu { get => kaisetsu; set { kaisetsu = value; RaisePropertyChanged(nameof(kaisetsu)); } }
        public bool IsProcess { get => isProcess; set { isProcess = value; RaisePropertyChanged(nameof(isProcess)); } }
    }
}