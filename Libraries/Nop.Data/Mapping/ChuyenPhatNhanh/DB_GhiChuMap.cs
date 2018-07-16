using Nop.Core.Domain.ChuyenPhatNhanh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.ChuyenPhatNhanh
{
    public class DB_GhiChuMap : NopEntityTypeConfiguration<DB_GhiChu>
    {
        public DB_GhiChuMap()
        {
            this.ToTable("CV_DB_GhiChu");
            this.HasKey(c => c.Id);
            this.Property(u => u.GhiChu).HasMaxLength(1000);
        }
    }
}
