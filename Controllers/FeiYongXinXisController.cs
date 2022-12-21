using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GongDiJiXie.Data;
using GongDiJiXie.Models;
using System.Transactions;
using PagedList;

namespace GongDiJiXie.Controllers
{
    public class FeiYongXinXisController : Controller
    {
        private readonly GongDiContext _context;

        public FeiYongXinXisController(GongDiContext context)
        {
            _context = context;
        }

        
        public IActionResult Index()
        {
            
            return View();/*await _context.ZhaTuChes.ToListAsync()*/
        }


        /// <summary>
        ///   获取所有费用分类的信息
        /// </summary>        
        [HttpPost]
        public JsonResult Get_Feiyong()/*string searchquery*/
        {
            int page = (Request.Form["page"] != "") ? int.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != "") ? int.Parse(Request.Form["rows"]) : 10;

            var xm = from c in _context.FeiYongXinXis
                     orderby c.Id
                     select new
                     {
                         id = c.Id,
                         feiyongleixing = c.FeiYongLeiXing                         
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
        /// 保存新增的渣土车信息
        /// </summary>
        [HttpPost]
        public JsonResult Save_Feiyong([Bind("FeiYongLeiXing")] FeiYongXinXi feiyong)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope transaction = new())//原子操作，事物错误回滚
                {
                    try
                    {
                        
                        _context.FeiYongXinXis.Add(feiyong);
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
                return Json(new { success = true, msg = "添加费用类型信息成功" });/*, "text/html"*/
            }
            return Json(new { success = false, msg = "添加费用类型信息失败，请再试一试！" });
        }


        /// <summary>
        /// 编辑挖掘机信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Update_feiyong(int id)  //string bmname 好像多余
        {
            var feiyong = _context.FeiYongXinXis.Where(c => c.Id == id).FirstOrDefault();

            feiyong.FeiYongLeiXing = Request.Form["feiyongleixing"];

            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.Entry(feiyong).State = EntityState.Modified;
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
            return Json(new { success = true, msg = "更改费用信息成功！" });
        }

        /// <summary>
        /// 删除挖掘机信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="xmid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Del_feiyong(int id)
        {
            FeiYongXinXi feiyong = _context.FeiYongXinXis.Find(id);

            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.FeiYongXinXis.Remove(feiyong);
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
