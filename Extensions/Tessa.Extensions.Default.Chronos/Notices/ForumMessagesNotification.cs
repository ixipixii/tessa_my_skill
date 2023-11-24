using System;
using System.Collections.Generic;
using System.Linq;

namespace Tessa.Extensions.Default.Chronos.Notices
{
    public class ForumMessagesNotification
    {
        #region Constructors

        public ForumMessagesNotification(
            Guid userID, 
            string normalizedWebAddress)
        {
            this.UserID = userID;
            this.WebAddress = normalizedWebAddress;

            this.TopicsNotifications = new List<TopicNotificationInfo>();
        }

        #endregion

        #region Properties

        public Guid UserID { get; private set; }
        public string WebAddress { get; private set; }

        public List<TopicNotificationInfo> TopicsNotifications { get; set; }
        

        public Dictionary<string, object> GetInfo()
        {
            return new Dictionary<string, object>(StringComparer.Ordinal)
            {
                [nameof(TopicsNotifications)] = this.TopicsNotifications.Select(x => (object)x.GetStorage()).ToList(),
                ["HasWeb"] = !string.IsNullOrWhiteSpace(WebAddress),
            };
        }

        #endregion
    }
}