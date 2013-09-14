using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;
using Web.Tools;
using Web.Model;
using Web.Repositories;

namespace WSite_1._0.Controllers
{
    public class WXApiController : Controller
    {
        //
        // GET: /WXApi/
        public ContentResult Index()
        {
            WXDal _wx = new WXDal();
            string strResult = "";  ////////////返回结果
            string postStr = "";
            if (Request.HttpMethod.ToLower() == "post")////////接受消息
            {
                Stream s = System.Web.HttpContext.Current.Request.InputStream;
                byte[] b = new byte[s.Length];
                s.Read(b, 0, (int)s.Length);
                postStr = Encoding.UTF8.GetString(b);
                if (!string.IsNullOrEmpty(postStr)) //请求处理
                {
                    #region 根据接收到的消息 写处理逻辑                   
                    WXRequestXML requestXML = _wx.GetRequest(postStr);
                    if (requestXML.MsgType == WXMsgType.T_TEXT)
                    {
                        string strContent = requestXML.Content;
                        if (strContent.StartsWith("cx"))
                        {

                        }
                        else
                        {
                            IList<WXArticle> articles = new List<WXArticle>();
                            WXArticle item = new WXArticle();
                            item.Title = "测试标题";
                            item.Description = "描述";
                            item.PicUrl = "";
                            item.Url = "http://www.baidu.com";
                            articles.Add(item);
                            strResult = _wx.SendMsg(requestXML, articles);
                        }
                    }
                    #endregion
                }
            }
            else//////////接口验证
            {
                Util.GetStringParam(Request, "", "");
                string signature = Util.GetStringParam(Request, "signature", "");
                string timestamp = Util.GetStringParam(Request, "timestamp", "");
                string nonce = Util.GetStringParam(Request, "nonce", "");
                string echoStr = Util.GetStringParam(Request, "echoStr", "");
                strResult = _wx.Validate(signature, timestamp, nonce, echoStr);
            }
            //return View();
            ContentResult result = new ContentResult();
            result.Content = strResult;
            return result;
        }

    } 
}
