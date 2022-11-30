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
using System.Threading;

namespace SpeechToLateXAssistant
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            Console.Read();
            Console.WriteLine("Welcome to EasyTeX: Speech To LaTeX System\n");
            Thread.Sleep(1500);
            Console.WriteLine("Please speak your document line by line\n");
            Thread.Sleep(1500);
            Console.WriteLine("To end and save the work, please say the exit word 'Terminate'\n");
            Thread.Sleep(1500);
            Console.WriteLine("Results would be saved at the Desktop location as Results.TeX\n");
            Thread.Sleep(1500);
            Console.WriteLine("I'm listening, please speak...\n");
            Console.ReadLine();
            string result = "\\documentclass{article}\n";
            string processedSpeecText = "";
            while (processedSpeecText.Equals("Terminate") != true)
            {
                processedSpeecText = ListenToSpeech().Result.Replace(".", String.Empty);
                Console.WriteLine(processedSpeecText);
                if (processedSpeecText.Contains("Terminate"))
                {
                    string filePath = "C:\\Users\\" + Environment.UserName + "\\Desktop\\Results.tex";
                    File.WriteAllText(filePath, result);
                    return;
                }
                LatexCommands latexCommands = new LatexCommands();
                ResponseModel response = GetLateXContentIntent(processedSpeecText).Result;
                // Console.WriteLine(response.query);
                // Console.WriteLine(response.prediction);
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
                            // Console.WriteLine("----------------------");
                            // Console.WriteLine(op.Variable1 == null ? "" : op.Variable1[0]);
                            // Console.WriteLine(op.Variable2 == null ? "" : op.Variable2[0]);
                            // Console.WriteLine(op.Variable3 == null ? "" : op.Variable3[0]);
                            if (op.Variable1 != null)
                            {
                                var1 = op.Variable1[0].ToLower();
                                varLen++;
                            }
                            if (op.Variable2 != null)
                            {
                                var2 = op.Variable2[0].ToLower();
                                varLen++;
                            }
                            if (op.Variable3 != null)
                            {
                                var3 = op.Variable3[0].ToLower();
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
                        if (operators[0].Contains("section") || operators[0].Contains("title"))
                        {
                            result += latexCommands.actionToCommand[operators[0]] + "{" + var1 + "}";
                        }
                        else
                        {
                            result += latexCommands.actionToCommand[operators[0]] + "(" + var1 + ")";
                        }
                    }
                    else if (opLen == 2 && varLen == 3)
                    {
                        result += var1 + " " + latexCommands.actionToCommand[operators[0]] + " " + var2 + " " + latexCommands.actionToCommand[operators[1]] + " " + var3;
                    }
                    else
                    {
                        result += response.query;
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
            var key = Environment.GetEnvironmentVariable("SpeechToTextKey");
            var region = "eastus";
            var config = SpeechConfig.FromSubscription(key, region);
            config.SpeechRecognitionLanguage = "en-US";

            var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            var speechRecognizer = new SpeechRecognizer(config, audioConfig);

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
            var predictionKey = Environment.GetEnvironmentVariable("LuisKey");
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
