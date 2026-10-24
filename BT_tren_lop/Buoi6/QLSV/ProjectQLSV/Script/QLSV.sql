

CREATE DATABASE IF NOT EXISTS QLSV
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE QLSV;

-- BẢNG LỚP HỌC (thực thể "cha" – quan hệ 1 - nhiều)
DROP TABLE IF EXISTS SinhVien;
DROP TABLE IF EXISTS LopHoc;

CREATE TABLE LopHoc
(
    MaLop  VARCHAR(10)  NOT NULL PRIMARY KEY,          -- Mã lớp (PK)
    TenLop VARCHAR(100) NOT NULL,                       -- Tên lớp
    GhiChu VARCHAR(200) NULL                            -- Ghi chú
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;

-- BẢNG SINH VIÊN (thực thể "con" – mỗi SV thuộc 1 lớp)
CREATE TABLE SinhVien
(
    MaSV     VARCHAR(10)   NOT NULL PRIMARY KEY,       -- Mã sinh viên (PK)
    HoTen    VARCHAR(100)  NOT NULL,                    -- Họ và tên
    NgaySinh DATE          NOT NULL,                    -- Ngày sinh
    GioiTinh VARCHAR(10)   NOT NULL,                    -- Nam / Nữ
    DienThoai VARCHAR(10)  NULL,                        -- Số điện thoại (10 chữ số)
    Email    VARCHAR(100)  NULL,                        -- Email
    MaLop    VARCHAR(10)   NOT NULL,                    -- Lớp (FK → LopHoc)
    Diem     DECIMAL(4, 2) NOT NULL DEFAULT 0,          -- Điểm (0 – 10)
    TrangThai VARCHAR(30)  NOT NULL,                    -- Đang học / Bảo lưu / Đã tốt nghiệp
    CONSTRAINT FK_SinhVien_LopHoc
        FOREIGN KEY (MaLop) REFERENCES LopHoc (MaLop)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;

-- ===================== DỮ LIỆU MẪU =====================
INSERT INTO LopHoc (MaLop, TenLop, GhiChu) VALUES
    ('CSE0001', 'Khoa học máy tính 01', 'Lớp chính quy'),
    ('CSE0002', 'Khoa học máy tính 02', 'Lớp chính quy'),
    ('CSE0003', 'Khoa học máy tính 03', 'Lớp chất lượng cao');

INSERT INTO SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, DienThoai, Email, MaLop, Diem, TrangThai) VALUES
    ('SV001', 'Nguyễn Văn An',   '2005-01-15', 'Nam', '0901234567', 'an.nguyenvan@gmail.com',    'CSE0001', 7.50, 'Đang học'),
    ('SV002', 'Trần Thị Bình',   '2005-03-22', 'Nữ',  '0912345678', 'binh.tranthi@gmail.com',    'CSE0002', 8.20, 'Đang học'),
    ('SV003', 'Lê Văn Cường',    '2004-11-05', 'Nam', '0923456781', 'cuong.levan@gmail.com',     'CSE0001', 6.40, 'Bảo lưu'),
    ('SV004', 'Phạm Thu Hà',     '2005-07-09', 'Nữ',  '0934567892', 'ha.phamthu@gmail.com',      'CSE0003', 9.10, 'Đã tốt nghiệp');

-- Kiểm tra dữ liệu sau khi chạy
-- SELECT * FROM LopHoc;
-- SELECT * FROM SinhVien;
