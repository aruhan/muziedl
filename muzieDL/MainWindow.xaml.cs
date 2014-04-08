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
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace muzieDL
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public Uri GetUrl(string url)
        {
            var uri = new Uri(url);
            var id = uri.Segments.Last();
            var player = new Uri(uri, "/js/player.php/" + id);

            using (var client = new HttpClient()) {
                var html = client.GetStringAsync(player).Result;
                var match = Regex.Match(html, @"""(.*[.]mp3)""", RegexOptions.None);
                return new Uri(uri, match.Groups[1].ToString());
            }
        }

        public void Download(Uri url, string filepath)
        {
            using (var client = new HttpClient())
            using (var file = System.IO.File.OpenWrite(filepath))
            using (var stream = client.GetStreamAsync(url).Result)
            {
                stream.CopyTo(file);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var url = this.BaseUrl.Text;
            var ret = GetUrl(url);

            var dlg = new SaveFileDialog() {
                FileName = ret.Segments.Last(),
                DefaultExt = ".mp3",
                Filter = "MP3 file (*.mp3)|*.mp3"
            };
            if (dlg.ShowDialog() == true)
            {
                Download(ret, dlg.FileName);
                MessageBox.Show("Download が完了しました");
            }

        }
    }
}
