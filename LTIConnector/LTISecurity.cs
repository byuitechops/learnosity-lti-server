using LtiLibrary.Extensions;
using LtiLibrary.Lti1;
using LtiLibrary.Core.OAuth;
using System;
using Byui.Desire2Learn.LTIConnector.Helpers;
using System.Web;
using Byui.LTIConnector.Models;

namespace Byui.LTIConnector
{
    public class LTISecurity
    {
        public static bool timeOut(long Timestamp)
        {
            var timeout = TimeSpan.FromMinutes(5);
            var oauthTimestampAbsolute = OAuthConstants.Epoch.AddSeconds(Timestamp);
            if (DateTime.UtcNow - oauthTimestampAbsolute > timeout)
            {
                return false;
            }
            return true;
        }
        public static bool consumeKey(string ConsumerKey)
        {
            if (ConsumerKey != LTIBusinessHelper.popcornKernel)
            {
                return false;
            }
            return true;
        }
        public static bool oAuthSig(string Signature, HttpRequestBase Request)
        {
            var oauthSignature = Request.GenerateOAuthSignature(LTIBusinessHelper.popcornSalt);

            if (!oauthSignature.Equals(Signature))
            {
                return false;
            }
            return true;
        }
        public static ToolModel ltiList(HttpRequestBase Request)
        {
            var ltiRequest = new LtiRequest(null);

            ltiRequest.ParseRequest(Request);

            var model = new ToolModel
            {
                ConsumerSecret = "",
                LtiRequest = ltiRequest
            };
            return model;
        }
    }
}