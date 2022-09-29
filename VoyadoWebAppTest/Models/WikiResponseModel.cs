namespace VoyadoWebAppTest.Models
{
    public class WikiResponseModel
    {
        public Query query { get; set; }
    }

    public class Query
    {
        public SearchInfo searchinfo { get; set; }
    }
    public class SearchInfo
    {
        public int totalhits { get; set; }
    }
}
