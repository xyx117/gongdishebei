using GongDiJiXie.Data;
using GongDiJiXie.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace GongDiJiXie.Controllers
{
    public class GongDiXiangMuController : Controller
    {

        private readonly GongDiContext _context;

        public GongDiXiangMuController(GongDiContext context) 
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }



        /// <summary>
        ///   获取所有项目的信息
        /// </summary>        
        [HttpPost]
        public JsonResult Get_xiangmus()/*string searchquery*/ 
        {
            int page = (Request.Form["page"] != "") ? int.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != "") ? int.Parse(Request.Form["rows"]) : 10;

            var xm = from c in _context.GongDiXiangMus
                     orderby c.Id
                     select new
                     {
                         id = c.Id,
                         gongdimingcheng = c.GongDiMingCheng, 
                         kaishishijian = c.KaiShiShiJian,
                         weizhi = c.Weizhi,
                         end_or = c.End_Or,
                         beizhu = c.BeiZhu 
                     };
            try
            {
                var easyUIPages = new Dictionary<string, object>
                {
                    { "total", xm.Count() },
                    { "rows", xm.ToPagedList(page, rows) }
                };
                return Json(easyUIPages);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// 保存新增的项目信息
        /// </summary>
        [HttpPost]
        public JsonResult Save_xiangmu([Bind("GongDiMingCheng,Weizhi,KaiShiShiJian,BeiZhu")] GongDiXiangMu gongdixiangmu) 
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope transaction = new())//原子操作，事物错误回滚
                {
                    try
                    {
                        gongdixiangmu.End_Or = false;
                        _context.GongDiXiangMus.Add(gongdixiangmu);
                        _context.SaveChanges();

                        transaction.Complete();
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, msg = ex.ToString() }, "text/html");
                    }
                    finally
                    {
                        transaction.Dispose();
                    }
                }
                return Json(new { success = true, msg = "添加项目信息成功" });/*, "text/html"*/
            }
            return Json(new { success = false, msg = "添加项目信息失败，请再试一试！" });
        }


        /// <summary>
        /// 编辑项目信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Update_xiangmu(int id)  //string bmname 好像多余
        {
            var xiangmu = _context.GongDiXiangMus.Where(c => c.Id == id).FirstOrDefault();

            xiangmu.GongDiMingCheng = Request.Form["gongdimingcheng"];
            xiangmu.Weizhi = Request.Form["weizhi"];
            xiangmu.End_Or = Convert.ToBoolean(Request.Form["end_or"]);
            xiangmu.KaiShiShiJian = Convert.ToDateTime( Request.Form["kaishishijian"]);            
            xiangmu.BeiZhu = Request.Form["beizhu"];

            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.Entry(xiangmu).State = EntityState.Modified;
                    _context.SaveChanges();
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, msg = ex.ToString() });
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true, msg = "更改项目信息成功！" });
        }

        /// <summary>
        /// 删除项目信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="xmid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Del_xiangmu(int id)
        {
            GongDiXiangMu xiangmu = _context.GongDiXiangMus.Find(id);

            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.GongDiXiangMus.Remove(xiangmu);
                    _context.SaveChanges();

                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, msg = ex.ToString() });
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return Json(new { success = true });
        }
    }
}
