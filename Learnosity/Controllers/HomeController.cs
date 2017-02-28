using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LearnositySDK.Request;
using LearnositySDK.Utils;
using NLog;
using Byui.LTIConnector;
using Byui.LTIConnector.Models;
using D2L.Extensibility.AuthSdk;
using LearnositySDK.Examples;
using LearnositySDK;

namespace Learnosity.Controllers
{
    public class HomeController : Controller
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            ////Parse LTI Information
            ToolModel tm = LTISecurity.ltiList(Request);

            ////Check LTI TimeStamp
            bool timeOut = LTISecurity.timeOut(tm.LtiRequest.Timestamp);
            ////Check Key
            bool consumerKey = LTISecurity.consumeKey(tm.LtiRequest.ConsumerKey);
            ////Check OAuth
            bool oAuthSig = LTISecurity.oAuthSig(tm.LtiRequest.Signature, Request);

            if (timeOut == false || consumerKey == false || oAuthSig == false)
            {
                return RedirectToAction("NoAuth", "Error");
            }

            

            string uuid = Uuid.generate();
            string courseId = tm.LtiRequest.ContextId;
            string questionsApiActivityJson = Assess.questionsApiActivity(uuid, courseId);
            JsonObject questionsApiActivity = JsonObjectFactory.fromString(questionsApiActivityJson);

            string service = "assess";

            JsonObject security = new JsonObject();
            security.set("consumer_key", Credentials.ConsumerKey);
            security.set("user_id", tm.LtiRequest.Parameters["ext_d2l_orgdefinedid"]);
            security.set("domain", Credentials.Domain);

            string secret = Credentials.ConsumerSecret;

            JsonObject request = new JsonObject();
            request.set("name", "Demo Activity (8 questions)");
            request.set("state", "initial");
            request.set("items", Assess.items(uuid));
            request.set("questionsApiActivity", questionsApiActivity);

            Init init = new Init(service, security, secret, request);
            ViewBag.Tim = init.generate();

            //ViewBag.SearchMe = "https://byui.brightspace.com/d2l/lp/manageUsers/main.d2l?ou=6606";

            return View();

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}