using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Web.Model;
using System.Web;
using System.Web.Security;

namespace Web.Repositories
{
    public class WXDal
    {
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="echoStr"></param>
        /// <returns></returns>
        public string Validate(string signature, string timestamp, string nonce, string echoStr)
        {
            string result = "";
            if (CheckSignature(signature, timestamp, nonce))
            {
                if (!string.IsNullOrEmpty(echoStr))
                {
                    // System.Web.HttpContext.Current.Response.Write(echoStr);
                    //  System.Web.HttpContext.Current.Response.End();
                    result = echoStr;
                }
            }
            return result;
        }
        /// <summary>
        /// 验证微信签名
        /// </summary>
        /// * 将token、timestamp、nonce三个参数进行字典序排序
        /// * 将三个参数字符串拼接成一个字符串进行sha1加密
        /// * 开发者获得加密后的字符串可与signature对比，标识该请求来源于微信。
        /// <returns></returns>
        private bool CheckSignature(string signature, string timestamp, string nonce)
        {
            string Token = "weixin";
            string[] ArrTmp = { Token, timestamp, nonce };
            Array.Sort(ArrTmp);     //字典排序
            string tmpStr = string.Join("", ArrTmp);
            tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            tmpStr = tmpStr.ToLower();
            if (tmpStr == signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 获取客户发送来的请求信息
        /// </summary>
        /// <param name="postStr"></param>
        /// <returns></returns>
        public WXRequestXML GetRequest(string postStr)
        {
            #region 处理接收到的数据
            //封装请求类
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(postStr);
            XmlElement rootElement = doc.DocumentElement;
            XmlNode MsgType = rootElement.SelectSingleNode("MsgType");
            WXRequestXML requestXML = new WXRequestXML();
            requestXML.ToUserName = rootElement.SelectSingleNode("ToUserName").InnerText;
            requestXML.FromUserName = rootElement.SelectSingleNode("FromUserName").InnerText;
            requestXML.CreateTime = rootElement.SelectSingleNode("CreateTime").InnerText;
            requestXML.MsgType = MsgType.InnerText;

            if (requestXML.MsgType == "text")
            {
                requestXML.Content = rootElement.SelectSingleNode("Content").InnerText;
                requestXML.MsgId = rootElement.SelectSingleNode("MsgId").InnerText;
            }
            else if (requestXML.MsgType == "location")
            {
                requestXML.Location_X = rootElement.SelectSingleNode("Location_X").InnerText;
                requestXML.Location_Y = rootElement.SelectSingleNode("Location_Y").InnerText;
                requestXML.Scale = rootElement.SelectSingleNode("Scale").InnerText;
                requestXML.Label = rootElement.SelectSingleNode("Label").InnerText;
                requestXML.MsgId = rootElement.SelectSingleNode("MsgId").InnerText;
            }
            else if (requestXML.MsgType == "image")
            {
                requestXML.PicUrl = rootElement.SelectSingleNode("PicUrl").InnerText;
                requestXML.MsgId = rootElement.SelectSingleNode("MsgId").InnerText;
            }
            else if (requestXML.MsgType == "link")
            {
                requestXML.Title = rootElement.SelectSingleNode("Title").InnerText;
                requestXML.Description = rootElement.SelectSingleNode("Description").InnerText;
                requestXML.Url = rootElement.SelectSingleNode("Url").InnerText;
                requestXML.MsgId = rootElement.SelectSingleNode("MsgId").InnerText;
            }
            else if (requestXML.MsgType == "event")
            {
                requestXML.Event = rootElement.SelectSingleNode("Event").InnerText;
            }
            #endregion
            return requestXML;
        }

        /// <summary>
        /// 回复文字消息
        /// </summary>
        /// <param name="requestXML">接收到的请求信息</param>
        /// <param name="txt">消息内容</param>
        /// <returns></returns>
        public string SendMsg(WXRequestXML requestXML, string txt)
        {
            string strResult = "";
            strResult = "<xml>"
                        + "<ToUserName><![CDATA[" + requestXML.FromUserName + "]]></ToUserName>"
                        + "<FromUserName><![CDATA[" + requestXML.ToUserName + "]]></FromUserName>"
                        + "<CreateTime>" + ConvertDateTimeInt(DateTime.Now) + "</CreateTime>"
                        + "<MsgType><![CDATA[text]]></MsgType>"
                        + "<Content><![CDATA[" + txt + "]]></Content>"
                        + "</xml>";
            return strResult;
        }

        /// <summary>
        /// 回复图文信息
        /// </summary>
        /// <param name="requestXML">接收到的请求信息</param>
        /// <param name="articleList">图文列表</param>
        /// <returns></returns>
        public string SendMsg(WXRequestXML requestXML, IList<WXArticle> articleList)
        {
            string strResult = "";
            var strItems = "";
            foreach (var item in articleList)
            {
                strItems += "<item>"
                          + "<Title><![CDATA[" + item.Title + "]]></Title>"
                          + "<Description><![CDATA[" + item.Description + "]]></Description>"
                          + "<PicUrl><![CDATA[" + item.PicUrl + "]]></PicUrl>"
                          + "<Url><![CDATA[" + item.Url + "]]></Url>"
                          + "</item>";
            }
            strResult = "<xml>"
                        + "<ToUserName><![CDATA[" + requestXML.FromUserName + "]]></ToUserName>"
                        + "<FromUserName><![CDATA[" + requestXML.ToUserName + "]]></FromUserName>"
                        + "<CreateTime>" + ConvertDateTimeInt(DateTime.Now) + "</CreateTime>"
                        + "<MsgType><![CDATA[news]]></MsgType>"
                        + "<ArticleCount>" + articleList.Count + "</ArticleCount>"
                        + "<Articles>"
                        + strItems
                        + "</Articles>"
                        + "</xml>";
            return strResult;
        }

        /// <summary>
        /// 回复音乐消息
        /// </summary>
        /// <param name="requestXML"></param>
        /// <param name="music"></param>
        /// <returns></returns>
        public string SendMsg(WXRequestXML requestXML, WXMusic music)
        {
            string strResult = "";
            strResult = "<xml>"
                        + "<ToUserName><![CDATA[" + requestXML.FromUserName + "]]></ToUserName>"
                        + "<FromUserName><![CDATA[" + requestXML.ToUserName + "]]></FromUserName>"
                        + "<CreateTime>" + ConvertDateTimeInt(DateTime.Now) + "</CreateTime>"
                        + "<MsgType><![CDATA[music]]></MsgType>"
                        + "<Music>"
                        + "<Title><![CDATA[" + music.Title + "]]></Title>"
                        + "<Description><![CDATA[" + music.Description + "]]></Description>"
                        + "<MusicUrl><![CDATA[" + music.MusicUrl + "]]></MusicUrl>"
                        + "<HQMusicUrl><![CDATA[" + music.HQMusicUrl + "]]></HQMusicUrl>"
                        + "</Music>"
                        + "</xml>";
            return strResult;
        }



        /// <summary>
        /// unix时间转换为datetime
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        private DateTime UnixTimeToTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }


        /// <summary>
        /// datetime转换为unixtime
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

      
    }
}
