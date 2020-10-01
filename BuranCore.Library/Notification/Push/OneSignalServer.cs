using Buran.Core.Library.Http;
using Buran.Core.Library.Utils;

namespace Buran.Core.Library.Notification.Push
{
    public class OneSignalServer
    {
        private const string Url = "https://onesignal.com/api/v1/notifications";
        private readonly string _key;
        private readonly string _appId;

        public class OnePushContent
        {
            public string en { get; set; }
        }

        public class OnePushBaseModel
        {
            public string app_id { get; set; }
            public OnePushContent contents { get; set; }
            public OnePushContent headings { get; set; }
            public bool content_available { get; set; }
            public bool mutable_content { get; set; }
        }

        public class OnePushDeviceModel : OnePushBaseModel
        {
            public string[] include_player_ids { get; set; }
        }
        public class OnePushUserModel : OnePushBaseModel
        {
            public string[] include_external_user_ids { get; set; }
        }

        public OneSignalServer(string key, string appId)
        {
            _key = key;
            _appId = appId;
        }

        public string PushToDevice(string to, string title, string body, bool contentAvailable, bool mutableContent)
        {
            var client = new WebRequest2();

            var msg = new OnePushDeviceModel
            {
                app_id = _appId,
                contents = new OnePushContent { en = body },
                headings = new OnePushContent { en = title },
                include_player_ids = new[] { to },
                content_available = contentAvailable,
                mutable_content = mutableContent
            };
            var tt = msg.ToJson();
            return client.PostJsonBasicAuth(Url, tt, _key);
        }

        public string PushToUser(string to, string title, string body, bool contentAvailable, bool mutableContent)
        {
            var client = new WebRequest2();

            var msg = new OnePushUserModel
            {
                app_id = _appId,
                contents = new OnePushContent { en = body },
                headings = new OnePushContent { en = title },
                include_external_user_ids = new[] { to },
                content_available = contentAvailable,
                mutable_content = mutableContent
            };
            var tt = msg.ToJson();
            return client.PostJsonBasicAuth(Url, tt, _key);
        }
    }
}
