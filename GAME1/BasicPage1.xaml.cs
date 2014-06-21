using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace GAME1
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class BasicPage1 : GAME1.Common.LayoutAwarePage
    {

        private DispatcherTimer timer;
        private double time;

        public BasicPage1()
        {
            this.InitializeComponent();
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
            Number.Text = time.ToString("0.#0");
            time = time - 0.04;

            if (time < 0)
            {
                timer_reset();
                await new MessageDialog("", "時間到~").ShowAsync();
            }
        }

        /// <summary>
        /// 初始化計時器
        /// </summary>
        private void timeInit()
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);//new TimeSpan(0, 0, 1);
            time = 30;
        }
        /// <summary>
        /// 計時器重置
        /// </summary>
        private void timer_reset()
        {
            timer.Stop();
            Number.Text = "30";
            Number.Foreground = new SolidColorBrush(Colors.Red);
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
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
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


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            timer_start();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            timer_reset();
        }

    }
}
