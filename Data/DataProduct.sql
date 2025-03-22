INSERT INTO [ManagementCafe].[dbo].[Product] 
([ProductID], [Name], [Description], [Price], [Status], [Image], [CateID])
VALUES
-- Cà phê (CateID = 1)
(1, N'Cà phê đen', N'Cà phê nguyên chất không đường', 25000, N'instock', N'coffee_black.png', 1),
(2, N'Cà phê sữa', N'Cà phê pha với sữa đặc', 30000, N'instock', N'coffee_milk.png', 1),
(3, N'Espresso', N'Cà phê Ý nguyên chất', 35000, N'instock', N'espresso.png', 1),
(4, N'Cappuccino', N'Cà phê Ý với bọt sữa', 40000, N'instock', N'cappuccino.png', 1),
(5, N'Latte', N'Cà phê sữa Ý', 40000, N'instock', N'latte.png', 1),
(6, N'Mocha', N'Cà phê sô cô la', 45000, N'instock', N'mocha.png', 1),
(7, N'Bạc xỉu', N'Cà phê sữa pha nhiều sữa hơn cà phê', 35000, N'instock', N'bac_xiu.png', 1),
(8, N'Matcha Latte', N'Trà xanh Nhật Bản với sữa', 45000, N'instock', N'matcha_latte.png', 1),


-- Trà & Trà sữa (CateID = 2)
(9, N'Trà đào', N'Trà pha với đào tươi', 35000, N'instock', N'peach_tea.png', 2),
(10, N'Trà chanh', N'Trà xanh pha với chanh', 30000, N'instock', N'lemon_tea.png', 2),
(11, N'Trà sữa truyền thống', N'Trà sữa với trân châu đen', 35000, N'instock', N'milk_tea.png', 2),
(12, N'Trà sữa matcha', N'Trà xanh Nhật Bản với sữa và trân châu', 40000, N'instock', N'matcha_milk_tea.png', 2),
(13, N'Trà việt quất', N'Trà pha với việt quất', 40000, N'instock', N'blueberry_tea.png', 2),
(14, N'Trà ổi hồng', N'Trà pha với ổi hồng', 40000, N'instock', N'pink_guava_tea.png', 2),
(15, N'Trà lài Kiwi', N'Trà lài pha với Kiwi', 40000, N'instock', N'kiwi_tea.png', 2),
(16, N'Trà vải thiều', N'Trà pha với vải thiều', 40000, N'instock', N'lychee_tea.png', 2),

-- Sinh tố & Nước ép (CateID = 3)
(17, N'Sinh tố bơ', N'Sinh tố bơ tươi', 40000, N'instock', N'avocado_smoothie.png', 3),
(18, N'Sinh tố xoài', N'Sinh tố xoài thơm ngon', 40000, N'instock', N'mango_smoothie.png', 3),
(19, N'Sinh tố dâu', N'Sinh tố dâu tây', 40000, N'instock', N'strawberry_smoothie.png', 3),

-- Nước ép (CateID = 4)
(20, N'Nước dừa', N'Nước dừa xiêm tươi', 35000, N'instock', N'coconut_water.png', 4),
(21, N'Nước chanh', N'Nước chanh tươi', 30000, N'instock', N'lemon_juice.png', 4),
(22, N'Nước ép cam', N'Nước ép cam tươi nguyên chất', 35000, N'instock', N'orange_juice.png', 4),
(23, N'Nước ép dưa hấu', N'Nước ép dưa hấu mát lạnh', 30000, N'instock', N'watermelon_juice.png', 4),
(24, N'Nước ép cà rốt', N'Nước ép cà rốt giàu vitamin A', 32000, N'instock', N'carrot_juice.png', 4),
(25, N'Nước ép táo', N'Nước ép táo ngọt dịu', 37000, N'instock', N'apple_juice.png', 4),
(26, N'Nước ép ổi', N'Nước ép ổi tươi bổ dưỡng', 33000, N'instock', N'guava_juice.png', 4),

-- Soda (CateID = 5)
(27, N'Soda chanh dây', N'Soda kết hợp với chanh dây', 40000, N'instock', N'passion_soda.png', 5),
(28, N'Soda việt quất', N'Soda pha với siro việt quất', 40000, N'instock', N'blueberry_soda.png', 5),
(29, N'Soda dâu', N'Soda pha với siro dâu', 40000, N'instock', N'strawberry_soda.png', 5),

-- Đá xay (CateID = 6)
(30, N'Cookie đá xay', N'Cookie xay với sữa và kem tươi', 45000, N'instock', N'cookie_blended.png', 6),
(31, N'Caramel đá xay', N'Cà phê caramel xay với đá', 47000, N'instock', N'caramel_blended.png', 6),
(32, N'Socola đá xay', N'Socola xay với kem béo', 46000, N'instock', N'choco_blended.png', 6),
(33, N'Matcha đá xay', N'Trà xanh matcha xay với đá', 48000, N'instock', N'matcha_blended.png', 6),
(34, N'Vanilla đá xay', N'Vanilla thơm ngon kết hợp với đá', 44000, N'instock', N'vanilla_blended.png', 6),

-- Bánh ngọt (CateID = 7)
(35, N'Bánh croissant', N'Bánh sừng bò kiểu Pháp', 25000, N'instock', N'croissant.png', 7),
(36, N'Bánh tart trứng', N'Bánh tart nhân kem trứng', 30000, N'instock', N'egg_tart.png', 7),
(37, N'Bánh mì nướng mật ong', N'Bánh mì nướng với mật ong', 35000, N'instock', N'honey_toast.png', 7),
(38, N'Bánh tiramisu', N'Bánh tiramisu kiểu Ý', 45000, N'instock', N'tiramisu.png', 7),
(39, N'Bánh red velvet', N'Bánh kem red velvet', 50000, N'instock', N'red_velvet.png', 7),
(40, N'Bánh mousse chanh dây', N'Bánh mousse vị chanh dây', 45000, N'instock', N'passion_mousse.png', 7),
(41, N'Cookies socola', N'Bánh quy socola', 25000, N'instock', N'choco_cookies.png', 7),

-- Topping (CateID = 8)
(42, N'Trân châu đen', N'Trân châu đen dẻo dai', 10000, N'instock', N'black_pearl.png', 8),
(43, N'Trân châu trắng', N'Trân châu trắng giòn giòn', 10000, N'instock', N'white_pearl.png', 8),
(44, N'Pudding trứng', N'Pudding trứng béo ngậy', 12000, N'instock', N'egg_pudding.png', 8),
(45, N'Rau câu dừa', N'Rau câu dừa ngọt mát', 10000, N'instock', N'coconut_jelly.png', 8),
(46, N'Foam phô mai', N'Lớp foam phô mai béo mịn', 15000, N'instock', N'cheese_foam.png', 8);
