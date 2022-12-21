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
    public class GongDiXinxisController : Controller
    {
        private readonly GongDiContext _context;

        public GongDiXinxisController(GongDiContext context)
        {
            _context = context;
        }

        // GET: GongDiXinxis        
        public IActionResult Index(string xm)
        {
            ViewBag.xm = xm;
            return View();
        }

        /// <summary>
        ///   获取所有渣土车的信息
        /// </summary>        
        [HttpPost]
        public JsonResult Get_gongdis(string xm)/*string searchquery*/
        {
            int page = (Request.Form["page"] != "") ? int.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != "") ? int.Parse(Request.Form["rows"]) : 10;

            var gongdixinxi = from c in _context.GongDiXinxis
                              where c.XiangMuMingCheng == xm
                             orderby c.Id
                             select new 
                             {
                                 id = c.Id,
                                 gongdileixin = c.GongDiLeiXin,
                                 juli = c.Juli
                             };
            try
            {
                var easyUIPages = new Dictionary<string, object>
                {
                    { "total", gongdixinxi.Count() },
                    { "rows", gongdixinxi.ToPagedList(page, rows) }
                };
                return Json(easyUIPages);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// 保存新增的工地信息
        /// </summary>
        [HttpPost]
        public JsonResult Save_gongdi([Bind("GongDiLeiXin,Juli")] GongDiXinxi gongdi,string xm)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope transaction = new())//原子操作，事物错误回滚
                {
                    try
                    {
                        gongdi.XiangMuMingCheng = xm;
                        _context.GongDiXinxis.Add(gongdi);
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
                return Json(new { success = true, msg = "添加工地信息成功" });/*, "text/html"*/
            }
            return Json(new { success = false, msg = "添加工地信息失败，请再试一试！" });
        }


        /// <summary>
        /// 编辑工地信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Update_gongdi(int id)  //string bmname 好像多余
        {
            var gongdi = _context.GongDiXinxis.Where(c => c.Id == id).FirstOrDefault();

            gongdi.GongDiLeiXin = Request.Form["gongdileixin"];
            gongdi.Juli = Convert.ToDecimal(Request.Form["juli"]);

            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.Entry(gongdi).State = EntityState.Modified;
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
            return Json(new { success = true, msg = "更改工地信息成功！" });
        }

        /// <summary>
        /// 删除工地信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="xmid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Del_gongdi(int id)
        { 
            GongDiXinxi gongdi = _context.GongDiXinxis.Find(id);

            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.GongDiXinxis.Remove(gongdi);
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
















        // GET: GongDiXinxis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gongDiXinxi = await _context.GongDiXinxis
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gongDiXinxi == null)
            {
                return NotFound();
            }

            return View(gongDiXinxi);
        }

        // GET: GongDiXinxis/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GongDiXinxis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GongDiLeiXin")] GongDiXinxi gongDiXinxi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gongDiXinxi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gongDiXinxi);
        }

        // GET: GongDiXinxis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gongDiXinxi = await _context.GongDiXinxis.FindAsync(id);
            if (gongDiXinxi == null)
            {
                return NotFound();
            }
            return View(gongDiXinxi);
        }

        // POST: GongDiXinxis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GongDiLeiXin")] GongDiXinxi gongDiXinxi)
        {
            if (id != gongDiXinxi.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gongDiXinxi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GongDiXinxiExists(gongDiXinxi.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gongDiXinxi);
        }

        // GET: GongDiXinxis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gongDiXinxi = await _context.GongDiXinxis
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gongDiXinxi == null)
            {
                return NotFound();
            }

            return View(gongDiXinxi);
        }

        // POST: GongDiXinxis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gongDiXinxi = await _context.GongDiXinxis.FindAsync(id);
            _context.GongDiXinxis.Remove(gongDiXinxi);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GongDiXinxiExists(int id)
        {
            return _context.GongDiXinxis.Any(e => e.Id == id);
        }
    }
}
