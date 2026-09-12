using System;

namespace QuanLyHinhHoc
{
    // Interface Hinh định nghĩa các phương thức chung cho các hình học
    public interface IHinh
    {
        double GetDienTich();
        double GetChuVi();
        void Nhap();
        void HienThi();
    }
}
