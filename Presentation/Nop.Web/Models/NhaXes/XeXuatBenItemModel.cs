﻿using Nop.Core.Domain.Chonves;
using Nop.Core.Domain.NhaXes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.NhaXes;
using Nop.Web.Models.VeXeKhach;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Models.NhaXes
{
    public class XeXuatBenItemModel : BaseNopEntityModel
    {

        public XeXuatBenItemModel()
        {
            laivaphuxes = new List<NhanVienLaiPhuXe>();
            nhatkys = new List<NhatKyXeXuatBen>();
            isEdit = true;
            SoGhe = 29;
        }

        public int NguonVeId { get; set; }
        public List<SelectListItem> nguonves { get; set; }
        public List<SelectListItem> loaixes { get; set; }
        public List<XeXuatBenItemModel.XeVanChuyenInfo> AllXeInfo { get; set; }
        public int LoaiXeId { get; set; }
        public int XeVanChuyenId { get; set; }
        public string BienSo { get; set; }
        public int TrangThaiId { get; set; }
        public ENTrangThaiXeXuatBen TrangThai
        {
            get
            {
                return (ENTrangThaiXeXuatBen)TrangThaiId;
            }
            set
            {
                TrangThaiId = (int)value;
            }
        }
        public string TrangThaiText { get; set; }

        public DateTime NgayDi { get; set; }
        public DateTime ThoiGianThuc { get; set; }        
        public int SoNguoi { get; set; }
        public DateTime NgayTao { get; set; }
        public int NguoiTaoId { get; set; }
        public string TenNguoiTao { get; set; }
        public string GhiChu { get; set; }
        public int HanhTrinhId { get; set; }
        public string TuyenXeChay { get; set; }
        public int NTVId { get; set; }
        public string TenLaiXe { get; set; }

        public string BienSoXe3So { get; set; }
        public string TenNTV { get; set; }
        public string GioDi { get; set; }
        public string GioDen { get; set; }

        public List<NhanVienLaiPhuXe> tatcalaivaphuxes { get; set; }
        public List<NhanVienLaiPhuXe> laivaphuxes { get; set; }
        public int LaiXeId { get; set; }
        public int PhuXeId { get; set; }
        public string ThongTinLaiPhuXe { get; set; }
        public List<NhatKyXeXuatBen> nhatkys { get; set; }
        public bool isEdit { get; set; }
        public class NhanVienLaiPhuXe:BaseNopEntityModel
        {          
            public NhanVienLaiPhuXe(int _id, string _thongtin)
            {
                Id = _id;
                ThongTin = _thongtin;
                TenLaiXe = _thongtin;
            }
            public string ThongTin { get; set; }
            public string TenLaiXe { get; set; }
        }
        public class NhatKyXeXuatBen:BaseNopEntityModel
        {
            public string GhiChu { get; set; }
            public DateTime NgayTao { get; set; }
            public int NguoiTaoId { get; set; }
            public string TenNguoiTao { get; set; }
        }
        public class XeVanChuyenInfo:BaseNopEntityModel
        {
            public XeVanChuyenInfo(int _id, string _bienso)
            {
                Id = _id;
                BienSo = _bienso;
            }
            public string BienSo { get; set; }
            public int LoaiXeId { get; set; }
        }
        public string TenLoaiXe { get; set; }
        public int SoGhe { get; set; }
        public string GioMoPhoi { get; set; }
        public string SoPhieuXN { get; set; }
        public string SoLenhVD { get; set; }
        public int SoKhachXB { get; set; }
        //thong tin dinh bien
        public string DBLaiXe { get; set; }
        public string DBPhuXe { get; set; }
        public string DBSoXe { get; set; }
        public List<DBGioMoPhoi> GioMoPhois { get; set; }
        public class DBGioMoPhoi
        {
            public DBGioMoPhoi(int _id, string _ten)
            {
                Id = _id;
                Ten = _ten;
            }
            public int Id { get; set; }
            public string Ten { get; set; }
        }
    }
}