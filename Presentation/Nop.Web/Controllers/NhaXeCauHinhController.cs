using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.UI.Captcha;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;
using WebGrease.Css.Extensions;
using Nop.Web.Models.NhaXes;
using Nop.Core.Data;
using Nop.Services.NhaXes;
using Nop.Core.Caching;
using Nop.Core.Domain.News;
using Nop.Core.Domain.NhaXes;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Services.Chonves;
using Nop.Services.Security;
using Nop.Core.Domain.Security;
using System.Globalization;
using Nop.Services.Catalog;
using Nop.Web.Models.VeXeKhach;
using Nop.Core.Domain.Chonves;
using Nop.Services.ChuyenPhatNhanh;
using Nop.Core.Domain.ChuyenPhatNhanh;
using System.Text.RegularExpressions;

namespace Nop.Web.Controllers
{
    public class NhaXeCauHinhController : BaseNhaXeController
    {
        const string _ITEMS = "[ITEMS]";
        const string _ITEM1S = "[ITEM1S]";
        #region Khoi Tao
        private readonly IStateProvinceService _stateProvinceService;
        private readonly INhaXeService _nhaxeService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IPictureService _pictureService;
        private readonly IPhieuChuyenPhatService _phieuchuyenphatService;
        private readonly ICustomerService _customerService;
        private readonly IDiaChiService _diachiService;
        private readonly INhanVienService _nhanvienService;
        private readonly IPermissionService _permissionService;
        private readonly IXeInfoService _xeinfoService;
        private readonly IHanhTrinhService _hanhtrinhService;
        private readonly IBenXeService _benxeService;
        private readonly INhaXeCustomerService _nhaxecustomerService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPhoiVeService _phoiveService;
        public NhaXeCauHinhController(IStateProvinceService stateProvinceService,
            INhaXeService nhaxeService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IPictureService pictureService,
             IPhieuChuyenPhatService phieuchuyenphatService,
            IHangHoaService hanghoaService,
            ICustomerService customerService,
            IDiaChiService diachiService,
            INhanVienService nhanvienService,
            IPermissionService permissionService,
            IXeInfoService xeinfoService,
            IHanhTrinhService hanhtrinhService,
            IBenXeService benxeService,
            INhaXeCustomerService nhaxecustomerService,
            IPriceFormatter priceFormatter,
            IPhoiVeService phoiveService
            )
        {
            this._phoiveService = phoiveService;
            this._priceFormatter = priceFormatter;
            this._stateProvinceService = stateProvinceService;
            this._nhaxeService = nhaxeService;
            this._phieuchuyenphatService = phieuchuyenphatService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._pictureService = pictureService;
            this._customerService = customerService;
            this._diachiService = diachiService;
            this._nhanvienService = nhanvienService;
            this._permissionService = permissionService;
            this._xeinfoService = xeinfoService;
            this._hanhtrinhService = hanhtrinhService;
            this._benxeService = benxeService;
            this._nhaxecustomerService = nhaxecustomerService;
        }
        #endregion
        NhaXeCauHinhModel fromEntity(NhaXeCauHinh item)
        {
            var model = new NhaXeCauHinhModel();
            model.Id = item.Id;
            model.kieudulieu = item.kieudulieu;
            model.MaCauHinh = item.MaCauHinh;
            model.Ten = item.Ten;
            model.GiaTri = item.GiaTri;
            return model;
        }
        NhaXeCauHinh fromModel(NhaXeCauHinhModel model)
        {
            var item = new NhaXeCauHinh();
            item.NhaXeId = _workContext.NhaXeId;
            item.MaCauHinh = model.MaCauHinh;
            item.kieudulieu = model.kieudulieu;
            item.Ten = model.Ten;
            item.GiaTri = model.GiaTri;
            return item;
        }
        void setGiaTriSubItem(NhaXeCauHinhModel model, bool isSet = true)
        {
            try
            {
                bool isCheck = false;
                switch (model.MaCauHinh)
                {

                    case ENNhaXeCauHinh.KY_GUI_MAU_HANG_HOA_XUAT_BEN:
                        {
                            var _ItemPerPage = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, ENNhaXeCauHinh.KY_GUI_MAU_HANG_HOA_XUAT_BEN_PAGES);
                            if (_ItemPerPage != null)
                                model.ItemPerPage = Convert.ToInt32(_ItemPerPage.GiaTri);
                            var _startendrepeat = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, ENNhaXeCauHinh.KY_GUI_MAU_HANG_HOA_XUAT_BEN_REPEATSTARTEND);
                            if (_startendrepeat != null)
                            {
                                string[] arrstartend = _startendrepeat.GiaTri.Split('|');
                                if (arrstartend.Length == 2)
                                {
                                    model.KyTuRepeatStart = arrstartend[0];
                                    model.KyTuRepeatEnd = arrstartend[1];
                                    isCheck = true;
                                }
                            }

                            var _solien = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, ENNhaXeCauHinh.KY_GUI_MAU_HANG_HOA_XUAT_BEN_LIEN);
                            if (_solien != null)
                                model.SoLien = Convert.ToInt32(_solien.GiaTri);

                            break;
                        }
                    case ENNhaXeCauHinh.VE_MAU_IN_PHOI:
                        {
                            var _ItemPerPage = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, ENNhaXeCauHinh.VE_MAU_IN_PHOI_PAGES);
                            if (_ItemPerPage != null)
                                model.ItemPerPage = Convert.ToInt32(_ItemPerPage.GiaTri);
                            var _startendrepeat = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, ENNhaXeCauHinh.VE_MAU_IN_PHOI_REPEATSTARTEND);
                            if (_startendrepeat != null)
                            {
                                string[] arrstartend = _startendrepeat.GiaTri.Split('|');
                                if (arrstartend.Length == 2)
                                {
                                    model.KyTuRepeatStart = arrstartend[0];
                                    model.KyTuRepeatEnd = arrstartend[1];
                                    isCheck = true;
                                }
                            }
                            break;
                        }
                    case ENNhaXeCauHinh.VE_MAU_IN_CUONG_VE:
                        {
                            var _solien = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, ENNhaXeCauHinh.VE_MAU_IN_CUONG_VE_LIEN);
                            if (_solien != null)
                                model.SoLien = Convert.ToInt32(_solien.GiaTri);
                            break;
                        }
                    case ENNhaXeCauHinh.KY_GUI_PHIEU_GUI_HANG:
                        {
                            var _solien = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, ENNhaXeCauHinh.KY_GUI_PHIEU_GUI_HANG_LIEN);
                            if (_solien != null)
                                model.SoLien = Convert.ToInt32(_solien.GiaTri);
                            string _startendrepeat = "<tr class=\"repeat\">|</tr>";
                            string[] arrstartend = _startendrepeat.Split('|');
                            if (arrstartend.Length == 2)
                            {
                                model.KyTuRepeatStart = arrstartend[0];
                                model.KyTuRepeatEnd = arrstartend[1];
                                isCheck = true;
                            }
                            break;
                        }
                }
                if (!isCheck || !isSet)
                    return;

                int _posrepeat = model.GiaTri.IndexOf(model.KyTuRepeatStart);
                if (_posrepeat > 0)
                {
                    string _part1 = model.GiaTri.Substring(0, _posrepeat);
                    string _part2 = model.GiaTri.Substring(_posrepeat);
                    //int _posendrepeat = _part2.IndexOf(model.KyTuRepeatEnd);
                    int _posendrepeat = _part2.LastIndexOf(model.KyTuRepeatEnd);
                    if(model.MaCauHinh == ENNhaXeCauHinh.KY_GUI_MAU_HANG_HOA_XUAT_BEN)
                        _posendrepeat = _part2.IndexOf(model.KyTuRepeatEnd);
                    string _part3 = _part2.Substring(_posendrepeat + model.KyTuRepeatEnd.Length);
                    _part2 = _part2.Substring(0, _posendrepeat + model.KyTuRepeatEnd.Length);
                    model.GiaTri = _part1 + _ITEMS + _part3;
                    model.GiaTriItem = _part2;
                    //kiem tra tiep co vong lap ko
                    _posrepeat = model.GiaTri.IndexOf(model.KyTuRepeatStart1);
                    if (_posrepeat > 0)
                    {
                        _part1 = model.GiaTri.Substring(0, _posrepeat);
                        _part2 = model.GiaTri.Substring(_posrepeat);
                        _posendrepeat = _part2.IndexOf(model.KyTuRepeatEnd1);
                        _part3 = _part2.Substring(_posendrepeat + model.KyTuRepeatEnd1.Length);
                        _part2 = _part2.Substring(0, _posendrepeat + model.KyTuRepeatEnd1.Length);
                        model.GiaTri = _part1 + _ITEM1S + _part3;
                        model.GiaTriItem1 = _part2;
                    }

                }


            }
            catch
            { }

        }
        string getGiaTri(string _item, string code, string val)
        {
            return _item.Replace("[" + code + "]", val);
        }
        void setGiaTri(NhaXeCauHinhModel model, string code, string val, bool isEmpty = false)
        {
            if (isEmpty)
                model.GiaTri = model.GiaTri.Replace(code, val);
            else
                model.GiaTri = model.GiaTri.Replace("[" + code + "]", val);
        }
        void setGiaTri(NhaXeCauHinhModel model, string code, int val)
        {
            model.GiaTri = model.GiaTri.Replace("[" + code + "]", val.ToSoNguyen());
        }
        void setGiaTri(NhaXeCauHinhModel model, string code, Decimal val)
        {
            model.GiaTri = model.GiaTri.Replace("[" + code + "]", val.ToTien(_priceFormatter));
        }
        void setGiaTriNgayThang(NhaXeCauHinhModel model, DateTime dt)
        {
            setGiaTri(model, "NGAY", dt.ToString("dd"));
            setGiaTri(model, "THANG", dt.ToString("MM"));
            setGiaTri(model, "NAM", dt.ToString("yyyy"));
        }
        
        void setGiaTriModel(NhaXeCauHinhModel model, int Id)
        {
            setGiaTriNgayThang(model, DateTime.Now);
            switch (model.MaCauHinh)
            {
                case ENNhaXeCauHinh.VE_MAU_IN_CUONG_VE:
                    {
                        //lay thong tin phoi ve

                        var phieuguihangs = _phieuchuyenphatService.GetAllPhieuChuyenPhat(_workContext.NhaXeId, 0, model.NgayTao, "", ENTrangThaiChuyenPhat.DenVanPhongNhan, 0, Id);


                        setGiaTri(model, "MA_VAN_PHONG", _workContext.CurrentVanPhong.Ma);


                        string _itemcontents = "";
                        int i = 1;
                        decimal tongcuoc = decimal.Zero;
                        foreach (var p in phieuguihangs)
                        {
                            var _CuocTanNoi = p.CuocTanNoi / 1000 + p.CuocNhanTanNoi / 1000;
                            var conlaiTB_HN = "";
                            var conlaiTB_ND = "";

                            string _itemcontent = model.GiaTriItem;
                            _itemcontent = getGiaTri(_itemcontent, "STT", i.ToString());
                            _itemcontent = getGiaTri(_itemcontent, "MA_PHIEU", p.MaPhieu);
                            _itemcontent = getGiaTri(_itemcontent, "NGUOIGUI_TEN", p.NguoiGui.HoTen);
                            _itemcontent = getGiaTri(_itemcontent, "NGUOIGUI_DIENTHOAI", p.NguoiGui.SoDienThoai);
                            _itemcontent = getGiaTri(_itemcontent, "VUNG", p.VanPhongNhan.Ma);
                            _itemcontent = getGiaTri(_itemcontent, "NGUOINHAN_TEN", p.NguoiNhan.HoTen);
                            _itemcontent = getGiaTri(_itemcontent, "NGUOINHAN_DIENTHOAI", p.NguoiNhan.SoDienThoai);
                            _itemcontent = getGiaTri(_itemcontent, "TEN_HANG", p.TenHang);
                            _itemcontent = getGiaTri(_itemcontent, "CUOC_DTT", Convert.ToInt32(p.TongCuocDaThanhToan / 1000).ToSoNguyen());
                            _itemcontent = getGiaTri(_itemcontent, "CUOC_CTT", Convert.ToInt32(p.TongTienCuoc / 1000 - p.TongCuocDaThanhToan / 1000).ToSoNguyen());
                            _itemcontent = getGiaTri(_itemcontent, "V_C", Convert.ToInt32(_CuocTanNoi).ToSoNguyen());
                            _itemcontent = getGiaTri(_itemcontent, "CON_LAI", Convert.ToInt32(p.TongTienCuoc / 1000 - _CuocTanNoi).ToSoNguyen());
                            _itemcontent = getGiaTri(_itemcontent, "CON_LAI_TB_HN", conlaiTB_HN);
                            _itemcontent = getGiaTri(_itemcontent, "CON_LAI_TB_ND", conlaiTB_ND);



                            _itemcontents = _itemcontents + _itemcontent;
                            tongcuoc += p.TongCuocDaThanhToan / 1000;
                            i++;
                        }
                        if (i < model.ItemPerPage)
                        {
                            for (int j = i; j <= model.ItemPerPage; j++)
                            {
                                string _itemcontent = model.GiaTriItem;
                                _itemcontent = getGiaTri(_itemcontent, "STT", j.ToString());
                                _itemcontent = getGiaTri(_itemcontent, "MA_PHIEU", "");
                                _itemcontent = getGiaTri(_itemcontent, "NGUOIGUI_TEN", "");
                                _itemcontent = getGiaTri(_itemcontent, "NGUOIGUI_DIENTHOAI", "");
                                _itemcontent = getGiaTri(_itemcontent, "VUNG", "");
                                _itemcontent = getGiaTri(_itemcontent, "NGUOINHAN_TEN", "");
                                _itemcontent = getGiaTri(_itemcontent, "NGUOINHAN_DIENTHOAI", "");
                                _itemcontent = getGiaTri(_itemcontent, "TEN_HANG", "");
                                _itemcontent = getGiaTri(_itemcontent, "CUOC_DTT", "");
                                _itemcontent = getGiaTri(_itemcontent, "CUOC_CTT", "");
                                _itemcontent = getGiaTri(_itemcontent, "V_C", "");
                                _itemcontent = getGiaTri(_itemcontent, "CON_LAI", "");
                                _itemcontent = getGiaTri(_itemcontent, "CON_LAI_TB_HN", "");
                                _itemcontent = getGiaTri(_itemcontent, "CON_LAI_TB_ND", "");

                                _itemcontents = _itemcontents + _itemcontent;
                            }
                        }
                        setGiaTri(model, _ITEMS, _itemcontents, true);
                        setGiaTri(model, "TONG_TIEN_CUOC", (tongcuoc * 1000).ToSoNguyen());

                        //  setGiaTri(model, "LIEN_NUM", 2);//tu dien vao so lien
                        break;
                    }

                case ENNhaXeCauHinh.KY_GUI_PHIEU_GUI_HANG:
                    {
                        //lay thong tin phieu gui hang
                        var item = _phieuchuyenphatService.GetPhieuChuyenPhatById(Id);
                        var thongtinhangs = _phieuchuyenphatService.GetPhieuChuyenPhatThongTinHangByPhieuChuyenPhatId(item.Id).Take(4);
                        var _tinhchathangs = _phieuchuyenphatService.GetPhieuChuyenPhatTinhChatHangPCPId(item.Id).Select(c=>c.TinhChatHangId).ToArray();
                        var _loaihangs = _phieuchuyenphatService.GetPhieuChuyenPhatLoaiHangPCPId(item.Id).Select(c => c.LoaiHangId).ToArray();
                        string _itemcontents = "";
                        string thongtinhang = "";
                        if (item != null && item.NhaXeId == _workContext.NhaXeId)
                        {
                            //
                            
                            foreach(var hang in thongtinhangs)
                            {
                                string _itemcontent = model.GiaTriItem;
                                _itemcontent = getGiaTri(_itemcontent, "STT", "1");
                                _itemcontent = getGiaTri(_itemcontent, "TENHANG", hang.TenHang);
                                _itemcontent = getGiaTri(_itemcontent, "SL", hang.SoLuong.ToString());
                                _itemcontent = getGiaTri(_itemcontent, "KL", "");
                                _itemcontent = getGiaTri(_itemcontent, "TRIGIA", "");
                                _itemcontent = getGiaTri(_itemcontent, "CUOCPHI", hang.GiaTien.ToString());
                                _itemcontents = _itemcontents + _itemcontent;
                            }

                            //cap nhat lan in:
                            item.DaIn = item.DaIn + 1;
                            _phieuchuyenphatService.UpdatePhieuChuyenPhat(item);
                            //luu log in
                            _phieuchuyenphatService.InsertPhieuChuyenPhatLog(_workContext.CurrentNhanVien.HoVaTen + "(" + _workContext.CurrentNhanVien.Email + ")" + " in phiếu biên nhận lần thứ " + item.DaIn.ToString(), item.Id);
                            setGiaTri(model, _ITEMS, _itemcontents, true);
                            var tinhchathangs = this.GetCVEnumSelectList<ENTinhChatHang>(_localizationService);
                            foreach (var tch in tinhchathangs)
                            {
                                if (_tinhchathangs.Contains(Convert.ToInt32(tch.Value)))
                                {
                                    setGiaTri(model, "TINHCHATHANG" + Convert.ToInt32(tch.Value), "<input checked type='checkbox' />");
                                }
                                else
                                {
                                    setGiaTri(model, "TINHCHATHANG" + Convert.ToInt32(tch.Value), "<input type='checkbox' />");
                                }
                            }
                            var loaihangs = this.GetCVEnumSelectList<ENLoaiHangKhongKhaiGiaTri>(_localizationService);
                            foreach (var tch in tinhchathangs)
                            {
                                if (_loaihangs.Contains(Convert.ToInt32(tch.Value)))
                                {
                                    setGiaTri(model, "LOAIHANG" + Convert.ToInt32(tch.Value), "<input checked type='checkbox' />");
                                }
                                else
                                {
                                    setGiaTri(model, "LOAIHANG" + Convert.ToInt32(tch.Value), "<input type='checkbox' />");
                                }
                            }
                            if (thongtinhangs.Count() > 0)
                            {
                                var STT = 1;
                                foreach (var hang in item.thongtinhangs)
                                {
                                    setGiaTri(model, "STT" + STT, STT.ToString());
                                    setGiaTri(model, "TENHANG" + STT, hang.TenHang);
                                    setGiaTri(model, "SL" + STT, hang.SoLuong.ToString());
                                    setGiaTri(model, "KL" + STT, "");
                                    setGiaTri(model, "TRIGIA" + STT, "");
                                    setGiaTri(model, "CUOCPHI" + STT, hang.GiaTien.ToString());
                                    STT++;
                                }
                                if (STT < 4)
                                {
                                    for (int i = STT; i <= 4; i++)
                                    {
                                        setGiaTri(model, "STT" + i, "");
                                        setGiaTri(model, "TENHANG" + i, "");
                                        setGiaTri(model, "SL" + i, "");
                                        setGiaTri(model, "KL" + i, "");
                                        setGiaTri(model, "TRIGIA" + i, "");
                                        setGiaTri(model, "CUOCPHI" + i, "");
                                    }
                                }
                                setGiaTri(model, "SLTONG", thongtinhangs.Sum(c => c.SoLuong).ToString());
                                setGiaTri(model, "KLTONG", "");
                                setGiaTri(model, "TRIGIATONG", "");
                                setGiaTri(model, "CUOCPHITONG", item.TongTienCuoc.ToString());
                                setGiaTri(model, "CUOCDTT", item.TongCuocDaThanhToan.ToString());
                                setGiaTri(model, "CUOCCTT", (item.TongTienCuoc - item.TongCuocDaThanhToan).ToString());
                            }

                            setGiaTri(model, "GIO", item.NgayTao.ToString("HH:mm"));
                            setGiaTri(model, "MA", item.MaPhieu);
                            setGiaTri(model, "VPGUI_TEN", item.VanPhongGui.TenVanPhong);
                            setGiaTri(model, "TENVANPHONGGUI", item.VanPhongGui.Ma);
                            setGiaTri(model, "SDTVPGUI", item.VanPhongGui.DienThoaiDatVe);


                            if (item.VanPhongGui.diachiinfo != null)
                            {
                                setGiaTri(model, "DIACHIVPGUI", item.VanPhongGui.diachiinfo.ToText());
                                setGiaTri(model, "VPGUI_FAX", item.VanPhongGui.diachiinfo.Fax);
                            }
                            else
                            {
                                setGiaTri(model, "DIACHIVPGUI", "");
                                setGiaTri(model, "VPGUI_FAX", "");
                            }

                            setGiaTri(model, "VPNHAN_TEN", item.VanPhongNhan.TenVanPhong);
                            setGiaTri(model, "TENVANPHONGNHAN", item.VanPhongNhan.Ma);
                            setGiaTri(model, "SDTVPNHAN", item.VanPhongNhan.DienThoaiDatVe);
                            if (item.VanPhongNhan.diachiinfo != null)
                            {
                                setGiaTri(model, "DIACHIVPNHAN", item.VanPhongNhan.diachiinfo.ToText());
                                setGiaTri(model, "VPNHAN_FAX", item.VanPhongNhan.diachiinfo.Fax);
                            }
                            else
                            {
                                setGiaTri(model, "DIACHIVPNHAN", "");
                                setGiaTri(model, "VPNHAN_FAX", "");
                            }

                            setGiaTri(model, "NGUOIGUI_TEN", item.NguoiGui.HoTen);
                            setGiaTri(model, "NGAY", item.NgayNhanHang.Day.ToString());
                            setGiaTri(model, "THANG", item.NgayNhanHang.Month.ToString());
                            setGiaTri(model, "NAM", item.NgayNhanHang.Year.ToString());
                            setGiaTri(model, "NGUOIGUI_DIACHI", item.NguoiGui.DiaChi);
                            setGiaTri(model, "NGUOIGUI_DIENTHOAI", item.NguoiGui.SoDienThoai);
                            setGiaTri(model, "NGUOINHAN_TEN", item.NguoiNhan.HoTen);
                            setGiaTri(model, "NGUOINHAN_DIACHI", item.NguoiNhan.DiaChi);
                            setGiaTri(model, "NGUOINHAN_DIENTHOAI", item.NguoiNhan.SoDienThoai);
                            setGiaTri(model, "GHI_CHU", item.GhiChu);
                            setGiaTri(model, "THONG_TIN_HANG_HOA", item.TenHang);
                            setGiaTri(model, "KHOI_LUONG", "0");
                            setGiaTri(model, "KICH_THUOC", "0");
                            setGiaTri(model, "CUOC_NHAN_TAN_NOI", item.CuocNhanTanNoi.ToTien(_priceFormatter));
                            setGiaTri(model, "CUOC_TRA_TAN_NOI", item.CuocTanNoi.ToTien(_priceFormatter));
                            setGiaTri(model, "CUOC_PHI", item.CuocPhi.ToTien(_priceFormatter));
                            setGiaTri(model, "CUOC_TRI_GIA", item.CuocGiaTri.ToTien(_priceFormatter));
                            setGiaTri(model, "CUOC_CAP_TOC", item.CuocCapToc.ToTien(_priceFormatter));
                            setGiaTri(model, "CUOC_VUOT_TUYEN", item.CuocVuotTuyen.ToTien(_priceFormatter));
                            setGiaTri(model, "TONG_TIEN_CUOC", item.TongTienCuoc.ToTien(_priceFormatter));
                            setGiaTri(model, "TIEN_CUOC_DA_THANH_TOAN", item.TongCuocDaThanhToan.ToTien(_priceFormatter));
                            setGiaTri(model, "TIEN_CUOC_CHUA_THANH_TOAN", (item.TongTienCuoc - item.TongCuocDaThanhToan).ToTien(_priceFormatter));
                            string tienchu = item.TongTienCuoc.ToTienBangChu();

                            setGiaTri(model, "TIEN_CUOC_CHU", tienchu);
                            string contents = "";
                            for (int l = 1; l <= model.SoLien; l++)
                            {
                                var text_lien = "";
                                if (l == 1)
                                    text_lien = "Lưu";
                                if (l == 2)
                                    text_lien = "Giao khách hàng";
                                if (l == 3)
                                    text_lien = "Dán lên hàng";


                                contents = contents + model.GiaTri.Replace("[LIEN_NUM]", string.Format("{0}:{1}", l, text_lien));
                            }
                            model.GiaTri = contents;
                        }
                        break;
                    }
                case ENNhaXeCauHinh.KY_GUI_MAU_HANG_HOA_XUAT_BEN:
                    {
                        var phieuvanchuyen = _phieuchuyenphatService.GetPhieuVanChuyenById(Id);
                        var phieuguihangs = phieuvanchuyen.phieuchuyenphats;

                        setGiaTri(model, "MA", phieuvanchuyen.SoLenhNum);
                        setGiaTri(model, "MA_VAN_PHONG", _workContext.CurrentVanPhong.Ma);
                        setGiaTri(model, "KHU_VUC", _workContext.CurrentVanPhong.khuvuc.TenVietTat + "-->" + phieuvanchuyen.KhuVucDen.TenVietTat);


                        string _itemcontents = "";
                        int i = 1;
                        foreach (var p in phieuguihangs)
                        {
                            var thongtinhangs = _phieuchuyenphatService.GetPhieuChuyenPhatThongTinHangByPhieuChuyenPhatId(p.Id);
                            foreach (var item in thongtinhangs)
                            {
                                string _itemcontent = model.GiaTriItem;
                                _itemcontent = getGiaTri(_itemcontent, "STT", i.ToString());
                                _itemcontent = getGiaTri(_itemcontent, "MAPHIEU", p.MaPhieu);
                                _itemcontent = getGiaTri(_itemcontent, "TENHANG", item.TenHang);
                                _itemcontent = getGiaTri(_itemcontent, "SOLUONG", item.SoLuong.ToString());
                                var tinhchathangs = _phieuchuyenphatService.GetPhieuChuyenPhatTinhChatHangPCPId(p.Id);
                                var tinhchathang=(ENTinhChatHang)tinhchathangs.FirstOrDefault().TinhChatHangId;
                                _itemcontent = getGiaTri(_itemcontent, "GHICHU", tinhchathang.ToCVEnumText(_localizationService));
                                _itemcontents = _itemcontents + _itemcontent;                                
                                i++;
                            }                                                
                        }
                        if (i < model.ItemPerPage)
                        {
                            for (int j = i; j <= model.ItemPerPage; j++)
                            {
                                string _itemcontent = model.GiaTriItem;
                                _itemcontent = getGiaTri(_itemcontent, "STT", j.ToString());
                                _itemcontent = getGiaTri(_itemcontent, "MAPHIEU", "");
                                _itemcontent = getGiaTri(_itemcontent, "TENHANG", "");
                                _itemcontent = getGiaTri(_itemcontent, "SOLUONG", "");
                                _itemcontent = getGiaTri(_itemcontent, "GHICHU", "");
                                _itemcontents = _itemcontents + _itemcontent;
                            }
                        }
                        setGiaTri(model, _ITEMS, _itemcontents, true);                     
                        break;
                    }
                case ENNhaXeCauHinh.VE_MAU_IN_PHOI:
                    {
                        var _historyxexuatben = _nhaxeService.GetHistoryXeXuatBenId(Id);
                        if (_historyxexuatben != null && _historyxexuatben.NguonVeInfo.NhaXeId == _workContext.NhaXeId)
                        {

                            setGiaTri(model, "LICH_TRINH", _historyxexuatben.NguonVeInfo.LichTrinhInfo.ThoiGianDi.ToString("HH:mm"));
                            setGiaTri(model, "VANPHONG_HIENTAI", _workContext.CurrentVanPhong.TenVanPhong);
                            setGiaTri(model, "SO_XE", _historyxexuatben.xevanchuyen.BienSo);
                            setGiaTri(model, "LAI_XE_1", _historyxexuatben.ThongTinLaiPhuXe());
                            setGiaTri(model, "LAI_XE_2", _historyxexuatben.ThongTinLaiPhuXe(1));
                            setGiaTri(model, "LAI_XE_3", _historyxexuatben.ThongTinLaiPhuXe(2));

                            //lay thong ti khach di xe
                            var hanhkhachs = _phoiveService.GetPhoiVeByChuyenDi(_historyxexuatben.NguonVeId, _historyxexuatben.NgayDi);
                            var hanhkhachnews = from hk in hanhkhachs
                                                group hk by new { hk.CustomerId, hk.getNguonVeXe().TenDiemDen, hk.GhiChu, hk.GiaVeHienTai, hk.customer }
                                                    into grp
                                                    select new
                                                    {
                                                        SoLuong = grp.Count(),
                                                        CustomerId = grp.Key.CustomerId,
                                                        GhiChu = grp.Key.GhiChu,
                                                        TenDiemDen = grp.Key.TenDiemDen,
                                                        GiaVeHienTai = grp.Key.GiaVeHienTai,
                                                        KhachHang = string.Format("{0} ({1})", grp.Key.customer.GetFullName(), grp.Key.customer.GetPhone()),
                                                        SoGhes = grp.Select(c => c.sodoghexequytac.Val).ToList()

                                                    };



                            string _itemcontents = "";
                            int i = 1;
                            int tongxuatphat = 0, tongdon = 0;
                            foreach (var hk in hanhkhachs)
                            {
                                //ve dang o trang thai cho xu ly, se la dang don
                                if (hk.TrangThai == ENTrangThaiPhoiVe.ChoXuLy)
                                    tongdon++;
                                else
                                    tongxuatphat++;
                            }

                            foreach (var hk in hanhkhachnews)
                            {
                                string _itemcontent = model.GiaTriItem;
                                _itemcontent = getGiaTri(_itemcontent, "SO_LUONG", hk.SoLuong.ToString());
                                _itemcontent = getGiaTri(_itemcontent, "DIEM_DEN", hk.TenDiemDen);
                                _itemcontent = getGiaTri(_itemcontent, "DON_GIA", hk.GiaVeHienTai.ToSoNguyen());
                                _itemcontent = getGiaTri(_itemcontent, "THANH_TIEN", (hk.GiaVeHienTai * hk.SoLuong).ToSoNguyen());
                                _itemcontent = getGiaTri(_itemcontent, "KHACH_HANG", hk.KhachHang);
                                string soghes = "";
                                foreach (var sg in hk.SoGhes)
                                {
                                    if (string.IsNullOrEmpty(soghes))
                                        soghes = sg;
                                    else
                                        soghes = soghes + ", " + sg;
                                }
                                _itemcontent = getGiaTri(_itemcontent, "SO_GHE", soghes);

                                var hktrangthai = hanhkhachs.Where(c => c.CustomerId == hk.CustomerId).First();
                                if (hktrangthai.TrangThai == ENTrangThaiPhoiVe.ChoXuLy)
                                    _itemcontent = getGiaTri(_itemcontent, "GHI_CHU", "Đón(Chưa thanh toán)");
                                else
                                    _itemcontent = getGiaTri(_itemcontent, "GHI_CHU", hk.GhiChu);
                                _itemcontents = _itemcontents + _itemcontent;

                                i++;
                            }
                            if (i < model.ItemPerPage)
                            {
                                for (int j = i; j <= model.ItemPerPage; j++)
                                {
                                    string _itemcontent = model.GiaTriItem;
                                    _itemcontent = getGiaTri(_itemcontent, "SO_LUONG", "");
                                    _itemcontent = getGiaTri(_itemcontent, "DIEM_DEN", "");
                                    _itemcontent = getGiaTri(_itemcontent, "DON_GIA", "");
                                    _itemcontent = getGiaTri(_itemcontent, "THANH_TIEN", "");
                                    _itemcontent = getGiaTri(_itemcontent, "KHACH_HANG", "");
                                    _itemcontent = getGiaTri(_itemcontent, "SO_GHE", "");
                                    _itemcontent = getGiaTri(_itemcontent, "GHI_CHU", "");
                                    _itemcontents = _itemcontents + _itemcontent;
                                }
                            }
                            setGiaTri(model, _ITEMS, _itemcontents, true);

                            setGiaTri(model, "DIEM_DI", _historyxexuatben.NguonVeInfo.TenDiemDon);
                            setGiaTri(model, "TONG_SOLUONG_XUATPHAT", tongxuatphat);
                            setGiaTri(model, "TONG_DON_DUONG", tongdon);
                            setGiaTri(model, "TONG_SO_LUONG", tongxuatphat + tongdon);
                            setGiaTri(model, "MA_LENH_HANG", "HH" + Id.ToString());
                            //vong lap thu 2
                            _itemcontents = "";
                            //lay thong tin cac cung duong
                            var diemdons = _historyxexuatben.NguonVeInfo.LichTrinhInfo.HanhTrinhInfo.DiemDons.OrderBy(c => c.ThuTu).ToList();
                            var diemdon1 = diemdons[0];
                            for (i = 1; i < diemdons.Count; i++)
                            {
                                string _itemcontent = model.GiaTriItem1;
                                _itemcontent = getGiaTri(_itemcontent, "KHACH_XUONG_CUNG_DUONG", string.Format("{0} - {1}", diemdon1.diemdon.TenDiemDon, diemdons[i].diemdon.TenDiemDon));
                                //lay thong tin so luong
                                int soluonghkxuong = 0;
                                string kyhieughes = "";
                                //foreach (var hk in hanhkhachs)
                                //{
                                //    //so sanh hanh khach  den diem den tren hanh trinh

                                //}
                                if (soluonghkxuong > 0)
                                    _itemcontent = getGiaTri(_itemcontent, "KHACH_XUONG_SO_LUONG", soluonghkxuong.ToString());
                                else
                                    _itemcontent = getGiaTri(_itemcontent, "KHACH_XUONG_SO_LUONG", "");
                                _itemcontent = getGiaTri(_itemcontent, "KHACH_XUONG_SO_GHE", kyhieughes);
                                _itemcontent = getGiaTri(_itemcontent, "GHI_CHU", "");
                                _itemcontents = _itemcontents + _itemcontent;
                            }
                            _itemcontents = _itemcontents + getGiaTri(model.GiaTriItem1, "KHACH_XUONG_CUNG_DUONG", string.Format("{0} - KHÁC", diemdon1.diemdon.TenDiemDon));

                            setGiaTri(model, _ITEM1S, _itemcontents, true);

                        }
                        break;
                    }

            }
        }
        public ActionResult InPhieu(int MaID, int Id, DateTime? NgayTao = null)
        {
            var model = new NhaXeCauHinhModel();
            //lay thong tin cau hinh truoc do
            ENNhaXeCauHinh cauhinh = (ENNhaXeCauHinh)MaID;
            var item = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, cauhinh);
            if (item != null)
            {
                model = fromEntity(item);
                if (NgayTao != null)
                    model.NgayTao = NgayTao.Value;
                setGiaTriSubItem(model);
                setGiaTriModel(model, Id);
            }
            else
            {
                model.kieudulieu = getKieuDuLieu(cauhinh);
                model.MaCauHinh = cauhinh;
                model.GiaTri = "Bạn chưa tạo mẫu";
            }
            return View(model);
        }


        public ActionResult Index(string Code)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return this.AccessDeniedView();
            var model = new NhaXeCauHinhModel();
            //lay thong tin cau hinh truoc do
            var item = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, Code);
            if (item != null)
            {
                model = fromEntity(item);
            }
            else
            {
                model.kieudulieu = getKieuDuLieu(Code);
                model.MaCauHinh = (ENNhaXeCauHinh)Enum.Parse(typeof(ENNhaXeCauHinh), Code);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(NhaXeCauHinhModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return this.AccessDeniedView();
            if (ModelState.IsValid)
            {
                var item = fromModel(model);
                _nhaxeService.Insert(item);
            }
            return View(model);
        }
        public ActionResult CuongVe()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return this.AccessDeniedView();
            var model = new NhaXeCauHinhModel();
            var item = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, ENNhaXeCauHinh.VE_MAU_IN_CUONG_VE);
            if (item != null)
            {
                model = fromEntity(item);
                setGiaTriSubItem(model);
            }
            else
            {
                model.Ten = "Mẫu cuống vé";
                model.kieudulieu = ENKieuDuLieu.KY_TU;
                model.MaCauHinh = ENNhaXeCauHinh.VE_MAU_IN_CUONG_VE;
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult CuongVe(NhaXeCauHinhModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return this.AccessDeniedView();
            if (ModelState.IsValid)
            {
                var item = fromModel(model);
                _nhaxeService.Insert(item);

                var _solien = new NhaXeCauHinh();
                _solien.kieudulieu = ENKieuDuLieu.SO;
                _solien.MaCauHinh = ENNhaXeCauHinh.VE_MAU_IN_CUONG_VE_LIEN;
                _solien.GiaTri = model.SoLien.ToString();
                _solien.NhaXeId = _workContext.NhaXeId;
                _solien.Ten = "Mẫu cuống vé (số liên)";
                _nhaxeService.Insert(_solien);

                SuccessNotification("Cập nhật thành công!");
            }
            return View(model);
        }

        public ActionResult PhieuGuiHangHoa()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return this.AccessDeniedView();
            var model = new NhaXeCauHinhModel();
            var item = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, ENNhaXeCauHinh.KY_GUI_PHIEU_GUI_HANG);
            if (item != null)
            {
                model = fromEntity(item);
                //setGiaTriSubItem(model);
            }
            else
            {
                model.Ten = "Mẫu phiếu gửi hàng hóa";
                model.kieudulieu = ENKieuDuLieu.KY_TU;
                model.MaCauHinh = ENNhaXeCauHinh.KY_GUI_PHIEU_GUI_HANG;
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult PhieuGuiHangHoa(NhaXeCauHinhModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return this.AccessDeniedView();
            if (ModelState.IsValid)
            {
                var item = fromModel(model);
                _nhaxeService.Insert(item);
                var _solien = new NhaXeCauHinh();
                _solien.kieudulieu = ENKieuDuLieu.SO;
                _solien.MaCauHinh = ENNhaXeCauHinh.KY_GUI_PHIEU_GUI_HANG_LIEN;
                _solien.GiaTri = model.SoLien.ToString();
                _solien.NhaXeId = _workContext.NhaXeId;
                _solien.Ten = "Mẫu phiếu gửi hàng hóa (số liên)";
                _nhaxeService.Insert(_solien);

                SuccessNotification("Cập nhật thành công!");
            }
            return View(model);
        }
        public ActionResult LenhChuyenHangHoa()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return this.AccessDeniedView();
            var model = new NhaXeCauHinhModel();
            var item = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, ENNhaXeCauHinh.KY_GUI_MAU_HANG_HOA_XUAT_BEN);
            if (item != null)
            {
                model = fromEntity(item);
                setGiaTriSubItem(model, false);

            }
            else
            {
                model.Ten = "Mẫu lệnh chuyển hàng hóa";
                model.kieudulieu = ENKieuDuLieu.KY_TU;
                model.MaCauHinh = ENNhaXeCauHinh.KY_GUI_MAU_HANG_HOA_XUAT_BEN;
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult LenhChuyenHangHoa(NhaXeCauHinhModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return this.AccessDeniedView();
            if (ModelState.IsValid)
            {
                var item = fromModel(model);
                _nhaxeService.Insert(item);
                //cap nhat subitem
                var _pagesize = new NhaXeCauHinh();
                _pagesize.kieudulieu = ENKieuDuLieu.SO;
                _pagesize.MaCauHinh = ENNhaXeCauHinh.KY_GUI_MAU_HANG_HOA_XUAT_BEN_PAGES;
                _pagesize.GiaTri = model.ItemPerPage.ToString();
                _pagesize.NhaXeId = _workContext.NhaXeId;
                _pagesize.Ten = "Mẫu lệnh chuyển hàng hóa (pagesize)";
                _nhaxeService.Insert(_pagesize);
                var _startend = new NhaXeCauHinh();
                _startend.kieudulieu = ENKieuDuLieu.KY_TU;
                _startend.MaCauHinh = ENNhaXeCauHinh.KY_GUI_MAU_HANG_HOA_XUAT_BEN_REPEATSTARTEND;
                _startend.GiaTri = string.Format("{0}|{1}", model.KyTuRepeatStart, model.KyTuRepeatEnd);
                _startend.NhaXeId = _workContext.NhaXeId;
                _startend.Ten = "Mẫu lệnh chuyển hàng hóa (startend)";
                _nhaxeService.Insert(_startend);
                var _solien = new NhaXeCauHinh();
                _solien.kieudulieu = ENKieuDuLieu.SO;
                _solien.MaCauHinh = ENNhaXeCauHinh.KY_GUI_MAU_HANG_HOA_XUAT_BEN_LIEN;
                _solien.GiaTri = model.SoLien.ToString();
                _solien.NhaXeId = _workContext.NhaXeId;
                _solien.Ten = "Mẫu lệnh vận chuyển (số liên)";
                _nhaxeService.Insert(_solien);
                SuccessNotification("Cập nhật thành công!");
            }
            return View(model);
        }

        public ActionResult LenhChuyenKhach()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return this.AccessDeniedView();
            var model = new NhaXeCauHinhModel();
            var item = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, ENNhaXeCauHinh.VE_MAU_IN_PHOI);
            if (item != null)
            {
                model = fromEntity(item);
                setGiaTriSubItem(model, false);
            }
            else
            {
                model.Ten = "Mẫu lệnh chuyển hành khách";
                model.kieudulieu = ENKieuDuLieu.KY_TU;
                model.MaCauHinh = ENNhaXeCauHinh.VE_MAU_IN_PHOI;
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult LenhChuyenKhach(NhaXeCauHinhModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return this.AccessDeniedView();
            if (ModelState.IsValid)
            {
                var item = fromModel(model);
                _nhaxeService.Insert(item);

                //cap nhat subitem
                var _pagesize = new NhaXeCauHinh();
                _pagesize.kieudulieu = ENKieuDuLieu.SO;
                _pagesize.MaCauHinh = ENNhaXeCauHinh.VE_MAU_IN_PHOI_PAGES;
                _pagesize.GiaTri = model.ItemPerPage.ToString();
                _pagesize.NhaXeId = _workContext.NhaXeId;
                _pagesize.Ten = "Mẫu lệnh chuyển hành khách (pagesize)";
                _nhaxeService.Insert(_pagesize);
                var _startend = new NhaXeCauHinh();
                _startend.kieudulieu = ENKieuDuLieu.KY_TU;
                _startend.MaCauHinh = ENNhaXeCauHinh.VE_MAU_IN_PHOI_REPEATSTARTEND;
                _startend.GiaTri = string.Format("{0}|{1}", model.KyTuRepeatStart, model.KyTuRepeatEnd);
                _startend.NhaXeId = _workContext.NhaXeId;
                _startend.Ten = "Mẫu lệnh chuyển hành khách (startend)";
                _nhaxeService.Insert(_startend);
                SuccessNotification("Cập nhật thành công!");

            }
            return View(model);
        }
        NhaXeCauHinhModel CreateCauHinhChung(string Ten, ENKieuDuLieu kieudulieu, ENNhaXeCauHinh MaCauHinh, string GiaTri)
        {
            var item = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, MaCauHinh);
            if (item != null)
            {
                return fromEntity(item);
            }
            else
            {
                var model = new NhaXeCauHinhModel();
                model.Ten = Ten;
                model.kieudulieu = kieudulieu;
                model.MaCauHinh = MaCauHinh;
                model.GiaTri = GiaTri;
                return model;
            }
        }
        /// <summary>
        /// Cau hinh cac thong tin don gian, cac gia tri
        /// </summary>
        /// <returns></returns>
        public ActionResult CauHinhChung()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return this.AccessDeniedView();
            var models = new List<NhaXeCauHinhModel>();
            models.Add(CreateCauHinhChung("Tỉ lệ giảm giá vé cho trẻ em", ENKieuDuLieu.PHAN_TRAM, ENNhaXeCauHinh.GIAM_GIA_CHO_TRE_EM, "50"));
            models.Add(CreateCauHinhChung("Số tiền trên mỗi lượt cho lái xe", ENKieuDuLieu.SO, ENNhaXeCauHinh.SO_TIEN_LUOT_CHO_LAIXE, "20000"));
            models.Add(CreateCauHinhChung("Số tiền trên mỗi lượt cho phụ xe", ENKieuDuLieu.SO, ENNhaXeCauHinh.SO_TIEN_LUOT_CHO_PHUXE, "10000"));
            models.Add(CreateCauHinhChung("Tỉ lệ hưởng lương trên doanh thu của lái và phụ xe", ENKieuDuLieu.PHAN_TRAM, ENNhaXeCauHinh.TI_LE_TINH_LUONG_LAIPHUXE, "7"));
            models.Add(CreateCauHinhChung("Tiền ăn cho lái, phụ xe 1 ngày", ENKieuDuLieu.SO, ENNhaXeCauHinh.TIEN_AN_LAI_PHU_XE, "40000"));
            models.Add(CreateCauHinhChung("Tiền cầu đường", ENKieuDuLieu.SO, ENNhaXeCauHinh.TIEN_CAU_DUONG, "80000"));
            models.Add(CreateCauHinhChung("SMS cho khách hàng", ENKieuDuLieu.KY_TU, ENNhaXeCauHinh.SMS_TEMPLATE, ""));
            models.Add(CreateCauHinhChung("Tự động SMS cho khách hàng sau khi nhận hàng", ENKieuDuLieu.SO, ENNhaXeCauHinh.SMS_AUTO_SEND, "0"));
            return View(models);
        }
        [HttpPost]
        public ActionResult UpdateCauHinhChung(string Ten, int MaId, int KieuDuLieuId, string GiaTri)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return this.AccessDeniedView();

            var _cauhinh = new NhaXeCauHinh();
            _cauhinh.kieudulieu = (ENKieuDuLieu)KieuDuLieuId;
            _cauhinh.MaCauHinh = (ENNhaXeCauHinh)MaId;
            _cauhinh.GiaTri = GiaTri;
            _cauhinh.NhaXeId = _workContext.NhaXeId;
            _cauhinh.Ten = Ten;
            _nhaxeService.Insert(_cauhinh);
            return ThanhCong();
        }
    }
}