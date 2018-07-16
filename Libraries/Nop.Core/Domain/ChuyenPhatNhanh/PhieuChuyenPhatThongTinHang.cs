using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.ChuyenPhatNhanh
{
    public class PhieuChuyenPhatThongTinHang : BaseEntity
    {
        public int Id { get; set; }
        public int PhieuChuyenPhatId { get; set; }
        public virtual PhieuChuyenPhat phieuchuyenphat { get; set; }
        public string TenHang { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaTien { get; set; }

    }
}
