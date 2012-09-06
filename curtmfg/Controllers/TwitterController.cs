using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;

namespace curtmfg.Controllers
{
    public class TwitterController : AsyncController
    {
        //
        // GET: /Index/

        public string GetTweets()
        {
            return Twitter.getTweets();
        }

    }

}
