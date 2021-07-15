namespace PixelIT.web
{
    public static class Globe
    {
        public static Config Config { get; set; }
        public static string ConnectionString
        {
            get
            {
                return $"Server={Config.MYSQL_HOST};Uid={Config.MYSQL_USER};Pwd={Config.MYSQL_PASSWORD};Database={Config.MYSQL_DATABASE};Connection Timeout=2000;CharSet=utf8;";
            }
        }
    }

    public class Config
    {
        public string MYSQL_HOST { get; set; }
        public string MYSQL_DATABASE { get; set; }
        public string MYSQL_USER { get; set; }
        public string MYSQL_PASSWORD { get; set; }
        public string DOWNLOAD_URL { get; set; }
    }
}
