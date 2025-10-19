document.addEventListener("DOMContentLoaded", () => {
    const searchBtn = document.getElementById("searchBtn");

    if (searchBtn) {
        searchBtn.addEventListener("click", () => {
            const startDate = document.getElementById("startDate").value;
            const endDate = document.getElementById("endDate").value;
            const startTime = document.getElementById("startTime").value;
            const endTime = document.getElementById("endTime").value;

            console.log(`Searching from ${startDate} ${startTime} to ${endDate} ${endTime}`);
            // TODO: Thêm logic lọc dữ liệu thật ở đây
        });
    }
});
