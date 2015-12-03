using System;
using System.IO;
using Simple.Json;

namespace Ipsync.Model
{
    [Serializable]
    public class Configuration
    {
        private int _delaySeconds;
        public string DropbopxPath { get; set; }

        public bool Enabled { get; set; }

        public int DelaySeconds
        {
            get
            {
                if (_delaySeconds <= 0)
                    _delaySeconds = 30;
                return _delaySeconds;
            }
            set { _delaySeconds = value; }
        }

        public bool Initialized { get; private set; }

        public static Configuration Load()
        {
            try
            {
                var configContent = File.ReadAllText(Constants.CONFIG_FILENAME);
                var config = JsonSerializer.Default.ParseJson<Configuration>(configContent);
                config.Initialized = true;
                return config;
            }
            catch (Exception e)
            {
                if (!(e is FileNotFoundException))
                {
                    Console.WriteLine(e);
                }
                return new Configuration { Initialized = false };
            }
        }

        public static void Save(Configuration config)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(File.Open(Constants.CONFIG_FILENAME, FileMode.Create)))
                {
                    string jsonString = JsonSerializer.Default.ToJson(config);
                    sw.Write(jsonString);
                    sw.Flush();
                }
            }
            catch (IOException e)
            {
                Console.Error.WriteLine(e);
            }
        }
    }
}