using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    public class Speaker 
    {
        System.Speech.Synthesis.SpeechSynthesizer speechSynthesizer =
            new System.Speech.Synthesis.SpeechSynthesizer();

        public void Speak(string speakWords)
        {
            speechSynthesizer.Speak(speakWords);
        }


        public void SpeakAsync(string speakWords)
        {
            //System.Threading.ThreadPool.QueueUserWorkItem()
            System.Speech.Synthesis.Prompt prompt =
                new System.Speech.Synthesis.Prompt(speakWords);
            speechSynthesizer.SpeakAsync(prompt);
        }
    }
}
