using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackyRSS
{
    public class User
    {

        public User (string username)
        {
            name = username;
            subscriptions = new List<RssChannel>();
            subIndex = 0;
        }

        public void loadChannel (RssChannel channel)
        {
            this.subscriptions.Add(channel);
        }
        public void removeChannel(RssChannel channel)
        {
            this.subscriptions.Remove(channel);
        }
        public void removeChannel(string link)
        {
            foreach (var channel in this.subscriptions)
            {
                if (channel.channelLink == link)
                {
                    subscriptions.Remove(channel);
                    return;
                }
            }
        }
        public string name { get; set; }
        public List<RssChannel> subscriptions { get; set; }
        public void saveUserInfo ()
        {
            //still under debate on whether or not to use a database for user information

        }

        public void loadUserInfo ()
        {

        }

        public int subIndex { get; set; }

    }
}
