﻿using Nop.Core.Domain.NhaXes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Models.NhaXes
{
   
    public class BaoCaoNhaXeModel : BaseNopEntityModel
    {
        public enum EN_LOAI_BAO_CAO
        {
            DANH_SACH_NHAN_VIEN = 1,
            DOANH_THU_THEO_GIO = 2,
            DOANH_THU_HANG_NGAY=3,
            HANG_HOA_VAN_PHONG=4,
            HANG_HOA_TONG_HOP=5,
            BAO_CAO_PHIEU_VAN_CHUYEN=6,
            PHIEU_VAN_CHUYEN_NGAY=7,
            PHIEU_VAN_CHUYEN_THANG = 8,
            CHI_TIEU_THANG=9,
            TO_VAN_CHUYEN_THANG=10,
            BAO_CAO_KHACH_HANG_GUI=11,
            BAO_CAO_KHACH_HANG_NHAN=12,
            BAO_CAO_SMS_GUI=13,
            BAO_CAO_VAN_PHONG_TRA = 14,
            BAO_CAO_DOANH_THU_THANG=15
        }

        public enum EN_BAO_CAO_DIEU_HANH_BEN
        {
            THEO_BEN_XE=1,
            THEO_TUYEN=2
        }
        public BaoCaoNhaXeModel()
        {
            ListLoai1 = new List<SelectListItem>();
            ListLoai2 = new List<SelectListItem>();
            ListQuy = new List<SelectListItem>();
            ListMonth = new List<SelectListItem>();
            ListYear = new List<SelectListItem>();
            VanPhongs = new List<SelectListItem>();
            KhuVucs = new List<SelectListItem>();
            Xe = new List<SelectListItem>();
        }
        public EN_LOAI_BAO_CAO LoaiBaoCao
        {
            get
            {
                return (EN_LOAI_BAO_CAO)LoaiBaoCaoId;
            }
            set
            {
                LoaiBaoCaoId = (int)value;
            }
        }
        public string FileNameExport
        {
            get
            {
                string _filenaname = LoaiBaoCao.ToString();
                _filenaname = _filenaname.ToLower();
                return _filenaname;
            }
        }
        public int LoaiBaoCaoId { get; set; }
        public int Loai1Id { get; set; }
        public int Loai2Id { get; set; }
        [NopResourceDisplayName("ChonVe.NhaXe.BaoCaoNhaXe.Quy")]
        public int QuyId { get; set; }
        [NopResourceDisplayName("ChonVe.NhaXe.BaoCaoNhaXe.Thang")]
        public int ThangId { get; set; }
        public int HanhTrinhId { get; set; }
        
        [NopResourceDisplayName("ChonVe.NhaXe.BaoCaoNhaXe.Nam")]
        public int NamId { get; set; }
        [UIHint("DateNullable")]
        [NopResourceDisplayName("ChonVe.NhaXe.BaoCaoNhaXe.TuNgay")]
        public DateTime TuNgay { get; set; }
        [UIHint("DateNullable")]
        [NopResourceDisplayName("ChonVe.NhaXe.BaoCaoNhaXe.DenNgay")]
        public DateTime DenNgay { get; set; }
         [UIHint("DateNullable")]
        public DateTime NgayGuiHang { get; set; }
        public DateTime GioChaySearch { get; set; }
        public string GioChay { get; set; }
        public int LichTrinhId { get; set; }

        [NopResourceDisplayName("ChonVe.NhaXe.BaoCaoNhaXe.ChonVe")]
        public bool isChonVe { get; set; }

        public string NgayBan { get; set; }
      
        public int XeId { get; set; }
        public int VanPhongId { get; set; }
        public int KhuVucId { get; set; }
        public int Loai3Id { get; set; }
        public IList<SelectListItem> VanPhongs { get; set; }
        public IList<SelectListItem> KhuVucs { get; set; }
        public IList<SelectListItem> GioChays { get; set; }
        public IList<SelectListItem> Xe { get; set; }
        public IList<SelectListItem> ListLoai1 { get; set; }
        public IList<SelectListItem> ListLoai2 { get; set; }
        public IList<SelectListItem> ListLoai3 { get; set; }
        public IList<SelectListItem> ListQuy { get; set; }
        public IList<SelectListItem> ListMonth { get; set; }
        public IList<SelectListItem> ListYear { get; set; }
        public string BienSoXe { get; set; }
        public string SoLenh { get; set; }

        //add by lent 11/12/2016
        //them cac thuoc tinh de hien bao cao + export bao cao

        public string KeySearch { get; set; }
        public int TuyenId { get; set; }
        public IList<SelectListItem> ListTuyens { get; set; }
        public int BenXeId { get; set; }
        public IList<SelectListItem> ListBenXes { get; set; }
        public String[] Title { get; set; }
        public List<String[]> TitleColSpan { get; set; }
        public string topPage { get; set; }
        public String[] headers { get; set; }
        public DataTable dataReport { get; set; }
        public bool isShowSTT { get; set; }
        public bool addSumRight { get; set; }
        public int idxColForSum { get; set; }
        public bool addSumBottom { get; set; }
        public class BaoCaoDoanhThuModel : ThongKeItem
        {
            public string ThoiGian { get; set; }
            public decimal TongDoanhThu { get; set; }
            public decimal DoanhThuChonVe { get; set; }
            public decimal DoanhThuNhaXe { get; set; }

        }
        public class BaoCaoDoanhThuNhanVienModel : ThongKeItem
        {
            public int NhanVienId { get; set; }
            public int NguonVeId { get; set; }
            public string TenNhanVien { get; set; }
            public string TrangThaiPhoiVeText { get; set; }
            public decimal TongDoanhThu { get; set; }
            public decimal DoanhThuChuaThanhToan { get; set; }
            public decimal DoanhThuChonVe { get; set; }
            public decimal DoanhThuNhaXe { get; set; }
            public string NgayBan { get; set; }
           
        }
        public class BaoCaoDoanhThuXeTungNgayModel : DoanhThuTheoXeItem
        {
           
            public string BienSo { get; set; }
            public string TrangThaiPhoiVeText { get; set; }
            public decimal TongDoanhThu { get; set; }
            public decimal DoanhThuXe { get; set; }
            public string NgayBan { get; set; }
            public string NgayDen { get; set; }
           
        }
        public class BaoCaoDetailDoanhThuKiGuiModel : ThongKeItem
        {
            public string NotPay { get; set; }
            public int NhanVienId { get; set; }
            public string NgayBan { get; set; }
        }
        public class KhachHangMuaVeModel
        {
            public int CustomerId { get; set; }
            public int NguonVeXeId { get; set; }
            public string TenKhachHang { get; set; }
            public string SoDienThoai { get; set; }
            public string ThongTinChuyenDi { get; set; }
            public int TrangThaiPhoiVeId { get; set; }
            public string TrangThaiPhoiVeText { get; set; }
            public string KyHieuGhe { get; set; }
            public bool isChonVe { get; set; }
            public decimal GiaVe { get; set; }
            public DateTime NgayDi { get; set; }
            public int SoLuot { get; set; }
        }
        public class BaoCaoKhachHangTiemNangModel
        {
            public int KhachHangId { get; set; }
            public string HoVaTen { get; set; }
            public string DiaChi { get; set; }
            public string SoDienThoai { get; set; }
            public int SoLanGD { get; set; }
            public int ThuTuSoLanGD { get; set; }
            public decimal TongSoTien { get; set; }
            public decimal SoTienDaTT { get; set; }
            public decimal SoTienChuaTT { get { return TongSoTien - SoTienDaTT; } }
            public decimal ThuTuTienTT { get; set; }
        }
        public class BaoCaoSMSGui
        {
            public string KhachHangNhan{get;set;}
            public string SoDienThoai {get;set;}
            public string NoiDung {get;set;}
            public string NgayTao{get;set;}
            public int VanPhongGuiId{get;set;}
        }
    }
}