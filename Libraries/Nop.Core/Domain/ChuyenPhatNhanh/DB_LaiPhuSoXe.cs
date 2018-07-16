using Nop.Core.Domain.NhaXes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.ChuyenPhatNhanh
{
    public class DB_LaiPhuSoXe : BaseEntity
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public DateTime NgayTao { get; set; }
        public string Ten { get; set; }
        public int LoaiId { get; set; }
        public LoaiLaiPhuSoXe loai
        {
            get
            {
                return (LoaiLaiPhuSoXe)LoaiId;
            }
            set
            {
                LoaiId = (int)value;
            }
        }
    }
}
