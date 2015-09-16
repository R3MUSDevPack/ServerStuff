using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKON.Slack
{
    public class MessagePayloadAttachment
    {
        [JsonProperty("fallback")]
        public string Fallback { get; set; }
        [JsonProperty("color")]
        public string Colour { get; set; }
        [JsonProperty("pretext")]
        public string Pretext { get; set; }
        [JsonProperty("author_name")]
        public string AuthorName { get; set; }
        [JsonProperty("author_link")]
        public string AuthorLink { get; set; }
        [JsonProperty("author_icon")]
        public string AuthorIcon { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("title_link")]
        public string TitleLink { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
        [JsonProperty("thumb_url")]
        public string ThumbUrl { get; set; }
    }
}
