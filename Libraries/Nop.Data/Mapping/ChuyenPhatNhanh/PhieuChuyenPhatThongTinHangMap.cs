using Nop.Core.Domain.ChuyenPhatNhanh;
using Nop.Core.Domain.NhaXes;
using System;
using System.Collections.Generic;


namespace Nop.Data.Mapping.ChuyenPhatNhanh
{
    public class PhieuChuyenPhatThongTinHangMap : NopEntityTypeConfiguration<PhieuChuyenPhatThongTinHang>
    {
        public PhieuChuyenPhatThongTinHangMap()
        {
            this.ToTable("CV_PhieuChuyenPhatThongTinHang");
            this.HasKey(c => c.Id);

            this.HasRequired(c => c.phieuchuyenphat)
                .WithMany(o => o.thongtinhangs)
                .HasForeignKey(c => c.PhieuChuyenPhatId);
        }
    }
}
