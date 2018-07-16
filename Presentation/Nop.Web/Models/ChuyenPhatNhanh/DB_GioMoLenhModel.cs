using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Models.ChuyenPhatNhanh
{
    public class DB_GioMoLenhModel
    {
        public DB_GioMoLenhModel()
        {
            ListThang = new List<SelectListItem>();
            ListNam = new List<SelectListItem>();
            //ListBenXe = new List<BenXe>();
        }

        public string Thang { get; set; }
        public string Nam { get; set; }
        public int ThangId { get; set; }
        public int NamId { get; set; }
        public bool isEnable { get; set; }
        public IList<SelectListItem> ListThang { get; set; }
        public IList<SelectListItem> ListNam { get; set; }
        public IList<BenXe> ListBenXe { get; set; }

        public class BenXe
        {
            public BenXe()
            {
                GioMoLenh = new List<string>();
            }
            public int BenXeId { get; set; }
            public string TenBenXe { get; set; }
            public IList<string> GioMoLenh{get;set;}
        }
        public class GioMoLenh
        {
            public int BenXeId { get; set; }
            public string Ten { get; set; }
        }
    }
}