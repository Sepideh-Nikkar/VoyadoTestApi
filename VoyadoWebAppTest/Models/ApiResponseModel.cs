namespace VoyadoWebAppTest.Models
{
    public class ApiResponseModel
    {
        public ApiResponseModel(int wikiTotalHit, int gitHubTotalHit, bool isWikiResponseOk, bool isGithubResponseOk)
        {
            this.wikipedia = wikiTotalHit;
            this.gitHub = gitHubTotalHit;
            this.isWikiResponseOk = isWikiResponseOk;
            this.isGithubResponseOk = isGithubResponseOk;

        }

        public int wikipedia { get; set; }
        public int gitHub { get; set; }
        public bool isWikiResponseOk { get; set; }
        public bool isGithubResponseOk { get; set; }

    }
}
