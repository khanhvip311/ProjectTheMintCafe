$(document).ready(function () {

    //sự kiện phân loại sản phẩm
    $(".category-link").click(function (e) {
        e.preventDefault();
        var categoryId = $(this).attr("data-category");

        $.ajax({
            url: "/Staff/FilterProducts",
            type: "GET",
            data: { categoryId: categoryId },
            success: function (response) {
                console.log("Response:", response); // Kiểm tra JSON
                var productContainer = $(".shop-product-wrap");
                productContainer.empty();

                $.each(response, function (index, product) {
                    var productHtml = `
                                                <div class="col-xl-3 col-md-4 col-sm-4 col-4">
                                                    <div class="product-item">
                                                        <div class="product-thumb">
                                                            <img src="/assets/images/product/${product.image}" alt="${product.name}">
                                                            <div class="product-action-link">
                                                                <a href="/assets/images/product/${product.image}" data-rel="lightcase"><i class="icofont-eye"></i></a>
                                                                <a href="#"><i class="icofont-heart-alt"></i></a>
                                                                <a href="#"><i class="icofont-cart-alt"></i></a>
                                                            </div>
                                                        </div>
                                                        <div class="product-content">
                                                            <input type="hidden" value="${product.productId}" />
                                                            <div class="product-title">
                                                                <h6><a href="#">${product.name}</a></h6>
                                                                <span class="price">${new Intl.NumberFormat().format(product.price)} VND</span>
                                                                <div class="rating">
                                                                    <i class="icofont-star"></i>
                                                                    <i class="icofont-star"></i>
                                                                    <i class="icofont-star"></i>
                                                                    <i class="icofont-star"></i>
                                                                    <i class="icofont-star"></i>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>`;
                    productContainer.append(productHtml);
                });
            },
            error: function (xhr, status, error) {
                console.log("Error: " + error);
                alert("Có lỗi xảy ra khi tải sản phẩm!");
            }
        });
    });




    // Hàm tính tổng tiền
    function updateTotalPrice() {
        // Tính tổng tiền trước chiết khấu
        var price = 0;
        $('.choose-list tbody tr').each(function () {
            var quantity = parseInt($(this).find('.quantity-input').val()) || 0;
            var unitPrice = parseInt($(this).data('unit-price')) || 0;
            price += unitPrice * quantity;
        });

        // Lấy giá trị chiết khấu (phần trăm) từ input
        var discountValue = parseInt($('#discountvalue').val()) || 0;
        discountValue = Math.max(0, Math.min(100, discountValue)); // Giới hạn discountValue từ 0 đến 100

        // Tính tổng tiền sau chiết khấu
        var totalPrice = price - (price * discountValue / 100);
        totalPrice = Math.max(0, totalPrice); // Đảm bảo tổng tiền không âm

        // Format và hiển thị tổng tiền
        var formattedTotal = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(totalPrice);
        $('#total-price-value').val(totalPrice);
        $('#totalPrice').text(formattedTotal);

        // Lưu giá trước chiết khấu để gửi lên server
        $('#total-price-value').data('price', price);
    }

    // Sử dụng event delegation để gắn sự kiện click cho product-content
    $('.shop-product-wrap').on('click', '.product-content', function (e) {
        e.preventDefault(); // Ngăn chặn hành vi mặc định (tải lại trang)

        // Lấy ProductId từ thẻ input hidden trong product-content
        var productId = $(this).find('input[type="hidden"]').val();

        // Kiểm tra xem sản phẩm đã tồn tại trong bảng chưa
        var existingRow = $('.choose-list tbody tr').filter(function () {
            return $(this).find('.product-id').val() === productId;
        });

        if (existingRow.length > 0) {
            // Nếu sản phẩm đã tồn tại, tăng số lượng lên
            var $quantityInput = existingRow.find('.quantity-input');
            var currentQuantity = parseInt($quantityInput.val()) || 0;
            var newQuantity = currentQuantity + 1;
            $quantityInput.val(newQuantity);

            // Cập nhật tổng tiền
            updateTotalPrice();
        } else {
            // Nếu sản phẩm chưa tồn tại, gửi yêu cầu AJAX để lấy thông tin sản phẩm
            $.ajax({
                url: '/Staff/GetProductById', // URL của action trong controller
                type: 'GET',
                data: { productId: productId }, // Gửi ProductId
                success: function (response) {
                    console.log("Response:", response); // Kiểm tra JSON
                    if (response.success) {
                        // Lấy dữ liệu sản phẩm từ response
                        var product = response.product;

                        // Tính STT mới (dựa trên số hàng hiện có trong bảng)
                        var stt = $('.choose-list tbody tr').length + 1;

                        // Format giá tiền
                        var price = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(product.price);

                        // Tạo HTML cho hàng mới
                        var newRow = `
                            <tr data-unit-price="${product.price}">
                                <td>${stt}</td>
                                <td>
                                    ${product.name}
                                    <input type="hidden" value="${product.productId}" class="product-id" />
                                </td>
                                <td class="quantity-cell">
                                    <div class="quantity-wrapper">
                                        <input type="number" class="quantity-input" value="1" min="1" step="1" />
                                    </div>
                                </td>
                                <td><textarea class="note-textarea"></textarea></td>
                                <td>${price}</td>
                                <td>
                                    <button type="button" class="btn-remove-item" style="color: red;">
                                        <i class="icofont-trash"></i> <!-- Biểu tượng thùng rác -->
                                    </button>
                                </td>
                            </tr>
                        `;

                        // Thêm hàng mới vào tbody của bảng
                        $('.choose-list tbody').append(newRow);

                        // Cập nhật tổng tiền
                        updateTotalPrice();
                    } else {
                        alert('Không tìm thấy sản phẩm!');
                    }
                },
                error: function () {
                    alert('Có lỗi xảy ra khi lấy dữ liệu sản phẩm!');
                }
            });
        }
    });

    // Xử lý khi số lượng thay đổi trực tiếp (qua input)
    $('.choose-list').on('change', '.quantity-input', function () {
        var $row = $(this).closest('tr');
        var newQuantity = parseInt($(this).val()) || 0;
        if (newQuantity <= 0) {
            $row.remove(); // Xóa hàng nếu số lượng về 0
            // Cập nhật lại STT
            $('.choose-list tbody tr').each(function (index) {
                $(this).find('td:first').text(index + 1);
            });
        }
        // Cập nhật tổng tiền
        updateTotalPrice();
    });


    //Xử lý discount
    $('#discountvalue').on('change', function () {
        updateTotalPrice();
    });

    // Xử lý nút Xóa
    $('#btnXoa').on('click', function () {
        $('.choose-list tbody').empty(); // Xóa toàn bộ hàng
        updateTotalPrice(); // Cập nhật tổng tiền về 0
    });
    // Xử lý nút Xóa từng món
    $('.choose-list').on('click', '.btn-remove-item', function () {
        var $row = $(this).closest('tr');
        $row.remove(); // Xóa hàng

        // Cập nhật lại STT
        $('.choose-list tbody tr').each(function (index) {
            $(this).find('td:first').text(index + 1);
        });

        // Cập nhật tổng tiền
        updateTotalPrice();
    });


    // Hàm cập nhật trạng thái nút Xác nhận
    function updateConfirmButtonState() {
        var paymentMethod = $('select[name="paymethod"]').val();
        if (paymentMethod == "1") {
            $('#btnXacNhan').prop('disabled', false); // Kích hoạt nút
            $('#btnMomo').hide(); // Ẩn nút

        } else if (paymentMethod == "2") {
            $('#btnXacNhan').prop('disabled', true); // Vô hiệu hóa nút
            $('#btnMomo').show(); // Hiển thị nút

        }
    }
    updateConfirmButtonState();
    // Thiết lập sự kiện change cho thẻ select
    $('select[name="paymethod"]').on('change', function () {
        updateConfirmButtonState();
    });

    // Xử lý nút xác nhận
    $('#btnXacNhan').on('click', function () {
        if (!confirm('Bạn có chắc muốn xác nhận đơn hàng này không?')) {
            return;
        }

        $('#btnXacNhan').prop('disabled', true).text('Đang xử lý...');

        var billDetails = [];
        $('.choose-list tbody tr').each(function () {
            var $row = $(this);
            var productId = parseInt($row.find('.product-id').val());
            var quantity = parseInt($row.find('.quantity-input').val()) || 0;
            var note = $row.find('.note-textarea').val() || '';

            if (quantity > 0) {
                billDetails.push({
                    ProductId: productId,
                    Quantity: quantity,
                    Note: note
                });
            }
        });

        if (billDetails.length === 0) {
            alert('Danh sách món trống! Vui lòng thêm món trước khi xác nhận.');
            $('#btnXacNhan').prop('disabled', false).text('Xác nhận');
            return;
        }

        var totalPrice = parseFloat($('#total-price-value').val()) || 0;
        var price = parseFloat($('#total-price-value').data('price')) || 0;
        var discountValue = parseInt($('#discountvalue').val()) || 0;
        var paymentMethod = $('select[name="paymethod"]').val();
        var paymentMethodValue = paymentMethod ? parseInt(paymentMethod) : 0;

        var billData = {
            BillDetails: billDetails,
            Price: price,
            TotalPrice: totalPrice,
            Discount: discountValue,
            PaymentMethod: paymentMethodValue
        };

        // Ghi log dữ liệu trước khi gửi
        console.log('Sending data:', JSON.stringify(billData, null, 2));

        $.ajax({
            url: '/Staff/SaveBill',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(billData),
            beforeSend: function (xhr) {
                console.log('Before send:', xhr);
            },
            success: function (response) {
                console.log('Response:', response);
                if (response.success) {
                    alert('Hóa đơn đã được lưu thành công!');
                    $('.choose-list tbody').empty();
                    $('#discountvalue').val(0);
                    updateTotalPrice();
                } else {
                    alert('Có lỗi xảy ra khi lưu hóa đơn: ' + (response.message || 'Lỗi không xác định từ server.'));
                }
            },
            error: function (xhr, status, error) {
                console.error('AJAX Error:', status, error, xhr.responseText);
                alert('Có lỗi xảy ra khi gửi yêu cầu đến server: ' + (xhr.responseText || 'Không xác định'));
            },
            complete: function () {
                $('#btnXacNhan').prop('disabled', false).text('Xác nhận');
            }
        });
    });

    function updateCartData() {
        var billDetails = [];
        $('.choose-list tbody tr').each(function () {
            var $row = $(this);
            var productId = parseInt($row.find('.product-id').val());
            var quantity = parseInt($row.find('.quantity-input').val()) || 0;
            var note = $row.find('.note-textarea').val() || '';

            if (quantity > 0) {
                billDetails.push({
                    ProductId: productId,
                    Quantity: quantity,
                    Note: note
                });
            }
        });

        var totalPrice = parseFloat($('#total-price-value').val()) || 0;
        var price = parseFloat($('#total-price-value').data('price')) || 0;
        var discountValue = parseInt($('#discountvalue').val()) || 0;
        var paymentMethod = $('select[name="paymethod"]').val();
        var paymentMethodValue = paymentMethod ? parseInt(paymentMethod) : 0;

        var cartData = {
            BillDetails: billDetails,
            Price: price,
            TotalPrice: totalPrice,
            Discount: discountValue,
            PaymentMethod: paymentMethodValue
        };

        $('#cartDataInput').val(JSON.stringify(cartData));
        console.log('Cart Data Updated:', JSON.stringify(cartData, null, 2));
    }


     //Xử lý nút thanh toán momo
    $('#btnMomo').on('click', function (e) {
        e.preventDefault();
        var billDetails = [];
        $('.choose-list tbody tr').each(function () {
            var $row = $(this);
            var productId = parseInt($row.find('.product-id').val());
            var quantity = parseInt($row.find('.quantity-input').val()) || 0;
            var note = $row.find('.note-textarea').val() || '';

            if (quantity > 0) {
                billDetails.push({
                    ProductId: productId,
                    Quantity: quantity,
                    Note: note
                });
            }
        });

        if (billDetails.length === 0) {
            alert('Danh sách món trống! Vui lòng thêm món trước khi thanh toán.');
            return;
        }

        updateCartData();
        $(this).closest('form').submit();
    });
   
});


