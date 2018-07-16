using Nop.Core.Domain.ChuyenPhatNhanh;
using Nop.Core.Domain.NhaXes;
using System;
using System.Collections.Generic;


namespace Nop.Data.Mapping.ChuyenPhatNhanh
{
    public class PhieuChuyenPhatLoaiHangMap : NopEntityTypeConfiguration<PhieuChuyenPhatLoaiHang>
    {
        public PhieuChuyenPhatLoaiHangMap()
        {
            this.ToTable("CV_PhieuChuyenPhat_LoaiHang");
            this.HasKey(c => c.Id);

            this.HasRequired(c => c.phieuchuyenphat)
                .WithMany(o => o.loaihangs)
                .HasForeignKey(c => c.PhieuChuyenPhatId);
        }
    }
}
