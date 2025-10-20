const PRODUCTS = [
    { id: 1, name: "Beef Crowich", price: 5.5, tag: "Sandwich", desc: "Delicious beef croissant." },
    { id: 2, name: "Buttermelt Croissant", price: 4.0, tag: "Pastry", desc: "Crispy butter croissant." },
    { id: 3, name: "Cereal Cream Donut", price: 2.45, tag: "Donut", desc: "Tasty cereal donut." },
    { id: 4, name: "Cheesy Cheesecake", price: 3.75, tag: "Cake", desc: "Creamy cheesecake." },
    { id: 5, name: "Solo Floss Bread", price: 4.5, tag: "Bread", desc: "Flossy bread." }
    // ... thêm các sản phẩm khác
];

const grid = document.getElementById("productsGrid");
function renderProducts(list) {
    grid.innerHTML = "";
    list.forEach((p) => {
        const col = document.createElement("div");
        col.className = "col-6 col-md-4 col-lg-3";
        col.innerHTML = `
      <div class="p-3 product-card h-100">
        <div class="text-end small text-muted">${p.tag}</div>
        <div class="text-center">
          <img src="${p.img ?? ''}" class="product-thumb mx-auto d-block" />
        </div>
        <h6 class="mt-2">${p.name}</h6>
        <div class="d-flex justify-content-between align-items-center">
          <div class="small text-muted">$${p.price.toFixed(2)}</div>
          <div>
            <button class="btn btn-sm btn-outline-primary me-1" onclick="openDetail(${p.id})"><i class="fa fa-eye"></i></button>
            <button class="btn btn-sm btn-primary" onclick="quickAdd(${p.id})"><i class="fa fa-plus"></i></button>
          </div>
        </div>
      </div>
    `;
        grid.appendChild(col);
    });
}
renderProducts(PRODUCTS);

document.getElementById("searchInput").addEventListener("input", (e) => {
    const q = e.target.value.toLowerCase();
    renderProducts(PRODUCTS.filter(p => p.name.toLowerCase().includes(q) || p.tag.toLowerCase().includes(q)));
});

let currentProduct = null;
const productModal = new bootstrap.Modal(document.getElementById("productModal"));

function openDetail(id) {
    const p = PRODUCTS.find((x) => x.id === id);
    currentProduct = p;
    document.getElementById("modalImg").src = p.img ?? "";
    document.getElementById("modalTitle").innerText = p.name;
    document.getElementById("modalDesc").innerText = p.desc;
    document.getElementById("modalPrice").innerText = "$" + p.price.toFixed(2);
    document.getElementById("qtyInput").value = 1;
    productModal.show();
}

document.getElementById("qtyMinus").addEventListener("click", () => {
    const el = document.getElementById("qtyInput");
    el.value = Math.max(1, parseInt(el.value || 1) - 1);
});
document.getElementById("qtyPlus").addEventListener("click", () => {
    const el = document.getElementById("qtyInput");
    el.value = parseInt(el.value || 1) + 1;
});

let CART = [];

function quickAdd(id) {
    const p = PRODUCTS.find((x) => x.id === id);
    addToCart(p, 1);
}

function addToCart(product, qty) {
    const ex = CART.find((i) => i.id === product.id);
    if (ex) {
        ex.qty += qty;
    } else CART.push({ ...product, qty });
    updateCartUI();
}

document.getElementById("addToCartBtn").addEventListener("click", () => {
    const qty = Math.max(1, parseInt(document.getElementById("qtyInput").value || 1));
    addToCart(currentProduct, qty);
    productModal.hide();
});

function updateCartUI() {
    const cartList = document.getElementById("cartList");
    cartList.innerHTML = "";
    if (CART.length === 0) {
        cartList.innerHTML = '<div class="text-muted">No item selected</div>';
    } else {
        CART.forEach((item) => {
            const div = document.createElement("div");
            div.className = "cart-item d-flex align-items-center";
            div.innerHTML = `
        <img src="${item.img ?? ''}" alt="" style="height:54px;width:70px;object-fit:cover;border-radius:8px;" class="me-3">
        <div class="flex-grow-1">
          <div class="fw-semibold">${item.name}</div>
          <div class="small text-muted">$${item.price.toFixed(2)} · <span class="small-pill">${item.tag}</span></div>
        </div>
        <div class="text-end">
          <div class="d-flex align-items-center gap-1">
            <button class="btn btn-sm btn-outline-secondary" onclick="changeQty(${item.id}, -1)">-</button>
            <div class="px-2">${item.qty}</div>
            <button class="btn btn-sm btn-outline-secondary" onclick="changeQty(${item.id}, 1)">+</button>
          </div>
          <div class="small text-muted mt-1">$${(item.price * item.qty).toFixed(2)}</div>
        </div>
      `;
            cartList.appendChild(div);
        });
    }

    const subtotal = CART.reduce((s, i) => s + i.price * i.qty, 0);
    const tax = subtotal * 0.1;
    const total = subtotal + tax;
    document.getElementById("subtotal").innerText = "$" + subtotal.toFixed(2);
    document.getElementById("tax").innerText = "$" + tax.toFixed(2);
    document.getElementById("total").innerText = "$" + total.toFixed(2);
}

function changeQty(id, delta) {
    const item = CART.find((i) => i.id === id);
    if (!item) return;
    item.qty = Math.max(0, item.qty + delta);
    if (item.qty === 0) CART = CART.filter((i) => i.id !== id);
    updateCartUI();
}

document.getElementById("clearCartBtn").addEventListener("click", () => {
    CART = [];
    updateCartUI();
});

document.getElementById("placeOrderBtn").addEventListener("click", () => {
    if (CART.length === 0) {
        alert("Cart trống!");
        return;
    }
    alert("Place Order (demo): " + CART.length + " items.");
    CART = [];
    updateCartUI();
    sessionStorage.setItem("CART", JSON.stringify(CART));
    window.location.href = "/Checkout";
});

if ("scrollRestoration" in history) {
    history.scrollRestoration = "manual";
}
window.scrollTo(0, 0);

updateCartUI();
