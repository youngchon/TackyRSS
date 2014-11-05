using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl.WPF;
using System.Drawing;
using LINQtoCSV;

namespace TackyRSS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class cityList
    {
        [CsvColumn(Name = "ID", FieldIndex = 1)]
        public int cid { get; set; }
        [CsvColumn(Name = "Country", FieldIndex = 2)]
        public string country { get; set; }
        [CsvColumn(Name = "State", FieldIndex = 3)]
        public string state { get; set; }
        [CsvColumn(Name = "City", FieldIndex = 4)]
        public string city { get; set; }
        [CsvColumn(Name = "PostalCode", FieldIndex = 5)]
        public string zip { get; set; }
        [CsvColumn(Name = "Latitude", FieldIndex = 6)]
        public float lat { get; set; }
        [CsvColumn(Name = "Longitude", FieldIndex = 7)]
        public float lng { get; set; }
        [CsvColumn(Name = "MetroCode", FieldIndex = 8)]
        public int metr { get; set; }
        [CsvColumn(Name = "AreaCode", FieldIndex = 9)]
        public int area { get; set; }
    }
    /// <summary>
    /// this encapsulates all data that will be used for teh viewer
    /// </summary>
    public partial class Pushpin : Microsoft.Maps.MapControl.WPF.Pushpin
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }

    }
    public partial class MainWindow : Window
    {
        RssChannel tester = new RssChannel();
        User currentUser;
        public SQLWrapper sqlwrap = new SQLWrapper();
        //  DispatcherTimer setup
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public CsvContext cities = new CsvContext();
        IEnumerable<cityList> cList ;
 //       Backgroundworker cityfinder = new Backgroundworker();

        public CsvFileDescription inputFileDescription = new CsvFileDescription
        {
            SeparatorChar = ',',
            FirstLineHasColumnNames = true
        };

        public MainWindow()
        {
            InitializeComponent();
            //initalize the system
            cList = cities.Read<cityList>(@"..\..\uslocations.csv", inputFileDescription); //load cities
            //then wait for an event


        }

        //this function adds a new link to the subscriptions list
       
        public void loadStoryTitleBox(List<Story> feed)
        {
            int numStories = feed.Count();
            int i = 0;
            StoryTitlesBox.SelectedIndex = -1;
            StoryTitlesBox.Items.Clear();
            foreach(Story article in feed)
            {
                i++;
                ListBoxItem item = new ListBoxItem();
                item.Content = article.storyTitle;
                if (!article.read)
                    item.FontWeight = FontWeights.Bold;
                else
                    item.FontWeight = FontWeights.Normal;
                StoryTitlesBox.Items.Add(item);

                loadProgressBar.Value = ((double)i / (double)numStories) * 100;
            }
        }
        /// <summary>
        /// if a story is selected. this event must update the gui
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StoryTitlesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if( StoryTitlesBox.SelectedIndex != -1)
            {
                //updates! change the text to not bold and change it from read to unread
                ListBoxItem item = (ListBoxItem)StoryTitlesBox.Items.GetItemAt(StoryTitlesBox.SelectedIndex);
                item.FontWeight = FontWeights.Normal;
                //update the hasread portion.
                //
                sqlwrap.updateHadRead(currentUser.subscriptions[currentUser.subIndex].channelItems[StoryTitlesBox.SelectedIndex], currentUser);
                
                StoryDescriptionBox.Text = currentUser.subscriptions[currentUser.subIndex].channelItems[StoryTitlesBox.SelectedIndex].storyDescription;//tester.channelItems[StoryTitlesBox.SelectedIndex].storyDescription;
                tackyrssbrowser.Navigate(currentUser.subscriptions[currentUser.subIndex].channelItems[StoryTitlesBox.SelectedIndex].storyLink);
            }
        }

        private void menuLogin_Click(object sender, RoutedEventArgs e)
        {
            Login dlg = new Login();
            dlg.Owner = this;
            dlg.ShowDialog();
            if (dlg.newuser)
            {
                //create the user then login after this portion of code
                if (!sqlwrap.createUser(dlg.username,dlg.password))
                {
                    MessageBox.Show("A user already exists of that name, please try again");
                    return;
                }
                else
                {
                    //create tables for the user
                    sqlwrap.createUserTables(dlg.username);
                }
                
            }

            //call the sql server
            if (sqlwrap.checkUser(dlg.username, dlg.password))
            {
                //login sucsessful
                //load the user information up!
                currentUser = new User(dlg.username);
                //user class is now created
                //get all teh subscriptions.
                List<string>[] subscriptioninfo = sqlwrap.getUserSubscriptions(dlg.username);
                
                if (subscriptioninfo[0].Count == 0)
                {
                    //nothing in here
                }
                else if (subscriptioninfo[0].Count > 0)
                {
                    //the user needs to get this channel information
                    for (int i = 0; i < subscriptioninfo[0].Count; i++)
                    {
                        //for each sub we need to geteverything
                        //i is the current sub
                        RssChannel newChan = new RssChannel();
                        newChan.channelLink = subscriptioninfo[0][i];
                        newChan.channelId = Convert.ToInt32(subscriptioninfo[1][i]);
                        newChan.channelTitle = subscriptioninfo[2][i];
                        newChan.version = Convert.ToSingle(subscriptioninfo[3][i]);
                        newChan.updateFrequency = Convert.ToInt32(subscriptioninfo[4][i]);
                        newChan.latestFetchedStory = subscriptioninfo[5][i]; //this is redundant. going to remove soon
                        // pulled basic channel information from  the server.
                        // get an updated version on this channel
                        RssChannel updated = new RssChannel();
                        updated.parseUrlRSSFile(newChan.channelLink);
                        updated.channelId = newChan.channelId;
                        updated.updateFrequency = newChan.updateFrequency;

                        sqlwrap.updateSubscriptionStories(updated, currentUser);

                        //get the user stories now
                        newChan.channelItems = sqlwrap.getUserStories(newChan.channelId, currentUser);

                        currentUser.subscriptions.Add(newChan);

                    }
                    ///
                    //update the viewer
                    List<string> subLinkList = sqlwrap.getColumn(subscriptioninfo, 3); 
                    foreach (string title in subLinkList)
                    {
                        ListBoxItem item = new ListBoxItem();
                        item.Content = title;
                        ChannelBox.Items.Add(item);
                    }
                }

                //user loading should be done
                //update the gui
            }
            else
            {
                MessageBox.Show("INCORRECT LOGIN AND PW");
            }

            dispatcherTimer.Tick += new EventHandler(timerupdate);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0); //every minute
            dispatcherTimer.Start();

        }
        public void timerupdate(object sender, EventArgs e)
        {
            foreach (RssChannel chan in currentUser.subscriptions)
            {
                if (chan.lastTime.Add(new TimeSpan(0,chan.updateFrequency, 0)) <= DateTime.Now)
                {
                    //this is overdue!
                    chan.channelItems.Clear();
                    chan.parseUrlRSSFile(chan.channelLink);
                }
            }
        }

        private void ChannelBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //the selected item has changed
            //find and load the information into the box!
            //List<Story> selected = sqlwrap.getUserStories(sqlwrap.getCid(ChannelBox.SelectedItem.ToString(), currentUser), currentUser);
            if (ChannelBox.SelectedIndex != -1)
            {
                loadStoryTitleBox(currentUser.subscriptions[ChannelBox.SelectedIndex].channelItems);
                currentUser.subIndex = ChannelBox.SelectedIndex;
                //load the info box
                updateChannelInfoBox();
            }

        }
        public void updateChannelInfoBox()
        {
            if (currentUser.subIndex != -1)
            {
                ChannelInformationBox.Items.Clear();
                ListBoxItem info = new ListBoxItem();
                info.Content = currentUser.subscriptions[currentUser.subIndex].channelTitle;
                ChannelInformationBox.Items.Add(info);
                info = new ListBoxItem();
                info.Content = currentUser.subscriptions[currentUser.subIndex].channelLink;
                ChannelInformationBox.Items.Add(info);
                info = new ListBoxItem();
                info.Content = "Version = " + currentUser.subscriptions[currentUser.subIndex].version.ToString();
                ChannelInformationBox.Items.Add(info);
                info = new ListBoxItem();
                info.Content = "Update Frequency = " + currentUser.subscriptions[currentUser.subIndex].updateFrequency.ToString();
                ChannelInformationBox.Items.Add(info);
            }
        }

        private void menuChannelAdd_Click(object sender, RoutedEventArgs e)
        {
            AddChannel dlg = new AddChannel();
            dlg.Owner = this;
            dlg.ShowDialog();

            if (dlg.url == "")
                MessageBox.Show("INVALID URL!");
            else
            {
                if (currentUser != null)
                {
                    RssChannel loadee = new RssChannel();
                    loadee.parseUrlRSSFile(dlg.url);
                    ListBoxItem item = new ListBoxItem();
                    item.Content = loadee.channelTitle;
                    ChannelBox.Items.Add(item);
                    sqlwrap.loadStories(loadee, currentUser);
                }
                else
                {
                    MessageBox.Show("NO ONE IS SIGNED IN!");
                    return;
                }
                //add this tot he list of channels
                currentUser.subIndex = currentUser.subscriptions.IndexOf(currentUser.subscriptions.Last());
                loadStoryTitleBox(currentUser.subscriptions.Last().channelItems);
            }
        }
        public void addTack(float lat, float lng, Story article)
        {
            Image tackImg = new Image();
            MapLayer imageLayer = new MapLayer();
            tackImg.Height = 25;

            BitmapImage bmpSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\..\..\tack.png", UriKind.Absolute));

            tackImg.Source = bmpSource;
            tackImg.Opacity = 0.7;
            Location loc = new Location() { Latitude = lat, Longitude = lng };
            PositionOrigin pos = PositionOrigin.Center;
            Pushpin tack = new Pushpin();
            tack.Location = loc;
            tack.Description = article.storyDescription;
            tack.Link = article.storyLink;
            tack.Title = article.storyTitle;
            tack.MouseLeftButtonDown += tack_MouseLeftButtonDown;
            tack.MouseRightButtonDown += tack_MouseRightButtonDown;
            tack.MouseRightButtonUp += tack_MouseRightButtonUp;
            ToolTipService.SetToolTip(tack, new ToolTip()
            {
                DataContext = tack,
                Style = Application.Current.Resources["CustomInfoboxStyle"] as Style
            });
            tackyMap.Children.Add(tack);
        }

        void tack_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            wintab.SelectedIndex = 1;
        }

        void tack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tackyrssbrowser.Navigate(((Pushpin)sender).Link);
            
        }

        void tack_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            tackyMap.Children.RemoveAt(tackyMap.Children.IndexOf(((Pushpin)sender)));
        }



        void tack_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            
        }
        public bool locateCities(string description)
        {
            //for each word in string description
            string[] word= description.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
            for (int i = 0; i< word.Count(); i++)
            {
                loadProgressBar.Value = ((double)i / (double)word.Count()) * 100;
                string potentialCity = word[i].ToLower() ;
                //see if 
                string twoWord = word[i].ToLower() + " " + word[i + 1].ToLower();
                float[] tw = cityLookUp(twoWord);
                if (tw[0] != 0.0f || tw[1] != 0.0f)
                {
                    MessageBox.Show("Found " + twoWord);
                    addTack(tw[0], tw[1], currentUser.subscriptions[currentUser.subIndex].channelItems[StoryTitlesBox.SelectedIndex]);
                    return true;
                }
                switch (potentialCity)
                {
                    
                    //abvevations.

                    case "nyc":
                        float[] nyc = cityLookUp("New York");
                        if (nyc[0] != 0.0f || nyc[1] != 0.0f)
                        {
                            MessageBox.Show("Found New York");
                            addTack(nyc[0], nyc[1], currentUser.subscriptions[currentUser.subIndex].channelItems[StoryTitlesBox.SelectedIndex]);
                            return true;
                        }
                        break;
                    case "l.a.":
                        float[] la = cityLookUp("los angeles");
                        if (la[0] != 0.0f || la[1] != 0.0f)
                        {
                            MessageBox.Show("Found los angeles" + potentialCity);
                            addTack(la[0], la[1], currentUser.subscriptions[currentUser.subIndex].channelItems[StoryTitlesBox.SelectedIndex]);
                            return true;
                        }
                        break;
                    case "d.c":
                        float[] dc = cityLookUp("washington dc");
                        if (dc[0] != 0.0f || dc[1] != 0.0f)
                        {
                            MessageBox.Show("Found our nations capitol" + potentialCity);
                            addTack(dc[0], dc[1], currentUser.subscriptions[currentUser.subIndex].channelItems[StoryTitlesBox.SelectedIndex]);
                            return true;
                        }
                        break;
                    default:
                        float[] something = cityLookUp(potentialCity);
                        if (something[0] != 0.0f || something[1] != 0.0f)
                        {
                            MessageBox.Show("Found " + potentialCity);
                            addTack(something[0], something[1], currentUser.subscriptions[currentUser.subIndex].channelItems[StoryTitlesBox.SelectedIndex]);
                            return true;
                        }
                        break;
                }
            }
            return false;
        }
        public float[] cityLookUp(string city)
        {
            float[] coord = new float[2];
            foreach (cityList c in cList)
            {
                if (c.city.ToLower() == city)
                {
                    coord[0] = c.lat;
                    coord[1] = c.lng;
                    return coord;
                }

            }
            coord[0] = 0.0f;
            coord[1] = 0.0f;
            return coord;
        }
        private void menuChannelRemove_Click(object sender, RoutedEventArgs e)
        {
            //call remove channel
            sqlwrap.removeChannel(currentUser.subscriptions[currentUser.subIndex], currentUser);
            //recall the server to load updated subscriptions list
            List<string>[] subscriptioninfo = sqlwrap.getUserSubscriptions(currentUser.name);
            currentUser.subscriptions.Clear();
            if (subscriptioninfo[0].Count == 0)
            {
                currentUser.subIndex = -1;
                StoryDescriptionBox.Text = "";
                StoryTitlesBox.Items.Clear();
                ChannelBox.Items.Clear();
                return;
            }
            else if (subscriptioninfo[0].Count > 0)
            {
                //the user needs to get this channel information
                for (int i = 0; i < subscriptioninfo[0].Count; i++)
                {
                    //for each sub we need to geteverything
                    //i is the current sub
                    RssChannel newChan = new RssChannel();
                    newChan.channelLink = subscriptioninfo[0][i];
                    newChan.channelId = Convert.ToInt32(subscriptioninfo[1][i]);
                    newChan.channelTitle = subscriptioninfo[2][i];
                    newChan.version = Convert.ToSingle(subscriptioninfo[3][i]);
                    newChan.updateFrequency = Convert.ToInt32(subscriptioninfo[4][i]);
                    newChan.latestFetchedStory = subscriptioninfo[5][i]; //this is redundant. going to remove soon
                    // pulled basic channel information from  the server.
                    // get an updated version on this channel
                    RssChannel updated = new RssChannel();
                    updated.parseUrlRSSFile(newChan.channelLink);
                    updated.channelId = newChan.channelId;
                    updated.updateFrequency = newChan.updateFrequency;

                    sqlwrap.updateSubscriptionStories(updated, currentUser);

                    //get the user stories now
                    newChan.channelItems = sqlwrap.getUserStories(newChan.channelId, currentUser);

                    currentUser.subscriptions.Add(newChan);

                }
                ///
                //update the viewer
                ChannelBox.Items.Clear();
                StoryTitlesBox.Items.Clear();
                List<string> subLinkList = sqlwrap.getColumn(subscriptioninfo, 3);
                foreach (string title in subLinkList)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = title;
                    ChannelBox.Items.Add(item);
                }
            }

        }

        private void menuChannelUpdate_Click(object sender, RoutedEventArgs e)
        {
            //change the update freqency of a selected object
            updatefrequency dlg = new updatefrequency();
            dlg.Owner = this;
            dlg.ShowDialog();

            if (dlg.time == "")
                MessageBox.Show("invalid time");
            else
            {
                if (currentUser.subIndex != -1)
                {
                    sqlwrap.channelUpdateFrequency(currentUser.subscriptions[currentUser.subIndex], currentUser, Convert.ToInt32(dlg.time));
                    updateChannelInfoBox();
                }
            }
        }

        private void menuLougout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuStoryMap_Click(object sender, RoutedEventArgs e)
        {

            if (StoryTitlesBox.SelectedIndex != -1)
            {
               if (locateCities(currentUser.subscriptions[currentUser.subIndex].channelItems[StoryTitlesBox.SelectedIndex].storyDescription))
               {
                   wintab.SelectedIndex = 0;
                   loadProgressBar.Value = 1;
               }
               else
               {
                   MessageBox.Show("Could not find city");
                   loadProgressBar.Value = 1;
               }
            }
            else
            {
                MessageBox.Show("Story Not Selected");
                loadProgressBar.Value = 1;
            }
        }

        private void menuStoryUnread_Click(object sender, RoutedEventArgs e)
        {
            if (StoryTitlesBox.SelectedIndex != -1)
            {
                currentUser.subscriptions[currentUser.subIndex].channelItems[StoryTitlesBox.SelectedIndex].read = false;
                sqlwrap.updateHadUnRead(currentUser.subscriptions[currentUser.subIndex].channelItems[StoryTitlesBox.SelectedIndex], currentUser);
                loadStoryTitleBox(currentUser.subscriptions[currentUser.subIndex].channelItems);
            }
        }

        private void tackyrssbrowser_Navigated(object sender, NavigationEventArgs e)
        {
            wintab.SelectedIndex = 1;
        }


    }
}
