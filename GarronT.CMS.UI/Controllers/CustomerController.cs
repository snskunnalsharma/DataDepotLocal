using GarronT.CMS.UI.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GarronT.CMS.UI.Controllers
{
   
    public class CustomerController : ApiController
    {
        // GET api/customer
       // [Route("api/books")]

        [HttpGet]
        [ActionName("info")]
        public HttpResponseMessage Getcustomer(int id)
        {

            return Request.CreateResponse(HttpStatusCode.OK, 12);
        
        }
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/customer/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/customer
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/customer/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/customer/5
        //public void Delete(int id)
        //{
        //}
    }
}
