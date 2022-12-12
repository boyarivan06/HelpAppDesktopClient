using System;
using System.IO;
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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HelpAppDesktopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    class HelpRequest
    {
        string author { get; set; }
        string title { get; set; }
        string text { get; set; }
        DateTime time_created { get; }
        int id { get; }
        HelpRequest(int _id, string _author, string _title, string _text, DateTime _time_created)
        {
            id = _id;
            author = _author;
            title = _title;
            text = _text;
            time_created = _time_created;
        }
    }

    public partial class MainWindow : Window
    {
        static HttpClient client = new HttpClient();

        public static HttpClient Client { get => client; set => client = value; }

        public MainWindow()
        {
            InitializeComponent();
            API_init();
        }

        static void Log(string text)
        {
            if (!File.Exists("logs.txt"))
            {
                File.WriteAllText("logs.txt", "-- start of logs --" + Environment.NewLine);
            }
            File.AppendAllText("logs.txt", $"{text} - {DateTime.Now}{Environment.NewLine}");
        }

        static async Task<string> GetRequestsAsync()
        {
            string ret = String.Empty;
            HttpResponseMessage response;
            Log("GetRequestAsync starts");
            try
            {
                response = await Client.GetAsync("api/help_requests");
                Log("response returned");
            }
            catch (HttpRequestException e)
            {
                Log($"HttpRequest exception, {e.Message}");
                ret = e.Message;
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            catch (TaskCanceledException e)
            {
                Log($"Task cancelled, {e.Message}");
                ret = e.Message;
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            catch (InvalidOperationException e)
            {
                Log($"Invalid Operation Exception,{e.Message}");
                ret = e.Message;
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            if (response.IsSuccessStatusCode)
            {
                Log("Success");
                //help = await response.Content.ReadAsAsync<HelpRequest>();
                ret = await response.Content.ReadAsStringAsync();
                Log($"Returns {ret}");
            }
            return ret;
        }

        static void API_init()
        {
            Client.BaseAddress = new Uri("http://127.0.0.1:5000/");
            //Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        private void GetButton_Click(object sender, RoutedEventArgs e)
        {
            string text = String.Empty;
            try
            {
                text = GetRequestsAsync().Result;
            }
            catch (AggregateException exc) {
                text = "AggregateException";
            }
            text = GetRequestsAsync().Result;
            showBox.Text = text;
        }
    }
}
