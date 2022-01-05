namespace Yuyuyui.PrivateServer
{
    public class AccessoryListEntity : BaseEntity<AccessoryListEntity>
    {
        public AccessoryListEntity(
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
            var player = GetPlayerFromCookies();

            if (player.accessories.Count == 0)
            {
                var newGyuuki = Accessory.DefaultAccessory();
                player.accessories.Add($"{newGyuuki.id}", newGyuuki);
                player.Save();
                Utils.Log("Assigned default accessory to player.");
            }

            Utils.LogWarning("Stub API!");

            Response responseObj = new()
            {
                accessories = player.accessories
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        public class Response
        {
            public IDictionary<string, Accessory> accessories = new Dictionary<string, Accessory>();
        }
    }
}