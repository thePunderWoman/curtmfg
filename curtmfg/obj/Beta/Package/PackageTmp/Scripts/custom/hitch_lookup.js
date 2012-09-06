$(function () {
    if ($.browser.msie && $.browser.version < 9) {
        $('.lookup_selector select').live('focus', function () {
            $(this).data('orig_width', $(this).css('width')).css('width', 'auto');
        }).live('blur', function () {
            $(this).css('width', $(this).data('orig_width'));
        });
    }

    $.each($('#hor_lookup').find('select'), function (select, i) {
        var first_option = $(this).find('option[value="0"]').text();
        var select = $(this).clone();
        var name = $(this).attr('name');
        var id = $(this).attr('id');
        var style = $(this).attr('style');
        var new_html = '<div class="lookup_selector"><span>' + first_option + '</span><select name="' + name + '" id="' + id + '" style="' + style + '">' + $(select).html() + '</select></div>';
        $(this).after(new_html);
        $(this).remove();
    });

    // Handle the change of mount
    $('#select_mount').live('change', function () {
        // Initiate make, model, and style to default state
        $('#lookupError').hide();
        var mount = $('#select_mount').val();
        $('#select_year').html('<option value="">Select Year</option>');
        $('#select_make').html('<option value="">Select Make</option>');
        $('#select_make').attr('disabled', 'disabled');
        $('#select_model').html('<option value="">Select Model</option>');
        $('#select_model').attr('disabled', 'disabled');
        $('#select_style').html('<option value="">Select Style</option>');
        $('#select_style').attr('disabled', 'disabled');

        if (mount != "") {
            $('#select_year').parent().find('img').show();
            $.get('http://api.curtmfg.com/V2/GetYear?dataType=JSONP&callback=loadYear', { 'mount': mount }, function () { }, 'jsonp');
        }
    });
    
    // Handle the change of year
    $('#select_year').live('change', function () {
        // Initiate make, model, and style to default state
        $('#lookupError').hide();
        var mount = $('#select_mount').val();
        var year = $('#select_year').val();
        $('#select_make').html('<option value="">Select Make</option>');
        $('#select_model').html('<option value="">Select Model</option>');
        $('#select_model').attr('disabled', 'disabled');
        $('#select_style').html('<option value="">Select Style</option>');
        $('#select_style').attr('disabled', 'disabled');

        if (year > 0) {
            $('#select_make').parent().find('img').show();
            $.get('http://api.curtmfg.com/V2/GetMake?dataType=JSONP&callback=loadMake', { 'mount': mount, 'year': year }, function () { }, 'jsonp');
        }
    });

    // Handle the change of make
    $('#select_make').live('change', function () {
        // Initiate make, model, and style to default state
        $('#lookupError').hide();
        var mount = $('#select_mount').val();
        var year = $('#select_year').val();
        var make = $('#select_make').val();
        $('#select_model').html('<option value="">Select Model</option>');
        $('#select_style').html('<option value="">Select Style</option>');
        $('#select_style').attr('disabled', 'disabled');
        if (year > 0) {
            $('#select_model').parent().find('img').show();
            $.get('http://api.curtmfg.com/V2/GetModel?dataType=JSONP&callback=loadModel', { 'mount': mount, 'year': year, 'make': make }, function () { }, 'jsonp');
        }
    });

    // Handle the change of model
    $('#select_model').live('change', function () {
        $('#lookupError').hide();
        var mount = $('#select_mount').val();
        var year = $('#select_year').val();
        var make = $('#select_make').val();
        var model = $('#select_model').val();
        $('#select_style').html('<option value="">Select Style</option>');
        if (year > 0) {
            $('#select_style').parent().find('img').show();
            $.get('http://api.curtmfg.com/V2/GetStyle?dataType=JSONP&callback=loadStyle', { 'mount': mount, 'year': year, 'make': make, 'model': model }, function () { }, 'jsonp');
        }
    });

    // Validate the form on submit
    $('#btnSubmit').live('click', function () {
        $('#lookupError').text('');
        var mount = $('#select_mount').val();
        var year = $('#select_year').val();
        var make = $('#select_make').val();
        var model = $('#select_model').val();
        var style = $('#select_style').val();
        var status = true;
        var msg = '';
        if ($.trim(mount) == "") {
            $('#lookupError').text('');
            status = false;
            msg = 'Please select a mount.';
            $('#lookupError').text(msg);
            $('#lookupError').show();
        }
        if ($.trim(year) == "") {
            $('#lookupError').text('');
            status = false;
            msg = 'Please select a year.';
            $('#lookupError').text(msg);
            $('#lookupError').show();
        }
        if ($.trim(make) == "") {
            $('#lookupError').text('');
            status = false;
            msg = 'Please select a make.';
            $('#lookupError').text(msg);
            $('#lookupError').show();
        }
        if ($.trim(model) == "") {
            $('#lookupError').text('');
            status = false;
            msg = 'Please select a model.';
            $('#lookupError').text(msg);
            $('#lookupError').show();
        }
        if ($.trim(style) == "") {
            $('#lookupError').text('');
            status = false;
            msg = 'Please select a style.';
            $('#lookupError').text(msg);
            $('#lookupError').show();
        }
        return status;
    });

    $('#resetLookup').live('click', function () {
        $('#select_year').val("");
        $('#select_year').html('<option value="0">- Select Year -</option>').attr('disabled', 'disabled');
        $('#select_make').html('<option value="0">- Select Make -</option>').attr('disabled', 'disabled');
        $('#select_model').html('<option value="0">- Select Model -</option>').attr('disabled', 'disabled');
        $('#select_style').html('<option value="0">- Select Style -</option>').attr('disabled', 'disabled');
        return false;
    });
});

function loadYear(years) {
    $('#hitchLookupForm div img').hide();
    $.each(years, function (i, year) {
        $('#select_year').append('<option>' + year + '</option>');
    });
    $('#select_year').removeAttr('disabled');
}

function loadMake(makes) {
    $('#hitchLookupForm div img').hide();
    $.each(makes, function (i, make) {
        $('#select_make').append('<option>' + make + '</option>');
    });
    $('#select_make').removeAttr('disabled');
}

function loadModel(models) {
    $('#hitchLookupForm div img').hide();
    $.each(models, function (i, model) {
        $('#select_model').append('<option>' + model + '</option>');
    });
    $('#select_model').removeAttr('disabled');
}

function loadStyle(styles) {
    $('#hitchLookupForm div img').hide();
    $.each(styles, function (i, style) {
        $('#select_style').append('<option>' + style + '</option>');
    });
    $('#select_style').removeAttr('disabled');
}