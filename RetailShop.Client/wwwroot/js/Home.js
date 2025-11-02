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
                <div class="d-flex justify-content-between align-items-center mb-2">
                    <div>
                        <strong>${p.name}</strong><br/>
                        <small>${p.qty} × $${p.price.toFixed(2)}</small>
                    </div>
                    <div class="fw-bold">$${(p.qty * p.price).toFixed(2)}</div>
                </div>
            `).join('');
        }

        const subtotal = cart.reduce((sum, p) => sum + p.qty * p.price, 0);
        const tax = subtotal * 0.1;
        const total = subtotal + tax;

        subtotalElem.textContent = `$${subtotal.toFixed(2)}`;
        taxElem.textContent = `$${tax.toFixed(2)}`;
        totalElem.textContent = `$${total.toFixed(2)}`;
    }

    document.getElementById('clearCartBtn').addEventListener('click', () => {
        cart = [];
        renderCart();
    });

    wireAddToCartButtons();
});
