using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Media.Imaging;

namespace TackyRSS
{
    public class RssChannel
    {
        //yc: this is the class to take apart an rss information
        //rss located via file path
        public RssChannel()
        {
            version = 0;
            channelTitle = "";
            channelLink = "";
            channelDescription = "";
            numItems = 0;
            updateFrequency = 60; // 60 minutes / 1 hour
            channelItems = new List<Story>();
            lastTime = DateTime.Now;
            latestFetchedStory = "";
            channelId = 0;
            //parseRawRSSFile(filepath);
        }

        //not done
        public RssChannel(string dbname)
        {

            version = 0;
            channelTitle = "";
            channelLink = "";
            channelDescription = "";
            numItems = 0;
            updateFrequency = 60; // 60 minutes / 1 hour
            channelItems = new List<Story>();
            lastTime = DateTime.Now;
            latestFetchedStory = "";
            channelId = 0;
        }
        //public RssReader(string url)
        //{
        //    version = 0;
        //    channelTitle = "";
        //    channelLink = url;
        //    channelDescription = "";
        //    numItems = 0;
        //    channelItems = new List<string>();
        //}
        public void parseUrlRSSFile(string url)
        {
            bool channelinfoloaded = false;
            this.channelLink = url;
            try
            {
                XElement site = XElement.Load(url);
                //string parentstring = site.Parent.ToString();
                foreach (var attribute in site.Attributes())
                {
                    if (attribute.Name == "version")
                        this.version = Convert.ToSingle(attribute.Value);
                }
                string channelString = site.FirstNode.ToString();
                foreach (var rssElement in site.Elements("channel"))
                {
                    //the channel should be loaded in rssElement, check for elements
                    if (rssElement.HasElements)
                    {
                        //first title and stuff should describe the channel information for rss atom1.0/cnn .com

                        foreach (var channelElement in rssElement.Elements())
                        {
                            //we've found an item!
                            if (channelElement.Name == "item")
                            {
                                string storyTitle = "";
                                string storyLink = "";
                                string storyDescription= "";
                                string storyDate = "";
                                channelinfoloaded = true;
                                foreach (var item in channelElement.Elements())
                                {
                                    //story information!
                                    if (item.Name == "title")
                                    {
                                        string[] desc = item.Value.Split(new char[] { '&', '<' });
                                        string value = desc[0].ToString();
                                        value = value.Replace("\"", "");
                                        value = value.Replace("'", "");
                                        storyTitle = value;
                                    }
                                    if (item.Name == "link")
                                        storyLink = item.Value;
                                    if (item.Name == "description")
                                    {
                                        //so lets clean up the description
                                        string[] desc = item.Value.Split(new char[] {'&', '<'});
                                        string value = desc[0].ToString();
                                        value = value.Replace("\"", "");
                                        value = value.Replace("'", "");
                                        value = value.Replace(",", "");
                                        storyDescription = value;
                                    }
                                    if (item.Name == "pubDate")
                                        storyDate = item.Value;
                                }

                                channelItems.Add(new Story(storyTitle, storyLink, storyDescription, storyDate));
                            }


                            //base channel info
                            if (channelinfoloaded == false)
                            {
                                if (channelElement.Name == "title")
                                {
                                    channelTitle = channelElement.Value.ToString();
                                }
                                if (channelElement.Name == "description")
                                {
                                    channelDescription = channelElement.Value.ToString();

                                }

                            }
                            
                        }
                    }
                    var locElement = rssElement.Elements();
                }
                this.numItems = channelItems.Count();
            }
            catch (Exception e)
            {

            }

            
        }
        
        //channel info
        public float version { get; set; }
        public string channelTitle { get; set; }
        public string channelLink { get; set; }
        public string channelDescription { get; set; }
        //story in side of channel info
        public int numItems { get; set; }
        public string channelLanguage { get; set; }
        public List<Story> channelItems { get; set; }
        public int updateFrequency { get; set; } //in minutes
        public string latestFetchedStory { get; set; }
        public DateTime lastTime { get; set; }
        public int channelId { get; set; }
    }
}
