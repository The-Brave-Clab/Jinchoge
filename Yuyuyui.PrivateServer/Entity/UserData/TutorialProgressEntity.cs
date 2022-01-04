using System.Text;

namespace Yuyuyui.PrivateServer
{
    public class TutorialProgressEntity : BaseEntity<TutorialProgressEntity>
    {
        public const string TUTORIAL_PROGRESS_SAVE_FILE = "tutorial_progress.json";

        public TutorialProgressEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            Config config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            bool isSession = this.GetSessionFromCookie(out var playerSession);
            if (!isSession)
            {
                throw new Exception("Session not found!");
            }

            string playerDataDir = PrivateServer.EnsurePlayerDataFolder(playerSession.player);
            string tutorialProgressFile = Path.Combine(playerDataDir, TUTORIAL_PROGRESS_SAVE_FILE);

            if (requestBody.Length > 0)
            {
                Console.WriteLine(Encoding.UTF8.GetString(requestBody));
                File.WriteAllBytes(tutorialProgressFile, requestBody);
            }

            if (!File.Exists(tutorialProgressFile))
            {
                File.WriteAllText(tutorialProgressFile, "{\"progress\": 0}");
            }

            byte[] progressFileContent = File.ReadAllBytes(tutorialProgressFile);
            Request request = Deserialize<Request>(progressFileContent)!;

            Response responseObj = new()
            {
                progress = request.progress,
                tutee = request.progress != 1000
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders(playerSession.sessionID);

            return Task.CompletedTask;
        }

        public class Request
        {
            public int progress { get; set; }
        }

        public class Response
        {
            public bool tutee { get; set; }
            public int progress { get; set; }
        }
    }
}