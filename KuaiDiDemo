using HtmlAgilityPack;
using ScrapySharp.Network;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Tools;

namespace ETAppConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var uri = new Uri("http://www.kuaidi100.com/");
            var browser = new ScrapingBrowser();
            var htmlData = browser.DownloadString(uri);
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(htmlData);
            HtmlNode html = htmlDocument.DocumentNode;
            var nodeList = html.CssSelect("#companyList dl a");

            //////////抓取快递100提供的可查询的快递公司数据
            IList<KuaiDiModel> listKD = new List<KuaiDiModel>();
            foreach (var htmlNode in nodeList)
            {
                string data_code = htmlNode.Attributes["data-code"].Value;
                var children = htmlNode.CssSelect("span");
                string data_name = "";
                foreach (var spanNode in children)
                {
                    data_name = spanNode.InnerHtml;
                }
                KuaiDiModel itemKD = new KuaiDiModel();
                itemKD.Code = data_code;
                itemKD.Name = data_name;

                if (listKD.Where(x => x.Code == data_code).Count() == 0)   ///////////在itemKD中插入数据前判断code是否已存在
                {
                    listKD.Add(itemKD);
                }
            }
            foreach (var item in listKD)
            {
                Console.WriteLine(item.Code + "|" + item.Name);
            }

            ////////测试查询   type为快递公司编码 postid为运单号
            string uuu = "http://www.kuaidi100.com/query?type=yunda&postid=1900491153505";
            var uri2 = new Uri(uuu);
            string kkkk = browser.DownloadString(uri2);
            KuaiDiLog dd = Util.ParseFromJson<KuaiDiLog>(kkkk);
            Console.ReadLine();
    }


    /// <summary>
    /// 快递公司
    /// </summary>
    public class KuaiDiModel
    {
        /// <summary>
        /// 快递编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 快递名
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// 快递信息
    /// </summary>
    public class KuaiDiLog
    {
        /// <summary>
        /// 无意义，请忽略
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 物流公司编号
        /// </summary>
        public string com { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string nu { get; set; }

        /// <summary>
        /// 无意义，请忽略
        /// </summary>
        public string ischeck { get; set; }

        /// <summary>
        /// 查询时间
        /// </summary>
        public DateTime updatetime { get; set; }

        /// <summary>
        /// 无意义，请忽略
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 无意义，请忽略
        /// </summary>
        public string condition { get; set; }

        /// <summary>
        /// 快递单当前的状态 ：　 
        ///0：在途，即货物处于运输过程中；
        ///1：揽件，货物已由快递公司揽收并且产生了第一条跟踪信息；
        ///2：疑难，货物寄送过程出了问题；
        ///3：签收，收件人已签收；
        ///4：退签，即货物由于用户拒签、超区等原因退回，而且发件人已经签收；
        ///5：派件，即快递正在进行同城派件；
        ///6：退回，货物正处于退回发件人的途中；
        /// </summary>
        public string state { get; set; }

        public IList<KuaiDiLogItem> data { get; set; }
    }

    /// <summary>
    /// 快递跟踪记录
    /// </summary>
    public class KuaiDiLogItem
    {
        /// <summary>
        /// 每条跟踪信息的时间
        /// </summary>
        public DateTime time { get; set; }

        /// <summary>
        /// 每条跟踪信息的时间
        /// </summary>
        public DateTime ftime { get; set; }

        /// <summary>
        /// 每条跟综信息的描述
        /// </summary>
        public string context { get; set; }
    }
}
