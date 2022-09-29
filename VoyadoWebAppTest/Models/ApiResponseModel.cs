namespace VoyadoWebAppTest.Models
{
    public class ApiResponseModel
    {
        public ApiResponseModel(int wikiTotalHit, int gitHubTotalHit, bool isResponseOk)
        {
            this.wikipedia = wikiTotalHit;
            this.gitHub = gitHubTotalHit;
            this.isResponseOk = isResponseOk;
        }

        public int wikipedia { get; set; }
        public int gitHub { get; set; }
        public bool isResponseOk { get; set; }
    }
}
