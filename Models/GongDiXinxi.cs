using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GongDiJiXie.Models
{
    //工地信息,地块信息
    public class GongDiXinxi
    {
        [Key]
        public int Id { get; set; }

        //工地类型，分为  地块、楼。这里也可以用枚举方式
        public string GongDiLeiXin { get; set; }

        //工地距离
        public decimal Juli { get; set; }        

        //这里关联工地项目中的 项目名
        public string XiangMuMingCheng { get; set; }

    }
}
