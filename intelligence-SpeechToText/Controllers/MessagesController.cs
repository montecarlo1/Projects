namespace ImageCaption.Controllers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Microsoft.Bot.Connector;
    using Services;
    using Microsoft.ProjectOxford.Vision;
    using Microsoft.ProjectOxford.Vision.Contract;
    using System.Text;
    using SpeechToText.Services;

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private static readonly string subscriptionKey = "da1f92f0-4cd3-4dfc-8122-b1727a061cdb";
        private static readonly string subscriptionKey2 = "e7d5aeecb42a426a89f3aa413e6133d1";
        private static readonly string apiBoot = "https://westus.api.cognitive.microsoft.com/vision/v1.0/analyze?visualFeatures=Description,Tags&subscription-key=da1f92f0-4cd3-4dfc-8122-b1727a061cdb";
        private readonly ICaptionService captionService = new MicrosoftCognitiveCaptionService();
        private readonly IVisionServiceClient textClient = new VisionServiceClient(subscriptionKey2);

        private readonly MicrosoftCognitiveSpeechService speechService = new MicrosoftCognitiveSpeechService();
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        //public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        //{
        //    if (activity.Type == ActivityTypes.Message)
        //    {
        //        var url = new Uri(activity.ServiceUrl);
        //        var connector = new ConnectorClient(url);
        //        string message = null;
        //        StringBuilder stringb = new StringBuilder();
        //        OcrResults myocr;
        //        myocr = await this.GetOrcTextAsync(activity, connector);
        //        if (myocr != null)
        //        {
        //            try
        //            {
        //message = await this.GetCaptionAsync(activity, connector);
        //var attach = activity.Attachments?.FirstOrDefault(a => a.ContentType.Contains("image")); 
        //message = await this.GetOrcTextAsync(activity.ServiceUrl, LanguageCodes.AutoDetect, true);
        //iterationResults = iterationResults.OrderBy(item => item.Number).ToList();
        //myocr.Language = LanguageCodes.ChineseSimplified;
        //var results = myocr;
        //foreach (var item in results.Regions)
        //{
        //    foreach (var line in item.Lines)
        //    {
        //        foreach (var word in line.Words)
        //        {
        //stringb.Append(word.Text);
        //stringb.Append(" ");
        //    message += word.Text + " ";
        //}
        //stringb.AppendLine();
        //}
        //stringb.AppendLine();
        //}
        //message = stringb.ToString();
        //            }
        //            catch (ArgumentException e)
        //            {
        //                message = "Did you upload an image? I'm more of a visual person. " +
        //                    "Try sending me an image or an image URL";
        //                Trace.TraceError(e.ToString());
        //            }
        //            catch (Exception e)
        //            {
        //                message = "Oops! Something went wrong. Try again later.";
        //                Trace.TraceError(e.ToString());
        //            }
        //            Activity reply = activity.CreateReply(message);
        //            await connector.Conversations.ReplyToActivityAsync(reply);
        //        }
        //        else
        //        {
        //            Trace.TraceInformation("the information is null! please choose an image with clear text");
        //        }
        //    }
        //    else
        //    {
        //        await this.HandleSystemMessage(activity);
        //    }

        //    var response = this.Request.CreateResponse(HttpStatusCode.OK);
        //    return response;
        //}
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                string message;

                try
                {
                    var audioAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Equals("audio/wav") || a.ContentType.Equals("application/octet-stream"));
                    if (audioAttachment != null)
                    {
                        var stream = await GetAudioStream(connector, audioAttachment);
                        var text = await this.speechService.GetTextFromAudioAsync(stream);
                        message = ProcessText(text);
                    }
                    else
                    {
                        message = "Did you upload an audio file? I'm more of an audible person. Try sending me a wav file";
                    }
                }
                catch (Exception e)
                {
                    message = "Oops! Something went wrong. Try again later.";

                    Trace.TraceError(e.ToString());
                }

                Activity reply = activity.CreateReply(message);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                await this.HandleSystemMessage(activity);
            }

            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
        private static string ProcessText(string text)
        {
            string message = "You said : " + text + ".";

            if (!string.IsNullOrEmpty(text))
            {
                var wordCount = text.Split(' ').Count(x => !string.IsNullOrEmpty(x));
                message += "\n\nWord Count: " + wordCount;

                var characterCount = text.Count(c => c != ' ');
                message += "\n\nCharacter Count: " + characterCount;

                var spaceCount = text.Count(c => c == ' ');
                message += "\n\nSpace Count: " + spaceCount;

                var vowelCount = text.ToUpper().Count("AEIOU".Contains);
                message += "\n\nVowel Count: " + vowelCount;
            }

            return message;
        }

        private static async Task<Stream> GetAudioStream(ConnectorClient connector, Attachment audioAttachment)
        {
            using (var httpClient = new HttpClient())
            {
                // The Skype attachment URLs are secured by JwtToken,
                // you should set the JwtToken of your bot as the authorization header for the GET request your bot initiates to fetch the image.
                // https://github.com/Microsoft/BotBuilder/issues/662
                var uri = new Uri(audioAttachment.ContentUrl);
                if (uri.Host.EndsWith("skype.com") && uri.Scheme == "https")
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync(connector));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                }

                return await httpClient.GetStreamAsync(uri);
            }
        }

        /// <summary>
        /// Gets the JwT token of the bot. 
        /// </summary>
        /// <param name="connector"></param>
        /// <returns>JwT token of the bot</returns>
        private static async Task<string> GetTokenAsync(ConnectorClient connector)
        {
            var credentials = connector.Credentials as MicrosoftAppCredentials;
            if (credentials != null)
            {
                return await credentials.GetTokenAsync();
            }

            return null;
        }
        private static async Task<Stream> GetImageStream(ConnectorClient connector, Attachment imageAttachment)
        {
            using (var httpClient = new HttpClient())
            {
                // The Skype attachment URLs are secured by JwtToken,
                // you should set the JwtToken of your bot as the authorization header for the GET request your bot initiates to fetch the image.
                // https://github.com/Microsoft/BotBuilder/issues/662
                var uri = new Uri(imageAttachment.ContentUrl);
                if (uri.Host.EndsWith("skype.com") && uri.Scheme == "https")
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync(connector));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                }
                return await httpClient.GetStreamAsync(uri);
            }
        }
        /// <summary>
        /// Gets the href value in an anchor element.
        /// </summary>
        ///  Skype transforms raw urls to html. Here we extract the href value from the url
        /// <param name="text">Anchor tag html.</param>
        /// <param name="url">Url if valid anchor tag, null otherwise</param>
        /// <returns>True if valid anchor element</returns>
        private static bool TryParseAnchorTag(string text, out string url)
        {
            var regex = new Regex("^<a href=\"(?<href>[^\"]*)\">[^<]*</a>$", RegexOptions.IgnoreCase);
            url = regex.Matches(text).OfType<Match>().Select(m => m.Groups["href"].Value).FirstOrDefault();
            return url != null;
        }
        /// <summary>
        /// Handles the system activity.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <returns>Activity</returns>
        private async Task<Activity> HandleSystemMessage(Activity activity)
        {
            switch (activity.Type)
            {
                case ActivityTypes.DeleteUserData:
                    // Implement user deletion here
                    // If we handle user deletion, return a real message
                    break;
                case ActivityTypes.ConversationUpdate:
                    // Greet the user the first time the bot is added to a conversation.
                    if (activity.MembersAdded.Any(m => m.Id == activity.Recipient.Id))
                    {
                        var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                        var response = activity.CreateReply();
                        response.Text = "Hi! I am SpeechToText Bot. I can understand the content of any audio" +
                                        " and try to describe it as well as any human. Try sending me a wav file.";
                        await connector.Conversations.ReplyToActivityAsync(response);
                    }
                    break;
                case ActivityTypes.ContactRelationUpdate:
                    // Handle add/remove from contact lists
                    break;
                case ActivityTypes.Typing:
                    // Handle knowing that the user is typing
                    break;
                case ActivityTypes.Ping:
                    break;
            }

            return null;
        }
    }
}