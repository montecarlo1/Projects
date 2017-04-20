using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Collections.Generic;
using StockBot2.Controllers;

namespace Bot_Application
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// Add Districts.
        /// </summary>
        /// <returns></returns>
        //private static Dictionary<int, string> Districts()
        //{
        //    Dictionary<int, string> districts = new Dictionary<int, string>();
        //    districts.Add(1, "闵行区");
        //    districts.Add(2, "徐汇区");
        //    districts.Add(3, "长宁区");
        //    districts.Add(4, "静安区");
        //    districts.Add(5, "杨浦区");
        //    districts.Add(6, "浦东新区");
        //    return districts;
        //}
        ///// <summary>
        ///// common replica method.
        ///// </summary>
        ///// <param name="replyToConversation"></param>
        ///// <param name="address"></param>
        ///// <param name="telephone"></param>
        //private void Reply(Activity replyToConversation, string address, string telephone)
        //{
        //    HeroCard hero = new HeroCard
        //    {
        //        Title = address,
        //        Subtitle = telephone,
        //        Buttons = new List<CardAction>() {
        //            new CardAction
        //            {
        //                Value = "http://gaode.com/search?query=" + address,
        //                Type = "openUrl",
        //                Title = "点我查看地图"
        //            }
        //        }
        //    };
        //    replyToConversation.Attachments.Add(hero.ToAttachment());
        //    replyToConversation.AttachmentLayout = "carousel";
        //}
        ///// <summary>
        ///// POST: api/Messages
        ///// Receive a message from a user and reply to it
        ///// </summary>
        //public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        //{
        //    Activity replyToConversation = activity.CreateReply("");
        //    replyToConversation.Recipient = activity.From;
        //    replyToConversation.Type = "message";
        //    replyToConversation.Attachments = new List<Attachment>();
        //    List<CardAction> cardButtons = new List<CardAction>();
        //    CardAction plButton = new CardAction()
        //    {
        //        Value = "http://gaode.com/search?query=",
        //        Type = "openUrl",
        //        Title = "点我查看地图详细地理位置信息"
        //    };
        //    Dictionary<int, string> dic = new Dictionary<int, string>();
        //    dic = MessagesController.Districts();
        //    if (activity.Type == ActivityTypes.Message)
        //    {
        //        ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
        //        List<string> districts = new List<string>();
        //        districts.Add("闵行区");
        //        districts.Add("徐汇区");
        //        districts.Add("长宁区");
        //        districts.Add("静安区");
        //        districts.Add("杨浦区");
        //        districts.Add("浦东新区");
        //        if (activity.Text == "预约保养")
        //        {
        //            for (int i = 1; i < 7; i++)
        //            {
        //                CardAction CardButton = new CardAction()
        //                {
        //                    Type = "imBack",
        //                    Title = dic[i],
        //                    Value = dic[i]
        //                };
        //                cardButtons.Add(CardButton);
        //            }
        //            HeroCard plCard = new HeroCard()
        //            {
        //                Text = "请选择区域：",
        //                Buttons = cardButtons
        //            };
        //            Attachment plAttachment = plCard.ToAttachment();
        //            replyToConversation.Attachments.Add(plAttachment);
        //            var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
        //        }
        //        foreach (var item in districts)
        //        {
        //            Activity replyx = activity.CreateReply(item);
        //            switch (item)
        //            {
        //                case "闵行区":
        //                    if (activity.Text == "闵行区")
        //                    {
        //                        List<dynamic> entityList = new List<dynamic>();
        //                        entityList.Add(new { address = "上海市闵行区华翔路245号", telephone = "电话：13482233605" });
        //                        entityList.Add(new { address = "上海市闵行区莘庄镇黎安路551号", telephone = "电话：15800585521" });
        //                        entityList.Add(new { address = "上海市闵行区沪闵路6088号B3层 ", telephone = "电话：021-54339132" });
        //                        entityList.Add(new { address = "上海市闵行区秀波路居家易24小时自助洗车金源中心网点", telephone = "电话：4008700121" });
        //                        foreach (var entity in entityList)
        //                        {
        //                            Reply(replyToConversation, entity.address, entity.telephone);
        //                        }
        //                        var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
        //                    }
        //                    break;
        //                case "徐汇区":
        //                    if (activity.Text == "徐汇区")
        //                    {
        //                        List<dynamic> entityList = new List<dynamic>();
        //                        entityList.Add(new { address = "上海市徐汇区华石路109号", telephone = "电话：18019389103" });
        //                        entityList.Add(new { address = "上海市徐汇区肇嘉浜路988号B3层", telephone = "电话：021-64269827" });
        //                        entityList.Add(new { address = "上海市徐汇区古井路与吴中路交叉口西北200米", telephone = "电话：021-51001966" });
        //                        entityList.Add(new { address = "上海市徐汇区龙吴路2453号加油站内", telephone = "电话：15900621102" });
        //                        foreach (var entity in entityList)
        //                        {
        //                            Reply(replyToConversation, entity.address, entity.telephone);
        //                        }
        //                        var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
        //                    }
        //                    break;
        //                case "浦东新区":
        //                    if (activity.Text == "浦东新区")
        //                    {
        //                        List<dynamic> entityList = new List<dynamic>();
        //                        entityList.Add(new { address = "上海市浦东新区花木路1378号嘉里城B2层", telephone = "电话：021-38953516" });
        //                        entityList.Add(new { address = "上海市浦东新区上南路6761号,吴迅中学门口对面", telephone = "电话：13585710772" });
        //                        entityList.Add(new { address = "上海市浦东新区东方路796号96广场B2层", telephone = "电话：021-58301048" });
        //                        entityList.Add(new { address = "上海市浦东新区银城中路68号B3楼时代金融中心", telephone = "电话：021-68800170" });
        //                        foreach (var entity in entityList)
        //                        {
        //                            Reply(replyToConversation, entity.address, entity.telephone);
        //                        }
        //                        var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
        //                    }
        //                    break;
        //                case "长宁区":
        //                    if (activity.Text == "长宁区")
        //                    {
        //                        List<dynamic> entityList = new List<dynamic>();
        //                        entityList.Add(new { address = "上海市长宁区长宁路1018号凯德龙之梦购物中心B3层", telephone = "电话：021-33727138" });
        //                        entityList.Add(new { address = "上海市长宁区延安西路488号上海日航饭店B2层,吴迅中学门口对面", telephone = "电话：15800419972" });
        //                        entityList.Add(new { address = "上海市长宁区虹桥路2222弄沪杏科技图书馆旁", telephone = "电话：13636416268" });
        //                        entityList.Add(new { address = "上海市长宁区新泾路90号", telephone = "电话：13774269267" });
        //                        foreach (var entity in entityList)
        //                        {
        //                            Reply(replyToConversation, entity.address, entity.telephone);
        //                        }
        //                        var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
        //                    }
        //                    break;
        //                case "静安区":
        //                    if (activity.Text == "静安区")
        //                    {
        //                        List<dynamic> entityList = new List<dynamic>();
        //                        entityList.Add(new { address = "上海市静安区平遥路112号", telephone = "电话：021-56652812" });
        //                        entityList.Add(new { address = "上海市静安区石门二路493号", telephone = "电话：021-62189867" });
        //                        entityList.Add(new { address = "上海市静安区灵石路697-3号巨丰商务3层", telephone = "电话：021-32059908" });
        //                        entityList.Add(new { address = "上海市静安区塘沽路与浙江北路交叉口东北50米", telephone = "电话：13816275100" });
        //                        foreach (var entity in entityList)
        //                        {
        //                            Reply(replyToConversation, entity.address, entity.telephone);
        //                        }
        //                        var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
        //                    }
        //                    break;
        //                case "杨浦区":
        //                    if (activity.Text == "杨浦区")
        //                    {
        //                        List<dynamic> entityList = new List<dynamic>();
        //                        entityList.Add(new { address = "上海市杨浦区中原路与国伟路交叉口东50米", telephone = "电话：13816245738" });
        //                        entityList.Add(new { address = "上海市杨浦区开鲁路16号", telephone = "电话：13564907579" });
        //                        entityList.Add(new { address = "上海市杨浦区佳木斯路312-318号", telephone = "电话：021-65680966" });
        //                        entityList.Add(new { address = "上海市杨浦区周家嘴路4230号杨浦汽配城12幢1-2号", telephone = "电话：021-65802501" });
        //                        foreach (var entity in entityList)
        //                        {
        //                            Reply(replyToConversation, entity.address, entity.telephone);
        //                        }
        //                        var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
        //                    }
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        HandleSystemMessage(activity);
        //    }
        //    var response = Request.CreateResponse(HttpStatusCode.OK);
        //    return response;
        //}
        //private Activity HandleSystemMessage(Activity message)
        //{
        //    if (message.Type == ActivityTypes.DeleteUserData)
        //    {
        //        // Implement user deletion here
        //        // If we handle user deletion, return a real message
        //    }
        //    else if (message.Type == ActivityTypes.ConversationUpdate)
        //    {
        //        // Handle conversation state changes, like members being added and removed
        //        // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
        //        // Not available in all channels
        //    }
        //    else if (message.Type == ActivityTypes.ContactRelationUpdate)
        //    {
        //        // Handle add/remove from contact lists
        //        // Activity.From + Activity.Action represent what happened
        //    }
        //    else if (message.Type == ActivityTypes.Typing)
        //    {
        //        // Handle knowing tha the user is typing
        //    }
        //    else if (message.Type == ActivityTypes.Ping)
        //    {
        //    }
        //    return null;
        //}
        public async Task<string> GetStock(string StockSymbol)
        {
            double? dblStockValue = await YahooBot2.GetStockRateAsync(StockSymbol);
            if (dblStockValue == null)
            {
                return string.Format("This \"{0}\" is not an valid stock symbol", StockSymbol);
            }
            else
            {
                return string.Format("Stock : {0}\n Price : {1}", StockSymbol, dblStockValue);
            }
        }
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {

            if (activity.Type == ActivityTypes.Message)
            {
                string StockRateString = await GetStock(activity.Text);
                Activity reply = activity.CreateReply(StockRateString);
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                await connector.Conversations.ReplyToActivityAsync(reply);

                //// calculate something for us to return
                //int length = (activity.Text ?? string.Empty).Length;

                //// return our reply to the user
                //Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
                //await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}