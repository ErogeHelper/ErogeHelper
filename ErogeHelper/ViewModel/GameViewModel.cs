using ErogeHelper.Common;
using ErogeHelper.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using log4net;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

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

        public double MainHeight { get; set; }
        public double MainWidth { get; set; }
        public double MainLeft { get; set; }
        public double MainTop { get; set; }

        private readonly MecabHelper _mecabHelper;
        private readonly MojiDictApi _mojiHelper;

        public ObservableCollection<SingleTextItem> DisplayTextCollection { get; set; }
        public TextTemplateType TextTemplateConfig { get; set; } = TextTemplateType.Default;
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
                // 啼寔(ユウマ)くんを好待(コウリャク)すれば２１０��(エン)か。なるほどなぁ´
                #region Render Model
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "ユウマ",
                    Text = "啼寔",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "兆�~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "くん",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "兆�~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "を",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "廁�~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "コウリャク",
                    Text = "好待",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "兆�~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "すれ",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "�嘖~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "ば",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "廁�~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "２",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "兆�~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "１",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "兆�~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "０",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "兆�~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "エン",
                    Text = "��",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "兆�~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "か",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "廁�~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "。",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "��催"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "なるほど",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "湖�嘖~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "なぁ",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "廁�~"
                });
                DisplayTextCollection.Add(new SingleTextItem
                {
                    RubyText = "",
                    Text = "´",
                    TextTemplateType = TextTemplateConfig,
                    PartOfSpeed = "��催"
                });
                #endregion

                CardInfo = new WordCardInfo()
                {
                    Word = "�Iう",
                    Ruby = "かう",
                    IsProcess = false,
                    Hinshi = "�嘖~",
                    Kaisetsu = new ObservableCollection<string>()
                    {
                        "1. 謹��謹方��俯謹。�┐燭�さん。謹くのもの。��",
                        "2. 謹磯��寄脅。�┐佞弔ΑＲ三磴法�たいてい。��"
                    }
                };

                MainHeight = 800;
                MainWidth = 600;
            }
            else
            {
                // Code runs "for real"
                CardInfo = new WordCardInfo();
                _mecabHelper = new MecabHelper();
                _mojiHelper = new MojiDictApi();
                WordSearchCommand = new RelayCommand<SingleTextItem>(WordSearch, CanWordSearch);
                PopupCloseCommand = new RelayCommand(() =>
                {
                    Messenger.Default.Send(new NotificationMessage("CloseCard"));
                });
                PinCommand = new RelayCommand(() => 
                {
                    TextPanelPin = !TextPanelPin;
                    log.Info(TextPanelPin);
                });

                Textractor.SelectedDataEvent += SelectedDataEventHandler;
            }
        }

        #region Text Data Dispatch
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
                    hp.Text = "海業寄噐80議猟云徭強柳狛";
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
        #endregion

        #region TextPin
        // Can't be init in constructor
        private bool _textPanelPin;

        public bool TextPanelPin
        {
            get => _textPanelPin;
            set
            {
                if (value == true)
                {
                    Messenger.Default.Send(new NotificationMessage("MakeTextPanelPin"));
                }
                else
                {
                    Messenger.Default.Send(new NotificationMessage("CancelTextPanelPin"));
                }
                _textPanelPin = value;
            }
        }

        public RelayCommand PinCommand { get; set; }
        #endregion

        #region MojiCard Search
        private WordCardInfo cardInfo;
        public WordCardInfo CardInfo { get => cardInfo; set { cardInfo = value; RaisePropertyChanged(nameof(cardInfo)); } }
        public RelayCommand<SingleTextItem> WordSearchCommand { get; private set; }
        public RelayCommand PopupCloseCommand { get; set; }

        private bool CanWordSearch(SingleTextItem item)
        {
            // This should be same as item.SubMarkColor
            if (item.PartOfSpeed == "廁�~")
            {
                return false;
            }
            else if (item.PartOfSpeed == "��催")
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
                CardInfo.Hinshi = "腎腎";
                CardInfo.Kaisetsu.Add("短嗤孀欺兔!");
            }
        }
        #endregion
    }
}