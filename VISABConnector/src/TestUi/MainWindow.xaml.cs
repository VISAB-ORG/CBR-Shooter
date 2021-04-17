using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static VISABConnector.VISABApi api;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Get_Connection(object sender, RoutedEventArgs e)
        {
            api = VISABConnector.VISABApi.InitiateSession("TestGame");
            MessageBox.Show("Connection object received!");
        }

        private void Ping_Test(object sender, RoutedEventArgs e)
        {
            if (api != null)
                MessageBox.Show($"Is VISAB API running? {api.IsReachable}");
        }

        private void Statistics_Test(object sender, RoutedEventArgs e)
        {
            if (api != null)
            {
                var stats = new TestStatistics { Kills = 12, PlayerName = "SomePlayerName", SessionId = Guid.NewGuid() };
                api.SendStatistics(stats);
            }
        }
    }
}
