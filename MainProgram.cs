using System;
using System.Collections;
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
using SpeechToTextAssistant;

namespace SpeechToLateXAssistant
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            // string processedSpeecText = ListenToSpeech().Result;
            // Console.WriteLine(processedSpeecText);

            ResponseModel response = GetLateXContentIntent("a plus b eqauls c").Result;
            Console.WriteLine(response.query);
            Console.WriteLine(response.prediction);
            if (response.prediction.topIntent == "LateX Operations")
            {
                var entities = response.prediction.entities;
               foreach (var op in entities.Variable)
                {
                    Console.WriteLine("----------------------");
                    Console.WriteLine(op.Variable1 == null ? "" : op.Variable1[0]);
                    Console.WriteLine(op.Variable2 == null ? "" : op.Variable2[0]);
                    Console.WriteLine(op.Variable3 == null ? "" : op.Variable3[0]);
                }

            }
            else
            {
                Console.WriteLine("Text");
            }
            var blah= Console.Read();
            Console.WriteLine(blah);
        }

        private static async Task<string> ListenToSpeech()
        {
            // move them to env variables later
            var key = "3bb96e80036d4050a22ecba1d2f3da46";
            var region = "eastus";
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

        private static async Task<ResponseModel> GetLateXContentIntent(string inputText)
        {
            // move them to env variables later
            var predictionKey = "d1c98d7dddfe4756992a962dc4659123";
            var predictionEndpoint = "https://speechtolatexluis.cognitiveservices.azure.com/luis/prediction/v3.0/apps/33147dce-30bc-4f7b-baa0-0e6551f3e2bc/slots/staging/predict?verbose=true&show-all-intents=true&log=true&subscription-key=d1c98d7dddfe4756992a962dc4659123&query=";

            var clientLUIS = new HttpClient();
            try
            {
                clientLUIS.DefaultRequestHeaders.Add("Ocp-Apin-Subscription-Key", predictionKey);
                var uri = string.Format(predictionEndpoint + "{0}", inputText);
                var response = await clientLUIS.GetAsync(uri);
                var result = await response.Content.ReadAsStringAsync();
                ResponseModel responseModel = JsonConvert.DeserializeObject<ResponseModel>(result);
                return responseModel;
            }
            catch (Exception e)
            {
                Console.WriteLine("Encountered some error");
                throw e;
            }
        }
    }
}
