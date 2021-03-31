using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace httpRequestTest
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpUploadFile("http://3.35.205.168:5022/api/analyze", @"fundus.jpg", @"D:\TTT\fundus.jpg");
        }

        public static void HttpUploadFile(string url, string fileName, string filePath)
        {
            string boundary = "--######################--";
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.UserAgent = ".NET Framework Test Client";
            //wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

            // filename
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, "file_name", "fundus.jpg");
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }

            // settings
            {
                var settings = new List<Tuple<String, int>>()
                {
                    new Tuple<String, int>("Drusen & Drusenoid Deposits", 2),
                    new Tuple<String, int>("Hemorrhage", 2),
                    new Tuple<String, int>("Hard Exudate", 2),
                    new Tuple<String, int>("Cotton Wool Patch", 2),
                    new Tuple<String, int>("Vascular Abnormality", 2),
                    new Tuple<String, int>("Glaucomatous Disc Change", 2),
                    new Tuple<String, int>("RNFL Defect", 2),
                    new Tuple<String, int>("Membrane", 2),
                    new Tuple<String, int>("Chorioretinal Atrophy/Scar", 2),
                    new Tuple<String, int>("Non-glaucomatous Disc Change", 2),
                    new Tuple<String, int>("Macular Hole", 2),
                    new Tuple<String, int>("Myelinated Nerve Fiber", 2),
                };

                String strSettings = "";
                strSettings += "[";
                foreach (var item in settings)
                {
                    strSettings += "{\"name\":\"";
                    strSettings += item.Item1;
                    strSettings += "\", \"sensitivity\": ";
                    strSettings += item.Item2;
                    strSettings += "},";
                }
                strSettings = strSettings.Remove(strSettings.Length - 1);
                strSettings += "]";

                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, "settings", strSettings);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }

            // force_gradable
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, "force_gradable", "False");
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }

            // file_data
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);

                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, "file_data", "file_data", "image/jpg");
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);

                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[fileStream.Length];
                int bytesRead = fileStream.Read(buffer, 0, buffer.Length);

                rs.Write(buffer, 0, bytesRead);

                fileStream.Close();
            }

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
            }
            catch (Exception ex)
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
        }
    }
}
