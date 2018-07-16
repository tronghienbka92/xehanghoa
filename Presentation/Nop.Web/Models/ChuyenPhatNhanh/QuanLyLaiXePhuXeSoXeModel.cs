using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Models.ChuyenPhatNhanh
{
    public class QuanLyLaiXePhuXeSoXeModel
    {
        public QuanLyLaiXePhuXeSoXeModel()
        {
            ListThang = new List<SelectListItem>();
            ListNam = new List<SelectListItem>();
            ListLaiXe = new List<string>();
            ListPhuXe = new List<string>();
            ListSoXe = new List<string>();
        }
        public string Nam { get; set; }
        public string Thang { get; set; }
        public int ThangId { get; set; }
        public int NamId { get; set; }
        public bool isEnable { get; set; }
        public IList<SelectListItem> ListThang { get; set; }
        public IList<SelectListItem> ListNam { get; set; }
        public IList<string> ListLaiXe { get; set; }
        public IList<string> ListPhuXe { get; set; }
        public IList<string> ListSoXe { get; set; }
    }
}