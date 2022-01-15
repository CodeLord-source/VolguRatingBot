using RatingBot.Services.LkVolsuParsing;

namespace RatingBot.Services.Parser
{
    public class ParserWorker
    {
        private readonly LkVolsuParser _parser;
        private readonly LkVolsuWebRequestsSender _requestSender;
        private readonly IHtmlDocLoader _docLoader;

        public ParserWorker(LkVolsuParser parser, LkVolsuWebRequestsSender requestSender, IHtmlDocLoader docLoader)
        {
            _parser = parser;
            _requestSender = requestSender;
            _docLoader = docLoader;
        }

        public async Task<bool> AuthorizationIsSuccess(string password, string login)
        {
            var loginResponse = await _requestSender.SendToLoginFormAsync(login, password);
            var loginContent = loginResponse.Content.ReadAsStream();
            var loginResponseDocument = await _docLoader.LoadAsync(loginContent);

            return _parser.ParseAuthorizationResponse(loginResponseDocument);
        }

        public async Task<string> GetDataMessage(string password, string login, int semester)
        {
            var loginResponse = await _requestSender.SendToLoginFormAsync(login, password);
            var loginContent = loginResponse.Content.ReadAsStream();
            var loginResponseDocument = await _docLoader.LoadAsync(loginContent);

            if (_parser.ParseAuthorizationResponse(loginResponseDocument) == false)
            {
                throw new Exception("Ошибка входа на сайт.");
            }

            var getDataPageResponse = await _requestSender.SendToDataPageAsync();
            var dataContent = getDataPageResponse.Content.ReadAsStream();
            var dataDocument = await _docLoader.LoadAsync(dataContent);
            var messageString = _parser.ParseDataPage(dataDocument, semester);

            return messageString;
        }
    }
}
