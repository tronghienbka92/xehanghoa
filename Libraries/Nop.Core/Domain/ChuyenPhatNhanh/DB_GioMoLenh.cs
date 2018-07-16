using Nop.Core.Domain.Chonves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.ChuyenPhatNhanh
{
    public class DB_GioMoLenh : BaseEntity
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public DateTime NgayTao { get; set; }
        public string GioMoLenh { get; set; }
        public int BenXeId { get; set; }
        public virtual BenXe benxe { get; set; }
    }
}
