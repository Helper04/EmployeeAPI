using EmployeeAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public JsonResult Get()
        {
            MongoClient dbclient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCon"));
            var dblist = dbclient.GetDatabase("TestDB").GetCollection<Employee>("Employee").AsQueryable();

            return new JsonResult(dblist);
        }

        [HttpPost]
        public JsonResult Post(Employee emp)
        {
            MongoClient dbclient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCon"));
            int lastDepId = dbclient.GetDatabase("TestDB").GetCollection<Employee>("Employee").AsQueryable().Count();
            emp.EmployeeId = lastDepId + 1;
            dbclient.GetDatabase("TestDB").GetCollection<Employee>("Employee").InsertOne(emp);

            return new JsonResult("Employee Record Added Succefully!");
        }

        [HttpPut]
        public JsonResult Put(Employee emp)
        {
            MongoClient dbclient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCon"));

            var filter = Builders<Employee>.Filter.Eq("EmployeetId", emp.EmployeeId);

            var update = Builders<Employee>.Update.Set("EmployeeName", emp.EmployeeName)
                                                   .Set("Department", emp.Department)
                                                   .Set("DateofJoining", emp.DateofJoining)
                                                   .Set("PhotoFileName", emp.PhotoFileName);


            dbclient.GetDatabase("TestDB").GetCollection<Employee>("Employee").UpdateMany(filter, update);

            return new JsonResult("Employee Record Updated Succefully!");
        }

        [HttpDelete]
        public JsonResult Delete(int id)
        {
            MongoClient dbclient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCon"));

            var filter = Builders<Employee>.Filter.Eq("EmployeeId", id);

            dbclient.GetDatabase("TestDB").GetCollection<Employee>("Employee").DeleteOne(filter);

            return new JsonResult("Employee Record Deleted Succefully!");
        }

        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;
                using (var stream = new FileStream(physicalPath,FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(filename);
            }
            catch (Exception)
            {
                return new JsonResult("anonymus.png");
            }
        }
    }
}
