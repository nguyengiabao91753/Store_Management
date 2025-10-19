document.addEventListener("DOMContentLoaded", () => {
    console.log("📊 Report page loaded");

    // Nút Download — ví dụ xử lý xuất báo cáo
    const btnDownload = document.getElementById("btnDownloadReport");
    if (btnDownload) {
        btnDownload.addEventListener("click", () => {
            alert("Downloading report as PDF...");
            // TODO: sau này gọi API xuất file hoặc window.open('/Report/ExportPdf')
        });
    }
});
