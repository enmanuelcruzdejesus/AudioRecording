using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MediaManager;
using Newtonsoft.Json;
using Plugin.AudioRecorder;
using Xamarin.Forms;

namespace AudioRecordingDemo
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        AudioRecorderService recorder = new AudioRecorderService();
        AudioPlayer player = new AudioPlayer();
        string filePath = string.Empty;
        public MainPage()
        {
            
            recorder.TotalAudioTimeout = TimeSpan.FromSeconds(15);
            recorder.StopRecordingOnSilence = false;
            recorder.AudioInputReceived += Recorder_AudioInputReceived;

            InitializeComponent();
            this.btnStart.Clicked += async (s, e) =>
            { if (!recorder.IsRecording)
                {
                    this.btnStop.IsEnabled = true;
                    DependencyService.Get<IAudio>().PlayAndRecord();
                    await recorder.StartRecording();
                }
               
             
            };
            this.btnStop.Clicked += async (s, e) => { if (recorder.IsRecording) await recorder.StopRecording(); };
            this.btnPlay.Clicked += (s, e) =>
            {
                filePath = recorder.GetAudioFilePath();
                Console.WriteLine(filePath);
                player.Play(filePath);
                
               
            };
            this.player.FinishedPlaying +=  async(s, e) =>
            {
                filePath = recorder.GetAudioFilePath();
                if (!string.IsNullOrEmpty(filePath))
                {
                    this.btnStart.IsEnabled = true;
                    this.btnStart.BackgroundColor = Color.FromHex("#7cbb45");
                    this.btnPlay.IsEnabled = true;
                    this.btnPlay.BackgroundColor = Color.FromHex("#7cbb45");
                    this.btnStop.IsEnabled = false;
                    this.btnStop.BackgroundColor = Color.Silver;
                }
                else
                {
                    await DisplayAlert("Info", "No audio to play","OK");
                }
              
            };
            this.btnPlayFromServer.Clicked += async (s, e) =>
             {
                 var uri = "http://10.0.0.241/FileUploadDownload/api/FileUpload/audio";
                 using(var client = new HttpClient())
                 {
                     this.btnPlayFromServer.BackgroundColor = Color.Blue;
                     var response = await client.GetAsync(uri);
                     if (response.IsSuccessStatusCode)
                     {
                         var content = await response.Content.ReadAsStreamAsync();
                         var x = await CrossMediaManager.Current.Play(content,"audio.wav");
                     }



                     this.btnPlayFromServer.BackgroundColor = Color.Red;


                 }
                 
                
             };

        }

        private async void Recorder_AudioInputReceived(object sender, string e)
        {
           
            if (!string.IsNullOrEmpty(e))
            {
                var audio = recorder.GetAudioFileStream();

                //saving audio in documents folder

                using(var client = new HttpClient())
                {
                    var content = new MultipartFormDataContent();
                    content.Headers.ContentType.MediaType = "multipart/form-data";
                    content.Add(new StreamContent(audio),"files","audio.wav");
                    var response =  await client.PostAsync("http://10.0.0.241/FileUploadDownload/api/FileUpload/", content);

                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        var contentdata = await response.Content.ReadAsStringAsync();
                        var msg = JsonConvert.DeserializeObject<string>(contentdata);
                        Console.WriteLine("MESSAGE = {0}", msg);
                    }
                    
                }


            }

        }
    }
}
