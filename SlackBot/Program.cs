using Slack_Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SlackBot
{
    public class Program
    {
        private TcpListener listener;
        private int port = 9999;

        r3mus_DBEntities db = new r3mus_DBEntities();

        static void Main(string[] args)
        {
            var program = new Program();
        }

        public Program()
        {
            try
            {
                listener = new TcpListener(port);
                listener.Start();
                Console.WriteLine("Web Server running");

                Thread thr = new Thread(new ThreadStart(StartListen));
                thr.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        public void StartListen()
        {
            int iStartPos = 0;
            String sRequest;
            String sDirName;
            String sRequestedFile;
            String sErrorMessage;
            String sLocalDir;
            String sMyWebServerRoot = "C:\\MyWebServerRoot\\";
            String sPhysicalFilePath = "";
            String sFormattedMessage = "";
            String sResponse = "";
            while (true)
            {
                //Accept a new connection
                Socket mySocket = listener.AcceptSocket();

                Console.WriteLine("Socket Type " + mySocket.SocketType);
                if (mySocket.Connected)
                {
                    Console.WriteLine("\nClient Connected!!\n==================\n Client IP {0}\n", mySocket.RemoteEndPoint);


                    //make a byte array and receive data from the client 
                    Byte[] bReceive = new Byte[1024];
                    int i = mySocket.Receive(bReceive, bReceive.Length, 0);


                    //Convert Byte to String
                    string sBuffer = Encoding.ASCII.GetString(bReceive);
                    string[] bits = sBuffer.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);


                    Console.WriteLine(sBuffer);
                    System.IO.File.WriteAllText(@"C:\Test\Log.txt", sBuffer);

                    if (bits[0] == "GET")
                    {
                        string[] commands = bits[1].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        if (commands[0].ToLower() == "rtmbot")
                        {
                            if (commands[1].ToLower() == "getbrowser")
                            {
                                string[] browserBits = bits[11].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                                browserBits[1] = browserBits[1].Replace("\r\n", " ").Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0];

                                SendToBrowser(string.Concat("Your browser is ", browserBits[0], " ", browserBits[1]), ref mySocket);
                            }
                            else if (commands[1].ToLower() == "hi")
                            {
                                if (commands.Count() == 2)
                                {
                                    SendToBrowser(string.Concat("Hi strangely unidentifed person!"), ref mySocket);
                                }
                                else
                                {
                                    for (var person = 2; person < commands.Count(); person++)
                                    {
                                        SendToBrowser(string.Concat("Hi ", commands[person], "!"), ref mySocket);
                                    }
                                }
                            }
                            else if (commands[1].ToLower() == "jokeoftheday")
                            {
                            }
                            else
                            {
                            }
                        }
                    }
                    else if (bits[0] == "POST")
                    {
                        var splitStr = sBuffer.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        HandlePOST(splitStr[splitStr.Length - 1].Replace("\r\n", " ").Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[1]);
                    }

                    // Look for HTTP request
                    iStartPos = sBuffer.IndexOf("HTTP", 1);


                    // Get the HTTP text and version e.g. it will return "HTTP/1.1"
                    string sHttpVersion = sBuffer.Substring(iStartPos, 8);


                    // Extract the Requested Type and Requested file/directory
                    sRequest = sBuffer.Substring(0, iStartPos - 1);


                    //Replace backslash with Forward Slash, if Any
                    sRequest.Replace("\\", "/");

                    mySocket.Close();
                }
            }
        }

        private void HandlePOST(string postData)
        {
            var postDataArr = postData.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            var argDict = new Dictionary<string, string>();

            var responses = new List<string>();

            try
            {

                postData.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(kvp =>
                    argDict.Add(kvp.Split(new string[] { "=" }, StringSplitOptions.None)[0],
                        kvp.Split(new string[] { "=" }, StringSplitOptions.None)[1]
                    ));

                if (argDict["token"] == "msstbOqT45Mf8xboRWiIdcye")
                {
                    var channelName = argDict["channel_name"];
                    var username = argDict["user_name"];
                    AspNetUser r3musUser;

                    try
                    {
                        r3musUser = db.AspNetUsers.Where(user => user.UserName.Replace(" ", "").ToLower() == username).FirstOrDefault();
                        if(r3musUser.UserName == string.Empty)
                        {
                            r3musUser = db.AspNetUsers.Where(user => user.UserName.ToLower().Contains(username)).FirstOrDefault();
                        }
                    }
                    catch(Exception ex)
                    {
                        r3musUser = db.AspNetUsers.Where(user => user.UserName.ToLower().Contains(username)).FirstOrDefault();
                    }

                    var rep = argDict["text"].Split(new string[] { "+" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    rep.RemoveAt(0);
                    argDict["text"] = string.Join(" ", rep.ToArray<string>());

                    try
                    {
                        if ((argDict["text"].ToLower().Contains("hi")) || argDict["text"].ToLower().Contains("hello"))
                        {
                            SendMessage(string.Format
                                (
                                    "Hi, {0}! ", 
                                    r3musUser.UserName.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0]
                                ));
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("1 " + ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void SendMessage(string message, string channel = "bottesting")
        {
            Slack.SendToRoom(message, channel, "https://hooks.slack.com/services/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");
        }

        private void SendToBrowser(string sData, ref Socket mySocket)
        {
            SendToBrowser(Encoding.ASCII.GetBytes(sData), ref mySocket);
        }
        private void SendToBrowser(Byte[] bSendData, ref Socket mySocket)
        {
            int numBytes = 0;
            try
            {
                if (mySocket.Connected)
                {
                    if ((numBytes = mySocket.Send(bSendData,
                          bSendData.Length, 0)) == -1)
                        Console.WriteLine("Socket Error cannot Send Packet");
                    else
                    {
                        Console.WriteLine("No. of bytes send {0}", numBytes);
                    }
                }
                else
                    Console.WriteLine("Connection Dropped....");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Occurred : {0} ", e);
            }
        }
    }
}
