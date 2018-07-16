using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.ChuyenPhatNhanh
{
    public class PhieuChuyenPhatLoaiHang : BaseEntity
    {
        public int PhieuChuyenPhatId { get; set; }
        public virtual PhieuChuyenPhat phieuchuyenphat { get; set; }
        public int LoaiHangId { get; set; }
    }
}
