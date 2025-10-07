$(function () {
    $("#example1, #example2").DataTable({
        "responsive": true,
        "lengthChange": false,
        "autoWidth": true,
        "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"]
    }).buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');
});

function confirmDelete() {
    return confirm('Are you sure you want to delete this?');
}

function confirmBlock() {
    return confirm('Are you sure?');
}