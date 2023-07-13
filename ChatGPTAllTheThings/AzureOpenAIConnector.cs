using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace ChatGPTAllTheThings
{
	internal class AzureOpenAIConnector
	{
		private const string SystemRole = "system";
		private const string UserRole = "user";

		private readonly Uri _url;
		private readonly string _apiKey;

		public AzureOpenAIConnector(string endpoint, string deployment, string apiKey)
		{
			_url = new Uri($"{endpoint}openai/deployments/{deployment}/chat/completions?api-version=2023-03-15-preview");
			_apiKey = apiKey;
		}

		public Response RequestChatCompletion(string prompt, string systemInstruction, IEnumerable<Message> historyMessages)
		{
			HttpClient client = new HttpClient();
			HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, _url);
			httpRequest.Headers.Add("api-key", _apiKey);

			Request request = new Request();
			Message existingSystemMessage = historyMessages?.FirstOrDefault(m => string.Equals(m.Role, SystemRole, StringComparison.OrdinalIgnoreCase));

			if (existingSystemMessage == null)
			{
				request.Messages.Add(new Message { Role = SystemRole, Content = systemInstruction });
			}

			if (historyMessages != null)
			{
				request.Messages.AddRange(historyMessages);
			}

			request.Messages.Add(new Message { Role = UserRole, Content = prompt });

			string messagesAsJson = JsonConvert.SerializeObject(request);

			StringContent content = new StringContent(messagesAsJson, Encoding.UTF8, "application/json");
			httpRequest.Content = content;

			HttpResponseMessage httpResponse = client.SendAsync(httpRequest).GetAwaiter().GetResult();
			Response response = null;

			if (httpResponse.IsSuccessStatusCode)
			{
				string value = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
				response = JsonConvert.DeserializeObject<Response>(value);
			}

			return response;
		}
	}

	internal class Request
	{
		[JsonProperty("messages")]
		public List<Message> Messages { get; } = new List<Message>();
	}

	internal class Response
	{
		[JsonProperty("choices")]
		public List<Choice> Choices { get; } = new List<Choice>();
	}

	internal class Message
	{
		[JsonProperty("role")]
		public string Role { get; set; }

		[JsonProperty("content")]
		public string Content { get; set; }
	}

	internal class Choice
	{
		[JsonProperty("message")]
		public Message Message { get; set; }
	}
}
