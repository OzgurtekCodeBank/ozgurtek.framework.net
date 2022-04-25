using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ozgurtek.framework.test.xamarin.Managers
{
    public class Util
    {
        public void Speech(string text)
        {
            var settings = new SpeechOptions
            {
                Volume = .75f,
                Pitch = 1.0f,
            };

            TextToSpeech.SpeakAsync(text, settings)
                .ContinueWith((t) => { }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
