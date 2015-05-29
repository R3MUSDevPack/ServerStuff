using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;

using HipchatApiV2;
using HipchatApiV2.Responses;
using HipchatApiV2.Enums;

namespace Hipchat_Plugin
{
    public class Hipchat
    {
        public static void SendToRoom(string message, string roomname, string token)
        {
            try
            {
                HipchatClient client = new HipchatClient(token);
                HipchatGetRoomResponse room = client.GetRoom(roomname);

                if (room != null)
                {
                    if (message.Contains("KILL"))
                    {
                        client.SendNotification(room.Id, message, RoomColors.Green, true, HipchatMessageFormat.Text);
                    }
                    else if (message.Contains("LOSS"))
                    {
                        client.SendNotification(room.Id, message, RoomColors.Red, true, HipchatMessageFormat.Text);
                    }
                    else
                    {
                        client.SendNotification(room.Id, message, RoomColors.Random, true, HipchatMessageFormat.Text);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void SendPM(string message, string username, string token)
        {
            try
            {
                HipchatClient client = new HipchatClient(token);
                HipchatGetAllUsersResponse users = client.GetAllUsers();

                foreach (HipchatUser user in users.Items)
                {
                    if (user.Name == username)
                    {
                        client.PrivateMessageUser(user.Id.ToString(), message, true, HipchatMessageFormat.Text);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static List<string> GetAllUsers(string token)
        {
            HipchatClient client = new HipchatClient(token);
            HipchatGetAllUsersResponse users = client.GetAllUsers();
            List<string> UserNames = new List<string>();

            foreach (HipchatUser user in users.Items)
            {
                UserNames.Add(user.Name);
            }

            return UserNames;
        }

        public static List<string> GetAllRooms(string token)
        {
            HipchatClient client = new HipchatClient(token);
            HipchatGetAllRoomsResponse rooms = client.GetAllRooms();
            List<string> RoomNames = new List<string>();

            foreach (HipchatGetAllRoomsResponseItems room in rooms.Items)
            {
                RoomNames.Add(room.Name);
            }

            return RoomNames;
        }
    }
}
