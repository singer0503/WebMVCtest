using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebMVCtest.Models;

namespace WebMVCtest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 檢查資料來源url是否存在
        /// </summary>
        /// <param name="url">網址</param>
        /// <returns></returns>
        public static bool isValidURL(string url)
        {
            WebRequest webRequest = WebRequest.Create(url);
            WebResponse webResponse;
            try
            {
                webResponse = webRequest.GetResponse();
            }
            catch //If exception thrown then couldn't get response from address
            {
                return false;
            }
            return true;
        }

        
        public ActionResult Test() {
            TestViewModel model = new TestViewModel();
            string SourceURL_1 = "http://www.coolpc.com.tw/evaluate.phpsdfsdfanlksjfiojsidhfsdfsdfsdfsdf";
            string SourceURL_2 = "http://www.coolpc.com.tw/evaluate.php";


            string Endpoint = "";
            if (isValidURL(SourceURL_1))
            {
                model.dataSource = "目前使用第一個資料來源";
                Endpoint = SourceURL_1;
            } else if (isValidURL(SourceURL_2))
            {
                model.dataSource = "目前使用第二個資料來源，第一個掛惹~";
                Endpoint = SourceURL_2;
            }
            else {
                model.dataSource = "兩個資料來源都掛惹~ zzz";
            }
            //-----------------------------------------------------------------------------------------
            WebClient url = new WebClient();
            //Note: 將網頁來源資料暫存到記憶體內
            MemoryStream ms = new MemoryStream(url.DownloadData(Endpoint));
            //Note: 使用預設編碼讀入 HTML  ,此為第三方套件 HtmlAgilityPack
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.Default);

            //-----------------------------------------------------------------------------------------
            //Note: 判斷需要的商品類型(以CPU為例)，再擷取該商品類型內選單的分類
            var list_type = new List<string>();
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//select[@name='n4']//optgroup"); //Note: 這個是XPath搜尋語法, 意思是直接選取select name為n4的那個節點,取得底下所有自定義的optgroup

            foreach (HtmlNode node in nodes)
            {
                var type = node.Attributes["label"].Value;
                list_type.Add(type);
            }

            //-----------------------------------------------------------------------------------------
            //Note: 擷取商品名稱與價格
            List<string> list_name = doc.DocumentNode.SelectSingleNode("//select[@name='n4']").InnerText.Split('\n').ToList();

            //Note: 去掉是空字串的資料
            list_name = list_name.Where(x => x != "").ToList();

            //Note: 刪除不必要的非商品選項
            list_name.RemoveRange(0, 1);

            //-----------------------------------------------------------------------------------------
            //Note: 將商品類型與名稱填入Model
            var models = new List<Product>();
            int number = 0;
            for (int i = 0; i < list_name.Count; i++)
            {
                string type = list_type[number];
                string name = list_name[i];

                if (name == "")
                {
                    number++;
                }
                else
                {
                    models.Add(new Product()
                    {
                        type = type,
                        name = name
                    });

                }
            }

            List<string> temp = models.Select(x => x.type + " " + x.name).ToList();
            model.data1 = string.Join("<br/>", temp);

            return View(model);
        }


        //這個還沒做好~
        public ActionResult Stock() {
            //指定來源網頁
            WebClient url = new WebClient();
            //將網頁來源資料暫存到記憶體內
            MemoryStream ms = new MemoryStream(url.DownloadData("http://tw.stock.yahoo.com/q/q?s=1101"));
            //以奇摩股市為例http://tw.stock.yahoo.com
            //1101 表示為股票代碼

            // 使用預設編碼讀入 HTML 
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.Default);

            // 裝載第一層查詢結果 
            HtmlDocument hdc = new HtmlDocument();

            //XPath 來解讀它 /html[1]/body[1]/center[1]/table[2]/tr[1]/td[1]/table[1] 
            hdc.LoadHtml(doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/center[1]/table[2]/tr[1]/td[1]/table[1]").InnerHtml);

            // 取得個股標頭 
            HtmlNodeCollection htnode = hdc.DocumentNode.SelectNodes("./tr[1]/th");
            // 取得個股數值 
            string[] txt = hdc.DocumentNode.SelectSingleNode("./tr[2]").InnerText.Trim().Split('\n');
            int i = 0;

            // 輸出資料 
            foreach (HtmlNode nodeHeader in htnode)
            {
                //將 "加到投資組合" 這個字串過濾掉
                Response.Write(nodeHeader.InnerText + ":" + txt[i].Trim().Replace("加到投資組合", "") + "");
                i++;
            }

            //清除資料
            doc = null;
            hdc = null;
            url = null;
            ms.Close();
            return View();
        }

        
    }
}