using HtmlAgilityPack; 

namespace RatingBot.Services.Parser
{
    public class ParserWorker<T> where T : class
    {
        IParser<T> _parser;

        public ParserWorker(IParser<T> parser)
        {
            _parser = parser;
        }

        public async Task<bool> CheckAuthorization(string log, string pass)
        {
            var _httpClient = new HttpClient();
            var values = new Dictionary<string, string>
            {
                {"LoginForm[identity]",$"{log}"},
                {"LoginForm[password]",$"{pass}" },
                {"LoginForm[rememberMe]","0" },
                {"login-button","" }
            };
            var content = new FormUrlEncodedContent(values);

            var response = await _httpClient.PostAsync("https://lk.volsu.ru/user/sign-in/login", content);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            { 
                return true;
            }

            return false;
        }

        public async Task<T> GetDataAsync(string log, string pass, int? semester)
        {
            var _httpClient = new HttpClient();
            var values = new Dictionary<string, string>
            {
                {"LoginForm[identity]",$"{log}"},
                {"LoginForm[password]",$"{pass}" },
                {"LoginForm[rememberMe]","0" },
                {"login-button","" }
            };
            var content = new FormUrlEncodedContent(values);
            var doc = new HtmlDocument();

            await _httpClient.PostAsync("https://lk.volsu.ru/user/sign-in/login", content);
            var response = await _httpClient.GetAsync("https://lk.volsu.ru/student/grade?id=0");
            doc.Load(response.Content.ReadAsStream());

            return _parser.Parse(doc, semester);
        }
    }
}
