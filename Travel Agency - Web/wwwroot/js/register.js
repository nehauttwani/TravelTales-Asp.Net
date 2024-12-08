function validatePersonalInfo() {
    var isValid = true;
    $('#personal input[required]').each(function () {
        validateField($(this));
        if ($(this).hasClass('is-invalid')) {
            isValid = false;
        }
    });
    return isValid;
}

    function nextTab() {
        if (validatePersonalInfo()) {
        $('#account-tab').tab('show');
        } else {
        alert('Please fill in all required fields before proceeding.');
        }
    }

    function previousTab() {
        $('#personal-tab').tab('show');
    }

    $(document).ready(function () {
        $('#registerForm').submit(function (e) {
            if (!$(this).valid()) {
                e.preventDefault();
                $('#personal-tab').tab('show');
            }
        });

    $('#account-tab').on('show.bs.tab', function (e) {
            // Save form data to localStorage when switching to account tab
            var formData = $('#registerForm').serializeArray();
    localStorage.setItem('registerFormData', JSON.stringify(formData));
        });

    $('#personal-tab').on('show.bs.tab', function (e) {
            // Restore form data from localStorage when switching back to personal tab
            var formData = JSON.parse(localStorage.getItem('registerFormData'));
    if (formData) {
        formData.forEach(function (field) {
            $('[name="' + field.name + '"]').val(field.value);
        });
            }
        });
    });

function formatPhoneNumber(input) {
    // Removing all non-digit characters
    var number = input.value.replace(/\D/g, '');

    // Formatting the number
    var formattedNumber = '';
    if (number.length > 0) {
        formattedNumber += '(' + number.substring(0, 3);
    }
    if (number.length > 3) {
        formattedNumber += ') ' + number.substring(3, 6);
    }
    if (number.length > 6) {
        formattedNumber += '-' + number.substring(6, 10);
    }

    // Setting the formatted value back to the input
    input.value = formattedNumber;
}

$('#personal input[required]').on('blur', function () {
    validateField($(this));
});

function validateField(field) {
    if (field.val().trim() === '') {
        field.addClass('is-invalid');
    } else {
        field.removeClass('is-invalid');
    }
}