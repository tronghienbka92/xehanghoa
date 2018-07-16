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
using Nop.Web.Models.ChuyenPhatNhanh;
using Nop.Core.Domain.ChuyenPhatNhanh;
using Nop.Services.ChuyenPhatNhanh;
using RestSharp;
using System.Web.Script.Serialization;
using Newtonsoft.Json;


namespace Nop.Web.Controllers
{
    public class HangHoaController : BaseNhaXeController
    {
        #region Khoi Tao
        private readonly IStateProvinceService _stateProvinceService;
        private readonly INhaXeService _nhaxeService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IPictureService _pictureService;
        private readonly ICustomerService _customerService;
        private readonly IDiaChiService _diachiService;
        private readonly INhanVienService _nhanvienService;
        private readonly IPermissionService _permissionService;
        private readonly IXeInfoService _xeinfoService;
        private readonly IHanhTrinhService _hanhtrinhService;
        private readonly IBenXeService _benxeService;
        private readonly INhaXeCustomerService _nhaxecustomerService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPhieuChuyenPhatService _phieuchuyenphatService;
        public HangHoaController(IStateProvinceService stateProvinceService,
            INhaXeService nhaxeService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IPictureService pictureService,
            ICustomerService customerService,
            IDiaChiService diachiService,
            INhanVienService nhanvienService,
            IPermissionService permissionService,
            IXeInfoService xeinfoService,
            IHanhTrinhService hanhtrinhService,
            IBenXeService benxeService,
            INhaXeCustomerService nhaxecustomerService,
            IPriceFormatter priceFormatter,
            CustomerSettings customerSettings,
            ICustomerRegistrationService customerRegistrationService,
            IGenericAttributeService genericAttributeService,
             IPhieuChuyenPhatService phieuchuyenphatService
            )
        {
            this._genericAttributeService = genericAttributeService;
            this._customerRegistrationService = customerRegistrationService;
            this._customerSettings = customerSettings;
            this._priceFormatter = priceFormatter;
            this._stateProvinceService = stateProvinceService;
            this._nhaxeService = nhaxeService;
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
            this._phieuchuyenphatService = phieuchuyenphatService;
        }
        #endregion

        #region ham chung
        public string SendSMS(string to, string text)
        {
            //var client = new RestClient("http://api-02.worldsms.vn/webapi/sendSMS");
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("postman-token", "3d0ccb9d-7893-03e3-f9ff-c600027bfea1");
            //request.AddHeader("cache-control", "no-cache");
            //request.AddHeader("authorization", "Basic dm1nYXBpdGVzdDpzdDEyMzk4Nw==");
            //request.AddHeader("content-type", "application/x-www-form-urlencoded");
            //object item = new
            //{
            //    from = "X.e VietNam",
            //    to = "841657556243",
            //    text = "kiem tra ket noi sms"
            //};
            //request.AddJsonBody(item);
            //IRestResponse response = client.Execute(request);
            //return response.Content;
            try
            {
                object item = new
                {
                    from = "X.e VietNam",
                    to = to,
                    text = text
                };

                var jsonSMS = JsonConvert.SerializeObject(item);

                var client = new RestClient("http://api-02.worldsms.vn/webapi/sendSMS");
                var request = new RestRequest(Method.POST);
                //request.AddHeader("postman-token", "3d0ccb9d-7893-03e3-f9ff-c600027bfea1");
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("authorization", "Basic eGV2aWV0bmFtOkU2eTZrczhTOGc=");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", jsonSMS, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                var tmpObj = JsonConvert.DeserializeObject<object>(response.Content);
                return response.Content;
            }
            catch (Exception ex)
            {
                var tmpErr = ex.Message;
                throw ex;
            }
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetDllPhieuVanChuyen(ListPhieuModel model)
        {
            var phieuvanchuyens = _phieuchuyenphatService.GetAllPhieuVanChuyen(_workContext.NhaXeId, model.VanPhongGuiId, "", ENTrangThaiPhieuVanChuyen.Moi, model.NgayTao);
            model.phieuvanchuyens = phieuvanchuyens.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.LoaiPhieuVanChuyen.ToCVEnumText(_localizationService) + ": " + c.SoLenh + "(" + c.KhuVucDen.TenKhuVuc + ")"
            }).ToList();
            model.phieuvanchuyens.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "-------Chưa xếp phiếu-----------"
            });
            return Json(model.phieuvanchuyens, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CbbListKhachHangInNhaXe(string SearchKhachhang)
        {
            var khachhangs = _phieuchuyenphatService.GetAllKhachHang(_workContext.NhaXeId, SearchKhachhang);
            return Json(khachhangs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CBBNhanVienNhaXe(string hoten, int? VanPhongId = 0)
        {
            var nhanviens = _nhanvienService.GetAllForGiaoDichKeVe(VanPhongId.GetValueOrDefault(0), hoten, _workContext.NhaXeId).Select(c =>
            {
                var item = new CustomerNhaXeModel();
                item.Id = c.Id;
                item.HoTen = c.ThongTin(false);
                return item;
            }).ToList();

            return Json(nhanviens, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CBBVanPhong(string TenVanPhong)
        {
            var vanphongs = _nhaxeService.GetAllVanPhongByNhaXeId(_workContext.NhaXeId).Select(c =>
            {
                var item = new VanPhongModel();
                item.Id = c.Id;
                item.TenVanPhong = c.TenVanPhong;
                return item;
            }).ToList();

            return Json(vanphongs, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CBBBienSoXe(string BienSo)
        {
            var biensos = _xeinfoService.GetAllXeVanChuyenByNhaXeId(_workContext.NhaXeId,BienSo).Where(c => c.TrangThaiXeId != (int)ENTrangThaiXe.Huy).Select(c =>
            {
                var item = new XeInfoModel();
                item.Id = c.Id;
                if(BienSo.Length==2)
                {
                    string str = c.BienSo.Substring(c.BienSo.Length - 2);
                    if(str==BienSo)
                    {
                        item.isDisable = false;
                    }
                    else
                    {
                        item.isDisable = true;
                    }
                    item.BienSo = c.BienSo;
                    if (c.laixe != null)
                        item.LaiXeText = c.laixe.HoVaTen;
                    return item;
                }
                item.BienSo = c.BienSo;
                if (c.laixe != null)
                    item.LaiXeText = c.laixe.HoVaTen;
                return item;
            }).ToList();
            biensos = biensos.Where(c => !c.isDisable).ToList();
            return Json(biensos, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CBBChuyenDi(string chuyendi, int VanPhongGuiId, int VanPhongNhanId, string NgayDi)
        {
            DateTime _ngaydi = DateTime.Parse(NgayDi).Date;
            var ddlchuyendis = _phieuchuyenphatService.GetAllChuyenDi(_workContext.NhaXeId, VanPhongGuiId, VanPhongNhanId, _ngaydi).Select(c =>
            {
                var item = new CustomerNhaXeModel();
                item.Id = c.Id;
                item.ChuyenDi = c.toMoTa();
                return item;
            }).ToList();
            return Json(ddlchuyendis, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CBBNhanVienDBNhaXe(string hoten, int? VanPhongId = 0)
        {
            var nhanviens = _phieuchuyenphatService.GetLaiPhuSoXe(DateTime.Now.Month, DateTime.Now.Year, LoaiLaiPhuSoXe.PHU_XE, hoten).Select(c =>
            {
                var item = new CustomerNhaXeModel();
                item.Id = c.Id;
                item.HoTen = c.Ten;
                return item;
            }).ToList();

            return Json(nhanviens, JsonRequestBehavior.AllowGet);
        }
        private string GetTrangThai(PhieuVanChuyen item)
        {
            return "(" + item != null ? item.TrangThai.GetLocalizedEnum(_localizationService, _workContext) : "NULL" + ")";
        }
        private string GetTrangThai(PhieuChuyenPhat item)
        {
            return "(" + item != null ? item.TrangThai.GetLocalizedEnum(_localizationService, _workContext) : "NULL" + ")";
        }

        #endregion

        #region  gui hang
        /// <summary>
        /// giao dien chinh gom 2 partial view: _PhieuChuyenPhatChinhSua, _PhieuChuyenPhatList
        /// </summary>
        /// <returns></returns>
        public ActionResult QLGuiHang()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
            {
                if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
                {
                    return AccessDeniedView();
                }
                return RedirectToAction("BangDieuChuyen", "HangHoa");
            }

            var model = new ListPhieuModel();
            model.NgayTao = DateTime.Now;
            model.TenVanPhongHienTai = _workContext.CurrentVanPhong.TenVanPhong;
            model.VanPhongGuiId = _workContext.CurrentVanPhong.Id;
            return View(model);
        }
        public ActionResult _ChiTietPhieuBienNhan(int Id)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var model = new PhieuChuyenPhatModel();
            var phieuchuyenphat = _phieuchuyenphatService.GetPhieuChuyenPhatById(Id);
            if (phieuchuyenphat != null)
                model = phieuchuyenphat.ToModel(_localizationService, _priceFormatter);
            model.nhatkys = phieuchuyenphat.nhatkys.ToList();
            return PartialView(model);
        }
        /// <summary>
        /// neu id=null, tao moi phieu
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult _PhieuChuyenPhatChinhSua(int? Id)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var model = new PhieuChuyenPhatModel();
            if (Id > 0)
            {
                //chinh sua phieu gui
                var phieuchuyenphat = _phieuchuyenphatService.GetPhieuChuyenPhatById(Id.Value);
                model = phieuchuyenphat.ToModel(_localizationService, _priceFormatter);
                model.TinhChatHangSelected = _phieuchuyenphatService.GetPhieuChuyenPhatTinhChatHangPCPId(Id.Value).Select(c => c.TinhChatHangId).ToArray();
                model.LoaiHangSelected = _phieuchuyenphatService.GetPhieuChuyenPhatLoaiHangPCPId(Id.Value).Select(c => c.LoaiHangId).ToArray();
            }

            else
            {
                //tao moi
                model.TenNhanvienGiaoDich = _workContext.CurrentNhanVien.HoVaTen;
                model.NhanVienGiaoDichId = _workContext.CurrentNhanVien.Id;
                model.VanPhongGuiId = _workContext.CurrentVanPhong.Id;
                model.VanPhongGuiText = _workContext.CurrentVanPhong.TenVanPhong;

            }
            model.LoaiHangNhanTraTanNoi = this.GetCVEnumSelectList<ENHangNhanTraTanNoi>(_localizationService);
            model.LoaiHangGiaTri = this.GetCVEnumSelectList<ENLoaiHangGiaTri>(_localizationService);
            model.TinhChatHangs = this.GetCVEnumSelectList<ENTinhChatHang>(_localizationService);
            model.LoaiHangs = this.GetCVEnumSelectList<ENLoaiHangKhongKhaiGiaTri>(_localizationService);
            PhieuChuyenPhatPrepareModel(model);
            return PartialView(model);
        }
        [NonAction]
        protected virtual void PhieuChuyenPhatPrepareModel(PhieuChuyenPhatModel model)
        {

            //ha xuong đon vi 1000 d
            model.CuocPhi = model.CuocPhi;
            model.CuocCapToc = model.CuocCapToc;
            model.CuocGiaTri = model.CuocGiaTri;
            model.CuocTanNoi = model.CuocTanNoi;
            model.CuocNhanTanNoi = model.CuocNhanTanNoi;
            model.CuocVuotTuyen = model.CuocVuotTuyen;
            model.TongCuocDaThanhToan = model.TongCuocDaThanhToan;

            model.loaiphieus = this.GetCVEnumSelectList<ENLoaiPhieuChuyenPhat>(_localizationService, model.LoaiPhieuId, false);
            //Danh sách văn phòng không chứa văn phòng hiện tại
            var khuvucs = _phieuchuyenphatService.GetAllKhuVuc(_workContext.NhaXeId);
            model.khuvucvanphongs = khuvucs.Select(c =>
            {
                var kv = new KhuVucVanPhongModel();
                kv.Id = c.Id;
                kv.TenKhuVuc = c.TenKhuVuc;
                kv.vanphongs = c.vanphongs.Where(p => !p.isDelete).Select(v =>
                {
                    var vp = new VanPhongModel();
                    vp.Id = v.Id;
                    vp.TenVanPhong = v.TenVanPhong;
                    vp.DienThoaiGuiHang = v.DienThoaiGuiHang;
                    return vp;

                }).ToList();
                return kv;

            }).ToList();

        }

        [NonAction]
        protected virtual void PhieuChuyenPhatModelToPhieuChuyenPhat(PhieuChuyenPhat nvto, PhieuChuyenPhatModel nvfrom)
        {
            //nvto.LoaiPhieuId = nvfrom.LoaiPhieuId;
            nvto.LoaiPhieu = nvfrom.CuocNhanTanNoi > 0 ? ENLoaiPhieuChuyenPhat.ThuTanNoi : ENLoaiPhieuChuyenPhat.ThuTaiVanPhong;
            nvto.VanPhongNhanId = nvfrom.VanPhongNhanId;
            nvto.NguoiGuiId = nvfrom.NguoiGuiId;
            nvto.NguoiNhanId = nvfrom.NguoiNhanId;
            nvto.TenHang = nvfrom.TenHang;
            nvto.CuocPhi = nvfrom.CuocPhi;
            nvto.CuocCapToc = nvfrom.CuocCapToc;
            nvto.CuocGiaTri = nvfrom.CuocGiaTri;
            nvto.CuocTanNoi = nvfrom.CuocTanNoi;
            nvto.CuocNhanTanNoi = nvfrom.CuocNhanTanNoi;
            nvto.CuocVuotTuyen = nvfrom.CuocVuotTuyen;
            //nvto.CuocVCTND = nvfrom.CuocVCTND * 1000m;
            nvto.TongCuocDaThanhToan = nvfrom.TongCuocDaThanhToan;
            nvto.NgayNhanHang = nvfrom.NgayNhanHang;
            nvto.GhiChu = nvfrom.GhiChu;
            nvto.ToVanChuyenNhanId = null;
            if (nvfrom.ToVanChuyenNhanId > 0)
                nvto.ToVanChuyenNhanId = nvfrom.ToVanChuyenNhanId;
            nvto.NguoiVanChuyenNhanId = null;
            if (nvfrom.NguoiVanChuyenNhanId > 0)
                nvto.NguoiVanChuyenNhanId = nvfrom.NguoiVanChuyenNhanId;
            if (nvfrom.ToVanChuyenTraId > 0)
                nvto.ToVanChuyenTraId = nvfrom.ToVanChuyenTraId;
            nvto.NguoiVanChuyenTraId = null;
            if (nvfrom.NguoiVanChuyenTraId > 0)
                nvto.NguoiVanChuyenTraId = nvfrom.NguoiVanChuyenTraId;

        }
        KhachHang CapNhatKhachHang(string TenKhachHang, string SoDienThoai, string DiaChiLienHe, int KhachHangId = 0)
        {
            if (KhachHangId > 0)
            {
                var khachhangupd = _phieuchuyenphatService.GetKhachHangById(KhachHangId);
                khachhangupd.HoTen = TenKhachHang;
                khachhangupd.SoDienThoai = SoDienThoai;
                khachhangupd.DiaChi = DiaChiLienHe;
                _phieuchuyenphatService.UpdateKhachHang(khachhangupd);
                return khachhangupd;
            }

            //insert bang khach hang
            var khachhang = new KhachHang();
            khachhang.HoTen = TenKhachHang;
            khachhang.SoDienThoai = SoDienThoai;
            khachhang.DiaChi = DiaChiLienHe;
            khachhang.NhaXeId = _workContext.NhaXeId;
            _phieuchuyenphatService.InsertKhachHang(khachhang);
            return khachhang;
        }
        [HttpPost]
        public ActionResult PhieuChuyenPhatChinhSua(PhieuChuyenPhatModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            if (ModelState.IsValid)
            {
                //add by mai: kiem tra viec chon van phong vuot tuyen

                //var vpvuottuyen = _phieuchuyenphatService.GetVanPhongVuotTuyenByVanPhongNhan(_workContext.CurrentVanPhong.Id, model.VanPhongNhanId);
                //if (vpvuottuyen != null && model.CuocVuotTuyen == 0)
                //{
                //    return Json("Chưa nhập cước vượt tuyến", JsonRequestBehavior.AllowGet);
                //}
                //if (vpvuottuyen == null && model.CuocVuotTuyen > 0)
                //{
                //    return Json("Không phải trường hợp vượt tuyến", JsonRequestBehavior.AllowGet);
                //}
                //cap nhat thong tin to van chuyen nhan
                if (model.CuocNhanTanNoi > 0)
                {
                    var _tovc = _workContext.CurrentVanPhong.tovanchuyens.Select(c => c.tovanchuyen).FirstOrDefault();

                    if (_tovc != null)
                    {
                        model.ToVanChuyenNhanId = _tovc.Id;
                        var _nvc = _tovc.nguoivanchuyens.FirstOrDefault();
                        //neu ko co thi lay item dau tien
                        if (_nvc != null)
                        {
                            model.NguoiVanChuyenNhanId = _nvc.Id;
                        }
                    }
                }

                //set up thong tin to van chuyen tra
                if (model.CuocTanNoi > 0)
                {
                    var _tovctra = _nhaxeService.GetVanPhongById(model.VanPhongNhanId).tovanchuyens.Select(c => c.tovanchuyen).FirstOrDefault();

                    if (_tovctra != null)
                    {
                        model.ToVanChuyenTraId = _tovctra.Id;
                        var _nvc = _tovctra.nguoivanchuyens.FirstOrDefault();
                        //neu ko co thi lay item dau tien
                        if (_nvc != null)
                        {
                            model.NguoiVanChuyenTraId = _nvc.Id;
                        }
                    }
                }

                //cap nhat thong tin nguoi gui
                var nguoigui = CapNhatKhachHang(model.NguoiGui.HoTen, model.NguoiGui.SoDienThoai, model.NguoiGui.DiaChi, model.NguoiGui.Id);
                // cap nhat thong tin nguoi nhan
                var nguoinhan = CapNhatKhachHang(model.NguoiNhan.HoTen, model.NguoiNhan.SoDienThoai, model.NguoiNhan.DiaChi, model.NguoiNhan.Id);
                model.NguoiGuiId = nguoigui.Id;
                model.NguoiNhanId = nguoinhan.Id;


                if (model.Id > 0)
                {
                    //chinh sua phieu gui
                    var phieuchuyenphat = _phieuchuyenphatService.GetPhieuChuyenPhatById(model.Id);
                    //neu phieu da in thi khong dc phep sua
                    if (phieuchuyenphat.DaIn > 0)
                        return Loi("Phiếu đã in không được phép chỉnh sửa");
                    PhieuChuyenPhatModelToPhieuChuyenPhat(phieuchuyenphat, model);
                    _phieuchuyenphatService.UpdatePhieuChuyenPhat(phieuchuyenphat);
                    _phieuchuyenphatService.InsertPhieuChuyenPhatLog("Phiếu được sửa bởi " + _workContext.CurrentNhanVien.HoVaTen, phieuchuyenphat.Id);
                
                }
                else
                {
                    //tao moi
                    var phieuchuyenphat = new PhieuChuyenPhat();
                    PhieuChuyenPhatModelToPhieuChuyenPhat(phieuchuyenphat, model);
                    phieuchuyenphat.NhaXeId = _workContext.NhaXeId;
                    phieuchuyenphat.NhanVienGiaoDichId = _workContext.CurrentNhanVien.Id;
                    phieuchuyenphat.VanPhongGuiId = _workContext.CurrentVanPhong.Id;
                    phieuchuyenphat.TrangThai = ENTrangThaiChuyenPhat.Moi;
                    _phieuchuyenphatService.InsertPhieuChuyenPhat(phieuchuyenphat);
                    _phieuchuyenphatService.InsertPhieuChuyenPhatLog("Phiếu được tạo bởi " + _workContext.CurrentNhanVien.HoVaTen, phieuchuyenphat.Id);
                    
                    //insert thong tin hang
                    var j = new JavaScriptSerializer();
                    var thongtinhans = new List<PhieuChuyenPhatModel.PhieuChuyenPhatThongTinHangModel>();
                    thongtinhans = j.Deserialize<List<PhieuChuyenPhatModel.PhieuChuyenPhatThongTinHangModel>>(model.ThongTinHang);
                    foreach (var item in thongtinhans)
                    {
                        var phieuchuyenphatthongtinhang = new PhieuChuyenPhatThongTinHang();
                        phieuchuyenphatthongtinhang.PhieuChuyenPhatId = phieuchuyenphat.Id;
                        phieuchuyenphatthongtinhang.TenHang = item.TenHang;
                        phieuchuyenphatthongtinhang.SoLuong = item.SoLuong;
                        phieuchuyenphatthongtinhang.GiaTien = item.GiaTien;
                        _phieuchuyenphatService.InertPhieuChuyenPhatThongTinHang(phieuchuyenphatthongtinhang);
                    }
                    model.Id = phieuchuyenphat.Id;
                }
                //tinh chat hang
                var tinhchathangs = this.GetCVEnumSelectList<ENTinhChatHang>(_localizationService);
                List<int> TinhChatHangSelectedInt=new List<int>();
                if(!string.IsNullOrEmpty(model.SelectedTinhChatHang))
                {
                    string[] TinhChatHangSelectedString = model.SelectedTinhChatHang.Split(',');
                    foreach (var _id in TinhChatHangSelectedString)
                    {
                        if (!string.IsNullOrEmpty(_id))
                        {
                            int id;
                            if (int.TryParse(_id, out id))
                            {
                                TinhChatHangSelectedInt.Add(id);
                            }
                        }
                    }

                    foreach (var kv in tinhchathangs)
                    {

                        if (TinhChatHangSelectedInt.Count() > 0 && TinhChatHangSelectedInt.ToArray().Contains(Convert.ToInt32(kv.Value)))
                        {
                            var htkv = _phieuchuyenphatService.GetPhieuChuyenPhatTinhChatHang(model.Id, Convert.ToInt32(kv.Value));
                            //new role
                            if (htkv == null)
                            {
                                var m = new PhieuChuyenPhatTinhChatHang();
                                m.PhieuChuyenPhatId = model.Id;
                                m.TinhChatHangId = Convert.ToInt32(kv.Value);
                                _phieuchuyenphatService.InsertPhieuChuyenPhatTinhChatHang(m);
                            }
                        }
                        else
                        {
                            var htkv = _phieuchuyenphatService.GetPhieuChuyenPhatTinhChatHang(model.Id, Convert.ToInt32(kv.Value));
                            //remove role
                            if (htkv != null)
                            {
                                _phieuchuyenphatService.DeletePhieuChuyenPhatTinhChatHang(htkv);
                            }

                        }
                    }
                }
                
                //loai hang
                var loaihangs = this.GetCVEnumSelectList<ENLoaiHangKhongKhaiGiaTri>(_localizationService);
                List<int> LoaiHangSelectedInt = new List<int>();
                if(!string.IsNullOrEmpty(model.SelectedLoaiHang))
                {
                    string[] LoaiHangSelectedString = model.SelectedLoaiHang.Split(',');
                    foreach (var _id in LoaiHangSelectedString)
                    {
                        if (!string.IsNullOrEmpty(_id))
                        {
                            int id;
                            if (int.TryParse(_id, out id))
                            {
                                LoaiHangSelectedInt.Add(id);
                            }
                        }
                    }

                    foreach (var kv in loaihangs)
                    {

                        if (LoaiHangSelectedInt.Count() > 0 && LoaiHangSelectedInt.Contains(Convert.ToInt32(kv.Value)))
                        {
                            var htkv = _phieuchuyenphatService.GetPhieuChuyenPhatLoaiHang(model.Id, Convert.ToInt32(kv.Value));
                            //new role
                            if (htkv == null)
                            {
                                var m = new PhieuChuyenPhatLoaiHang();
                                m.PhieuChuyenPhatId = model.Id;
                                m.LoaiHangId = Convert.ToInt32(kv.Value);
                                _phieuchuyenphatService.InsertPhieuChuyenPhatLoaiHang(m);
                            }
                        }
                        else
                        {
                            var htkv = _phieuchuyenphatService.GetPhieuChuyenPhatLoaiHang(model.Id, Convert.ToInt32(kv.Value));
                            //remove role
                            if (htkv != null)
                            {
                                _phieuchuyenphatService.DeletePhieuChuyenPhatLoaiHang(htkv);
                            }

                        }
                    }
                }            
            }

            return ThanhCong();
        }

        public ActionResult PhieuChuyenPhatXoa(int Id)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var phieugui = _phieuchuyenphatService.GetPhieuChuyenPhatById(Id);
            if (phieugui == null)
                return Loi();
            //if (phieugui.TrangThai != ENTrangThaiChuyenPhat.Moi)
            //{
            //    return Loi();
            //}
            if (!_workContext.CurrentNhanVien.isAdmin)
            {
                if (phieugui.TrangThaiId == (int)ENTrangThaiChuyenPhat.DenVanPhongNhan || phieugui.TrangThaiId == (int)ENTrangThaiChuyenPhat.KetThuc)
                    return Json("Không thể xóa", JsonRequestBehavior.AllowGet);
            }

            phieugui.TrangThaiId = (int)ENTrangThaiChuyenPhat.Huy;
            _phieuchuyenphatService.UpdatePhieuChuyenPhat(phieugui);
            _phieuchuyenphatService.InsertPhieuChuyenPhatLog("Phiếu được xóa bởi " + _workContext.CurrentNhanVien.HoVaTen, phieugui.Id);
            return ThanhCong();
        }

        public ActionResult PhieuChuyenPhatDelete(int Id)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var phieugui = _phieuchuyenphatService.GetPhieuChuyenPhatById(Id);
            if (phieugui == null)
                return Loi();
            //if (phieugui.TrangThai != ENTrangThaiChuyenPhat.Moi)
            //{
            //    return Loi();
            //}
            if (!_workContext.CurrentNhanVien.isAdmin)
            {
                return Json("Không thể xóa", JsonRequestBehavior.AllowGet);
            }

            phieugui.TrangThaiId = (int)ENTrangThaiChuyenPhat.Huy;
            _phieuchuyenphatService.UpdatePhieuChuyenPhat(phieugui);
            _phieuchuyenphatService.InsertPhieuChuyenPhatLog("Phiếu được xóa bởi " + _workContext.CurrentNhanVien.HoVaTen, phieugui.Id);
            return ThanhCong();
        }
        /// <summary>
        /// danh sach phieu chuyen phat, chon phieu gui -> load partialview _PhieuChuyenPhatChinhSua
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _PhieuChuyenPhatList(ListPhieuModel model)
        {

            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            if (model.PhieuVanChuyenId > 0)
                model.NgayTao = null;
            if (_workContext.CurrentNhanVien.isAdmin)
                model.VanPhongGuiId = 0;
            var items = _phieuchuyenphatService.GetAllPhieuChuyenPhat(_workContext.NhaXeId, model.VanPhongGuiId, model.NgayTao, model.HangHoaInfo, model.TrangThai, model.PhieuVanChuyenId, model.VanPhongNhanId, model.TuNgay, model.DenNgay).OrderByDescending(c => c.VanPhongNhanId).ToList();
            if (model.VanPhongId > 0)
                items = items.Where(c => c.VanPhongNhanId == model.VanPhongId).ToList();
            //loai bo nhung phieu neu co trong Phieu van chuyen
            if (model.OutPhieuVanChuyenId > 0)
            {
                var itemexcludes = _phieuchuyenphatService.GetAllPhieuChuyenPhat(_workContext.NhaXeId, 0, null, "", ENTrangThaiChuyenPhat.All, model.OutPhieuVanChuyenId).Select(c => c.Id).ToArray();
                if (itemexcludes.Length > 0)
                {
                    items = items.Where(c => !itemexcludes.Contains(c.Id)).ToList();
                }
                //chi lay cac phieu co van phong nhan nam trong khu vuc cua phieu van chuyen
                var phieuvanchuyen = _phieuchuyenphatService.GetPhieuVanChuyenById(model.OutPhieuVanChuyenId);
                var vanhphongnhanids = phieuvanchuyen.KhuVucDen.vanphongs.Select(c => c.Id).ToList();
                items = items.Where(c => vanhphongnhanids.Contains(c.VanPhongNhanId)).ToList();
            }
            //chi lay nhung chuyen phat co diem cuoi la van phong hien tai, truong truong hop tra hang
            if (model.TrangThaiId == (int)ENTrangThaiChuyenPhat.DenVanPhongNhan)
            {
                items = items.Where(c => c.VanPhongNhanId == _workContext.CurrentVanPhong.Id).ToList();
            }
            if (model.isXepPhieu)
            {
                //sap xep theo tung vung
                //items = items.OrderBy(c => c.VanPhongNhan.KhuVucId).ThenByDescending(c => c.Id).ToList();
            }
            if (model.isCoCuocTanNoi)
            {
                //lay nhung phieu co cuoc tan noi
                items = items.Where(c => c.CuocTanNoi > 0).ToList();
            }


            var _item = items.Select(x =>
            {

                var m = x.ToModel(_localizationService, _priceFormatter);
                return m;
            }).ToList();
            if (!string.IsNullOrEmpty(model.ThongTinChuyen))
            {
                _item = _item.Where(c => c.ThongTinXe != null && c.ThongTinXe.Contains(model.ThongTinChuyen)).ToList();
            }
            var gridModel = new DataSourceResult
            {
                Data = _item,
                Total = _item.Count
            };

            return Json(gridModel);
        }
        [HttpPost]
        public ActionResult _PhieuChuyenPhatPageList(DataSourceRequest command, ListPhieuModel model)
        {

            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            if (model.PhieuVanChuyenId > 0)
                model.NgayTao = null;
            //var items = _phieuchuyenphatService.GetAllPhieuChuyenPhat(_workContext.NhaXeId, model.VanPhongGuiId, model.NgayTao, model.HangHoaInfo, model.TrangThai, model.PhieuVanChuyenId, model.VanPhongNhanId, model.TuNgay, model.DenNgay);
            var items = _phieuchuyenphatService.GetAllPhieuChuyenPhatPageList(_workContext.NhaXeId, model.VanPhongGuiId, model.NgayTao, model.HangHoaInfo, model.TrangThai, model.PhieuVanChuyenId, model.VanPhongNhanId, model.TuNgay, model.DenNgay, command.Page - 1, command.PageSize);
            //loai bo nhung phieu neu co trong Phieu van chuyen           
            var _item = items.Select(x =>
            {

                var m = x.ToModel(_localizationService, _priceFormatter);
                return m;
            }).ToList();
            if (!string.IsNullOrEmpty(model.ThongTinChuyen))
            {
                _item = _item.Where(c => c.ThongTinXe != null && c.ThongTinXe.Contains(model.ThongTinChuyen)).ToList();
            }
            var gridModel = new DataSourceResult
            {
                Data = _item,
                Total = _item.Count
            };

            return Json(gridModel);
        }
        public ActionResult _ToVanChuyenNhanHang(int ToVanChuyenNhanId, int NguoiVanChuyenNhanId)
        {
            var model = new ToVanChuyenModel();
            model.ToVanChuyenIdSelect = ToVanChuyenNhanId;
            model.NguoiVanChuyenId = NguoiVanChuyenNhanId;
            var _tovc = _workContext.CurrentVanPhong.tovanchuyens.Select(c => c.tovanchuyen).Where(c => c.Id == model.ToVanChuyenIdSelect).FirstOrDefault();
            //neu ko co thi lay item dau tien
            if (_tovc == null)
            {
                _tovc = _workContext.CurrentVanPhong.tovanchuyens.Select(c => c.tovanchuyen).FirstOrDefault();
            }
            if (_tovc != null)
            {
                model.ToVanChuyenIdSelect = _tovc.Id;
                var _nvc = _tovc.nguoivanchuyens.Where(c => c.Id == model.NguoiVanChuyenId).FirstOrDefault();
                //neu ko co thi lay item dau tien
                if (_nvc == null)
                {
                    _nvc = _tovc.nguoivanchuyens.FirstOrDefault();
                }
                if (_nvc != null)
                {
                    model.NguoiVanChuyenId = _nvc.Id;
                }
                model.nguoivanchuyens = _tovc.nguoivanchuyens.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.HoVaTen + "(" + c.DienThoai + ")",
                    Selected = c.Id == model.NguoiVanChuyenId
                }).ToList();
            }
            model.tovanchuyens = _workContext.CurrentVanPhong.tovanchuyens.Select(c => c.tovanchuyen).Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.TenTo + " - " + c.MoTa,
                Selected = c.Id == model.ToVanChuyenIdSelect
            }).ToList();

            return PartialView(model);
        }
        #endregion
        #region xep phieu

        /// <summary>
        /// nghiep vu xep phieu: gom 3 partialview: danh sach phieu chuyen phat moi, danh sach cac phieu van chuyen,
        /// danh sach phieu chuyen phat thuoc phieu van chuyen
        /// chon phieu van chuyen -> load thong tin cac phieu chuyen phat thuoc phieu van chuyen da chon
        /// </summary>
        /// <returns></returns>
        public ActionResult QLXepPhieu()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var model = new ListPhieuModel();
            model.NgayTao = DateTime.Now;
            model.TenVanPhongHienTai = _workContext.CurrentVanPhong.TenVanPhong;
            model.VanPhongGuiId = _workContext.CurrentVanPhong.Id;
            return View(model);
        }
        public ActionResult QLXepPhieuNew()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var model = new ListPhieuModel();
            model.TenVanPhongHienTai = _workContext.CurrentVanPhong.TenVanPhong;
            model.VanPhongGuiId = _workContext.CurrentVanPhong.Id;
            model.NgayTao = DateTime.Now;
            //lay thong tin phieu van chuyen moi trong ngay
            var phieuvanchuyens = _phieuchuyenphatService.GetAllPhieuVanChuyen(_workContext.NhaXeId, model.VanPhongGuiId, "", ENTrangThaiPhieuVanChuyen.Moi, model.NgayTao);
            model.phieuvanchuyens = phieuvanchuyens.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.LoaiPhieuVanChuyen.ToCVEnumText(_localizationService) + ": " + c.SoLenh + "(" + c.KhuVucDen.TenKhuVuc + ")"
            }).ToList();
            model.phieuvanchuyens.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "-------Chưa xếp phiếu-----------"
            });
            return View(model);
        }
        public ActionResult QLPhieuVanChuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var model = new ListPhieuModel();
            model.NgayTao = DateTime.Now;
            model.TenVanPhongHienTai = _workContext.CurrentVanPhong.TenVanPhong;
            model.VanPhongGuiId = _workContext.CurrentVanPhong.Id;
            return View(model);
        }
        [HttpPost]
        public ActionResult _PhieuVanChuyenList(ListPhieuModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var phieuvanchuyens = _phieuchuyenphatService.GetAllPhieuVanChuyen(_workContext.NhaXeId, model.VanPhongGuiId, model.SoLenh, model.TrangThaiVanChuyen, model.NgayTao, model.VanPhongNhanId, model.TuNgay, model.DenNgay);
            if (!String.IsNullOrEmpty(model.TrangThaiVanChuyenIds))
            {
                int[] trangthaiids = Array.ConvertAll(model.TrangThaiVanChuyenIds.Split(','), s => int.Parse(s));
                phieuvanchuyens = phieuvanchuyens.Where(p => trangthaiids.Contains(p.TrangThaiId)).ToList();
            }
            var items = phieuvanchuyens
                .Select(c =>
                {
                    var m = c.ToModel(_localizationService, _priceFormatter, true);
                    return m;
                }).OrderByDescending(c => c.Id).ToList();
            var gridModel = new DataSourceResult
            {
                Data = items,
                Total = items.Count
            };

            return Json(gridModel);
        }

        /// <summary>
        /// xep phieu chuyen phat vao phieu van chuyen
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult XepPhieu(int PhieuChuyenPhatId, int PhieuVanChuyenId)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var phieuchuyenphat = _phieuchuyenphatService.GetPhieuChuyenPhatById(PhieuChuyenPhatId);
            if (phieuchuyenphat != null && phieuchuyenphat.TrangThai == ENTrangThaiChuyenPhat.Moi && phieuchuyenphat.PhieuVanChuyenId == null)
            {
                phieuchuyenphat.PhieuVanChuyenId = PhieuVanChuyenId;
                phieuchuyenphat.TrangThai = ENTrangThaiChuyenPhat.DaXepLenh;
                _phieuchuyenphatService.UpdatePhieuChuyenPhat(phieuchuyenphat);
                _phieuchuyenphatService.InsertPhieuChuyenPhatLog("Phiếu được chuyển trạng thái thành: Đã xếp lệnh bởi " + _workContext.CurrentNhanVien.HoVaTen, phieuchuyenphat.Id);
                return ThanhCong();
            }
            return Loi();
        }
        [HttpPost]
        public ActionResult XepNhieuPhieu(string PhieuChuyenPhatIds, int PhieuVanChuyenId)
        {
            if (!String.IsNullOrEmpty(PhieuChuyenPhatIds))
            {
                string[] arrId = PhieuChuyenPhatIds.Split(',');
                foreach (var s in arrId)
                {
                    int phieuid = Convert.ToInt32(s);
                    XepPhieu(phieuid, PhieuVanChuyenId);
                }
            }
            return ThanhCong();
        }
        [HttpPost]
        public ActionResult HuyXepPhieu(int PhieuChuyenPhatId, int PhieuVanChuyenId)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var phieuchuyenphat = _phieuchuyenphatService.GetPhieuChuyenPhatById(PhieuChuyenPhatId);
            if (phieuchuyenphat != null && phieuchuyenphat.TrangThai == ENTrangThaiChuyenPhat.DaXepLenh && phieuchuyenphat.PhieuVanChuyenId.HasValue && PhieuVanChuyenId == phieuchuyenphat.PhieuVanChuyenId)
            {
                phieuchuyenphat.PhieuVanChuyenId = null;
                phieuchuyenphat.TrangThai = ENTrangThaiChuyenPhat.Moi;
                _phieuchuyenphatService.UpdatePhieuChuyenPhat(phieuchuyenphat);
                return ThanhCong();
            }
            return Loi();
        }
        public ActionResult _PhieuVanChuyenChinhSua(int Id, string phieuids = null)
        {
            var model = new PhieuVanChuyenModel();
            if (Id > 0)
            {
                var phieuvanchuyen = _phieuchuyenphatService.GetPhieuVanChuyenById(Id);
                model = phieuvanchuyen.ToModel(_localizationService, _priceFormatter, false);
            }
            else
            {
                int solenh = _phieuchuyenphatService.GetSoLenhVanChuyenTiepTheo(_workContext.NhaXeId, _workContext.CurrentVanPhong.Id);
                model.SoLenh = string.Format("{0}-{1}", _workContext.CurrentVanPhong.Ma, solenh);
                model.phieuchuyenphatids = phieuids;
                //lay thong tin khu vu cua phieu dau tien
                if (!String.IsNullOrEmpty(model.phieuchuyenphatids))
                {
                    string[] arrId = model.phieuchuyenphatids.Split(',');
                    int phieuchuyenphatid = Convert.ToInt32(arrId[0]);
                    var phieuchuyenphat = _phieuchuyenphatService.GetPhieuChuyenPhatById(phieuchuyenphatid);
                    if (phieuchuyenphat != null && phieuchuyenphat.VanPhongNhan != null)
                    {
                        model.KhuVucDenId = phieuchuyenphat.VanPhongNhan.KhuVucId.GetValueOrDefault(0);
                        //kiem tra thong tin phieu vuot tuyen
                        var vpvuottuyen = _phieuchuyenphatService.GetVanPhongVuotTuyenByVanPhongNhan(phieuchuyenphat.VanPhongGuiId, phieuchuyenphat.VanPhongNhanId);
                        if (vpvuottuyen != null)
                        {
                            model.LoaiPhieuVanChuyen = ENLoaiPhieuVanChuyen.VuotTuyen;
                        }
                    }
                    //
                    for (int i = 0; i < arrId.Length; i++)
                    {
                        int id = Convert.ToInt32(arrId[i]);
                        var phieuchuyenphat1 = _phieuchuyenphatService.GetPhieuChuyenPhatById(id);
                        var ngaytao = phieuchuyenphat1.NgayTao;
                        if (ngaytao.AddDays(1).Day == DateTime.Now.Day)
                        {
                            phieuchuyenphat1.NgayNhanHang = phieuchuyenphat1.NgayNhanHang.AddDays(1);
                            _phieuchuyenphatService.UpdatePhieuChuyenPhat(phieuchuyenphat1);
                        }
                    }

                }
            }
            model.loaiphieus = this.GetCVEnumSelectList<ENLoaiPhieuVanChuyen>(_localizationService, model.LoaiPhieuVanChuyenId, false);
            model.khuvucdens = _phieuchuyenphatService.GetAllKhuVuc(_workContext.NhaXeId).Select(c =>
            {
                var item = new SelectListItem();
                item.Text = c.TenKhuVuc;
                item.Value = c.Id.ToString();
                item.Selected = c.Id == model.KhuVucDenId;
                return item;
            }).ToList();
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult _PhieuVanChuyenChinhSua(PhieuVanChuyenModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var phieuvanchuyen = new PhieuVanChuyen();
            phieuvanchuyen.NhaXeId = _workContext.NhaXeId;
            phieuvanchuyen.TrangThai = ENTrangThaiPhieuVanChuyen.Moi;
            phieuvanchuyen.VanPhongId = _workContext.CurrentVanPhong.Id;
            if (model.Id > 0)
            {
                phieuvanchuyen = _phieuchuyenphatService.GetPhieuVanChuyenById(model.Id);
            }
            phieuvanchuyen.SoLenh = model.SoLenh;
            phieuvanchuyen.KhuVucDenId = model.KhuVucDenId;
            phieuvanchuyen.LoaiPhieuVanChuyen = model.LoaiPhieuVanChuyen;
            phieuvanchuyen.NgayTao = DateTime.Now;
            int _solenhnum;
            string _solenhtext = model.SoLenh.Replace("-", "").Replace(_workContext.CurrentVanPhong.Ma, "").Trim();
            if (!int.TryParse(_solenhtext, out _solenhnum))
            {
                //neu convert ko dung dang so thi tim so tiep theo
                _solenhnum = _phieuchuyenphatService.GetSoLenhVanChuyenTiepTheo(_workContext.NhaXeId, _workContext.CurrentVanPhong.Id);
            }
            phieuvanchuyen.SoLenhNum = _solenhnum;
            if (model.Id > 0)
                _phieuchuyenphatService.UpdatePhieuVanChuyen(phieuvanchuyen);
            else
            {
                _phieuchuyenphatService.InsertPhieuVanChuyen(phieuvanchuyen);
                //update phieu chuyen phat
                if (!String.IsNullOrEmpty(model.phieuchuyenphatids))
                {
                    string[] arrId = model.phieuchuyenphatids.Split(',');
                    foreach (var s in arrId)
                    {
                        int phieuid = Convert.ToInt32(s);
                        XepPhieu(phieuid, phieuvanchuyen.Id);
                    }
                }
            }


            return Json(phieuvanchuyen.Id, JsonRequestBehavior.AllowGet); ;
        }
        /// <summary>
        /// xoa vơi dieu kien khong co gi
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _XoaPhieuVanChuyen(int Id)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var phieuvanchuyen = _phieuchuyenphatService.GetPhieuVanChuyenById(Id);

            if (phieuvanchuyen != null && phieuvanchuyen.TrangThai == ENTrangThaiPhieuVanChuyen.Moi)
            {
                if (phieuvanchuyen.phieuchuyenphats == null || phieuvanchuyen.phieuchuyenphats.Count == 0)
                {
                    _phieuchuyenphatService.DeletePhieuVanChuyen(phieuvanchuyen);
                    return ThanhCong();
                }
                return Loi("Đã được xếp phiếu chuyển phát ");
            }
            return Loi("Phiếu không hợp lệ hoặc phiếu đang ở trạng thái không được phép hủy " + GetTrangThai(phieuvanchuyen));
        }
        public ActionResult _PhieuVanChuyenChinhSuaChuyenDi(int Id)
        {
            var model = new PhieuVanChuyenModel();
            var phieuvanchuyen = _phieuchuyenphatService.GetPhieuVanChuyenById(Id);
            if (phieuvanchuyen == null)
                return PartialView(null);
            model = phieuvanchuyen.ToModel(_localizationService, _priceFormatter, false);
            model.NgayDi = DateTime.Now;
            //tao nhat ky chuyen di
            model.NhatKyVanChuyenHienTai.VanPhongGuiId = _workContext.CurrentVanPhong.Id;
            model.NhatKyVanChuyenHienTai.NguoiGiaoId = _workContext.CurrentNhanVien.Id;
            model.NhatKyVanChuyenHienTai.NguoiGiaoText = _workContext.CurrentNhanVien.HoVaTen;
            //lay van phong nhan dau tien trong khu vuc cua phieu van chuyen

            var vpnhans = phieuvanchuyen.KhuVucDen.vanphongs;
            if (phieuvanchuyen.LoaiPhieuVanChuyen == ENLoaiPhieuVanChuyen.VuotTuyen)
            {
                vpnhans = vpnhans.Where(c => c.Id != _workContext.CurrentVanPhong.Id).ToList();
                if (phieuvanchuyen.KhuVucDen.Id == 4)
                    //nếu là khu vực nam định thái bình, thì văn phòng nhận là bến xe thái bình id=104
                    model.NhatKyVanChuyenHienTai.VanPhongNhanId = 104;
                else
                    model.NhatKyVanChuyenHienTai.VanPhongNhanId = vpnhans.First().Id;
            }
            else //neu la phieu cung tuyen thi lay van phong nhan dau tien cua phieu bien nhan
            {
                model.NhatKyVanChuyenHienTai.VanPhongNhanId = phieuvanchuyen.phieuchuyenphats.First().VanPhongNhanId;
            }





            if (model.nhatkyvanchuyens.Count > 0)
            {
                var _nklast = model.nhatkyvanchuyens.Last();
                //neu van phong hien tai la van phong tao thong tin chuyen di nay thi gan cho nhat ky chuyen di hien tai
                if (_nklast.VanPhongGuiId == _workContext.CurrentVanPhong.Id)
                {
                    model.NhatKyVanChuyenHienTai = _nklast;
                }
            }
            //lay danh sach bien so xe
            model.biensos = _xeinfoService.GetAllXeVanChuyenByNhaXeId(_workContext.NhaXeId,"").Where(c => c.TrangThaiXeId != (int)ENTrangThaiXe.Huy).Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.BienSo,
                Selected = c.Id == model.NhatKyVanChuyenHienTai.XeId
            }).ToList();
            //lay danh sach lai xe
            model.laixes = _nhanvienService.GetAllLaiXePhuXeByNhaXeId(_workContext.NhaXeId).Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.HoVaTen,
                Selected = c.Id == model.NhatKyVanChuyenHienTai.LaiXeId
            }).ToList();

            //lay cac van phong thuoc cung tuyen voi van phong dang thao tac
            var vanphongids = _phieuchuyenphatService.GetAllVanPhongByVanPhongId(_workContext.NhaXeId, model.NhatKyVanChuyenHienTai.VanPhongGuiId).Select(c => c.Id).ToArray();
            //lay thong tin van phong theo khu vuc, va loc van phong tren tuyen chay
            var ddlvpnhans = phieuvanchuyen.KhuVucDen.vanphongs.Where(c => vanphongids.Contains(c.Id) && c.Id != _workContext.CurrentVanPhong.Id).OrderByDescending(c => c.Id).Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = string.Format("{0}({1})", c.TenVanPhong, c.DienThoaiGuiHang),
                Selected = c.Id == model.NhatKyVanChuyenHienTai.VanPhongNhanId
            }).ToList();
            if (ddlvpnhans.Count == 0)
            {
                //khong tim dc van phong cung khu vuc voi van phong can chuyen toi, 
                //thi lay tat ca van phong cung tuyen cho nhan vien lua chon can chuyen den
                ddlvpnhans = _phieuchuyenphatService.GetAllVanPhongByVanPhongId(_workContext.NhaXeId, model.NhatKyVanChuyenHienTai.VanPhongGuiId).Where(p => p.Id != _workContext.CurrentVanPhong.Id).Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = string.Format("{0}({1})", c.TenVanPhong, c.DienThoaiGuiHang),
                    Selected = c.Id == model.NhatKyVanChuyenHienTai.VanPhongGuiId
                }).ToList();

            }
            ddlvpnhans.Insert(0, new SelectListItem { Text = "----Chọn văn phòng nhận-----", Value = "0" });
            model.vanphongnhans = ddlvpnhans;
            //lay thong tin 
            //lay thong tin chuyen di theo phieu van chuyen

            var ddlchuyendis = _phieuchuyenphatService.GetAllChuyenDi(_workContext.NhaXeId, model.NhatKyVanChuyenHienTai.VanPhongGuiId
                , model.NhatKyVanChuyenHienTai.VanPhongNhanId, phieuvanchuyen.NgayTao.Date)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.toMoTa(),
                    Selected = c.Id == model.NhatKyVanChuyenHienTai.ChuyenDiId
                }).ToList();
            ddlchuyendis.Insert(0, new SelectListItem { Text = "----Chọn chuyến đi-----", Value = "0" });
            model.chuyendis = ddlchuyendis;
            return PartialView(model);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult getChuyenDiByPhieuVanChuyen(int PhieuVanChuyenId, int VanPhongGuiId, int VanPhongNhanId, string NgayDi)
        {
            var _ngaydi = DateTime.Parse(NgayDi).Date;
            var phieuvanchuyen = _phieuchuyenphatService.GetPhieuVanChuyenById(PhieuVanChuyenId);
            var ddlchuyendis = _phieuchuyenphatService.GetAllChuyenDi(_workContext.NhaXeId, VanPhongGuiId, VanPhongNhanId, _ngaydi).Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.toMoTa(),
            }).ToList();
            ddlchuyendis.Insert(0, new SelectListItem { Text = "----Chọn chuyến đi-----", Value = "0" });
            return Json(ddlchuyendis, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult getPhuXeChuyenDi(int ChuyenDiId)
        {
            var chuyendi = _nhaxeService.GetHistoryXeXuatBenId(ChuyenDiId);
            if (chuyendi != null)
            {
                var phuxes = _phieuchuyenphatService.GetLaiPhuSoXe(chuyendi.NgayDi.Month, chuyendi.NgayDi.Year, LoaiLaiPhuSoXe.PHU_XE, chuyendi.PhuXe).FirstOrDefault();
                return Json(new
                {
                    NhanVienId = phuxes != null ? phuxes.Id : 0,
                    HoVaTen = chuyendi.PhuXe
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                NhanVienId = 0,
                HoVaTen = ""
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult _PhieuVanChuyenChinhSuaChuyenDi(PhieuVanChuyenModel model)
        {
            //thiet dat thong tin van chuyen
            var phieuvanchuyen = _phieuchuyenphatService.GetPhieuVanChuyenById(model.Id);
            if (phieuvanchuyen == null || phieuvanchuyen.TrangThai == ENTrangThaiPhieuVanChuyen.KetThuc)
                return Loi("Phiếu vận chuyển không tồn tại hoặc ở trạng thái không hợp lệ " + GetTrangThai(phieuvanchuyen));
            //var chuyendi = _nhaxeService.GetHistoryXeXuatBenId(model.NhatKyVanChuyenHienTai.ChuyenDiId);
            // if (chuyendi == null)
            //       return Loi("Chuyến đi không tồn tại");
            if (model.NhatKyVanChuyenHienTai.Id > 0)
            {
                _phieuchuyenphatService.DeletePhieuVanChuyenLog(model.NhatKyVanChuyenHienTai.Id);
            }
            //tao moi chuyen di
            if (model.NhatKyVanChuyenHienTai.ChuyenDiId == 0)
            {
                var historyxexuatben = new HistoryXeXuatBen();
                //historyxexuatben.NguonVeId = model.NguonVeId;
                //var laixe = _nhanvienService.GetById(model.NhatKyVanChuyenHienTai.LaiXeId);
                //historyxexuatben.LaiXe = laixe.HoVaTen;
                var soxe = _xeinfoService.GetXeInfoById(model.NhatKyVanChuyenHienTai.XeId);
                historyxexuatben.SoXe = soxe.BienSo;
                historyxexuatben.NgayDi = DateTime.Now;
                historyxexuatben.NhaXeId = _workContext.NhaXeId;
                historyxexuatben.NgayTao = DateTime.Now;
                historyxexuatben.TrangThai = ENTrangThaiXeXuatBen.CHO_XUAT_BEN;
                historyxexuatben.NguoiTaoId = _workContext.CurrentNhanVien.Id;

                var diemdon = _hanhtrinhService.GetDiemDonByByVanPhongId(model.NhatKyVanChuyenHienTai.VanPhongGuiId);
                var hanhtrinhdiemdon = _hanhtrinhService.GetHanhTrinhDiemDonByDiemDonId(diemdon.Id);
                historyxexuatben.HanhTrinhId = hanhtrinhdiemdon.hanhtrinh.Id;
                //historyxexuatben.NgayDi = model.NgayDi;
                // historyxexuatben.HanhTrinhId = model.HanhTrinhId;
                // historyxexuatben.SoKhachXB = model.SoKhachXB;
                //   historyxexuatben.SoLenhVD = model.SoLenhVD;
                //   historyxexuatben.SoPhieuXN = model.SoPhieuXN;
                //  historyxexuatben.GioMoPhoi = model.GioMoPhoi;
                //   historyxexuatben.GhiChu = model.GhiChu;
                _nhaxeService.InsertHistoryXeXuatBen(historyxexuatben);
                model.NhatKyVanChuyenHienTai.ChuyenDiId = historyxexuatben.Id;
            }
            var chuyendi = _nhaxeService.GetHistoryXeXuatBenId(model.NhatKyVanChuyenHienTai.ChuyenDiId);
            //update ngaynhanhang cua phieuchuyenphat la ngay xe chay
            //add by HienNT
            if (model.Id > 0)
            {
                var phieuchuyenphats = _phieuchuyenphatService.GetPhieuChuyenPhatByPhieuVanChuyenId(model.Id);
                var ngaynhanhang = chuyendi.NgayDi.Date;
                foreach (var item in phieuchuyenphats)
                {
                    item.NgayNhanHang = ngaynhanhang;
                    _phieuchuyenphatService.UpdatePhieuChuyenPhat(item);
                }
            }
            var phieuvanchuyenlog = _phieuchuyenphatService.GetPhieuVanChuyenLog(phieuvanchuyen.Id, chuyendi.Id);
            if (phieuvanchuyenlog == null)
                phieuvanchuyenlog = new PhieuVanChuyenLog();
            phieuvanchuyenlog.PhieuVanChuyenId = phieuvanchuyen.Id;
            phieuvanchuyenlog.ChuyenDiId = model.NhatKyVanChuyenHienTai.ChuyenDiId;
            phieuvanchuyenlog.HanhTrinhId = chuyendi.HanhTrinhId;
            phieuvanchuyenlog.KhuVucId = phieuvanchuyen.KhuVucDenId;
            phieuvanchuyenlog.NguoiGiaoId = model.NhatKyVanChuyenHienTai.NguoiGiaoId;
            //phieuvanchuyenlog.NguoiNhanId = model.NhatKyVanChuyenHienTai.NguoiNhanId;
            phieuvanchuyenlog.TenNguoiNhan = model.NhatKyVanChuyenHienTai.NguoiNhanText;
            phieuvanchuyenlog.TongCuoc = phieuvanchuyen.phieuchuyenphats.Sum(c => c.TongCuocDaThanhToan);
            //phieuvanchuyenlog.TuyenId = chuyendi.HanhTrinh.TuyenVanDoanhId;
            phieuvanchuyenlog.VanPhongGuiId = model.NhatKyVanChuyenHienTai.VanPhongGuiId;
            phieuvanchuyenlog.VanPhongNhanId = model.NhatKyVanChuyenHienTai.VanPhongNhanId;
            phieuvanchuyenlog.XeId = model.NhatKyVanChuyenHienTai.XeId;
            phieuvanchuyenlog.LaiXeId = model.NhatKyVanChuyenHienTai.LaiXeId;
            _phieuchuyenphatService.UpdatePhieuVanChuyenLog(phieuvanchuyenlog);
            //cap nhat thong tin trang thai phieu van chuyen
            phieuvanchuyen.TrangThai = ENTrangThaiPhieuVanChuyen.DangVanChuyen;
            _phieuchuyenphatService.UpdatePhieuVanChuyen(phieuvanchuyen);
            //update trang thai tat ca phieu chuyen phat sang trang thai moi
            //add by Mai 03/03 update nhat ki phieu chuye phat
            foreach (var pcp in phieuvanchuyen.phieuchuyenphats)
            {
                pcp.TrangThai = ENTrangThaiChuyenPhat.DangVanChuyen;
                _phieuchuyenphatService.UpdatePhieuChuyenPhat(pcp);
                _phieuchuyenphatService.InsertPhieuChuyenPhatLog("Phiếu được chuyển trạng thái thành: Đang vận chuyển bởi " + _workContext.CurrentNhanVien.HoVaTen, pcp.Id);

                //if (pcp.phieuvanchuyen.nhatkyvanchuyens.Count == 0)
                //    return Loi("Phiếu vận chuyển không có thông tin nhật ký di chuyển");
                var nhatkyphieuvanchuyen = phieuvanchuyenlog;
                //cap nhat nhat ky 
                var nhatky = _phieuchuyenphatService.GetPhieuChuyenPhatVanChuyenById(pcp.Id, nhatkyphieuvanchuyen.ChuyenDiId);
                if (nhatky == null)
                    nhatky = new PhieuChuyenPhatVanChuyen();
                nhatky.ChuyenDiId = nhatkyphieuvanchuyen.ChuyenDiId;
                nhatky.CuocVuotTuyen = pcp.CuocVuotTuyen;
                nhatky.HanhTrinhId = nhatkyphieuvanchuyen.HanhTrinhId;
                nhatky.KhuVucId = nhatkyphieuvanchuyen.KhuVucId;
                nhatky.PhieuChuyenPhatId = pcp.Id;
                nhatky.PhieuVanChuyenId = nhatkyphieuvanchuyen.PhieuVanChuyenId;
                nhatky.TongCuoc = pcp.TongTienCuoc;
                //add by Mai 08.04.2017 truong hop vuot tuyen cuoc tuyen 1=cuoc phi+ (gt+ct)/2, tuyen 2 = vt+ (gt+ct)/2
                if (pcp.CuocVuotTuyen > 0)
                {
                    //kiem tra xem no la tuyen 1 hay tuyen 2
                    if (pcp.nhatkyvanchuyens.Count() == 0)
                    {
                        //no la tuyen 1
                        nhatky.TongCuoc = pcp.CuocPhi + (pcp.CuocCapToc + pcp.CuocGiaTri) / 2;
                    }
                    if (pcp.nhatkyvanchuyens.Count() == 1)
                    {
                        //tuyen 2 
                        nhatky.TongCuoc = pcp.CuocVuotTuyen + (pcp.CuocCapToc + pcp.CuocGiaTri) / 2;
                    }
                }
                nhatky.TuyenId = nhatkyphieuvanchuyen.TuyenId;
                nhatky.VanPhongGuiId = nhatkyphieuvanchuyen.VanPhongGuiId;

                nhatky.VanPhongNhanId = nhatkyphieuvanchuyen.VanPhongNhanId != 0 ? nhatkyphieuvanchuyen.VanPhongNhanId : pcp.VanPhongNhanId;
                _phieuchuyenphatService.UpdatePhieuChuyenPhatVanChuyen(nhatky);

            }

            return ThanhCong();
        }
        [HttpPost]
        public ActionResult _PhieuVanChuyenHuyChuyenDi(int PhieuVanChuyenLogId)
        {
            var itemlog = _phieuchuyenphatService.GetPhieuVanChuyenLogById(PhieuVanChuyenLogId);
            if (itemlog == null)
                return Loi("Thông tin không tồn tại");
            if (itemlog.phieuvanchuyen.TrangThai == ENTrangThaiPhieuVanChuyen.KetThuc)
                return Loi("Không được phép hủy khi phiếu vận chuyển đã kết thúc");
            //lay phieu chuyen phat van chuyen

            //huy thiet lap chuyen di hien tai
            _phieuchuyenphatService.DeletePhieuVanChuyenLog(PhieuVanChuyenLogId);
            //chuyen trang thai
            var phieuvanchuyen = _phieuchuyenphatService.GetPhieuVanChuyenById(itemlog.PhieuVanChuyenId);
            if (phieuvanchuyen.nhatkyvanchuyens.Count == 0)
            {
                //cap nhat thong tin trang thai phieu van chuyen
                phieuvanchuyen.TrangThai = ENTrangThaiPhieuVanChuyen.Moi;
                _phieuchuyenphatService.UpdatePhieuVanChuyen(phieuvanchuyen);
                //cap nhat lai phieu chuyen phat
                foreach (var pcp in phieuvanchuyen.phieuchuyenphats)
                {
                    pcp.TrangThai = ENTrangThaiChuyenPhat.DaXepLenh;
                    _phieuchuyenphatService.UpdatePhieuChuyenPhat(pcp);
                    _phieuchuyenphatService.InsertPhieuChuyenPhatLog("Hủy chuyến đi, phiếu đưa về trạng thái đã xếp lệnh bởi " + _workContext.CurrentNhanVien.HoVaTen, pcp.Id);
                    //xoa nhat ki xep chuyen di
                    var _PhieuChuyenPhatVanChuyen = _phieuchuyenphatService.GetPhieuChuyenPhatVanChuyen(pcp.Id, phieuvanchuyen.Id);
                    _phieuchuyenphatService.DeletePhieuChuyenPhatVanChuyen(_PhieuChuyenPhatVanChuyen.Id);
                }
            }
            else
            {
                var phieuchuyenphat_vanchuyens = _phieuchuyenphatService.GetPhieuChuyenPhatVanChuyenByChuyenDiId(itemlog.ChuyenDiId, itemlog.PhieuVanChuyenId);
                //delete phieu chuyen phat van chuyen
                foreach (var item in phieuchuyenphat_vanchuyens)
                {
                    _phieuchuyenphatService.DeletePhieuChuyenPhatVanChuyen(item);
                }
            }
            return ThanhCong();

        }
        #endregion

        #region nhan hang

        public ActionResult QLNhanHang()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var model = new ListPhieuModel();
            model.TenVanPhongHienTai = _workContext.CurrentVanPhong.TenVanPhong;
            model.NgayTao = DateTime.Now;
            model.VanPhongNhanId = _workContext.CurrentVanPhong.Id;
            model.trangthais = this.GetCVEnumSelectList<ENTrangThaiChuyenPhat>(_localizationService);
            return View(model);
        }

        public ActionResult _PhieuChuyenPhatDanhSach(int Id)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var phieuvanchuyen = _phieuchuyenphatService.GetPhieuVanChuyenById(Id);
            var model = phieuvanchuyen.ToModel(_localizationService, _priceFormatter, true);
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult _NhanPhieuChuyenPhat(string SelectIds)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            if (string.IsNullOrEmpty(SelectIds))
                return Loi("Chưa chọn phiếu chuyển phát");
            string[] arrId = SelectIds.Split(',');
            int PhieuVanChuyenId = 0;
            foreach (var _id in arrId)
            {
                if (!string.IsNullOrEmpty(_id))
                {
                    int id;
                    if (int.TryParse(_id, out id))
                    {
                        var phieuchuyenphat = _phieuchuyenphatService.GetPhieuChuyenPhatById(id);
                        if (phieuchuyenphat.phieuvanchuyen.nhatkyvanchuyens.Count == 0)
                            return Loi("Phiếu vận chuyển không có thông tin nhật ký di chuyển");
                        var nhatkyphieuvanchuyen = phieuchuyenphat.phieuvanchuyen.nhatkyvanchuyens.Last();
                        PhieuVanChuyenId = phieuchuyenphat.PhieuVanChuyenId.GetValueOrDefault(0);
                        if (phieuchuyenphat.TrangThai == ENTrangThaiChuyenPhat.DangVanChuyen)
                        {
                            //kiem tra tinh logic

                            //////////////////////////////
                            phieuchuyenphat.TrangThai = ENTrangThaiChuyenPhat.DenVanPhongNhan;
                            phieuchuyenphat.NgayDenVanPhongNhan = DateTime.Now;
                            _phieuchuyenphatService.UpdatePhieuChuyenPhat(phieuchuyenphat);
                            _phieuchuyenphatService.InsertPhieuChuyenPhatLog("Phiếu đến văn phòng nhận, được nhận bởi " + _workContext.CurrentNhanVien.HoVaTen, phieuchuyenphat.Id);
                            //cap nhat nhat ky 
                            //var nhatky = _phieuchuyenphatService.GetPhieuChuyenPhatVanChuyenById(phieuchuyenphat.Id,nhatkyphieuvanchuyen.ChuyenDiId);
                            //if (nhatky == null)
                            //    nhatky = new PhieuChuyenPhatVanChuyen();
                            //nhatky.ChuyenDiId = nhatkyphieuvanchuyen.ChuyenDiId;
                            //nhatky.CuocVuotTuyen = phieuchuyenphat.CuocVuotTuyen;
                            //nhatky.HanhTrinhId = nhatkyphieuvanchuyen.HanhTrinhId;
                            //nhatky.KhuVucId = nhatkyphieuvanchuyen.KhuVucId;
                            //nhatky.PhieuChuyenPhatId = phieuchuyenphat.Id;
                            //nhatky.PhieuVanChuyenId = nhatkyphieuvanchuyen.PhieuVanChuyenId;
                            //nhatky.TongCuoc = phieuchuyenphat.TongTienCuoc;
                            //nhatky.TuyenId = nhatkyphieuvanchuyen.TuyenId;
                            //nhatky.VanPhongGuiId = nhatkyphieuvanchuyen.VanPhongGuiId;

                            //nhatky.VanPhongNhanId = nhatkyphieuvanchuyen.VanPhongNhanId != 0 ? nhatkyphieuvanchuyen.VanPhongNhanId : phieuchuyenphat.VanPhongNhanId;
                            //_phieuchuyenphatService.UpdatePhieuChuyenPhatVanChuyen(nhatky);
                        }

                    }
                }
                //kiem tra phieu nay da het nhan het chua, neu het roi thi ket thuc
                if (PhieuVanChuyenId > 0)
                {
                    var pvcitem = _phieuchuyenphatService.GetPhieuVanChuyenById(PhieuVanChuyenId);
                    pvcitem.TrangThai = ENTrangThaiPhieuVanChuyen.DenVanPhongNhan;
                    foreach (var p in pvcitem.phieuchuyenphats)
                    {
                        //neu co bat ky 1 phieu nao trong danh sach chua tra het thi van dang di 
                        if (p.TrangThai == ENTrangThaiChuyenPhat.DangVanChuyen)
                        {
                            pvcitem.TrangThai = ENTrangThaiPhieuVanChuyen.DangVanChuyen;
                            break;
                        }
                    }

                    _phieuchuyenphatService.UpdatePhieuVanChuyen(pvcitem);

                }
            }
            //tu dong send sms neu dc thiet dat
            var smsautosend_config = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, ENNhaXeCauHinh.SMS_AUTO_SEND);
            if (smsautosend_config != null && smsautosend_config.GiaTri.Equals("1"))
                _NhanTinPhieuChuyenPhat(SelectIds);
            return ThanhCong();
        }
        [HttpPost]
        public ActionResult _TraLaiPhieuChuyenPhat(int Id)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var phieuchuyenphat = _phieuchuyenphatService.GetPhieuChuyenPhatById(Id);
            if (phieuchuyenphat == null || phieuchuyenphat.TrangThai != ENTrangThaiChuyenPhat.DenVanPhongNhan)
                return Loi("Phiếu chuyển phát không tồn tại, hoặc trạng thái không đúng " + GetTrangThai(phieuchuyenphat));
            phieuchuyenphat.TrangThai = ENTrangThaiChuyenPhat.DangVanChuyen;
            _phieuchuyenphatService.UpdatePhieuChuyenPhat(phieuchuyenphat);
            //xoa thong tin chuyen di cuoi
            if (phieuchuyenphat.nhatkyvanchuyens.Count > 0)
            {
                _phieuchuyenphatService.DeletePhieuChuyenPhatVanChuyen(phieuchuyenphat.nhatkyvanchuyens.Last().Id);
            }
            return ThanhCong();
        }

        #endregion
        #region Tra hang

        public ActionResult QLTraHang()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var model = new ListPhieuModel();
            model.TenVanPhongHienTai = _workContext.CurrentVanPhong.TenVanPhong;
            model.VanPhongNhanId = _workContext.CurrentVanPhong.Id;
            model.NgayTao = DateTime.Now;
            model.trangthais = new List<SelectListItem>();
            var NhanHangId = (int)ENTrangThaiChuyenPhat.DangVanChuyen;
            var TraHangId = (int)ENTrangThaiChuyenPhat.DenVanPhongNhan;
            model.trangthais.Add(new SelectListItem { Value = NhanHangId.ToString(), Text = "Nhận hàng" });
            model.trangthais.Add(new SelectListItem { Value = TraHangId.ToString(), Text = "Trả hàng" });
            return View(model);
        }
        public ActionResult GomPhieuChuyenPhat()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var model = new ListPhieuModel();
            model.TenVanPhongHienTai = _workContext.CurrentVanPhong.TenVanPhong;
            model.NgayTao = DateTime.Now;
            //thong tin trang thai           
            model.trangthais = this.GetCVEnumSelectList<ENTrangThaiChuyenPhat>(_localizationService);
            //thong tin van phong
            var vanphongs = _nhaxeService.GetAllVanPhongByNhaXeId(_workContext.NhaXeId).Where(c => c.KieuVanPhong == ENKieuVanPhong.VanPhong).ToList();
            model.VanPhongs = vanphongs.Select(c => new SelectListItem
            {
                Text = c.TenVanPhong,
                Value = c.Id.ToString(),
            }).ToList();
            model.VanPhongs.Insert(0, new SelectListItem { Value = "0", Text = "----------Tất cả----------" });
            return View(model);
        }
        [HttpPost]
        public ActionResult _TraPhieuChuyenPhat(string SelectIds)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            if (string.IsNullOrEmpty(SelectIds))
                return Loi("Chưa chọn phiếu chuyển phát");
            string[] arrId = SelectIds.Split(',');
            int PhieuVanChuyenId = 0;
            foreach (var _id in arrId)
            {
                if (!string.IsNullOrEmpty(_id))
                {
                    int id;
                    if (int.TryParse(_id, out id))
                    {
                        var phieuchuyenphat = _phieuchuyenphatService.GetPhieuChuyenPhatById(id);
                        PhieuVanChuyenId = phieuchuyenphat.PhieuVanChuyenId.GetValueOrDefault(0);
                        if (phieuchuyenphat.TrangThai == ENTrangThaiChuyenPhat.DenVanPhongNhan)
                        {
                            //neu phieu chuyen phat da den van phong nhan cuoi cung thi ket thuc
                            phieuchuyenphat.TrangThai = ENTrangThaiChuyenPhat.KetThuc;
                            phieuchuyenphat.NgayKetThuc = DateTime.Now;
                            _phieuchuyenphatService.UpdatePhieuChuyenPhat(phieuchuyenphat);
                            _phieuchuyenphatService.InsertPhieuChuyenPhatLog("Phiếu được trả cho khách hàng bởi " + _workContext.CurrentNhanVien.HoVaTen, phieuchuyenphat.Id);

                        }

                    }
                }
                //kiem tra phieu nay da het nhan het chua, neu het roi thi ket thuc
                if (PhieuVanChuyenId > 0)
                {
                    var pvcitem = _phieuchuyenphatService.GetPhieuVanChuyenById(PhieuVanChuyenId);
                    if (!pvcitem.phieuchuyenphats.Any(c => c.TrangThai != ENTrangThaiChuyenPhat.KetThuc))
                    {
                        //tat ca da ket thuc
                        pvcitem.TrangThai = ENTrangThaiPhieuVanChuyen.KetThuc;
                        _phieuchuyenphatService.UpdatePhieuVanChuyen(pvcitem);
                    }
                }
            }
            return ThanhCong();
        }


        [HttpPost]
        public ActionResult _NhanTinPhieuChuyenPhat(string SelectIds)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            if (string.IsNullOrEmpty(SelectIds))
                return Loi("Chưa chọn phiếu chuyển phát");
            string[] arrId = SelectIds.Split(',');
            //lay thong tin template
            var itemtemplate = _nhaxeService.GetNhaXeCauHinhByCode(_workContext.NhaXeId, ENNhaXeCauHinh.SMS_TEMPLATE);
            //vn.worldsms.wcf.APISMS apisms = new vn.worldsms.wcf.APISMS();

            //string Sender = "CHUYỂN PHÁT HẢI ÂU";
            //string Username = "chonve";
            //string Password = "@#chonve";
            string ret = "";
            foreach (var _id in arrId)
            {
                if (!string.IsNullOrEmpty(_id))
                {
                    int id;
                    if (int.TryParse(_id, out id))
                    {
                        var phieuchuyenphat = _phieuchuyenphatService.GetPhieuChuyenPhatById(id);
                        string strsms = itemtemplate.GiaTri.Trim();
                        strsms = strsms.Replace("[VAN_PHONG]", phieuchuyenphat.VanPhongNhan.TenVanPhong);
                        strsms = strsms.Replace("[DIA_CHI]", phieuchuyenphat.VanPhongNhan.diachiinfo != null ? phieuchuyenphat.VanPhongNhan.diachiinfo.DiaChi1 : "");
                        strsms = strsms.LoaiBoDauTiengViet();
                        if (!string.IsNullOrEmpty(phieuchuyenphat.NguoiNhan.SoDienThoai))
                        {

                            //string _result = apisms.PushMsg2Phone(Sender, strsms, phieuchuyenphat.NguoiNhan.SoDienThoai, Username, Password);
                            string _result = SendSMS(phieuchuyenphat.NguoiNhan.SoDienThoai, strsms);
                            string _retmsg = "Lỗi";
                            if (_result.Equals(@"{""status"":1}"))
                                _retmsg = "OK";
                            ret = ret + string.Format("SMS: {0} ({1}) -> {2}", phieuchuyenphat.NguoiNhan.HoTen, phieuchuyenphat.NguoiNhan.SoDienThoai, _retmsg);
                            if (_result.Equals(@"{""status"":1}"))
                            {
                                phieuchuyenphat.DaSMS++;
                                _phieuchuyenphatService.UpdatePhieuChuyenPhat(phieuchuyenphat);
                                _phieuchuyenphatService.InsertPhieuChuyenPhatLog("SMS đến khách hàng lần " + phieuchuyenphat.DaSMS + " bởi " + _workContext.CurrentNhanVien.HoVaTen, phieuchuyenphat.Id);
                            }

                        }

                    }
                }
            }
            return Loi(ret);
        }

        public ActionResult InPhieuTraHang(string SelectIds)
        {
            var model = new InPhieuModel();
            var phieuguihangs = new List<PhieuChuyenPhat>();
            if (SelectIds != null)
            {
                var phieuguis = SelectIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                phieuguihangs.AddRange(_phieuchuyenphatService.GetPhieuChuyenPhatsByIds(phieuguis));
            }
            model.PhieuChuyenPhats = phieuguihangs
                .Select(c =>
                {
                    return c.ToModel(_localizationService, _priceFormatter);
                }).ToList();
            model.TenVanPhongNhan = _workContext.CurrentVanPhong.Ma;
            return View(model);
        }
        public ActionResult InPhieuNhanHang(string NgayNhanHang)
        {
            var model = new InPhieuModel();
            DateTime _ngayNhan = Convert.ToDateTime(NgayNhanHang);
            model.PhieuChuyenPhats = _phieuchuyenphatService.GetAllPhieuChuyenPhat(_workContext.NhaXeId, 0, _ngayNhan, "", ENTrangThaiChuyenPhat.DangVanChuyen, 0, _workContext.CurrentVanPhong.Id)
                .Select(c =>
                {
                    return c.ToModel(_localizationService, _priceFormatter);
                }).ToList();
            model.TenVanPhongNhan = _workContext.CurrentVanPhong.Ma;
            return View(model);
        }
        public ActionResult _ToVanChuyenTraHang(string SelectIds)
        {
            var model = new ToVanChuyenModel();
            model.phieubiennhanids = SelectIds;
            //lay thong tin phieu bien nhan dau tien
            string[] arrId = SelectIds.Split(',');
            int phieuid;
            if (int.TryParse(arrId[0], out phieuid))
            {
                var phieubiennhan = _phieuchuyenphatService.GetPhieuChuyenPhatById(phieuid);
                model.ToVanChuyenIdSelect = phieubiennhan.ToVanChuyenTraId.GetValueOrDefault(0);
                model.NguoiVanChuyenId = phieubiennhan.NguoiVanChuyenTraId.GetValueOrDefault(0);
            }
            var _tovc = _workContext.CurrentVanPhong.tovanchuyens.Select(c => c.tovanchuyen).Where(c => c.Id == model.ToVanChuyenIdSelect).FirstOrDefault();
            //neu ko co thi lay item dau tien
            if (_tovc == null)
            {
                _tovc = _workContext.CurrentVanPhong.tovanchuyens.Select(c => c.tovanchuyen).FirstOrDefault();
            }
            if (_tovc != null)
            {
                model.ToVanChuyenIdSelect = _tovc.Id;
                var _nvc = _tovc.nguoivanchuyens.Where(c => c.Id == model.NguoiVanChuyenId).FirstOrDefault();
                //neu ko co thi lay item dau tien
                if (_nvc == null)
                {
                    _nvc = _tovc.nguoivanchuyens.FirstOrDefault();
                }
                if (_nvc != null)
                {
                    model.NguoiVanChuyenId = _nvc.Id;
                }
                model.nguoivanchuyens = _tovc.nguoivanchuyens.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.HoVaTen + "(" + c.DienThoai + ")",
                    Selected = c.Id == model.NguoiVanChuyenId
                }).ToList();
            }
            model.tovanchuyens = _workContext.CurrentVanPhong.tovanchuyens.Select(c => c.tovanchuyen).Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.TenTo + " - " + c.MoTa,
                Selected = c.Id == model.ToVanChuyenIdSelect
            }).ToList();

            return PartialView(model);
        }
        [HttpPost]
        public ActionResult _ToVanChuyenTraHang(ToVanChuyenModel model)
        {
            string[] arrId = model.phieubiennhanids.Split(',');
            foreach (var _id in arrId)
            {
                if (!string.IsNullOrEmpty(_id))
                {
                    int id;
                    if (int.TryParse(_id, out id))
                    {
                        var phieuchuyenphat = _phieuchuyenphatService.GetPhieuChuyenPhatById(id);
                        phieuchuyenphat.ToVanChuyenTraId = model.ToVanChuyenIdSelect;
                        phieuchuyenphat.NguoiVanChuyenTraId = model.NguoiVanChuyenId;
                        _phieuchuyenphatService.UpdatePhieuChuyenPhat(phieuchuyenphat);
                        _phieuchuyenphatService.InsertPhieuChuyenPhatLog("Phiếu được gán tổ vận chuyển bởi " + _workContext.CurrentNhanVien.HoVaTen, phieuchuyenphat.Id);

                    }
                }
            }
            return ThanhCong();
        }
        [HttpPost]
        public ActionResult _ToVanChuyenHuyTraHang(ToVanChuyenModel model)
        {
            string[] arrId = model.phieubiennhanids.Split(',');
            foreach (var _id in arrId)
            {
                if (!string.IsNullOrEmpty(_id))
                {
                    int id;
                    if (int.TryParse(_id, out id))
                    {
                        var phieuchuyenphat = _phieuchuyenphatService.GetPhieuChuyenPhatById(id);
                        phieuchuyenphat.ToVanChuyenTraId = null;
                        phieuchuyenphat.NguoiVanChuyenTraId = null;
                        _phieuchuyenphatService.UpdatePhieuChuyenPhat(phieuchuyenphat);
                        _phieuchuyenphatService.InsertPhieuChuyenPhatLog("Tổ vận chuyển hủy trả hàng bởi " + _workContext.CurrentNhanVien.HoVaTen, phieuchuyenphat.Id);
                    }
                }
            }
            return ThanhCong();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetNguoiVanChuyens(int ToVanChuyenId)
        {
            var _tovc = _phieuchuyenphatService.GetToVanChuyenById(ToVanChuyenId);


            var dlls = _tovc.nguoivanchuyens.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.HoVaTen + "(" + c.DienThoai + ")",
            }).ToList();
            return Json(dlls, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region Lich su giao dich
        public ActionResult LSPhieuChuyenPhat()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var model = new ListPhieuModel();
            model.TenVanPhongHienTai = _workContext.CurrentVanPhong.TenVanPhong;
            model.VanPhongGuiId = _workContext.CurrentVanPhong.Id;
            model.trangthais = this.GetCVEnumSelectList<ENTrangThaiChuyenPhat>(_localizationService);
            model.TuNgay = DateTime.Now;
            model.DenNgay = DateTime.Now.AddDays(1);
            return View(model);
        }

        public ActionResult LSPhieuVanChuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var model = new ListPhieuModel();
            model.TenVanPhongHienTai = _workContext.CurrentVanPhong.TenVanPhong;
            model.VanPhongGuiId = _workContext.CurrentVanPhong.Id;
            model.trangthaivanchuyens = this.GetCVEnumSelectList<ENTrangThaiPhieuVanChuyen>(_localizationService);
            model.TuNgay = DateTime.Now.AddDays(-3);
            model.DenNgay = DateTime.Now.AddDays(1);
            return View(model);
        }
        public ActionResult LSNhanHang()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var model = new ListPhieuModel();
            model.TenVanPhongHienTai = _workContext.CurrentVanPhong.TenVanPhong;
            model.VanPhongNhanId = _workContext.CurrentVanPhong.Id;
            model.trangthaivanchuyens = this.GetCVEnumSelectList<ENTrangThaiPhieuVanChuyen>(_localizationService);
            model.TuNgay = DateTime.Now;
            model.DenNgay = DateTime.Now.AddDays(1);
            return View(model);
        }
        public ActionResult LSTraHang()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var model = new ListPhieuModel();
            model.TenVanPhongHienTai = _workContext.CurrentVanPhong.TenVanPhong;
            model.VanPhongNhanId = _workContext.CurrentVanPhong.Id;
            model.trangthais = this.GetCVEnumSelectList<ENTrangThaiChuyenPhat>(_localizationService);
            model.TuNgay = DateTime.Now;
            model.DenNgay = DateTime.Now.AddDays(1);
            return View(model);
        }
        public ActionResult _NhatKiPhieuChuyenPhat(int Id)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var model = new PhieuChuyenPhatModel();
            model.nhatkys = _phieuchuyenphatService.GetPhieuChuyenPhatById(Id).nhatkys.ToList();
            return PartialView(model);
        }

        #endregion
        #region Quan ly to van chuyen va nguoi van chuyen
        public ActionResult QLToVanChuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return AccessDeniedView();
            return View();
        }
        ToVanChuyenModel toModel(ToVanChuyen e)
        {
            var model = new ToVanChuyenModel();
            model.Id = e.Id;
            model.TenTo = e.TenTo;
            model.MoTa = e.MoTa;
            return model;
        }
        void toEntity(ToVanChuyen item, ToVanChuyenModel e)
        {
            item.NhaXeId = _workContext.NhaXeId;
            item.Id = e.Id;
            item.TenTo = e.TenTo;
            item.MoTa = e.MoTa;
        }
        public JsonpResult ListToVanChuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return null;
            var items = _phieuchuyenphatService.GetAllToVanChuyen(_workContext.NhaXeId);
            var itemmodels = items.Select(c =>
            {
                return toModel(c);
            }).ToList();
            return this.Jsonp(itemmodels);
        }

        public JsonpResult CreateToVanChuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return null;
            var models = this.DeserializeObject<IEnumerable<ToVanChuyenModel>>("models");

            if (models != null)
            {
                foreach (var model in models)
                {
                    var item = new ToVanChuyen();
                    toEntity(item, model);
                    _phieuchuyenphatService.InsertToVanChuyen(item);
                    break;
                }
            }
            return this.Jsonp(models);
        }

        public JsonpResult EditToVanChuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return null;
            var models = this.DeserializeObject<IEnumerable<ToVanChuyenModel>>("models");
            if (models != null)
            {
                foreach (var model in models)
                {
                    var item = _phieuchuyenphatService.GetToVanChuyenById(model.Id);
                    toEntity(item, model);
                    _phieuchuyenphatService.UpdateToVanChuyen(item);
                    break;

                }
            }
            return this.Jsonp(models);
        }
        public JsonpResult DeleteToVanChuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return null;
            var models = this.DeserializeObject<IEnumerable<ToVanChuyenModel>>("models");
            if (models != null)
            {
                foreach (var model in models)
                {
                    var item = _phieuchuyenphatService.GetToVanChuyenById(model.Id);
                    _phieuchuyenphatService.DeleteToVanChuyen(item);
                    break;

                }
            }
            return this.Jsonp(models);
        }
        public ActionResult QLNguoiVanChuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var model = new NguoiVanChuyenModel();
            model.tovanchuyens = _workContext.CurrentVanPhong.tovanchuyens.Select(c => c.tovanchuyen).Select(c =>
            {
                var i = new SelectListItem();
                i.Value = c.Id.ToString();
                i.Text = c.TenTo + " - " + c.MoTa;
                return i;
            }).ToList();
            return View(model);
        }
        NguoiVanChuyenModel toModel(NguoiVanChuyen e)
        {
            var model = new NguoiVanChuyenModel();
            model.Id = e.Id;
            model.HoVaTen = e.HoVaTen;
            model.DienThoai = e.DienThoai;
            model.CMT = e.CMT;
            model.ToVanChuyenId = e.ToVanChuyenId;
            return model;
        }
        void toEntity(NguoiVanChuyen model, NguoiVanChuyenModel e)
        {
            model.Id = e.Id;
            model.HoVaTen = e.HoVaTen;
            model.DienThoai = e.DienThoai;
            model.CMT = e.CMT;
        }
        public JsonpResult ListNguoiVanChuyen(int ToVanChuyenId)
        {
            var items = _phieuchuyenphatService.GetAllNguoiVanChuyen(ToVanChuyenId);
            var itemmodels = items.Select(c =>
            {
                return toModel(c);
            }).ToList();
            return this.Jsonp(itemmodels);
        }

        public JsonpResult CreateNguoiVanChuyen(int ToVanChuyenId)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return null;
            var models = this.DeserializeObject<IEnumerable<NguoiVanChuyenModel>>("models");

            if (models != null)
            {
                foreach (var model in models)
                {
                    var item = new NguoiVanChuyen();
                    toEntity(item, model);
                    item.ToVanChuyenId = ToVanChuyenId;
                    _phieuchuyenphatService.InsertNguoiVanChuyen(item);
                    break;
                }
            }
            return this.Jsonp(models);
        }

        public JsonpResult EditNguoiVanChuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return null;
            var models = this.DeserializeObject<IEnumerable<NguoiVanChuyenModel>>("models");
            if (models != null)
            {
                foreach (var model in models)
                {
                    var item = _phieuchuyenphatService.GetNguoiVanChuyenById(model.Id);
                    toEntity(item, model);
                    _phieuchuyenphatService.UpdateNguoiVanChuyen(item);
                    break;

                }
            }
            return this.Jsonp(models);
        }
        public JsonpResult DeleteNguoiVanChuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return null;
            var models = this.DeserializeObject<IEnumerable<NguoiVanChuyenModel>>("models");
            if (models != null)
            {
                foreach (var model in models)
                {
                    var item = _phieuchuyenphatService.GetNguoiVanChuyenById(model.Id);
                    _phieuchuyenphatService.DeleteNguoiVanChuyen(item);
                    break;

                }
            }
            return this.Jsonp(models);
        }
        #endregion
        #region hang ton
        public ActionResult HangTon()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            var model = new ListPhieuModel();
            model.TenVanPhongHienTai = _workContext.CurrentVanPhong.TenVanPhong;
            model.VanPhongNhanId = _workContext.CurrentVanPhong.Id;
            model.trangthais = this.GetCVEnumSelectList<ENTrangThaiHangTrongKho>(_localizationService);
            return View(model);
        }
        [HttpPost]
        public ActionResult _GetHangTonVaThatLac(ListPhieuModel model)
        {

            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVHangHoaKiGui))
                return AccessDeniedView();
            if (model.PhieuVanChuyenId > 0)
                model.NgayTao = null;
            var items = _phieuchuyenphatService.GetPhieuTonVaThatLac(_workContext.NhaXeId, model.HangHoaInfo, model.VanPhongNhanId, model.TrangThaiId);

            var _item = items.Select(x =>
            {

                var m = x.ToModel(_localizationService, _priceFormatter);
                return m;
            }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = _item,
                Total = _item.Count
            };

            return Json(gridModel);
        }
        #endregion
        #region tuyen hanh trinh
        public ActionResult QLTuyenHanhTrinh()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return AccessDeniedView();
            return View();
        }
        TuyenHanhTrinhModel toModel(TuyenVanDoanh e)
        {
            var model = new TuyenHanhTrinhModel();
            model.Id = e.Id;
            model.NhaXeId = e.NhaXeId;
            model.Ten = e.TenTuyen;
            model.TenVietTat = e.TenVietTat;
            return model;
        }
        void toEntity(TuyenVanDoanh item, TuyenHanhTrinhModel e)
        {
            item.NhaXeId = _workContext.NhaXeId;
            item.Id = e.Id;
            item.TenTuyen = e.Ten;
            item.TenVietTat = e.TenVietTat;
        }
        public JsonpResult ListTuyenHanhTrinh()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return null;
            var items = _phieuchuyenphatService.GetAllTuyenVanDoanh(_workContext.NhaXeId);
            var itemmodels = items.Select(c =>
            {
                return toModel(c);
            }).ToList();
            return this.Jsonp(itemmodels);

        }

        public JsonpResult CreateTuyenHanhTrinh()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return null;
            var models = this.DeserializeObject<IEnumerable<TuyenHanhTrinhModel>>("models");

            if (models != null)
            {
                foreach (var model in models)
                {
                    var item = new TuyenVanDoanh();
                    toEntity(item, model);
                    _phieuchuyenphatService.InsertTuyenVanDoanh(item);
                    break;
                }
            }
            return this.Jsonp(models);
        }

        public JsonpResult EditTuyenHanhTrinh()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return null;
            var models = this.DeserializeObject<IEnumerable<TuyenHanhTrinhModel>>("models");
            if (models != null)
            {
                foreach (var model in models)
                {
                    var item = _phieuchuyenphatService.GetTuyenVanDoanhById(model.Id);
                    toEntity(item, model);
                    _phieuchuyenphatService.UpdateTuyenVanDoanh(item);
                    break;

                }
            }
            return this.Jsonp(models);
        }
        public JsonpResult DeleteTuyenHanhTrinh()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return null;
            var models = this.DeserializeObject<IEnumerable<TuyenHanhTrinhModel>>("models");
            if (models != null)
            {
                foreach (var model in models)
                {
                    var item = _phieuchuyenphatService.GetTuyenVanDoanhById(model.Id);
                    _phieuchuyenphatService.DeleteTuyenVanDoanh(item);
                    break;

                }
            }
            return this.Jsonp(models);
        }
        #endregion
        #region khu vuc
        public ActionResult QLKhuVuc()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return AccessDeniedView();
            return View();
        }
        KhuVucModel toModel(KhuVuc e)
        {
            var model = new KhuVucModel();
            model.Id = e.Id;
            model.NhaXeId = e.NhaXeId;
            model.TenKhuVuc = e.TenKhuVuc;
            model.TenVietTat = e.TenVietTat;
            return model;
        }
        void toEntity(KhuVuc item, KhuVucModel e)
        {
            item.NhaXeId = _workContext.NhaXeId;
            item.Id = e.Id;
            item.TenKhuVuc = e.TenKhuVuc;
            item.TenVietTat = e.TenVietTat;
        }
        public JsonpResult ListKhuVuc()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return null;
            var items = _phieuchuyenphatService.GetAllKhuVuc(_workContext.NhaXeId);
            var itemmodels = items.Select(c =>
            {
                return toModel(c);
            }).ToList();
            return this.Jsonp(itemmodels);
        }

        public JsonpResult CreateKhuVuc()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return null;
            var models = this.DeserializeObject<IEnumerable<KhuVucModel>>("models");

            if (models != null)
            {
                foreach (var model in models)
                {
                    var item = new KhuVuc();
                    toEntity(item, model);
                    _phieuchuyenphatService.InsertKhuVuc(item);
                    break;
                }
            }
            return this.Jsonp(models);
        }

        public JsonpResult EditKhuVuc()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return null;
            var models = this.DeserializeObject<IEnumerable<KhuVucModel>>("models");
            if (models != null)
            {
                foreach (var model in models)
                {
                    var item = _phieuchuyenphatService.GetKhuVucById(model.Id);
                    toEntity(item, model);
                    _phieuchuyenphatService.UpdateKhuVuc(item);
                    break;

                }
            }
            return this.Jsonp(models);
        }
        public JsonpResult DeleteKhuVuc()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVThongTinNhaXe))
                return null;
            var models = this.DeserializeObject<IEnumerable<KhuVucModel>>("models");
            if (models != null)
            {
                foreach (var model in models)
                {
                    var item = _phieuchuyenphatService.GetKhuVucById(model.Id);
                    _phieuchuyenphatService.DeleteKhuVuc(item);
                    break;

                }
            }
            return this.Jsonp(models);
        }
        #endregion

        #region Dinh Bien
        public ActionResult BangDieuChuyen()
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            var model = new DBDanhSachChuyenDiModel();
            model.NgayDi = DateTime.Now;
            model.KhungGioId = (int)ENKhungGio.All;
            model.khunggios = this.GetCVEnumSelectList<ENKhungGio>(_localizationService, model.KhungGioId);
            //lay tat ca thong tin dinh bien
            var lsall = _phieuchuyenphatService.GetLaiPhuSoXe(model.NgayDi.Month, model.NgayDi.Year);
            model.LaiXes = lsall.Where(c => c.loai == LoaiLaiPhuSoXe.LAI_XE).Select(c =>
            {
                return new DBDanhSachChuyenDiModel.DBLaiPhuSoXe(c.Id, c.Ten);
            }).ToList();
            model.PhuXes = lsall.Where(c => c.loai == LoaiLaiPhuSoXe.PHU_XE).Select(c =>
            {
                return new DBDanhSachChuyenDiModel.DBLaiPhuSoXe(c.Id, c.Ten);
            }).ToList();
            model.AllXeInfo = lsall.Where(c => c.loai == LoaiLaiPhuSoXe.SO_XE).Select(c =>
            {
                return new DBDanhSachChuyenDiModel.DBLaiPhuSoXe(c.Id, c.Ten);
            }).ToList();

            return View(model);
        }
        XeXuatBenItemModel prepareChuyenDiItemModel(HistoryXeXuatBen xxb, List<DB_LaiPhuSoXe> lsall)
        {
            var m = xxb.toModel(_localizationService);
            //lay thong tin dinh bien
            var lpsoxeitem = lsall.Where(tt => tt.Ten.Equals(m.DBLaiXe) && tt.loai == LoaiLaiPhuSoXe.LAI_XE).FirstOrDefault();
            if (lpsoxeitem != null)
                m.LaiXeId = lpsoxeitem.Id;
            lpsoxeitem = lsall.Where(tt => tt.Ten.Equals(m.DBPhuXe) && tt.loai == LoaiLaiPhuSoXe.PHU_XE).FirstOrDefault();
            if (lpsoxeitem != null)
                m.PhuXeId = lpsoxeitem.Id;
            lpsoxeitem = lsall.Where(tt => tt.Ten.Equals(m.DBSoXe) && tt.loai == LoaiLaiPhuSoXe.SO_XE).FirstOrDefault();
            if (lpsoxeitem != null)
                m.XeVanChuyenId = lpsoxeitem.Id;
            return m;
        }
        [HttpGet]
        public ActionResult _BangDieuChuyen(DBDanhSachChuyenDiModel model)
        {
            var modelnew = new BangDieuChuyenModel();
            //build thong tin bang dieu chuyen
            modelnew.NgayDi = model.NgayDi;
            //lay thong tin hanh trinh, de lang hang
            var hanhtrinhs = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(_workContext.NhaXeId);

            KhungThoiGian khungtg = null;
            if (model.khunggio != ENKhungGio.All)
            {
                khungtg = new KhungThoiGian(model.khunggio);

            }
            //lay tat ca thong tin dinh bien
            var lsall = _phieuchuyenphatService.GetLaiPhuSoXe(model.NgayDi.Month, model.NgayDi.Year);
            //tao bang dieu chuyen
            modelnew.arrBangDieuChuyen = new List<BangDieuChuyenModel.BanDieuChuyenHanhTrinh>();
            foreach (var ht in hanhtrinhs)
            {
                var htchuyendis = new BangDieuChuyenModel.BanDieuChuyenHanhTrinh();
                htchuyendis.hanhtrinhinfo = ht;
                htchuyendis.LichTrinhItems = new List<BangDieuChuyenModel.BangDieuChuyenItem>();
                //lay thong tin lich trinh, de lam cot
                var lichtrinhs = _hanhtrinhService.GetAllLichTrinhByHanhTrinhId(ht.Id, _workContext.NhaXeId);
                var lichtrinhids = lichtrinhs.Select(c => c.Id).ToList();
                if (khungtg != null)
                {
                    lichtrinhids = lichtrinhs.Where(c => c.ThoiGianDi.Hour >= khungtg.GioTu
                                && c.ThoiGianDi.Hour < khungtg.GioDen).Select(c => c.Id).ToList();
                }
                var nguonves = _hanhtrinhService.GetAllNguonVeXeByHanhTrinh(lichtrinhids).OrderBy(c => c.ThoiGianDiHienTai).ToList();
                foreach (var nv in nguonves)
                {
                    var item = new BangDieuChuyenModel.BangDieuChuyenItem(nv.Id, ht.Id, ht.MaHanhTrinh, nv.LichTrinhId, nv.LichTrinhInfo.ThoiGianDi.ToString("HH:mm"));
                    //lay tat ca chuyen di trong lich trinh nay
                    var chuyendis = _nhaxeService.GetHistoryXeXuatBensByNguonVeId(nv.Id, model.NgayDi).OrderBy(c => c.NgayDi).ToList();

                    item.chuyendis = chuyendis.Select(c =>
                    {
                        return prepareChuyenDiItemModel(c, lsall);
                    }).ToList();
                    htchuyendis.LichTrinhItems.Add(item);
                }
                modelnew.arrBangDieuChuyen.Add(htchuyendis);
            }

            return PartialView(modelnew);
        }
        public ActionResult _ChiTietDieuChuyen(int ChuyenDiId, int NguonVeId, string NgayDi)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            var _ngaydi = Convert.ToDateTime(NgayDi);
            //lay thong tin nguon ve
            var nguonve = _hanhtrinhService.GetNguonVeXeById(NguonVeId);
            //lay tat ca thong tin dinh bien
            var lsall = _phieuchuyenphatService.GetLaiPhuSoXe(_ngaydi.Month, _ngaydi.Year);
            //lay thong tin hanh trinh, ben xe
            var hanhtrinh = nguonve.LichTrinhInfo.HanhTrinhInfo;
            var htdiemdon = hanhtrinh.DiemDons.OrderBy(c => c.ThuTu).FirstOrDefault();
            int BenXeId = 0;
            if (htdiemdon != null)
            {
                BenXeId = htdiemdon.diemdon.benxe != null ? htdiemdon.diemdon.benxe.Id : 0;
            }
            var model = new XeXuatBenItemModel();
            model.NgayDi = _ngaydi.AddHours(nguonve.ThoiGianDi.Hour).AddMinutes(nguonve.ThoiGianDi.Minute);
            model.NguonVeId = NguonVeId;
            model.HanhTrinhId = hanhtrinh.Id;
            if (ChuyenDiId > 0)
            {
                var chuyendi = _nhaxeService.GetHistoryXeXuatBenId(ChuyenDiId);
                model = prepareChuyenDiItemModel(chuyendi, lsall);
            }
            //lay thong tin gio mo phoi
            var lsgiomophoi = _phieuchuyenphatService.GetGioMoLenh(_ngaydi.Month, _ngaydi.Year, BenXeId);
            model.GioMoPhois = lsgiomophoi.Select(c => new XeXuatBenItemModel.DBGioMoPhoi(c.Id, c.GioMoLenh)).ToList();


            return PartialView(model);
        }
        [HttpPost]
        public ActionResult CapNhatDieuChuyen(XeXuatBenItemModel model)
        {
            if (this.CheckNoAccessIntoNhaXe(_workContext, _permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            var _ngayxuatben = Convert.ToDateTime(model.NgayDi);
            HistoryXeXuatBen historyxexuatben = null;
            if (model.Id > 0)
            {
                historyxexuatben = _nhaxeService.GetHistoryXeXuatBenId(model.Id);
            }
            else
            {
                //historyxexuatben = _nhaxeService.GetHistoryXeXuatBenByNguonVeId(NguonVeId, _ngayxuatben);
            }
            //lay tat ca thong tin dinh bien
            var lsall = _phieuchuyenphatService.GetLaiPhuSoXe(_ngayxuatben.Month, _ngayxuatben.Year);
            if (historyxexuatben != null)
            {
                //up date lai xe mơi               
                var laixe = _phieuchuyenphatService.GetLaiPhuSoXeById(model.LaiXeId);
                historyxexuatben.LaiXe = laixe.Ten;
                var phuxe = _phieuchuyenphatService.GetLaiPhuSoXeById(model.PhuXeId);
                if (phuxe != null)
                    historyxexuatben.PhuXe = phuxe.Ten;
                var soxe = _phieuchuyenphatService.GetLaiPhuSoXeById(model.XeVanChuyenId);
                historyxexuatben.SoXe = soxe.Ten;
                historyxexuatben.NgayDi = model.NgayDi;
                historyxexuatben.SoKhachXB = model.SoKhachXB;
                historyxexuatben.SoLenhVD = model.SoLenhVD;
                historyxexuatben.SoPhieuXN = model.SoPhieuXN;
                historyxexuatben.GioMoPhoi = model.GioMoPhoi;
                historyxexuatben.GhiChu = model.GhiChu;
                _nhaxeService.UpdateHistoryXeXuatBen(historyxexuatben);
            }
            else
            {
                historyxexuatben = new HistoryXeXuatBen();
                historyxexuatben.NguonVeId = model.NguonVeId;
                var laixe = _phieuchuyenphatService.GetLaiPhuSoXeById(model.LaiXeId);
                historyxexuatben.LaiXe = laixe.Ten;
                var phuxe = _phieuchuyenphatService.GetLaiPhuSoXeById(model.PhuXeId);
                if (phuxe != null)
                    historyxexuatben.PhuXe = phuxe.Ten;
                var soxe = _phieuchuyenphatService.GetLaiPhuSoXeById(model.XeVanChuyenId);
                historyxexuatben.SoXe = soxe.Ten;
                historyxexuatben.NgayDi = _ngayxuatben;
                historyxexuatben.NhaXeId = _workContext.NhaXeId;
                historyxexuatben.NgayTao = DateTime.Now;
                historyxexuatben.TrangThai = ENTrangThaiXeXuatBen.CHO_XUAT_BEN;
                historyxexuatben.NguoiTaoId = _workContext.CurrentNhanVien.Id;
                historyxexuatben.NgayDi = model.NgayDi;
                historyxexuatben.HanhTrinhId = model.HanhTrinhId;
                historyxexuatben.SoKhachXB = model.SoKhachXB;
                historyxexuatben.SoLenhVD = model.SoLenhVD;
                historyxexuatben.SoPhieuXN = model.SoPhieuXN;
                historyxexuatben.GioMoPhoi = model.GioMoPhoi;
                historyxexuatben.GhiChu = model.GhiChu;
                _nhaxeService.InsertHistoryXeXuatBen(historyxexuatben);

            }
            //lay lai thong tin
            return ThanhCong();
        }
        public ActionResult QLLaiPhuSoXe()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            var model = new QuanLyLaiXePhuXeSoXeModel();
            model.ThangId = DateTime.Now.Month;
            model.NamId = DateTime.Now.Year;
            for (int i = 2015; i <= DateTime.Now.Year; i++)
            {
                model.ListNam.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = (i == model.NamId) });
            }
            for (int i = 1; i <= 12; i++)
            {
                model.ListThang.Add(new SelectListItem { Text = "Tháng " + i.ToString(), Value = i.ToString(), Selected = (i == model.ThangId) });
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult GetLaiPhuSoXe(string Nam, string Thang)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            int nam = Convert.ToInt32(Nam);
            int thang = Convert.ToInt32(Thang);
            var laiphusoxe = new QuanLyLaiXePhuXeSoXeModel();
            if (nam >= DateTime.Now.Year)
            {
                if (thang >= DateTime.Now.Month)
                {
                    laiphusoxe.isEnable = true;
                }
                else
                {
                    laiphusoxe.isEnable = false;
                }
            }
            else
            {
                laiphusoxe.isEnable = false;
            }
            laiphusoxe.ListLaiXe = _phieuchuyenphatService.GetLaiPhuSoXe(thang, nam, LoaiLaiPhuSoXe.LAI_XE).Select(c => c.Ten).ToList();
            laiphusoxe.ListPhuXe = _phieuchuyenphatService.GetLaiPhuSoXe(thang, nam, LoaiLaiPhuSoXe.PHU_XE).Select(c => c.Ten).ToList();
            laiphusoxe.ListSoXe = _phieuchuyenphatService.GetLaiPhuSoXe(thang, nam, LoaiLaiPhuSoXe.SO_XE).Select(c => c.Ten).ToList();
            return Json(laiphusoxe);
        }
        [HttpPost]
        public ActionResult InsertLaiPhuSoXe(string Nam, string Thang, string LaiXe, string PhuXe, string SoXe)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            var arrLaiXe = LaiXe.Replace("\r", "").Split('\n').Select(c => c.Trim()).Distinct().ToList();
            var arrPhuXe = PhuXe.Replace("\r", "").Split('\n').Select(c => c.Trim()).Distinct().ToList();
            var arrSoXe = SoXe.Replace("\r", "").Split('\n').Select(c => c.Trim()).Distinct().ToList();
            int nam = Convert.ToInt32(Nam);
            int thang = Convert.ToInt32(Thang);
            if (nam < DateTime.Now.Year)
            {
                return Content("error");
            }
            else
            {
                if (thang < DateTime.Now.Month)
                {
                    return Content("error");
                }
            }
            if (_phieuchuyenphatService.GetLaiPhuSoXe(thang, nam, LoaiLaiPhuSoXe.LAI_XE).Count() > 0)
            {
                var listLaiPhuSoXe = _phieuchuyenphatService.GetLaiPhuSoXe(thang, nam, LoaiLaiPhuSoXe.LAI_XE);
                foreach (var item in listLaiPhuSoXe)
                {
                    _phieuchuyenphatService.DeleteLaiPhuSoXe(item);
                }

            }
            foreach (var item in arrLaiXe)
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                var laiphusoxe = new DB_LaiPhuSoXe();
                laiphusoxe.Ten = item;
                laiphusoxe.LoaiId = (int)LoaiLaiPhuSoXe.LAI_XE;
                laiphusoxe.Nam = nam;
                laiphusoxe.Thang = thang;
                laiphusoxe.NgayTao = DateTime.Now;
                _phieuchuyenphatService.InsertLaiPhuSoXe(laiphusoxe);
            }

            if (_phieuchuyenphatService.GetLaiPhuSoXe(thang, nam, LoaiLaiPhuSoXe.PHU_XE).Count() > 0)
            {
                var listLaiPhuSoXe = _phieuchuyenphatService.GetLaiPhuSoXe(thang, nam, LoaiLaiPhuSoXe.PHU_XE);
                foreach (var item in listLaiPhuSoXe)
                {
                    _phieuchuyenphatService.DeleteLaiPhuSoXe(item);
                }
            }
            foreach (var item in arrPhuXe)
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                var laiphusoxe = new DB_LaiPhuSoXe();
                laiphusoxe.Ten = item;
                laiphusoxe.LoaiId = (int)LoaiLaiPhuSoXe.PHU_XE;
                laiphusoxe.Nam = nam;
                laiphusoxe.Thang = thang;
                laiphusoxe.NgayTao = DateTime.Now;
                _phieuchuyenphatService.InsertLaiPhuSoXe(laiphusoxe);
            }

            if (_phieuchuyenphatService.GetLaiPhuSoXe(thang, nam, LoaiLaiPhuSoXe.SO_XE).Count() > 0)
            {
                var listLaiPhuSoXe = _phieuchuyenphatService.GetLaiPhuSoXe(thang, nam, LoaiLaiPhuSoXe.SO_XE);
                foreach (var item in listLaiPhuSoXe)
                {
                    _phieuchuyenphatService.DeleteLaiPhuSoXe(item);
                }
            }
            foreach (var item in arrSoXe)
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                var laiphusoxe = new DB_LaiPhuSoXe();
                laiphusoxe.Ten = item;
                laiphusoxe.LoaiId = (int)LoaiLaiPhuSoXe.SO_XE;
                laiphusoxe.Nam = nam;
                laiphusoxe.Thang = thang;
                laiphusoxe.NgayTao = DateTime.Now;
                _phieuchuyenphatService.InsertLaiPhuSoXe(laiphusoxe);
            }
            return Content("success");
        }
        public ActionResult QuanLyGioMoLenh()
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            var model = new DB_GioMoLenhModel();
            model.ThangId = DateTime.Now.Month;
            model.NamId = DateTime.Now.Year;

            for (int i = 2015; i <= DateTime.Now.Year + 1; i++)
            {
                model.ListNam.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = (i == DateTime.Now.Year) });
            }
            for (int i = 1; i <= 12; i++)
            {
                model.ListThang.Add(new SelectListItem { Text = "Tháng " + i.ToString(), Value = i.ToString(), Selected = (i == DateTime.Now.Month) });
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult GetGioMoLenh(string Nam, string Thang)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            int nam = Convert.ToInt32(Nam);
            int thang = Convert.ToInt32(Thang);
            var listGioMoLenh = new DB_GioMoLenhModel();
            if (nam == DateTime.Now.Year)
            {
                if (thang >= DateTime.Now.Month)
                {
                    listGioMoLenh.isEnable = true;
                }
                else
                {
                    listGioMoLenh.isEnable = false;
                }
            }
            else if (nam > DateTime.Now.Year)
            {
                listGioMoLenh.isEnable = true;
            }
            else
            {
                listGioMoLenh.isEnable = false;
            }
            var listBenXe = _benxeService.GetAllBenXe();
            var listData = new List<DB_GioMoLenhModel.BenXe>();
            foreach (var item in listBenXe)
            {
                var benxe = new DB_GioMoLenhModel.BenXe();
                benxe.TenBenXe = item.TenBenXe;
                benxe.BenXeId = item.Id;
                benxe.GioMoLenh = _phieuchuyenphatService.GetGioMoLenh(thang, nam, item.Id).Select(c => c.GioMoLenh).ToList();
                listData.Add(benxe);
            }
            listGioMoLenh.ListBenXe = listData;
            return Json(listGioMoLenh);
        }
        [HttpPost]
        public ActionResult InsertGioMoLenh(string Nam, string Thang, string str)
        {
            if (!this.isRightAccess(_permissionService, StandardPermissionProvider.CVQLChuyen))
                return AccessDeniedView();
            int nam = Convert.ToInt32(Nam);
            int thang = Convert.ToInt32(Thang);
            if (nam < DateTime.Now.Year)
            {
                return Content("error");
            }
            else if (nam == DateTime.Now.Year)
            {
                if (thang < DateTime.Now.Month)
                {
                    return Content("error");
                }
            }

            JavaScriptSerializer j = new JavaScriptSerializer();
            var listGioMoLenh = new List<DB_GioMoLenhModel.GioMoLenh>();
            listGioMoLenh = j.Deserialize<List<DB_GioMoLenhModel.GioMoLenh>>(str);

            foreach (var item in listGioMoLenh)
            {
                if (_phieuchuyenphatService.GetGioMoLenh(thang, nam, item.BenXeId).Count() > 0)
                {
                    var list = _phieuchuyenphatService.GetGioMoLenh(thang, nam, item.BenXeId);
                    foreach (var item1 in list)
                    {
                        _phieuchuyenphatService.DeleteGioMoLenh(item1);
                    }
                }
                var arrGioMoLenh = item.Ten.Replace("\r", "").Split('\n').Select(c => c.Trim()).Distinct().ToList();
                foreach (var item1 in arrGioMoLenh)
                {
                    if (string.IsNullOrEmpty(item1))
                        continue;
                    var giomolenh = new DB_GioMoLenh();
                    giomolenh.Thang = thang;
                    giomolenh.Nam = nam;
                    giomolenh.NgayTao = DateTime.Now;
                    giomolenh.GioMoLenh = item1;
                    giomolenh.BenXeId = item.BenXeId;
                    _phieuchuyenphatService.InsertGioMoLenh(giomolenh);
                }
            }
            return Content("success");
        }
        #endregion

    }
}