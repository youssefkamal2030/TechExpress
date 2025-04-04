// Cart count update
function updateCartCount() {
    fetch('/Cart/GetCount')
        .then(response => response.json())
        .then(data => {
            const cartCount = document.querySelector('.cart-count');
            if (cartCount) {
                cartCount.textContent = data.count;
                if (data.count > 0) {
                    cartCount.style.display = 'inline-block';
                } else {
                    cartCount.style.display = 'none';
                }
            }
        });
}

// Initialize cart count on page load
document.addEventListener('DOMContentLoaded', function() {
    updateCartCount();
});

// Add to cart animation
document.addEventListener('click', function(e) {
    if (e.target.closest('form[action*="AddToCart"]')) {
        const button = e.target.closest('button');
        if (button) {
            button.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Adding...';
            setTimeout(() => {
                button.innerHTML = '<i class="fas fa-shopping-cart"></i> Add to cart';
                updateCartCount();
            }, 1000);
        }
    }
});

// Price range validation
document.addEventListener('DOMContentLoaded', function() {
    const minPriceInput = document.querySelector('input[name="minPrice"]');
    const maxPriceInput = document.querySelector('input[name="maxPrice"]');

    if (minPriceInput && maxPriceInput) {
        minPriceInput.addEventListener('change', function() {
            if (this.value && maxPriceInput.value && parseFloat(this.value) > parseFloat(maxPriceInput.value)) {
                this.value = maxPriceInput.value;
            }
        });

        maxPriceInput.addEventListener('change', function() {
            if (this.value && minPriceInput.value && parseFloat(this.value) < parseFloat(minPriceInput.value)) {
                this.value = minPriceInput.value;
            }
        });
    }
});

// Smooth scroll for anchor links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        document.querySelector(this.getAttribute('href')).scrollIntoView({
            behavior: 'smooth'
        });
    });
}); 