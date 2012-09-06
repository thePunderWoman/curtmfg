$(function () {
    $('#contact_form').submit(function (event) {
        $('#contact_form input').removeClass('error');
        $('#contact_form textarea').removeClass('error');
        var errors = 0;
        if ($('#first_name').val() == 'Enter your first name.') {
            $('#first_name').val('');
        }
        if ($('#last_name').val() == 'Enter your last name.') {
            $('#last_name').val('');
        }
        if ($('#email').val() == 'Enter your e-mail address.') {
            $('#email').addClass('error');
            errors++;
        }
        if (validateEmail($('#email').val()) == false) {
            $('#email').addClass('error');
            errors++;
        }
        if ($('#type').val() == "") {
            $('#type').addClass('error');
            errors++;
        }
        if ($('#subject').val() == 'Enter the subject.') {
            $('#subject').addClass('error');
            errors++;
        }
        if ($.trim($('#contactmessage').val()) == '') {
            $('#contactmessage').addClass('error');
            errors++;
        }
        if (errors > 0) {
            event.preventDefault();
        }
    });
});

function validateEmail(email) { 
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
} 