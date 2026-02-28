using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using System.Text;
using Ude;

namespace Applet.Nat.Ux.Models
{
    public static class Format
    {
        public static string Compress(string value)
        {
            //Transform string into byte[]  
            byte[] byteArray = new byte[value.Length];
            int indexBA = 0;
            foreach (char item in value.ToCharArray())
            {
                byteArray[indexBA++] = (byte)item;
            }
            return Compress(byteArray);
        }
        public static string Compress(byte[] _byteArray)
        {
            //Prepare for compress
            MemoryStream ms = new MemoryStream();
            System.IO.Compression.GZipStream sw = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress);

            //Compress
            sw.Write(_byteArray, 0, _byteArray.Length);
            //Close, DO NOT FLUSH cause bytes will go missing...
            sw.Close();

            //Transform byte[] zip data to string
            _byteArray = ms.ToArray();
            System.Text.StringBuilder sB = new System.Text.StringBuilder(_byteArray.Length);
            foreach (byte item in _byteArray)
            {
                sB.Append((char)item);
            }
            ms.Close();
            sw.Dispose();
            ms.Dispose();
            return Convert.ToBase64String(_byteArray);
        }
        public static string UnCompress(string value, Encoding vioEncoding)
        {
            return vioEncoding.GetString(UnCompress2(value));
        }
        public static byte[] UnCompress2(string value)
        {
            byte[] byteArray = Convert.FromBase64String(value);
            //Prepare for decompress
            using (var mso = new MemoryStream())
            {
                MemoryStream ms = new System.IO.MemoryStream(byteArray);
                System.IO.Compression.GZipStream sr = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress);
                sr.CopyTo(mso);
                sr.Close();
                ms.Close();
                sr.Dispose();
                ms.Dispose();
                return mso.ToArray();

            }

        }
        public static string RemoveSpacesFromJson(string vivstrJson)
        {
            if (string.IsNullOrEmpty(vivstrJson))
                return vivstrJson;

            var lioJson = JsonConvert.DeserializeObject(vivstrJson);
            return JsonConvert.SerializeObject(lioJson, Newtonsoft.Json.Formatting.None);
        }

        public static async Task<Encoding> DetectEncoding(IBrowserFile file)
        {
            using (Stream stream = file.OpenReadStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.Default, true))
            {
                char[] buffer = new char[1];
                if (await reader.ReadAsync(buffer, 0, 1) > 0) // Asynchronous read
                {
                }
                return reader.CurrentEncoding;
            }
        }
    }
}

