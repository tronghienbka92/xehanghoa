using Nop.Core.Domain.NhaXes;
using System;
using System.Collections.Generic;

namespace Nop.Data.Mapping.NhaXes
{
    public class VanPhongMap : NopEntityTypeConfiguration<VanPhong>
    {
        public VanPhongMap()
        {
            this.ToTable("CV_VanPhong");
            this.HasKey(c => c.Id);
            this.Property(c => c.TenVanPhong).HasMaxLength(500);
            this.Property(c => c.Ma).HasMaxLength(50);
            this.Property(c => c.DienThoaiDatVe).HasMaxLength(100);
            this.Property(c => c.DienThoaiGuiHang).HasMaxLength(100);
            this.HasRequired(c => c.diachiinfo)
           .WithMany()
           .HasForeignKey(c => c.DiaChiID);
            this.HasOptional(c => c.khuvuc)
          .WithMany(o => o.vanphongs)
          .HasForeignKey(c => c.KhuVucId);
           
            this.Ignore(c => c.KieuVanPhong);


        }
    }
}
