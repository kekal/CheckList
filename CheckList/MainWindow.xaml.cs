using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CheckList
{
    public enum Status { Empty, Opened, InProgress, Completed };
    public partial class MainWindow
    {
        public static MainWindow Wm;
        //private TaskManager _taskManager;
        public ObservableCollection<Item> Custdata { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Wm = this;
            CSVEnvire.ReadCSV();
        }
        private void TextBoxSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Text = "";
        }
        private void TextBoxSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (((TextBox)sender).Text == "")
            {
                ((TextBox) sender).Text = "🔎 Search";
            }
        }
        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Wm == null) { return; }
            TaskManager.Add(null,
                delegate
                {
                    var content = ((TextBox)sender).Text;
                    if (content.Length < 1 || ((TextBox)sender).Text == "🔎 Search")
                    {
                        MainDataGrid.ItemsSource = Custdata;
                        return;
                    }
                    var filteredCollection = Custdata.Where(item => item.Description.Contains(content));
                    MainDataGrid.ItemsSource = filteredCollection;
                });
        }
        private void ButtonClearSearchBox_Click(object sender, RoutedEventArgs e)
        {
            TaskManager.Add(null,
                delegate
                {
                    SearchBox.Text = "🔎 Search";
                });
        }
        private void ButtonRereadSource_Click(object sender, RoutedEventArgs e)
        {
            MainDataGrid.ItemsSource = null;
            CSVEnvire.ReadCSV();
        }
        public static void InMainDispatch(Action dlg)
        {
            if (Thread.CurrentThread.Name == "Main Thread")
            {
                dlg();
            }
            else
            {
                Wm.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<string>(delegate { dlg(); }), "?");
            }
        }
    }
}
