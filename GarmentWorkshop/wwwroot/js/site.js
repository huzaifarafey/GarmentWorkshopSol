// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Mobile sidebar toggle (UI only)
(function () {
    var sidebar = document.getElementById('gwSidebar');
    var toggle = document.getElementById('gwSidebarToggle');
    var backdrop = document.getElementById('gwSidebarBackdrop');

    function closeSidebar() {
        if (sidebar) sidebar.classList.remove('show');
        if (backdrop) backdrop.classList.remove('show');
    }

    if (toggle && sidebar && backdrop) {
        toggle.addEventListener('click', function () {
            sidebar.classList.toggle('show');
            backdrop.classList.toggle('show');
        });
        backdrop.addEventListener('click', closeSidebar);
    }
})();

// Bootstrap tooltips for icon-only action buttons (UI only)
(function () {
    if (typeof bootstrap === 'undefined' || !bootstrap.Tooltip) return;
    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(function (el) {
        new bootstrap.Tooltip(el);
    });
})();

// DataTables — search, sort & pagination on every list table (UI only)
(function () {
    if (typeof jQuery === 'undefined' || typeof jQuery.fn.DataTable === 'undefined') return;

    jQuery(function ($) {
        $('.gw-datatable').each(function () {
            $(this).DataTable({
                pageLength: 10,
                lengthMenu: [5, 10, 25, 50, 100],
                language: {
                    search: "_INPUT_",
                    searchPlaceholder: "Search...",
                    lengthMenu: "Show _MENU_ entries",
                    info: "Showing _START_ to _END_ of _TOTAL_ entries",
                    infoEmpty: "No entries to show",
                    zeroRecords: "No matching records found",
                    paginate: { previous: "Prev", next: "Next" }
                },
                columnDefs: [
                    { orderable: false, targets: -1 }
                ]
            });
        });

        // Smaller dashboard summary tables: sortable, no search/paging clutter
        $('.gw-datatable-mini').each(function () {
            $(this).DataTable({
                paging: false,
                searching: false,
                info: false,
                ordering: true
            });
        });
    });
})();
