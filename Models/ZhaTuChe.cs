using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GongDiJiXie.Models
{
    //渣土车信息
    public class ZhaTuChe
    {
        [Key]
        public int Id { get; set; }

        //车牌
        public string ChePai { get; set; }

        //车主
        public string CheZhu { get; set; }

        //车型
        public string CheXing { get; set; }

        //产权方式
        public string ChanQuan { get; set; }

        //联系方式
        public string LianXiFangShi { get; set; }

        //这里关联工地项目中的 项目名
        public string XiangMuMingCheng { get; set; }
    }
}
