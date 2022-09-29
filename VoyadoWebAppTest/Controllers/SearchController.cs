using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Text.Json;
using System.Text.RegularExpressions;
using VoyadoWebAppTest.Models;
using System.Configuration;

namespace VoyadoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class apiController : ControllerBase
    {
        readonly string mainWikiUrl = "https://en.wikipedia.org/w/api.php?action=query&list=search&srsearch=";
        readonly string mainGithubUrl = "https://api.github.com/search/code?q=";

        [HttpGet(Name = "GetSearchHits")]
        public string GetSearchHits(string keyword, string githubApiToken)
        {
            var validKeyword = Validate(keyword);
            var keywords = validKeyword.Split(' ');

            var isWikiResponseOk = false;
            var isGithubResponseOk = false;

            var searchHitsOnWiki = 0;
            var searchHitOnGithub = 0;

            for (int i = 0; i < keywords.Length; i++)
            {
                var endpointWiki = mainWikiUrl + "{" + keywords[i] + "}&utf8=&format=json";
                var responseWiki = GetSearchResponse(endpointWiki, out isWikiResponseOk);
                var temp = GetSearchHits(responseWiki, SeachEngineType.Wikipedia);
                if (isWikiResponseOk && temp > 0)
                {
                    searchHitsOnWiki += temp;
                };

                var endPointGitHub = mainGithubUrl + keywords[i] + "+repo:jquery/jquery";
                var responseGithub = GetSearchResponse(endPointGitHub, out isGithubResponseOk, githubApiToken);
                temp = GetSearchHits(responseGithub, SeachEngineType.Github);
                if (isGithubResponseOk && temp > 0)
                {
                    searchHitOnGithub += temp;
                }
            }

            var returnObj = new ApiResponseModel(searchHitsOnWiki, searchHitOnGithub, isWikiResponseOk, isGithubResponseOk);
            var result = JsonSerializer.Serialize(returnObj);
            return result;
        }

        private static string Validate(string keyword)
        {
            keyword = keyword.Trim();
            keyword = Regex.Replace(keyword, @"\s+", " ");
            return keyword;
        }

        private static string GetSearchResponse(string url, out bool isResponseOk, string githubToken = "")
        {
            var returnVal = "";
            isResponseOk = false;
            var client = new RestClient("https://api.github.com/search/code?q=ask+repo:jquery/jquery");
            var request = new RestRequest(url, Method.Get);
            if (githubToken != String.Empty)
            {
                request.AddHeader("Authorization", "Bearer " + githubToken);
            }

            var response = client.Execute(request);
            if (response.IsSuccessStatusCode)
            {
                isResponseOk = true;
                returnVal = response.Content;
            }

            return returnVal;
        }

        private static int GetSearchHits(string response, SeachEngineType engineType)
        {
            int returnVal = -1;

            if (response == String.Empty) return returnVal;

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
