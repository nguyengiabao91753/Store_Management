document.addEventListener('DOMContentLoaded', function () {
    const cartList = document.getElementById('cartList');
    const subtotalElem = document.getElementById('subtotal');
    const taxElem = document.getElementById('tax');
    const totalElem = document.getElementById('total');
    let cart = [];

    const productsGrid = document.getElementById('productsGrid');
    const categoryBar = document.getElementById('categoryBar');
    const searchInput = document.getElementById('searchInput');
    const searchBtn = document.getElementById('searchBtn');

    let currentCategory = '';
    let currentQuery = searchInput ? searchInput.value : '';

    async function loadProducts() {
        const params = new URLSearchParams();
        if (currentCategory) params.append('categoryId', currentCategory);
        if (currentQuery) params.append('q', currentQuery);

        const res = await fetch(`/Home/GetProducts?${params.toString()}`, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });
        const html = await res.text();
        productsGrid.innerHTML = html;
        wireAddToCartButtons();
    }

    categoryBar.addEventListener('click', (e) => {
        const btn = e.target.closest('[data-category]');
        if (!btn) return;
        categoryBar.querySelectorAll('[data-category]').forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
        currentCategory = btn.dataset.category || '';
        loadProducts();
    });

    searchBtn.addEventListener('click', () => {
        currentQuery = searchInput.value.trim();
        loadProducts();
    });

    searchInput.addEventListener('keydown', (e) => {
        if (e.key === 'Enter') {
            currentQuery = searchInput.value.trim();
            loadProducts();
        }
        if (e.key === 'Escape') {
            searchInput.value = '';
            if (currentQuery !== '') {
                currentQuery = '';
                loadProducts();
            }
        }
    });

    searchInput.addEventListener('input', () => {
        const val = searchInput.value.trim();
        if (val === '' && currentQuery !== '') {
            currentQuery = '';
            loadProducts();
        }
    });

    function wireAddToCartButtons() {
        document.querySelectorAll('.add-to-cart-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                const id = btn.dataset.id;
                const name = btn.dataset.name;
                const price = parseFloat(btn.dataset.price || '0');
                const image = btn.dataset.image;

                const existing = cart.find(p => p.id === id);
                if (existing) existing.qty++;
                else cart.push({ id, name, price, image, qty: 1 });

                renderCart();
            });
        });
    }

    function renderCart() {
        if (cart.length === 0) {
            cartList.innerHTML = `<div class="text-muted">No item selected</div>`;
        } else {
            cartList.innerHTML = cart.map(p => `
            <div class="d-flex justify-content-between align-items-center mb-2 border-bottom pb-2">
                <div class="d-flex align-items-center gap-2">
                    <img src="${p.image}" alt="${p.name}" style="width:40px;height:40px;object-fit:cover;border-radius:4px;">
                    <div>
                        <strong>${p.name}</strong><br/>
                        <small>$${p.price.toFixed(2)}</small>
                    </div>
                </div>
                <div class="d-flex align-items-center gap-2">
                    <button class="btn btn-sm btn-outline-secondary qty-minus" data-id="${p.id}">−</button>
                    <span>${p.qty}</span>
                    <button class="btn btn-sm btn-outline-secondary qty-plus" data-id="${p.id}">+</button>
                    <div class="fw-bold ms-2">$${(p.qty * p.price).toFixed(2)}</div>
                </div>
            </div>
        `).join('');
        }

        const subtotal = cart.reduce((sum, p) => sum + p.qty * p.price, 0);
        const tax = subtotal * 0.1;
        const total = subtotal + tax;

        subtotalElem.textContent = `$${subtotal.toFixed(2)}`;
        taxElem.textContent = `$${tax.toFixed(2)}`;
        totalElem.textContent = `$${total.toFixed(2)}`;

        wireQuantityButtons(); // thêm dòng này
    }

    document.getElementById('clearCartBtn').addEventListener('click', () => {
        cart = [];
        renderCart();
    });

    wireAddToCartButtons();


    function wireQuantityButtons() {
        document.querySelectorAll('.qty-plus').forEach(btn => {
            btn.addEventListener('click', () => {
                const id = btn.dataset.id;
                const item = cart.find(p => p.id === id);
                if (item) item.qty++;
                renderCart();
            });
        });

        document.querySelectorAll('.qty-minus').forEach(btn => {
            btn.addEventListener('click', () => {
                const id = btn.dataset.id;
                const item = cart.find(p => p.id === id);
                if (item) {
                    item.qty--;
                    if (item.qty <= 0) cart = cart.filter(x => x.id !== id);
                }
                renderCart();
            });
        });
    }



    document.getElementById('placeOrderBtn').addEventListener('click', async () => {
        if (cart.length === 0) {
            alert("Your cart is empty!");
            return;
        }

        const products = cart.map(item => ({
            productId: item.id,
            productName: item.name,
            productImage: item.image,
            barcode: item.barcode,
            price: item.price,
            unit: item.unit,
            quantity: item.qty
        }));

        const data = JSON.stringify(products);
        const encoded = toBase64Unicode(data);
        window.location.href = `/Checkout?data=${encoded}`;

       //fetch('/Checkout', {
       //     method: 'POST',
       //     headers: {
       //         'Content-Type': 'application/json'
       //     },
       //     body: JSON.stringify(products)
       //}).then(() => window.location.href = '/Checkout');           ;

        //if (response.ok) {
        //    const html = await response.text();
        //    document.open();
        //    document.write(html);
        //    document.close();
        //} else {
        //    alert("Failed to open checkout page!");
        //}
    });

    function toBase64Unicode(str) {
        return btoa(unescape(encodeURIComponent(str)));
    }

    function fromBase64Unicode(str) {
        return decodeURIComponent(escape(atob(str)));
    }


});
