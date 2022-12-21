using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GongDiJiXie.ViewModels
{
    public class Zhifuchaxun
    {
        public string Chezhu { get; set; }

        //渣土车和挖掘机合计给的金额
        public decimal Jine { get; set; }

        //余额
        public decimal Yue { get; set; }

        //支付记录中，已经支付过的金额
        public decimal Yifu { get; set; }

        //费用上的支付，例如 加油、维修、保险、人工等
        public decimal Feiyong { get; set; }

        //超额金额,就是  费用支付的 金额 和 已支付金额   的合计金额 超过了 ，渣土车+挖掘机  的合计金额
        public decimal Chaoe { get; set; }
    }
}
