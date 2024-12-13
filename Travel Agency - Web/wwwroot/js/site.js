// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Profile Picture Handling
function previewImage(input) {
    if (input.files && input.files[0]) {
        const file = input.files[0];
        const errorElement = document.getElementById('profilePictureError');

        // Reset error
        errorElement.textContent = '';
        errorElement.style.display = 'none';
        input.classList.remove('is-invalid');

        // Validate file size
        const maxSize = 2 * 1024 * 1024; // 2MB
        if (file.size > maxSize) {
            errorElement.textContent = 'File size must be less than 2MB';
            errorElement.style.display = 'block';
            input.classList.add('is-invalid');
            input.value = '';
            return;
        }

        // Validate file type
        const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png'];
        if (!allowedTypes.includes(file.type)) {
            errorElement.textContent = 'Only JPG, JPEG and PNG files are allowed';
            errorElement.style.display = 'block';
            input.classList.add('is-invalid');
            input.value = '';
            return;
        }

        // Preview image
        const reader = new FileReader();
        reader.onload = function (e) {
            document.getElementById('profilePreview').src = e.target.result;
        };
        reader.readAsDataURL(file);
    }
}

function validateProfilePicture() {
    const input = document.getElementById('profilePicture');
    if (!input.files || input.files.length === 0) {
        const errorElement = document.getElementById('profilePictureError');
        errorElement.textContent = 'Please select a file';
        errorElement.style.display = 'block';
        input.classList.add('is-invalid');
        return false;
    }
    return true;
}

// Auto-hide alerts
$(document).ready(function () {
    setTimeout(function () {
        $('.alert').alert('close');
    }, 5000);
});