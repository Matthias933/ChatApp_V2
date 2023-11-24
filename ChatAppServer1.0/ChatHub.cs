using ChatAppServer1._0.DataBase;
using ChatAppServer1._0.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer1._0
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, string> Users = new Dictionary<string, string>();

        public async Task<int> SendMessage(string senderName, string receiverName, string message)
        {
            //Get user from DB
            if (Users.TryGetValue(receiverName, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
                
                //Get Sender and Receiver from DB
                using(var context = new Context())
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

        public List<Chat> LoadChats(string receiverName, string currentUserName)
        {
            using (var context = new Context())
            {
                var currentUser = context.Users.Single(u => u.Name == currentUserName);
                var receiver = context.Users.Single(u => u.Name == receiverName);

                List<Chat> chats = new List<Chat>();


                //Get all chats between user
                context.Chats.Where(c => (c.SenderId == currentUser.UserId && c.ReceiverId == receiver.UserId) || (c.SenderId == receiver.UserId && c.ReceiverId == currentUser.UserId)).ToList()
                    .ForEach(chat => { chats.Add(chat); });

                return chats;
            };
        }

        public void RegisterUser(User user)
        {
            if (CheckUser(user) == 0)
            {
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
        }

        private int CheckUser(User user)
        {
            if (user.Email == null)
            {

                //Check if user really has an accoutn
                using (var context = new Context())
                {
                    var loginUser = context.Users
                         .FirstOrDefault(u => u.Name == user.Name && u.Password == user.Password);

                    if (loginUser != null)
                    {
                        return 0; //User does has an account SUCCESSFULL
                    }
                    else
                    {
                        return -1; // User does not has an account
                    }
                }
            }
            else if (user.Email != null)
            {
                using (var context = new Context())
                {
                    context.Users.Add(user);
                    context.SaveChanges();
                    return 0;
                }
            }
            return 0;
        }
    }
}
