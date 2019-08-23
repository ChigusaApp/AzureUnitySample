using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Chigusa.AzureApiApp.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //// GET api/values/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // GET api/values/5
        public HttpResponseMessage Get(string id)
        {
            //  サーバー番号
            var serverId = int.Parse(REST.GetHeaderValue("x-chigusa-serverid", this.Request.Headers));
            //  userId の復号化
            var encryptedUserId = REST.GetHeaderValue("x-chigusa-userid", this.Request.Headers);
            //var userId = REST.Decrypt(encryptedUserId);
            //  AppVersion
            var appVersion = REST.GetHeaderValue("x-chigusa-appversion", this.Request.Headers);

            //  ハッシュで検査
            string addHeader = "x-chigusa-serverid:" + serverId;
            addHeader += "\n" + "x-chigusa-userid:" + encryptedUserId;
            addHeader += "\n" + "x-chigusa-appversion:" + appVersion;
            if (!REST.CheckHash("GET", id, this.Request, addHeader))
                throw new Exception("Arg Exception");

            return Request.CreateResponse(HttpStatusCode.OK, "value");
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
