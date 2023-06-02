using Newtonsoft.Json;
using Sonarqube.Analyzer.Request;
using Sonarqube.Analyzer.Response.Analyzer;
using Sonarqube.Analyzer.Response.ProjectsInfo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Sonarqube.Analyzer
{
    class Program
    {
        private static readonly string sonarQubeUrl = ConfigurationManager.AppSettings["sonarQubeUrl"];
        private static readonly string sonarQubeUsername = ConfigurationManager.AppSettings["sonarQubeUsername"];
        private static readonly string sonarQubePassword = ConfigurationManager.AppSettings["sonarQubePassword"];
        private static readonly string types = ConfigurationManager.AppSettings["types"];
        private static readonly string statuses = ConfigurationManager.AppSettings["statuses"];
        private static readonly string projectSonrStatusUrl = ConfigurationManager.AppSettings["projectSonrStatusUrl"];

        private enum AnalyzeType
        {
            BUG, CODE_SMELL, VULNERABILITY
        }

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var projectKeys = await GetProjectKeys();

            HttpClient client = SetUpHttpClent();

            List<AnalyzerRequest> analyzerRequestList = new List<AnalyzerRequest>();

            await FetchAnalyticsData(projectKeys, client, analyzerRequestList);

            await DispatchAnalyzerResponse(analyzerRequestList);
        }

        private static async Task FetchAnalyticsData(List<string> projectKeys, HttpClient client, List<AnalyzerRequest> analyzerRequestList)
        {
            foreach (var projectKey in projectKeys)
            {
                foreach (var type in types.Split(','))
                {
                    string apiUrl = $"/api/issues/search?projects={projectKey}&types={type}&statuses={statuses}";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($" Success response from  {apiUrl}  @ {DateTime.UtcNow}");

                        string responseBody = await response.Content.ReadAsStringAsync();
                        var responseAnalyzer = JsonConvert.DeserializeObject<ResponseAnalyzer>(responseBody);

                        SetUpDispatchRequest(responseAnalyzer, projectKey, analyzerRequestList, type);
                    }
                    else
                    {
                        Console.WriteLine($" Failed response from  {apiUrl}  @ {DateTime.UtcNow}");
                    }
                }
            }
        }

        private static async Task DispatchAnalyzerResponse(List<AnalyzerRequest> analyzerRequestList)
        {
            if (analyzerRequestList.Count > 0)
            {
                using (HttpClient client = new HttpClient())
                {
                    string requestBody = JsonConvert.SerializeObject(analyzerRequestList);

                    HttpContent content = new StringContent(requestBody);

                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await client.PostAsync(projectSonrStatusUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($" Success response from  {projectSonrStatusUrl}  @ {DateTime.UtcNow}");
                        return;
                    }
                    Console.WriteLine($" Failed response from  {projectSonrStatusUrl}  @ {DateTime.UtcNow}");
                }
            }
        }

        private static void SetUpDispatchRequest(ResponseAnalyzer responseAnalyzer, string projectKey, List<AnalyzerRequest> analyzerRequestList, string type)
        {
            var projectAnalyzer = analyzerRequestList.FirstOrDefault(x => x.Key == projectKey);
            int count = responseAnalyzer.total;

            var analyzer = (projectAnalyzer == null ? new AnalyzerRequest() : projectAnalyzer);

            if (type == Enum.GetName(typeof(AnalyzeType), AnalyzeType.BUG))
                analyzer.Bugs = new Bugs() { Statuses = statuses, Count = count };
            if (type == Enum.GetName(typeof(AnalyzeType), AnalyzeType.CODE_SMELL))
                analyzer.CodeSmell = new CodeSmell() { Statuses = statuses, Count = count };
            if (type == Enum.GetName(typeof(AnalyzeType), AnalyzeType.VULNERABILITY))
                analyzer.Vulnerability = new Vulnerability() { Statuses = statuses, Count = count };

            if (projectAnalyzer == null)
            {
                analyzer.Key = projectKey;
                analyzerRequestList.Add(analyzer);
            }
        }

        private static async Task<List<string>> GetProjectKeys()
        {
            HttpClient client = SetUpHttpClent();

            string apiUrl = $"/api/projects/search";

            HttpResponseMessage response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($" Success response from  {apiUrl}  @ {DateTime.UtcNow}");

                string responseBody = await response.Content.ReadAsStringAsync();
                var projectsInfo = JsonConvert.DeserializeObject<ProjectsInfo>(responseBody);

                return projectsInfo.components.Select(x => x.key).ToList();
            }

            Console.WriteLine($" Failed response from  {apiUrl}  @ {DateTime.UtcNow}");

            return default(List<string>);
        }

        private static HttpClient SetUpHttpClent()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(sonarQubeUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{sonarQubeUsername}:{sonarQubePassword}")));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}
