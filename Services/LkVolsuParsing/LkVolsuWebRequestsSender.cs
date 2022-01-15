namespace RatingBot.Services.LkVolsuParsing
{
    public class LkVolsuWebRequestsSender
    {
        private readonly RequestsSettings _settings;

        public LkVolsuWebRequestsSender(RequestsSettings settings)
        {
            _settings = settings;
        }

        public async Task<HttpResponseMessage> SendToLoginFormAsync(string password, string login)
        {
            var httpClient = new HttpClient();
            var values = new Dictionary<string, string>
            {
                {"LoginForm[identity]",$"{login}"},
                {"LoginForm[password]",$"{password}" },
                {"LoginForm[rememberMe]","0" },
                {"login-button","" }
            };
            var content = new FormUrlEncodedContent(values);
            var response = await httpClient.PostAsync(_settings.LoginFormUrl, content);

            return response;
        }

        public async Task<HttpResponseMessage> SendToDataPageAsync()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(_settings.DataPageUrl);

            return response;
        }
    }
}
