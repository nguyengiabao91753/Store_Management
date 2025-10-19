document.addEventListener("DOMContentLoaded", () => {
    console.log("💰 Checkout Status page loaded");

    // 📦 Lấy trạng thái từ URL: ?status=success hoặc ?status=failed
    const params = new URLSearchParams(window.location.search);
    const status = params.get("status") || "success";

    const card = document.getElementById("statusCard");
    const iconDiv = card.querySelector(".status-icon");
    const title = card.querySelector("h3");
    const message = card.querySelector("p");

    if (status === "failed") {
        iconDiv.classList.remove("success");
        iconDiv.classList.add("failed");
        iconDiv.innerHTML = `<i class="fa-solid fa-circle-xmark"></i>`;
        title.textContent = "Payment Failed";
        message.textContent = "Your transaction could not be completed.";
    } else {
        iconDiv.classList.add("success");
        iconDiv.innerHTML = `<i class="fa-solid fa-circle-check"></i>`;
        title.textContent = "Payment Successful";
        message.textContent = "Your order has been completed successfully!";
    }

    // ⏳ Đếm ngược 5 giây rồi về trang home
    let countdown = 5;
    const countdownEl = document.getElementById("countdown");
    const interval = setInterval(() => {
        countdown--;
        countdownEl.textContent = countdown;
        if (countdown === 0) {
            clearInterval(interval);
            window.location.href = "/Home";
        }
    }, 1000);
});
