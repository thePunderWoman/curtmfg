var vehicleTable;

$(function () {
    Shadowbox.init({
        "continuous": true
    });
    $('.tip[title]').tooltip();
    $('#addreview #submitreview').hide();
    $('#addreview #reviewrecaptcha').hide();
    $("#addreview").dialog({
        modal: true,
        autoOpen: false,
        title: "Submit Your Review",
        height: 300,
        width: 475,
        buttons: {
            "Submit Your Review": function () {
                var partID = $('#partID').val();
                var subject = $.trim($('#subject').val());
                var review = $.trim($('#review_text').val());
                var rating = $.trim($('#rating').val());
                var name = $.trim($('#name').val());
                var email = $.trim($('#email').val());
                rating = (isNaN(rating)) ? 0 : Number(rating);
                errors = false;
                if (rating == 0) {
                    $('#ratinglabel').addClass('ui-state-error');
                    errors = true;
                }
                if (subject == "") {
                    $('#subject').addClass('ui-state-error');
                    errors = true;
                }
                if (review == "") {
                    $('#review_text').addClass('ui-state-error');
                    errors = true;
                }
                if (!errors) {
                    $("#addreview").dialog("close");
                    postdata = { subject: subject, review_text: review, rating: rating, name: name, email: email };
                    $.post('/Part/AddReviewAjax/' + partID, postdata, function (data) {
                        if (data.result == "success") {
                            $('#subject').val("");
                            $('#email').val("");
                            $('#review_text').val("");
                            $('#rating').val(0);
                            $('#name').val("");
                            restoreStarRating();
                            $("#success-message").html("Thank you! Your review was submitted for approval.")
                            $("#success-message").dialog({
                                autoOpen: true,
                                modal: true,
                                title: "Success!!",
                                buttons: {
                                    Ok: function () {
                                        $(this).dialog("close");
                                    }
                                }
                            });
                        } else {
                            $("#success-message").html("There was a problem on our side with your submission.  We'd appreciate it if you'd try again later. Sorry!")
                            $("#success-message").dialog({
                                autoOpen: true,
                                modal: true,
                                title: "There was a problem...Sorry!!",
                                buttons: {
                                    Ok: function () {
                                        $(this).dialog("close");
                                    }
                                }
                            });

                        }
                    }, "json");
                }
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        },
        close: function () {
            $('#addtitle').val("");
            $('#testimonial').val("");
            $('#rating').val("0");
            $('#first_name').val("");
            $('#last_name').val("");
            $('#location').val("");
            restoreStarRating();
        }
    });

    $(document).on('click', '#primaryVideo', function () {
        var partid = $(this).data('partid');
        var analyticstr = 'Viewed Primary Video for part ' + partid;
        if (typeof (_gaq) != 'undefined') {
            _gaq.push(['_trackEvent', 'Part', 'Primary Video', analyticstr]);
        }
    });

    $(document).on('click', '#showVideo', function () {
        var partid = $(this).data('partid');
        var analyticstr = 'Viewed Installation Video for part ' + partid;
        if (typeof (_gaq) != 'undefined') {
            _gaq.push(['_trackEvent', 'Part', 'Installation Video', analyticstr]);
        }
    });

    $('.writeReview').click(function (event) {
        event.preventDefault();
        $('#addreview').dialog('open');
    });

    vehicleTable = $('#vehicleTable').dataTable({
        "bFilter": false,
        "bInfo": false,
        "bLengthChange": false,
        "bProcessing": true,
        "aaSorting": [[0, "desc"]],
        "bDeferRender": true,
        "bPaginate": true
    });

    $('#vehiclePrev').hide();
    if (Number($('#vehiclePagination').data('pages')) == 1) {
        $('#vehicleNext').hide();
    }

    $(document).on('click', '#vehiclePrev', function () {
        // Previous Page
        $('#vehiclePrev').disableSelection();
        var page = Number($('#vehiclePagination').data('page'));
        var pages = Number($('#vehiclePagination').data('pages'));
        if (page > 1) {
            page--;
            $('#vehicleNext').show()
            $('#vehiclePagination').data('page', page)
            $('#vehiclePage').html(page);
            vehicleTable.fnPageChange("previous");
            if (page <= 1) {
                $('#vehiclePrev').hide();
            }
        }
    });

    $(document).on('click', '#vehicleNext', function () {
        // Next Page
        $('#vehicleNext').disableSelection();
        var page = Number($('#vehiclePagination').data('page'));
        var pages = Number($('#vehiclePagination').data('pages'));
        if (page < pages) {
            page++;
            $('#vehiclePrev').show()
            $('#vehiclePagination').data('page', page)
            $('#vehiclePage').html(page);
            vehicleTable.fnPageChange("next");
            if (page >= pages) {
                $('#vehicleNext').hide();
            }
        }
    });

    $('#fromName').live('blur', function () {
        if ($.trim($(this).val()) != '') {
            var text = $(this).val() + end;
            $('#messagePreview').html(text);
        }
    });
    $('.mail').toggle();
    $('.mail img').toggle();
    $('.mainImg').hide();
    $('.mainImg:first').show();
    $('#smImages a:first').hide();

    $('#smImages a').click(function (event) {
        event.preventDefault();
        $('.mainImg').hide();
        var idstr = $(this).attr('id').split('_')[1];
        $('#hitchimg_' + idstr).fadeIn();
        $('#smImages a').show()
        $('#hitchthumb_' + idstr).hide();
    });

    restoreStarRating();
    if (Number($('#rating').val()) == 0) {
        $("#addreview").hide();
    }

    $('#rate .star').click(function () {
        rating = $(this).attr('id').split('_')[1];
        $('#rating').val(rating);
        restoreStarRating();
    });

    $('#rate .star').hover(function () {
        var currate = Number($(this).attr('id').split('_')[1])
        for (var i = 1; i <= 5; i++) {
            if (i <= currate) {
                $('#star_' + i).attr('class', 'star hovered');
            } else {
                $('#star_' + i).attr('class', 'star');
            }
        }
    }, function () { restoreStarRating(); });
});

function restoreStarRating() {
    $('#rate span').each( function () {$(this).attr('class','star');});
    rating = Number($('#rating').val());
    for(var i = 1; i <= rating; i++) {
        $('#star_' + i).addClass('selected');
    }
}

function mailPart(prodID) {
    var message = "[Your name] would like to share a CURT Part with you. http://www.curtmfg.com/part/" + prodID;
    end = " would like to share a CURT Part with you. http://www.curtmfg.com/part/" + prodID;
    $('#messagePreview').text(message);
    var html = $('#shadowbox_mail').html();
    Shadowbox.open({
        content: html,
        player: 'html',
        width: 600,
        height: 475
    });
}

$(window).load(function () {
    $('#accessories span img').each(function () {
        var ratio = $(this).width() / $(this).height();
        var pratio = $(this).parent().width() / $(this).parent().height();
        if (ratio < pratio) {
            // image is taller than it is wide
            $(this).css('height', '100%');
            $(this).css('width', 'auto');
        }
    });
});