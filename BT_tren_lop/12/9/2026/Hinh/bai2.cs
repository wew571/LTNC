using System;
using System.Collections.Generic;

namespace QuanLyHinhHoc
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            // Danh sách chứa các đối tượng có interface IHinh (Thể hiện tính đa hình)
            List<IHinh> danhSachHinh = new List<IHinh>();

            while (true)
            {
                Console.WriteLine("\n================ QUẢN LÝ CÁC HÌNH HỌC ================");
                Console.WriteLine("1. Thêm Hình Tròn");
                Console.WriteLine("2. Thêm Hình Chữ Nhật");
                Console.WriteLine("3. Thêm Hình Tam Giác");
                Console.WriteLine("4. Hiển thị danh sách tất cả các hình (Tính đa hình)");
                Console.WriteLine("5. Tính tổng chu vi & diện tích các hình (Tính đa hình)");
                Console.WriteLine("0. Thoát");
                Console.Write("Chọn chức năng (0-5): ");

                string choice = Console.ReadLine() ?? "";

                if (choice == "0")
                {
                    Console.WriteLine("Chương trình kết thúc. Tạm biệt!");
                    break;
                }

                IHinh hinhMoi = null;

                switch (choice)
                {
                    case "1":
                        hinhMoi = new HinhTron();
                        hinhMoi.Nhap();
                        danhSachHinh.Add(hinhMoi);
                        Console.WriteLine("=> Đã thêm Hình Tròn thành công!");
                        break;

                    case "2":
                        hinhMoi = new HinhChuNhat();
                        hinhMoi.Nhap();
                        danhSachHinh.Add(hinhMoi);
                        Console.WriteLine("=> Đã thêm Hình Chữ Nhật thành công!");
                        break;

                    case "3":
                        hinhMoi = new HinhTamGiac();
                        hinhMoi.Nhap();
                        danhSachHinh.Add(hinhMoi);
                        Console.WriteLine("=> Đã thêm Hình Tam Giác thành công!");
                        break;

                    case "4":
                        Console.WriteLine("\n--- DANH SÁCH TẤT CẢ CÁC HÌNH ---");
                        if (danhSachHinh.Count == 0)
                        {
                            Console.WriteLine("Danh sách đang trống!");
                        }
                        else
                        {
                            // Tính đa hình: Duyệt qua từng IHinh và gọi HienThi()
                            for (int i = 0; i < danhSachHinh.Count; i++)
                            {
                                Console.Write(string.Format("{0}. ", i + 1));
                                danhSachHinh[i].HienThi();
                            }
                        }
                        break;

                    case "5":
                        if (danhSachHinh.Count == 0)
                        {
                            Console.WriteLine("Danh sách trống!");
                            break;
                        }

                        double tongChuVi = 0;
                        double tongDienTich = 0;

                        // Tính đa hình: Gọi GetChuVi() và GetDienTich() trên từng đối tượng IHinh
                        foreach (IHinh hinh in danhSachHinh)
                        {
                            tongChuVi += hinh.GetChuVi();
                            tongDienTich += hinh.GetDienTich();
                        }

                        Console.WriteLine(string.Format("\nTổng số lượng hình: {0}", danhSachHinh.Count));
                        Console.WriteLine(string.Format("Tổng chu vi các hình: {0:F2}", tongChuVi));
                        Console.WriteLine(string.Format("Tổng diện tích các hình: {0:F2}", tongDienTich));
                        break;

                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ! Vui lòng chọn từ 0 đến 5.");
                        break;
                }
            }
        }
    }
}