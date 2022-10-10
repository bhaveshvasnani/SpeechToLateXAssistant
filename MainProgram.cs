using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Authoring;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Authoring.Models;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Newtonsoft.Json;

namespace SpeechToLateXAssistant
{
    class MainProgram
    {
        static string key = "key";
        static string region = "region";

        static void Main(string[] args)
        {
            string processedSpeecText = ListenToSpeech().Result;
            var intents = GetLateXContentIntent(processedSpeecText).Result;
        }

        private static async Task<string> ListenToSpeech()
        {
            var config = SpeechConfig.FromSubscription(key, region);
            config.SpeechRecognitionLanguage = "en-US";

            var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            var speechRecognizer = new SpeechRecognizer(config, audioConfig);

            Console.WriteLine("I'm listening, please speak...");
            var recognizedResult = await speechRecognizer.RecognizeOnceAsync();
            if (recognizedResult.Reason == ResultReason.RecognizedSpeech) {
                return recognizedResult.Text;
            }
            else {
                return "Couldn't understand, please try again";
            }
        }

        private static async Task<string> GetLateXContentIntent(string inputText)
        {
            var predictionKey = "blah";
            var predictionEndpoint = "blah";

            var clientLUIS = new HttpClient();
            try
            {
                clientLUIS.DefaultRequestHeaders.Add("Ocp-Apin-Subscription-Key", predictionKey);
                var uri = String.Format("luis endpoint url", predictionEndpoint, "SpeechToLateXAssistant", inputText);
                var response = await clientLUIS.GetAsync(uri);
                var lateXIntent = await response.Content.ReadAsStringAsync();
                return lateXIntent;
            }
            catch (Exception e)
            {
                Console.WriteLine("Encountered some error");
                throw e;
            }
        }
    }
}
