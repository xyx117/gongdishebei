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
using System.Text;

namespace GongDiJiXie.Controllers
{
    public class ZhaTuChesController : Controller
    {
        private readonly GongDiContext _context;

        public ZhaTuChesController(GongDiContext context)
        {
            _context = context;
        }

        // GET: ZhaTuChes
        public IActionResult Index(string xm)
        {
            ViewBag.xm = xm;
            return View()  ;/*await _context.ZhaTuChes.ToListAsync()*/
        }


        /// <summary>
        ///   获取所有渣土车的信息
        /// </summary>        
        [HttpPost] 
        public JsonResult Get_zhatuches(string searchquery,string xm)/*string searchquerystring xm*/
        {
            int page = (Request.Form["page"] != "") ? int.Parse(Request.Form["page"]) : 1;
            int rows = (Request.Form["rows"] != "") ? int.Parse(Request.Form["rows"]) : 10;

            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //Console.WriteLine(Encoding.GetEncoding("GB2312"));

            var xm_str = xm;

            var zhatuche = from c in _context.ZhaTuChes            
                     where c.XiangMuMingCheng == xm
                     orderby c.Id
                     select new
                     {
                         id = c.Id,
                         chepai = c.ChePai,
                         chezhu = c.CheZhu,
                         lianxifangshi = c.LianXiFangShi,
                         chanquan = c.ChanQuan
                     };            
            try
            {
                var easyUIPages = new Dictionary<string, object>
                {
                    { "total", zhatuche.Count() },
                    { "rows", zhatuche.ToPagedList(page, rows) }
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
        public JsonResult Save_zhatuche([Bind("ChePai,CheZhu,CheXing,ChanQuan,LianXiFangShi")] ZhaTuChe zhatuche,string xm)
        {            
            if (ModelState.IsValid)
            {
                using (TransactionScope transaction = new())//原子操作，事物错误回滚
                {
                    try
                    {
                        zhatuche.XiangMuMingCheng = xm;
                        _context.ZhaTuChes.Add(zhatuche);
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
                return Json(new { success = true, msg = "添加渣土车信息成功" });/*, "text/html"*/
            }
            return Json(new { success = false, msg = "添加渣土车信息失败，请再试一试！" });
        }


        /// <summary>
        /// 编辑渣土车信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Update_zhatuche(int id)  //string bmname 好像多余
        {
            var zhatuche = _context.ZhaTuChes.Where(c => c.Id == id).FirstOrDefault();

            zhatuche.ChePai = Request.Form["chepai"];

            zhatuche.CheZhu = Request.Form["chezhu"];

            zhatuche.LianXiFangShi = Request.Form["lianxifangshi"];

            zhatuche.ChanQuan = Request.Form["chanquan"];

            zhatuche.CheXing = Request.Form["chexing"];

            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.Entry(zhatuche).State = EntityState.Modified;
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
        /// 删除渣土车信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="xmid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Del_zhatuche(int id)
        {
            ZhaTuChe zhatuche = _context.ZhaTuChes.Find(id);

            using (TransactionScope transaction = new())//原子操作，事物错误回滚
            {
                try
                {
                    _context.ZhaTuChes.Remove(zhatuche);
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















        // GET: ZhaTuChes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zhaTuChe = await _context.ZhaTuChes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zhaTuChe == null)
            {
                return NotFound();
            }

            return View(zhaTuChe);
        }

        // GET: ZhaTuChes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ZhaTuChes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChePai,CheZhu,LianXiFangShi")] ZhaTuChe zhaTuChe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zhaTuChe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(zhaTuChe);
        }

        // GET: ZhaTuChes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zhaTuChe = await _context.ZhaTuChes.FindAsync(id);
            if (zhaTuChe == null)
            {
                return NotFound();
            }
            return View(zhaTuChe);
        }

        // POST: ZhaTuChes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChePai,CheZhu,LianXiFangShi")] ZhaTuChe zhaTuChe)
        {
            if (id != zhaTuChe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zhaTuChe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZhaTuCheExists(zhaTuChe.Id))
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
            return View(zhaTuChe);
        }

        // GET: ZhaTuChes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zhaTuChe = await _context.ZhaTuChes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zhaTuChe == null)
            {
                return NotFound();
            }

            return View(zhaTuChe);
        }

        // POST: ZhaTuChes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zhaTuChe = await _context.ZhaTuChes.FindAsync(id);
            _context.ZhaTuChes.Remove(zhaTuChe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ZhaTuCheExists(int id)
        {
            return _context.ZhaTuChes.Any(e => e.Id == id);
        }
    }
}
