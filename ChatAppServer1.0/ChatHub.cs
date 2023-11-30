using ChatAppServer1._0.DataBase;
using ChatAppServer1._0.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer1._0
{
	public class ChatHub : Hub
	{
		private static readonly Dictionary<string, string> Users = new Dictionary<string, string>();

		public override async Task OnConnectedAsync()
		{
			Console.WriteLine("New device Connected to server");
			await base.OnConnectedAsync();
		}

		private string GetUserName()
		{
			var connectionId = Context.ConnectionId;

			return Users.FirstOrDefault(x => x.Value == connectionId).Key;
		}
		public async Task GetCurrentUser()
		{
			try
			{
				using (var context = new Context())
				{
					

					string currentUserName = GetUserName();

					if (currentUserName != null)
					{
						var user = context.Users.SingleOrDefault(u => u.Name == currentUserName);

						if (user != null)
						{
							Thread.Sleep(10);
							await Clients.Caller.SendAsync("GetCurrentUserCompleted", user);
						}
						else
						{
							await Clients.Caller.SendAsync("GetCurrentUserFailed", "User not found in the database");
						}
					}
					else
					{
						await Clients.Caller.SendAsync("GetCurrentUserFailed", "User name not found");
					}
				}
			}
			catch (Exception ex)
			{
				await Clients.Caller.SendAsync("GetCurrentUserFailed", $"An error occurred: {ex.Message}");
			}
		}


		public async Task<int> SendMessage(string receiverName, string message)
		{
			string senderName = GetUserName();
			//Get user from DB
			if (Users.TryGetValue(receiverName, out var connectionId))
			{
				await Clients.Client(connectionId).SendAsync("ReceiveMessage", message, senderName);

				//Get Sender and Receiver from DB
				using (var context = new Context())
				{
					var receiverUser = context.Users.Single(u => u.Name == receiverName);
					var senderUser = context.Users.Single(u => u.Name == senderName);

					Chat chat = new Chat
					{
						ReceiverId = receiverUser.UserId,
						SenderId = senderUser.UserId,
						Message = message,
						TimeStamp = DateTime.Now
					};

					context.Chats.Add(chat);
					await context.SaveChangesAsync();
				}
				return 0; //SUCCESSFULL
			}
			else
			{
				return -1; //ERROR Sending no user found
			}
		}

		public async Task SendGroupMessage(string groupName, string message)
		{
			await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
		}

		public void CreateGroup(List<string> selectedUsers)
		{
			string groupName = String.Format($"{selectedUsers.Count} Members");
			Groups.AddToGroupAsync(Context.ConnectionId, groupName).Wait();

			foreach (var user in selectedUsers)
			{
				if (Users.TryGetValue(user, out var connectionId))
				{
					Groups.AddToGroupAsync(connectionId, groupName).Wait();
				}
			}
		}

		public async void GetAllUsers()
		{
			using (var context = new Context())
			{
				await Clients.Caller.SendAsync("GetAllUserResult", context.Users.ToList());
			}
		}

		public List<Chat> LoadChats(string receiverName, string currentUserName)
		{
			using (var context = new Context())
			{
				var currentUser = context.Users.Single(u => u.Name == currentUserName);
				var receiver = context.Users.Single(u => u.Name == receiverName);

				List<Chat> chats = new List<Chat>();


				//Get all chats between user
				context.Chats.Where(c => (c.SenderId == currentUser.UserId && c.ReceiverId == receiver.UserId) || (c.SenderId == receiver.UserId && c.ReceiverId == currentUser.UserId))
					.OrderBy(c => c.TimeStamp)
					.ToList()
					.ForEach(chat => { chats.Add(chat); });

				return chats;
			};
		}

		public void LoginUser(User user)
		{
			Console.WriteLine("User tries to register");

			var connectionID = Context.ConnectionId; //Gets connection id for new User
			if (Users.ContainsKey(user.Name))
			{
				Users[user.Name] = connectionID;
			}
			else
			{
				Users.Add(user.Name, connectionID);
			}
		}

		//Return value > 0 success else not successful
		public int RegisterUser(User user)
		{
			using (var context = new Context())
			{
				context.Users.Add(user);
				var ret = context.SaveChanges();
				return ret;
			}
		}
	}
}
