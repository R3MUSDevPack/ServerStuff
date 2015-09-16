using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace JKON.Slack
{
    //A simple C# class to post messages to a Slack channel
    //Note: This class uses the Newtonsoft Json.NET serializer available via NuGet
    public class SlackClient
    {
        private readonly Uri m_Uri;
        private readonly Encoding m_Encoding = new UTF8Encoding();

        public SlackClient(string v_UrlWithAccessToken)
        {
            m_Uri = new Uri(v_UrlWithAccessToken);
        }
        //Post a message using simple strings
        public void PostMessage(string v_Message, string v_Username = null, string v_Channel = null)
        {
            MessagePayload payload = new MessagePayload()
            {
                Channel = v_Channel,
                Username = v_Username,
                Text = v_Message
            };
            PostMessage(payload);
        }
        //Post a message using a Payload object
        public void PostMessage(MessagePayload v_Payload)
        {
            string l_PayloadJson = JsonConvert.SerializeObject(v_Payload);
            using (WebClient l_Client = new WebClient())
            {
                NameValueCollection l_Data = new NameValueCollection();
                l_Data["payload"] = l_PayloadJson;
                var l_Response = l_Client.UploadValues(m_Uri, "POST", l_Data);
                //The response text is usually "ok"
                string l_ResponseText = m_Encoding.GetString(l_Response);
            }
        }
    }

    ////This class serializes into the Json payload required by Slack Incoming WebHooks
    //public class Payload
    //{
    //    [JsonProperty("channel")]
    //    public string Channel { get; set; }
    //    [JsonProperty("username")]
    //    public string Username { get; set; }
    //    [JsonProperty("text")]
    //    public string Text { get; set; }

    //    [JsonProperty("attachments")]
    //    public List<Attachment> Attachments { get; set; }
    //}

    //public class Attachment
    //{
    //    [JsonProperty("fallback")]
    //    public string Fallback { get; set; }
    //    [JsonProperty("color")]
    //    public string Colour { get; set; }
    //    [JsonProperty("pretext")]
    //    public string Pretext { get; set; }
    //    [JsonProperty("author_name")]
    //    public string AuthorName { get; set; }
    //    [JsonProperty("author_link")]
    //    public string AuthorLink { get; set; }
    //    [JsonProperty("author_icon")]
    //    public string AuthorIcon { get; set; }
    //    [JsonProperty("title")]
    //    public string Title { get; set; }
    //    [JsonProperty("title_link")]
    //    public string TitleLink { get; set; }
    //    [JsonProperty("text")]
    //    public string Text { get; set; }
    //    [JsonProperty("image_url")]
    //    public string ImageUrl { get; set; }
    //    [JsonProperty("thumb_url")]
    //    public string ThumbUrl { get; set; }
    //}
}