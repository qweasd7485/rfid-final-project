using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace GAME1
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class Game1 : GAME1.Common.LayoutAwarePage
    {
        public Common.RankSource RankModel { get; set; }
        private MobileServiceCollection<Common.RankSource, Common.RankSource> items;
        private IMobileServiceTable<Common.RankSource> todoTable = App.MobileService.GetTable<Common.RankSource>();
        private DispatcherTimer timer;
        private double time;

        public Game1()
        {
            this.InitializeComponent();
            RankModel = new Common.RankSource();
           
        }

    
        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected  override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
          
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {

        }

        //目前題數
        int i = 1;

        //按鈕鎖定
        string buttonlock = "close";

        //答案
        List<int> questions = new List<int>();

        //目前答題
        //List<int> answers = new List<int>();
        int answers = 0;


        //按下Start按鈕
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           GameStart();  
        }

        
        
      
        /// <summary>
        /// 遊戲開始流程。
        /// </summary>
        private void GameStart()
        {
            //隨機產生變數(題目)
            Random rnd = new Random();
            int rand = rnd.Next(1, 10);

            //變數加入陣列
            questions.Add(rand);

            //隱藏Start按鈕
            Start.Visibility = Visibility.Collapsed;

            CreateAnimation();
            
            //播放音效
            //media.Play();
        }

       /// <summary>
       /// 產生題目動畫。
       /// </summary>
        private async void CreateAnimation()
        {
            level.Text = "Level:"+i.ToString();
            Storyboard[] myStoryboard = new Storyboard[i];
            //button鎖定，避免玩家在出題目時點擊按鈕。
            buttonlock = "close";
            for (int story = 0; story < i; story++)
            {
                
                myStoryboard[story] = new Storyboard();

                DoubleAnimation newDoubleAnimation = new DoubleAnimation();
                Storyboard.SetTargetName(newDoubleAnimation, "Button"+questions[story]);
                Storyboard.SetTargetProperty(newDoubleAnimation, "Opacity");
                double from = 1.0;
                double to = 0.0;
                newDoubleAnimation.From = from;
                newDoubleAnimation.To = to;
                newDoubleAnimation.Duration = TimeSpan.FromMilliseconds(200);
                newDoubleAnimation.AutoReverse = true;

                myStoryboard[story].Children.Add(newDoubleAnimation);
                myStoryboard[story].BeginTime = TimeSpan.FromMilliseconds(050);

                //myStoryBoardabc.Children.Add(myStoryboard[story]);
                pageRoot.Resources.Add("myStoryBoard" + story, myStoryboard[story]);
                //myStoryBoardabc.Begin();
                await myStoryboard[story].BeginAsync();
            }
            
            i++;


            
            buttonlock = "open";
            Time2.Foreground = new SolidColorBrush(Colors.Red);
            timer_start();
        }

        /// <summary>
        /// 檢查題目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button1_Click(object sender, RoutedEventArgs e)
        {

            //media.Play();

            //取的使用者輸入的答案
            int ans = Convert.ToInt32(((Button)(sender)).Content);


            if (buttonlock == "open")
            {
                //檢查答案
                if (questions[answers] == ans)//答對
                {
                    answers++;
                    if (answers >= questions.Count)
                    {
                        answers = 0;
                        ClearAnimation();
                        CreateExam();
                        timer_reset();
                    }
                }
                else//答錯
                {
                    //Read Local file
                    var txt = await StorageHelper.ReadTextFromFile("Score.txt");

                    //if local have score.
                    if (!String.IsNullOrEmpty(txt))
                    {
                        RankModel = JsonHelper.Deserizlize<Common.RankSource>(txt);

                        //If score > your local score.
                        if (i - 1 > RankModel.Score)
                        {
                            string Name = await UserInformation.GetDisplayNameAsync();

                            //Save new score to Local.
                            RankModel.Name = Name;
                            RankModel.Score = i - 2;
                            RankModel.Date = DateTime.Now;
                            await StorageHelper.SaveTextToFile
                            ("Score.txt", JsonHelper.Serialize<Common.RankSource>(RankModel));

                            //Check have 50 topic Insert to Db.
                            RankModel.Id = 0;
                            await todoTable.InsertAsync(RankModel);

                        }
                    }
                    else//If local is null
                    {
                        string Name = await UserInformation.GetDisplayNameAsync();

                        //Save score to local
                        RankModel.Name = Name;
                        RankModel.Score = i - 1;
                        RankModel.Date = DateTime.Now;
                        await StorageHelper.SaveTextToFile
                        ("Score.txt", JsonHelper.Serialize<Common.RankSource>(RankModel));
                        await todoTable.InsertAsync(RankModel);
                        //Check have 50 topi Insert to Db
                    }
                    timer_reset();
                    await new MessageDialog("嗚嗚~答錯了 :)", "過" + (i - 1) + "關").ShowAsync();
                    ResetGame();




                }

            }


        }


        /// <summary>
        /// 清除目前動畫
        /// </summary>
        private void ClearAnimation()
        {
            pageRoot.Resources.Clear();
        }

        /// <summary>
        /// 產生考題
        /// </summary>
        private void CreateExam()
        {
            Random rnd = new Random();
            int rand = rnd.Next(1, 10);
            questions.Add(rand);
            CreateAnimation();
        }

        /// <summary>
        /// 重置遊戲
        /// </summary>
        private void ResetGame()
        {
            i = 1;
            answers = 0;
            level.Text = "Level:1";
            questions.Clear();
            ClearAnimation();
            Start.Visibility = Visibility.Visible;
            buttonlock = "close";
            
        }


        /// <summary>
        /// 計時開始
        /// </summary>
        private void timer_start()
        {
            timer = new DispatcherTimer();
            timeInit();
            timer.Tick += timer_tick;
            timer.Start();
        }

        //Setting Timer async
        private async void timer_tick(object sender, object e)
        {
            Time2.Text = time.ToString("00.00");
            time = time - 0.04;

            if (time <0)
            {
                timer_reset();
                await new MessageDialog("", "時間到~").ShowAsync();
                ResetGame();
            }
        }

        /// <summary>
        /// 初始化計時器
        /// </summary>
        private void timeInit()
        {
            timer.Interval = new TimeSpan(0, 0, 0,0,1);
            time = 30;
        }
        /// <summary>
        /// 計時器重置
        /// </summary>
        private void timer_reset()
        {
            timer.Stop();
            Time2.Text = "30";
            Time2.Foreground = new SolidColorBrush(Colors.LightGreen);
        }

        private void GameOver(int i)
        {
            //case 1:

            //break;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
            try
            {
                timer_reset();
            }
            catch { }
        }
    }
}
