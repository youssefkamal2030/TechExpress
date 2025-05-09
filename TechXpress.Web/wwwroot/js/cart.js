/**
 * TechXpress Cart Functionality
 */

document.addEventListener('DOMContentLoaded', function() {
    // Initialize the CSRF token for AJAX requests
    initializeCSRFToken();
    
    // Set up event listeners
    setupEventListeners();
});

// Initialize CSRF token for AJAX requests
function initializeCSRFToken() {
    window.csrfToken = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
}

// Set up event listeners for cart-related elements
function setupEventListeners() {
    // Example event listener for "Add to Cart" buttons that aren't using the onclick attribute
    const addToCartButtons = document.querySelectorAll('.add-to-cart-btn');
    addToCartButtons.forEach(button => {
        button.addEventListener('click', function() {
            const productId = this.getAttribute('data-product-id');
            const quantity = parseInt(this.getAttribute('data-quantity') || '1');
            addToCart(productId, quantity);
        });
    });
}

/**
 * Add a product to the cart
 * @param {number} productId - The ID of the product to add
 * @param {number} quantity - The quantity to add
 * @returns {Promise} - Promise that resolves when the product is added
 */
function addToCart(productId, quantity) {
    // Show loading state if needed
    const loadingToast = new bootstrap.Toast(document.getElementById('loadingToast'));
    loadingToast?.show();
    
    return fetch('/Cart/AddToCart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': window.csrfToken || document.querySelector('input[name="__RequestVerificationToken"]')?.value
        },
        body: JSON.stringify({
            productId: productId,
            quantity: quantity
        })
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        // Hide loading toast if shown
        loadingToast?.hide();
        
        // Show success message
        const successToast = new bootstrap.Toast(document.getElementById('cartToast'));
        successToast?.show();
        
        // Update cart count in UI
        updateCartCount(data.cartItemCount || 0);
        
        return data;
    })
    .catch(error => {
        console.error('Error adding item to cart:', error);
        
        // Hide loading toast if shown
        loadingToast?.hide();
        
        // Show error message
        const errorToast = new bootstrap.Toast(document.getElementById('errorToast'));
        errorToast?.show();
    });
}

/**
 * Update the cart count displayed in the UI
 * @param {number} count - The new cart count
 */
function updateCartCount(count) {
    const cartCountElements = document.querySelectorAll('.cart-count');
    cartCountElements.forEach(element => {
        element.textContent = count;
    });
}

/**
 * Remove an item from the cart
 * @param {number} cartItemId - The ID of the cart item to remove
 */
function removeFromCart(cartItemId) {
    return fetch('/Cart/RemoveFromCart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': window.csrfToken || document.querySelector('input[name="__RequestVerificationToken"]')?.value
        },
        body: JSON.stringify({
            cartItemId: cartItemId
        })
    })
    .then(response => response.json())
    .then(data => {
        // Update cart UI
        updateCartCount(data.cartItemCount || 0);
        
        // If we're on the cart page, refresh or update the UI
        if (window.location.pathname.includes('/Cart')) {
            window.location.reload();
        }
        
        return data;
    })
    .catch(error => console.error('Error removing item from cart:', error));
}

/**
 * Update the quantity of an item in the cart
 * @param {number} cartItemId - The ID of the cart item to update
 * @param {number} quantity - The new quantity
 */
function updateCartItemQuantity(cartItemId, quantity) {
    if (quantity < 1) {
        return Promise.reject(new Error('Quantity cannot be less than 1'));
    }
    
    return fetch('/Cart/UpdateQuantity', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': window.csrfToken || document.querySelector('input[name="__RequestVerificationToken"]')?.value
        },
        body: JSON.stringify({
            cartItemId: cartItemId,
            quantity: quantity
        })
    })
    .then(response => response.json())
    .then(data => {
        // If we're on the cart page, update the UI
        if (window.location.pathname.includes('/Cart')) {
            // Update item subtotal
            const itemSubtotal = document.querySelector(`.cart-item-subtotal[data-cart-item-id="${cartItemId}"]`);
            if (itemSubtotal) {
                itemSubtotal.textContent = `$${data.subtotal.toFixed(2)}`;
            }
            
            // Update cart total
            const cartTotal = document.querySelector('.cart-total');
            if (cartTotal) {
                cartTotal.textContent = `$${data.cartTotal.toFixed(2)}`;
            }
        }
        
        return data;
    })
    .catch(error => console.error('Error updating cart item quantity:', error));
} 