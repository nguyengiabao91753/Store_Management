document.addEventListener("DOMContentLoaded", () => {
    console.log("📊 Report page loaded");

    const ctx = document.getElementById("BarChart").getContext("2d");
    ctx.canvas.height = 220;
    let barChart = null;
    let lineChart = null;

    function showError(msg) {
        alert(msg);
    }

    function diffDays(from, to) {
        return Math.ceil((to - from) / (1000 * 60 * 60 * 24));
    }

    const today = new Date();
    const from_date = new Date();
    from_date.setDate(today.getDate() - 30);
    loadBarChart(from_date.toISOString(), today.toISOString());

    function generateEmptyChartData(from_date, to_date) {
        const result = [];
        let start = new Date(from_date);
        const end = new Date(to_date);
        while (start <= end) {
            result.push({
                date: start.toISOString().split("T")[0],
                revenue: 0,
                orders: 0,
                products: 0
            });
            start.setDate(start.getDate() + 1);
        }
        return result;
    }

    function loadBarChart(from_date, to_date) {
        $.ajax({
            type: "POST",
            url: "/Report/GetReportBarChart",
            data: { from_date, to_date },
            success: function (data) {
                let chartData = [];

                if (!Array.isArray(data) || data.length === 0) {
                    chartData = generateEmptyChartData(from_date, to_date);
                } else {
                    chartData = data.map(item => ({
                        date: item.date.split("T")[0],
                        revenue: parseFloat(item.totalRevenue || 0),
                        orders: item.totalOrders || 0,
                        products: item.totalProductsSold || 0
                    }));

                    const allZero = chartData.every(x => x.revenue === 0);
                    if (allZero) {
                        chartData = generateEmptyChartData(from_date, to_date);
                    }
                }

                if (barChart) {
                    barChart.destroy();
                }
                barChart = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: chartData.map(x => x.date),
                        datasets: [{
                            label: 'Doanh thu (VNĐ)',
                            data: chartData.map(x => x.revenue),
                            backgroundColor: '#007bff'
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        scales: {
                            x: {
                                title: { display: true, text: "Ngày" },
                                ticks: { autoSkip: true, maxTicksLimit: 10 }
                            },
                            y: {
                                title: { display: true, text: "Doanh thu (VNĐ)" },
                                beginAtZero: true
                            }
                        },
                        plugins: {
                            tooltip: {
                                callbacks: {
                                    label: (ctx) => `${ctx.dataset.label}: ${ctx.formattedValue.toLocaleString()} VNĐ`
                                }
                            },
                            legend: { display: false }
                        },
                        onClick: (event, elements) => {
                            if (elements.length > 0) {
                                const index = elements[0].index;
                                const row = chartData[index];
                                console.log("🖱 Click:", row);
                                loadReportDetails(row.date);
                            }
                        }
                    }
                });
            },
            error: function () {
                showError("Không thể tải dữ liệu biểu đồ doanh thu.");
            }
        });
    }

    $("#btnFilter").on("click", function () {
        const from = new Date($("#fromDate").val());
        const to = new Date($("#toDate").val());

        if (isNaN(from) || isNaN(to)) {
            showError("Vui lòng chọn đầy đủ ngày bắt đầu và kết thúc.");
            return;
        }

        const days = diffDays(from, to);
        if (days > 30) {
            showError("Khoảng thời gian không được vượt quá 30 ngày.");
            return;
        }

        console.log(`📅 Lọc từ ${from.toISOString()} đến ${to.toISOString()}`);
        loadBarChart(from.toISOString(), to.toISOString());
    });

    function loadReportDetails(date) {
        console.log("🕓 Xem chi tiết ngày:", date);

        $.ajax({
            type: "POST",
            url: "/Report/GetReportDetails",
            contentType: "application/json",
            data: JSON.stringify(date),
            success: function (details) {
                if (!Array.isArray(details) || details.length === 0) {
                    alert("Không có dữ liệu chi tiết trong ngày này.");
                    return;
                }

                // 🧱 Xây HTML
                let html = `
                <div class="report-detail-card">
                    <div class="report-detail-header p-4"  style="background-color: whitesmoke;">
                        <div class="row" style="align-items: center;">
                            <h5 class="col-md-11">Chi tiết ngày ${date}</h5>
                            <button id="closeDetailBtn" class="btn-close btn-close col-md-1" style="height: 30px; width: 30px;"></button>
                        </div>
                    </div>
                    <div class="report-detail-body">
                        <table class="table table-bordered table-striped">
                            <thead class="table-light">
                                <tr>
                                    <th scope="col">Sản phẩm</th>
                                    <th scope="col">Số lượng</th>
                                    <th scope="col">Đơn giá (VNĐ)</th>
                                    <th scope="col">Thành tiền (VNĐ)</th>
                                </tr>
                            </thead>
                            <tbody>
            `;

                details.forEach(d => {
                    html += `
                    <tr>
                        <td>${d.productName}</td>
                        <td>${d.quantity}</td>
                        <td>${d.unitPrice.toLocaleString()}</td>
                        <td>${(d.quantity * d.unitPrice).toLocaleString()}</td>
                    </tr>`;
                });

                html += `
                            </tbody>
                        </table>
                    </div>
                </div>
            `;

                $("#ReportDetailModal").html(html).fadeIn(200);

                $("#closeDetailBtn").on("click", function () {
                    $("#ReportDetailModal").fadeOut(200, function () {
                        $(this).empty();
                    });
                });

                $(window).on("click", function (e) {
                    if ($(e.target).is("#ReportDetailModal")) {
                        $("#ReportDetailModal").fadeOut(200);
                    }
                });
            },
            error: function () {
                alert("Không thể tải chi tiết doanh thu.");
            }
        });
    }

    //Line Chart
    function generateEmptyLineData(from_date, to_date, groupBy) {
        const result = [];
        const start = new Date(from_date);
        const end = new Date(to_date);

        if (groupBy === "day") {
            while (start <= end) {
                result.push({ date: start.toISOString().split("T")[0], totalRevenue: 0 });
                start.setDate(start.getDate() + 1);
            }
        } else if (groupBy === "month") {
            while (start <= end) {
                const label = `${start.getMonth() + 1}-${start.getFullYear()}`;
                result.push({ date: label, totalRevenue: 0 });
                start.setMonth(start.getMonth() + 1);
            }
        } else if (groupBy === "year") {
            while (start <= end) {
                const label = start.getFullYear().toString();
                result.push({ date: label, totalRevenue: 0 });
                start.setFullYear(start.getFullYear() + 1);
            }
        }
        return result;
    }

    function loadLineChart(groupBy = "month") {
        console.log(`📈 Load Line Chart nhóm theo: ${groupBy}`);

        $.ajax({
            type: "POST",
            url: "/Report/GetReportLineChart",
            data: {groupBy },
            success: function (res) {
                let chartData = [];

                if (!res || !Array.isArray(res) || res.length === 0) {
                    chartData = generateEmptyLineData(from_date, to_date, groupBy);
                } else {
                    chartData = res.map(x => ({
                        date: formatLabel(x.date, groupBy),
                        totalRevenue: parseFloat(x.totalRevenue || 0)
                    }));

                    const allZero = chartData.every(x => x.totalRevenue === 0);
                    if (allZero) chartData = generateEmptyLineData(from_date, to_date, groupBy);
                }

                const labels = chartData.map(x => x.date);
                const values = chartData.map(x => x.totalRevenue);

                if (lineChart) lineChart.destroy();

                const ctxLine = document.getElementById("LineChart").getContext("2d");
                lineChart = new Chart(ctxLine, {
                    type: "line",
                    data: {
                        labels,
                        datasets: [{
                            label: "Doanh thu (VNĐ)",
                            data: values,
                            borderColor: "#28a745",
                            backgroundColor: "rgba(40,167,69,0.15)",
                            fill: true,
                            tension: 0.3,
                            pointRadius: 4,
                            pointHoverRadius: 6
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        scales: {
                            x: { title: { display: true, text: getXAxisLabel(groupBy) } },
                            y: { beginAtZero: true, title: { display: true, text: "Doanh thu (VNĐ)" } }
                        },
                        plugins: {
                            tooltip: {
                                callbacks: {
                                    label: ctx => ` ${ctx.formattedValue.toLocaleString()} VNĐ`
                                }
                            },
                            legend: { display: false }
                        }
                    }
                });
            },
            error: function () {
                alert("Không thể tải dữ liệu biểu đồ đường.");
            }
        });
    }   

    function getXAxisLabel(groupBy) {
        switch (groupBy) {
            case "month": return "Tháng";
            case "year": return "Năm";
            default: return "";
        }
    }

    function formatLabel(date, groupBy) {
        const d = new Date(date);
        if (groupBy === "day")
            return d.toLocaleDateString("vi-VN", { day: "2-digit", month: "2-digit", year: "numeric" });
        if (groupBy === "month")
            return `${d.getMonth() + 1}-${d.getFullYear()}`;
        return d.getFullYear().toString();
    }

    const fromDateLine = new Date();
    fromDateLine.setMonth(fromDateLine.getMonth() - 6);
    const toDateLine = new Date();

    loadLineChart("month");

    $(document).on("click", ".chart-view-option", function () {
        const groupBy = $(this).data("group");
        $("#chartViewDropdown").text($(this).text());
        loadLineChart(groupBy);
    });

    function loadValue(groupBy) {
        $.ajax({
            url: '/Report/GetValue',
            type: 'POST',
            data: { groupBy },
            success: function (res) {
                if (Array.isArray(res) && res.length > 0) {
                    const item = res[0];
                    const revenue = item.totalRevenue || 0;
                    const products = item.totalProductsSold || 0;
                    const orders = item.totalOrders || 0;

                    $('#totalRevenue').text(revenue.toLocaleString('vi-VN') + ' VNĐ');
                    $('#totalProductsSold').text(products + ' Sản phẩm');
                    $('#totalOrders').text(orders + ' Đơn hàng');
                } else {
                    $('#totalRevenue').text('0 VNĐ');
                    $('#totalProductsSold').text('0 Sản phẩm');
                    $('#totalOrders').text('0 Đơn hàng');
                }
            },
            error: function () {
                $('#totalRevenue').text('0 VNĐ');
                $('#totalProductsSold').text('0 Sản phẩm');
                $('#totalOrders').text('0 Đơn hàng');
            }
        });
    }

    loadValue("year");

    function loadBestSellingProducts() {
        $.ajax({
            type: "GET",
            url: "/Report/BestSellingProducts",
            success: function (data) {
                const container = $("#bestSellingList");
                container.empty();

                if (!Array.isArray(data) || data.length === 0) {
                    container.html("<li class='text-muted py-2'>Không có sản phẩm bán chạy nào.</li>");
                    return;
                }

                data.forEach((item, index) => {
                    const product = item.bestSellingProduct;
                    if (!product) return;

                    const productName = product.productName || "Không rõ tên";
                    const category = product.categoryName || "Danh mục khác";
                    const quantity = item.totalProductsSold || 0;

                    // Nếu có ảnh thì lấy từ product.imageUrl, nếu không thì dùng ảnh placeholder
                    const imageUrl = product.productImage
                        ? `${product.productImage}`
                        : `https://via.placeholder.com/48?text=${encodeURIComponent(productName.substring(0, 3))}`;

                    const li = `
                    <li class="d-flex justify-content-between align-items-center py-2 border-bottom">
                        <div class="d-flex align-items-center gap-2">
                            <img src="${imageUrl}" class="rounded" style="width:48px;height:48px;">
                            <div>
                                <div class="fw-semibold">${productName}</div>
                                <small class="text-muted">${category}</small>
                            </div>
                        </div>
                        <div>${quantity} Times</div>
                    </li>
                `;
                    container.append(li);
                });
            },
            error: function () {
                $(".fav-list ul").html("<li class='text-danger py-2'>Không thể tải danh sách sản phẩm bán chạy.</li>");
            }
        });
    }

    function loadLoyalCustomers() {
        $.ajax({
            type: "GET", // API của bạn đang dùng [HttpPost]
            url: "/Report/LoyalCustomers",
            success: function (data) {
                const container = $("#loyalCustomersList");
                container.empty();

                if (!Array.isArray(data) || data.length === 0) {
                    container.html("<li class='text-muted py-2'>Không có khách hàng nào.</li>");
                    return;
                }

                data.forEach((item) => {
                    const customer = item.loyalCustomer;
                    if (!customer) return;

                    const name = customer.customerName || "Khách hàng không rõ";
                    const email = customer.email || "Không có email";
                    const totalOrders = item.totalOrders || 0;
                    const totalRevenue = item.totalRevenue?.toLocaleString("vi-VN") || "0";

                    // Nếu có ảnh thì lấy từ avatar, nếu không thì placeholder
                    const imageUrl = customer.avatarUrl
                        ? `/images/customers/${customer.avatarUrl}`
                        : `https://via.placeholder.com/48?text=${encodeURIComponent(name.substring(0, 2).toUpperCase())}`;

                    const li = `
                    <li class="d-flex justify-content-between align-items-center py-2 border-bottom">
                        <div class="d-flex align-items-center gap-2">
                            <img src="${imageUrl}" class="rounded-circle" style="width:48px;height:48px;">
                            <div>
                                <div class="fw-semibold">${name}</div>
                                <small class="text-muted">${email}</small>
                            </div>
                        </div>
                        <div>${totalOrders} Đơn</div>
                    </li>
                `;
                    container.append(li);
                });
            },
            error: function () {
                $("#loyalCustomersList").html("<li class='text-danger py-2'>Không thể tải danh sách khách hàng thân thiết.</li>");
            }
        });
    }

    loadLoyalCustomers();
    loadBestSellingProducts();
});
