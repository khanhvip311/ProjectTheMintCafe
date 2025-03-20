INSERT INTO [ManagementCafe].[dbo].[Product] 
([ProductID], [Name], [Description], [Price], [Status], [Image], [CateID]) 
VALUES
(1, N'Cà phê đen', N'Cà phê nguyên chất không đường', 25000, N'sales', N'coffee_black.jpg', 1),
(2, N'Cà phê sữa', N'Cà phê pha với sữa đặc', 30000, N'sales', N'coffee_milk.jpg', 1),
(3, N'Espresso', N'Cà phê Ý nguyên chất', 35000, N'sales', N'espresso.jpg', 1),
(4, N'Cappuccino', N'Cà phê Ý với bọt sữa', 40000, N'sales', N'cappuccino.jpg', 1),
(5, N'Latte', N'Cà phê sữa Ý', 40000, N'sales', N'latte.jpg', 1),
(6, N'Mocha', N'Cà phê sô cô la', 45000, N'sales', N'mocha.jpg', 1),
(7, N'Bạc xỉu', N'Cà phê sữa pha nhiều sữa hơn cà phê', 35000, N'sales', N'bac_xiu.jpg', 1),

(8, N'Matcha Latte', N'Trà xanh Nhật Bản với sữa', 45000, N'sales', N'matcha_latte.jpg', 2),
(9, N'Trà đào', N'Trà pha với đào tươi', 35000, N'sales', N'peach_tea.jpg', 2),
(10, N'Trà chanh', N'Trà xanh pha với chanh', 30000, N'sales', N'lemon_tea.jpg', 2),
(11, N'Trà sữa truyền thống', N'Trà sữa với trân châu đen', 35000, N'sales', N'milk_tea.jpg', 2),
(12, N'Trà sữa matcha', N'Trà xanh Nhật Bản với sữa và trân châu', 40000, N'sales', N'matcha_milk_tea.jpg', 2),
(13, N'Trà việt quất', N'Trà pha với việt quất', 40000, N'sales', N'blueberry_tea.jpg', 2),
(14, N'Trà ổi hồng', N'Trà pha với ổi hồng', 40000, N'sales', N'pink_guava_tea.jpg', 2),
(15, N'Trà lài Kiwi', N'Trà lài pha với Kiwi', 40000, N'sales', N'kiwi_tea.jpg', 2),
(16, N'Trà vải thiều', N'Trà pha với vải thiều', 40000, N'sales', N'lychee_tea.jpg', 2),

(17, N'Sinh tố bơ', N'Sinh tố bơ tươi', 40000, N'sales', N'avocado_smoothie.jpg', 3),
(18, N'Sinh tố xoài', N'Sinh tố xoài thơm ngon', 40000, N'sales', N'mango_smoothie.jpg', 3),
(19, N'Sinh tố dâu', N'Sinh tố dâu tây', 40000, N'sales', N'strawberry_smoothie.jpg', 3),
(20, N'Nước cam', N'Nước cam tươi nguyên chất', 35000, N'sales', N'orange_juice.jpg', 3),
(21, N'Nước chanh', N'Nước chanh tươi', 30000, N'sales', N'lemon_juice.jpg', 3),
(22, N'Nước dừa', N'Nước dừa xiêm tươi', 35000, N'sales', N'coconut_water.jpg', 3),

(23, N'Soda chanh dây', N'Soda kết hợp với chanh dây', 40000, N'sales', N'passion_soda.jpg', 4),
(24, N'Soda việt quất', N'Soda pha với siro việt quất', 40000, N'sales', N'blueberry_soda.jpg', 4),
(25, N'Soda dâu', N'Soda pha với siro dâu', 40000, N'sales', N'strawberry_soda.jpg', 4),

(26, N'Bánh croissant', N'Bánh sừng bò kiểu Pháp', 25000, N'sales', N'croissant.jpg', 5),
(27, N'Bánh tart trứng', N'Bánh tart nhân kem trứng', 30000, N'sales', N'egg_tart.jpg', 5),
(28, N'Bánh mì nướng mật ong', N'Bánh mì nướng với mật ong', 35000, N'sales', N'honey_toast.jpg', 5),
(29, N'Bánh tiramisu', N'Bánh tiramisu kiểu Ý', 45000, N'sales', N'tiramisu.jpg', 5),
(30, N'Bánh red velvet', N'Bánh kem red velvet', 50000, N'sales', N'red_velvet.jpg', 5),
(31, N'Bánh mousse chanh dây', N'Bánh mousse vị chanh dây', 45000, N'sales', N'passion_mousse.jpg', 5),
(32, N'Cookies socola', N'Bánh quy socola', 25000, N'sales', N'choco_cookies.jpg', 5);
