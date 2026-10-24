using System;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        int a, b, c;

        while (true)
        {
            Console.Write("Nhập số a: ");
            if (int.TryParse(Console.ReadLine(), out a))
                break;
            Console.WriteLine("Nhập sai, vui lòng nhập lại một số nguyên hợp lệ!");
        }

        while (true)
        {
            Console.Write("Nhập số b: ");
            if (int.TryParse(Console.ReadLine(), out b))
                break;
            Console.WriteLine("Nhập sai, vui lòng nhập lại một số nguyên hợp lệ!");
        }

        while (true)
        {
            Console.Write("Nhập số c: ");
            if (int.TryParse(Console.ReadLine(), out c))
                break;
            Console.WriteLine("Nhập sai, vui lòng nhập lại một số nguyên hợp lệ!");
        }

        int tong = a + b + c;
        Console.WriteLine($"Tổng của 3 số {a} + {b} + {c} = {tong}");
    }
}