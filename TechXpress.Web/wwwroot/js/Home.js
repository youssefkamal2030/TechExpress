// TechXpress Main JavaScript File

document.addEventListener('DOMContentLoaded', function () {
    // Initialize Swiper for testimonials
    const testimonialSwiper = new Swiper('.swiper-container.text-slider', {
        autoplay: {
            delay: 5000,
            disableOnInteraction: false
        },
        loop: true,
        navigation: {
            nextEl: '.swiper-button-next',
            prevEl: '.swiper-button-prev',
        },
        spaceBetween: 30,
        slidesPerView: 1,
        breakpoints: {
            // when window width is >= 768px
            768: {
                slidesPerView: 1,
                spaceBetween: 30
            },
        }
    });

    // Smooth scrolling for navigation links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();

            const targetId = this.getAttribute('href');
            if (targetId === '#') return;

            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                window.scrollTo({
                    top: targetElement.offsetTop - 70,
                    behavior: 'smooth'
                });
            }
        });
    });

    // Form validation and handling
    const newsletterForm = document.getElementById('newsletterForm');
    if (newsletterForm) {
        newsletterForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const emailInput = document.getElementById('nemail');
            const email = emailInput.value.trim();

            if (!isValidEmail(email)) {
                showError(emailInput, 'Please enter a valid email address');
                return;
            }

            // Simulate form submission
            const submitButton = this.querySelector('.form-control-submit-button');
            submitButton.disabled = true;
            submitButton.textContent = 'SUBSCRIBING...';

            // Simulate API call with timeout
            setTimeout(function () {
                submitButton.textContent = 'SUBSCRIBED!';
                submitButton.style.backgroundColor = '#28a745';

                // Reset form after successful submission
                setTimeout(function () {
                    newsletterForm.reset();
                    submitButton.disabled = false;
                    submitButton.textContent = 'SUBSCRIBE';
                    submitButton.style.backgroundColor = '';
                }, 2000);
            }, 1500);
        });
    }

    // Email validation helper
    function isValidEmail(email) {
        const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);
    }

    // Show error message
    function showError(inputElement, message) {
        const formGroup = inputElement.closest('.form-group');
        const errorElement = formGroup.querySelector('.help-block');

        if (errorElement) {
            errorElement.textContent = message;
            errorElement.style.color = '#dc3545';
            inputElement.style.borderColor = '#dc3545';

            setTimeout(function () {
                errorElement.textContent = '';
                inputElement.style.borderColor = '';
            }, 3000);
        }
    }

    // Animate elements on scroll
    const animateOnScroll = function () {
        const elements = document.querySelectorAll('.card, .h2-heading, .text-wrapper, .form');

        elements.forEach(element => {
            const position = element.getBoundingClientRect();
            const windowHeight = window.innerHeight;

            // If element is in viewport
            if (position.top < windowHeight * 0.8) {
                element.classList.add('fade-in');
            }
        });
    };

    // Add animation class
    const styleSheet = document.createElement('style');
    styleSheet.textContent = `
    .fade-in {
      animation: fadeIn 0.8s ease-in-out forwards;
    }
    
    @keyframes fadeIn {
      from {
        opacity: 0;
        transform: translateY(30px);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }
    
    .card, .h2-heading, .text-wrapper, .form {
      opacity: 0;
    }
  `;
    document.head.appendChild(styleSheet);

    // Run animation on scroll
    window.addEventListener('scroll', animateOnScroll);

    // Run once on page load
    animateOnScroll();

    // Product image hover effect
    const productImages = document.querySelectorAll('.cards-2 .card img');
    productImages.forEach(img => {
        img.addEventListener('mouseenter', function () {
            this.style.transform = 'scale(1.1)';
        });

        img.addEventListener('mouseleave', function () {
            this.style.transform = 'scale(1)';
        });
    });

    // Header animation
    const header = document.querySelector('.header');
    if (header) {
        setTimeout(() => {
            header.classList.add('loaded');
        }, 300);
    }

    // Add this style for header animation
    const headerStyle = document.createElement('style');
    headerStyle.textContent = `
    .header {
      opacity: 0;
      transform: translateY(-20px);
      transition: opacity 1s ease, transform 1s ease;
    }
    
    .header.loaded {
      opacity: 1;
      transform: translateY(0);
    }
  `;
    document.head.appendChild(headerStyle);
});