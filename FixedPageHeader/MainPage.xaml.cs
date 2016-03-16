using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FixedPageHeader.Resources;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Data;

namespace FixedPageHeader
{
    public partial class MainPage : PhoneApplicationPage
    {

        // Constructor

        private ApplicationBarIconButton _setAddAppBarIconBtn;
        public ObservableCollection<MessageModel> MessageItems { get; private set; }
        public MainPage()
        {
            InitializeComponent();

            _setAddAppBarIconBtn = new ApplicationBarIconButton
            {
                IconUri = new Uri("/Assets/save.png", UriKind.Relative),
                Text = "save"
            };
            _setAddAppBarIconBtn.Click += AddToList_Click;

            ApplicationBar.Buttons.Add(_setAddAppBarIconBtn);

            LoadMessage();
            this.Loaded += MainPage_Loaded;
        }

        private void LoadMessage()
        {
            MessageItems = new ObservableCollection<MessageModel>();

            for (int i = 55; i <= 150; i++)
            {
                int mType = i % 3 == 0 ? 3 : i % 3;
                MessageItems.Add(new MessageModel
                {
                    Message = i.ToString(),
                    MessageType = mType
                });
            }
            llsMessage.ItemsSource = MessageItems;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            BindToKeyboardFocus();
        }

        public void ScrollToLast()
        {
            try
            {
                if (llsMessage.ItemsSource.Count <= 0) return;

                var lastItem = llsMessage.ItemsSource[llsMessage.ItemsSource.Count - 1];
                llsMessage.ScrollTo(lastItem);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Scroll To Last : " + ex.StackTrace);
            }
        }

        private void AddToList_Click(object sender, EventArgs e)
        {
            var txt = messageBox.Text;
            if (!string.IsNullOrEmpty(txt))
            {
                messageBox.Text = "";
                MessageItems.Add(new MessageModel
                {
                    Message = txt,
                    MessageType = 2
                });
            }
            ScrollToLast();
        }        

        private void MessageBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            LayoutRoot.Margin = new Thickness();
        }



        #region toFixed Header

        private static double _newValue;

        private static readonly DependencyProperty TranslateYProperty = DependencyProperty.Register("TranslateY", typeof(double), typeof(MainPage), new PropertyMetadata(0d, OnRenderXPropertyChanged));

        private double TranslateY
        {
            get { return (double)GetValue(TranslateYProperty); }
        }

        private static void OnRenderXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((double)e.NewValue <= _newValue)
                ((MainPage)d).UpdateTopMargin((double)e.NewValue);
            _newValue = (double)e.NewValue;
        }

        private void BindToKeyboardFocus()
        {
            var frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame == null) return;
            var group = frame.RenderTransform as TransformGroup;
            if (@group == null) return;
            var translate = @group.Children[0] as TranslateTransform;
            var translateYBinding = new Binding("Y") { Source = translate };
            SetBinding(TranslateYProperty, translateYBinding);
        }

        private void UpdateTopMargin(double translateY)
        {
            double prevTopMargin = LayoutRoot.Margin.Top;
            LayoutRoot.Margin = new Thickness(0, -translateY, 0, 0);
        }

        #endregion

    }

    public class MessageModel : INotifyPropertyChanged
    {
        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }

        private int _messageType;
        public int MessageType
        {
            get
            {
                return _messageType;
            }
            set
            {
                _messageType = value;
                RaisePropertyChanged("MessageType");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}