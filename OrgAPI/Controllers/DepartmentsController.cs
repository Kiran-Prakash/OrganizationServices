using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrgDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DepartmentsController : Controller
    {
        OrganizationDbContext dbContext;
        UserManager<IdentityUser> userManager;
        public DepartmentsController(OrganizationDbContext dbContext, UserManager<IdentityUser> _userManager)
        {
            this.dbContext = dbContext;
            userManager = _userManager;
        }

        //Approach 1
        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    var Depts = await dbContext.Departments.Include(x=>x.Employees)
        //        .Select(x=> new Department
        //        {
        //            Did = x.Did,
        //            DName = x.DName,
        //            Description = x.Description,
        //            Employees = x.Employees.Select(y=>new Employee
        //            {
        //                Eid=y.Eid,
        //                Name=y.Name,
        //                Gender=y.Gender,
        //                Did = y.Did
        //            })
        //        })
        //        .ToListAsync();
        //    if (Depts.Count != 0)
        //        return Ok(Depts);
        //    else
        //        return NotFound(); 
        //}

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var Depts = await dbContext.Departments.Include(x => x.Employees)
                        .ToListAsync();
                if (Depts.Count != 0)
                {
                    var jsonResult = JsonConvert.SerializeObject(Depts, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                    return Ok(jsonResult);
                }
                else
                    return NotFound();
            }
            catch (Exception E)
            {
                return StatusCode(500,E.Message);
            }
        }


        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Dept = await dbContext.Departments.Where(x=>x.Did==id).FirstOrDefaultAsync();
            if (Dept != null)
                return Ok(Dept);
            else
                return NotFound();
        }

        [HttpGet("getByName/{dname}")]
        public async Task<IActionResult> GetByName(string dname)
        {
            var Dept = await dbContext.Departments.Where(x => x.DName == dname).FirstOrDefaultAsync();
            if (Dept != null)
                return Ok(Dept);
            else
                return NotFound();
        }

        [HttpGet("getByIdAndName")]
        public async Task<IActionResult> GetByName(int id,string dname)
        {
            var Dept = await dbContext.Departments.Where(x => x.Did == id && x.DName == dname).FirstOrDefaultAsync();
            if (Dept != null)
                return Ok(Dept);
            else
                return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var Dept =await dbContext.Departments.Where(x => x.Did == id).FirstOrDefaultAsync();
            if (Dept != null)
            {
                dbContext.Remove(Dept);
                await dbContext.SaveChangesAsync();
                return Ok("Deleted the department with ID " + id);
            }
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Post(Department D)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                D.Id = user.Id;
                dbContext.Add(D);
                await dbContext.SaveChangesAsync();
                return CreatedAtAction("Get", new { id = D.Did }, D);
            }
            else
                return BadRequest(ModelState);
            
        }

        [HttpPut]
        public async Task<IActionResult> Put(Department D)
        {
            int id = D.Did;
            var Dept = await dbContext.Departments.Where(x => x.Did == id).AsNoTracking().FirstOrDefaultAsync();
            if (Dept!=null)
            {
                if (ModelState.IsValid)
                {
                    dbContext.Update(D);
                    await dbContext.SaveChangesAsync();
                    return NoContent();
                }
                else
                    return BadRequest(ModelState);
            }
            else
            return NotFound();
        }
       
    }
}
