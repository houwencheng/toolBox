using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    public static class Speaker
    {
        /// <summary>
        /// 朗读
        /// </summary>
        /// <param name="speakWords"></param>
        public static void Speak(string speakWords)
        {
            SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
            //speechSynthesizer.SetOutputToDefaultAudioDevice();
            speechSynthesizer.Speak(speakWords);
        }

        /// <summary>
        /// 朗读
        /// </summary>
        /// <param name="speakWords"></param>
        public static void Speak(string speakWords, int speed)
        {
            SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
            //speechSynthesizer.SetOutputToDefaultAudioDevice();
            speechSynthesizer.Rate = speed;
            speechSynthesizer.Speak(speakWords);
        }

        /// <summary>
        /// 朗读
        /// </summary>
        /// <param name="speakWords"></param>
        public static void Speak(List<Voice> voices)
        {
            voices.ForEach(x =>
            {
                System.Threading.Thread.Sleep(x.Delay);
                Speak(x.Txt, x.Speed);
            });
        }

        /// <summary>
        /// 朗读
        /// </summary>
        /// <param name="speakWords"></param>
        public static void SpeakAsync(string speakWords)
        {
            SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.SpeakAsync(new Prompt(speakWords));
        }
    }

    public class Voice
    {
        public int Delay { get; set; }
        public int Speed { get; set; }
        public string Txt { get; set; }
    }
}
