using Nop.Core.Domain.ChuyenPhatNhanh;
using Nop.Core.Domain.NhaXes;
using System;
using System.Collections.Generic;


namespace Nop.Data.Mapping.ChuyenPhatNhanh
{
    public class PhieuChuyenPhatTinhChatHangMap : NopEntityTypeConfiguration<PhieuChuyenPhatTinhChatHang>
    {
        public PhieuChuyenPhatTinhChatHangMap()
        {
            this.ToTable("CV_PhieuChuyenPhat_TinhChatHang");
            this.HasKey(c => c.Id);

            this.HasRequired(c => c.phieuchuyenphat)
                .WithMany(o => o.tinhchathangs)
                .HasForeignKey(c => c.PhieuChuyenPhatId);
        }
    }
}
