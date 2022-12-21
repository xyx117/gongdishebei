using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GongDiJiXie.Models
{
    //费用信息
    public class FeiYongXinXi
    {
        [Key]
        public int Id { get; set; }

        //费用类型，分为  加油、维修。这里也可以用枚举方式
        public string FeiYongLeiXing { get; set; } 
        
    }
}
