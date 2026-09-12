using System;

namespace QuanLyHinhHoc
{
    public class HinhTamGiac : IHinh
    {
        private double _canhA;
        private double _canhB;
        private double _canhC;

        // Phương thức kiểm tra điều kiện 3 cạnh tạo thành tam giác hợp lệ
        public static bool KiemTraHopLe(double a, double b, double c)
        {
            return (a > 0 && b > 0 && c > 0) &&
                   (a + b > c) && (a + c > b) && (b + c > a);
        }

        // Phương thức thiết lập 3 cạnh kèm kiểm tra hợp lệ
        public void Set3Canh(double a, double b, double c)
        {
            if (!KiemTraHopLe(a, b, c))
            {
                throw new ArgumentException("3 cạnh phải > 0 và thỏa mãn bất đẳng thức tam giác (tổng 2 cạnh luôn lớn hơn cạnh còn lại).");
            }
            _canhA = a;
            _canhB = b;
            _canhC = c;
        }

        public double CanhA { get { return _canhA; } }
        public double CanhB { get { return _canhB; } }
        public double CanhC { get { return _canhC; } }

        // Phương thức khởi tạo không tham số (tam giác đều cạnh 1)
        public HinhTamGiac()
        {
            _canhA = 1.0;
            _canhB = 1.0;
            _canhC = 1.0;
        }

        // Phương thức khởi tạo có tham số
        public HinhTamGiac(double a, double b, double c)
        {
            Set3Canh(a, b, c);
        }

        public double GetChuVi()
        {
            return _canhA + _canhB + _canhC;
        }

        public double GetDienTich()
        {
            // Tính diện tích theo công thức Heron
            double p = GetChuVi() / 2.0;
            return Math.Sqrt(p * (p - _canhA) * (p - _canhB) * (p - _canhC));
        }

        public void Nhap()
        {
            while (true)
            {
                try
                {
                    Console.Write("Nhập cạnh A của tam giác: ");
                    double a = double.Parse(Console.ReadLine() ?? "0");

                    Console.Write("Nhập cạnh B của tam giác: ");
                    double b = double.Parse(Console.ReadLine() ?? "0");

                    Console.Write("Nhập cạnh C của tam giác: ");
                    double c = double.Parse(Console.ReadLine() ?? "0");

                    Set3Canh(a, b, c);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Lỗi] " + ex.Message + " Vui lòng nhập lại!");
                }
            }
        }

        public void HienThi()
        {
            Console.WriteLine(string.Format("[Hình Tam Giác] 3 cạnh: ({0:F2}, {1:F2}, {2:F2}) | Chu vi: {3:F2} | Diện tích: {4:F2}",
                _canhA, _canhB, _canhC, GetChuVi(), GetDienTich()));
        }
    }
}
