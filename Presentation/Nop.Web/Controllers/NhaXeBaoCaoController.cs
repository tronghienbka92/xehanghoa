﻿using System;
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
using System.IO;
using Nop.Services.ExportImport;
using Nop.Web.Models.NhaXeBaoCao;
using System.Data;
using System.Text.RegularExpressions;
using Nop.Services.ChuyenPhatNhanh;
using Nop.Core.Domain.ChuyenPhatNhanh;


namespace Nop.Web.Controllers
{
    public class NhaXeBaoCaoController : BaseNhaXeController
    {
        #region Khoi Tao
        private readonly IStateProvinceService _stateProvinceService;
        private readonly INhaXeService _nhaxeService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IDiaChiService _diachiService;
        private readonly INhanVienService _nhanvienService;
        private readonly IPermissionService _permissionService;
        private readonly IHanhTrinhService _hanhtrinhService;
        private readonly IVeXeService _vexeService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPhoiVeService _phoiveService;
        private readonly IPhieuGuiHangService _phieuguihangService;
        private readonly IHangHoaService _hanghoaService;
        private readonly IXeInfoService _xeinfoService;
        private readonly IBaoCaoService _baocaoService;
        private readonly IExportManager _exportManager;
        private readonly IPhieuChuyenPhatService _phieuchuyenphatService;
        private readonly IBenXeService _benxeService;
        public NhaXeBaoCaoController(IBenXeService benxeService,
            IStateProvinceService stateProvinceService,
            INhaXeService nhaxeService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICustomerService customerService,
            IDiaChiService diachiService,
            INhanVienService nhanvienService,
            IPermissionService permissionService,
            IHanhTrinhService hanhtrinhService,
             IVeXeService vexeService,
            IPriceFormatter priceFormatter,
            IPhoiVeService phoiveService,
            IPhieuGuiHangService phieuguihangService,
            IHangHoaService hanghoaService,
            IXeInfoService xeinfoService,
            IBaoCaoService baocaoService,
             IExportManager exportManager,
             IPhieuChuyenPhatService phieuchuyenphatService
            )
        {
            this._benxeService = benxeService;
            this._baocaoService = baocaoService;
            this._hanghoaService = hanghoaService;
            this._phieuguihangService = phieuguihangService;
            this._stateProvinceService = stateProvinceService;
            this._nhaxeService = nhaxeService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._customerService = customerService;
            this._diachiService = diachiService;
            this._nhanvienService = nhanvienService;
            this._permissionService = permissionService;
            this._hanhtrinhService = hanhtrinhService;
            this._vexeService = vexeService;
            this._priceFormatter = priceFormatter;
            this._phoiveService = phoiveService;
            this._xeinfoService = xeinfoService;
            this._exportManager = exportManager;
            this._phieuchuyenphatService = phieuchuyenphatService;
        }
        #endregion
        #region Common
        [NonAction]
        protected virtual void PrepareThongKeItemToModel(ThongKeItem model, ThongKeItem item)
        {
            model.Nhan = item.Nhan;
            model.NhanSapXep = item.NhanSapXep;
            model.ItemId = item.ItemId;
            model.ItemDataDate = item.ItemDataDate;
            model.GiaTri = item.GiaTri;
            model.SoLuong = item.SoLuong;
            model.GiaTri1 = item.GiaTri1;
            model.GiaTri2 = item.GiaTri2;
        }
        [NonAction]
        protected virtual void PrepareListQuy(BaoCaoNhaXeModel model)
        {
            if (DateTime.Now.Month < 4)
                model.QuyId = 1;
            else if (DateTime.Now.Month < 7)
                model.QuyId = 2;
            else if (DateTime.Now.Month < 10)
                model.QuyId = 3;
            else
                model.QuyId = 4;
            model.ListQuy = this.GetCVEnumSelectList<ENBaoCaoQuy>(_localizationService, model.QuyId);
        }

        [NonAction]
        protected virtual void PrepareListNgayThangNam(BaoCaoNhaXeModel model)
        {
            model.ThangId = DateTime.Now.Month;
            model.NamId = DateTime.Now.Year;
            for (int i = 2015; i <= DateTime.Now.Year; i++)
            {
                model.ListYear.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = (i == model.NamId) });
            }
            for (int i = 1; i <= 12; i++)
            {
                model.ListMonth.Add(new SelectListItem { Text = "Tháng " + i.ToString(), Value = i.ToString(), Selected = (i == model.ThangId) });
            }
        }

        [NonAction]
        protected virtual IList<SelectListItem> GetListLoaiThoiGian()
        {
            return this.GetCVEnumSelectList<ENBaoCaoLoaiThoiGian>(_localizationService, 0);
        }
        [NonAction]
        protected virtual IList<SelectListItem> GetListChuKyThoiGian()
        {
            return this.GetCVEnumSelectList<ENBaoCaoChuKyThoiGian>(_localizationService, 0);
        }
        [NonAction]
        protected virtual IList<SelectListItem> GetListBaoCaoKHTiemNangTheoKV()
        {
            return this.GetCVEnumSelectList<BaoCaoKhachHangTiemNangTheoKV>(_localizationService, 0);
        }
        protected virtual IList<SelectListItem> GetListBaoCaoKHTiemNangTheoNguoiGuiNhan()
        {
            return this.GetCVEnumSelectList<BaoCaoKhachHangTiemNangTheoNguoiGuiNhan>(_localizationService, 0);
        }
        protected virtual IList<SelectListItem> GetListBaoCaoKHTiemNangSapXep()
        {
            return this.GetCVEnumSelectList<BaoCaoKhachHangTiemNangSapXep>(_localizationService, 0);
        }
        protected virtual IList<SelectListItem> GetListBaoCaoDieuHanhBen()
        {
            return this.GetCVEnumSelectList<BaoCaoNhaXeModel.EN_BAO_CAO_DIEU_HANH_BEN>(_localizationService, 0);
        }
        void PrepareListVanPhongModel(BaoCaoNhaXeModel model, bool isGetTruSo = true, bool isChonVP = true)
        {
            if (this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao))
            {
                var vanphongs = _nhaxeService.GetAllVanPhongByNhaXeId(_workContext.NhaXeId);
                if (!isGetTruSo)
                    vanphongs = vanphongs.Where(c => c.KieuVanPhong == ENKieuVanPhong.VanPhong).ToList();
                model.VanPhongs = vanphongs.Select(c => new SelectListItem
                {
                    Text = c.TenVanPhong,
                    Value = c.Id.ToString(),
                }).ToList();

                if (isChonVP)
                    model.VanPhongs.Insert(0, new SelectListItem { Value = "0", Text = "----------Tất cả----------" });
            }
            else
            {
                SelectListItem item = new SelectListItem();
                item.Text = _workContext.CurrentVanPhong.TenVanPhong;
                item.Value = _workContext.CurrentVanPhong.Id.ToString();
                item.Selected = true;
                model.VanPhongs.Add(item);
            }
        }
        void PrepareListKhuVucModel(BaoCaoNhaXeModel model)
        {
            model.KhuVucs = _phieuchuyenphatService.GetAllKhuVuc(_workContext.NhaXeId).Select(c => new SelectListItem
            {
                Text = c.TenKhuVuc,
                Value = c.Id.ToString(),
            }).ToList();
        }

        void PrepareListXeModel(BaoCaoNhaXeModel model)
        {
            var xevanchuyen = _xeinfoService.GetAllXeVanChuyenByNhaXeId(_workContext.NhaXeId,"");
            model.Xe = xevanchuyen.Select(c => new SelectListItem
            {
                Text = c.BienSo,
                Value = c.Id.ToString(),
            }).ToList();
        }
        #endregion
        #region bao cao doanh thu

        public ActionResult BaoCaoDoanhThu()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.ListLoai1 = GetListChuKyThoiGian();
            PrepareListNgayThangNam(modeldoanhthu);
            return View(modeldoanhthu);
        }

        [HttpPost]
        public ActionResult BaoCaoDoanhThu(DataSourceRequest command, BaoCaoNhaXeModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();

            var items = _phoiveService.GetAllPhoiVe(model.ThangId, model.NamId, _workContext.NhaXeId, (ENBaoCaoChuKyThoiGian)model.Loai1Id);
            ENBaoCaoChuKyThoiGian loaiid = (ENBaoCaoChuKyThoiGian)model.Loai1Id;
            var doanhthus = items.Select(c =>
            {
                var _doanhthu = new BaoCaoNhaXeModel.BaoCaoDoanhThuModel();
                PrepareThongKeItemToModel(_doanhthu, c);
                switch (loaiid)
                {
                    case ENBaoCaoChuKyThoiGian.HangNgay:
                        {
                            _doanhthu.ThoiGian = string.Format("{0}/{1}/{2}", _doanhthu.Nhan, model.ThangId, model.NamId);
                            break;
                        }
                    case ENBaoCaoChuKyThoiGian.HangThang:
                        {
                            _doanhthu.ThoiGian = string.Format("{0}/{1}", _doanhthu.Nhan, model.NamId);
                            break;
                        }
                    case ENBaoCaoChuKyThoiGian.HangNam:
                        {
                            _doanhthu.ThoiGian = _doanhthu.Nhan;
                            break;
                        }
                }
                _doanhthu.TongDoanhThu = c.GiaTri;
                _doanhthu.DoanhThuChonVe = c.GiaTri1;
                _doanhthu.DoanhThuNhaXe = c.GiaTri2;
                return _doanhthu;
            }).ToList();


            var gridModel = new DataSourceResult
            {
                Data = doanhthus,
                Total = doanhthus.Count
            };

            return Json(gridModel);
        }
        public ActionResult _ChiTietDoanhThu(string thoigian)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel();
            model.NgayBan = thoigian;
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult _ChiTietDoanhThu(DataSourceRequest command, string thoigian)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            DateTime _ngaydi = Convert.ToDateTime(thoigian);
            var items = _phoiveService.GetDetailDoanhThu(_workContext.NhaXeId, _ngaydi).Select(c => new BaoCaoNhaXeModel.KhachHangMuaVeModel
            {
                CustomerId = c.CustomerId,
                NguonVeXeId = c.NguonVeXeId,
                KyHieuGhe = c.KyHieuGhe,
                isChonVe = c.isChonVe,
                GiaVe = c.GiaVe,
                NgayDi = c.NgayDi,
                SoDienThoai = c.SoDienThoai,
                TenKhachHang = c.TenKhachHang,
                ThongTinChuyenDi = c.ThongTinChuyenDi,

            }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = items,
                Total = items.Count
            };

            return Json(gridModel);
        }


        public ActionResult ThongKeDoanhThu()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.ListLoai1 = GetListChuKyThoiGian();
            PrepareListNgayThangNam(modeldoanhthu);
            return View(modeldoanhthu);
        }
        [HttpPost]
        public ActionResult BieuDoDoanhThu(DataSourceRequest command, BaoCaoNhaXeModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var items = _phoiveService.GetAllPhoiVe(model.ThangId, model.NamId, _workContext.NhaXeId, (ENBaoCaoChuKyThoiGian)model.Loai1Id);
            return Json(items);
        }

        #endregion
        #region Doanh thu theo xe
        public ActionResult DoanhThuBanVeTheoXe()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel();
            model.TuNgay = DateTime.Now.AddMonths(-1);
            model.DenNgay = DateTime.Now;
            PrepareListXeModel(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult DoanhThuBanVeTheoXe(DataSourceRequest command, DateTime tuNgay, DateTime denNgay, int XeId)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var items = _phoiveService.GetDoanhThuBanVeTungXeTheoNgay(tuNgay, denNgay, _workContext.NhaXeId, XeId);
            var doanhthus = items.Select(c =>
            {
                var model = new BaoCaoNhaXeModel.BaoCaoDoanhThuXeTungNgayModel();
                model.NgayBan = c.Nhan;
                model.TongDoanhThu = c.GiaTri;
                model.ThongTinChuyenDi = c.ThongTinChuyenDi;
                model.NgayBan = c.ItemDataDate.ToString("yyyy-MM-dd");
                model.SoLuong = c.SoLuong;
                return model;
            }).ToList();
            var gridModel = new DataSourceResult
            {
                Data = doanhthus,
                Total = doanhthus.Count
            };
            return Json(gridModel);
        }

        public ActionResult _ChiTietDoanhThuTheoXe(int XeId, string NgayBan)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel.BaoCaoDoanhThuXeTungNgayModel();
            model.XeId = XeId;
            model.NgayBan = NgayBan;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _ChiTietDoanhThuTheoXe(DataSourceRequest command, int XeId, string NgayBan)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            DateTime _NgayBan = Convert.ToDateTime(NgayBan);
            var items = _phoiveService.GetDetailDoanhThuBanVeTungXeTheoNgay(_NgayBan, _workContext.NhaXeId, XeId).Select(c =>
            {

                var model = new BaoCaoNhaXeModel.KhachHangMuaVeModel();
                model.CustomerId = c.CustomerId;
                model.NguonVeXeId = c.NguonVeXeId;
                model.KyHieuGhe = c.KyHieuGhe;
                model.isChonVe = c.isChonVe;
                model.GiaVe = c.GiaVe;
                model.TrangThaiPhoiVeId = c.TrangThaiPhoiVeId;
                if (c.TrangThai == ENTrangThaiPhoiVe.ChoXuLy)
                    model.TrangThaiPhoiVeText = "Chưa thanh toán";
                if (c.TrangThai == ENTrangThaiPhoiVe.DaGiaoHang)
                    model.TrangThaiPhoiVeText = "Đã thanh toán";
                model.NgayDi = c.NgayDi;
                model.SoDienThoai = c.SoDienThoai;
                model.TenKhachHang = c.TenKhachHang;
                model.ThongTinChuyenDi = c.ThongTinChuyenDi;
                return model;

            }).ToList();
            var gridModel = new DataSourceResult
            {
                Data = items,
                Total = items.Count
            };

            return Json(gridModel);
        }
        #endregion
        #region doanh thu theo nhan vien
        public ActionResult DoanhThuNhanvien()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.TuNgay = DateTime.Now.AddMonths(-1);
            modeldoanhthu.DenNgay = DateTime.Now;
            PrepareListVanPhongModel(modeldoanhthu);
            return View(modeldoanhthu);
        }
        [HttpPost]
        public ActionResult DoanhThuBanVeTheoNgay(DataSourceRequest command, DateTime tuNgay, DateTime denNgay, int VanPhongId)
        {

            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            var items = _phoiveService.GetDoanhThuBanVeTheoNgay(tuNgay, denNgay, _workContext.NhaXeId, VanPhongId);
            var doanhthus = items.Select(c =>
            {
                var _doanhthu = new BaoCaoNhaXeModel.BaoCaoDoanhThuNhanVienModel();
                PrepareThongKeItemToModel(_doanhthu, c);
                _doanhthu.NgayBan = c.Nhan;
                _doanhthu.TongDoanhThu = c.GiaTri;
                return _doanhthu;
            }).ToList();
            var gridModel = new DataSourceResult
            {
                Data = doanhthus,
                Total = doanhthus.Count
            };
            return Json(gridModel);
        }
        [HttpPost]
        public ActionResult DoanhThuBanVeTheoNhanVien(DataSourceRequest command, int VanPhongId, string NgayBan)
        {

            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            DateTime ngayban = Convert.ToDateTime(NgayBan);
            var items = _phoiveService.GetDoanhThuBanVeTheoNhanVien(_workContext.NhaXeId, VanPhongId, ngayban);
            var doanhthus = items.Select(c =>
            {
                var _doanhthu = new BaoCaoNhaXeModel.BaoCaoDoanhThuNhanVienModel();
                PrepareThongKeItemToModel(_doanhthu, c);
                _doanhthu.NhanVienId = c.ItemId;
                _doanhthu.NgayBan = NgayBan;
                _doanhthu.TenNhanVien = _nhanvienService.GetById(_doanhthu.NhanVienId).HoVaTen;
                _doanhthu.TongDoanhThu = c.GiaTri;
                return _doanhthu;
            }).ToList();


            var gridModel = new DataSourceResult
            {
                Data = doanhthus,
                Total = doanhthus.Count
            };

            return Json(gridModel);
        }
        [HttpPost]
        public ActionResult DoanhThuBanVeTheoTrangThai(DataSourceRequest command, int VanPhongId, string NgayBan, int NhanvienId)
        {

            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            DateTime ngayban = Convert.ToDateTime(NgayBan);
            var items = _phoiveService.GetDoanhThuBanVeTheoTrangThai(_workContext.NhaXeId, VanPhongId, ngayban, NhanvienId);
            var doanhthus = items.Select(c =>
            {
                var _doanhthu = new BaoCaoNhaXeModel.BaoCaoDoanhThuNhanVienModel();
                PrepareThongKeItemToModel(_doanhthu, c);
                _doanhthu.NhanVienId = NhanvienId;
                _doanhthu.NgayBan = NgayBan;
                _doanhthu.TrangThaiPhoiVeId = c.TrangThaiPhoiVeId;
                if (_doanhthu.TrangThai == ENTrangThaiPhoiVe.DaGiaoHang)
                    _doanhthu.TrangThaiPhoiVeText = "Đã thanh toán";
                if (_doanhthu.TrangThai == ENTrangThaiPhoiVe.ChoXuLy)
                    _doanhthu.TrangThaiPhoiVeText = "Chưa thanh toán";
                _doanhthu.TenNhanVien = _nhanvienService.GetById(_doanhthu.NhanVienId).HoVaTen;
                _doanhthu.TongDoanhThu = c.GiaTri;
                return _doanhthu;
            }).ToList();


            var gridModel = new DataSourceResult
            {
                Data = doanhthus,
                Total = doanhthus.Count
            };

            return Json(gridModel);
        }
        public ActionResult _ChiTietDoanhThuNhanVien(int NhanVienId, string NgayBan)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel.BaoCaoDoanhThuNhanVienModel();
            model.NhanVienId = NhanVienId;
            model.NgayBan = NgayBan;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _ChiTietDoanhThuNhanVien(DataSourceRequest command, int NhanVienId, string NgayBan)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            DateTime _NgayBan = Convert.ToDateTime(NgayBan);
            var items = _phoiveService.GetDetailDoanhThu(_workContext.NhaXeId, _NgayBan, NhanVienId).Select(c =>
            {
                var model = new BaoCaoNhaXeModel.KhachHangMuaVeModel();
                model.CustomerId = c.CustomerId;
                model.NguonVeXeId = c.NguonVeXeId;
                model.KyHieuGhe = c.KyHieuGhe;
                model.isChonVe = c.isChonVe;
                model.GiaVe = c.GiaVe;
                model.TrangThaiPhoiVeId = c.TrangThaiPhoiVeId;
                if (c.TrangThai == ENTrangThaiPhoiVe.ChoXuLy)
                    model.TrangThaiPhoiVeText = "Chưa thanh toán";
                if (c.TrangThai == ENTrangThaiPhoiVe.DaGiaoHang)
                    model.TrangThaiPhoiVeText = "Đã thanh toán";
                model.NgayDi = c.NgayDi;
                model.SoDienThoai = c.SoDienThoai;
                model.TenKhachHang = c.TenKhachHang;
                model.ThongTinChuyenDi = c.ThongTinChuyenDi;
                return model;

            }).ToList();
            var gridModel = new DataSourceResult
            {
                Data = items,
                Total = items.Count
            };

            return Json(gridModel);
        }
        #endregion
        #region thong ke theo hanh trinh
        public ActionResult DoanhThuTuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();

            modeldoanhthu.ListLoai2 = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                var item = new SelectListItem();
                item.Text = string.Format("{0} ({1})", c.MoTa, c.MaHanhTrinh);
                item.Value = c.Id.ToString();
                return item;
            }).ToList();
            var _item = new SelectListItem();
            _item.Text = "Chọn hành trình";
            _item.Value = "0";
            modeldoanhthu.ListLoai2.Insert(0, _item);
            modeldoanhthu.ListLoai1 = GetListLoaiThoiGian();
            PrepareListNgayThangNam(modeldoanhthu);
            PrepareListQuy(modeldoanhthu);
            return View(modeldoanhthu);
        }
        public ActionResult BieuDoDoanhThuTuyen(DataSourceRequest command, BaoCaoNhaXeModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var ListItem = new List<ThongKeItem>();
            if (model.Loai2Id == 0)
            {
                ListItem = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId)
                .Select(c =>
                {
                    var _doanhthu = new ThongKeItem();
                    _doanhthu.Nhan = c.MoTa;
                    var lichtrinhs = _hanhtrinhService.GetAllLichTrinhByHanhTrinhId(c.Id, _workContext.NhaXeId).ToList();
                    int _sl;
                    _doanhthu.GiaTri = _phoiveService.DoanhThuTuyen(lichtrinhs.Select(lt => lt.Id).ToList(), model.ThangId, model.NamId, (ENBaoCaoQuy)model.QuyId, (ENBaoCaoLoaiThoiGian)model.Loai1Id, out _sl);
                    _doanhthu.SoLuong = _sl;
                    return _doanhthu;
                }).ToList();
            }
            else
            {

                var _DiemDons = _hanhtrinhService.GetAllHanhTrinhDiemDonByHanhTrinhId(model.Loai2Id).Where(c => c.ThuTu > 0).OrderBy(c => c.ThuTu).Select(c => c.diemdon).ToList();

                foreach (var item in _DiemDons)
                {
                    var _doanhthu = new ThongKeItem();
                    _doanhthu.Nhan = item.TenDiemDon;
                    var _nguonveids = _hanhtrinhService.GetAllNguonVeXe(0, 0, model.Loai2Id, 0, item.Id).Select(c => c.Id).ToList();
                    int _sl;
                    _doanhthu.GiaTri = _phoiveService.DoanhThuTuyenCon(_nguonveids, model.ThangId, model.NamId, (ENBaoCaoQuy)model.QuyId, (ENBaoCaoLoaiThoiGian)model.Loai1Id, out _sl);
                    _doanhthu.SoLuong = _sl;
                    ListItem.Add(_doanhthu);
                }
            }
            return Json(ListItem.ToList());
        }
        public ActionResult DoanhThuLichTrinh()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.ListLoai1 = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                var item = new SelectListItem();
                item.Text = string.Format("{0} ({1})", c.MoTa, c.MaHanhTrinh);
                item.Value = c.Id.ToString();
                return item;
            }).ToList();
            modeldoanhthu.ListLoai2 = GetListLoaiThoiGian();
            PrepareListNgayThangNam(modeldoanhthu);
            PrepareListQuy(modeldoanhthu);

            return View(modeldoanhthu);
        }
        public ActionResult BieuDoDoanhThuLichTrinh(DataSourceRequest command, BaoCaoNhaXeModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var lichtrinhs = _hanhtrinhService.GetAllLichTrinhByHanhTrinhId(model.Loai1Id, _workContext.NhaXeId).Select(c =>
                {
                    var _doanhthu = new ThongKeItem();
                    _doanhthu.Nhan = string.Format("{0}-{1}", c.ThoiGianDi.ToShortTimeString(), c.ThoiGianDen.ToShortTimeString());
                    int _sl;
                    _doanhthu.GiaTri = _phoiveService.DoanhThuLichTrinh(c.Id, model.ThangId, model.NamId, (ENBaoCaoQuy)model.QuyId, (ENBaoCaoLoaiThoiGian)model.Loai2Id, out _sl);
                    _doanhthu.SoLuong = _sl;
                    return _doanhthu;
                }).ToList();


            return Json(lichtrinhs);


        }
        public ActionResult DoanhThuVanPhong()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.ListLoai1 = GetListLoaiThoiGian();
            PrepareListNgayThangNam(modeldoanhthu);
            PrepareListQuy(modeldoanhthu);

            return View(modeldoanhthu);
        }
        public ActionResult BieuDoDoanhThuVanPhong(DataSourceRequest command, BaoCaoNhaXeModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var vanphongs = _nhaxeService.GetAllVanPhongByNhaXeId(_workContext.NhaXeId).Select(c =>
                {
                    var _doanhthu = new ThongKeItem();
                    _doanhthu.Nhan = c.TenVanPhong;
                    int _sl;
                    var listnhavien = _nhanvienService.GetAllByVanPhongId(c.Id);
                    _doanhthu.GiaTri = _phoiveService.DoanhThuVanPhong(listnhavien.Select(nv => nv.Id).ToList(), model.ThangId, model.NamId, (ENBaoCaoQuy)model.QuyId, (ENBaoCaoLoaiThoiGian)model.Loai1Id, out _sl);
                    _doanhthu.SoLuong = _sl;
                    return _doanhthu;
                }).ToList();

            return Json(vanphongs);
        }
        #endregion
        #region Báo cáo doanh thu theo tuyến
        public ActionResult BaoCaoDoanhThuTuyen()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel();
            model.TuNgay = DateTime.Now.AddMonths(-1);
            model.DenNgay = DateTime.Now;
            model.ListLoai2 = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                var item = new SelectListItem();
                item.Text = string.Format("{0} ({1})", c.MoTa, c.MaHanhTrinh);
                item.Value = c.Id.ToString();
                return item;
            }).ToList();
            var _item = new SelectListItem();
            _item.Text = "Chọn hành trình";
            _item.Value = "0";
            model.ListLoai2.Insert(0, _item);
            return View(model);
        }
        [HttpPost]
        public ActionResult BaoCaoDoanhThuTuyen(DataSourceRequest command, DateTime tuNgay, DateTime denNgay, int HanhTrinhId)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var lichtrinhs = _hanhtrinhService.GetAllLichTrinhByHanhTrinhId(HanhTrinhId, _workContext.NhaXeId).ToList();
            var items = _phoiveService.GetDoanhThuTheoTuyen(tuNgay, denNgay, lichtrinhs.Select(lt => lt.Id).ToList());

            var doanhthus = items.Select(c =>
            {
                var model = new BaoCaoNhaXeModel.BaoCaoDoanhThuXeTungNgayModel();
                model.NgayBan = c.Nhan;
                model.TongDoanhThu = c.GiaTri;
                model.NgayBan = c.ItemDataDate.ToString("yyyy-MM-dd");
                model.SoLuong = c.SoLuong;
                return model;
            }).ToList();
            var gridModel = new DataSourceResult
            {
                Data = doanhthus,
                Total = doanhthus.Count
            };
            return Json(gridModel);
        }
        public ActionResult _ChiTietDoanhThuTheoChang(int NguonVeId, string NgayBan)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel.BaoCaoDoanhThuNhanVienModel();
            model.NguonVeId = NguonVeId;
            model.NgayBan = NgayBan;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _ChiTietDoanhThuTheoChang(DataSourceRequest command, int NhanVienId, string NgayBan)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            DateTime _NgayBan = Convert.ToDateTime(NgayBan);
            var items = _phoiveService.GetDetailDoanhThu(_workContext.NhaXeId, _NgayBan, NhanVienId).Select(c =>
            {
                var model = new BaoCaoNhaXeModel.KhachHangMuaVeModel();
                model.CustomerId = c.CustomerId;
                model.NguonVeXeId = c.NguonVeXeId;
                model.KyHieuGhe = c.KyHieuGhe;
                model.isChonVe = c.isChonVe;
                model.GiaVe = c.GiaVe;
                model.TrangThaiPhoiVeId = c.TrangThaiPhoiVeId;
                if (c.TrangThai == ENTrangThaiPhoiVe.ChoXuLy)
                    model.TrangThaiPhoiVeText = "Chưa thanh toán";
                if (c.TrangThai == ENTrangThaiPhoiVe.DaGiaoHang)
                    model.TrangThaiPhoiVeText = "Đã thanh toán";
                model.NgayDi = c.NgayDi;
                model.SoDienThoai = c.SoDienThoai;
                model.TenKhachHang = c.TenKhachHang;
                model.ThongTinChuyenDi = c.ThongTinChuyenDi;
                return model;

            }).ToList();
            var gridModel = new DataSourceResult
            {
                Data = items,
                Total = items.Count
            };

            return Json(gridModel);
        }

        #endregion
        #region Bao cao ky gui hang hoa
        public ActionResult DoanhThuKyGuiHangNgay()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.TuNgay = DateTime.Now.AddMonths(-1);
            modeldoanhthu.DenNgay = DateTime.Now;
            PrepareListVanPhongModel(modeldoanhthu);
            return View(modeldoanhthu);
        }
        [HttpPost]
        public ActionResult DoanhThuKyGuiHangNgay(DataSourceRequest command, DateTime tuNgay, DateTime denNgay, int VanPhongId)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();

            var items = _phieuguihangService.GetDoanhThuNhanvien(tuNgay, denNgay, _workContext.NhaXeId, VanPhongId);
            var doanhthus = items.Select(c =>
            {
                var _doanhthu = new BaoCaoNhaXeModel.BaoCaoDoanhThuNhanVienModel();
                PrepareThongKeItemToModel(_doanhthu, c);
                _doanhthu.NhanVienId = c.ItemId;
                _doanhthu.ItemDataDate = Convert.ToDateTime(c.ItemDataDay + "-" + c.ItemDataMonth + "-" + c.ItemDataYear);
                _doanhthu.NgayBan = _doanhthu.ItemDataDate.ToString("yyyy-MM-dd");

                _doanhthu.TenNhanVien = _nhanvienService.GetById(_doanhthu.NhanVienId).HoVaTen;
                _doanhthu.TongDoanhThu = c.GiaTri;
                return _doanhthu;
            }).ToList();


            var gridModel = new DataSourceResult
            {
                Data = doanhthus,
                Total = doanhthus.Count
            };

            return Json(gridModel);
        }
        public ActionResult _ChiTietDoanhThuKyGui(int NhanVienId, string NgayThu, string NotPay)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel.BaoCaoDetailDoanhThuKiGuiModel();
            model.NhanVienId = NhanVienId;
            model.NgayBan = NgayThu;
            model.NotPay = NotPay;
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult _ChiTietDoanhThuKyGui(DataSourceRequest command, int NhanVienId, string NgayThu, string NotPay)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            DateTime _NgayBan = Convert.ToDateTime(NgayThu);
            var items = new List<PhieuGuiHang>();
            if (NotPay == "null")
            {
                items = _phieuguihangService.GetAllByNhanVien(_workContext.NhaXeId, NhanVienId, _NgayBan);
            }

            else
            {
                items = _phieuguihangService.GetDetailDoanhThuKiGuiNotPay(_workContext.NhaXeId, NhanVienId, _NgayBan);
            }

            var gridModel = new DataSourceResult
            {
                Data = items.Select(x =>
                {
                    var hanghoas = _hanghoaService.GetAllHangHoaByPhieuGuiHangId(x.Id);
                    var m = x.ToModel(_localizationService, _priceFormatter, hanghoas);
                    return m;
                }),
                Total = items.Count
            };
            return Json(gridModel);
        }
        public ActionResult DoanhThuKyGuiHangNgayNotPay()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.TuNgay = DateTime.Now.AddMonths(-1);
            modeldoanhthu.DenNgay = DateTime.Now;
            PrepareListVanPhongModel(modeldoanhthu);
            return View(modeldoanhthu);
        }

        [HttpPost]
        public ActionResult _DoanhThuKyGuiHangNgayNotPay(DataSourceRequest command, DateTime tuNgay, DateTime denNgay, int VanPhongId)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();

            var items = _phieuguihangService.GetDoanhThuKiGuiNotPay(tuNgay, denNgay, _workContext.NhaXeId, VanPhongId);
            var doanhthus = items.Select(c =>
            {
                var _doanhthu = new BaoCaoNhaXeModel.BaoCaoDoanhThuNhanVienModel();
                PrepareThongKeItemToModel(_doanhthu, c);
                _doanhthu.NhanVienId = c.ItemId;
                _doanhthu.ItemDataDate = Convert.ToDateTime(c.ItemDataDay + "-" + c.ItemDataMonth + "-" + c.ItemDataYear);
                _doanhthu.NgayBan = _doanhthu.ItemDataDate.ToString("yyyy-MM-dd");
                _doanhthu.TenNhanVien = _nhanvienService.GetById(_doanhthu.NhanVienId).HoVaTen;
                _doanhthu.TongDoanhThu = c.GiaTri;
                return _doanhthu;
            }).ToList();


            var gridModel = new DataSourceResult
            {
                Data = doanhthus,
                Total = doanhthus.Count
            };

            return Json(gridModel);
        }

        // thống kê theo doanh thu
        public ActionResult HangHoaTheoDoanhThu()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.ListLoai1 = GetListChuKyThoiGian();
            PrepareListNgayThangNam(modeldoanhthu);
            return View(modeldoanhthu);
        }
        [HttpPost]
        public ActionResult HangHoaTheoDoanhThu(DataSourceRequest command, BaoCaoNhaXeModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            //_phieuguihangService.InsertPhieuGuiHang(phieugui);
            var items = _phieuguihangService.GetAllPhieuGuiHangByCuoc(model.ThangId, model.NamId, _workContext.NhaXeId, (ENBaoCaoChuKyThoiGian)model.Loai1Id);
            return Json(items);
        }
        public ActionResult HangHoaTheoVanPhong()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.ListLoai1 = GetListLoaiThoiGian();
            PrepareListNgayThangNam(modeldoanhthu);
            PrepareListQuy(modeldoanhthu);

            return View(modeldoanhthu);
        }
        [HttpPost]
        public ActionResult HangHoaTheoVanPhong(DataSourceRequest command, BaoCaoNhaXeModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var vanphongs = _nhaxeService.GetAllVanPhongByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                var _doanhthu = new ThongKeItem();
                _doanhthu.Nhan = c.TenVanPhong;
                int _sl;
                var listnhavien = _nhanvienService.GetAllByVanPhongId(c.Id);
                _doanhthu.GiaTri = _phieuguihangService.HangHoaDoanhThuVanPhong(listnhavien.Select(nv => nv.Id).ToList(), model.ThangId, model.NamId, (ENBaoCaoQuy)model.QuyId, (ENBaoCaoLoaiThoiGian)model.Loai1Id, out _sl);
                _doanhthu.SoLuong = _sl;
                return _doanhthu;
            }).ToList();

            return Json(vanphongs);
        }
        #endregion

        #region Bao cao theo lai xe, phu xe, doanh thu, chuyen di
        public ActionResult BaoCaoLaiXePhuXe()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var model = new BaoCaoLaiXePhuXeListModel();
            model.TuNgay = DateTime.Now.NgayDauThang();
            model.DenNgay = DateTime.Now.NgayCuoiThang();
            return View(model);
        }


        #endregion
        #region doanh thu ve quay
        public ActionResult DoanhThuVeQuayTheoHanhTrinh()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel();
            model.TuNgay = DateTime.Now.AddDays(-7);
            model.DenNgay = DateTime.Now.AddDays(1);
            model.ListLoai2 = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                var item = new SelectListItem();
                item.Text = string.Format("{0} ({1})", c.MoTa, c.MaHanhTrinh);
                item.Value = c.Id.ToString();
                return item;
            }).ToList();
            var _item = new SelectListItem();
            _item.Text = "Chọn hành trình";
            _item.Value = "0";
            model.ListLoai2.Insert(0, _item);
            return View(model);
        }
        [HttpPost]
        public ActionResult DoanhThuVeQuayTheoHanhTrinh(DataSourceRequest command, DateTime tuNgay, DateTime denNgay, int HanhTrinhId)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            var items = _phoiveService.ThongKeVeQuayDaBanTheoTuyen(tuNgay, denNgay, HanhTrinhId);

            var gridModel = new DataSourceResult
            {
                Data = items,
                Total = items.Count
            };
            return Json(gridModel);
        }
        public ActionResult ExportExcelHanhTrinhVeQuay(DateTime TuNgay, DateTime DenNgay, int HanhTrinhId)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            try
            {



                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    var tenhanhtrinh = _hanhtrinhService.GetHanhTrinhById(HanhTrinhId).MoTa;

                    var items = _phoiveService.ThongKeVeQuayDaBanTheoTuyen(TuNgay, DenNgay, HanhTrinhId);
                    _exportManager.ExportHanhTrinhVeQuay(stream, items, TuNgay, DenNgay, tenhanhtrinh);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "ThongKeVeQuayHanhTrinh" + TuNgay.ToString("ddMMyyyy") + "_" + DenNgay.ToString("ddMMyyyy") + ".xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }
        public ActionResult _ChiTietDoanhThuHanhTrinhVeQuay(int HanhTrinhId, DateTime NgayBan, int SoQuyen)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var items = _phoiveService.GetDetailDoanhThuVeQuay(HanhTrinhId, NgayBan, SoQuyen);
            return PartialView(items);
        }
        #endregion
        #region Khach hang
        public ActionResult BaoCaoKhachHang()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel();
            model.TuNgay = DateTime.Now.AddMonths(-1);
            model.DenNgay = DateTime.Now;
            model.ListLoai2 = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                var item = new SelectListItem();
                item.Text = string.Format("{0} ({1})", c.MoTa, c.MaHanhTrinh);
                item.Value = c.Id.ToString();
                return item;
            }).ToList();
            var _item = new SelectListItem();
            _item.Text = "Chọn hành trình";
            _item.Value = "0";
            model.ListLoai2.Insert(0, _item);
            return View(model);
        }
        [HttpPost]
        public ActionResult BaoCaoKhachHang(DataSourceRequest command, DateTime tuNgay, DateTime denNgay, int HanhTrinhId)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var lichtrinhs = _hanhtrinhService.GetAllLichTrinhByHanhTrinhId(HanhTrinhId, _workContext.NhaXeId).ToList();
            var items = _phoiveService.GetKhachHangMuaVeTheoTuyen(_workContext.NhaXeId, tuNgay, denNgay, lichtrinhs.Select(lt => lt.Id).ToList());
            var khachhangs = items.Select(c => new BaoCaoNhaXeModel.KhachHangMuaVeModel
            {
                CustomerId = c.CustomerId,
                TenKhachHang = c.TenKhachHang,
                SoDienThoai = c.SoDienThoai,
                ThongTinChuyenDi = c.ThongTinChuyenDi,
                SoLuot = c.SoLuot
            }).ToList();
            var gridModel = new DataSourceResult
            {
                Data = khachhangs,
                Total = khachhangs.Count
            };
            return Json(gridModel);
        }
        public ActionResult _ChiTietKhachHangLuotDi(int KhachHangId, DateTime tuNgay, DateTime denNgay, int HanhTrinhId)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var lichtrinhs = _hanhtrinhService.GetAllLichTrinhByHanhTrinhId(HanhTrinhId, _workContext.NhaXeId).ToList();
            var items = _phoiveService.GetKhachHangMuaVeTheoTuyen(_workContext.NhaXeId, tuNgay, denNgay, lichtrinhs.Select(lt => lt.Id).ToList(), KhachHangId);
            return PartialView(items);
        }
        public ActionResult ExportExcelBaoCaoKhachHang(DateTime TuNgay, DateTime DenNgay, int HanhTrinhId)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            try
            {



                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    var lichtrinhs = _hanhtrinhService.GetAllLichTrinhByHanhTrinhId(HanhTrinhId, _workContext.NhaXeId).ToList();
                    var items = _phoiveService.GetKhachHangMuaVeTheoTuyen(_workContext.NhaXeId, TuNgay, DenNgay, lichtrinhs.Select(lt => lt.Id).ToList());
                    _exportManager.ExportBaoCaoKhachHang(stream, items, TuNgay, DenNgay);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "BaoCaoKhachHang" + TuNgay.ToString("ddMMyyyy") + "_" + DenNgay.ToString("ddMMyyyy") + ".xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }
        #endregion
        #region doanh thu tuyen
        public ActionResult VTDoanhThuNhanVienLuot()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new DoanhThuNhanVienLuotModel();
            modeldoanhthu.ListLoai1 = GetListLoaiThoiGian();
            PrepareListNgayThangNamToLuot(modeldoanhthu);
            //PrepareListQuyToLuot(modeldoanhthu);

            return View(modeldoanhthu);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult _GetDoanhThuNhanVienLuot(int ThangId, int NamId, int Loai1Id, string SearchName)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var model = new DoanhThuNhanVienLuotModel();
            model.ThangId = ThangId;
            model.NamId = NamId;
            model.Loai1Id = Loai1Id;
            DoanhThuNhanvienLuotPrepare(model, SearchName);
            return PartialView(model);
        }
        [NonAction]
        protected virtual void PrepareListNgayThangNamToLuot(DoanhThuNhanVienLuotModel model)
        {
            model.ThangId = DateTime.Now.Month;
            model.NamId = DateTime.Now.Year;
            for (int i = 2015; i <= DateTime.Now.Year; i++)
            {
                model.ListYear.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = (i == model.NamId) });
            }
            for (int i = 1; i <= 12; i++)
            {
                model.ListMonth.Add(new SelectListItem { Text = "Tháng " + i.ToString(), Value = i.ToString(), Selected = (i == model.ThangId) });
            }
        }
        [NonAction]
        protected virtual void DoanhThuNhanvienLuotPrepare(DoanhThuNhanVienLuotModel model, string SearchName)
        {

            int SoNhan = 0;
            DateItem[] arrNhan = new DateItem[] { };
            var arrHanhTrinh = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId);
            var arrHanhTrinhId = arrHanhTrinh.Select(c => c.Id).ToArray();

            model.SoHanhTrinh = arrHanhTrinh.Count();


            switch (model.Loai1Id)
            {
                case (int)ENBaoCaoChuKyThoiGian.HangNgay:
                    {
                        SoNhan = DateTime.DaysInMonth(model.NamId, model.ThangId);
                        arrNhan = Enumerable.Range(1, SoNhan).Select(c =>
                        {
                            var item = new DateItem();
                            item.nhan = c;
                            item.day = c;
                            item.month = model.ThangId;
                            item.year = model.NamId;
                            return item;
                        }).ToArray();
                        break;
                    }
                case (int)ENBaoCaoChuKyThoiGian.HangThang:
                    {
                        SoNhan = 12;
                        arrNhan = Enumerable.Range(1, SoNhan).Select(c =>
                        {
                            var item = new DateItem();
                            item.day = 0;
                            item.month = c;
                            item.nhan = c;
                            item.year = model.NamId;
                            return item;
                        }).ToArray();
                        model.ThangId = 0;
                        break;
                    }
                case (int)ENBaoCaoChuKyThoiGian.HangNam:
                    {
                        //lay trong vong 5 nam
                        SoNhan = 5;
                        arrNhan = Enumerable.Range(DateTime.Now.Year - SoNhan + 1, SoNhan).Select(c =>
                        {
                            var item = new DateItem();
                            item.day = 0;
                            item.month = 0;
                            item.nhan = c;
                            item.year = c;
                            return item;
                        }).ToArray();
                        model.ThangId = 0;
                        model.NamId = 0;
                        break;
                    }
            }

            //lay thong tin du lieu 
            var _dataview = _nhaxeService.GetThongKeTheoTuyenNgay(model.ThangId, model.NamId, arrHanhTrinhId);

            model.SoNhan = SoNhan;
            model.DoanhThuHangTrinh = new HanhTrinhDateItem[model.SoHanhTrinh + 1, SoNhan + 1];
            for (int i = 0; i < model.SoHanhTrinh + 1; i++)
            {
                for (int j = 0; j < SoNhan + 1; j++)
                {
                    if (i == 0 && j > 0)
                    {
                        model.DoanhThuHangTrinh[0, j] = new HanhTrinhDateItem();
                        model.DoanhThuHangTrinh[0, j].Nhan = arrNhan[j - 1].nhan;

                    }
                    if (j == 0 && i > 0)
                    {
                        model.DoanhThuHangTrinh[i, 0] = new HanhTrinhDateItem();
                        model.DoanhThuHangTrinh[i, 0].HanhTrinhId = arrHanhTrinh[i - 1].Id;
                        model.DoanhThuHangTrinh[i, 0].TenHanhTrinh = arrHanhTrinh[i - 1].MoTa;



                    }

                    if (i > 0 && j > 0)
                    {
                        model.DoanhThuHangTrinh[i, j] = new HanhTrinhDateItem();
                        model.DoanhThuHangTrinh[i, j].Nhan = arrNhan[j - 1].nhan;
                        model.DoanhThuHangTrinh[i, j].HanhTrinhId = arrHanhTrinh[i - 1].Id;
                        model.DoanhThuHangTrinh[i, j].SoChuyen = CountXeXuatBen(_dataview, arrHanhTrinh[i - 1].Id, arrNhan[j - 1].day, arrNhan[j - 1].month, arrNhan[j - 1].year);
                        model.DoanhThuHangTrinh[i, j].SoKhach = SoKhachXuatBen(_dataview, arrHanhTrinh[i - 1].Id, arrNhan[j - 1].day, arrNhan[j - 1].month, arrNhan[j - 1].year);
                        model.DoanhThuHangTrinh[i, j].DoanhThu = DoanhthuXuatBen(_dataview, arrHanhTrinh[i - 1].Id, arrNhan[j - 1].day, arrNhan[j - 1].month, arrNhan[j - 1].year);

                    }

                }

            }

        }
        [NonAction]
        protected virtual int CountXeXuatBen(List<ThongKeDoanhThuTuyenItem> dataview, int HanhTrinhId, int Ngay, int Thang, int Nam)
        {
            return dataview.Where(c => c.HanhTrinhId == HanhTrinhId && c.Ngay == Ngay && c.Thang == Thang && c.Nam == Nam).Count();
        }
        protected virtual decimal DoanhthuXuatBen(List<ThongKeDoanhThuTuyenItem> dataview, int HanhTrinhId, int Ngay, int Thang, int Nam)
        {
            return dataview.Where(c => c.HanhTrinhId == HanhTrinhId && c.Ngay == Ngay && c.Thang == Thang && c.Nam == Nam).Sum(c => c.DoanhThu);

        }
        protected virtual int SoKhachXuatBen(List<ThongKeDoanhThuTuyenItem> dataview, int HanhTrinhId, int Ngay, int Thang, int Nam)
        {
            return dataview.Where(c => c.HanhTrinhId == HanhTrinhId && c.Ngay == Ngay && c.Thang == Thang && c.Nam == Nam).Sum(c => c.SoKhach);

        }
        public ActionResult ExportExcelDoanhThuTuyen(int ThangId, int NamId, int Loai1Id, string SearchName)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            try
            {



                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    var model = new DoanhThuNhanVienLuotModel();
                    model.ThangId = ThangId;
                    model.NamId = NamId;
                    model.Loai1Id = Loai1Id;
                    DoanhThuNhanvienLuotPrepare(model, SearchName);
                    var items = model.DoanhThuHangTrinh;
                    _exportManager.ExportBaoCaoDoanhThu(stream, items, ThangId, NamId, model.SoHanhTrinh, model.SoNhan);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "BaoCaoDoanhThu" + ThangId + "_" + NamId + ".xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }
        #endregion
        #region doanh thu luot, cham cong cho nhan vien
        [NonAction]
        protected virtual void PrepareListQuyToLuot(DoanhThuNhanVienLuotModel model)
        {
            if (DateTime.Now.Month < 4)
                model.QuyId = 1;
            else if (DateTime.Now.Month < 7)
                model.QuyId = 2;
            else if (DateTime.Now.Month < 10)
                model.QuyId = 3;
            else
                model.QuyId = 4;
            model.ListQuy = this.GetCVEnumSelectList<ENBaoCaoQuy>(_localizationService, model.QuyId);
        }
        public ActionResult TongHopChamCongNhanVien()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var modeldoanhthu = new DoanhThuNhanVienLuotModel();
            modeldoanhthu.ListLoai1 = GetListLoaiThoiGian();
            PrepareListNgayThangNamToLuot(modeldoanhthu);
            PrepareListQuyToLuot(modeldoanhthu);
            modeldoanhthu.ListLoai2 = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                var item = new SelectListItem();
                item.Text = string.Format("{0} ({1})", c.MoTa, c.MaHanhTrinh);
                item.Value = c.Id.ToString();
                return item;
            }).ToList();
            var _item = new SelectListItem();
            _item.Text = "Chọn hành trình";
            _item.Value = "0";
            modeldoanhthu.ListLoai2.Insert(0, _item);
            return View(modeldoanhthu);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult _GetChamCongNhanVien(int ThangId, int NamId, int Loai1Id, int Loai2Id, string SearchName)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var model = new DoanhThuNhanVienLuotModel();
            model.ThangId = ThangId;
            model.NamId = NamId;
            model.Loai1Id = Loai1Id;
            model.Loai2Id = Loai2Id;
            ChamCongNhanVien(model, SearchName);
            return PartialView(model);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult _GetVTDoanhThuNhanVien(int ThangId, int NamId, int Loai1Id, string SearchName)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var model = new DoanhThuNhanVienLuotModel();
            model.ThangId = ThangId;
            model.NamId = NamId;
            model.Loai1Id = Loai1Id;
            ChamCongNhanVien(model, SearchName);
            return PartialView(model);
        }
        [NonAction]
        protected virtual int CountXeXuatBenByNhanVien(List<ThongKeLuotXuatBenItem> dataview, int NhanVienId, int Ngay, int Thang, int Nam)
        {
            return dataview.Where(c => c.NhanVienIds.Contains(NhanVienId) && c.Ngay == Ngay && c.Thang == Thang && c.Nam == Nam).Count();
        }
        protected virtual decimal DoanhthuXuatBenByNhanVien(List<ThongKeLuotXuatBenItem> dataview, int NhanVienId, int Ngay, int Thang, int Nam)
        {
            return dataview.Where(c => c.NhanVienIds.Contains(NhanVienId) && c.Ngay == Ngay && c.Thang == Thang && c.Nam == Nam).Sum(c => c.DoanhThu);

        }
        [NonAction]
        protected virtual void ChamCongNhanVien(DoanhThuNhanVienLuotModel model, string SearchName)
        {

            int SoNhan = 0;
            DoanhThuNhanVienLuotModel.DateModel[] arrNhan = new DoanhThuNhanVienLuotModel.DateModel[] { };
            var arrNhanVien = _nhaxeService.GetAllNhanVienByNhaXe(_workContext.NhaXeId, new ENKieuNhanVien[] { ENKieuNhanVien.LaiXe, ENKieuNhanVien.PhuXe }, SearchName);
            var arrNhanVienId = arrNhanVien.Select(c => c.Id).ToArray();

            model.SoNhanVien = arrNhanVien.Count();


            switch (model.Loai1Id)
            {
                case (int)ENBaoCaoChuKyThoiGian.HangNgay:
                    {
                        SoNhan = DateTime.DaysInMonth(model.NamId, model.ThangId);
                        arrNhan = Enumerable.Range(1, SoNhan).Select(c =>
                        {
                            var item = new DoanhThuNhanVienLuotModel.DateModel();
                            item.nhan = c;
                            item.day = c;
                            item.month = model.ThangId;
                            item.year = model.NamId;
                            return item;
                        }).ToArray();
                        break;
                    }
                case (int)ENBaoCaoChuKyThoiGian.HangThang:
                    {
                        SoNhan = 12;
                        arrNhan = Enumerable.Range(1, SoNhan).Select(c =>
                        {
                            var item = new DoanhThuNhanVienLuotModel.DateModel();
                            item.day = 0;
                            item.month = c;
                            item.nhan = c;
                            item.year = model.NamId;
                            return item;
                        }).ToArray();
                        model.ThangId = 0;
                        break;
                    }
                case (int)ENBaoCaoChuKyThoiGian.HangNam:
                    {
                        //lay trong vong 5 nam
                        SoNhan = 5;
                        arrNhan = Enumerable.Range(DateTime.Now.Year - SoNhan + 1, SoNhan).Select(c =>
                        {
                            var item = new DoanhThuNhanVienLuotModel.DateModel();
                            item.day = 0;
                            item.month = 0;
                            item.nhan = c;
                            item.year = c;
                            return item;
                        }).ToArray();
                        model.ThangId = 0;
                        model.NamId = 0;
                        break;
                    }
            }

            //lay thong tin du lieu 
            var _dataview = _nhaxeService.GetThongKeLuotXuatBen(model.ThangId, model.NamId, arrNhanVienId, model.Loai2Id);

            model.SoNhan = SoNhan;
            model.DoanhThuLuot = new DoanhThuNhanVienLuotModel.NhanVienDateModel[model.SoNhanVien + 1, SoNhan + 1];
            for (int i = 0; i < model.SoNhanVien + 1; i++)
            {
                for (int j = 0; j < SoNhan + 1; j++)
                {
                    if (i == 0 && j > 0)
                    {
                        model.DoanhThuLuot[0, j] = new DoanhThuNhanVienLuotModel.NhanVienDateModel();
                        model.DoanhThuLuot[0, j].Nhan = arrNhan[j - 1].nhan;

                    }
                    if (j == 0 && i > 0)
                    {
                        model.DoanhThuLuot[i, 0] = new DoanhThuNhanVienLuotModel.NhanVienDateModel();
                        model.DoanhThuLuot[i, 0].NhanVienId = arrNhanVien[i - 1].Id;
                        model.DoanhThuLuot[i, 0].TenNhanVien = arrNhanVien[i - 1].HoVaTen;



                    }

                    if (i > 0 && j > 0)
                    {
                        model.DoanhThuLuot[i, j] = new DoanhThuNhanVienLuotModel.NhanVienDateModel();
                        model.DoanhThuLuot[i, j].Nhan = arrNhan[j - 1].nhan;
                        model.DoanhThuLuot[i, j].NhanVienId = arrNhanVien[i - 1].Id;
                        model.DoanhThuLuot[i, j].soLuot = CountXeXuatBenByNhanVien(_dataview, arrNhanVien[i - 1].Id, arrNhan[j - 1].day, arrNhan[j - 1].month, arrNhan[j - 1].year);
                        model.DoanhThuLuot[i, j].DoanhThu = DoanhthuXuatBenByNhanVien(_dataview, arrNhanVien[i - 1].Id, arrNhan[j - 1].day, arrNhan[j - 1].month, arrNhan[j - 1].year);

                    }

                }

            }

        }



        #endregion
        #region doanh thu hàng hóa theo tuyến
        public ActionResult HangHoaTheoTuyen()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel();
            model.TuNgay = DateTime.Now.AddDays(-7);
            model.DenNgay = DateTime.Now.AddDays(1);
            model.ListLoai2 = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                var item = new SelectListItem();
                item.Text = string.Format("{0} ({1})", c.MoTa, c.MaHanhTrinh);
                item.Value = c.Id.ToString();
                return item;
            }).ToList();
            var _item = new SelectListItem();
            _item.Text = "Chọn hành trình";
            _item.Value = "0";
            model.ListLoai2.Insert(0, _item);
            return View(model);
        }
        [HttpPost]
        public ActionResult HangHoaTheoTuyen(DataSourceRequest command, DateTime tuNgay, DateTime denNgay, int HanhTrinhId)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            var items = _phieuguihangService.HanhTrinhPhieuGuiHang(tuNgay, denNgay, HanhTrinhId);

            var gridModel = new DataSourceResult
            {
                Data = items,
                Total = items.Count
            };
            return Json(gridModel);
        }
        public ActionResult ExportExcelHangHoaTheoTuyen(DateTime TuNgay, DateTime DenNgay, int HanhTrinhId)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            try
            {



                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    var tenhanhtrinh = _hanhtrinhService.GetHanhTrinhById(HanhTrinhId).MoTa;

                    var items = _phieuguihangService.HanhTrinhPhieuGuiHang(TuNgay, DenNgay, HanhTrinhId);
                    _exportManager.ExportHanhTrinhHangHoa(stream, items, TuNgay, DenNgay, tenhanhtrinh);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "BC hang hoa theo tuyen" + TuNgay.ToString("ddMMyyyy") + "_" + DenNgay.ToString("ddMMyyyy") + ".xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }
        public ActionResult _ChiTietHangHoaTuyen(int HanhTrinhId, DateTime NgayBan, int SoQuyen)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();
            var items = _phoiveService.GetDetailDoanhThuVeQuay(HanhTrinhId, NgayBan, SoQuyen);
            return PartialView(items);
        }
        #endregion
        #region ve huy
        public ActionResult DanhSachVeHuy()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            var modeldoanhthu = new BaoCaoNhaXeModel();
            modeldoanhthu.TuNgay = DateTime.Now.AddMonths(-1);
            modeldoanhthu.DenNgay = DateTime.Now;
            PrepareListVanPhongModel(modeldoanhthu);
            var item = new SelectListItem();
            item.Value = "0";
            item.Text = "--Chọn văn phòng---";
            modeldoanhthu.VanPhongs.Add(item);
            return View(modeldoanhthu);
        }
        [HttpPost]
        public ActionResult _DanhSachVeHuy(DataSourceRequest command, DateTime tuNgay, DateTime denNgay, int VanPhongId)
        {

            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVBaoCao) && !this.isRightAccess(_permissionService, StandardPermissionProvider.CVHoatDongBanVe))
                return AccessDeniedView();

            var items = _phoiveService.GetVeHuy(tuNgay, denNgay, _workContext.NhaXeId, VanPhongId);

            var gridModel = new DataSourceResult
            {
                Data = items,
                Total = items.Count
            };
            return Json(gridModel);
        }
        #endregion
        #region Các method sử dụng cho bao cao chung
        private String GetTopPageOfReport()
        {
            var item = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, ENNhaXeCauHinh.HEADER_BAO_CAO);
            if (item != null)
                return item.GiaTri;
            return "";

        }
        private ActionResult exportToExcel(BaoCaoNhaXeModel model, string filename)
        {
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _exportManager.ExportDataTableToExcel(stream, model.headers, model.dataReport, model.Title, model.TitleColSpan, model.topPage, model.isShowSTT, model.addSumRight, model.idxColForSum, model.addSumBottom);
                bytes = stream.ToArray();

            }
            return File(bytes, "text/xls", filename);
        }
        BaoCaoNhaXeModel createDataBaoCaoNhaXeModel(BaoCaoNhaXeModel model)
        {
            switch (model.LoaiBaoCao)
            {
                case BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.DANH_SACH_NHAN_VIEN:
                    model = createDanhSachNhanVien(model);
                    break;
                case BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.DOANH_THU_THEO_GIO:
                    model = createDoanhThuTheoGio(model);
                    break;
                case BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.DOANH_THU_HANG_NGAY:
                    model = createDoanhThuHangNgay(model);
                    break;
                case BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.HANG_HOA_VAN_PHONG:
                    model = createDoanhThuHangHoaVanPhong(model);
                    break;
                case BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.BAO_CAO_VAN_PHONG_TRA:
                    model = createDoanhThuHangHoaVanPhongTra(model);
                    break;
                case BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.HANG_HOA_TONG_HOP:
                    model = createDoanhThuHangHoaTongHop(model);
                    break;
                case BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.BAO_CAO_PHIEU_VAN_CHUYEN:
                    model = createBaoCaoPhieuVanChuyen(model);
                    break;
                case BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.PHIEU_VAN_CHUYEN_NGAY:
                    model = createLenhVanChuyenNgay(model);
                    break;
                case BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.PHIEU_VAN_CHUYEN_THANG:
                    model = createLenhVanChuyenThang(model);
                    break;
                case BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.CHI_TIEU_THANG:
                    model = createChiTieuThang(model);
                    break;
                case BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.TO_VAN_CHUYEN_THANG:
                    model = createToVanchuyenThang(model);
                    break;
                case BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.BAO_CAO_KHACH_HANG_GUI:
                    model = createBaoCaoKhachHangTiemNang(model);
                    break;
                case BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.BAO_CAO_SMS_GUI:
                    model = createBaoCaoSMSGui(model);
                    break;
                case BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.BAO_CAO_DOANH_THU_THANG:
                    model = createBaoCaoDoanhThuThang(model);
                    break;
            }
            return model;
        }
        public static DataTable ToDataTableSimple(int NumOfCol)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < NumOfCol; i++)
            {
                DataColumn dc = new DataColumn("Col" + i.ToString(), typeof(System.String));
                dt.Columns.Add(dc);
            }
            return dt;
        }
        [HttpPost]
        public ActionResult _BaoCaoTongHop(BaoCaoNhaXeModel model)
        {
            model = createDataBaoCaoNhaXeModel(model);
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult _ExportBaoCao(BaoCaoNhaXeModel model)
        {
            model = createDataBaoCaoNhaXeModel(model);
            return exportToExcel(model, model.FileNameExport + DateTime.Now.ToString("yyyyMMddHHmm") + ".xls");
        }
        #endregion
        #region excel nhan vien

        public ActionResult _ExportNhanVien(int LoaiBaoCaoId)
        {
            var model = new BaoCaoNhaXeModel();
            model.LoaiBaoCaoId = LoaiBaoCaoId;
            model = createDataBaoCaoNhaXeModel(model);
            return exportToExcel(model, model.FileNameExport + DateTime.Now.ToString("yyyyMMddHHmm") + ".xls");
        }
        BaoCaoNhaXeModel createDanhSachNhanVien(BaoCaoNhaXeModel model)
        {

            //cau hinh bao cao
            model.isShowSTT = true;
            model.topPage = GetTopPageOfReport();

            model.Title = new string[] { "DANH SÁCH NHÂN VIÊN" };
            //lay thong tin ngay trong thang thong ke
            //tao header cua bao cao
            var headers = new List<String>();
            headers.Add("Họ và tên");
            headers.Add("Điện thoại");
            headers.Add("Chức vụ");
            headers.Add("Số CMT");
            headers.Add("Ghi chú");

            //loc thong tin xe di theo hanh trinh,
            //lay tat ca tuyen
            model.TitleColSpan = new List<string[]>();
            //add header colspan ngay
            model.TitleColSpan.Add(new string[] { "", "1", "" });
            var nhanviens = _nhanvienService.GetAll(_workContext.NhaXeId);

            model.headers = headers.ToArray();
            //tao du lieu trong bao cao
            DataTable dataReport = ToDataTableSimple(model.headers.Length);

            foreach (var nv in nhanviens)
            {
                var dr = dataReport.NewRow();

                dr[0] = nv.HoVaTen;
                dr[1] = nv.DienThoai;


                var chucvu = nv.KieuNhanVien.GetLocalizedEnum(_localizationService, _workContext);
                dr[2] = chucvu;
                dr[3] = nv.CMT_Id;
                string ghichu = "";
                if (!string.IsNullOrEmpty(nv.GhiChu))
                {

                    ghichu = Regex.Replace(nv.GhiChu, @"<[^>]*>", String.Empty);
                }
                dr[4] = ghichu;
                dataReport.Rows.Add(dr);
            }

            model.dataReport = dataReport;

            return model;
        }
        #endregion
        #region doanh thu theo gio chay
        public ActionResult DoanhThuTheoGioChay()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel();
            model.TuNgay = DateTime.Now.AddDays(-3);
            model.DenNgay = DateTime.Now.AddDays(1);


            model.Xe = _xeinfoService.GetAllXeVanChuyenByNhaXeId(_workContext.NhaXeId,"").Where(c => c.TrangThaiXeId != (int)ENTrangThaiXe.DungHoatDong).Select(c =>
            {
                var item = new SelectListItem();
                item.Text = c.BienSo;
                item.Value = c.Id.ToString();
                return item;
            }).ToList();


            model.ListLoai2 = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                var item = new SelectListItem();
                item.Text = string.Format("{0} ({1})", c.MoTa, c.MaHanhTrinh);
                item.Value = c.Id.ToString();
                return item;
            }).ToList();
            var hanhtrinhall = new SelectListItem();
            hanhtrinhall.Text = "Chọn hành trình";
            hanhtrinhall.Value = "0";
            model.ListLoai2.Insert(0, hanhtrinhall);


            model.LoaiBaoCao = BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.DOANH_THU_THEO_GIO;
            PrepareListNgayThangNam(model);
            return View(model);
        }
        BaoCaoNhaXeModel createDoanhThuTheoGio(BaoCaoNhaXeModel model)
        {

            //cau hinh bao cao
            model.isShowSTT = true;
            model.topPage = GetTopPageOfReport();

            model.Title = new string[] { "BÁO CÁO DOANH THU THEO GIỜ TỪ NGÀY " + model.TuNgay.ToString("dd/MM/yyyy") + "ĐẾN NGÀY" + model.DenNgay.ToString("dd/MM/yyyy") };
            //lay thong tin ngay trong thang thong ke
            //tao header cua bao cao
            var headers = new List<String>();
            headers.Add("Ngày");
            headers.Add("Biển số");
            headers.Add("Lái xe");
            headers.Add("Phụ xe");
            headers.Add("Giờ chạy");
            headers.Add("Hành trình");
            headers.Add("Tổng khách");
            headers.Add("Tổng DT");
            //loc thong tin xe di theo hanh trinh,
            //lay tat ca tuyen
            model.TitleColSpan = new List<string[]>();
            //add header colspan ngay
            model.TitleColSpan.Add(new string[] { "", "1", "" });
            model.GioChaySearch = Convert.ToDateTime(model.GioChay);
            var doanhthus = _phoiveService.GetDoanhThuTheoGio(model.TuNgay, model.DenNgay, model.KeySearch, model.GioChaySearch, model.Loai2Id);

            model.headers = headers.ToArray();
            //tao du lieu trong bao cao
            DataTable dataReport = ToDataTableSimple(model.headers.Length);

            foreach (var nv in doanhthus)
            {
                var dr = dataReport.NewRow();

                dr[0] = nv.NgayDi.ToString("dd/MM/yyyy");
                dr[1] = nv.BienSo;
                dr[2] = nv.TenLaiXe;
                dr[3] = nv.TenPhuXe;
                dr[4] = nv.NgayDi.ToString("HH:mm");
                dr[5] = nv.TenHanhTrinh;
                dr[6] = nv.TongKhach;
                dr[7] = nv.TongDT.ToString("###,###");

                dataReport.Rows.Add(dr);
            }

            model.dataReport = dataReport;

            return model;
        }
        #endregion
        #region doanh thu tong hop hang ngay
        public ActionResult DoanhThuHangNgay()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel();
            model.TuNgay = DateTime.Now.NgayDauThang();
            model.DenNgay = DateTime.Now.AddDays(1);


            model.ListLoai2 = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                var item = new SelectListItem();
                item.Text = string.Format("{0} ({1})", c.MoTa, c.MaHanhTrinh);
                item.Value = c.Id.ToString();
                return item;
            }).ToList();
            var hanhtrinhall = new SelectListItem();
            hanhtrinhall.Text = "Chọn hành trình";
            hanhtrinhall.Value = "0";
            model.ListLoai2.Insert(0, hanhtrinhall);

            model.LoaiBaoCao = BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.DOANH_THU_HANG_NGAY;
            PrepareListNgayThangNam(model);
            return View(model);
        }
        BaoCaoNhaXeModel createDoanhThuHangNgay(BaoCaoNhaXeModel model)
        {

            //cau hinh bao cao
            model.isShowSTT = true;
            model.topPage = GetTopPageOfReport();
            model.Title = new string[] { "BÁO CÁO DOANH THU HÀNG NGÀY " + model.TuNgay.ToString("dd/MM/yyyy") + "ĐẾN NGÀY" + model.DenNgay.ToString("dd/MM/yyyy") };

            //lay thong tin ngay trong thang thong ke
            //tao header cua bao cao
            var headers = new List<String>();
            headers.Add("Ngày");
            headers.Add("Số chuyến");
            model.idxColForSum = 2;
            headers.Add("DT vé bán ngay");
            headers.Add("DT vé bán trước");
            headers.Add("DT vé hủy");
            headers.Add("DT hàng hóa");
            model.addSumBottom = true;
            model.idxColForSum = 4;
            model.addSumRight = true;
            //loc thong tin xe di theo hanh trinh,
            //lay tat ca tuyen
            model.TitleColSpan = new List<string[]>();
            //add header colspan ngay
            // model.TitleColSpan.Add(new string[] { "", "1", "" });
            var doanhthus = _phoiveService.GetDoanhThuHangNgay(model.TuNgay, model.DenNgay, model.Loai2Id);

            model.headers = headers.ToArray();
            //tao du lieu trong bao cao
            DataTable dataReport = ToDataTableSimple(model.headers.Length);

            foreach (var nv in doanhthus)
            {
                var dr = dataReport.NewRow();

                dr[0] = nv.NgayDi.ToString("dd/MM/yyyy");
                dr[1] = nv.SoChuyen.ToString();
                dr[2] = nv.DTVeBanNgay.ToString("###,###");
                dr[3] = nv.DTVeBanTruoc.ToString("###,###");
                dr[4] = nv.DTVeHuy.ToString("###,###");
                dr[5] = nv.DTHangHoa.ToString("###,###");


                dataReport.Rows.Add(dr);
            }

            model.dataReport = dataReport;

            return model;
        }

        #endregion
        #region doanh thu hang hoa theo van phong hang ngay
        void prepareTuyen(BaoCaoNhaXeModel model)
        {
            //chon tuyen
            model.ListTuyens = _phieuchuyenphatService.GetAllTuyenVanDoanh(_workContext.NhaXeId).Select(c =>
            {
                var item = new SelectListItem();
                item.Text = c.TenTuyen;
                item.Value = c.Id.ToString();
                return item;
            }).ToList();
            var hanhtrinhall = new SelectListItem();
            hanhtrinhall.Text = "Chọn tuyến ";
            hanhtrinhall.Value = "0";
            model.ListTuyens.Insert(0, hanhtrinhall);

        }
        void prepareBenXe(BaoCaoNhaXeModel model)
        {
            //chon tuyen
            model.ListBenXes = _benxeService.GetAllBenXe().Select(c =>
            {
                var item = new SelectListItem();
                item.Text = c.TenBenXe;
                item.Value = c.Id.ToString();
                return item;
            }).ToList();
            var itemfirst = new SelectListItem();
            itemfirst.Text = "Chọn bến xe";
            itemfirst.Value = "0";
            model.ListBenXes.Insert(0, itemfirst);
        }
        public ActionResult DoanhThuHangHoaVanPhong()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
            {
                if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                    return AccessDeniedView();
            }

            var model = new BaoCaoNhaXeModel();
            model.NgayGuiHang = DateTime.Now;
            //loc tuyen
            prepareTuyen(model);
            //loc van phong
            PrepareListVanPhongModel(model, true);

            model.LoaiBaoCao = BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.HANG_HOA_VAN_PHONG;
            return View(model);
        }


        BaoCaoNhaXeModel createDoanhThuHangHoaVanPhong(BaoCaoNhaXeModel model)
        {

            //cau hinh bao cao
            model.isShowSTT = true;
            model.topPage = GetTopPageOfReport();
            model.Title = new string[] { "BÁO CÁO DOANH THU NGÀY " + model.NgayGuiHang.ToString("dd/MM/yyyy") };

            //lay thong tin ngay trong thang thong ke
            //tao header cua bao cao
            model.TitleColSpan = new List<string[]>();
            var headers = new List<String>();
            model.TitleColSpan.Add(new string[] { "", "20", "" });
            headers.Add("Mã phiếu");
            headers.Add("Ngày tháng");
            headers.Add("VP TRẢ HÀNG");
            headers.Add("SỐ XE");
            headers.Add("GIỜ CHẠY");
            //add by lent
            headers.Add("NGƯỜI GỬI");
            headers.Add("NGƯỜI NHẬN");
            /////
            headers.Add("CƯỚC ĐÃ TT");
            headers.Add("CƯỚC CHƯA TT");
            headers.Add("THỰC THU");
            model.addSumBottom = true;
            model.addSumRight = false;
            //loc thong tin xe di theo hanh trinh,
            //lay tat ca tuyen
            //add header colspan ngay
            model.TitleColSpan = new List<string[]>();
            //model.TitleColSpan.Add(new string[] { "", "1", "" });          
            var giaodich = _phieuchuyenphatService.GetAllPhieuChuyenPhatTrongThang(_workContext.NhaXeId, model.VanPhongId, model.NgayGuiHang, model.KeySearch, 0, null, null, 0, 0, null, true, model.TuyenId);


            model.headers = headers.ToArray();
            //tao du lieu trong bao cao
            DataTable dataReport = ToDataTableSimple(model.headers.Length);

            foreach (var nv in giaodich)
            {
                var dr = dataReport.NewRow();
                int i = 0;
                dr[i] = nv.MaPhieu;
                i++;
                dr[i] = nv.LoaiPhieu.ToCVEnumText(_localizationService);
                i++;
                dr[i] = nv.VanPhongNhan.Ma;
                i++;

                dr[i] = nv.phieuvanchuyen != null ? nv.phieuvanchuyen.SoLenh : "";
                i++;
                dr[i] = Nhatki != null ? Nhatki.chuyendi.SoXe : "";
                i++;
                dr[i] = Nhatki != null ? Nhatki.chuyendi.NgayDi.ToString("HH:mm") : "";
                i++;

                dr[i] = nv.NguoiGui.toText();
                i++;
                dr[i] = nv.NguoiNhan.toText();
                i++;

                dr[i] = nv.TenHang;
                i++;
                dr[i] = nv.TongCuocDaThanhToan.ToSoNguyenCuocPhi();
                i++;
                dr[i] = (nv.TongTienCuoc - nv.TongCuocDaThanhToan).ToSoNguyenCuocPhi();
                i++;

                //foreach (var item in tovanchuyen)
                //{
                //    decimal cuocvc = 0;
                //    if (nv.CuocTanNoi > 0)
                //    {
                //        if (nv.ToVanChuyenTraId == null)
                //        {
                //            //add by mai: lay gan to van chuyen trong khu vuc cua van phong nhan
                //            var tovc = nv.VanPhongNhan.tovanchuyens.FirstOrDefault();
                //            if (tovc != null)
                //                nv.ToVanChuyenTraId = tovc.Id;
                //        }
                //    }
                //    if (item.Id == nv.ToVanChuyenTraId)
                //        cuocvc = nv.CuocTanNoi;

                //    if (item.Id == nv.ToVanChuyenNhanId)
                //        cuocvc = nv.CuocNhanTanNoi;
                //    dr[i] = cuocvc.ToSoNguyenCuocPhi();
                //    i++;
                //}
                //cuoc vuot tuyen

                //dr[i] = nv.CuocVuotTuyen.ToSoNguyenCuocPhi();
                //i++;
                /*decimal nhan = nv.CuocPhi + (nv.CuocCapToc + nv.CuocGiaTri) / 2;
                decimal tra = nv.CuocVuotTuyen + (nv.CuocCapToc + nv.CuocGiaTri) / 2;

                if (nv.phieuvanchuyen.LoaiPhieuVanChuyen == ENLoaiPhieuVanChuyen.VuotTuyen)
                {
                    dr[i] = nhan.ToSoNguyenCuocPhi();
                    i++;

                    dr[i] = tra.ToSoNguyenCuocPhi();
                    i++;
                }
                else
                {
                    dr[i] = 0;
                    i++;

                    dr[i] = 0;
                    i++;
                }
                */
                ///////////////////////
                //dr[i] = cong.ToSoNguyenCuocPhi();
                dr[i] = nv.TongCuocDaThanhToan.ToSoNguyenCuocPhi();
                dataReport.Rows.Add(dr);
            }
            model.dataReport = dataReport;

            return model;
        }
        #endregion
        #region doanh thu theo vp tra
        public ActionResult DoanhThuTheoVanPhongTra()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
            {
                if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                    return AccessDeniedView();
            }

            var model = new BaoCaoNhaXeModel();
            model.NgayGuiHang = DateTime.Now;
            //loc tuyen
            prepareTuyen(model);
            //loc van phong
            PrepareListVanPhongModel(model, true);

            model.LoaiBaoCao = BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.BAO_CAO_VAN_PHONG_TRA;
            return View(model);
        }


        BaoCaoNhaXeModel createDoanhThuHangHoaVanPhongTra(BaoCaoNhaXeModel model)
        {

            //cau hinh bao cao
            model.isShowSTT = true;
            model.topPage = GetTopPageOfReport();
            model.Title = new string[] { "BÁO CÁO DOANH THU VĂN PHÒNG TRẢ NGÀY " + model.NgayGuiHang.ToString("dd/MM/yyyy") };

            //lay thong tin ngay trong thang thong ke
            //tao header cua bao cao
            model.TitleColSpan = new List<string[]>();
            var headers = new List<String>();
            model.TitleColSpan.Add(new string[] { "", "20", "" });
            headers.Add("TUYẾN");
            headers.Add("Mã phiếu");
            headers.Add("LOẠI PHIẾU");

            headers.Add("VP NHẬN HÀNG");
            headers.Add("SỐ LỆNH");
            headers.Add("SỐ XE");
            headers.Add("GIỜ CHẠY");
            //add by lent
            headers.Add("NGƯỜI GỬI");
            headers.Add("NGƯỜI NHẬN");
            /////
            headers.Add("SL HÀNG");
            model.idxColForSum = 9;
            headers.Add("CƯỚC ĐÃ TT");
            headers.Add("CƯỚC CHƯA TT");
            //var tovanchuyen = _phieuchuyenphatService.GetAllToVanChuyen(_workContext.NhaXeId);
            //foreach (var item in tovanchuyen)
            //{
            //    headers.Add(item.TenTo);
            //}
            //model.TitleColSpan.Add(new string[] { "CƯỚC VƯỢT TUYẾN", "2", "" });
            //headers.Add("Cước vượt tuyến(nhận)");
            //headers.Add("Cước vượt tuyến(trả)");
            //headers.Add("Cước vượt tuyến");
            headers.Add("THỰC THU");
            model.addSumBottom = true;
            model.addSumRight = false;
            //loc thong tin xe di theo hanh trinh,
            //lay tat ca tuyen
            //add header colspan ngay
            model.TitleColSpan = new List<string[]>();
            //model.TitleColSpan.Add(new string[] { "", "1", "" });          
            var giaodich = _phieuchuyenphatService.GetAllPhieuChuyenPhatTrongThangTheoVPTra(_workContext.NhaXeId, 0, null, model.KeySearch, model.VanPhongId, null, null, 0, 0, model.NgayGuiHang, true, model.TuyenId);


            model.headers = headers.ToArray();
            //tao du lieu trong bao cao
            DataTable dataReport = ToDataTableSimple(model.headers.Length);

            foreach (var nv in giaodich)
            {
                var dr = dataReport.NewRow();
                int i = 0;
                string tuyen = "";
                var Nhatki = new PhieuChuyenPhatVanChuyen();
                if (nv.nhatkyvanchuyens.Count > 0)
                {
                    Nhatki = nv.nhatkyvanchuyens.FirstOrDefault();
                    tuyen = Nhatki.chuyendi.HanhTrinh.tuyen.TenVietTat;

                }
                else
                    Nhatki = null;
                dr[i] = tuyen;
                i++;
                dr[i] = nv.MaPhieu;
                i++;
                dr[i] = nv.LoaiPhieu.ToCVEnumText(_localizationService);
                i++;
                dr[i] = nv.VanPhongGui.Ma;
                i++;

                dr[i] = nv.phieuvanchuyen != null ? nv.phieuvanchuyen.SoLenh : "";
                i++;
                dr[i] = Nhatki != null ? Nhatki.chuyendi.SoXe : "";
                i++;
                dr[i] = Nhatki != null ? Nhatki.chuyendi.NgayDi.ToString("HH:mm") : "";
                i++;

                dr[i] = nv.NguoiGui.toText();
                i++;
                dr[i] = nv.NguoiNhan.toText();
                i++;

                dr[i] = nv.TenHang;
                i++;
                dr[i] = nv.TongCuocDaThanhToan.ToSoNguyenCuocPhi();
                i++;
                dr[i] = (nv.TongTienCuoc - nv.TongCuocDaThanhToan).ToSoNguyenCuocPhi();
                i++;

                //foreach (var item in tovanchuyen)
                //{
                //    decimal cuocvc = 0;
                //    if (nv.CuocTanNoi > 0)
                //    {
                //        if (nv.ToVanChuyenTraId == null)
                //        {
                //            //add by mai: lay gan to van chuyen trong khu vuc cua van phong nhan
                //            var tovc = nv.VanPhongNhan.tovanchuyens.FirstOrDefault();
                //            if (tovc != null)
                //                nv.ToVanChuyenTraId = tovc.Id;
                //        }
                //    }
                //    if (item.Id == nv.ToVanChuyenTraId)
                //        cuocvc = nv.CuocTanNoi;

                //    if (item.Id == nv.ToVanChuyenNhanId)
                //        cuocvc = nv.CuocNhanTanNoi;
                //    dr[i] = cuocvc.ToSoNguyenCuocPhi();
                //    i++;
                //}
                //cuoc vuot tuyen

                //dr[i] = nv.CuocVuotTuyen.ToSoNguyenCuocPhi();
                //i++;
                /*decimal nhan = nv.CuocPhi + (nv.CuocCapToc + nv.CuocGiaTri) / 2;
                decimal tra = nv.CuocVuotTuyen + (nv.CuocCapToc + nv.CuocGiaTri) / 2;

                if (nv.phieuvanchuyen.LoaiPhieuVanChuyen == ENLoaiPhieuVanChuyen.VuotTuyen)
                {
                    dr[i] = nhan.ToSoNguyenCuocPhi();
                    i++;

                    dr[i] = tra.ToSoNguyenCuocPhi();
                    i++;
                }
                else
                {
                    dr[i] = 0;
                    i++;

                    dr[i] = 0;
                    i++;
                }
                */
                ///////////////////////             
                //dr[i] = cong.ToSoNguyenCuocPhi();
                dr[i] = (nv.TongTienCuoc - nv.TongCuocDaThanhToan).ToSoNguyenCuocPhi();
                dataReport.Rows.Add(dr);
            }
            model.dataReport = dataReport;

            return model;
        }
        #endregion
        #region doanh thu hang hoa theo van phong hang thang
        public ActionResult DoanhThuHangHoaTongHop()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel();
            //loc van phong
            PrepareListVanPhongModel(model, false, true);
            model.TuNgay = DateTime.Now.NgayDauThang();
            //model.TuNgay = DateTime.Now;
            model.DenNgay = DateTime.Now.AddDays(1);
            model.LoaiBaoCao = BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.HANG_HOA_TONG_HOP;
            return View(model);
        }


        BaoCaoNhaXeModel createDoanhThuHangHoaTongHop(BaoCaoNhaXeModel model)
        {

            //cau hinh bao cao
            model.isShowSTT = true;
            model.topPage = GetTopPageOfReport();
            model.Title = new string[] { "BÁO CÁO DOANH THU TỪ NGÀY " + model.TuNgay.ToString("dd/MM/yyyy") + "ĐẾN NGÀY" + model.DenNgay.ToString("dd/MM/yyyy") };

            //lay thong tin ngay trong thang thong ke
            //tao header cua bao cao
            var headers = new List<String>();
            headers.Add("NGÀY");
            headers.Add("TÊN");

            headers.Add("NHÓM PHIẾU");
            headers.Add("DTTB/GD");
            headers.Add("SỐ GD NHẬN");
            headers.Add("TỔNG CƯỚC");

            var tovanchuyen = _phieuchuyenphatService.GetAllToVanChuyen(_workContext.NhaXeId);
            foreach (var item in tovanchuyen)
            {
                headers.Add(item.TenTo);
            }

           // headers.Add("CƯỚC VƯỢT TUYẾN");



            var tuyens = _phieuchuyenphatService.GetAllTuyenVanDoanh(_workContext.NhaXeId);
            foreach (var m in tuyens)
            {
                headers.Add("Còn lại " + m.TenVietTat);
            }
            headers.Add("THỰC THU");
            model.addSumBottom = false;
            // model.addSumRight = true;
            //loc thong tin xe di theo hanh trinh,
            //lay tat ca tuyen
            model.TitleColSpan = new List<string[]>();
            //add header colspan ngay



            model.headers = headers.ToArray();
            //tao du lieu trong bao cao
            DataTable dataReport = ToDataTableSimple(model.headers.Length);
            //for (var dt = model.TuNgay; dt <= model.DenNgay; dt = dt.AddDays(1))
            //{
               // CreateDTTongHop(dataReport, false, dt, model, tovanchuyen, tuyens);
            //}

            CreateDTTongHop(dataReport, true, DateTime.Now, model, tovanchuyen, tuyens);
            model.dataReport = dataReport;

            return model;
        }
        void CreateDTTongHop(DataTable dataReport, bool isthang, DateTime dt, BaoCaoNhaXeModel model, List<ToVanChuyen> tovanchuyen, List<TuyenVanDoanh> tuyens)
        {
            var giaodichs = new List<PhieuChuyenPhat>();
            var giaodichtra = new List<PhieuChuyenPhat>();
            if (!isthang)
            {
                giaodichs = _phieuchuyenphatService.GetAllPhieuChuyenPhatTrongThang(_workContext.NhaXeId, model.VanPhongId, dt, model.KeySearch);
                giaodichtra = _phieuchuyenphatService.GetAllPhieuChuyenPhatTrongThang(_workContext.NhaXeId, 0, null, model.KeySearch, model.VanPhongId, null, null, 0, 0, dt);

            }
            else
            {
                giaodichs = _phieuchuyenphatService.GetAllPhieuChuyenPhatTrongThang(_workContext.NhaXeId, model.VanPhongId, null, model.KeySearch, 0, model.TuNgay, model.DenNgay);
                giaodichtra = _phieuchuyenphatService.GetAllPhieuChuyenPhatTrongThang(_workContext.NhaXeId, 0, null, model.KeySearch, model.VanPhongId, model.TuNgay, model.DenNgay, 0, 0, null, false);
            }

            var nhomphieu = this.GetCVEnumSelectList<ENNhomPhieuChuyenPhat>(_localizationService, 0, false).Where(c => c.Value != Convert.ToString((int)ENLoaiPhieuChuyenPhat.All)).ToList(); ;
            DataTable dataReport1 = ToDataTableSimple(model.headers.Length);
            DataTable dataReport2 = ToDataTableSimple(model.headers.Length);
            decimal tongtbvp = 0;
            decimal tongtbtn = 0;
            foreach (var np in nhomphieu)
            {
                var arrgdvp = giaodichs.Where(c => c.NhomPhieuId == Convert.ToInt32(np.Value));
                //var  phieufirst=new PhieuChuyenPhat ();
                //if (arrgdvp.Count() > 0)
                //    phieufirst = arrgdvp.ToList().First().phieuchuyenphat;
                var dr = dataReport.NewRow();
                int i = 0;
                dr[i] = dt.Day;
                if (isthang)
                    dr[i] = "Tổng";

                i++;
                string loaiphieutext = "";
                if (Convert.ToInt32(np.Value) < 20)
                    loaiphieutext = "VP nhận";
                else loaiphieutext = "TN nhận";
                dr[i] = loaiphieutext;
                i++;
                dr[i] = np.Text;
                i++;
                decimal tongcuoc = arrgdvp.Sum(c => c.TongTienCuoc);
                //decimal tongcuoc = arrgdvp.Sum(c => c.CuocPhi);
                int sogd = arrgdvp.Count();
                decimal tb = 0;
                if (sogd > 0)
                    tb = tongcuoc / sogd;
                if (Convert.ToInt32(np.Value) < 20)
                    tongtbvp += tb;
                else
                    tongtbtn += tb;

                dr[i] = tb.ToSoNguyenCuocPhi();
                i++;
                dr[i] = sogd;
                i++;
                dr[i] = tongcuoc.ToSoNguyenCuocPhi();
                i++;

                foreach (var item in tovanchuyen)
                {
                    //var cuocvc = arrgdvp.Where(c => c.ToVanChuyenNhanId == item.Id).Sum(c => c.CuocNhanTanNoi);
                    var cuocvc = arrgdvp.Where(c => c.ToVanChuyenNhanId == item.Id && c.NhomPhieuId == Convert.ToInt32(np.Value)).Sum(c => c.CuocNhanTanNoi);

                    //var cuocvctra = arrgdvp.Where(c => c.ToVanChuyenTraId == item.Id).Sum(c => c.CuocTanNoi);
                    var cuocvctra = arrgdvp.Where(c => c.ToVanChuyenTraId == item.Id && c.NhomPhieuId == Convert.ToInt32(np.Value)).Sum(c => c.CuocTanNoi);

                    cuocvc = cuocvc + cuocvctra;
                    dr[i] = cuocvc.ToSoNguyenCuocPhi();
                    i++;
                }
                //cuoc vuot tuyen               
                //decimal cuocvt = arrgdvp.Sum(c => c.CuocVuotTuyen);
               // dr[i] = cuocvt.ToSoNguyenCuocPhi();
                //i++;
                decimal ThucThu = 0;
                foreach (var m in tuyens)
                {
                    decimal cuoctuyen = 0;
                    var t = arrgdvp.Where(c=>c.nhatkyvanchuyens !=null && c.nhatkyvanchuyens.Count() >0).ToList();
                    arrgdvp=arrgdvp.Where(c=>c.nhatkyvanchuyens.Count() >0);
                    var CuocTuyenTong = arrgdvp.Where(c => c.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId.Value == m.Id).Sum(c => c.TongTienCuoc);
                    var CuocPhi = arrgdvp.Where(c => c.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId.Value == m.Id).Sum(c => c.CuocPhi);
                    var CuocTuyenVC = arrgdvp.Where(c => c.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId.Value == m.Id).Sum(c => c.CuocTanNoi);

                    var CuocTuyenVCNhan = arrgdvp.Where(c => c.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId.Value == m.Id).Sum(c => c.CuocNhanTanNoi);

                    cuoctuyen = CuocTuyenTong - CuocTuyenVC - CuocTuyenVCNhan;
                    if (np.Value == ((int)ENNhomPhieuChuyenPhat.TN_VT).ToString() || np.Value == ((int)ENNhomPhieuChuyenPhat.VP_VT).ToString())
                    {
                        // cuoc tuyen 1= cuocphi-cuocvanchuyen+ 1/2 (cuoccaptoc+cuocgiatri)     
                        if (arrgdvp.Any(c => c.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId.Value == m.Id))
                        {
                            var CuocCapToc = arrgdvp.Where(c => c.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId.Value == m.Id).Sum(c => c.CuocCapToc);
                            var CuocTriGia = arrgdvp.Where(c => c.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId.Value == m.Id).Sum(c => c.CuocGiaTri);
                            cuoctuyen = CuocPhi + (CuocCapToc + CuocTriGia) / 2;
                        }
                        // cuoc tuyen 2
                        if (arrgdvp.Any(c => c.nhatkyvanchuyens.Count() > 1 && c.nhatkyvanchuyens.LastOrDefault().hanhtrinh.TuyenVanDoanhId == m.Id))
                        {
                            var CuocCapToc = arrgdvp.Where(c => c.nhatkyvanchuyens.LastOrDefault().hanhtrinh.TuyenVanDoanhId.Value == m.Id).Sum(c => c.CuocCapToc);
                            var CuocTriGia = arrgdvp.Where(c => c.nhatkyvanchuyens.LastOrDefault().hanhtrinh.TuyenVanDoanhId.Value == m.Id).Sum(c => c.CuocGiaTri);
                            var CuocVT = arrgdvp.Where(c => c.nhatkyvanchuyens.LastOrDefault().hanhtrinh.TuyenVanDoanhId.Value == m.Id).Sum(c => c.CuocVuotTuyen);
                            cuoctuyen = CuocVT + (CuocCapToc + CuocTriGia) / 2;
                        }
                    }
                    dr[i] = cuoctuyen.ToSoNguyenCuocPhi();
                    i++;
                    ThucThu = ThucThu + cuoctuyen;
                }
                dr[i] = ThucThu.ToSoNguyenCuocPhi();
                dataReport.Rows.Add(dr);
                if (Convert.ToInt32(np.Value) < 20)
                {
                    dataReport1.ImportRow(dr);
                }

                else
                    dataReport2.ImportRow(dr);

            }
            if (!isthang)
                return;

            var loaiphieu = this.GetCVEnumSelectList<ENLoaiPhieuChuyenPhat>(_localizationService, 0, false).Where(c => c.Value != Convert.ToString((int)ENLoaiPhieuChuyenPhat.All)).ToList();
            DataTable dataReport3 = ToDataTableSimple(model.headers.Length);
            foreach (var np in loaiphieu)
            {
                DataTable dataReport4 = dataReport1.Copy();
                if (Convert.ToInt32(np.Value) == (int)ENLoaiPhieuChuyenPhat.ThuTanNoi)
                    dataReport4 = dataReport2.Copy();
                //tong hop
                var arrgdvp = giaodichs.Where(c => c.LoaiPhieuId == Convert.ToInt32(np.Value));

                var dr1 = dataReport.NewRow();
                int i = 0;
                dr1[i] = dt.Day;
                if (isthang)
                    dr1[i] = "Tổng";
                i++;
                dr1[i] = "Cộng";
                i++;
                dr1[i] = np.Text; ;
                i++;
                decimal tongcuoc = arrgdvp.Sum(c => c.TongTienCuoc);
                //decimal tongcuoc = arrgdvp.Sum(c => c.CuocPhi);
                int sogd = arrgdvp.Count();
                decimal tb = 0;
                if (sogd > 0)
                    tb = tongcuoc / sogd;

                dr1[i] = tb.ToSoNguyenCuocPhi();
                //if (Convert.ToInt32(np.Value)==(int)ENLoaiPhieuChuyenPhat.ThuTaiVanPhong)
                    //dr1[i] = tongtbvp.ToSoNguyenCuocPhi();
                //if (Convert.ToInt32(np.Value) == (int)ENLoaiPhieuChuyenPhat.ThuTanNoi)
                    //dr1[i] = tongtbtn.ToSoNguyenCuocPhi();
                i++;
                dr1[i] = sogd;
                i++;
                dr1[i] = tongcuoc.ToSoNguyenCuocPhi();
                i++;
                foreach (var item in tovanchuyen)
                {
                    decimal sum = 0;
                    //object sumObject;
                    //sumObject = dataReport4.Compute("Sum(Col20)", "");

                    foreach (DataRow dr in dataReport4.Rows)
                    {
                        var sotien = dr.ItemArray[i].ToString();
                        if (sotien != "")
                            sum += Convert.ToInt32(sotien.Replace(".", ""));
                    }
                    dr1[i] = sum.ToString();
                    i++;

                }
                //cuoc vuot tuyen                

                /*decimal sumvt2 = 0;
                foreach (DataRow dr in dataReport4.Rows)
                {

                    sumvt2 += Convert.ToInt32(dr[i].ToInt());

                }
                dr1[i] = sumvt2.ToSoNguyenCuocPhi();
                i++;*/

                decimal ThucThuTong = 0;
                foreach (var m in tuyens)
                {
                    decimal sum = 0;
                    foreach (DataRow dr in dataReport4.Rows)
                    {
                        var sotien = dr.ItemArray[i].ToString();
                        if (sotien != "")
                            sum += Convert.ToInt32(sotien.Replace(".", ""));
                    }
                    dr1[i] = sum.ToString();
                    i++;
                    
            
                }
                
                foreach (DataRow dr in dataReport4.Rows)
                {
                    var item = dr.ItemArray[i].ToString();
                    if (item != "")
                        ThucThuTong += Convert.ToInt32(item.Replace(".", ""));

                }
                dr1[i] = ThucThuTong.ToString();
                dataReport.Rows.Add(dr1);
                dataReport3.ImportRow(dr1);

            }

            var dr2 = dataReport.NewRow();
            int k = 0;
            dr2[k] = dt.Day;
            if (isthang)
                dr2[k] = "Tổng";
            k++;
            dr2[k] = "Tổng ngày";
            k++;
            dr2[k] = "";
            k++;
            decimal tongcuocngay = giaodichs.Sum(c => c.TongTienCuoc);
            int sogdngay = giaodichs.Count();
            decimal tbcuoc = 0;
            if (sogdngay > 0)
                tbcuoc = tongcuocngay / sogdngay;
            dr2[k] = tbcuoc.ToSoNguyenCuocPhi();
            k++;
            dr2[k] = sogdngay;
            k++;
            dr2[k] = tongcuocngay.ToSoNguyenCuocPhi();
            k++;
            foreach (var item in tovanchuyen)
            {
                decimal sum = 0;
                foreach (DataRow dr in dataReport3.Rows)
                {

                    var sotien = dr.ItemArray[k].ToString();
                    if (sotien != "")
                        sum += Convert.ToInt32(sotien.Replace(".", ""));
                }
                dr2[k] = sum.ToString();
                k++;
            }
            //cuoc vuot tuyen
           /* decimal sumvt = 0;
            foreach (DataRow dr in dataReport3.Rows)
            {
                sumvt += Convert.ToInt32(dr[k].ToInt());
            }
            dr2[k] = sumvt.ToSoNguyenCuocPhi();
            k++;*/
            foreach (var m in tuyens)
            {
                decimal sum = 0;
                foreach (DataRow dr in dataReport3.Rows)
                {
                    var sotien = dr.ItemArray[k].ToString();
                    if (sotien != "")
                        sum += Convert.ToInt32(sotien.Replace(".", ""));
                }
                dr2[k] = sum.ToString();
                k++;
            }
            decimal ThucThuNgay = 0;
            foreach (DataRow dr in dataReport3.Rows)
            {
                var sotien = dr.ItemArray[k].ToString();
                if (sotien != "")
                    ThucThuNgay += Convert.ToInt32(sotien.Replace(".", ""));               

            }
            dr2[k] = ThucThuNgay.ToString();
            dataReport.Rows.Add(dr2);
            //giao dich tra hang
            var vanphongs = _nhaxeService.GetAllVanPhongByNhaXeId(_workContext.NhaXeId).Take(18);
            var dr3 = dataReport.NewRow();
            int p = 0;
            dr3[p] = dt.Day;
            if (isthang)
                dr3[p] = "Tổng";
            p++;
            dr3[p] = "GD trả hàng";
            p++;
            dr3[p] = "Văn phòng";
            p++;
            dr3[p] = "Tổng";
            p++;

            foreach (var item in vanphongs)
            {

                dr3[p] = item.Ma;
                p++;
            }

            dataReport.Rows.Add(dr3);
            //count gd tra
            var dr4 = dataReport.NewRow();
            int v = 0;
            dr4[v] = dt.Day;
            v++;
            dr4[v] = "";
            v++;
            dr4[v] = "Số GD trả";
            v++;
            dr4[v] = giaodichtra.Count();
            v++;

            foreach (var item in vanphongs)
            {
                int sogd = 0;
                sogd = giaodichtra.Where(c => c.VanPhongNhanId == item.Id).Count();
                dr4[v] = sogd;
                v++;
            }
            dataReport.Rows.Add(dr4);
            //tao tong hop to van chuyen nhan

        }

        #endregion
        #region doanh thu phieu van chuyen
        public ActionResult BaoCaoPhieuVanChuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao) && this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
            {
                return AccessDeniedView();
            }

            var model = new BaoCaoNhaXeModel();
            prepareBenXe(model);
            prepareTuyen(model);
            model.TuNgay = DateTime.Now;
            model.ListLoai1 = GetListBaoCaoDieuHanhBen();
            model.LoaiBaoCao = BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.BAO_CAO_PHIEU_VAN_CHUYEN;
            return View(model);
        }
        public ActionResult GetTuyenByBenXeId(int BenXeId)
        {
            var hanhtrinhs = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId, 0, 0)
                        .Where(c => c.DiemDons.OrderBy(d => d.ThuTu).First().diemdon.BenXeId == BenXeId).ToList();
            var tuyenids = hanhtrinhs.Select(c => c.TuyenVanDoanhId).Distinct().ToArray();
            var tuyens = _phieuchuyenphatService.GetAllTuyenVanDoanh(_workContext.NhaXeId).Where(c => tuyenids.Contains(c.Id)).ToList();
            return Json(tuyens, JsonRequestBehavior.AllowGet);
        }
        BaoCaoNhaXeModel createBaoCaoPhieuVanChuyen(BaoCaoNhaXeModel model)
        {

            //cau hinh bao cao
            model.isShowSTT = true;
            model.topPage = GetTopPageOfReport();
            string tenbenxe = "TẤT CẢ CÁC BẾN";
            if (model.BenXeId > 0)
            {
                var benxe = _benxeService.GetById(model.BenXeId);
                tenbenxe = benxe.TenBenXe.ToUpper();
            }
            model.Title = new string[] { ("BÁO CÁO ĐIỀU HÀNH " + tenbenxe + " : " + model.TuNgay.toThu() + ", ngày " + model.TuNgay.ToString("dd/MM/yyyy")).ToUpper(), "", "Cán bộ điều hành ca: Ca 1: ................................................, Ca 2: ................................................" };
            model.DenNgay = model.TuNgay.AddDays(1);
            //lay thong tin ngay trong thang thong ke
            //tao header cua bao cao
            var headers = new List<String>();
            if (model.TuyenId >= 0)
                headers.Add("Tuyến");
            headers.Add("SỐ XE");
            headers.Add("BẾN");
            headers.Add("GIỜ MỞ PHƠI");
            headers.Add("GIỜ XB");

            headers.Add("SỐ PXN");
            headers.Add("SỐ LVD");
            model.idxColForSum = 6;
            headers.Add("K. BẾN");
            headers.Add("LÁI XE");
            headers.Add("PHỤ XE");
            headers.Add("GHI CHÚ");

            model.addSumBottom = true;
            model.addSumRight = false;
            //loc thong tin xe di theo hanh trinh,
            //lay tat ca tuyen
            model.TitleColSpan = new List<string[]>();
            //add header colspan ngay
            model.headers = headers.ToArray();
            //tao du lieu trong bao cao
            DataTable dataReport = ToDataTableSimple(model.headers.Length);
            int[] hanhtrinhids = null;
            if (model.BenXeId > 0)
            {
                var hanhtrinhs = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId, 0, 0)
                    .Where(c => c.DiemDons.OrderBy(d => d.ThuTu).First().diemdon.BenXeId == model.BenXeId);
                if (model.TuyenId > 0)
                    hanhtrinhs = hanhtrinhs.Where(c => c.TuyenVanDoanhId == model.TuyenId);
                hanhtrinhids = hanhtrinhs.Select(c => c.Id).ToArray();
                //hanhtrinhids = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId, 0, 0)
                //.Where(c => c.DiemDons.OrderBy(d => d.ThuTu).First().diemdon.BenXeId == model.BenXeId)
                //.Select(c => c.Id).ToArray();
                if (hanhtrinhids.Length == 0)
                    hanhtrinhids = null;
            }
            var xexuatbens = _baocaoService.GetXeXuatBens(_workContext.NhaXeId, -1, hanhtrinhids, null, model.TuNgay, model.DenNgay, model.BenXeId);
            if (!string.IsNullOrEmpty(model.KeySearch))
            {
                xexuatbens = xexuatbens.Where(c => c.LaiPhuXes.Any(l => l.nhanvien.HoVaTen.Contains(model.KeySearch)) || c.SoXe.Contains(model.KeySearch)).ToList();
            }
            foreach (var chuyen in xexuatbens)
            {

                var dr = dataReport.NewRow();
                int i = 0;
                if (model.TuyenId >= 0)
                {
                    dr[i] = chuyen.HanhTrinh.tuyen.TenVietTat;
                    i++;
                }


                dr[i] = chuyen.SoXe;
                i++;

                var diemdon = chuyen.HanhTrinh.DiemDons.OrderBy(c => c.ThuTu).First().diemdon;
                var _tenbenxe = diemdon.benxe != null ? diemdon.benxe.TenBenXe : diemdon.TenDiemDon;
                //neu co thong tin ben xe thi uu tien lay thong tin ben xe tu thong tin chuyen di
                //if(chuyen.benxuatphat!=null)
                // {
                // _tenbenxe = chuyen.benxuatphat.TenBenXe;
                // }
                dr[i] = _tenbenxe;
                i++;

                dr[i] = chuyen.GioMoPhoi;
                i++;

                dr[i] = chuyen.NgayDi.ToString("HH:mm");
                i++;
                //so lenh van danh
                //var lenh = _phieuchuyenphatService.GetAllPhieuVanChuyenByChuyenId(chuyen.Id, _workContext.NhaXeId);
                dr[i] = chuyen.SoPhieuXN;
                i++;
                dr[i] = chuyen.SoLenhVD;
                i++;
                dr[i] = chuyen.SoKhachXB.GetValueOrDefault(0);
                i++;

                dr[i] = chuyen.LaiXe;
                i++;
                dr[i] = chuyen.PhuXe;
                i++;
                dr[i] = chuyen.GhiChu;
                dataReport.Rows.Add(dr);
            }

            model.dataReport = dataReport;

            return model;
        }
        public ActionResult LenhVanChuyenHangNgay()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
            {
                if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                    return AccessDeniedView();
            }

            var model = new BaoCaoNhaXeModel();
            model.NgayGuiHang = DateTime.Now;
            //loc tuyen
            prepareTuyen(model);
            //loc van phong
            PrepareListVanPhongModel(model);

            model.LoaiBaoCao = BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.PHIEU_VAN_CHUYEN_NGAY;

            return View(model);
        }


        BaoCaoNhaXeModel createLenhVanChuyenNgay(BaoCaoNhaXeModel model)
        {

            //cau hinh bao cao
            model.isShowSTT = true;
            model.topPage = GetTopPageOfReport();
            model.Title = new string[] { "BÁO CÁO TỔNG HỢP NGÀY THEO TUYẾN", string.Format("NGÀY {0} THÁNG {1} NĂM {2}", model.NgayGuiHang.Day, model.NgayGuiHang.Month, model.NgayGuiHang.Year) };
            model.TitleColSpan = new List<string[]>();
            model.addSumBottom = true;
            //add header colspan ngay
            var headers = new List<String>();
            //model.TitleColSpan.Add(new string[] { "", "3", "" });
            var tuyens = _phieuchuyenphatService.GetAllTuyenVanDoanh(_workContext.NhaXeId);
            if (model.TuyenId > 0)
            {
                tuyens = tuyens.Where(c => c.Id == model.TuyenId).ToList();
            }
            model.idxColForSum = 0;
            foreach (var tuyen in tuyens)
            {
                model.TitleColSpan.Add(new string[] { tuyen.TenTuyen, "3", "" });
                headers.Add("LVC");
                headers.Add("SỐ XE");
                headers.Add("SỐ TIỀN");
            }
            //lay thong tin ngay trong thang thong ke            
            var LenhVanChuyens = _phieuchuyenphatService.GetAllPhieuChuyenPhatVanChuyen(model.NgayGuiHang, model.NgayGuiHang, model.VanPhongId, 0, model.BienSoXe, model.SoLenh, model.TuyenId)

                .GroupBy(g => new { g.TuyenId, g.phieuvanchuyen.SoLenh, g.chuyendi.SoXe,g.phieuvanchuyen })
                .Select(c => new
                {
                    phieuvanchuyen=c.Key.phieuvanchuyen,
                    TuyenId = c.Key.TuyenId,
                    SoLenh = c.Key.SoLenh,
                    BienSo = c.Key.SoXe,
                    TongTien = c.Sum(a => a.TongCuoc)+c.Sum(a=>a.CuocVuotTuyen),
                    TongTienCuoc = c.Sum(a=>a.phieuchuyenphat.TongTienCuoc),
                    CuocPhi=c.Sum(a=>a.phieuchuyenphat.CuocPhi),
                    CuocTanNoi =c.Sum(a=>a.phieuchuyenphat.CuocTanNoi),
                    CuocVuotTuyen=c.Sum(a=>a.phieuchuyenphat.CuocVuotTuyen),
                    CuocNhanTanNoi=c.Sum(a=>a.phieuchuyenphat.CuocNhanTanNoi),
                    CuocGiaTri=c.Sum(a=>a.phieuchuyenphat.CuocGiaTri),
                    CuocCapToc=c.Sum(a=>a.phieuchuyenphat.CuocCapToc),
                    CuocVCTND=c.Sum(a=>a.phieuchuyenphat.CuocVCTND)
                    //TongTien = c.Sum(a=>a.phieuchuyenphat.CuocPhi)+c.Sum(a=>a.phieuchuyenphat.CuocVuotTuyen)
                }).ToList();
            //tao header cua bao cao
            model.headers = headers.ToArray();
            //tao du lieu trong bao cao
            DataTable dataReport = ToDataTableSimple(model.headers.Length);
            //bat dau tao data table
            while (LenhVanChuyens.Count > 0)
            {
                //tao tung row
                var dr = dataReport.NewRow();
                int col = 0;
                foreach (var tuyen in tuyens)
                {
                    //lay phan tu dau tien co tuyen =tuyen.id
                    var item = LenhVanChuyens.Where(c => c.TuyenId == tuyen.Id).FirstOrDefault();
                    if (item != null)
                    {
                        dr[col] = item.SoLenh;
                        dr[col + 1] = item.BienSo;
                        decimal cuoctuyen = 0;
                        if (item.phieuvanchuyen.LoaiPhieuVanChuyenId == (int)ENLoaiPhieuVanChuyen.TrongTuyen)
                        {
                            cuoctuyen = item.TongTienCuoc - item.CuocNhanTanNoi - item.CuocTanNoi;
                        }
                        else
                        {
                            var cuoc1 = (item.CuocGiaTri + item.CuocCapToc) / 2;
                            if(tuyen.Id==item.phieuvanchuyen.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId)
                            {
                                cuoctuyen = item.CuocPhi + cuoc1;
                            }
                            if(item.phieuvanchuyen.nhatkyvanchuyens.Count()>1)
                            {
                                if(tuyen.Id==item.phieuvanchuyen.nhatkyvanchuyens.LastOrDefault().hanhtrinh.TuyenVanDoanhId)
                                {
                                    cuoctuyen = item.CuocVuotTuyen + cuoc1;
                                }
                            }
                        }
                        //dr[col + 2] = item.TongTien.ToSoNguyenCuocPhi();
                        dr[col + 2] = cuoctuyen.ToSoNguyenCuocPhi();
                        //loai bo ra khoi danh sach, de tiep tuc tao record moi
                        LenhVanChuyens.Remove(item);
                    }
                    col = col + 3;
                }
                dataReport.Rows.Add(dr);
            }

            model.dataReport = dataReport;

            return model;
        }
        public ActionResult LenhVanChuyenHangThang()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
            {
                if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                    return AccessDeniedView();
            }

            var model = new BaoCaoNhaXeModel();
            PrepareListNgayThangNam(model);
            //loc tuyen
            prepareTuyen(model);
            //loc van phong
            PrepareListVanPhongModel(model);

            model.LoaiBaoCao = BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.PHIEU_VAN_CHUYEN_THANG;

            return View(model);
        }


        BaoCaoNhaXeModel createLenhVanChuyenThang(BaoCaoNhaXeModel model)
        {

            //cau hinh bao cao
            model.isShowSTT = true;
            model.topPage = GetTopPageOfReport();
            model.Title = new string[] { "BÁO CÁO TỔNG HỢP THÁNG THEO TUYẾN", string.Format("THÁNG {0} NĂM {1}", model.ThangId, model.NamId) };
            model.TitleColSpan = new List<string[]>();
            model.addSumBottom = true;

            //add header colspan ngay
            var headers = new List<String>();
            //model.TitleColSpan.Add(new string[] { "", "3", "" });
            var tuyens = _phieuchuyenphatService.GetAllTuyenVanDoanh(_workContext.NhaXeId);
            if (model.TuyenId > 0)
            {
                tuyens = tuyens.Where(c => c.Id == model.TuyenId).ToList();
            }
            model.TitleColSpan.Add(new string[] { "TỔNG HỢP THEO TUYẾN", tuyens.Count().ToString(), "" });
            model.idxColForSum = 0;
            foreach (var tuyen in tuyens)
            {
                headers.Add(tuyen.TenTuyen);
            }
            //lay thong tin ngay trong thang thong ke       
            DateTime dt1 = new DateTime(model.NamId, model.ThangId, 1);
            DateTime dt2 = dt1.AddMonths(1).AddDays(-1);
            var LenhVanChuyens = _phieuchuyenphatService.GetAllPhieuChuyenPhatVanChuyen(dt1, dt2, model.VanPhongId, 0, model.BienSoXe, model.SoLenh, model.TuyenId);
            //tao header cua bao cao
            model.headers = headers.ToArray();
            //tao du lieu trong bao cao
            DataTable dataReport = ToDataTableSimple(model.headers.Length);
            //bat dau tao data table
            for (var dt = dt1; dt <= dt2; dt = dt.AddDays(1))
            {
                var dr = dataReport.NewRow();
                int col = 0;
                foreach (var tuyen in tuyens)
                {
                    //lay phan tu dau tien co tuyen =tuyen.id
                    var lenhvanchuyens = LenhVanChuyens.Where(c => c.TuyenId == tuyen.Id && c.phieuchuyenphat.NgayNhanHang.Day == dt.Day)
                        .GroupBy(g => new { g.TuyenId, g.phieuvanchuyen.SoLenh, g.chuyendi.SoXe, g.phieuvanchuyen })
                        .Select(c => new
                        {
                            phieuvanchuyen = c.Key.phieuvanchuyen,
                            TuyenId = c.Key.TuyenId,
                            SoLenh = c.Key.SoLenh,
                            BienSo = c.Key.SoXe,
                            TongTien = c.Sum(a => a.TongCuoc) + c.Sum(a => a.CuocVuotTuyen),
                            TongTienCuoc = c.Sum(a => a.phieuchuyenphat.TongTienCuoc),
                            CuocPhi = c.Sum(a => a.phieuchuyenphat.CuocPhi),
                            CuocTanNoi = c.Sum(a => a.phieuchuyenphat.CuocTanNoi),
                            CuocVuotTuyen = c.Sum(a => a.phieuchuyenphat.CuocVuotTuyen),
                            CuocNhanTanNoi = c.Sum(a => a.phieuchuyenphat.CuocNhanTanNoi),
                            CuocGiaTri = c.Sum(a => a.phieuchuyenphat.CuocGiaTri),
                            CuocCapToc = c.Sum(a => a.phieuchuyenphat.CuocCapToc),
                            CuocVCTND = c.Sum(a=>a.phieuchuyenphat.CuocVCTND)
                            //TongTien = c.Sum(a=>a.phieuchuyenphat.CuocPhi)+c.Sum(a=>a.phieuchuyenphat.CuocVuotTuyen)
                        }).ToList();
                    if (lenhvanchuyens.Count > 0)
                    {
                        //dr[col] = (item.Sum(c => c.TongCuoc)+ item.Sum(c=>c.CuocVuotTuyen)).ToSoNguyenCuocPhi();
                        //dr[col] = item.Sum(c => c.phieuchuyenphat.CuocPhi).ToSoNguyenCuocPhi();
                        decimal cuoctuyenTong = 0;
                        foreach(var item in lenhvanchuyens)
                        {
                            decimal cuoctuyen=0;
                            if (item.phieuvanchuyen.LoaiPhieuVanChuyenId == (int)ENLoaiPhieuVanChuyen.TrongTuyen)
                            {
                                cuoctuyen = item.TongTienCuoc - item.CuocNhanTanNoi - item.CuocTanNoi;
                            }
                            else
                            {
                                var cuoc1 = (item.CuocGiaTri + item.CuocCapToc) / 2;
                                if (tuyen.Id == item.phieuvanchuyen.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId)
                                {
                                    cuoctuyen = item.CuocPhi + cuoc1;
                                }
                                if (item.phieuvanchuyen.nhatkyvanchuyens.Count() > 1)
                                {
                                    if (tuyen.Id == item.phieuvanchuyen.nhatkyvanchuyens.LastOrDefault().hanhtrinh.TuyenVanDoanhId)
                                    {
                                        cuoctuyen = item.CuocVuotTuyen + cuoc1;
                                    }
                                }
                            }
                            cuoctuyenTong += cuoctuyen;
                        }
                        dr[col] = cuoctuyenTong.ToSoNguyenCuocPhi();
                    }
                    col = col + 1;
                }
                dataReport.Rows.Add(dr);
            }
            model.dataReport = dataReport;

            return model;
        }

        #endregion
        #region chi tieu chuyen phat
        public ActionResult ChiTieuChuyenPhatThang()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
            {
                if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                    return AccessDeniedView();
            }

            var model = new BaoCaoNhaXeModel();
            PrepareListNgayThangNam(model);
            //loc tuyen
            prepareTuyen(model);
            //loc van phong
            PrepareListVanPhongModel(model);

            model.LoaiBaoCao = BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.CHI_TIEU_THANG;

            return View(model);
        }
        BaoCaoNhaXeModel createChiTieuThang(BaoCaoNhaXeModel model)
        {

            //cau hinh bao cao
            model.isShowSTT = true;
            model.topPage = GetTopPageOfReport();
            model.Title = new string[] { "CHỈ TIÊU CHUYỂN PHÁT", string.Format("THÁNG {0} NĂM {1}", model.ThangId, model.NamId) };
            model.TitleColSpan = new List<string[]>();
            //add header colspan ngay
            var headers = new List<String>();
            //model.TitleColSpan.Add(new string[] { "", "3", "" });
            var tuyens = _phieuchuyenphatService.GetAllTuyenVanDoanh(_workContext.NhaXeId);
            if (model.TuyenId > 0)
            {
                tuyens = tuyens.Where(c => c.Id == model.TuyenId).ToList();
            }
            model.TitleColSpan.Add(new string[] { "", "2", "" });
            headers.Add("VP");
            headers.Add("Số nhân viên");
            model.TitleColSpan.Add(new string[] { "Xếp loại", "2", "" });
            headers.Add("Xếp loại");
            headers.Add("Hàng");
            model.TitleColSpan.Add(new string[] { "Số giao dịch", "3", "" });
            headers.Add("GD nhận hàng");
            headers.Add("GD trả hàng");
            headers.Add("Tổng GD");
            model.TitleColSpan.Add(new string[] { "Số GDTB/người", "4", "" });
            headers.Add("Số GD");
            headers.Add("Xếp thứ tự");
            headers.Add("Xếp loại");
            headers.Add("Mã hóa xếp loại");
            model.TitleColSpan.Add(new string[] { "Hiệu suất/GD", "4", "" });
            headers.Add("Hiệu suất");
            headers.Add("Xếp thứ tự");
            headers.Add("Xếp loại");
            headers.Add("Mã hóa xếp loại");
            model.TitleColSpan.Add(new string[] { "", "2", "" });
            headers.Add("Doanh thu");
            headers.Add("VC TND");
            //lay thong tin ngay trong thang thong ke       
            var vanphongs = _nhaxeService.GetAllVanPhongByNhaXeId(_workContext.NhaXeId);

            //tao header cua bao cao
            model.headers = headers.ToArray();
            model.addSumBottom = true;
            //tao du lieu trong bao cao
            DataTable dataReport = ToDataTableSimple(model.headers.Length);
            //bat dau tao data table
            foreach (var item in vanphongs)
            {
                var dr = dataReport.NewRow();
                int col = 0;
                dr[col] = item.Ma;
                col = col + 4;

                var phieunhan = _phieuchuyenphatService.GetAllPhieuChuyenPhatTrongThang(_workContext.NhaXeId, item.Id, null, "", 0, null, null, model.ThangId, model.NamId);
                var GDNhan = phieunhan.Count();
                dr[col] = GDNhan.ToString();
                col++;
                var GDTra = _phieuchuyenphatService.GetAllPhieuChuyenPhatTrongThang(_workContext.NhaXeId, 0, null, "", item.Id, null, null, model.ThangId, model.NamId, null, false).Count();
                dr[col] = GDTra.ToString();
                col++;
                dr[col] = (GDTra + GDNhan).ToString();
                col = col + 9;
                //dr[col] = phieunhan.Where(c => c.LoaiPhieu == ENLoaiPhieuChuyenPhat.ThuTaiVanPhong).Sum(c => (c.TongTienCuoc - c.CuocTanNoi - c.CuocNhanTanNoi)).ToSoNguyenCuocPhi();
                //dr[col] = (phieunhan.Where(c => c.LoaiPhieu == ENLoaiPhieuChuyenPhat.ThuTaiVanPhong).Sum(c => c.CuocPhi) + phieunhan.Where(c => c.LoaiPhieu == ENLoaiPhieuChuyenPhat.ThuTaiVanPhong).Sum(c => c.CuocVuotTuyen)).ToSoNguyenCuocPhi();
                var cuocnhan = phieunhan.Where(c => c.LoaiPhieu == ENLoaiPhieuChuyenPhat.ThuTaiVanPhong).Sum(c => (c.TongTienCuoc - c.CuocTanNoi - c.CuocNhanTanNoi));
                var cuoctra = phieunhan.Where(c => c.LoaiPhieu == ENLoaiPhieuChuyenPhat.ThuTanNoi).Sum(c => (c.TongTienCuoc - c.CuocTanNoi - c.CuocNhanTanNoi));
                dr[col] = (cuocnhan+cuoctra).ToSoNguyenCuocPhi();
                col++;
                //dr[col] = phieunhan.Where(c => c.LoaiPhieu == ENLoaiPhieuChuyenPhat.ThuTanNoi).Sum(c => (c.TongTienCuoc - c.CuocTanNoi - c.CuocNhanTanNoi)).ToSoNguyenCuocPhi();
                dr[col] = phieunhan.Sum(c => c.CuocVCTND).ToSoNguyenCuocPhi();
                dataReport.Rows.Add(dr);
            }
            model.dataReport = dataReport;

            return model;
        }
        #endregion
        #region Bao cao to van chuyen van phong
        public ActionResult BaoCaoToVanChuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
            {
                if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                    return AccessDeniedView();
            }

            var model = new BaoCaoNhaXeModel();
            PrepareListNgayThangNam(model);
            //loc tuyen
            prepareTuyen(model);
            //loc van phong
            PrepareListVanPhongModel(model);

            model.LoaiBaoCao = BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.TO_VAN_CHUYEN_THANG;

            return View(model);
        }
        BaoCaoNhaXeModel createToVanchuyenThang(BaoCaoNhaXeModel model)
        {

            //cau hinh bao cao
            model.isShowSTT = true;
            model.topPage = GetTopPageOfReport();
            model.Title = new string[] { "BÁO CÁO TỔ VẬN CHUYỂN NHẬN", string.Format("THÁNG {0} NĂM {1}", model.ThangId, model.NamId) };
            model.TitleColSpan = new List<string[]>();
            model.addSumBottom = true;
            //add header colspan ngay
            var headers = new List<String>();
            headers.Add("TỔ VC");
            headers.Add("SỐ GD NHẬN");
            headers.Add("Tổng cước nhận/trả tận nơi");
            headers.Add("TỔNG CƯỚC");

            var tovanchuyen = _phieuchuyenphatService.GetAllToVanChuyen(_workContext.NhaXeId);
            foreach (var item in tovanchuyen)
            {
                headers.Add(item.TenTo);
            }

            //headers.Add("CƯỚC VƯỢT TUYẾN");



            var tuyens = _phieuchuyenphatService.GetAllTuyenVanDoanh(_workContext.NhaXeId);
            foreach (var m in tuyens)
            {
                headers.Add("Còn lại " + m.TenVietTat);
            }
            headers.Add("THỰC THU");
            //lay thong tin ngay trong thang thong ke       
            var Tovcs = _phieuchuyenphatService.GetAllToVanChuyen(_workContext.NhaXeId);
            //tao header cua bao cao
            model.headers = headers.ToArray();
            //tao du lieu trong bao cao
            DataTable dataReport = ToDataTableSimple(model.headers.Length);
            var phieuchuyenphats = _phieuchuyenphatService.GetAllPhieuChuyenPhatTrongThang(_workContext.NhaXeId, 0, null, "", 0, null, null, model.ThangId, model.NamId);
            //bat dau tao data table
            foreach (var item in Tovcs)
            {
                //get vp theo to vc
                var vps = item.tovanchuyenvps.Select(c => c.VanPhongId).ToArray();
                var dr = dataReport.NewRow();
                int col = 0;
                dr[col] = item.TenTo;
                col++;
                var phieunhan = phieuchuyenphats.Where(c => c.LoaiPhieu == ENLoaiPhieuChuyenPhat.ThuTanNoi && vps.Contains(c.VanPhongGuiId)).ToList();
                var GDNhan = phieunhan.Count();
                dr[col] = GDNhan.ToString();
                col++;
                var nhantannoi = phieunhan.Sum(c => c.CuocNhanTanNoi);
                var tratannoi = phieunhan.Sum(c => c.CuocTanNoi);
                var cuoctranhan=nhantannoi+tratannoi;
                dr[col] = cuoctranhan.ToSoNguyenCuocPhi();
                col++;
                //dr[col] = phieunhan.Sum(c => c.TongTienCuoc).ToSoNguyenCuocPhi();

                var tongcuoc = phieuchuyenphats.Where(c => c.ToVanChuyenNhanId == item.Id).Sum(c => c.CuocNhanTanNoi) + phieuchuyenphats.Where(c=>c.ToVanChuyenTraId==item.Id).Sum(c => c.CuocTanNoi);
                dr[col] = tongcuoc.ToSoNguyenCuocPhi();
                col++;
                foreach (var _item in tovanchuyen)
                {
                    var cuocvc = phieunhan.Where(c => c.ToVanChuyenNhanId == _item.Id).Sum(c => c.CuocNhanTanNoi);
                    //var cuocvctra = phieunhan.Where(c => c.ToVanChuyenTraId == _item.Id).Sum(c => c.CuocTanNoi);
                    //cuocvc = cuocvc + cuocvctra;
                    dr[col] = cuocvc.ToSoNguyenCuocPhi();
                    col++;
                }
                //cuoc vuot tuyen               
                //decimal cuocvt = phieunhan.Sum(c => c.CuocVuotTuyen);
                //dr[col] = cuocvt.ToSoNguyenCuocPhi();
                //col++;
                decimal ThucThu = 0;
                var phieuthuong = phieunhan.Where(c => c.NhomPhieu != ENNhomPhieuChuyenPhat.TN_VT);
                foreach (var m in tuyens)
                {
                    decimal cuoctuyen = 0;
                    var CuocTuyenTong = phieuthuong.Where(c => c.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId == m.Id).Sum(c => c.TongTienCuoc);
                    var CuocPhi = phieuthuong.Where(c => c.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId == m.Id).Sum(c => c.CuocPhi);
                    var CuocTuyenVC = phieuthuong.Where(c => c.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId == m.Id).Sum(c => c.CuocTanNoi);
                    var CuocTuyenVCNhan = phieuthuong.Where(c => c.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId == m.Id).Sum(c => c.CuocNhanTanNoi);
                    var PhieuVT = phieunhan.Where(c => c.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId == m.Id && c.NhomPhieu == ENNhomPhieuChuyenPhat.TN_VT);
                    cuoctuyen = CuocTuyenTong - CuocTuyenVC - CuocTuyenVCNhan;
                    decimal cuoctuyenvt = 0;
                    if (PhieuVT.Count() > 0)
                    {
                        var CuocPhiVT = PhieuVT.Where(c => c.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId == m.Id).Sum(c => c.CuocPhi);
                        var CuocVT = PhieuVT.Where(c => c.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId == m.Id).Sum(c => c.CuocVuotTuyen);
                        var CuocCapToc = PhieuVT.Where(c => c.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId == m.Id).Sum(c => c.CuocCapToc);
                        var CuocTriGia = PhieuVT.Where(c => c.nhatkyvanchuyens.FirstOrDefault().hanhtrinh.TuyenVanDoanhId == m.Id).Sum(c => c.CuocGiaTri);
                        // cuoc tuyen 1= cuocphi-cuocvanchuyen+ 1/2 (cuoccaptoc+cuocgiatri)                      
                        cuoctuyenvt = CuocPhiVT + (CuocCapToc + CuocTriGia) / 2;
                        // cuoc tuyen 2
                        if (PhieuVT.Any(c => c.nhatkyvanchuyens.Count() > 1 && c.nhatkyvanchuyens.LastOrDefault().hanhtrinh.TuyenVanDoanhId == m.Id))
                        {
                            cuoctuyenvt = CuocVT + (CuocCapToc + CuocTriGia) / 2;
                        }
                    }
                    cuoctuyen = cuoctuyen + cuoctuyenvt;
                    dr[col] = cuoctuyen.ToSoNguyenCuocPhi();
                    col++;
                    ThucThu = ThucThu + cuoctuyen;
                }
                dr[col] = ThucThu.ToSoNguyenCuocPhi();
                dataReport.Rows.Add(dr);
            }
            model.dataReport = dataReport;
            return model;
        }
        #endregion
        #region Báo cáo khách hàng tiềm năng
        public ActionResult BaoCaoKhachHangTiemNang()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
            {
                return AccessDeniedView();
            }
            var model = new BaoCaoNhaXeModel();
            //loc van phong
            PrepareListVanPhongModel(model, true, false);
            //loc khu vuc
            PrepareListKhuVucModel(model);
            model.TuNgay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            model.DenNgay = model.TuNgay.AddMonths(1).AddDays(-1);
            model.ListLoai1 = GetListBaoCaoKHTiemNangTheoKV();
            model.ListLoai2 = GetListBaoCaoKHTiemNangTheoNguoiGuiNhan();
            model.ListLoai3 = GetListBaoCaoKHTiemNangSapXep();
            model.LoaiBaoCao = BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.BAO_CAO_KHACH_HANG_GUI;

            return View(model);
        }
        BaoCaoNhaXeModel createBaoCaoKhachHangTiemNang(BaoCaoNhaXeModel model)
        {
            model.isShowSTT = true;
            model.topPage = GetTopPageOfReport();
            var title = "";
            if (model.Loai2Id == (int)BaoCaoKhachHangTiemNangTheoNguoiGuiNhan.THEO_NGUOI_GUI)
            {
                title = "Báo cáo theo văn phòng nhận hàng";
            }
            else
            {
                title = "Báo cáo theo văn phòng trả hàng";
            }
            if (model.Loai1Id == (int)BaoCaoKhachHangTiemNangTheoKV.THEO_VAN_PHONG)
            {
                var vanphong = _nhaxeService.GetVanPhongById(model.VanPhongId);
                model.Title = new string[] { 
                    title,
                    "Văn phòng " + vanphong.TenVanPhong,
                    "Báo cáo danh sách khách hàng từ ngày "+model.TuNgay.ToString("dd/MM/yyyy") +" đến ngày "+ model.DenNgay.ToString("dd/MM/yyyy") };
            }
            else
            {
                var khuvuc = _phieuchuyenphatService.GetKhuVucById(model.KhuVucId);
                model.Title = new string[] {
                    title,
                    "Tỉnh. TP(Khu vực) " + khuvuc.TenKhuVuc,
                    "Báo cáo danh sách khách hàng từ ngày "+model.TuNgay.ToString("dd/MM/yyyy") +" đến ngày "+ model.DenNgay.ToString("dd/MM/yyyy") };
            }
            model.TitleColSpan = new List<string[]>();
            var headers = new List<String>();

            model.TitleColSpan.Add(new string[] { "", "2", "" });
            if (model.Loai2Id == (int)BaoCaoKhachHangTiemNangTheoNguoiGuiNhan.THEO_NGUOI_GUI)
            {
                headers.Add("Người gửi");
            }
            else
            {
                headers.Add("Người nhận");
            }
            headers.Add("Địa chỉ - SĐT");
            model.TitleColSpan.Add(new string[] { "Số giao dịch", "2", "" });
            headers.Add("Số lần GD");
            headers.Add("Xếp thứ");
            model.TitleColSpan.Add(new string[] { "Số tiền", "3", "" });
            headers.Add("Đã TT(đầu gửi TT)");
            headers.Add("Chưa TT(Đầu nhận TT)");
            headers.Add("Xếp thứ");
            model.TitleColSpan.Add(new string[] { "", "2", "" });
            headers.Add("Tỉnh TP trả hàng");
            headers.Add("Loại hàng gửi");
            model.headers = headers.ToArray();

            DataTable dataReport = ToDataTableSimple(model.headers.Length);
            //lay thong tin phieu chuyen phat theo van phong 
            var phieuchuyenphats = new List<PhieuChuyenPhat>();
            if (model.Loai1Id == (int)BaoCaoKhachHangTiemNangTheoKV.THEO_VAN_PHONG)
            {
                if (model.Loai2Id == (int)BaoCaoKhachHangTiemNangTheoNguoiGuiNhan.THEO_NGUOI_GUI)
                    phieuchuyenphats = _phieuchuyenphatService.GetAllPhieuChuyenPhat(_workContext.NhaXeId, model.VanPhongId, null, "", ENTrangThaiChuyenPhat.All, 0, 0, model.TuNgay, model.DenNgay);
                else
                    phieuchuyenphats = _phieuchuyenphatService.GetAllPhieuChuyenPhat(_workContext.NhaXeId, 0, null, "", ENTrangThaiChuyenPhat.All, 0, model.VanPhongId, model.TuNgay, model.DenNgay);
            }
            else
            {
                var listVanPhong = _nhaxeService.GetlAllVanPhongByKhuVucId(model.KhuVucId);
                foreach (var item in listVanPhong)
                {
                    if (model.Loai2Id == (int)BaoCaoKhachHangTiemNangTheoNguoiGuiNhan.THEO_NGUOI_GUI)
                    {
                        var list = _phieuchuyenphatService.GetAllPhieuChuyenPhat(_workContext.NhaXeId, item.Id, null, "", ENTrangThaiChuyenPhat.All, 0, 0, model.TuNgay, model.DenNgay);
                        phieuchuyenphats.AddRange(list);
                    }
                    else
                    {
                        var list = _phieuchuyenphatService.GetAllPhieuChuyenPhat(_workContext.NhaXeId, 0, null, "", ENTrangThaiChuyenPhat.All, 0, item.Id, model.TuNgay, model.DenNgay);
                        phieuchuyenphats.AddRange(list);
                    }

                }
            }
            var khachhangs = new List<BaoCaoNhaXeModel.BaoCaoKhachHangTiemNangModel>();
            if (model.Loai2Id == (int)BaoCaoKhachHangTiemNangTheoNguoiGuiNhan.THEO_NGUOI_GUI)
            {
                //lay thong tin khach hang trong phieu chuyen phat
                khachhangs = phieuchuyenphats.GroupBy(c => new { c.NguoiGui }).Select(g => new BaoCaoNhaXeModel.BaoCaoKhachHangTiemNangModel
                {
                    KhachHangId = g.Key.NguoiGui.Id,
                    HoVaTen = g.Key.NguoiGui.HoTen,
                    DiaChi = g.Key.NguoiGui.DiaChi,
                    SoDienThoai = g.Key.NguoiGui.SoDienThoai,
                    SoLanGD = g.Count(),
                    SoTienDaTT = g.Sum(c => c.TongCuocDaThanhToan),
                    TongSoTien = g.Sum(c => (c.CuocPhi + c.CuocTanNoi + c.CuocNhanTanNoi + c.CuocCapToc + c.CuocGiaTri + c.CuocVuotTuyen))
                }).ToList();


                int ThuTuSoLanGD = 1;
                foreach (var item in khachhangs.OrderByDescending(c => c.SoLanGD).ToList())
                {
                    item.ThuTuSoLanGD = ThuTuSoLanGD;
                    ThuTuSoLanGD++;
                }

                int ThuTuTienTT = 1;
                foreach (var item in khachhangs.OrderByDescending(c => c.TongSoTien).ToList())
                {
                    item.ThuTuTienTT = ThuTuTienTT;
                    ThuTuTienTT++;
                }
            }
            else
            {
                //lay thong tin khach hang trong phieu chuyen phat
                khachhangs = phieuchuyenphats.GroupBy(c => new { c.NguoiNhan }).Select(g => new BaoCaoNhaXeModel.BaoCaoKhachHangTiemNangModel
                {
                    KhachHangId = g.Key.NguoiNhan.Id,
                    HoVaTen = g.Key.NguoiNhan.HoTen,
                    DiaChi = g.Key.NguoiNhan.DiaChi,
                    SoDienThoai = g.Key.NguoiNhan.SoDienThoai,
                    SoLanGD = g.Count(),
                    SoTienDaTT = g.Sum(c => c.TongCuocDaThanhToan),
                    TongSoTien = g.Sum(c => (c.CuocPhi + c.CuocTanNoi + c.CuocNhanTanNoi + c.CuocCapToc + c.CuocGiaTri + c.CuocVuotTuyen))
                }).ToList();


                int ThuTuSoLanGD = 1;
                foreach (var item in khachhangs.OrderByDescending(c => c.SoLanGD).ToList())
                {
                    item.ThuTuSoLanGD = ThuTuSoLanGD;
                    ThuTuSoLanGD++;
                }

                int ThuTuTienTT = 1;
                foreach (var item in khachhangs.OrderByDescending(c => c.TongSoTien).ToList())
                {
                    item.ThuTuTienTT = ThuTuTienTT;
                    ThuTuTienTT++;
                }
            }
            if (model.Loai3Id == (int)BaoCaoKhachHangTiemNangSapXep.SO_LAN_GD)
                khachhangs = khachhangs.OrderBy(c => c.ThuTuSoLanGD).ToList();
            else
                khachhangs = khachhangs.OrderBy(c => c.ThuTuTienTT).ToList();
            foreach (var item in khachhangs)
            {

                var dr = dataReport.NewRow();
                int col = 0;
                dr[col] = item.HoVaTen;
                col++;
                dr[col] = string.Format("{0} - {1}", item.DiaChi, item.SoDienThoai);
                col++;
                dr[col] = item.SoLanGD;
                col++;
                dr[col] = item.ThuTuSoLanGD;
                col++;
                dr[col] = item.SoTienDaTT.ToSoNguyenCuocPhi();
                col++;
                dr[col] = item.SoTienChuaTT.ToSoNguyenCuocPhi();
                col++;
                dr[col] = item.ThuTuTienTT;
                col++;
                dr[col] = "";
                col++;
                dr[col] = "";
                dataReport.Rows.Add(dr);
            }

            model.dataReport = dataReport;
            return model;
        }
        #endregion
        #region Báo cáo tin nhắn đã gửi
        public ActionResult BaoCaoTinNhanDaGui()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
            {
                return AccessDeniedView();
            }
            var model = new BaoCaoNhaXeModel();
            //loc van phong
            model.TuNgay = DateTime.Now.AddDays(-1);
            model.DenNgay = DateTime.Now;
            model.LoaiBaoCao = BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.BAO_CAO_SMS_GUI;
            return View(model);
        }
        BaoCaoNhaXeModel createBaoCaoSMSGui(BaoCaoNhaXeModel model)
        {
            model.isShowSTT = true;
            model.topPage = GetTopPageOfReport();
            var title = "Báo cáo SMS đã gửi";
            var vanphong=_nhaxeService.GetVanPhongById(model.VanPhongId);
            model.Title = new string[]{
                            title,
                            "Báo cáo danh sách SMS đã gửi từ ngày "+model.TuNgay.ToString("dd/MM/yyyy")+" đến ngày "+model.DenNgay.ToString("dd/MM/yyyy")};
            var headers = new List<String>();
            headers.Add("Ngày gửi");
            var vanphongs = _nhaxeService.GetAllVanPhongByNhaXeId(_workContext.NhaXeId);
            foreach(var item in vanphongs)
            {
                headers.Add(item.Ma);
            }
            headers.Add("Tổng");
            model.headers = headers.ToArray();
            model.addSumBottom = true;
            model.addSumRight = false;
            DataTable dataReport = ToDataTableSimple(model.headers.Length);
            //lay cac phieu chuyen phat log gui tin nhan o van phong nhan
            for (var dt = model.TuNgay; dt <= model.DenNgay;dt=dt.AddDays(1))
            {
                var dr = dataReport.NewRow();
                int col = 0;
                dr[col] = dt.ToString("dd-MM-yyyy");
                col++;
                var phieuchuyenphatlogs = _phieuchuyenphatService.GetAllPhieuChuyenPhatLog(dt);
                foreach (var item in vanphongs)
                {
                    var sotinnhan = phieuchuyenphatlogs.Where(c=>c.phieuchuyenphat.VanPhongNhanId==item.Id).Count();                   
                    dr[col] = sotinnhan.ToString();
                    col++;
                }
                dr[col] = phieuchuyenphatlogs.Count();
                dataReport.Rows.Add(dr);
            }
            model.dataReport = dataReport;    
            return model;
        }
        #endregion
        #region doanh thu hang hoa theo van phong hang thang
        public ActionResult BaoCaoDoanhThuThang()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVBaoCao))
                return AccessDeniedView();
            var model = new BaoCaoNhaXeModel();
            //loc van phong
            PrepareListVanPhongModel(model, false, true);
            model.TuNgay = DateTime.Now.NgayDauThang();
            //model.TuNgay = DateTime.Now;
            model.DenNgay = DateTime.Now.AddDays(1);
            model.LoaiBaoCao = BaoCaoNhaXeModel.EN_LOAI_BAO_CAO.BAO_CAO_DOANH_THU_THANG;
            return View(model);
        }


        BaoCaoNhaXeModel createBaoCaoDoanhThuThang(BaoCaoNhaXeModel model)
        {

            //cau hinh bao cao
            model.isShowSTT = true;
            model.topPage = GetTopPageOfReport();
            model.Title = new string[] { "BÁO CÁO DOANH THU TỪ NGÀY " + model.TuNgay.ToString("dd/MM/yyyy") + "ĐẾN NGÀY" + model.DenNgay.ToString("dd/MM/yyyy") };

            //lay thong tin ngay trong thang thong ke
            //tao header cua bao cao
            var headers = new List<String>();
            model.TitleColSpan = new List<string[]>();
            model.TitleColSpan.Add(new string[] { "", "1", "" });
            headers.Add("Ngày tháng");
            var vanphongs = _nhaxeService.GetAllVanPhongByNhaXeId(_workContext.NhaXeId);
            foreach(var item in vanphongs)
            {
                model.TitleColSpan.Add(new string[] { item.Ma, "2", "" });
                headers.Add("Tổng Bill");
                headers.Add("Tổng doanh thu");

            }
            model.TitleColSpan.Add(new string[] { "", "1", "" });
            headers.Add("Tổng");
            model.headers = headers.ToArray();
            model.addSumBottom = true;
            var giaodichs = _phieuchuyenphatService.GetAllPhieuChuyenPhatTrongThang(_workContext.NhaXeId,0, null, model.KeySearch, 0, model.TuNgay, model.DenNgay, 0, 0, null, true,0);
            //tao du lieu trong bao cao
            DataTable dataReport = ToDataTableSimple(model.headers.Length);
            var Days = Enumerable.Range(0, model.DenNgay.Subtract(model.TuNgay).Days + 1)
                     .Select(d => model.TuNgay.AddDays(d));
            var arrDay = Days.ToList();
            foreach (var m in arrDay)
            {
                var ngaynhanhang=m.Date;
                var _giaodichs = giaodichs.Where(c => c.NgayNhanHang == ngaynhanhang);
                var dr = dataReport.NewRow();
                int i = 0;
                dr[i] = ngaynhanhang.ToString("yyyy-MM-dd");
                i++;
                foreach(var item in vanphongs)
                {
                    var gd = _giaodichs.Where(c => c.VanPhongGuiId == item.Id);
                    dr[i] = gd.Count();
                    i++;
                    dr[i] = gd.Sum(c => c.TongTienCuoc).ToSoNguyenCuocPhi();
                    i++;
                }
                dr[i] = _giaodichs.Sum(c => c.TongTienCuoc).ToSoNguyenCuocPhi();
                dataReport.Rows.Add(dr);
            }  
                       
            model.dataReport = dataReport;

            return model;
        }
        
        #endregion
    }
}