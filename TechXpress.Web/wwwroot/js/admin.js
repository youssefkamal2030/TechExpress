// Initialize DataTables
$(document).ready(function() {
    $('.datatable').DataTable({
        responsive: true,
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        "order": []
    });

    // Product image preview
    $('#ImageUrl').on('change', function() {
        const input = this;
        if (input.value) {
            $('#imagePreview').attr('src', input.value).removeClass('d-none');
        } else {
            $('#imagePreview').addClass('d-none');
        }
    });

    // Confirm delete
    $('.delete-confirm').on('click', function(e) {
        if (!confirm('Are you sure you want to delete this item? This action cannot be undone.')) {
            e.preventDefault();
        }
    });
    
    // Chart.js initialization (if available)
    if (typeof Chart !== 'undefined' && document.getElementById('salesChart')) {
        const ctx = document.getElementById('salesChart').getContext('2d');
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: chartData.labels,
                datasets: [{
                    label: 'Sales',
                    data: chartData.data,
                    backgroundColor: 'rgba(52, 152, 219, 0.2)',
                    borderColor: 'rgba(52, 152, 219, 1)',
                    borderWidth: 2,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }
}); 