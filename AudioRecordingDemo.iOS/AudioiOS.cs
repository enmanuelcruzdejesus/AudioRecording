using System;
using AudioRecordingDemo.iOS;
using AVFoundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioiOS))]
namespace AudioRecordingDemo.iOS
{
    public class AudioiOS : IAudio
    {
        public AudioiOS()
        {
        }

        public void PlayAndRecord()
        {
            var audioSession = AVAudioSession.SharedInstance();
            var err = audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);
        }

        public void PreparePlayBack()
        {
            AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Playback);
        }

        public void PrepareRecord()
        {
            AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Record);
        }
    }
}
