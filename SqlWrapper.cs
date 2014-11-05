using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using MySql.Data.Common;

namespace TackyRSS
{
    public class SQLWrapper
    {
        private MySqlConnection connection { get; set; }
        private string server { get; set; }
        private string db { get; set; }
        private string uid { get; set; }
        private string pw{ get; set;}
        
        //constructor connects to db
        public SQLWrapper()
        {
            init();
        }


        public void init()
        {
            server = "localhost";
            db = "tackyrss";
            uid = "admin";
            pw = "12345";
            string connectionString;
            connectionString = "Server=" + server + ";" + "DATABASE=" + db + ";" + "UID=" + uid + ";" + "PASSWORD=" + pw + ";";

            connection = new MySqlConnection(connectionString);
        }
        public bool openConnection()
        {
            try 
            {
                
                connection.Open();
                return true;
            }
            catch(MySqlException ex)
            {

                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }
        public bool closeConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
  

        public List<string> getRow(List<string>[] table, int row)
        {
            List<string> tuple = new List<string>();
            for (int i = 0; i < table.Count(); i++)
                tuple.Add(table[i][row - 1]);
            return tuple;
        }
        public List<string> getColumn(List<string>[] table, int col)
        {
            List<string> column = new List<string>();
            for (int i = 0; i < table[0].Count(); i ++)
                column.Add(table[col - 1][i]);
            return column;
        }
        public string printTuple(List<string> tuple)
        {
            string line = "";
            for (int i = 0; i < tuple.Count; i++)
            {
                line += tuple[i].ToString();
                if (i < tuple.Count - 1)
                    line += " || ";
            }

            return line;
        }

        public bool createUser(string user, string pw)
        {
            IAsyncResult waiter;
            if (!checkUser(user, pw))
            {
                string query = "insert into userlogininfo(username, password) values ('" + user + "', '" + pw + "');";
                if (openConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    waiter = cmd.BeginExecuteNonQuery();
                    //(new AsyncCallback(AsyncCommandCompletionCallback), cmd);
                    waiter.AsyncWaitHandle.WaitOne();
                    cmd.EndExecuteNonQuery(waiter);

                    this.closeConnection();
                    return true;
                    
                }
                
            }
            else
            {
                //already exists
                
            }
            return false;
        }
        public bool checkUser(string user, string pw)
        {
            List<string>[] sList = null; //seleciton list

            string query = ""; 
            MySqlCommand cmd;
            if (openConnection() == true)
            {
                IAsyncResult waiter;

                //setup inserts

                //do a check for story link
                query = "select * from userlogininfo where username='" + user + "' AND password= '" + pw +"';";
                MySqlCommand ex = new MySqlCommand(query, connection);
                MySqlDataReader dr = ex.ExecuteReader();
                sList = new List<string>[dr.FieldCount];
                for (int i = 0; i < dr.FieldCount; i++)
                    sList[i] = new List<string>();
                int max = 0;
                while (dr.Read() && max < 1000)
                {
                    for (int j = 0; j < dr.FieldCount; j++)
                    {
                        sList[j].Add(dr[j].ToString());
                    }
                    max++;
                }

                dr.Close();
                if (sList[0].Count != 0) //found some one
                {
                    this.closeConnection();
                    return true;
                }
            }
            this.closeConnection();
            return false;

        }

        public void createUserTables(string currentUser)
        {
            //first thing first, check to see if the table iexits
            if (openConnection() == true)
            {
                //make subscriptions first due to fk needs
                string tablequery = "create table if not exists ";
                tablequery += currentUser + "Subs( storyLink varchar(500) primary key, ";
                tablequery += "cid int, storyTitle char(255), ";
                tablequery += "storyDescription longtext, storyDate char(50), isRead int);";
                MySqlCommand createtable = new MySqlCommand(tablequery, connection);
                createtable.ExecuteNonQuery();

                //create user
                tablequery = "create table if not exists ";
                tablequery += currentUser + " ( channelLink varchar(500) primary key, ";
                tablequery += "channelid int references " + currentUser+ "Subs(cid), channelTitle char(255), ";
                tablequery += "version float, updateFrequency int, lastUpdate char(50));";
                createtable = new MySqlCommand(tablequery, connection);
                createtable.ExecuteNonQuery();
                //user table has been created and ready for updates!
                this.closeConnection();
 
            }

        }
        public List<string>[] getUserSubscriptions(string user)
        {
            List<string>[] sList = null; //seleciton list

            string query = "select * from " + user + ";"; //first lets make our channel!

            if (openConnection() == true) //connect
            {
                IAsyncResult waiter;
                MySqlCommand ex = new MySqlCommand(query, connection);
                MySqlDataReader dr = ex.ExecuteReader();
                sList = new List<string>[dr.FieldCount];
                for (int i = 0; i < dr.FieldCount; i++)
                    sList[i] = new List<string>();
                int max = 0;
                while (dr.Read() && max < 1000)
                {
                    for (int j = 0; j < dr.FieldCount; j++)
                    {
                        sList[j].Add(dr[j].ToString());
                    }
                    max++;
                }


                closeConnection();//close the conenction
            }

            return sList;
        }
        public int getCid(string title, User currentUser)
        {
            string[] nt = title.Split(':');
            title = nt[1].Substring(1);
            string query = "select channelId from " + currentUser.name + " where channelTitle='" + title +"';"; //get all teh stories fo rhits channelid
            List<string>[] sList;
            if (openConnection() == true) //connect
            {
                IAsyncResult waiter;
                MySqlCommand ex = new MySqlCommand(query, connection);
                MySqlDataReader dr = ex.ExecuteReader();
                sList = new List<string>[dr.FieldCount];
                for (int i = 0; i < dr.FieldCount; i++)
                    sList[i] = new List<string>();
                int max = 0;
                while (dr.Read() && max < 1000)
                {
                    for (int j = 0; j < dr.FieldCount; j++)
                    {
                        sList[j].Add(dr[j].ToString());
                    }
                    max++;
                }


                closeConnection();//close the conenction
                if (sList[0].Count > 0)
                    return Convert.ToInt32(sList[0][0]);
            }
            //

            return -1;
        }
        //given a cid, find all the subscriptions for the user 
        public List<Story> getUserStories(int cid, User currentUser)
        {
            List<string>[] sList = null; //seleciton list
            List<Story> stories = new List<Story>();
            if (cid == -1)
                return null;

            string query = "select * from " + currentUser.name + "subs where cid='" + cid.ToString() + "' order by storyDate desc;"; //get all teh stories fo rhits channelid

            if (openConnection() == true) //connect
            {
                IAsyncResult waiter;
                MySqlCommand ex = new MySqlCommand(query, connection);
                MySqlDataReader dr = ex.ExecuteReader();
                sList = new List<string>[dr.FieldCount];
                for (int i = 0; i < dr.FieldCount; i++)
                    sList[i] = new List<string>();
                int max = 0;
                while (dr.Read() && max < 1000)
                {
                    for (int j = 0; j < dr.FieldCount; j++)
                    {

                        sList[j].Add(dr[j].ToString());
                    }
                    max++;
                }


                closeConnection();//close the conenction
            }

            //the list is loaded
            //six attributes
            for (int i = 0; i < sList[0].Count(); i++ )
            {
                Story s = new Story(sList[2][i], sList[1][i], sList[0][i], sList[3][i], sList[4][i], sList[5][i]);
                stories.Add(s);
            }

            return stories;
        }
        ///need to have the same function but with a time checker too


        //load the channel information into the db
        //user tables must be already created
        public bool loadChannel(RssChannel channel, User currentUser)
        {

            //given the channel information store it as a subscription
            string query = ""; 
            List<string>[] sList = null; //seleciton list

            if (openConnection() == true) //connect
            {
                IAsyncResult waiter;
                //insure that there is no item like this arleady in! if so, then we gotta do an update
                query = "select * from " + currentUser.name + " where channelLink='" + channel.channelLink + "';";
                MySqlCommand ex = new MySqlCommand(query, connection);
                MySqlDataReader dr = ex.ExecuteReader();
                sList = new List<string>[dr.FieldCount];
                for (int i = 0; i < dr.FieldCount; i++)
                    sList[i] = new List<string>();
                int max = 0;
                while (dr.Read() && max < 1000)
                {
                    for (int j = 0; j < dr.FieldCount; j++)
                    {
                        sList[j].Add(dr[j].ToString());
                    }
                    max++;
                }

                dr.Close();

                if (sList[0].Count == 0) //not already inserted
                {
                    //insert part
                    query = "insert into " + currentUser.name + "(channelLink, channelId, channelTitle, version, updateFrequency, lastUpdate) values ('";
                    query += channel.channelLink + "', '" + channel.channelId.ToString() + "', '" + channel.channelTitle+ "', '" +channel.version.ToString() + "', '";
                    query += channel.updateFrequency.ToString() + "', '" + channel.latestFetchedStory + "');";

                    ex = new MySqlCommand(query, connection);
                    waiter = ex.BeginExecuteNonQuery();
                    waiter.AsyncWaitHandle.WaitOne();
                    ex.EndExecuteNonQuery(waiter);

                }
                else
                {
                    //channel has already been loaded
                    //update the last time this thing has been fetched
                    query = "update " + currentUser.name + " set lastUpdate='" + channel.latestFetchedStory + "' where channelLink='" + channel.channelId.ToString() + "';";
                    ex = new MySqlCommand(query, connection);
                    waiter = ex.BeginExecuteNonQuery();
                    waiter.AsyncWaitHandle.WaitOne();
                    ex.EndExecuteNonQuery(waiter);
                }

                closeConnection();//close the conenction
            }

            return false;
        }

        //user must be created first
        //this is a manual load given an rss channel, place it in for the user 
        //load it into the db and add it to the users list of channels
        public List<string>[] loadStories(RssChannel channel, User currentUser)
        {
            List<string>[] sList = null; //seleciton list
            string query = ""; 
            MySqlCommand cmd;
            //first creat the channel and update the db
            channel.channelId = currentUser.subscriptions.Count + 1;
            channel.latestFetchedStory = DateTime.Now.ToString("F");
            channel.lastTime = DateTime.Now;
            currentUser.subscriptions.Add(channel);
            //then update teh database for this channel
            loadChannel(channel, currentUser);
            //now load the stories
            if (openConnection() == true)
            {
                IAsyncResult waiter;
                //first add the subscription into the user
                //setup inserts
                foreach (Story article in channel.channelItems)
                {
                    //do a check for story link
                    query = "select * from " + currentUser.name + "subs where storyLink='" + article.storyLink + "';";
                    MySqlCommand ex = new MySqlCommand(query, connection);
                    MySqlDataReader dr = ex.ExecuteReader();
                    sList = new List<string>[dr.FieldCount];
                    for (int i = 0; i < dr.FieldCount; i++)
                        sList[i] = new List<string>();
                    int max = 0;
                    while (dr.Read() && max < 1000)
                    {
                        for (int j = 0; j < dr.FieldCount; j++)
                        {
                            sList[j].Add(dr[j].ToString());
                        }
                        max++;
                    }

                    dr.Close();
                    if (sList[0].Count == 0) //not already inserted
                    {
                        query = "insert into " + currentUser.name + "Subs(storyLink,cid,storyTitle, storyDescription, storyDate, isRead) values ('";
                        query += article.storyLink + "', '" + currentUser.subscriptions.Count + "', '" + article.storyTitle;
                        query += "', '" + article.storyDescription + "', '" + article.storyDate + "', '";
                        if (article.read)
                            query += "1');";
                        else
                            query += "0');";

                        cmd = new MySqlCommand(query, connection);
                        waiter = cmd.BeginExecuteNonQuery();
                        //(new AsyncCallback(AsyncCommandCompletionCallback), cmd);
                        waiter.AsyncWaitHandle.WaitOne();
                        cmd.EndExecuteNonQuery(waiter);
                    }
                    else
                    {
                        //dupilcate handling
                    }
                }
                //at this point the sql server now has all your subscriptions for a given a channel;
                //insert this channel into the user's selected channels, or updat the last update time.
            }
            this.closeConnection();
            return sList;
        }


        public bool updateHadRead(Story article, User currentUser)
        {

            string query = "";
            //update the article
            article.read = true;

            if (openConnection() == true) //connect
            {
                IAsyncResult waiter;
                query = "update " + currentUser.name + "Subs set isRead='1' where storylink='" + article.storyLink + "';";

                MySqlCommand ex = new MySqlCommand(query, connection);
                waiter = ex.BeginExecuteNonQuery();
                waiter.AsyncWaitHandle.WaitOne();
                ex.EndExecuteNonQuery(waiter);


                closeConnection();//close the conenction
                return true;
            }

            return false;
        }
        public bool updateHadUnRead(Story article, User currentUser)
        {

            string query = "";
            //update the article

            if (openConnection() == true) //connect
            {
                IAsyncResult waiter;
                query = "update " + currentUser.name + "Subs set isRead='0' where storylink='" + article.storyLink + "';";

                MySqlCommand ex = new MySqlCommand(query, connection);
                waiter = ex.BeginExecuteNonQuery();
                waiter.AsyncWaitHandle.WaitOne();
                ex.EndExecuteNonQuery(waiter);


                closeConnection();//close the conenction
                return true;
            }

            return false;
        }
        //this function updates the database with new stories from a given channel
        //the channel should be identical to the original channel, except stories have been recently fetched
        public bool updateSubscriptionStories(RssChannel channel, User currentUser)
        {
            List<string>[] sList = null; //seleciton list
            string query = "";
            MySqlCommand cmd;

            //update the last time
            channel.latestFetchedStory = DateTime.Now.ToString("F");
            channel.lastTime = DateTime.Now;
            //reload the informationa bout time to this guy!
            loadChannel(channel, currentUser);
            //now load the stories
            if (openConnection() == true)
            {
                IAsyncResult waiter;
                //first add the subscription into the user
                //setup inserts
                foreach (Story article in channel.channelItems)
                {
                    //do a check for story link if it already exist
                    query = "select * from " + currentUser.name + "subs where storyLink='" + article.storyLink + "';";
                    MySqlCommand ex = new MySqlCommand(query, connection);
                    MySqlDataReader dr = ex.ExecuteReader();
                    sList = new List<string>[dr.FieldCount];
                    for (int i = 0; i < dr.FieldCount; i++)
                        sList[i] = new List<string>();
                    int max = 0;
                    while (dr.Read() && max < 1000)
                    {
                        for (int j = 0; j < dr.FieldCount; j++)
                        {
                            sList[j].Add(dr[j].ToString());
                        }
                        max++;
                    }

                    dr.Close();
                    if (sList[0].Count == 0) //not already inserted
                    {
                        //new article!
                        query = "insert into " + currentUser.name + "Subs(storyLink,cid,storyTitle, storyDescription, storyDate, isRead) values ('";
                        query += article.storyLink + "', '" + channel.channelId.ToString() +"', '" + article.storyTitle;
                        query += "', '" + article.storyDescription + "', '" + article.storyDate + "', '";
                        if (article.read)
                            query += "1');";
                        else
                            query += "0');";

                        cmd = new MySqlCommand(query, connection);
                        waiter = cmd.BeginExecuteNonQuery();
                        //(new AsyncCallback(AsyncCommandCompletionCallback), cmd);
                        waiter.AsyncWaitHandle.WaitOne();
                        cmd.EndExecuteNonQuery(waiter);
                    }
                    else
                    {
                        //dupilcate handling
                        //ignroe this for now, we dont care about any story already in teh db.. 
                    }
                }
                //at this point the sql server now has all your subscriptions for a given a channel;
                //insert this channel into the user's selected channels, or updat the last update time.
                this.closeConnection();
                return true;
            }
            return false;

        }
        public bool removeChannel(RssChannel channel, User currentUser)
        {
            int i = channel.channelId; //the removing index!

            string query = "delete from " + currentUser.name + "subs where cid='" + i.ToString() + "';";

            if (openConnection() == true) //connect
            {
                if (currentUser.subscriptions.Count() > 1)
                {
                    IAsyncResult waiter;
                    MySqlCommand ex = new MySqlCommand(query, connection);
                    waiter = ex.BeginExecuteNonQuery();
                    waiter.AsyncWaitHandle.WaitOne();
                    ex.EndExecuteNonQuery(waiter);
                    //now delete from the user subscriptions
                    query = "delete from " + currentUser.name + " where channelid='" + i.ToString() + "';";
                    ex = new MySqlCommand(query, connection);
                    waiter = ex.BeginExecuteNonQuery();
                    waiter.AsyncWaitHandle.WaitOne();
                    ex.EndExecuteNonQuery(waiter);
                    //now update the rest
                    for (int j = i; j < currentUser.subscriptions.Count(); j++)
                    {
                        query = "update " + currentUser.name + "subs set cid='" + j.ToString() + "' where cid='" + (j + 1).ToString() + "';";
                        ex = new MySqlCommand(query, connection);
                        waiter = ex.BeginExecuteNonQuery();
                        waiter.AsyncWaitHandle.WaitOne();
                        ex.EndExecuteNonQuery(waiter);
                        query = "update " + currentUser.name + " set channelid='" + j.ToString() + "' where channelid='" + (j + 1).ToString() + "';";
                        ex = new MySqlCommand(query, connection);
                        waiter = ex.BeginExecuteNonQuery();
                        waiter.AsyncWaitHandle.WaitOne();
                        ex.EndExecuteNonQuery(waiter);
                    }
                    closeConnection();//close the conenction
                    return true;
                }
                else
                {
                    IAsyncResult waiter;
                    MySqlCommand ex = new MySqlCommand(query, connection);
                    waiter = ex.BeginExecuteNonQuery();
                    waiter.AsyncWaitHandle.WaitOne();
                    ex.EndExecuteNonQuery(waiter);
                    //now delete from the user subscriptions
                    query = "delete from " + currentUser.name + " where channelid='" + i.ToString() + "';";
                    ex = new MySqlCommand(query, connection);
                    waiter = ex.BeginExecuteNonQuery();
                    waiter.AsyncWaitHandle.WaitOne();
                    ex.EndExecuteNonQuery(waiter);
                    query = "delete from " + currentUser.name + " where channelid='" + i.ToString() + "';";
                    ex = new MySqlCommand(query, connection);
                    waiter = ex.BeginExecuteNonQuery();
                    waiter.AsyncWaitHandle.WaitOne();
                    ex.EndExecuteNonQuery(waiter);
                    closeConnection();//close the conenction
                    return true;
                }
            }

            return false;
        }
        //this function updates the channels update frequency locally and in the database.
        public bool channelUpdateFrequency(RssChannel channel, User currentUser, int updatedFreq)
        {
            string query = "";
            channel.updateFrequency = updatedFreq;


            if (openConnection() == true) //connect
            {
                IAsyncResult waiter;
                query = "update " + currentUser.name + " set updateFrequency='" + channel.updateFrequency +"' where channelLink='" +channel.channelLink + "';";

                MySqlCommand ex = new MySqlCommand(query, connection);
                waiter = ex.BeginExecuteNonQuery();
                waiter.AsyncWaitHandle.WaitOne();
                ex.EndExecuteNonQuery(waiter);


                closeConnection();//close the conenction
                return true;
            }

            return false;
        }
    }
}
