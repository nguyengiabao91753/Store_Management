document.addEventListener("DOMContentLoaded", () => {
    const orders = [
        {
            name: "Mike",
            table: "04",
            type: "Dine In",
            time: "10:00 AM",
            items: ["Beef Crowich", "Grains Pan Bread", "Cheezy Sourdough", "Sliced Black Forest"],
            status: "On Kitchen Hand",
            total: 4,
        },
        {
            name: "Billie",
            table: "04",
            type: "Dine In",
            time: "10:00 AM",
            items: ["Beef Crowich", "Cereal Cream Donut", "Cheezy Sourdough"],
            status: "All Done",
            total: 6,
        },
        {
            name: "Richard",
            table: "04",
            type: "Dine In",
            time: "10:00 AM",
            items: ["Beef Crowich", "Cereal Cream Donut", "Cheezy Sourdough"],
            status: "On Kitchen Hand",
            total: 6,
        },
        {
            name: "Sharon",
            table: "04",
            type: "Dine In",
            time: "10:00 AM",
            items: ["Beef Crowich", "Cereal Cream Donut", "Cheezy Sourdough"],
            status: "On Kitchen Hand",
            total: 6,
        },
        {
            name: "Daniel",
            table: "08",
            type: "Take Away",
            time: "10:30 AM",
            items: ["Avocado Toast", "Latte"],
            status: "All Done",
            total: 2,
        },
    ];

    const container = document.getElementById("trackOrderContainer");
    const btnLeft = document.getElementById("scrollLeft");
    const btnRight = document.getElementById("scrollRight");

    function renderOrders(list) {
        container.innerHTML = list
            .map(
                (order) => `
      <div class="track-card">
        <div class="d-flex justify-content-between align-items-center mb-1">
          <div class="name">${order.name}</div>
          <span class="status ${order.status === "All Done" ? "done" : "kitchen"
                    }">${order.status}</span>
        </div>
        <small>Table: ${order.table} · ${order.type}</small><br>
        <small>${order.time}</small>
        <ul class="mt-2 mb-1 small text-muted">
          ${order.items.slice(0, 3).map((i) => `<li>${i}</li>`).join("")}
        </ul>
        <a href="#" class="see-more">See More</a>
        <div class="mt-2 small text-muted">Total Order: ${order.total} Items</div>
      </div>
    `
            )
            .join("");

        updateScrollButtons();
    }

    function updateScrollButtons() {
        const maxScroll = container.scrollWidth - container.clientWidth;
        const scrollLeft = container.scrollLeft;

        btnLeft.disabled = scrollLeft <= 0;
        btnRight.disabled = scrollLeft >= maxScroll - 5;
    }

    container.addEventListener("scroll", updateScrollButtons);

    btnLeft.addEventListener("click", () => {
        container.scrollBy({ left: -250, behavior: "smooth" });
        setTimeout(updateScrollButtons, 400);
    });

    btnRight.addEventListener("click", () => {
        container.scrollBy({ left: 250, behavior: "smooth" });
        setTimeout(updateScrollButtons, 400);
    });

    document.getElementById("searchOrder").addEventListener("input", (e) => {
        const q = e.target.value.toLowerCase();
        const filtered = orders.filter(
            (o) => o.name.toLowerCase().includes(q) || o.table.toLowerCase().includes(q)
        );
        renderOrders(filtered);
    });

    renderOrders(orders);
});
