using System;
namespace AudioRecordingDemo
{
    public interface IAudio
    {
        void PlayAndRecord();
        void PrepareRecord();
        void PreparePlayBack();
    }
}
