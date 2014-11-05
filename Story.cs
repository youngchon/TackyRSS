using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackyRSS
{
    public class Story
    {
        public string storyTitle { get;set;}
        public string storyLink { get; set; }
        public string storyDescription { get; set; }
        public string storyDate { get; set; }
        public bool read { get; set; }
        public bool hasCity { get; set; }
        public bool favorited { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }

        public int channelId { get; set; }
        //Every stroy must have at least the following
        public Story (string title, string url)
        {
            storyTitle = title;
            storyLink = url;
            storyDescription = "";
            storyDate = "";
            read = false;
            hasCity = false;
            favorited = false;
            city = "";
            state = "";
            country = "";
            channelId = 0;
        }
        public Story(string title, string url, string description, string date)
        {
            storyTitle = title;
            storyLink = url;
            storyDescription = description;
            storyDate = date;
            read = false;
            hasCity = false;
            favorited = false;
            city = "";
            state = "";
            country = "";
            channelId = 0;
        }
        public Story(string title, string cid, string url, string description, string date, string isread)
        {
            storyTitle = title;
            storyLink = url;
            storyDescription = description;
            storyDate = date;
            if (isread == "1")
                read = true;
            else
                read = false;
            hasCity = false;
            favorited = false;
            city = "";
            state = "";
            country = "";
            channelId = Convert.ToInt32(cid);
        }
    }
}
