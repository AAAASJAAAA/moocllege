using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace moocollege
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string cookie = "";
        string courseId = "";
        private void button2_Click(object sender, EventArgs e)
        {
            courseId = textBox3.Text;
            cookie = textBox1.Text;

            if (courseId == "" && cookie == "")
            {
                MessageBox.Show("参数为空");
                return;
            }
            string postdata = "{\"courseId\":\""+ courseId + "\"}";
            string res = SendDataByPost1("http://student.zjedu.moocollege.com/nodeapi/3.0.1/student/course/plan/list", postdata, cookie);
            JObject jsonObj = JObject.Parse(res);
            if (jsonObj["code"].ToString() == "20000")
            {
                foreach (var item in jsonObj["data"])
                {
                    foreach (var item1 in item["data"])
                    {
                        foreach (var item2 in item1["data"])
                        {
                            try
                            {
                                if (item2["next"]["status"].ToString() == "0" || item2["next"]["status"].ToString() == "1")
                                {
                                    if (item2["next"]["type"].ToString() == "3")
                                    {
                                        textBox2.Text+=(item2["next"]["name"].ToString())+ (item2["next"]["unitId"].ToString()+"：文档")+"\r\n";
                                        string postdata1 = "{\"unitId\":"+ item2["next"]["unitId"].ToString() + ",\"courseId\":\""+ courseId + "\",\"playPosition\":0}";
                                        string res1 = SendDataByPost1("http://student.zjedu.moocollege.com/nodeapi/3.0.1/student/course/uploadLearnRate", postdata1, cookie);
                                        textBox2.Text+=res1 + "\r\n";
                                    }
                                    else if((item2["next"]["type"].ToString() == "1"))
                                    {
                                        textBox2.Text+=(item2["next"]["name"].ToString()) + (item2["next"]["unitId"].ToString()+"：视频") + "\r\n";
                                        int playpostion = 0;
                                        while (true)
                                        {
                                            string postdata1 = "{\"unitId\":" + item2["next"]["unitId"].ToString() + ",\"courseId\":\""+ courseId + "\",\"playPosition\":"+playpostion+"}";
                                            string res1 = SendDataByPost1("http://student.zjedu.moocollege.com/nodeapi/3.0.1/student/course/uploadLearnRate", postdata1, cookie);
                                            playpostion += 20;
                                            JObject jsonObj1 = JObject.Parse(res1);
                                            if (jsonObj1["code"].ToString() == "20000")
                                            {
                                                textBox2.Text+=jsonObj1.ToString() + "\r\n";
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    
                                }
                            }
                            catch (Exception ex)
                            {
                                textBox2.Text+=ex.ToString() + "\r\n";
                            }
                        }
                    }
                }
            }
            else
            {
                textBox2.Text+=(jsonObj.ToString()) + "\r\n";
            }
        }

        public string SendDataByPost1(string Url, string postDataStr, string cookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Safari/537.36";
            request.Headers.Add("Cookie", cookie);
            request.ContentLength = postDataStr.Length;
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
    }
}
