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
            Response.ContentType = "image/jpeg";

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
            //Get lucky level and choose picture

            var initImage = System.Drawing.Image.FromFile("C:/DrawMyLuck/BigLuck.png");
            resultGraphic = Graphics.FromImage(initImage);
            drawLuckLevel();
            drawContext(name, 0);
            drawSubject(subject);
            drawDate(year, month, day);
      
            var ms = new MemoryStream();
            initImage.Save(ms, ImageFormat.Jpeg);
            Response.BinaryWrite(ms.ToArray());
            initImage.Dispose();
            Response.End();
        }

        private void drawLuckLevel()
        {
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            Rectangle rect = new Rectangle(0, 200, 657, 240);

            Font theFont = new Font("康煕字典體(Demo)", 43.97f, System.Drawing.FontStyle.Regular);
            Color theColor = Color.FromArgb(199, 68, 52);
            Brush theBrush = new SolidBrush(theColor);
            resultGraphic.DrawString(luckyNode.ChildNodes.Item(0).InnerText, theFont, theBrush, rect, stringFormat);
 
        }

        private void drawContext(string name, int type)
        {
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;


            Font theFont = new Font("微软雅黑", 6.48f, System.Drawing.FontStyle.Bold);
            Color theColor = Color.FromArgb(0, 0, 0);
            Brush theBrush = new SolidBrush(theColor);

            var nodes = luckyNode.ChildNodes;
            for(var i=1; i<nodes.Count; i++)
            {
                Rectangle rect = new Rectangle(0, 420+42*(i-1), 657, 36);
                var sentence = nodes.Item(i).InnerText.Replace("（）", name);
                resultGraphic.DrawString(sentence, theFont, theBrush, rect, stringFormat);
            }

        }

        private void drawSubject(string subject)
        {
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            Font theFont = new Font("思源黑体 CN Heavy", 14.46f, System.Drawing.FontStyle.Bold);
            Color theColor = Color.FromArgb(255,255,255);
            Brush theBrush = new SolidBrush(theColor);

            Rectangle rect = new Rectangle(0, 40, 657, 100);

            resultGraphic.DrawString("《"+subject+"》", theFont, theBrush, rect, stringFormat);
        }

        private void drawDate(string year, string month, string day)
        {
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            Font theFont = new Font("思源黑体 CN Regular", 4.98f, System.Drawing.FontStyle.Regular);
            Color theColor = Color.FromArgb(255, 255, 255);
            Brush theBrush = new SolidBrush(theColor);

            Rectangle rect = new Rectangle(0, 116, 657, 32);

            resultGraphic.DrawString(year+"年"+month+"月"+day+"日", theFont, theBrush, rect, stringFormat);
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