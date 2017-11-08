using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util;

namespace ReadEmails
{
	public class Program
	{
		private UserCredential _credential;
		static void Main(string[] args)
		{

			Program p = new Program();
			var test = p.GetCredential();
			Console.WriteLine("user id :" + test.Result.UserId);
			int m = p.getmessagecount();

		}

		public int getmessagecount()
		{
			var service = new GmailService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = _credential,
				ApplicationName = "ReadEmails",
			});
			UsersResource.MessagesResource.ListRequest getRequest = service.Users.Messages.List("me");
			getRequest.Q = "larger:1K";
			getRequest.MaxResults = 1000;
			var message = getRequest.Execute().Messages;
			Console.WriteLine("Total no of mails :" + message.Count);
			Console.WriteLine("Message Tag------");
			for (int i = 0; i < message.Count; i++)
			{
				var getmessage = service.Users.Messages.Get("me", message[i].Id);
				getmessage.Format=UsersResource.MessagesResource.GetRequest.FormatEnum.Metadata;
				getmessage.MetadataHeaders = new Repeatable<string>(
					new[] { "Subject", "Date", "From" });
				var messagedetails = getmessage.Execute();
				string from= messagedetails.Payload.Headers.FirstOrDefault(h => h.Name == "From").Value;
				string Subject = messagedetails.Payload.Headers.FirstOrDefault(h => h.Name == "Subject").Value;
				
				Console.WriteLine("Message From -" + from + "Subject Line-"+Subject);
			}
			Console.ReadLine();
			return message.Count;
		}

		public async Task<UserCredential> GetCredential()
		{
			var scopes = new[] { GmailService.Scope.GmailModify };
			var uri = new Uri("ms-appx:///client_id.json");
			var clientsec = new ClientSecrets();
			clientsec.ClientId = "140369219402-ho3acjvfm6aosfm9l1mb86r158c4q19q.apps.googleusercontent.com";
			clientsec.ClientSecret = "hPyM-JnlBHEBMQVb6m2LSNXi";

			_credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
				clientsec, scopes, "user", CancellationToken.None);
			return _credential;
		}




	}
}
