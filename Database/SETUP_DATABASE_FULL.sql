-- ========================================
-- PORTO E-COMMERCE DATABASE - FULL SETUP
-- File duy nhất để setup toàn bộ database
-- ========================================

USE master;
GO

-- ========================================
-- BƯỚC 1: TẠO DATABASE
-- ========================================

PRINT '========================================';
PRINT 'BƯỚC 1: Tạo Database PortoDB';
PRINT '========================================';

-- Drop database nếu đã tồn tại (CẢNH BÁO!)
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'PortoDB')
BEGIN
    ALTER DATABASE PortoDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE PortoDB;
    PRINT '✓ Đã xóa database cũ';
END

-- Tạo database mới
CREATE DATABASE PortoDB;
GO

USE PortoDB;
GO

PRINT '✓ Đã tạo database PortoDB';
PRINT '';

-- ========================================
-- BƯỚC 2: TẠO CÁC BẢNG
-- ========================================

PRINT '========================================';
PRINT 'BƯỚC 2: Tạo các bảng';
PRINT '========================================';

-- Bảng User
CREATE TABLE [dbo].[User](
	[Username] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[UserRole] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Username] ASC)
);
PRINT '✓ Đã tạo bảng User';

-- Bảng Category
CREATE TABLE [dbo].[Category](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([CategoryID] ASC)
);
PRINT '✓ Đã tạo bảng Category';

-- Bảng Product (với cột ProductQuantity)
CREATE TABLE [dbo].[Product](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryID] [int] NOT NULL,
	[ProductName] [nvarchar](200) NOT NULL,
	[ProductDecription] [nvarchar](max) NULL,
	[ProductPrice] [decimal](18, 2) NOT NULL,
	[ProductImage] [nvarchar](200) NULL,
	[ProductQuantity] [int] NOT NULL DEFAULT 0,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([ProductID] ASC),
 CONSTRAINT [FK_Product_Category] FOREIGN KEY([CategoryID]) REFERENCES [dbo].[Category] ([CategoryID])
);
PRINT '✓ Đã tạo bảng Product (có cột ProductQuantity)';

-- Bảng Customer
CREATE TABLE [dbo].[Customer](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerName] [nvarchar](100) NOT NULL,
	[CustomerEmail] [nvarchar](100) NULL,
	[CustomerPhone] [nvarchar](20) NULL,
	[CustomerAddress] [nvarchar](max) NULL,
	[Username] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([CustomerID] ASC),
 CONSTRAINT [FK_Customer_User] FOREIGN KEY([Username]) REFERENCES [dbo].[User] ([Username])
);
PRINT '✓ Đã tạo bảng Customer';

-- Bảng Order
CREATE TABLE [dbo].[Order](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[OrderDate] [datetime] NOT NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
	[PaymentStatus] [nvarchar](50) NULL,
	[AddressDelivery] [nvarchar](max) NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([OrderID] ASC),
 CONSTRAINT [FK_Order_Customer] FOREIGN KEY([CustomerID]) REFERENCES [dbo].[Customer] ([CustomerID])
);
PRINT '✓ Đã tạo bảng Order';

-- Bảng OrderDetail
CREATE TABLE [dbo].[OrderDetail](
	[OrderDetailID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_OrderDetail] PRIMARY KEY CLUSTERED ([OrderDetailID] ASC),
 CONSTRAINT [FK_OrderDetail_Order] FOREIGN KEY([OrderID]) REFERENCES [dbo].[Order] ([OrderID]),
 CONSTRAINT [FK_OrderDetail_Product] FOREIGN KEY([ProductID]) REFERENCES [dbo].[Product] ([ProductID])
);
PRINT '✓ Đã tạo bảng OrderDetail';
PRINT '';

-- ========================================
-- BƯỚC 3: INSERT DỮ LIỆU MẪU
-- ========================================

PRINT '========================================';
PRINT 'BƯỚC 3: Thêm dữ liệu mẫu';
PRINT '========================================';

-- Users
INSERT INTO [User] (Username, Password, UserRole) VALUES ('admin', 'admin123', 'Admin');
INSERT INTO [User] (Username, Password, UserRole) VALUES ('customer1', 'pass123', 'Customer');
PRINT '✓ Đã thêm 2 users';

-- Categories
INSERT INTO [Category] (CategoryName) VALUES (N'Điện thoại');
INSERT INTO [Category] (CategoryName) VALUES (N'Laptop');
INSERT INTO [Category] (CategoryName) VALUES (N'Phụ kiện');
INSERT INTO [Category] (CategoryName) VALUES (N'Tablet');
INSERT INTO [Category] (CategoryName) VALUES (N'Đồng hồ thông minh');
PRINT '✓ Đã thêm 5 categories';

-- Products - Điện thoại
INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (1, N'iPhone 15 Pro Max', 
N'iPhone 15 Pro Max - Đỉnh cao công nghệ với chip A17 Pro mạnh mẽ nhất từ trước đến nay. Màn hình Super Retina XDR 6.7 inch, camera 48MP với zoom quang học 5x, khung titan cao cấp, pin 29 giờ phát video. Hỗ trợ 5G, sạc nhanh 20W, chống nước IP68. Bộ nhớ từ 256GB đến 1TB.', 
29990000, 'iphone15.jpg', 25);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (1, N'Samsung Galaxy S24 Ultra', 
N'Samsung Galaxy S24 Ultra - Flagship Android đỉnh cao với chip Snapdragon 8 Gen 3 for Galaxy. Màn hình Dynamic AMOLED 2X 6.8 inch 120Hz, camera 200MP, S Pen tích hợp, pin 5000mAh, sạc nhanh 45W. RAM 12GB, bộ nhớ 256GB/512GB. Chống nước IP68, Gorilla Armor siêu bền.', 
26990000, 'samsung-s24.jpg', 30);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (1, N'iPhone 14 Pro', 
N'iPhone 14 Pro - Dynamic Island độc đáo, chip A16 Bionic mạnh mẽ. Màn hình 6.1 inch Super Retina XDR ProMotion 120Hz, camera 48MP chụp đêm tuyệt đẹp, pin 23 giờ phát video. Khung thép không gỉ sang trọng, 4 màu sắc lựa chọn.', 
24990000, 'iphone14pro.jpg', 20);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (1, N'Samsung Galaxy Z Fold 5', 
N'Galaxy Z Fold 5 - Điện thoại gập đột phá với màn hình chính 7.6 inch và màn hình phụ 6.2 inch. Chip Snapdragon 8 Gen 2, RAM 12GB, camera 50MP, bút S Pen hỗ trợ. Pin 4400mAh, gập không kẽ hở, độ bền vượt trội.', 
39990000, 'zfold5.jpg', 15);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (1, N'Xiaomi 14 Ultra', 
N'Xiaomi 14 Ultra - Camera phone hàng đầu với Leica Summilux, cảm biến 50MP x4, zoom quang 5x. Chip Snapdragon 8 Gen 3, màn hình AMOLED 120Hz, sạc nhanh 90W, sạc không dây 80W. RAM 16GB, pin 5000mAh.', 
22990000, 'xiaomi14ultra.jpg', 22);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (1, N'Xiaomi 14', 
N'Xiaomi 14 - Thiết kế gọn nhẹ với màn hình 6.36 inch AMOLED 120Hz. Chip Snapdragon 8 Gen 3, camera Leica 50MP, pin 4610mAh với sạc nhanh 90W. Hiệu năng mạnh mẽ, giá cạnh tranh.', 
18990000, 'xiaomi14.jpg', 18);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (1, N'OPPO Find X7 Pro', 
N'OPPO Find X7 Pro - Camera Hasselblad đỉnh cao, cảm biến 50MP Sony LYT-900. Chip Snapdragon 8 Gen 3, màn hình cong 120Hz, sạc nhanh 100W. Thiết kế cao cấp, hiệu năng tuyệt đỉnh.', 
21990000, 'oppo-findx7.jpg', 35);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (1, N'Google Pixel 8 Pro', 
N'Google Pixel 8 Pro - AI photography đỉnh cao, Magic Editor siêu thông minh. Chip Google Tensor G3, camera 50MP với Night Sight tuyệt vời, màn hình 6.7 inch 120Hz, Android thuần túy, cập nhật 7 năm.', 
24990000, 'pixel8pro.jpg', 28);

-- Products - Laptop
INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (2, N'MacBook Pro M3 14 inch', 
N'MacBook Pro M3 - Hiệu năng đột phá với chip M3 Pro/Max. Màn hình Liquid Retina XDR 14.2 inch, pin 18 giờ, 16GB RAM, SSD 512GB-1TB. Cổng Thunderbolt 4, MagSafe 3, bàn phím Magic Keyboard. Lý tưởng cho sáng tạo chuyên nghiệp.', 
49990000, 'macbook-m3.jpg', 20);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (2, N'MacBook Air M2 13 inch', 
N'MacBook Air M2 - Siêu mỏng nhẹ chỉ 1.24kg, chip M2 8 nhân CPU, 10 nhân GPU. Màn hình Retina 13.6 inch, pin 18 giờ, 8GB RAM, SSD 256GB. Không quạt, hoàn toàn im lặng. Hoàn hảo cho học tập, văn phòng.', 
28990000, 'macbookair-m2.jpg', 25);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (2, N'Dell XPS 15 9530', 
N'Dell XPS 15 - Laptop Windows cao cấp với màn hình OLED 15.6 inch 3.5K. Intel Core i7-13700H, RTX 4050, 16GB RAM, SSD 512GB. Thiết kế viền mỏng InfinityEdge, bàn phím có đèn nền, pin 86Wh.', 
42990000, 'dell-xps15.jpg', 15);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (2, N'Asus ROG Strix G16', 
N'Asus ROG Strix G16 - Gaming laptop mạnh mẽ với Intel Core i7-13650HX, RTX 4060 8GB. Màn hình 16 inch QHD 240Hz, RAM 16GB DDR5, SSD 512GB. Tản nhiệt ROG Intelligent Cooling, RGB Aura Sync.', 
35990000, 'asus-rog-g16.jpg', 18);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (2, N'Lenovo ThinkPad X1 Carbon Gen 11', 
N'Lenovo ThinkPad X1 Carbon - Business laptop cao cấp, siêu bền với carbon fiber. Intel Core i7-1355U, 16GB RAM, SSD 512GB. Màn hình 14 inch WUXGA, bàn phím TrackPoint huyền thoại, pin 12 giờ.', 
38990000, 'thinkpad-x1.jpg', 22);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (2, N'HP Spectre x360 14', 
N'HP Spectre x360 - Laptop 2-in-1 sang trọng với màn hình OLED 13.5 inch cảm ứng. Intel Core i7-1355U, 16GB RAM, SSD 512GB. Xoay 360 độ, bút HP Pen đi kèm, pin 16 giờ, thiết kế gem-cut.', 
36990000, 'hp-spectre.jpg', 20);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (2, N'Acer Aspire 5 A515', 
N'Acer Aspire 5 - Laptop tầm trung hiệu năng tốt với Intel Core i5-1235U, 8GB RAM, SSD 512GB. Màn hình 15.6 inch Full HD IPS, pin 8 giờ. Phù hợp học tập, văn phòng với giá cả hợp lý.', 
13990000, 'acer-aspire5.jpg', 30);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (2, N'MSI Titan 18 HX', 
N'MSI Titan 18 HX - Gaming laptop đỉnh cao với Intel Core i9-14900HX, RTX 4090 16GB. Màn hình Mini LED 18 inch 4K 120Hz, RAM 64GB, SSD 2TB. Tản nhiệt Cooler Boost 5, bàn phím Cherry MX.', 
99990000, 'msi-titan18.jpg', 12);

-- Products - Phụ kiện
INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (3, N'AirPods Pro 2nd Gen', 
N'AirPods Pro 2 - Tai nghe true wireless cao cấp với chip H2, chống ồn chủ động ANC 2x tốt hơn. Hộp sạc MagSafe, pin 30 giờ, chống nước IPX4, Spatial Audio. Điều khiển cảm ứng vuốt, tìm chính xác với Find My.', 
6490000, 'airpods-pro2.jpg', 50);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (3, N'Samsung Galaxy Buds 2 Pro', 
N'Galaxy Buds 2 Pro - Tai nghe cao cấp với ANC thông minh, 360 Audio, Hi-Fi 24bit. Pin 8 giờ, sạc không dây, IPX7. Thiết kế nhỏ gọn, thoải mái cả ngày, kết nối đa thiết bị seamless.', 
4490000, 'galaxy-buds2pro.jpg', 45);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (3, N'Sạc nhanh 65W GaN', 
N'Sạc nhanh GaN 65W - Công nghệ GaN nhỏ gọn, 3 cổng USB (2x USB-C, 1x USB-A). Sạc laptop, điện thoại, tablet cùng lúc. Hỗ trợ PD 3.0, QC 4.0, bảo vệ quá dòng/nhiệt. Gọn nhẹ, phù hợp di động.', 
690000, 'charger-65w.jpg', 60);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (3, N'Cáp sạc USB-C to Lightning', 
N'Cáp sạc USB-C to Lightning - Chính hãng Apple MFi, hỗ trợ sạc nhanh 20W. Dài 1m/2m, bện nylon siêu bền, đầu cắm chống gãy. Tương thích iPhone, iPad, AirPods. Đồng bộ dữ liệu tốc độ cao.', 
490000, 'cable-lightning.jpg', 100);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (3, N'Ốp lưng iPhone 15 Pro Max Silicone', 
N'Ốp lưng Silicone cao cấp - Chính hãng Apple, lót nhung mềm mại bên trong. Bảo vệ toàn diện, grip tốt, không bám vân tay. Hỗ trợ MagSafe. Nhiều màu sắc thời trang: Đen, Xanh, Hồng, Trắng.', 
890000, 'case-iphone15.jpg', 40);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (3, N'Kính cường lực Gorilla Glass', 
N'Kính cường lực Gorilla Glass - Độ cứng 9H, chống va đập, chống trầy. Tráng oleophobic chống bám vân tay. Dễ dàng dán, không bọt khí. Độ trong suốt cao 99%, cảm ứng nhạy. Bảo vệ màn hình tối đa.', 
190000, 'screen-protector.jpg', 55);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (3, N'Pin sạc dự phòng 20000mAh', 
N'Pin sạc dự phòng 20000mAh - Dung lượng lớn, sạc được iPhone 4-5 lần. 2 cổng USB-C PD 20W + 1 USB-A QC 3.0. Màn hình LED hiển thị %, sạc không dây 10W. Nhỏ gọn, nhẹ 400g, an toàn.', 
590000, 'powerbank-20000.jpg', 35);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (3, N'Giá đỡ điện thoại xoay 360', 
N'Giá đỡ điện thoại đa năng - Xoay 360 độ, gập gọn tiện lợi. Dán chắc chắn, không rơi. Tương thích MagSafe cho iPhone 12 trở lên. Vật liệu kim loại cao cấp, nhiều màu sắc. Dùng làm chân đế xem phim.', 
290000, 'phone-stand.jpg', 30);

-- Products - Tablet
INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (4, N'iPad Pro 12.9 inch M2', 
N'iPad Pro 12.9 M2 - Màn hình Liquid Retina XDR mini-LED tuyệt đẹp. Chip M2 mạnh mẽ, camera TrueDepth, Face ID. Hỗ trợ Apple Pencil 2, Magic Keyboard. 128GB-2TB, pin 10 giờ. Lý tưởng cho sáng tạo chuyên nghiệp.', 
28990000, 'ipad-pro-129.jpg', 20);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (4, N'iPad Air 10.9 inch M1', 
N'iPad Air M1 - Hiệu năng mạnh mẽ với chip M1, màn hình Liquid Retina 10.9 inch. Touch ID cạnh viền, camera 12MP, USB-C. Hỗ trợ Magic Keyboard, Apple Pencil 2. 64GB-256GB. Cân bằng hoàn hảo giữa giá và hiệu năng.', 
16990000, 'ipad-air.jpg', 25);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (4, N'iPad 10.9 inch 2022', 
N'iPad 10.9 - Thiết kế viền mỏng hiện đại, chip A14 Bionic. Màn hình Liquid Retina 10.9 inch, camera 12MP trước sau. Hỗ trợ Apple Pencil gen 1, Magic Keyboard Folio. 64GB-256GB. Tuyệt vời cho học tập, giải trí.', 
10990000, 'ipad-109.jpg', 30);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (4, N'Samsung Galaxy Tab S9 Ultra', 
N'Galaxy Tab S9 Ultra - Màn hình khủng 14.6 inch Dynamic AMOLED 2X 120Hz. Chip Snapdragon 8 Gen 2, RAM 12GB, S Pen đi kèm. Pin 11200mAh, chống nước IP68. Tuyệt vời cho đa nhiệm, xem phim, làm việc.', 
26990000, 'tab-s9-ultra.jpg', 22);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (4, N'Xiaomi Pad 6', 
N'Xiaomi Pad 6 - Màn hình 11 inch 144Hz siêu mượt, chip Snapdragon 870. RAM 8GB, bộ nhớ 256GB, pin 8840mAh với sạc nhanh 33W. 4 loa Dolby Atmos. Giá cả phải chăng cho hiệu năng tốt.', 
8990000, 'xiaomi-pad6.jpg', 18);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (4, N'Lenovo Tab P11 Pro Gen 2', 
N'Lenovo Tab P11 Pro - Màn hình OLED 11.2 inch 2.5K, chip MediaTek Kompanio 1300T. RAM 8GB, bộ nhớ 256GB, 4 loa JBL. Hỗ trợ Lenovo Pen Plus, pin 8000mAh. Tốt cho giải trí, làm việc.', 
11990000, 'lenovo-tabp11.jpg', 20);

-- Products - Đồng hồ thông minh
INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (5, N'Apple Watch Ultra 2', 
N'Apple Watch Ultra 2 - Đồng hồ thể thao cực hạn với vỏ Titanium 49mm siêu bền. Màn hình Retina 3000 nits sáng nhất, chip S9, pin 36 giờ. GPS kép, độ sâu 100m, cảm biến nhiệt độ. Lý tưởng cho thể thao, phiêu lưu.', 
21990000, 'watch-ultra2.jpg', 25);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (5, N'Apple Watch Series 9', 
N'Apple Watch Series 9 - Chip S9 mạnh mẽ, màn hình Retina sáng hơn 2000 nits. Cử chỉ nhấn đôi ngón, theo dõi sức khỏe toàn diện, ECG, đo nồng độ oxy. Vỏ nhôm/thép, nhiều màu sắc. Pin 18 giờ, sạc nhanh.', 
10990000, 'watch-series9.jpg', 30);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (5, N'Samsung Galaxy Watch 6 Classic', 
N'Galaxy Watch 6 Classic - Vòng xoay vật lý cổ điển, màn hình Super AMOLED 1.5 inch. Chip Exynos W930, RAM 2GB, theo dõi sức khỏe toàn diện. Pin 425mAh, sạc nhanh. Tương thích Android và iPhone (giới hạn).', 
8990000, 'galaxy-watch6.jpg', 28);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (5, N'Xiaomi Watch S2', 
N'Xiaomi Watch S2 - Thiết kế sang trọng với vỏ thép không gỉ. Màn hình AMOLED 1.43 inch, pin 12 ngày. Hỗ trợ 190+ chế độ thể thao, GPS kép, đo SpO2, nhịp tim 24/7. Giá cả phải chăng với tính năng cao cấp.', 
4990000, 'xiaomi-watch-s2.jpg', 35);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (5, N'Amazfit GTR 4', 
N'Amazfit GTR 4 - Pin khủng 14 ngày, màn hình AMOLED 1.43 inch. GPS kép chính xác, đo SpO2, nhịp tim, giấc ngủ. 150+ chế độ thể thao, chống nước 5ATM. Alexa tích hợp. Giá tốt cho tính năng phong phú.', 
3990000, 'amazfit-gtr4.jpg', 40);

INSERT INTO [Product] (CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage, ProductQuantity) 
VALUES (5, N'Huawei Watch GT 4', 
N'Huawei Watch GT 4 - Thiết kế octagonal độc đáo, màn hình AMOLED 1.43 inch. Pin 14 ngày, GPS kép, 100+ chế độ thể thao. Theo dõi sức khỏe toàn diện với TruSeen 5.5+. Bluetooth calling, NFC.', 
6990000, 'huawei-gt4.jpg', 32);

PRINT '✓ Đã thêm 40+ sản phẩm (có số lượng tồn kho)';

-- Sample Customer
INSERT INTO [Customer] (CustomerName, CustomerEmail, CustomerPhone, CustomerAddress, Username)
VALUES (N'Nguyễn Văn A', 'nguyenvana@email.com', '0901234567', N'123 Lê Lợi, Q1, TP.HCM', 'customer1');

PRINT '✓ Đã thêm customer mẫu';
PRINT '';

-- ========================================
-- BƯỚC 4: THỐNG KÊ
-- ========================================

PRINT '========================================';
PRINT 'BƯỚC 4: Thống kê dữ liệu';
PRINT '========================================';

-- Thống kê tổng quan
PRINT 'TỔNG QUAN:';
SELECT 'Users' AS N'Bảng', COUNT(*) AS N'Số lượng' FROM [User]
UNION ALL
SELECT 'Categories', COUNT(*) FROM [Category]
UNION ALL
SELECT 'Products', COUNT(*) FROM [Product]
UNION ALL
SELECT 'Customers', COUNT(*) FROM [Customer];

-- Thống kê sản phẩm theo danh mục
PRINT '';
PRINT 'SẢN PHẨM THEO DANH MỤC:';
SELECT 
    c.CategoryName AS N'Danh mục',
    COUNT(p.ProductID) AS N'Số sản phẩm',
    SUM(p.ProductQuantity) AS N'Tổng tồn kho',
    AVG(p.ProductQuantity) AS N'TB tồn kho',
    MIN(p.ProductPrice) AS N'Giá thấp nhất',
    MAX(p.ProductPrice) AS N'Giá cao nhất'
FROM Product p
INNER JOIN Category c ON p.CategoryID = c.CategoryID
GROUP BY c.CategoryName
ORDER BY c.CategoryName;

PRINT '';
PRINT '========================================';
PRINT '✅ HOÀN TẤT SETUP DATABASE!';
PRINT '========================================';
PRINT '';
PRINT 'THÔNG TIN ĐĂNG NHẬP:';
PRINT '  Admin   : admin / admin123';
PRINT '  Customer: customer1 / pass123';
PRINT '';
PRINT '📊 Database đã sẵn sàng sử dụng!';
PRINT '';

GO

