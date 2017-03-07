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
            ToolModel tm = LTISecurity.ltiList(base.Request);

            ////Check LTI TimeStamp
            bool timeOut = LTISecurity.timeOut(tm.LtiRequest.Timestamp);
            ////Check Key
            bool consumerKey = LTISecurity.consumeKey(tm.LtiRequest.ConsumerKey);
            ////Check OAuth
            bool oAuthSig = LTISecurity.oAuthSig(tm.LtiRequest.Signature, base.Request);

            if (timeOut == false || consumerKey == false || oAuthSig == false)
            {
                return RedirectToAction("NoAuth", "Error");
            }

            string query = base.Request.UrlReferrer.Query;
            Dictionary<string, string> queries = new Dictionary<string, string>();
            if(query.Length > 3)
            {
                query = query.Substring(1);
                foreach (string value in query.Split('&'))
                {
                    var values = value.Split('=');
                    queries.Add(values[0], values[1]);
                }
            }

            string uuid = Uuid.generate();
            string courseId = tm.LtiRequest.ContextId;

            string service = queries["service"];

            if (queries["service"] == "items")
            {
                ViewBag.Tim = Items.Simple(queries["activity"], tm.LtiRequest.Parameters["ext_d2l_orgdefinedid"]);
            }
            else if(queries["service"] == "assess")
            {
                string questionsApiActivityJson = Assess.questionsApiActivity(uuid, courseId);
                JsonObject questionsApiActivity = JsonObjectFactory.fromString(questionsApiActivityJson);

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
            }
            else if(queries["service"] == "author")
            {
                return RedirectToAction("QuestionList");
            }
            else
            {
                return RedirectToAction("NoAuth", "Error");
            }
                //ViewBag.SearchMe = "https://byui.brightspace.com/d2l/lp/manageUsers/main.d2l?ou=6606";

                return View();

        }

        public ActionResult QuestionList()
        {
            ViewBag.Tim = Author.Simple("item_list");
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