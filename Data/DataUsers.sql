INSERT INTO [ManagementCafe].[dbo].[Users] 
([UserID], [Name], [Phone], [Email], [Address], [Pass], [Role]) 
VALUES
(1, N'Nguyễn Văn A', '0987654321', 'nguyenvana@gmail.com', N'123 Đường ABC, TP.HCM', 'password123', 'admin'),
(2, N'Trần Thị B', '0912345678', 'tranthib@gmail.com', N'456 Đường XYZ, Hà Nội', 'pass456', 'staff'),
(3, N'Lê Văn C', '0909123456', 'levanc@gmail.com', N'789 Đường DEF, Đà Nẵng', 'securepass', 'customer'),
(4, N'Phạm Thị D', '0933222111', 'phamthid@gmail.com', N'101 Đường GHI, Cần Thơ', '12345678', 'customer'),
(5, N'Hoàng Minh E', '0977888999', 'hoangminhe@gmail.com', N'202 Đường JKL, Hải Phòng', 'abcdefg', 'staff');
