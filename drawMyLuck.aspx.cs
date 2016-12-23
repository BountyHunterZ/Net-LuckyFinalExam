using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace LuckyFinalExam
{
    public partial class drawMyLuck : System.Web.UI.Page
    {
        private Graphics resultGraphic;
        private XmlDocument xmlDoc = new XmlDocument();
        private XmlNode luckyNode;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ClearContent();
            Response.ContentType = "image/PNG";

            string name = Request.Params["name"];
            string subject = Request.Params["subject"];
            string year = Request.Params["year"];
            string month = Request.Params["month"];
            string day = Request.Params["day"];

            int finalNum = 0;
            finalNum += encodeToNum(name);
            finalNum += encodeToNum(subject);
            finalNum += encodeToNum(year);
            finalNum += encodeToNum(month);
            finalNum += encodeToNum(day);

            xmlDoc.Load("C:/DrawMyLuck/expressions.xml");
            XmlNode rootNode = xmlDoc.SelectSingleNode("Table");
            XmlNodeList expressions = rootNode.ChildNodes;
            var totalSentence = expressions.Count;
            int luckyNum = finalNum % totalSentence;
            luckyNode = expressions.Item(luckyNum);
            /*
            if (name == "钟典")
            {
                luckyNode.ChildNodes.Item(0).InnerText = "多吉";
            }

            else if (name == "李童茜")
            {
                luckyNode.RemoveAll();
                for(var i=0; i<4; i++)
                {
                    var newElement = xmlDoc.CreateElement("express");
                    luckyNode.AppendChild(newElement);
                }
                luckyNode.ChildNodes.Item(0).InnerText = "爆棚";
                luckyNode.ChildNodes.Item(1).InnerText = "都说漂亮的女孩子运气不会太差";
                luckyNode.ChildNodes.Item(2).InnerText = "何况是最漂亮的"+name+"呢";
                luckyNode.ChildNodes.Item(3).InnerText = "——哲学家呆似萌";
            }
            */

            var initImage = System.Drawing.Image.FromFile("C:/DrawMyLuck/BigLuck.png");
            resultGraphic = Graphics.FromImage(initImage);
            drawLuckLevel();
            drawContext(name, 0);
            drawSubject(subject);
            drawDate(year, month, day);
      
            var ms = new MemoryStream();
            initImage.Save(ms, ImageFormat.Png);
            Response.BinaryWrite(ms.ToArray());
            initImage.Dispose();
            Response.End();
        }

        private void drawLuckLevel()
        {

            Font aFont = new Font("方正榜书行简体", 33.94f, System.Drawing.FontStyle.Regular);
            Font bFont = new Font("方正榜书行简体", 61.93f, System.Drawing.FontStyle.Regular);
            Color theColor = Color.FromArgb(199, 68, 52);
            Brush theBrush = new SolidBrush(theColor);
            resultGraphic.DrawString("过", bFont, theBrush, 325, 175);
            int startX = -10, gapX = 120;
            for (var i = 0; i < 3; i++)
            {
                resultGraphic.DrawString(luckyNode.ChildNodes.Item(0).InnerText.Substring(i, 1), aFont, theBrush, startX + gapX * i, 245);
            }
        }

        private void drawContext(string name, int type)
        {
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            float fontsize;
            int startY, gapY;

            var nodes = luckyNode.ChildNodes;
            switch (nodes.Count)
            {
                case 3: fontsize = 8.06f; startY = 440; gapY = 50; break;
                case 4: fontsize = 6.13f; startY = 430; gapY = 42; break;
                default: fontsize = 5.8f; startY = 420; gapY = 38; break;
            }
            for(var i=1; i<nodes.Count; i++)
            {
                Font aFont = new Font("微软雅黑", fontsize, System.Drawing.FontStyle.Bold);
                Color aColor = Color.FromArgb(0, 0, 0);
                Brush aBrush = new SolidBrush(aColor);

                Rectangle rect = new Rectangle(0, startY+gapY*(i-1), 657, 36);
                var sentence = nodes.Item(i).InnerText.Replace("（）", name);
                resultGraphic.DrawString(sentence, aFont, aBrush, rect, stringFormat);
            }

            Font bFont = new Font("思源黑体 CN Heavy", 7.48f, System.Drawing.FontStyle.Bold);
            Color bColor = Color.FromArgb(255, 255, 255);
            Brush bBrush = new SolidBrush(bColor);

            var bSize = resultGraphic.MeasureString(name + "的", bFont);
            var waterMark = System.Drawing.Image.FromFile("C:/DrawMyLuck/watermark.png");
            var totalWidth = (float)waterMark.Size.Width + bSize.Width - 4;
            var startX = (int)((657.0 - totalWidth) / 2);
            resultGraphic.DrawString(name + "的", bFont, bBrush, startX, 28);
            resultGraphic.DrawImage(waterMark, startX + (int)bSize.Width -4, 20);
        }

        private void drawSubject(string subject)
        {
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            Font theFont = new Font("思源黑体 CN Heavy", 14.46f, System.Drawing.FontStyle.Bold);
            Color theColor = Color.FromArgb(255,255,255);
            Brush theBrush = new SolidBrush(theColor);

            Rectangle rect = new Rectangle(0, 70, 657, 100);

            resultGraphic.DrawString("《"+subject+"》", theFont, theBrush, rect, stringFormat);
        }

        private void drawDate(string year, string month, string day)
        {
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            Font theFont = new Font("思源黑体 CN Regular", 4.23f, System.Drawing.FontStyle.Regular);
            Color theColor = Color.FromArgb(255, 255, 255);
            Brush theBrush = new SolidBrush(theColor);

            Rectangle rect = new Rectangle(0, 142, 657, 32);

            resultGraphic.DrawString("考试时间 | "+year+"年"+month+"月"+day+"日", theFont, theBrush, rect, stringFormat);
        }
        
        private int encodeToNum(string input)
        {
            if(input == null)
            {
                return 0;
            }
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(input);
            int result = 0;
            foreach (byte oneByte in encodedBytes) {
                result += (int)oneByte;
            }
            return result;
        }
    }
}