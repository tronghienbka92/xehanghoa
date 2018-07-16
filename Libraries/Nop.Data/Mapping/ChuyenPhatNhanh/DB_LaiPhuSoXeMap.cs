using Nop.Core.Domain.ChuyenPhatNhanh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.ChuyenPhatNhanh
{
    public class DB_LaiPhuSoXeMap : NopEntityTypeConfiguration<DB_LaiPhuSoXe>
    {
        public DB_LaiPhuSoXeMap()
        {
            this.ToTable("CV_DB_LaiPhuSoXe");
            this.HasKey(c => c.Id);
            this.Property(u => u.Ten).HasMaxLength(200);

            this.Ignore(c => c.loai);
           
        }
    }
}
