using GongDiJiXie.Data;
using GongDiJiXie.Models;
using GongDiJiXie.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GongDiJiXie.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ApplicationDbContext _context; 
        public RoleController(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            this.roleManager = roleManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetRole() 
        {
            int page = (Request.Form["page"] != "") ? int.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != "") ? int.Parse(Request.Form["rows"]) : 10;            
            var role = from c in _context.Roles
                     orderby c.Id
                     select new { id = c.Id, role = c.Name, description = c.Description};
            try
            {
                // 返回到前台的值必须按照如下的格式包括 total and rows 
                var easyUIPages = new Dictionary<string, object>
                {
                    { "total", role.Count() },
                    { "rows", role.ToPagedList(page, rows) }
                };
                return Json(easyUIPages);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //添加角色
        [HttpPost]
        public async Task<IActionResult> Addrole(AddRoleViewModel addRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new ApplicationRole
                {
                    Name = addRoleViewModel.Name,
                    Description = addRoleViewModel.Description                    
                };
                var result = await roleManager.CreateAsync(role);  //添加角色                
                if (result.Succeeded )
                {
                    //return RedirectToAction("Index", "Role");
                    return Json(new { success = true, msg = "添加角色成功" });
                }
            }
            return View("Index");
        }

    }
}
