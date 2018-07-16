using Nop.Core.Domain.ChuyenPhatNhanh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.ChuyenPhatNhanh
{
    public class DB_GioMoLenhMap : NopEntityTypeConfiguration<DB_GioMoLenh>
    {
        public DB_GioMoLenhMap()
        {
            this.ToTable("CV_DB_GioMoLenh");
            this.HasKey(c => c.Id);
            this.Property(u => u.GioMoLenh).HasMaxLength(200);
             this.HasRequired(c => c.benxe)
              .WithMany()
              .HasForeignKey(c => c.BenXeId);
        }
    }
}
