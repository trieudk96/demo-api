using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using DemoApi.Database;
using DemoApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApi.Controllers
{
    [Route("api/[controller]/{id?}")]
    [ApiController]
    [EnableCors]
    public class UserController : ControllerBase
    {
        private readonly DemoDbContext _context;

        public UserController(DemoDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        //
        public ActionResult<dynamic> Get([FromQuery]int pageIndex = 1, [FromQuery]int pageSize = 10, [FromQuery]string searchString = null)
        {
            dynamic res = new ExpandoObject();
            var total = _context.Users.Count();
            res.total_count = total;

            var startIndex = pageSize * (pageIndex - 1);
            var segments = startIndex + pageSize < total ? pageSize : total - startIndex;
            res.payload =
                _context.Users.Where(s => string.IsNullOrEmpty(searchString) || (
                    !string.IsNullOrEmpty(s.UserName) && s.UserName.Contains(searchString) ||
                    !string.IsNullOrEmpty(s.Name) && s.Name.Contains(searchString) ||
                    s.Age.ToString().Contains(searchString) ||
                    !string.IsNullOrEmpty(s.Gender) && s.Gender.Contains(searchString)))
                    .Skip(startIndex).Take(segments).ToList();
            return res;
        }
        //[HttpGet]
        //public ActionResult<IEnumerable<User>> GetAll()
        //{
        //    return _context.Users.AsNoTracking().ToList();
        //}
        [HttpDelete]
        public ActionResult<bool> Delete(int id)
        {
            var item = _context.Users.FirstOrDefault(s => s.Id == id);
            if (item == null) return NotFound();
            _context.Users.Remove(item);
            return _context.SaveChanges() > 0;
        }


    }
}
