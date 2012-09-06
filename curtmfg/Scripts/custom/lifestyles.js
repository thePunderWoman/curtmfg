$(function () {
    $('.lifestyleDetails').hide();
    $('a.lifestyleBox').click(function (event) {
        event.preventDefault();
        var idstr = $(this).attr('id').split('-')[1];
        if ($('a.lifestyleBox.active').length == 0 || ($('a.lifestyleBox.active').length > 0 && $('a.lifestyleBox.active').attr('id').split('-')[1] != idstr)) {
            var previd = ($('a.lifestyleBox.active').length > 0) ? $('a.lifestyleBox.active').removeClass('active').attr('id').split('-')[1] : null;
            $(this).addClass('active');
            if (previd != null) {
                $('#' + previd).slideUp('fast', function () { $('#' + idstr).slideDown(); });
            } else {
                $('#' + idstr).slideDown();
            }
        } else {
            $('.lifestyleDetails').slideUp('fast');
            $('a.lifestyleBox.active').removeClass('active');
        }
    });
});