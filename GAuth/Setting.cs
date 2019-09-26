using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace GAuth
{
    [DataContract(Name = "Setting")]
    public class Json
    {
        [DataMember(Name = "secret")]
        public string secret { get; set; }
    }

    public static class Setting
    {
        public static Json readSetting(string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path))) throw new DirectoryNotFoundException();

            Json json = null;
            if (!File.Exists(path))
            {
                json = new Json();
                json.secret = string.Empty;
                return json;
            }
            else
            {
                byte[] bytes;
                using (FileStream fs = File.OpenRead(path))
                {
                    int length = (int)fs.Length;
                    bytes = new byte[length];
                    fs.Read(bytes, 0, length);
                }

                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Json));
                    json = (Json)serializer.ReadObject(ms);
                }
            }
            return json;
        }

        public static int writeSetting(string path, Json json)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path))) throw new DirectoryNotFoundException();

            byte[] bytes;
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Json));
                serializer.WriteObject(ms, json);
                bytes = ms.ToArray();
            }

            int length = bytes.Length;
            using (FileStream fs = File.OpenWrite(path))
            {
                fs.Write(bytes, 0, length);
                fs.Flush();
            }
            return length;
        }
    }
}
