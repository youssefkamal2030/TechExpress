// Function to update cart count
function updateCartCount() {
    fetch('/Cart/GetCartCount')
        .then(response => response.text())
        .then(count => {
            document.querySelectorAll('.cart-count').forEach(element => {
                element.textContent = count;
            });
        })
        .catch(error => console.error('Error updating cart count:', error));
}

// Function to handle add to cart
function addToCart(productId, quantity = 1) {
    const formData = new FormData();
    formData.append('id', productId);
    formData.append('quantity', quantity);

    fetch('/Cart/AddToCart', {
        method: 'POST',
        body: formData
    })
    .then(response => {
        if (response.ok) {
            // Update cart count
            updateCartCount();
            
            // Show success message
            const toast = new bootstrap.Toast(document.getElementById('cartToast'));
            toast.show();
        } else {
            throw new Error('Failed to add item to cart');
        }
    })
    .catch(error => {
        console.error('Error adding to cart:', error);
        alert('Failed to add item to cart. Please try again.');
    });
}

// Add event listeners when document is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Add click handlers to all add-to-cart buttons
    document.querySelectorAll('form[action="/Cart/AddToCart"]').forEach(form => {
        form.addEventListener('submit', function(e) {
            e.preventDefault();
            const productId = this.querySelector('input[name="id"]').value;
            const quantity = this.querySelector('input[name="quantity"]').value;
            addToCart(productId, quantity);
        });
    });
}); 