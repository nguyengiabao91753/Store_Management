document.addEventListener("DOMContentLoaded", () => {
	const searchBtn = document.getElementById("searchBtn");
	const tbody = document.getElementById("orderTableBody");

	function formatDateTime(iso) {
		if (!iso) return "-";
		const d = new Date(iso);
		if (isNaN(d.getTime())) return "-";
		const pad = (n) => (n < 10 ? "0" + n : n);
		let hours = d.getHours();
		const minutes = pad(d.getMinutes());
		const ampm = hours >= 12 ? "PM" : "AM";
		hours = hours % 12;
		hours = hours ? hours : 12;
		const day = pad(d.getDate());
		const month = pad(d.getMonth() + 1);
		const year = d.getFullYear();
		return `${day}/${month}/${year} - ${pad(hours)}:${minutes} ${ampm}`;
	}

	function renderRows(rows) {
		if (!rows || rows.length === 0) {
			tbody.innerHTML = `<tr><td colspan="7" class="text-center text-muted py-4">No orders found</td></tr>`;
			return;
		}

		const html = rows.map(o => {
			const statusClass = o.orderStatus === "Done" ? "done" : (o.orderStatus === "Canceled" ? "canceled" : "pending");
			const paidBadge = o.paymentStatus === "Paid"
				? `<span class="badge-status paid">Paid</span>`
				: `<span class="badge-status unpaid">Unpaid</span>`;

			return `<tr>
				<td>${o.orderId}</td>
				<td>${formatDateTime(o.orderDate)}</td>
				<td>${o.customerName ?? "-"}</td>
				<td><span class="status ${statusClass}">${o.orderStatus}</span></td>
				<td>USD ${Number(o.totalPayment ?? 0).toFixed(2)}</td>
				<td>${paidBadge}</td>
				<td><a href="/OrderHistory/Details/${o.orderId}" class="text-primary text-decoration-none">Detail</a></td>
			</tr>`;
		}).join("");

		tbody.innerHTML = html;
	}

	async function fetchFiltered() {
		const startDate = document.getElementById("startDate").value;
		const endDate = document.getElementById("endDate").value;
		const startTime = document.getElementById("startTime").value;
		const endTime = document.getElementById("endTime").value;

		const params = new URLSearchParams();
		if (startDate) params.append("startDate", startDate);
		if (endDate) params.append("endDate", endDate);
		if (startTime) params.append("startTime", startTime);
		if (endTime) params.append("endTime", endTime);

		const url = `/OrderHistory/GetOrders?${params.toString()}`;
		try {
			const res = await fetch(url, { headers: { "Accept": "application/json" } });
			if (!res.ok) throw new Error(`HTTP ${res.status}`);
			const rows = await res.json();
			renderRows(rows);
		} catch (e) {
			console.error("Failed to fetch filtered orders", e);
		}
	}

	if (searchBtn) {
		searchBtn.addEventListener("click", () => {
			fetchFiltered();
		});
	}
});
