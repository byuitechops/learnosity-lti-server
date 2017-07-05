using System;
using System.Text;
using LearnositySDK.Request;
using LearnositySDK.Utils;

namespace LearnositySDK.Examples
{
    public class Items
    {
        public static string Simple(string activityId, string userId, string outcomeUrl, string sourcedId)
        {
            // prepare all the params
            string service = "items";

            JsonObject security = new JsonObject();
            security.set("consumer_key", Credentials.ConsumerKey);
            security.set("domain", Credentials.Domain);
            security.set("user_id", userId);

            string secret = Credentials.ConsumerSecret;

            JsonObject config = new JsonObject();
            config.set("title", "Test");

            JsonObject request = new JsonObject();
            request.set("activity_template_id", activityId);
            request.set("activity_id", activityId);
            request.set("name", "assessment of " + activityId);
            request.set("session_id", Uuid.generate());
            request.set("user_id", userId);

            JsonObject lis_data = new JsonObject();
            lis_data.set("lis_outcome_service_url", outcomeUrl);
            lis_data.set("lis_result_sourcedid", sourcedId);
            lis_data.set("oauth_consumer_key", "SuperBadKey");

            request.set("lis_data", lis_data);

            // Instantiate Init class
            Init init = new Init(service, security, secret, request);

            // Call the generate() method to retrieve a JavaScript object
            return init.generate();
        }
    }
}