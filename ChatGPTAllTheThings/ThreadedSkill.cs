using System;
using System.Linq;
using System.Threading;
using MistyRobotics.Common.Types;
using MistyRobotics.SDK.Messengers;
using MistyRobotics.Common.Data;

namespace ChatGPTAllTheThings
{
	internal sealed class ThreadedSkill
	{
		private const string AzureCognitiveServicesRegion = "Add Your Cognitive Services Speech Region";
		private const string AzureCognitiveServicesKey = "Add Your Cognitive Services Speech Key";

		private const string AzureOpenAiKey = "Add Your Open AI Key";
		private const string AzureOpenAiEndpoint = "Add Your Open AI End Point";
		private const string DeploymentName = "Add Your Open AI Model Deployment Name";

		private const string VoiceName = "en-US-JennyNeural";
		private const string Language = "en-US";

		private readonly IRobotMessenger _misty;

		public ThreadedSkill(IRobotMessenger misty)
		{
			_misty = misty;
		}

		public void Start()
		{
			
			// Start from a known head location, and with a known expression
			_misty.DisplayImage("e_defaultcontent.jpg", null, false, null);
			_misty.MoveHead(0, 0, 0, 85, AngularUnit.Position, null);

			SpeakAndWait("Ask me a question");

			// This gets the string that the robot heard
			string capturedText =  ListenAndWait();

			// Look up and to the right
			_misty.MoveHead(-30, 20, -50, 85, AngularUnit.Position, null);

			AzureOpenAIConnector connector = new AzureOpenAIConnector(AzureOpenAiEndpoint, DeploymentName, AzureOpenAiKey);
			Response response = connector.RequestChatCompletion(capturedText, "You provide short, polite responses to queries.", null);

			if (response == null || !response.Choices.Any())
			{
				SpeakAndWait("Sorry, I guess I didn't get a response.");
			}
			else
			{
				SpeakAndWait(response.Choices[0].Message.Content);
			}

			_misty.DisplayImage("e_joy.jpg", null, false, null);
			_misty.SkillCompleted();
			
		}

		private void SpeakAndWait(string text)
		{
			AutoResetEvent are1 = new AutoResetEvent(false);

			// This callback gets invoked when the speech synthesis completes, but only if utteranceId is provided
			_misty.RegisterTextToSpeechCompleteEvent((e) =>
			{
				// Release the wait handle
				are1.Set();
			}, 0, false, "tts-content-complete", null);

			_misty.SpeakAzure(text, AzureCognitiveServicesKey, AzureCognitiveServicesRegion, Language, VoiceName, Guid.NewGuid().ToString(), false, true, false, 0, null);

			// Wait for access to the handle
			are1.WaitOne();
		}

		private string ListenAndWait()
		{
            AutoResetEvent are = new AutoResetEvent(false);
            string capturedText = string.Empty;

            _misty.RegisterVoiceRecordEvent((e) =>
            {
                capturedText = e.SpeechRecognitionResult;
                are.Set();
            }, 0, false, "capture-speech-complete", null);

         _misty.CaptureSpeechAzure(false, 6000, 1000, AzureCognitiveServicesKey, AzureCognitiveServicesRegion, Language, null);
            are.WaitOne();
            return capturedText;
		}
	}
}
