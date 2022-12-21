using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GongDiJiXie.Models
{
    public class FeiYongJiLu
    {
        [Key]
        public int Id { get; set; }

        //这里指的是 挖掘机或者渣土车 产生的费用
        public string Leixing { get; set; }

        //这里是为了在后期做统计添加的
        public string Chezhu { get; set; }

        //费用对象   这里指的是  车牌或者机牌
        public string Feiyongduixiang { get; set; }

        //费用日期  
        public DateTime Time { get; set; }

        //费用金额
        public decimal Total { get; set; } 

        //费用类型   这里指的是  工资、维修、保险、加油等
        public string Feiyongleixing { get; set; }

        //这里关联工地项目中的 项目名
        public string XiangMuMingCheng { get; set; }
    }
}
