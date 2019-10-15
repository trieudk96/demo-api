using DemoApi.Database;
using DemoApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Linq;

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

            var data =
                _context.Users.Where(s => string.IsNullOrEmpty(searchString) || (
                                              !string.IsNullOrEmpty(s.UserName) && s.UserName.Contains(searchString) ||
                                              !string.IsNullOrEmpty(s.Name) && s.Name.Contains(searchString) ||
                                              s.Age.ToString().Contains(searchString) ||
                                              !string.IsNullOrEmpty(s.Gender) && s.Gender.Contains(searchString)));
            var total = data.Count();
            var startIndex = pageSize * (pageIndex - 1);
            var offset = startIndex + pageSize < total ? pageSize : total - startIndex;
            res.payload = data.Skip(startIndex).Take(offset).ToList(); ;

            res.total_count = total;
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
            var item = _context.Users.FirstOrDefault(s => s.Id.Equals(id));
            if (item == null) return NotFound();
            _context.Users.Remove(item);
            //foreach (var itemId in id)
            //{
            //    var item = _context.Users.FirstOrDefault(s => s.Id.Equals(itemId));
            //    if (item == null) return NotFound();
            //    _context.Users.Remove(item);
            //}
            return _context.SaveChanges() > 0;
        }
        //[HttpPost]
        //public ActionResult<User> Add([FromBody]User user)
        //{
        //    //var userAdding = _context.Users.FirstOrDefault(x => x.UserName.Equals(user.UserName));
        //    //if (userAdding != null) return null;
        //    user.Id = _context.Users.OrderBy(x => x.Id).Last().Id + 1;
        //    _context.Users.Add(user);
        //    _context.SaveChanges();
        //    return user;
        //}

        [HttpPost]
        public ActionResult<User> Add([FromQuery]string name,[FromQuery]string userName,[FromQuery]int age,[FromQuery]string gender)
        {
            var user = new User
            {
                Name = name,
                UserName = userName,
                Age = age,
                Gender = gender,
                Id = _context.Users.OrderBy(x => x.Id).Last().Id + 1
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }


        //[HttpPut]
        //public ActionResult<User> Update([FromBody]User user)
        //{
        //    var userUpdating = _context.Users.FirstOrDefault(x => x.Id.Equals(user.Id));
        //    if (userUpdating == null) return null;
        //    userUpdating.Age = user.Age;
        //    userUpdating.Name = user.Name;
        //    userUpdating.UserName = user.UserName;
        //    userUpdating.Gender = user.Gender;
        //    _context.SaveChanges();
        //    return userUpdating;
        //}

        [HttpPut]
        public ActionResult<User> Update([FromQuery]int id, [FromQuery]string name, [FromQuery]string userName, [FromQuery]int age, [FromQuery]string gender)
        {
            var userUpdating = _context.Users.FirstOrDefault(x => x.Id.Equals(id));
            if (userUpdating == null) return null;
            userUpdating.Age = age;
            userUpdating.Name = name;
            userUpdating.UserName = userName;
            userUpdating.Gender = gender;
            _context.SaveChanges();
            return userUpdating;
        }
    }
}
