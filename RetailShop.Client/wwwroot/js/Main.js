// =========================
// 📌 MAIN SCRIPT FOR POS UI (ASP.NET Core Edition)
// =========================

// 1️⃣ DOM READY ENTRYPOINT
document.addEventListener('DOMContentLoaded', () => {
    // Header & Sidebar đã được render sẵn qua Partial, không cần fetch
    initOpenCloseButton(); // xử lý nút mở/đóng order
    wireSidebarCloseButton(); // xử lý nút đóng sidebar

    // Bắt đầu cập nhật đồng hồ trên header
    updateHeaderClock();
    setInterval(updateHeaderClock, 1000);

    // Khởi tạo các tính năng còn lại trên trang
    initTrackOrderFixed();
});

// =========================
// 3️⃣ SIDEBAR BEHAVIORS
// =========================

// Nút đóng sidebar (nếu có)
function wireSidebarCloseButton() {
    const closeBtn = document.getElementById('sidebarCloseBtn');
    const appSide = document.getElementById('appSidebar');
    if (closeBtn && appSide) {
        closeBtn.addEventListener('click', () => appSide.classList.remove('show'));
    }
}

// =========================
// 4️⃣ HEADER CLOCK
// =========================
function updateHeaderClock() {
    const dateEl = document.getElementById('headerDate');
    const timeEl = document.getElementById('headerTime');
    if (!dateEl || !timeEl) return;

    const now = new Date();
    const optsDate = {
        weekday: 'short',
        day: 'numeric',
        month: 'short',
        year: 'numeric'
    };
    dateEl.textContent = now.toLocaleDateString('en-US', optsDate);
    timeEl.textContent = now.toLocaleTimeString('en-US', {
        hour: '2-digit',
        minute: '2-digit'
    });
}

// =========================
// 5️⃣ OPEN/CLOSE ORDER BUTTON
// =========================
function initOpenCloseButton() {
    const wrapper = document.querySelector('.open-close-store');
    const toggleBtn = document.getElementById('toggle-btn');
    const statusText = document.getElementById('status-text');
    if (!wrapper || !toggleBtn || !statusText) return;

    toggleBtn.addEventListener('click', () => {
        const isClosed = wrapper.classList.toggle('closed');
        if (isClosed) {
            statusText.textContent = 'Close Order';
            statusText.style.color = '#dc3545'; // đỏ
            toggleBtn.classList.remove('btn-outline-success');
            toggleBtn.classList.add('btn-outline-danger');
        } else {
            statusText.textContent = 'Open Order';
            statusText.style.color = '#198754'; // xanh
            toggleBtn.classList.remove('btn-outline-danger');
            toggleBtn.classList.add('btn-outline-success');
        }
    });
}

// =========================
// 6️⃣ TRACK ORDER PANEL
// =========================
function initTrackOrderFixed() {
    const panelRoot = document.getElementById('trackOrderFixed');
    const toggleBtn = document.getElementById('trackToggleBtn');
    const closeBtn = document.getElementById('trackCloseBtn');
    if (!panelRoot || !toggleBtn) return;
    const trackPanel = panelRoot.querySelector('.track-panel');
    const scrollArea = panelRoot.querySelector('.track-content');

    function positionPanel() {
        let mainCol =
            document.querySelector('.container-fluid .row > .col-lg-9') ||
            document.querySelector('.container-fluid .row > .col-md-9') ||
            document.querySelector('.container-fluid');
        if (!mainCol) return;
        const rect = mainCol.getBoundingClientRect();
        const left = Math.max(12, rect.left + 12);
        const width = Math.max(320, rect.width - 24);
        panelRoot.style.left = left + 'px';
        trackPanel.style.width = width + 'px';
    }

    function openPanel() {
        panelRoot.classList.add('open');
        panelRoot.setAttribute('aria-hidden', 'false');
    }
    function closePanel() {
        panelRoot.classList.remove('open');
        panelRoot.setAttribute('aria-hidden', 'true');
    }
    function togglePanel() {
        panelRoot.classList.toggle('open');
    }

    toggleBtn.addEventListener('click', (e) => {
        e.stopPropagation();
        togglePanel();
    });
    if (closeBtn)
        closeBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            closePanel();
        });

    document.addEventListener('click', (e) => {
        if (!panelRoot.classList.contains('open')) return;
        if (panelRoot.contains(e.target) || toggleBtn.contains(e.target)) return;
        closePanel();
    });

    window.addEventListener('resize', positionPanel);
    window.addEventListener('load', () => setTimeout(positionPanel, 100));
    positionPanel();

    // drag-to-scroll support
    let isDown = false,
        startX = 0,
        scrollLeft = 0;
    scrollArea.addEventListener('mousedown', (e) => {
        isDown = true;
        scrollArea.classList.add('dragging');
        startX = e.pageX - scrollArea.offsetLeft;
        scrollLeft = scrollArea.scrollLeft;
    });
    scrollArea.addEventListener('mouseleave', () => {
        isDown = false;
        scrollArea.classList.remove('dragging');
    });
    scrollArea.addEventListener('mouseup', () => {
        isDown = false;
        scrollArea.classList.remove('dragging');
    });
    scrollArea.addEventListener('mousemove', (e) => {
        if (!isDown) return;
        e.preventDefault();
        const x = e.pageX - scrollArea.offsetLeft;
        const walk = (x - startX) * 1.2;
        scrollArea.scrollLeft = scrollLeft - walk;
    });
}
