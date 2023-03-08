using EmployeeAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        public JsonResult Get()
        {
            MongoClient dbclient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCon"));
            var dblist = dbclient.GetDatabase("TestDB").GetCollection<Department>("Department").AsQueryable();

            return new JsonResult(dblist);
        }

        [HttpPost]
        public JsonResult Post(Department dep)
        {
            MongoClient dbclient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCon"));
            int lastDepId = dbclient.GetDatabase("TestDB").GetCollection<Department>("Department").AsQueryable().Count();
            dep.DepartmentId = lastDepId + 1;
            dbclient.GetDatabase("TestDB").GetCollection<Department>("Department").InsertOne(dep);

            return new JsonResult("Deparment Record Added Succefully!");
        }

        [HttpPut]
        public JsonResult Put(Department dep)
        {
            MongoClient dbclient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCon"));

            var filter = Builders<Department>.Filter.Eq("DepartmentId", dep.DepartmentId);

            var update = Builders<Department>.Update.Set("DepartmentName", dep.DepartmentName);


            dbclient.GetDatabase("TestDB").GetCollection<Department>("Department").UpdateOne(filter, update);

            return new JsonResult("Deparment Record Updated Succefully!");
        }

        [HttpDelete]
        public JsonResult Delete(int id)
        {
            MongoClient dbclient = new MongoClient(_configuration.GetConnectionString("EmployeeAppCon"));

            var filter = Builders<Department>.Filter.Eq("DepartmentId", id);

            dbclient.GetDatabase("TestDB").GetCollection<Department>("Department").DeleteOne(filter);

            return new JsonResult("Deparment Record Deleted Succefully!");
        }
    }
}
