document.addEventListener("DOMContentLoaded", () => {
    console.log("💳 Checkout page loaded");

    // 🧩 Render danh sách đơn hàng
    renderOrderItems();

    // 🎯 Nút xác nhận thanh toán
    const confirmBtn = document.getElementById("confirmPayment");
    if (confirmBtn) {
        confirmBtn.addEventListener("click", () => {
            window.location.href = "/CheckoutStatus?status=success";
        });
    }

    // 🎯 Xử lý nút back (cancel)
    const backBtn = document.getElementById("headerBackBtn");
    if (backBtn) {
        backBtn.addEventListener("click", showCancelConfirm);
    }

    updateHeaderButton();
});

// 📦 Dữ liệu mẫu
const ORDER_ITEMS = [
    { id: 1, name: "Mexican tacos", addons: "7 delicious add ons", qty: 2, price: 12.77, img: "https://via.placeholder.com/64x64?text=Tacos" },
    { id: 2, name: "Submarine sandwich", addons: "3 delicious add ons", qty: 2, price: 19.46, img: "https://via.placeholder.com/64x64?text=Sandwich" },
    { id: 3, name: "Garlic toast", addons: "2 delicious add ons", qty: 2, price: 8.69, img: "https://via.placeholder.com/64x64?text=Toast" }
];

// 🧮 Hiển thị danh sách đơn hàng
function renderOrderItems() {
    const container = document.getElementById("order-list");
    if (!container) return;

    if (!ORDER_ITEMS.length) {
        container.innerHTML = `<p class="text-muted">No item selected</p>`;
        return;
    }

    container.innerHTML = ORDER_ITEMS.map(item => `
    <div class="d-flex align-items-center justify-content-between border-bottom py-2">
      <div class="d-flex align-items-center">
        <img src="${item.img}" alt="${item.name}" class="rounded me-3" width="56" height="56">
        <div>
          <div class="fw-semibold">${item.name}</div>
          <div class="small text-muted">${item.addons}</div>
        </div>
      </div>
      <div class="text-end">
        <div class="small text-muted">x${item.qty}</div>
        <div class="fw-semibold">$${item.price.toFixed(2)}</div>
      </div>
    </div>
  `).join("");

    const subtotal = ORDER_ITEMS.reduce((sum, i) => sum + i.price, 0);
    document.getElementById("subtotal").textContent = `$${subtotal.toFixed(2)}`;
}

// 💬 Xác nhận hủy giao dịch
function showCancelConfirm() {
    const overlay = document.getElementById("cancelConfirm");
    if (!overlay) return;
    overlay.classList.remove("d-none");

    const yesBtn = document.getElementById("confirmYes");
    const noBtn = document.getElementById("confirmNo");

    yesBtn.onclick = () => {
        overlay.classList.add("d-none");
        window.location.href = "/CheckoutStatus?status=failed";
    };
    noBtn.onclick = () => overlay.classList.add("d-none");
}

function updateHeaderButton() {
    const headerMenuBtn = document.getElementById("headerMenuBtn");
    if (!headerMenuBtn) return;

    const currentPage = window.location.pathname.toLowerCase();

    // nếu URL có /checkout (hoặc /payment/checkout)
    if (currentPage.includes("/checkout")) {
        headerMenuBtn.id = "headerBackBtn";
        headerMenuBtn.innerHTML = `<i class="fa fa-arrow-left"></i>`;
        headerMenuBtn.removeAttribute("data-bs-toggle");
        headerMenuBtn.removeAttribute("data-bs-target");
        headerMenuBtn.onclick = showCancelConfirm;
    } else {
        headerMenuBtn.id = "headerMenuBtn";
        headerMenuBtn.innerHTML = `<i class="fa fa-bars"></i>`;
        headerMenuBtn.setAttribute("data-bs-toggle", "offcanvas");
        headerMenuBtn.setAttribute("data-bs-target", "#menuSidebar");
        headerMenuBtn.onclick = null;
    }
}
