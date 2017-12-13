using MahApps.Metro.Controls;
using SimpleBlockChain.WalletUI.Events;
using SimpleBlockChain.WalletUI.Pages;
using SimpleBlockChain.WalletUI.Stores;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SimpleBlockChain.WalletUI
{
    public partial class MainWin : MetroWindow
    {
        private readonly HomePage _homePage;

        public MainWin(HomePage homePage)
        {
            _homePage = homePage;
            InitializeComponent();
            this.Loaded += Load;
            this.Closing += MetroNavigationWindow_Closing;
        }

        void Load(object sender, RoutedEventArgs e)
        {
            frame.Navigated += PART_Frame_Navigated;
            frame.Navigating += PART_Frame_Navigating;
            frame.NavigationFailed += PART_Frame_NavigationFailed;
            frame.NavigationProgress += PART_Frame_NavigationProgress;
            frame.NavigationStopped += PART_Frame_NavigationStopped;
            frame.LoadCompleted += PART_Frame_LoadCompleted;
            frame.FragmentNavigation += PART_Frame_FragmentNavigation;
            PART_BackButton.Click += PART_BackButton_Click;
            PART_ForwardButton.Click += PART_ForwardButton_Click;
            MainWindowStore.Instance().DisplayFlyoutEvt += DisplayFlyout;
        }

        private void DisplayFlyout(object sender, FlyoutEventArgs e)
        {
            flyoutsControl.Items.Clear();
            flyoutsControl.Items.Add(e.Data);
            e.Data.IsOpen = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.frame.Navigate(_homePage);
        }

        void PART_ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (CanGoForward)
                GoForward();
        }

        void PART_Frame_FragmentNavigation(object sender, FragmentNavigationEventArgs e)
        {
            if (FragmentNavigation != null)
                FragmentNavigation(this, e);
        }

        void PART_Frame_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (LoadCompleted != null)
                LoadCompleted(this, e);
        }

        void PART_Frame_NavigationStopped(object sender, NavigationEventArgs e)
        {
            if (NavigationStopped != null)
                NavigationStopped(this, e);
        }

        void PART_Frame_NavigationProgress(object sender, NavigationProgressEventArgs e)
        {
            if (NavigationProgress != null)
                NavigationProgress(this, e);
        }

        void PART_Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (NavigationFailed != null)
                NavigationFailed(this, e);
        }

        void PART_Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (Navigating != null)
                Navigating(this, e);
        }

        void PART_BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (CanGoBack)
                GoBack();
        }

        void MetroNavigationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            frame.FragmentNavigation -= PART_Frame_FragmentNavigation;
            frame.Navigating -= PART_Frame_Navigating;
            frame.NavigationFailed -= PART_Frame_NavigationFailed;
            frame.NavigationProgress -= PART_Frame_NavigationProgress;
            frame.NavigationStopped -= PART_Frame_NavigationStopped;
            frame.LoadCompleted -= PART_Frame_LoadCompleted;
            frame.Navigated -= PART_Frame_Navigated;
            PART_ForwardButton.Click -= PART_ForwardButton_Click;
            PART_BackButton.Click -= PART_BackButton_Click;
            this.Loaded -= Load;
            this.Closing -= MetroNavigationWindow_Closing;
        }
        
        void PART_Frame_Navigated(object sender, NavigationEventArgs e)
        {
            PART_Title.Content = ((Page)frame.Content).Title;
            PageContent = frame.Content;
            PART_BackButton.IsEnabled = CanGoBack;
            PART_ForwardButton.IsEnabled = CanGoForward;
            if (Navigated != null)
                Navigated(this, e);
        }

        public static readonly DependencyProperty OverlayContentProperty = DependencyProperty.Register("OverlayContent", typeof(object), typeof(MetroNavigationWindow));

        public object OverlayContent
        {
            get { return GetValue(OverlayContentProperty); }
            set { SetValue(OverlayContentProperty, value); }
        }

        public static readonly DependencyProperty PageContentProperty = DependencyProperty.Register("PageContent", typeof(object), typeof(MetroNavigationWindow));

        public object PageContent
        {
            get { return GetValue(PageContentProperty); }
            private set { SetValue(PageContentProperty, value); }
        }


        public IEnumerable ForwardStack { get { return frame.ForwardStack; } }
        public IEnumerable BackStack { get { return frame.BackStack; } }
        public NavigationService NavigationService { get { return frame.NavigationService; } }
        public bool CanGoBack { get { return frame.CanGoBack; } }
        public bool CanGoForward { get { return frame.CanGoForward; } }
        public Uri Source { get { return frame.Source; } set { frame.Source = value; } }
        public void AddBackEntry(CustomContentState state)
        {
            frame.AddBackEntry(state);
        }

        public JournalEntry RemoveBackEntry()
        {
            return frame.RemoveBackEntry();
        }
        
        public void GoBack()
        {
            frame.GoBack();
        }

        public void GoForward()
        {
            frame.GoForward();
        }
        
        public bool Navigate(Object content)
        {
            return frame.Navigate(content);
        }

        public bool Navigate(Uri source)
        {
            return frame.Navigate(source);
        }

        public bool Navigate(Object content, Object extraData)
        {
            return frame.Navigate(content, extraData);
        }

        public bool Navigate(Uri source, Object extraData)
        {
            return frame.Navigate(source, extraData);
        }

        public void StopLoading()
        {
            frame.StopLoading();
        }
        
        public event FragmentNavigationEventHandler FragmentNavigation;
        public event NavigatingCancelEventHandler Navigating;
        public event NavigationFailedEventHandler NavigationFailed;
        public event NavigationProgressEventHandler NavigationProgress;
        public event NavigationStoppedEventHandler NavigationStopped;
        public event NavigatedEventHandler Navigated;
        public event LoadCompletedEventHandler LoadCompleted;
    }
}
