﻿using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Chonves;
using Nop.Core.Domain.ChuyenPhatNhanh;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.NhaXes;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Plugin.NhaXeAPI.Models;
using Nop.Services.Authentication;
using Nop.Services.Catalog;
using Nop.Services.Chonves;
using Nop.Services.ChuyenPhatNhanh;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.NhaXes;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Nop.Plugin.NhaXeAPI.Controllers
{
    public class ApiController : BaseController
    {
        #region Fields
        private readonly RestServiceSettings _settings;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly INhaXeService _nhaxeService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IPhieuGuiHangService _phieuguihangService;
        private readonly IHangHoaService _hanghoaService;
        private readonly ICustomerService _customerService;
        private readonly IChonVeService _chonveService;
        private readonly IDiaChiService _diachiService;
        private readonly INhanVienService _nhanvienService;
        private readonly IPermissionService _permissionService;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IXeInfoService _xeinfoService;
        private readonly IHanhTrinhService _hanhtrinhService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IBenXeService _benxeService;
        private readonly IVeXeService _vexeService;
        private readonly IPhoiVeService _phoiveService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IAuthenticationService _authenticationService;
        private readonly INhaXeCustomerService _nhaxecustomerService;
        private readonly IGiaoDichKeVeXeService _giaodichkeveService;
        private readonly IPhieuChuyenPhatService _phieuchuyenphatService;
        private Customer _customer;
        private NhanVien _nhanvien;
        private HistoryXeXuatBen xexuatben;
        int _customerId = 0;
        static List<CustomerBenXeModel> dicCustomerBenXe = new List<CustomerBenXeModel>();
        #endregion

        #region Ctor

        public ApiController(
            IPhieuChuyenPhatService phieuchuyenphatService,
            RestServiceSettings settings,
            IStateProvinceService stateProvinceService,
            INhaXeService nhaxeService,
            ILocalizationService localizationService,
            IWorkContext workContext,
             IPhieuGuiHangService phieuguihangService,
            IHangHoaService hanghoaService,
            ICustomerService customerService,
            IChonVeService chonveService,
            IDiaChiService diachiService,
            INhanVienService nhanvienService,
            IPermissionService permissionService,
            CustomerSettings customerSettings,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            IXeInfoService xeinfoService,
            IHanhTrinhService hanhtrinhService,
            IPriceFormatter priceFormatter,
            IBenXeService benxeService,
             IVeXeService vexeService,
            IPhoiVeService phoiveService,
            IShoppingCartService shoppingCartService,
            IAuthenticationService authenticationService,
            INhaXeCustomerService nhaxecustomerService,
            IGiaoDichKeVeXeService giaodichkeveService
            )
        {
            this._phieuchuyenphatService = phieuchuyenphatService;
            this._giaodichkeveService = giaodichkeveService;
            this._settings = settings;
            this._stateProvinceService = stateProvinceService;
            this._nhaxeService = nhaxeService;
            this._hanghoaService = hanghoaService;
            this._phieuguihangService = phieuguihangService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._customerService = customerService;
            this._chonveService = chonveService;
            this._diachiService = diachiService;
            this._nhanvienService = nhanvienService;
            this._permissionService = permissionService;
            this._customerSettings = customerSettings;
            this._customerRegistrationService = customerRegistrationService;
            this._customerActivityService = customerActivityService;
            this._genericAttributeService = genericAttributeService;
            this._xeinfoService = xeinfoService;
            this._hanhtrinhService = hanhtrinhService;
            this._priceFormatter = priceFormatter;
            this._benxeService = benxeService;
            this._vexeService = vexeService;
            this._phoiveService = phoiveService;
            this._shoppingCartService = shoppingCartService;
            this._authenticationService = authenticationService;
            this._nhaxecustomerService = nhaxecustomerService;

        }

        #endregion
        #region Common
        void AddCustomerBenXeModel(int _cusId,int _benxeId)
        {
            if (dicCustomerBenXe.Any(c => c.CustomerId == _cusId))
                return;
            dicCustomerBenXe.Add(new CustomerBenXeModel(_cusId, _benxeId));
        }
        int? getBenXeId(int _cusId)
        {
            var item = dicCustomerBenXe.Where(c => c.CustomerId == _cusId).FirstOrDefault();
            if (item == null)
                return null;
            return item.BenXeId;
        }
        Customer currentCustomer
        {
            get
            {
                if (_customer != null)
                    return _customer;
                _customer = _customerService.GetCustomerById(_customerId);
                return _customer;
            }
        }
        NhanVien currentNhanVien
        {
            get
            {
                if (_nhanvien != null)
                    return _nhanvien;
                _nhanvien = _nhanvienService.GetByCustomerId(_customerId);
                return _nhanvien;
            }
        }

        private bool isRightAccess()
        {
            if (_permissionService.Authorize(StandardPermissionProvider.CVHoatDongBanVeKyGuiTrenTuyen, currentCustomer))
                return true;
            return false;
        }

        string isAuthentication(int NhaXeId, int CustomerId, string apiToken)
        {
            if (!IsApiTokenValid(apiToken))
                return "Không đúng apiToken";
            _customerId = CustomerId;
            //kiem tra co quyen ko 
            if (!isRightAccess())
            {
                return "Bạn không có quyền vào chức năng này !";
            }
            //kiem tra thong tin nhan vien
            _nhanvien = _nhanvienService.GetByCustomerId(CustomerId);
            if (_nhanvien == null)
                return "Nhân viên không tồn tại";
            //xem nhan vien co thuoc nha xe ko 
            if (_nhanvien.NhaXeID != NhaXeId)
                return "Nhân viên không thuộc nhà xe";
            return String.Empty;
        }
        string isAuthentication(int NhaXeId, int CustomerId, string apiToken, int XeXuatBenId)
        {
            string _isauthen = isAuthentication(NhaXeId, CustomerId, apiToken);
            if (!String.IsNullOrEmpty(_isauthen))
                return _isauthen;
            xexuatben = _nhaxeService.GetHistoryXeXuatBenId(XeXuatBenId);
            if (xexuatben == null)
                return "Không tồn tại thông tin xe xuất bến";
            if (xexuatben.NguonVeInfo.NhaXeId != NhaXeId)
                return "Xe xuất bến không thuộc nhà xe";
            return String.Empty;
        }
        #endregion
        #region Customers

        public ActionResult Login(string Email, string Password)
        {
            //BienSoXe = BienSoXe + "@chonve.vn";
            var loginresult = _customerRegistrationService.ValidateCustomer(Email, Password);
            if (loginresult != CustomerLoginResults.Successful)
            {
                return ErrorOccured("Tài khoản hoặc mật khẩu không đúng !");
            }
            _customer = _customerService.GetCustomerByEmail(Email);

            //kiem tra co quyen ko 
            if (!isRightAccess())
            {
                return ErrorOccured("Bạn không có quyền vào chức năng này !");
            }
            //kiem tra cac thong tin nha xe
            NhaXe currentNhaXe = _nhaxeService.GetNhaXeByCustommerId(currentCustomer.Id);
            if (currentNhaXe == null)
            {
                return ErrorOccured("Không xác định nhà xe !");
            }
            //luu nhat ky
            _customerActivityService.InsertActivity("PublicStore.Login", "Tài khoản nhà xe đăng nhập", currentCustomer);
            //lay thong tin van phong
            _nhanvien = _nhanvienService.GetByCustomerId(currentCustomer.Id);
            //OK, lay thong tin va truyen xuong client
            var loginInfo = new
            {
                Id = currentCustomer.Id,
                NhaXeId = currentNhaXe.Id,
                GuidId = _settings.ApiToken,
                FullName = currentCustomer.GetFullName(),
                TenNhaXe = currentNhaXe.TenNhaXe,
                VanPhongId = _nhanvien.VanPhongID.GetValueOrDefault(0)
            };
            return Successful(loginInfo);
        }
        /// <summary>
        /// Lay id xe tu bien so xe
        /// </summary>
        /// <param name="NhaXeId"></param>
        /// <param name="CustomerId"></param>
        /// <param name="BienSoXe"></param>
        /// <param name="apiToken"></param>
        /// <returns></returns>
        public ActionResult GetXeInfo(int NhaXeId, int CustomerId, string BienSoXe, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);

            //lay thong tin xe van chuyen tu bien so xe
            var xevanchuyen = _xeinfoService.GetXeInfoByBienSo(NhaXeId, BienSoXe);
            if (xevanchuyen == null)
                return ErrorOccured("Không tìm thấy xe có biển số này trong nhà xe");
            return SuccessfulSimple(xevanchuyen.Id.ToString());
        }

        /// <summary>
        /// Lay thong ti chuyen di hien tai theo bien so xe
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <param name="NhaXeId"></param>
        /// <param name="BienSoXe"></param>
        /// <param name="apiToken"></param>
        /// <returns></returns>
        public ActionResult GetChuyenDiHienTai(int NhaXeId, int CustomerId, int XeId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);

            //lay thong tin xe van chuyen tu bien so xe
            var xevanchuyen = _xeinfoService.GetXeInfoById(XeId);
            if (xevanchuyen == null)
                return ErrorOccured("Xe không tồn tại");

            var chuyendis = _nhaxeService.GetHistoryXeXuatBenByXeVanChuyen(xevanchuyen.Id);
            if (chuyendis.Count == 0)
                return ErrorOccured("Không có chuyến đi nào tại thời điểm này");

            //Lấy thông tin cac chuyen di
            var chuyendijson = chuyendis.Select(c => new
            {
                Id = c.Id,
                NgayDi = c.NgayDi.toStringDate(),
                GioXuatBen = c.NguonVeInfo.LichTrinhInfo.ThoiGianDi.ToString("HH:mm"),
                LaiXe = c.ThongTinLaiPhuXe(0, true),
                ThongTin = c.NguonVeInfo.GetHanhTrinh(),
                SoHangGhe = xevanchuyen.loaixe.sodoghe.SoHang,
                SoCotGhe = xevanchuyen.loaixe.sodoghe.SoCot,
                SoTang = (int)xevanchuyen.loaixe.KieuXe,
                TrangThaiId = c.TrangThaiId,
                HanhTrinhId = c.NguonVeInfo.LichTrinhInfo.HanhTrinhId
            }).ToList();
            return Successful(chuyendijson);
        }
        public ActionResult GetAllHanhTrinhDiemDon(int NhaXeId, int CustomerId, int HanhTrinhId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);
            //lay thong tin hanh trinh           

            var hanhtrinhinfo = _hanhtrinhService.GetHanhTrinhById(HanhTrinhId);
            if (hanhtrinhinfo == null)
                return ErrorOccured("Hành trình không tồn tại");

            var diemdons = hanhtrinhinfo.DiemDons.OrderBy(c => c.ThuTu).Select(d => new
            {
                Id = d.DiemDonId,
                HanhTrinhId = d.HanhTrinhId,
                TenDiemDon = d.diemdon.TenDiemDon,
                KhoangCach = d.KhoangCach
            }).ToList();
            return Successful(diemdons);
        }

        /// <summary>
        /// Lay thong tin hanh trinh gia ve
        /// </summary>
        /// <param name="NhaXeId"></param>
        /// <param name="CustomerId"></param>
        /// <param name="XeXuatBenId"></param>
        /// <param name="apiToken"></param>
        /// <returns></returns>
        public ActionResult GetAllHanhTrinhGiaVe(int NhaXeId, int CustomerId, int HanhTrinhId, int XeXuatBenId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);
            //lay tat ca thong tin phoi ve
            var phoives = _phoiveService.GetPhoiVeByChuyenDi(xexuatben.NguonVeId, xexuatben.NgayDi);
            //lay thong tin hanh trinh
            var hanhtrinhves = _hanhtrinhService.GetallHanhTrinhGiaVe(HanhTrinhId, NhaXeId);
            var hanhtrinhvesjson = hanhtrinhves.Select(c => new
            {
                Id = c.Id,
                HanhTrinhId = c.HanhTrinhId,
                DiemDonId = c.DiemDonId,
                DiemDenId = c.DiemDenId,
                GiaVe = Convert.ToInt32(c.GiaVe),
                SoLuong = phoives.Count(pv => pv.ChangId == c.Id)
            }).ToList();
            return Successful(hanhtrinhvesjson);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="NhaXeId"></param>
        /// <param name="CustomerId"></param>
        /// <param name="HanhTrinhId"></param>
        /// <param name="apiToken"></param>
        /// <returns></returns>
        public ActionResult GetAllVeXeDaBan(int NhaXeId, int CustomerId, int HanhTrinhId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);
            var vexeitems = _giaodichkeveService.GetVeXeBanByDay(NhaXeId, DateTime.Now, HanhTrinhId, 0, ENVeXeItemTrangThai.DA_BAN);
            var vexeitemjson = vexeitems.Select(c => new
            {
                Id = c.Id,
                SoSeri = c.SoSeri,
                ChangId = c.ChangId
            }).ToList();
            return Successful(vexeitemjson);
        }
        public ActionResult GetSoDoGheXe(int NhaXeId, int CustomerId, int XeXuatBenId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);

            //lay thong tin so do ghe xe
            var sodoghequytacs = _xeinfoService.GetAllSoDoGheXeQuyTac(xexuatben.xevanchuyen.LoaiXeId);
            var phoives = new List<PhoiVeMobileModel>();
            //kiem tra trang thai tung ghe tren so do
            foreach (var sodo in sodoghequytacs)
            {
                var item = new PhoiVeMobileModel();
                //thong tin ghe/vi tri tri
                item.Val = sodo.Val;
                item.x = sodo.x;
                item.y = sodo.y;
                item.Tang = sodo.Tang;
                item.SoDoGheXeQuyTacId = sodo.Id;
                //thong tin trang thai vi tri
                var phoive = _phoiveService.GetPhoiVe(xexuatben.NguonVeId, sodo, xexuatben.NgayDi, true);
                item.Id = phoive.Id;
                item.TrangThaiId = phoive.TrangThaiId;
                item.NguonVeXeId = xexuatben.NguonVeId;
                item.NgayDi = phoive.NgayDi.toStringDate();
                item.GiaVe = Convert.ToInt32(phoive.GiaVeHienTai);
                item.CustomerId = phoive.CustomerId;

                item.ChangId = phoive.ChangId.GetValueOrDefault(0);
                item.VeXeItemId = phoive.VeXeItemId.GetValueOrDefault(0);
                item.MaVe = phoive.MaVe;

                if (phoive.customer != null)
                {
                    item.TenKhachHang = phoive.customer.GetFullName();
                    item.SoDienThoai = phoive.customer.GetPhone();
                }
                if (String.IsNullOrEmpty(phoive.ViTriXuongXe) && phoive.nguonvexe != null)
                    item.ViTriXuong = phoive.nguonvexe.GetDiemDen();
                else
                {
                    if (phoive.nguonvexe != null)
                        item.ViTriXuong = phoive.ViTriXuongXe;
                }
                if (phoive.nguonvexecon != null)
                {
                    item.ViTriXuong = phoive.nguonvexecon.TenDiemDen;
                }
                phoives.Add(item);
            }

            return Successful(phoives);
        }
        public ActionResult LenXe(int NhaXeId, int CustomerId, int XeXuatBenId, int SoDoGheXeQuyTacId, int GiaTien, string DiemLen, string DiemXuong, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);


            var sodo = _xeinfoService.GetSoDoGheXeQuyTacById(SoDoGheXeQuyTacId);
            if (sodo == null)
                return ErrorOccured("Dữ liệu không hợp lệ");

            var phoive = _phoiveService.GetPhoiVe(xexuatben.NguonVeId, sodo, xexuatben.NgayDi, true);
            phoive.GiaVeHienTai = Convert.ToDecimal(GiaTien);
            phoive.ViTriLenXe = DiemLen;
            phoive.ViTriXuongXe = DiemXuong;

            phoive.NguoiDatVeId = currentNhanVien.Id;
            phoive.CustomerId = CommonHelper.KhachVangLaiId;//khach vang lai
            if (_phoiveService.DatVe(phoive, ENTrangThaiPhoiVe.DaGiaoHang))
            {
                return SuccessfulSimple(phoive.Id.ToString());
            }
            return ErrorOccured("Không đặt được ở vị trí này");
        }
        /// <summary>
        /// Su dung trong truong hop len xe o nha xe Viet Thanh
        /// </summary>
        /// <param name="NhaXeId"></param>
        /// <param name="CustomerId"></param>
        /// <param name="XeXuatBenId"></param>
        /// <param name="SoDoGheXeQuyTacId"></param>
        /// <param name="HanhTrinhGiaVeId"></param>
        /// <param name="DiemLen"></param>
        /// <param name="apiToken"></param>
        /// <returns></returns>
        static void cv_DownloadStringCompleted(object sender,
                  DownloadStringCompletedEventArgs e)
        {
            var xmlElm = XElement.Parse(e.Result);

            var status = (from elm in xmlElm.Descendants()
                          where elm.Name == "status"
                          select elm).FirstOrDefault();
            if (status.Value.ToLower() == "ok")
            {
                var res = (from elm in xmlElm.Descendants()
                           where elm.Name == "formatted_address"
                           select elm).FirstOrDefault();
                Console.WriteLine(res.Value);
            }
            else
            {
                Console.WriteLine("No Address Found");
            }
        }
        public ActionResult LenXeV1(int NhaXeId, int CustomerId, int XeXuatBenId, int SoDoGheXeQuyTacId, int HanhTrinhGiaVeId, string SoSeri, string DiemLen, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);
            //kiem tra trang thai xe xuat ben
            if (xexuatben.TrangThai == ENTrangThaiXeXuatBen.KET_THUC)
            {
                return ErrorOccured("Chuyến đi đã kết thúc");
            }
            var giave = _hanhtrinhService.GetHanhTrinhGiaVeId(HanhTrinhGiaVeId);
            if (giave == null)
                return ErrorOccured("Dữ liệu không hợp lệ");
            var hanhtrinh = _hanhtrinhService.GetHanhTrinhById(giave.HanhTrinhId);
            var phoive = new PhoiVe();

            if (SoDoGheXeQuyTacId > 0)
            {
                var sodo = _xeinfoService.GetSoDoGheXeQuyTacById(SoDoGheXeQuyTacId);
                if (sodo == null)
                    return ErrorOccured("Dữ liệu không hợp lệ");
                phoive = _phoiveService.GetPhoiVe(xexuatben.NguonVeId, sodo, xexuatben.NgayDi, true);
            }
            else
            {
                phoive.NguonVeXeId = xexuatben.NguonVeId;
                phoive.NgayDi = xexuatben.NgayDi;
                phoive.TrangThai = ENTrangThaiPhoiVe.ConTrong;
            }
            phoive.GiaVeHienTai = giave.GiaVe;
            phoive.ChangId = HanhTrinhGiaVeId;
            phoive.ViTriLenXe = DiemLen;
            phoive.ViTriXuongXe = giave.DiemDen.TenDiemDon;


            phoive.NguoiDatVeId = currentNhanVien.Id;
            phoive.CustomerId = CommonHelper.KhachVangLaiId;//khach vang lai
            phoive.ChuyenDiId = XeXuatBenId;
            if (!string.IsNullOrEmpty(SoSeri))
            {
                //su dung ve da ban o quay de len xe
                var vexeitemsold = _giaodichkeveService.SuDungVe(NhaXeId, xexuatben.NguonVeId, SoSeri, true);
                if (vexeitemsold != null)
                {
                    phoive.MaVe = vexeitemsold.SoSeri;
                    phoive.VeXeItemId = vexeitemsold.Id;
                    phoive.ChangId = vexeitemsold.ChangId.GetValueOrDefault(0);
                }
                else
                {
                    return ErrorOccured("Seri vé không hợp lệ");
                }

            }
            else
            {
                //lay thong tin ve xe item theo menh gia, va thuc hien ban ve
                var vexeitemsold = _giaodichkeveService.BanVe(NhaXeId, currentNhanVien.Id, XeXuatBenId, xexuatben.NguonVeId, HanhTrinhGiaVeId, giave.GiaVe, true);
                if (vexeitemsold != null)
                {
                    phoive.MaVe = vexeitemsold.SoSeri;
                    phoive.VeXeItemId = vexeitemsold.Id;
                    phoive.ChangId = HanhTrinhGiaVeId;
                }
                else
                {
                    return ErrorOccured("Hết vé");
                }
            }


            if (_phoiveService.DatVe(phoive, ENTrangThaiPhoiVe.DaGiaoHang))
            {
                var datveinfo = new
                {
                    Id = phoive.Id,
                    MaVe = phoive.MaVe,
                    VeXeItemId = phoive.VeXeItemId,
                    HanhTrinhGiaVeId = phoive.ChangId
                };
                return Successful(datveinfo);
            }
            return ErrorOccured("Không đặt được ở vị trí này");
        }
        /// <summary>
        /// Su dung trong truong hop nha xe ko quan tam toi so do, so tien tuy bien
        /// </summary>
        /// <param name="NhaXeId"></param>
        /// <param name="CustomerId"></param>
        /// <param name="XeXuatBenId"></param>
        /// <param name="HanhTrinhGiaVeId"></param>
        /// <param name="DiemLen"></param>
        /// <param name="apiToken"></param>
        /// <returns></returns>
        public ActionResult LenXeV2(int NhaXeId, int CustomerId, int XeXuatBenId, int HanhTrinhGiaVeId, int GiaTien, string DiemLen, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);
            //kiem tra trang thai xe xuat ben
            if (xexuatben.TrangThai == ENTrangThaiXeXuatBen.KET_THUC)
            {
                return ErrorOccured("Chuyến đi đã kết thúc");
            }
            var giave = _hanhtrinhService.GetHanhTrinhGiaVeId(HanhTrinhGiaVeId);
            if (giave == null)
                return ErrorOccured("Dữ liệu không hợp lệ");
            var hanhtrinh = _hanhtrinhService.GetHanhTrinhById(giave.HanhTrinhId);
            var phoive = new PhoiVe();
            phoive.NguonVeXeId = xexuatben.NguonVeId;
            phoive.NgayDi = xexuatben.NgayDi;
            phoive.TrangThai = ENTrangThaiPhoiVe.ConTrong;

            phoive.GiaVeHienTai = Convert.ToDecimal(GiaTien);
            phoive.ChangId = HanhTrinhGiaVeId;
            phoive.ViTriLenXe = DiemLen;
            phoive.ViTriXuongXe = giave.DiemDen.TenDiemDon;

            phoive.NguoiDatVeId = currentNhanVien.Id;
            phoive.CustomerId = CommonHelper.KhachVangLaiId;//khach vang lai


            if (_phoiveService.DatVe(phoive, ENTrangThaiPhoiVe.DaGiaoHang))
            {
                var datveinfo = new
                {
                    Id = phoive.Id,
                    MaVe = phoive.MaVe,
                    VeXeItemId = phoive.VeXeItemId,
                    HanhTrinhGiaVeId = phoive.ChangId
                };
                return Successful(datveinfo);
            }
            return ErrorOccured("Không đặt được ở vị trí này");
        }
        /// <summary>
        /// Co 2 truong hop luc xuong la: xuong that, hoac huy ve
        /// </summary>
        /// <param name="NhaXeId"></param>
        /// <param name="CustomerId"></param>
        /// <param name="XeXuatBenId"></param>
        /// <param name="PhoiVeId"></param>
        /// <param name="ViTriXuong"></param>
        /// <param name="isXuongXe"></param>
        /// <param name="apiToken"></param>
        /// <returns></returns>
        public ActionResult XuongXe(int NhaXeId, int CustomerId, int XeXuatBenId, int PhoiVeId, string ViTriXuong, int isXuongXe, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);

            var phoive = _phoiveService.GetPhoiVeById(PhoiVeId);
            if (isXuongXe == 1)
                phoive.TrangThai = ENTrangThaiPhoiVe.KetThuc;
            else
            {
                //truong hop la huy ve
                //kiem tra phai cung nguoi dat ve thi moi dc huy
                if (phoive.NguoiDatVeId != currentNhanVien.Id)
                    return ErrorOccured("Bạn không thể hủy vé này");
                //kiem tra thoi gian, qua thoi gian 30p thi ko dc huy
                if (phoive.NgayTao.AddMinutes(30) < DateTime.Now)
                    return ErrorOccured("Quá thời hạn hủy 30 phút, bạn không thể hủy vé này");
                phoive.TrangThai = ENTrangThaiPhoiVe.Huy;
                //cap nhat thong tin vexe item neu co
                if (phoive.VeXeItemId > 0)
                {
                    _giaodichkeveService.HuyBanVe(NhaXeId, currentNhanVien.Id, phoive.VeXeItemId.GetValueOrDefault(0));
                }
            }

            phoive.ViTriXuongXe = ViTriXuong;
            _phoiveService.UpdatePhoiVe(phoive);
            return SuccessfulSimple("OK");
        }
        public ActionResult ChuyenCho(int NhaXeId, int CustomerId, int XeXuatBenId, int PhoiVeId, int SoDoGheXeQuyTacId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);
            var phoive = _phoiveService.GetPhoiVeById(PhoiVeId);
            phoive.SoDoGheXeQuyTacId = SoDoGheXeQuyTacId;
            _phoiveService.UpdatePhoiVe(phoive);
            return SuccessfulSimple("OK");
        }
        /// <summary>
        /// cap nhat vi tri dinh vi cua xe, ko can xac thuc
        /// Dinh ky chay tam 2p update 1 lan
        /// </summary>
        /// <param name="XeId"></param>
        /// <param name="Latitude">dang chuoi so /1000.000.000</param>
        /// <param name="Longitude"></param>
        /// <returns></returns>
        public ActionResult CapNhatViTri(int XeId, string Latitude, string Longitude)
        {
            if (Latitude == "0" || Longitude == "0")
                return ErrorOccured("Vị trí ko hợp lệ");
            //lay thong tin xe van chuyen tu bien so xe
            var xevanchuyen = _xeinfoService.GetXeInfoById(XeId);
            if (xevanchuyen == null)
                return ErrorOccured("Xe không tồn tại");

            xevanchuyen.Latitude = Convert.ToDecimal(Latitude) / 1000000000m;
            xevanchuyen.Longitude = Convert.ToDecimal(Longitude) / 1000000000m;
            xevanchuyen.NgayGPS = DateTime.Now;
            if (xevanchuyen.Latitude == Decimal.Zero || xevanchuyen.Longitude == Decimal.Zero)
                return ErrorOccured("Vị trí ko hợp lệ");
            _xeinfoService.UpdateXeInfo(xevanchuyen);
            return SuccessfulSimple("OK");


        }
        public ActionResult XeKhoiHanh(int NhaXeId, int CustomerId, int XeXuatBenId, string apiToken)
        {
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);
            _nhaxeService.UpdateHistoryXeXuatBenTrangThai(XeXuatBenId, currentNhanVien.Id, ENTrangThaiXeXuatBen.DANG_DI);
            return SuccessfulSimple("OK");
        }
        public ActionResult XeVeBen(int NhaXeId, int CustomerId, int XeXuatBenId, string apiToken)
        {
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);
            _nhaxeService.UpdateHistoryXeXuatBenTrangThai(XeXuatBenId, currentNhanVien.Id, ENTrangThaiXeXuatBen.KET_THUC);
            return SuccessfulSimple("OK");
        }
        #endregion

        #region "Hang hoa"
        public ActionResult GetHangHoaTrenXe(int NhaXeId, int CustomerId, int XeXuatBenId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);

            //lay thong tin hang hoa tren xe
            var phieuguihangs = _phieuguihangService.GetAll(NhaXeId, XeXuatBenId, 0, 0, ENTinhTrangVanChuyen.DangVanChuyen);
            var arrpgh = phieuguihangs.Select(pgh =>
            {
                var item = new PhieuGuiHangMobileModel();
                item.Id = pgh.Id;
                item.MaPhieu = pgh.MaPhieu;
                item.NguoiGuiTen = pgh.NguoiGui.HoTen;
                item.NguoiGuiDienThoai = pgh.NguoiGui.DienThoai;
                item.DiemGui = pgh.DiemGui;
                item.NguoiNhanTen = pgh.NguoiNhan.HoTen;
                item.NguoiNhanDienThoai = pgh.NguoiNhan.DienThoai;
                item.DiemTra = pgh.DiemTra;
                item.TenHang = pgh.ThongTinHanHoa();
                item.SoTien = Convert.ToInt32(pgh.HangHoas.Sum(c => c.GiaCuoc * c.SoLuong));
                return item;
            }).ToList();

            return Successful(arrpgh);
        }

        /// <summary>
        /// So tien <0 co nghia la chua thu cuoc
        /// </summary>
        /// <param name="NhaXeId"></param>
        /// <param name="CustomerId"></param>
        /// <param name="XeXuatBenId"></param>
        /// <param name="GuiTen"></param>
        /// <param name="GuiSDT"></param>
        /// <param name="NhanTen"></param>
        /// <param name="NhanSDT"></param>
        /// <param name="DiemGui"></param>
        /// <param name="DiemTra"></param>
        /// <param name="TenHang"></param>
        /// <param name="SoTien"></param>
        /// <param name="apiToken"></param>
        /// <returns></returns>
        public ActionResult NhanHang(int NhaXeId, int CustomerId, int XeXuatBenId
            , string GuiTen
            , string GuiSDT
            , string NhanTen
            , string NhanSDT
            , string DiemGui
            , string DiemTra
            , string TenHang
            , int SoTien
            , string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);

            var phieugui = new PhieuGuiHang();
            //them nguoi gui
            var nguoigui = _nhaxecustomerService.CreateNew(NhaXeId, GuiTen, GuiSDT, DiemGui);
            //them nguoi nhan
            var nguoinhan = _nhaxecustomerService.CreateNew(NhaXeId, NhanTen, NhanSDT, DiemTra);
            //them phieu gui hàng               
            phieugui.NhaXeId = NhaXeId;
            phieugui.XeXuatBenId = XeXuatBenId;
            phieugui.NguoiGuiId = nguoigui.Id;
            phieugui.NguoiNhanId = nguoinhan.Id;
            phieugui.VanPhongGuiId = currentNhanVien.VanPhongID.GetValueOrDefault();
            phieugui.VanPhongNhanId = currentNhanVien.VanPhongID.GetValueOrDefault();
            phieugui.NguoiTaoId = currentNhanVien.Id;
            phieugui.NguoiKiemTraHangId = currentNhanVien.Id;
            phieugui.TinhTrangVanChuyen = ENTinhTrangVanChuyen.DangVanChuyen;
            phieugui.NgayTao = DateTime.Now;
            phieugui.NgayUpdate = DateTime.Now;
            phieugui.GhiChu = "Hàng hóa được nhận trên quá trình di chuyển";
            phieugui.DiemGui = DiemGui;
            phieugui.DiemTra = DiemTra;
            if (SoTien > 0)
            {
                phieugui.DaThuCuoc = true;
                phieugui.NgayThanhToan = DateTime.Now;
                phieugui.NhanVienThuTienId = currentNhanVien.Id;
            }
            else
                phieugui.DaThuCuoc = false;

            _phieuguihangService.InsertPhieuGuiHang(phieugui);
            //them hàng hóa
            var hanghoa = new HangHoa();
            hanghoa.TenHangHoa = TenHang;
            hanghoa.LoaiHangHoa = ENLoaiHangHoa.LoaiKhac;
            hanghoa.GiaCuoc = Math.Abs(SoTien);
            hanghoa.SoLuong = 1;
            hanghoa.PhieuGuiHangId = phieugui.Id;
            _hanghoaService.InsertHangHoa(hanghoa);

            return SuccessfulSimple(phieugui.Id.ToString());
        }
        /// <summary>
        /// Co 2 truong hop luc xuong la: tra that, hoac huy hang
        /// </summary>
        /// <param name="NhaXeId"></param>
        /// <param name="CustomerId"></param>
        /// <param name="XeXuatBenId"></param>
        /// <param name="PhieuGuiHangId"></param>
        /// <param name="isTranhang"></param>
        /// <param name="apiToken"></param>
        /// <returns></returns>
        public ActionResult TraHang(int NhaXeId, int CustomerId, int XeXuatBenId, int PhieuGuiHangId, string DiemTra, int isTraHang, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);


            var phieuguihang = _phieuguihangService.GetPhieuGuiById(PhieuGuiHangId);
            if (phieuguihang == null)
                return ErrorOccured("Phiếu gửi hàng không hợp lệ");

            if (isTraHang == 1)
            {
                phieuguihang.DiemTra = DiemTra;
                if (!phieuguihang.DaThuCuoc)
                {
                    phieuguihang.DaThuCuoc = true;
                    phieuguihang.NgayThanhToan = DateTime.Now;
                    phieuguihang.NhanVienThuTienId = currentNhanVien.Id;

                }
                phieuguihang.TinhTrangVanChuyen = ENTinhTrangVanChuyen.KetThuc;
            }
            else
            {
                //truong hop la huy ve
                //kiem tra phai cung nguoi dat ve thi moi dc huy
                if (phieuguihang.NguoiTaoId != currentNhanVien.Id)
                    return ErrorOccured("Bạn không thể hủy phiếu này");
                //kiem tra thoi gian, qua thoi gian 30p thi ko dc huy
                if (phieuguihang.NgayTao.AddMinutes(30) < DateTime.Now)
                    return ErrorOccured("Quá thời hạn hủy 30 phút, bạn không thể hủy phiếu này");
                phieuguihang.TinhTrangVanChuyen = ENTinhTrangVanChuyen.Huy;
            }
            _phieuguihangService.UpdatePhieuGuiHang(phieuguihang);
            return SuccessfulSimple("OK");
        }
        #endregion
        #region Nhat ky
        public ActionResult GetNhatKy(int NhaXeId, int CustomerId, int XeXuatBenId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);
            var nhatkys = xexuatben.NhatKys.OrderByDescending(c => c.Id).Select(c => new
            {
                Id = c.Id,
                NgayTao = c.NgayTao.toStringDateTime(),
                GhiChu = c.GhiChu
            }).ToList();
            return Successful(nhatkys);
        }

        HistoryXeXuatBenLog CreateHistoryXeXuatBenLog(ENTrangThaiXeXuatBen trangthai, String ghichu, int XeXuatBenId, int NguoiTaoId)
        {
            var item = new HistoryXeXuatBenLog();
            item.TrangThai = trangthai;
            item.GhiChu = ghichu;
            item.XeXuatBenId = XeXuatBenId;
            item.NguoiTaoId = NguoiTaoId;
            _nhaxeService.InsertHistoryXeXuatBenLog(item);
            return item;
        }
        public ActionResult TaoNhatKy(int NhaXeId, int CustomerId, int XeXuatBenId
            , string GhiChu
            , string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);

            var item = CreateHistoryXeXuatBenLog(xexuatben.TrangThai, GhiChu, XeXuatBenId, currentNhanVien.Id);
            return SuccessfulSimple(item.Id.ToString());
        }
        /// <summary>
        /// Co 2 truong hop luc xuong la: tra that, hoac huy hang
        /// </summary>
        /// <param name="NhaXeId"></param>
        /// <param name="CustomerId"></param>
        /// <param name="XeXuatBenId"></param>
        /// <param name="PhieuGuiHangId"></param>
        /// <param name="isTranhang"></param>
        /// <param name="apiToken"></param>
        /// <returns></returns>
        public ActionResult SuaNhatKy(int NhaXeId, int CustomerId, int XeXuatBenId, int NhatKyId, string GhiChu, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);
            var item = _nhaxeService.GetHistoryXeXuatBenLogById(NhatKyId);
            if (item.NguoiTaoId != currentCustomer.Id)
                return ErrorOccured("Nhật ký không được phép sửa");
            item.GhiChu = GhiChu;
            _nhaxeService.UpdateHistoryXeXuatBenLog(item);
            return SuccessfulSimple("OK");
        }
        public ActionResult XoaNhatKy(int NhaXeId, int CustomerId, int XeXuatBenId, int NhatKyId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);
            var item = _nhaxeService.GetHistoryXeXuatBenLogById(NhatKyId);
            if (item.NguoiTaoId != currentCustomer.Id)
                return ErrorOccured("Nhật ký không được phép xóa");
            _nhaxeService.DeleteHistoryXeXuatBenLog(item);
            return SuccessfulSimple("OK");
        }
        public ActionResult ChuyenTrangThaiXe(int NhaXeId, int CustomerId, int XeXuatBenId, int TrangThaiId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken, XeXuatBenId);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);

            xexuatben.TrangThai = (ENTrangThaiXeXuatBen)TrangThaiId;
            _nhaxeService.UpdateHistoryXeXuatBen(xexuatben);
            switch (xexuatben.TrangThai)
            {
                case ENTrangThaiXeXuatBen.DANG_DI:
                    {
                        CreateHistoryXeXuatBenLog(xexuatben.TrangThai, "Xe xuất bến, bắt đầu hành trình", XeXuatBenId, currentNhanVien.Id);
                        break;
                    }
                case ENTrangThaiXeXuatBen.KET_THUC:
                    {
                        CreateHistoryXeXuatBenLog(xexuatben.TrangThai, "Xe vào bến, kết thúc hành trình", XeXuatBenId, currentNhanVien.Id);
                        break;
                    }
            }
            return SuccessfulSimple("OK");

        }
        #endregion
        #region Chot khach
        public ActionResult ChotKhach(int NhaXeId, int XeXuatBenId, string Email, string Password, int slpm, int sltt, string Latitude, string Longitude, string VitriChot, string apiToken)
        {
            var loginresult = _customerRegistrationService.ValidateCustomer(Email, Password);
            if (loginresult != CustomerLoginResults.Successful)
            {
                return ErrorOccured("Tài khoản hoặc mật khẩu không đúng !");
            }
            var tkchot = _customerService.GetCustomerByEmail(Email);

            //luu nhat ky
            _customerActivityService.InsertActivity("PublicStore.Login", "Tài khoản nhà xe đăng nhập để chốt khách", tkchot);
            //lay thong tin van phong
            var nhanvienchot = _nhanvienService.GetByCustomerId(tkchot.Id);
            if (nhanvienchot == null)
            {
                return ErrorOccured("Thông tin người chốt không hợp lệ");
            }
            if (!nhanvienchot.DiemDonId.HasValue)
            {
                return ErrorOccured("Thông tin người chốt không hợp lệ");
            }
            xexuatben = _nhaxeService.GetHistoryXeXuatBenId(XeXuatBenId);
            if (xexuatben == null)
                return ErrorOccured("Không tồn tại thông tin xe xuất bến");
            if (xexuatben.NguonVeInfo.NhaXeId != NhaXeId)
                return ErrorOccured("Xe xuất bến không thuộc nhà xe");
            //thuc hien thong tin chot khach
            //kiem tra co thong tin chot trc chua
            var lschotkhach = _nhaxeService.GetChotKhachs(NhaXeId, HistoryXeXuatBenId: XeXuatBenId);
            var itemck = new ChotKhach();
            if (lschotkhach.Count > 0)
                itemck = lschotkhach.FirstOrDefault();
            itemck.SoLuongThucTe = sltt;
            itemck.SoLuongPhanMem = slpm;
            itemck.NguoiChotId = nhanvienchot.Id;
            itemck.DiemDonId = nhanvienchot.DiemDonId.Value;
            itemck.NgayChot = DateTime.Now;
            itemck.NhaXeId = NhaXeId;
            itemck.HistoryXeXuatBenId = xexuatben.Id;
            itemck.Latitude = Convert.ToDecimal(Latitude) / 1000000000m;
            itemck.Longitude = Convert.ToDecimal(Longitude) / 1000000000m;
            itemck.ViTriChot = VitriChot;
            if (itemck.Id > 0)
                _nhaxeService.UpdateChotKhach(itemck);
            else
                _nhaxeService.InsertChotKhach(itemck);
            itemck = _nhaxeService.GetChotKhachById(itemck.Id);

            string ghichu = string.Format("Chốt khách tại {0} : SL khách thực tế/phần mềm: {1}/{2}; người chốt: {3}", itemck.diemchot.TenDiemDon, sltt, slpm, nhanvienchot.HoVaTen);
            var item = CreateHistoryXeXuatBenLog(xexuatben.TrangThai, ghichu, XeXuatBenId, nhanvienchot.Id);
            var nhatkyinfo = new
            {
                Id = item.Id,
                NgayTao = item.NgayTao.toStringDateTime(),
                GhiChu = item.GhiChu
            };
            return Successful(nhatkyinfo);
        }

        #endregion

        #region Xe xuat ben
        public ActionResult GetAllBenXe(int NhaXeId, int CustomerId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);

            //lay thong tin xe van chuyen 
            var lsall = _benxeService.GetAllBenXe();
            var dbjson = lsall.Select(c => new
            {
                Id = c.Id,
                TenBenXe = c.TenBenXe
            }).ToList();
            return Successful(dbjson);
        }
        public ActionResult GetAllDBGhiChu(int NhaXeId, int CustomerId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);

            //lay thong tin xe van chuyen 
            var lsall = _phieuchuyenphatService.GetAllDBGhiChu();
            var dbjson = lsall.Select(c => new
            {
                Id = c.Id,
                GhiChu = c.GhiChu
            }).ToList();
            return Successful(dbjson);
        }
        public ActionResult GetAllLaiPhuSoXe(int NhaXeId, int CustomerId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);

            //lay thong tin xe van chuyen 
            var lsall = _phieuchuyenphatService.GetLaiPhuSoXe(DateTime.Now.Month, DateTime.Now.Year);
            if (lsall.Count == 0)
                return ErrorOccured("Lái phụ và số xe không tồn tại");
            var dbjson = lsall.Select(c => new
            {
                Id = c.Id,
                Ten = c.Ten,
                LoaiId = c.LoaiId
            }).ToList();
            return Successful(dbjson);
        }
        public ActionResult GetAllGioMoLenh(int NhaXeId, int CustomerId, int BenXeId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);
            //them vao danh sach toan khoan - ben xe
            AddCustomerBenXeModel(CustomerId, BenXeId);
            //lay thong tin xe van chuyen 
            var lsall = _phieuchuyenphatService.GetGioMoLenh(DateTime.Now.Month, DateTime.Now.Year, BenXeId);
            if (lsall.Count == 0)
                return ErrorOccured("Giờ mở lệnh không tồn tại");
            var dbjson = lsall.Select(c => new
            {
                Id = c.Id,
                GioMoLenh = c.GioMoLenh
            }).ToList();
            return Successful(dbjson);
        }
        public ActionResult GetAllBienSoXe(int NhaXeId, int CustomerId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);

            //lay thong tin xe van chuyen 
            var xevanchuyens = _xeinfoService.GetAllXeInfoByNhaXeId(NhaXeId).Where(c => c.TrangThaiXeId == (int)ENTrangThaiXe.HoatDong).OrderBy(b => b.BienSo).ToList();
            if (xevanchuyens.Count == 0)
                return ErrorOccured("Xe không tồn tại");
            var xevanchuyensjson = xevanchuyens.Select(c => new
            {
                Id = c.Id,
                BienSo = c.BienSo,
                LoaiXeId = c.LoaiXeId
            }).ToList();
            return Successful(xevanchuyensjson);
        }
        public ActionResult GetAllLaiPhuXe(int NhaXeId, int CustomerId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);

            //lay thong tin xe van chuyen 
            var nhanviens = _nhanvienService.GetAllLaiXePhuXeByNhaXeId(NhaXeId).Where(c => c.TrangThaiID == (int)ENTrangThaiNhanVien.DangLamViec).OrderBy(n => n.TenVaHo).ToList();
            if (nhanviens.Count == 0)
                return ErrorOccured("Lái phụ xe không tồn tại");
            var nhanviensjson = nhanviens.Select(c => new
            {
                Id = c.Id,
                TenDayDu = string.Format("{0}({1})", c.HoVaTen, c.DienThoai),
            }).ToList();
            return Successful(nhanviensjson);
        }
        public ActionResult GetAllHanhTrinh(int NhaXeId, int CustomerId, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);

            //lay thong tin hanh trinh
            var vpids = currentNhanVien.VanPhongs.Select(c => c.Id).ToArray();
            var hanhtrinhs = _hanhtrinhService.GetAllHanhTrinhByNhaXeId(NhaXeId, vpids);
            if (hanhtrinhs.Count == 0)
                return ErrorOccured("Hành trình không tồn tại");
            var hanhtrinhsjson = hanhtrinhs.Select(c => new
            {
                Id = c.Id,
                MoTa = c.MoTa,
            }).ToList();
            return Successful(hanhtrinhsjson);
        }
        public ActionResult GetAllChuyenDi(int NhaXeId, int CustomerId, int HanhTrinhId, string NgayDi, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);
            var _ngaydi = Convert.ToDateTime(NgayDi);
            var chuyendis = _nhaxeService.GetAllChuyenDiTrongNgay(NhaXeId, _ngaydi, HanhTrinhId).OrderByDescending(c => c.NgayDi).ToList();

            //Lấy thông tin cac chuyen di
            var chuyendijson = chuyendis.Select(c => new
            {
                Id = c.Id,
                NgayDi = c.NgayDi.toStringDate(),
                GioXuatBen = c.NgayDi.ToString("HH:mm"),
                LaiXe = "",
                ThongTin = c.NguonVeInfo.GetHanhTrinh(),
                SoHangGhe = 0,
                SoCotGhe = 0,
                SoTang = 1,
                TrangThaiId = c.TrangThaiId,
                HanhTrinhId = c.HanhTrinhId,
                LaiXeId = c.LaiPhuXes.Count > 0 ? c.LaiPhuXes.ElementAt(0).NhanVien_Id : 0,
                PhuXeId = c.LaiPhuXes.Count > 1 ? c.LaiPhuXes.ElementAt(1).NhanVien_Id : 0,
                XeVanChuyenId = c.XeVanChuyenId,
                GhiChu = c.GhiChu,
                GioMoPhoi=c.GioMoPhoi,
                SoPhieuXN = c.SoPhieuXN,
                SoLenhVD = c.SoLenhVD,
                SoKhachXB = c.SoKhachXB.GetValueOrDefault(0)
            }).ToList();
            return Successful(chuyendijson);
        }
        public ActionResult CapNhatChuyenDi(int NhaXeId, int CustomerId, int ChuyenDiId, int HanhTrinhId, string GioDi, int XeVanChuyenId, int LaiXeId, int PhuXeId, string GhiChu, int TrangThaiId, string GioMoPhoi, string SoPhieuXN, string SoLenhVD, int SoKhachXB, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);
            var _ngayxuatben = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd ") + GioDi + ":00");
            int[] idlaiphuxes = new int[] { LaiXeId, PhuXeId };            
            HistoryXeXuatBen historyxexuatben = null;
            if (ChuyenDiId > 0)
            {
                historyxexuatben = _nhaxeService.GetHistoryXeXuatBenId(ChuyenDiId);
            }
            ENTrangThaiXeXuatBen trangthai = (ENTrangThaiXeXuatBen)TrangThaiId;
            if(trangthai==ENTrangThaiXeXuatBen.HUY)
            {
                if(historyxexuatben.NgayDi.AddMinutes(30)<DateTime.Now)
                {
                    return ErrorOccured("Xe xuất bến hơn 30 phút, bạn không thể hủy chuyến đi này");                    
                }              
                historyxexuatben.TrangThai = trangthai;
                _nhaxeService.UpdateHistoryXeXuatBen(historyxexuatben);
            }
            else
            {
                //tim thong tin nguon ve
                var nguonves = _hanhtrinhService.GetAllNguonVeXeToXuatBen(NhaXeId, 0, HanhTrinhId);
                if (nguonves.Count == 0)
                    return ErrorOccured("Nguồn vé không tồn tại");
                int NguonVeId = nguonves.First().Id;
                foreach (var nv in nguonves)
                {
                    if (_ngayxuatben >= nv.ThoiGianDiHienTai)
                    {
                        NguonVeId = nv.Id;
                    }
                }
                if (historyxexuatben != null)
                {
                    // xóa hết các lái xe cũ
                    historyxexuatben.LaiPhuXes.Clear();
                    _nhaxeService.DeleteHistoryXeXuatBenNhanVien(historyxexuatben.Id);
                    //up date lai xe mơi               

                    for (int i = 0; i < idlaiphuxes.Length; i++)
                    {
                        var nhanvien = _nhanvienService.GetById(idlaiphuxes[i]);
                        if (nhanvien != null)
                        {
                            var _nhanvienxuatben = new HistoryXeXuatBen_NhanVien();
                            _nhanvienxuatben.NhanVien_Id = nhanvien.Id;
                            _nhanvienxuatben.HistoryXeXuatBen_Id = historyxexuatben.Id;
                            if (i == 0)
                                _nhanvienxuatben.KieuNhanVien = ENKieuNhanVien.LaiXe;
                            else
                                _nhanvienxuatben.KieuNhanVien = ENKieuNhanVien.PhuXe;
                            historyxexuatben.LaiPhuXes.Add(_nhanvienxuatben);
                        }

                    }
                    historyxexuatben.TrangThaiId = TrangThaiId;
                    historyxexuatben.NguonVeId = NguonVeId;
                    historyxexuatben.NgayDi = _ngayxuatben;
                    historyxexuatben.XeVanChuyenId = XeVanChuyenId;
                    historyxexuatben.GhiChu = GhiChu;
                    historyxexuatben.GioMoPhoi = GioMoPhoi;
                    historyxexuatben.SoPhieuXN = SoPhieuXN;
                    historyxexuatben.SoLenhVD = SoLenhVD;
                    historyxexuatben.SoKhachXB = SoKhachXB;

                    _nhaxeService.UpdateHistoryXeXuatBen(historyxexuatben);
                }
                else
                {

                    historyxexuatben = new HistoryXeXuatBen();
                    historyxexuatben.NguonVeId = NguonVeId;
                    for (int i = 0; i < idlaiphuxes.Length; i++)
                    {
                        var nhanvien = _nhanvienService.GetById(idlaiphuxes[i]);
                        if (nhanvien != null)
                        {
                            var _nhanvienxuatben = new HistoryXeXuatBen_NhanVien();
                            _nhanvienxuatben.NhanVien_Id = nhanvien.Id;
                            _nhanvienxuatben.HistoryXeXuatBen_Id = historyxexuatben.Id;
                            if (i == 0)
                                _nhanvienxuatben.KieuNhanVien = ENKieuNhanVien.LaiXe;
                            else
                                _nhanvienxuatben.KieuNhanVien = ENKieuNhanVien.PhuXe;
                            historyxexuatben.LaiPhuXes.Add(_nhanvienxuatben);
                        }
                    }
                    historyxexuatben.XeVanChuyenId = XeVanChuyenId;
                    historyxexuatben.NgayDi = _ngayxuatben;
                    historyxexuatben.NhaXeId = NhaXeId;
                    historyxexuatben.NgayTao = DateTime.Now;
                    historyxexuatben.TrangThaiId = TrangThaiId;
                    historyxexuatben.NguoiTaoId = currentNhanVien.Id;
                    historyxexuatben.HanhTrinhId = HanhTrinhId;
                    historyxexuatben.GhiChu = GhiChu;
                    historyxexuatben.GioMoPhoi = GioMoPhoi;
                    historyxexuatben.SoPhieuXN = SoPhieuXN;
                    historyxexuatben.SoLenhVD = SoLenhVD;
                    historyxexuatben.SoKhachXB = SoKhachXB;
                    _nhaxeService.InsertHistoryXeXuatBen(historyxexuatben);

                }
            }
            
            CapNhatTrangThaiXeXuatBen(historyxexuatben.Id, historyxexuatben.TrangThai);
            //lay lai thong tin
            return GetAllChuyenDi(NhaXeId, CustomerId, HanhTrinhId, DateTime.Now.Date.ToString("yyyy-MM-dd"), apiToken);

        }
        int GetDBLaiPhuSoXeId(List<DB_LaiPhuSoXe> lsall, string Ten, LoaiLaiPhuSoXe loai)
        {
            var item = lsall.Where(c => c.Ten.Equals(Ten) && c.loai == loai).FirstOrDefault();
            if (item != null)
                return item.Id;
            return 0;
        }
        public ActionResult GetAllChuyenDiV1(int NhaXeId, int CustomerId, int HanhTrinhId, string NgayDi, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);
            var _ngaydi = Convert.ToDateTime(NgayDi);
            var chuyendis = _nhaxeService.GetAllChuyenDiTrongNgay(NhaXeId, _ngaydi, HanhTrinhId).OrderByDescending(c => c.NgayDi).ToList();
            //lay tat ca thong tin dinh bien
            var lsall = _phieuchuyenphatService.GetLaiPhuSoXe(_ngaydi.Month, _ngaydi.Year);
            //Lấy thông tin cac chuyen di
            var chuyendijson = chuyendis.Select(c => new
            {
                Id = c.Id,
                NgayDi = c.NgayDi.toStringDate(),
                GioXuatBen = c.NgayDi.ToString("HH:mm"),                
                ThongTin = c.NguonVeInfo.GetHanhTrinh(),
                SoHangGhe = 0,
                SoCotGhe = 0,
                SoTang = 1,
                TrangThaiId = c.TrangThaiId,
                HanhTrinhId = c.HanhTrinhId,
                LaiXeId = GetDBLaiPhuSoXeId(lsall,c.LaiXe,LoaiLaiPhuSoXe.LAI_XE),
                PhuXeId = GetDBLaiPhuSoXeId(lsall, c.PhuXe, LoaiLaiPhuSoXe.PHU_XE),
                XeVanChuyenId = GetDBLaiPhuSoXeId(lsall, c.SoXe, LoaiLaiPhuSoXe.SO_XE),
                GhiChu = String.IsNullOrEmpty(c.GhiChu)?"":c.GhiChu,
                GioMoPhoi = String.IsNullOrEmpty(c.GioMoPhoi) ? "" : c.GioMoPhoi,
                SoPhieuXN = String.IsNullOrEmpty(c.SoPhieuXN) ? "" : c.SoPhieuXN,
                SoLenhVD = String.IsNullOrEmpty(c.SoLenhVD)?"":c.SoLenhVD,
                SoKhachXB = c.SoKhachXB.GetValueOrDefault(0),
                LaiXe=String.IsNullOrEmpty(c.LaiXe)?"":c.LaiXe,
                PhuXe = String.IsNullOrEmpty(c.PhuXe)?"":c.PhuXe,
                SoXe = String.IsNullOrEmpty(c.SoXe)?"":c.SoXe,
            }).ToList();
            return Successful(chuyendijson);
        }
        public ActionResult CapNhatChuyenDiV1(int NhaXeId, int CustomerId, int ChuyenDiId, int HanhTrinhId, string GioDi, int XeVanChuyenId, int LaiXeId, int PhuXeId, string GhiChu, int TrangThaiId, string GioMoPhoi, string SoPhieuXN, string SoLenhVD, int SoKhachXB, string apiToken)
        {
            //kiem tra xac thuc
            string _checkauthentication = isAuthentication(NhaXeId, CustomerId, apiToken);
            if (!String.IsNullOrEmpty(_checkauthentication))
                return ErrorOccured(_checkauthentication);
            var _ngayxuatben = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd ") + GioDi + ":00");
            HistoryXeXuatBen historyxexuatben = null;
            if (ChuyenDiId > 0)
            {
                historyxexuatben = _nhaxeService.GetHistoryXeXuatBenId(ChuyenDiId);
            }
            ENTrangThaiXeXuatBen trangthai = (ENTrangThaiXeXuatBen)TrangThaiId;
            if (trangthai == ENTrangThaiXeXuatBen.HUY)
            {
                if (historyxexuatben.NgayDi.AddMinutes(30) < DateTime.Now)
                {
                    return ErrorOccured("Xe xuất bến hơn 30 phút, bạn không thể hủy chuyến đi này");
                }
                historyxexuatben.TrangThai = trangthai;
                _nhaxeService.UpdateHistoryXeXuatBen(historyxexuatben);
            }
            else
            {
                //tim thong tin nguon ve
                var nguonves = _hanhtrinhService.GetAllNguonVeXeToXuatBen(NhaXeId, 0, HanhTrinhId);
                if (nguonves.Count == 0)
                    return ErrorOccured("Nguồn vé không tồn tại");
                int NguonVeId = nguonves.First().Id;
                foreach (var nv in nguonves)
                {
                    if (_ngayxuatben >= nv.ThoiGianDiHienTai)
                    {
                        NguonVeId = nv.Id;
                    }
                }
                //lay tat ca thong tin dinh bien
                var lsall = _phieuchuyenphatService.GetLaiPhuSoXe(_ngayxuatben.Month, _ngayxuatben.Year);
                if (historyxexuatben != null)
                {
                    //up date lai xe mơi               
                    var laixe = lsall.Where(c=>c.Id==LaiXeId).FirstOrDefault();
                    historyxexuatben.LaiXe =laixe!=null? laixe.Ten:"";
                    var phuxe = lsall.Where(c => c.Id == PhuXeId).FirstOrDefault();
                    historyxexuatben.PhuXe = phuxe != null ? phuxe.Ten : "";
                    var soxe = lsall.Where(c => c.Id == XeVanChuyenId).FirstOrDefault();
                    historyxexuatben.SoXe = soxe != null ? soxe.Ten : "";

                    historyxexuatben.TrangThaiId = TrangThaiId;
                    historyxexuatben.NguonVeId = NguonVeId;
                    historyxexuatben.NgayDi = _ngayxuatben;
                    historyxexuatben.GhiChu = GhiChu;
                    historyxexuatben.GioMoPhoi = GioMoPhoi;
                    historyxexuatben.SoPhieuXN = SoPhieuXN;
                    historyxexuatben.SoLenhVD = SoLenhVD;
                    historyxexuatben.SoKhachXB = SoKhachXB;
                    historyxexuatben.BenXuatPhatId = getBenXeId(CustomerId);
                    _nhaxeService.UpdateHistoryXeXuatBen(historyxexuatben);
                }
                else
                {

                    historyxexuatben = new HistoryXeXuatBen();
                    historyxexuatben.NguonVeId = NguonVeId;
                    var laixe = lsall.Where(c => c.Id == LaiXeId).FirstOrDefault();
                    historyxexuatben.LaiXe = laixe != null ? laixe.Ten : "";
                    var phuxe = lsall.Where(c => c.Id == PhuXeId).FirstOrDefault();
                    historyxexuatben.PhuXe = phuxe != null ? phuxe.Ten : "";
                    var soxe = lsall.Where(c => c.Id == XeVanChuyenId).FirstOrDefault();
                    historyxexuatben.SoXe = soxe != null ? soxe.Ten : "";
                    
                    historyxexuatben.NgayDi = _ngayxuatben;
                    historyxexuatben.NhaXeId = NhaXeId;
                    historyxexuatben.NgayTao = DateTime.Now;
                    historyxexuatben.TrangThaiId = TrangThaiId;
                    historyxexuatben.NguoiTaoId = currentNhanVien.Id;
                    historyxexuatben.HanhTrinhId = HanhTrinhId;
                    historyxexuatben.GhiChu = GhiChu;
                    historyxexuatben.GioMoPhoi = GioMoPhoi;
                    historyxexuatben.SoPhieuXN = SoPhieuXN;
                    historyxexuatben.SoLenhVD = SoLenhVD;
                    historyxexuatben.SoKhachXB = SoKhachXB;
                    historyxexuatben.BenXuatPhatId = getBenXeId(CustomerId);
                    _nhaxeService.InsertHistoryXeXuatBen(historyxexuatben);

                }
            }

            CapNhatTrangThaiXeXuatBen(historyxexuatben.Id, historyxexuatben.TrangThai);
            //lay lai thong tin
            return GetAllChuyenDiV1(NhaXeId, CustomerId, HanhTrinhId, DateTime.Now.Date.ToString("yyyy-MM-dd"), apiToken);

        }
        void CapNhatTrangThaiXeXuatBen(int Id, ENTrangThaiXeXuatBen trangthai)
        {
           
            var _log = new HistoryXeXuatBenLog();
            _log.NguoiTaoId = currentNhanVien.Id;
            _log.NgayTao = DateTime.Now;
            _log.TrangThai = trangthai;
            switch (trangthai)
            {
                case ENTrangThaiXeXuatBen.DANG_DI:
                    {
                        _log.GhiChu = "Xe xuất bến, bắt đầu hành trình";
                        break;
                    }
                case ENTrangThaiXeXuatBen.KET_THUC:
                    {
                        _log.GhiChu = "Xe về bến, kết thúc hành trình";
                        break;
                    }
                case ENTrangThaiXeXuatBen.HUY:
                    {
                        _log.GhiChu = "Hủy xe xuất bến";
                        break;
                    }
            }
            _log.XeXuatBenId = Id;
            _nhaxeService.InsertHistoryXeXuatBenLog(_log);
        }
        #endregion

        #region Misc

        public ActionResult InvalidApiToken(string apiToken)
        {
            var errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(apiToken))
                errorMessage = "No API token supplied.";
            else
                errorMessage = string.Format("Invalid API token: {0}", apiToken);

            return ErrorOccured(errorMessage);
        }

        public ActionResult ErrorOccured(string errorMessage)
        {
            return ErrorOccured(errorMessage, 0);
        }
        public ActionResult ErrorOccured(string errorMessage, int _errid)
        {
            return Json(new
            {
                success = false,
                data = errorMessage,
                errid = _errid
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Successful(object data)
        {
            return Json(new
            {
                success = true,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SuccessfulSimple(string val)
        {
            var _data = new
            {
                RetMsg = val
            };
            return Successful(_data);
        }
        #endregion

        #region Helper methods

        private bool IsApiTokenValid(string apiToken)
        {
            if (string.IsNullOrWhiteSpace(apiToken) ||
                string.IsNullOrWhiteSpace(_settings.ApiToken))
                return false;

            return _settings.ApiToken.Trim().Equals(apiToken.Trim(),
                StringComparison.InvariantCultureIgnoreCase);
        }

        private object GetCustomerJson(Customer customer)
        {
            var customerJson = new
            {
                Id = customer.Id,
                Email = customer.Email,
                FullName = customer.GetFullName(),
                Phone = customer.GetPhone()
            };

            return customerJson;
        }




        #endregion

    }
}