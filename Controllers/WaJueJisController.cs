using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GongDiJiXie.Data;
using GongDiJiXie.Models;
using PagedList;
using System.Transactions;

namespace GongDiJiXie.Controllers
{
    public class WaJueJisController : Controller
    {
        private readonly GongDiContext _context;

        public WaJueJisController(GongDiContext context)
        {
            _context = context;
        }

        // GET: WaJueJis
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.WaJueJis.ToListAsync());
        //}


        public IActionResult Index(string xm)
        {
            ViewBag.xm = xm;
            return View();/*await _context.ZhaTuChes.ToListAsync()*/
        }


        /// <summary>
        ///   获取所有渣土车的信息
        /// </summary>        
        [HttpPost]
        public JsonResult Get_wajuejis(string xm)/*string searchquery*/
        {
            int page = (Request.Form["page"] != "") ? int.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != "") ? int.Parse(Request.Form["rows"]) : 10;

            var wajueji = from c in _context.WaJueJis
                          where c.XiangMuMingCheng == xm
                         orderby c.Id
                         select new
                         {
                             id = c.Id,
                             xinhao = c.XinHao,
                             chezhu = c.CheZhu,                                                          
                             lianxifangshi = c.LianXiFangShi,
                             jipai = c.JiPai,
                             chanquan = c.ChanQuan
                         };
            try
            {
                var easyUIPages = new Dictionary<string, object>
                {
                    { "total", wajueji.Count() },
                    { "rows", wajueji.ToPagedList(page, rows) }
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
        public JsonResult Save_wajueji([Bind("JiPai,XinHao,CheZhu,DunWei,ChanQuan,LianXiFangShi")] WaJueJi wajueji,string xm)
        {
            if (ModelState.IsValid)
            { 
                using (TransactionScope transaction = new())//原子操作，事物错误回滚
                {
                    try
                    {
                        wajueji.XiangMuMingCheng = xm;
                        _context.WaJueJis.Add(wajueji);
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
                return Json(new { success = true, msg = "添加挖掘机信息成功" });/*, "text/html"*/
            }
            return Json(new { success = false, msg = "添加挖掘机信息失败，请再试一试！" });
        }


        /// <summary>
        /// 编辑挖掘机信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Update_wajueji(int id)  //string bmname 好像多余
        {
            var wajueji = _context.WaJueJis.Where(c => c.Id == id).FirstOrDefault();

            wajueji.JiPai = Request.Form["jipai"];

            wajueji.XinHao = Request.Form["xinhao"];

            wajueji.CheZhu = Request.Form["chezhu"];

            wajueji.ChanQuan = Request.Form["chanquan"];

            wajueji.LianXiFangShi = Request.Form["lianxifangshi"];                        

            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.Entry(wajueji).State = EntityState.Modified;
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
            return Json(new { success = true, msg = "更改渣土车信息成功！" });
        }

        /// <summary>
        /// 删除挖掘机信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="xmid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Del_wajueji(int id)
        {
            WaJueJi wajueji = _context.WaJueJis.Find(id);

            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.WaJueJis.Remove(wajueji);
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



        // GET: WaJueJis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waJueJi = await _context.WaJueJis
                .FirstOrDefaultAsync(m => m.Id == id);
            if (waJueJi == null)
            {
                return NotFound();
            }

            return View(waJueJi);
        }

        // GET: WaJueJis/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WaJueJis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,XinHao,CheZhu,DunWei,SuoShuFangShi,LianXiFangShi")] WaJueJi waJueJi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(waJueJi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(waJueJi);
        }

        // GET: WaJueJis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waJueJi = await _context.WaJueJis.FindAsync(id);
            if (waJueJi == null)
            {
                return NotFound();
            }
            return View(waJueJi);
        }

        // POST: WaJueJis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,XinHao,CheZhu,DunWei,SuoShuFangShi,LianXiFangShi")] WaJueJi waJueJi)
        {
            if (id != waJueJi.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(waJueJi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaJueJiExists(waJueJi.Id))
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
            return View(waJueJi);
        }

        // GET: WaJueJis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waJueJi = await _context.WaJueJis
                .FirstOrDefaultAsync(m => m.Id == id);
            if (waJueJi == null)
            {
                return NotFound();
            }

            return View(waJueJi);
        }

        // POST: WaJueJis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var waJueJi = await _context.WaJueJis.FindAsync(id);
            _context.WaJueJis.Remove(waJueJi);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WaJueJiExists(int id)
        {
            return _context.WaJueJis.Any(e => e.Id == id);
        }
    }
}
