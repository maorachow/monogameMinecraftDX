using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace monogameMinecraftNetworking.Utility
{
    public class UserAccessControllingManager
    {
        public List<string> bannedUsers;

        public bool CheckIsUsernameBanned(string username)
        {
            if (bannedUsers.Contains(username))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void BanUser(string username)
        {
            if (!bannedUsers.Contains(username))
            {
                bannedUsers.Add(username);
                Console.WriteLine("username successfully banned");
            }
            else
            {
                Console.WriteLine("username already banned");
            }
       
        }

        public List<string> GetBannedUsers()
        {
            return bannedUsers;
        }
        public void UnbanUser(string username)
        {
            if (bannedUsers.Contains(username))
            {
                bannedUsers.Remove(username);
                Console.WriteLine("username successfully unbanned");
            }
            else
            {
                Console.WriteLine("username not banned");
            }

        }
        public void ReadBannedUsersList(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("banned player list reading failed: file not exist");
                return;
            }
            string dataString;
            List<string> dataDeserialized;
            try
            {
                dataString = File.ReadAllText(filePath);
                dataDeserialized = JsonSerializer.Deserialize<List<string>>(dataString);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return;
            }
            bannedUsers=new List<string>(dataDeserialized);
        }

        public void SaveBannedUsersList(string filePath)
        {
            string path= Path.GetDirectoryName(filePath);
            string fileName= Path.GetFileName(filePath);
            if (path!=null&&!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string dataString=JsonSerializer.Serialize(bannedUsers);
            if (!File.Exists(filePath))
            {
                FileStream fs = new FileStream(filePath, FileMode.Create);
                fs.Close();
                
            }
            else
            {
                FileStream fs = new FileStream(filePath, FileMode.Truncate);
                fs.Close();
            }
            File.WriteAllText(filePath,dataString);

            
        }

        public UserAccessControllingManager(string filePath)
        {
            bannedUsers=new List<string>();
            ReadBannedUsersList(filePath);
        }
    }
}
