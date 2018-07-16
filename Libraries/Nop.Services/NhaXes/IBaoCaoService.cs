using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.NhaXes;
using Nop.Core.Domain.Chonves;
using System;

namespace Nop.Services.NhaXes
{
    public partial interface IBaoCaoService
    {
        #region Xe xuat ben
        List<HistoryXeXuatBen> GetXeXuatBens(int NhaXeId, int XeVanChuyenId = 0, int[] hanhtrinhIds = null, int[] laiphuxeids = null, DateTime? TuNgay = null, DateTime? DenNgay = null, int BenXuatPhatId=0);
        #endregion
        #region Phoi ve
        Decimal GetTongDoanhThuChuyenDi(int[] NguoVeIds, DateTime? TuNgay = null, DateTime? DenNgay = null);
        #endregion
       
    }
}
