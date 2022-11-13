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
using System.IO;

namespace SpeechToLateXAssistant
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            string result = "";
            string processedSpeecText = "";
            while (processedSpeecText.Equals("Terminate") != true)
            {
                processedSpeecText = ListenToSpeech().Result;
                Console.WriteLine(processedSpeecText);
                if (processedSpeecText.Contains("Terminate"))
                {
                    string filePath = "C:\\Users\\" + Environment.UserName + "\\Desktop\\MSCS\\latex.txt";
                    Console.WriteLine(filePath);
                    Console.ReadLine();
                    File.WriteAllText(filePath, result);
                    return;
                }
                LatexCommands latexCommands = new LatexCommands();
                ResponseModel response = GetLateXContentIntent(processedSpeecText).Result;
                Console.WriteLine(response.query);
                Console.WriteLine(response.prediction);
                if (response.prediction.topIntent == "LateX Operations")
                {
                    var entities = response.prediction.entities;
                    var operators = response.prediction.entities.Operator;
                    for (var i = 0; i < operators.Count; i++)
                    {
                        operators[i] = operators[i].ToLower();
                        operators[i] = operators[i].Replace(".", String.Empty);
                    }
                    int opLen = operators.Count;
                    int varLen = 0;
                    var var1 = "";
                    var var2 = "";
                    var var3 = "";
                    if (entities.Variable != null)
                    {
                        foreach (var op in entities.Variable)
                        {
                            Console.WriteLine("----------------------");
                            Console.WriteLine(op.Variable1 == null ? "" : op.Variable1[0]);
                            Console.WriteLine(op.Variable2 == null ? "" : op.Variable2[0]);
                            Console.WriteLine(op.Variable3 == null ? "" : op.Variable3[0]);
                            if (op.Variable1 != null)
                            {
                                var1 = op.Variable1[0];
                                varLen++;
                            }
                            if (op.Variable2 != null)
                            {
                                var2 = op.Variable2[0];
                                varLen++;
                            }
                            if (op.Variable2 != null)
                            {
                                var2 = op.Variable2[0];
                                varLen++;
                            }
                        }
                    }

                    if (opLen == 1 && varLen == 0)
                    {
                        result += latexCommands.actionToCommand[operators[0]] + "";
                    }
                    else if (opLen == 1)
                    {
                        result += latexCommands.actionToCommand[operators[0]] + "(" + var1 + ")";
                    }
                    else if (opLen == 2 && varLen == 3)
                    {
                        result += var1 + " " + latexCommands.actionToCommand[operators[0]] + " " + var2 + " " + latexCommands.actionToCommand[operators[1]] + " " + var3;
                    }
                }
                else
                {
                    result += response.query;
                }
                result += Environment.NewLine;
            }
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
            var predictionKey = "7610220b97b94a979b0c1c322afcbdcb";
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
