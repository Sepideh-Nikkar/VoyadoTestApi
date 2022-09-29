using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Text.Json;
using System.Text.RegularExpressions;
using VoyadoWebAppTest.Models;

namespace VoyadoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class apiController : ControllerBase
    {
        readonly string mainWikiUrl = "https://en.wikipedia.org/w/api.php?action=query&list=search&srsearch=";
        readonly string mainGithubUrl = "https://api.github.com/search/code?q=";

        [HttpGet(Name = "GetSearchHits")]
        public string GetSearchHits(string keyword)
        {
            var validKeyword = Validate(keyword);
            var keywords = validKeyword.Split(' ');

            var isWikiResponseOk = false;
            var isGithubResponseOk = false;
            var isResponseOk = false;

            var searchHitsOnWiki = 0;
            var searchHitOnGithub = 0;

            for (int i = 0; i < keywords.Length; i++)
            {
                var endpointWiki = mainWikiUrl + "{" + keywords[i] + "}&utf8=&format=json";
                var responseWiki = GetSearchResponse(endpointWiki, false, out isWikiResponseOk);
                var temp = GetSearchHits(responseWiki, SeachEngineType.Wikipedia);
                if (temp >= 0)
                {
                    searchHitsOnWiki += temp;
                };

                var endPointGitHub = mainGithubUrl + keywords[i] + "+repo:jquery/jquery";
                var responseGithub = GetSearchResponse(endPointGitHub, true, out isGithubResponseOk);
                temp = GetSearchHits(responseGithub, SeachEngineType.Github);
                if (temp >= 0)
                {
                    searchHitOnGithub += temp;
                }
            }

            if (isWikiResponseOk && isGithubResponseOk)
            {
                isResponseOk = true;
            }

            var returnObj = new ApiResponseModel(searchHitsOnWiki, searchHitOnGithub, isResponseOk);
            var result = JsonSerializer.Serialize(returnObj);
            return result;
        }

        private static string Validate(string keyword)
        {
            keyword = keyword.Trim();
            keyword = Regex.Replace(keyword, @"\s+", " ");
            return keyword;
        }

        private static string GetSearchResponse(string url, bool authorization, out bool isResponseOk)
        {
            var returnVal = "";
            isResponseOk = true;
            var client = new RestClient("https://api.github.com/search/code?q=ask+repo:jquery/jquery");
            var request = new RestRequest(url, Method.Get);
            if (authorization == true)
            {
                request.AddHeader("Authorization", "Bearer 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");
            }

            try
            {
                var response = client.Execute(request);
                returnVal = response != null ? response.Content : "";
                if (!response.IsSuccessStatusCode)
                {
                    isResponseOk = false;
                }
            }
            catch
            {
                isResponseOk = false;
            }

            return returnVal;
        }

        private static int GetSearchHits(string response, SeachEngineType engineType)
        {
            int returnVal = -1;

            if (response == null) return returnVal;

            switch (engineType)
            {
                case SeachEngineType.Wikipedia:
                    {
                        var contentResponse = JsonSerializer.Deserialize<WikiResponseModel>(response);
                        if (contentResponse != null)
                        {
                            returnVal = contentResponse.query.searchinfo.totalhits;
                        }
                    }
                    break;
                case SeachEngineType.Github:
                    {
                        var gitHubResponseModel = JsonSerializer.Deserialize<GitHubResponseModel>(response);
                        if (gitHubResponseModel != null)
                        {
                            return gitHubResponseModel.total_count;
                        }
                    }
                    break;
                default:
                    break;
            }

            return returnVal;
        }

        private enum SeachEngineType
        {
            Wikipedia,
            Github
        }
    }
}
