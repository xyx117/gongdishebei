using GongDiJiXie.Data;
using GongDiJiXie.Models;
using GongDiJiXie.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GongDiJiXie.Controllers
{
    public class TongjiController : Controller
    {
        private readonly GongDiContext _context;

        public TongjiController(GongDiContext context) 
        {
            _context = context;
        }

        public IActionResult Index(string xm)
        {
            ViewBag.xm = xm;
            return View();
        }

        public IActionResult Test()
        {            
            return View();
        }

        //统计页面柱形图 返回的数据
        public JsonResult GetDataList(string xm)
        {
            //只要是在 渣土车记录表  有记录的车主都有钱            
            var zhatucheLst = new List<string>();
            var zhatucheQry = from c in _context.ZhaTuChes_Jilus
                              where c.XiangMuMingCheng == xm         //筛选条件是 这个项目里 的渣土车
                              orderby c.Id
                              select c.CheZhu;
            zhatucheLst.AddRange(zhatucheQry.Distinct());

            //找挖掘机工作时的车主
            var wajuejiLst = new List<string>();
            var wajuejiQry = from c in _context.WaJueJis_JiLus
                             where c.XiangMuMingCheng == xm         //筛选条件是 这个项目里 的渣土车
                             orderby c.Id
                             select c.Chezhu;
            wajuejiLst.AddRange(wajuejiQry.Distinct());

            var chejin_list = new List<decimal>();  //渣土车  车主的 金额合计
            var waji_jinelist = new List<decimal>();//挖掘机  车主的 金额合计
            var chejin_feiyong_list = new List<decimal>(); //渣土车  费用 合计
            var wajijin_feiyong_list = new List<decimal>();  //挖掘机  费用 合计
            var chejin_zhifu_list = new List<decimal>();  //渣土车 支付金额 合计
            var wajijin_zhifu_list = new List<decimal>();  //挖掘机 支付金额 合计

            //计算车主的 渣土车合金
            foreach (var item in zhatucheLst)
            {                
                var chatuche_jilu = _context.ZhaTuChes_Jilus.Where(s => s.XiangMuMingCheng == xm && s.CheZhu == item);
                var chatuche_feiyong = _context.FeiYongJiLus.Where(t => t.XiangMuMingCheng == xm && t.Leixing == "渣土车" && t.Chezhu == item);
                var chatuche_zhifu = _context.ZhifuXins.Where(z => z.XiangMuMingCheng == xm && z.Chezhu == item);
                //支付信息没有确定是否要指明 支付车主的 对象，就是说 一笔支付款，可能是给 单独渣土车，也可能是渣土车和挖掘机一起算
                //var chatuche_zhifu = _context.ZhifuXins.Where(k=>k.XiangMuMingCheng==xm&&k)    
                decimal jin_che = 0;
                decimal jin_feiyong_zhatuche = 0;
                decimal jin_zhifu_zhatuche = 0;
                foreach (var item_che in chatuche_jilu)
                {
                    var danjia = item_che.Price;
                    var checi = item_che.CheCi;
                    jin_che += danjia * checi;
                }
                chejin_list.Add(jin_che);

                //计算 渣土车 车主的 费用录入  合计
                foreach (var item_che_feiyong in chatuche_feiyong)    
                {
                    jin_feiyong_zhatuche += item_che_feiyong.Total;
                }
                chejin_feiyong_list.Add(jin_feiyong_zhatuche);

                foreach (var item_zhifu in chatuche_zhifu)
                {
                    jin_zhifu_zhatuche += item_zhifu.Zhifujine;
                }
                chejin_zhifu_list.Add(jin_zhifu_zhatuche);
            }

            //计算车主的 挖掘机合金
            foreach (var item in wajuejiLst)
            {
                var wajueji_jilu = _context.WaJueJis_JiLus.Where(d => d.XiangMuMingCheng == xm && d.Chezhu == item);
                var wajueji_feiyong = _context.FeiYongJiLus.Where(y => y.XiangMuMingCheng == xm && y.Leixing == "挖掘机" && y.Chezhu == item);
                decimal jin_waji = 0;
                decimal jin_waji_feiyong = 0;
                foreach (var item_che in wajueji_jilu)
                {
                    var danjia = item_che.Price;
                    var gongzuoliang = item_che.Gongzuoliang;
                    jin_waji += danjia * gongzuoliang;
                }
                waji_jinelist.Add(jin_waji);
                foreach (var item_waji_feiyong in wajueji_feiyong)
                {
                    jin_waji_feiyong += item_waji_feiyong.Total;
                }
                wajijin_feiyong_list.Add(jin_waji_feiyong);
            }
            var Obj = new
            {
                chezhu = zhatucheLst,
                che_jin = chejin_list,
                che_jin_feiyong = chejin_feiyong_list,
                wajichezhu = wajuejiLst,
                waji_jine = waji_jinelist,
                waji_jine_feiyong = wajijin_feiyong_list,
                che_jin_zhifu = chejin_zhifu_list
            };

            return Json(Obj);
        }
    }
}
