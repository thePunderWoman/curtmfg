$(function () {
    $('#addtestimonial').hide();
    $('#addtestimonial h4').hide();
    $('#addtestimonial #submittestimonial').hide();
    $('#addtestimonial #testimonialrecaptcha').hide();
    $("#addtestimonial").dialog({
        modal: true,
        autoOpen: false,
        title: "Submit Your Testimonial",
        height: 300,
        width: 475,
        buttons: {
            "Submit Your Testimonial": function () {
                var title = $.trim($('#addtitle').val());
                var testimonial = $.trim($('#testimonial').val());
                var rating = $.trim($('#rating').val());
                var firstname = $.trim($('#first_name').val());
                var lastname = $.trim($('#last_name').val());
                var location = $.trim($('#location').val());
                rating = (isNaN(rating)) ? 0 : Number(rating);
                errors = false;
                if (rating == 0) {
                    $('#ratinglabel').addClass('ui-state-error');
                    errors = true;
                }
                if (title == "") {
                    $('#addtitle').addClass('ui-state-error');
                    errors = true;
                }
                if (testimonial == "") {
                    $('#testimonial').addClass('ui-state-error');
                    errors = true;
                }
                if (!errors) {
                    postdata = { addtitle: title, testimonial: testimonial, rating: rating, first_name: firstname, last_name: lastname, location: location };
                    $.post('/Testimonials/AddTestimonialAjax', postdata, function (data) {
                        $("#addtestimonial").dialog("close");
                        if (!(isNaN(data.testimonialID)) && data.testimonialID != 0) {
                            $("#success-message").html("Thank you! Your Testimonial was submitted for approval.")
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

    $('#submitTestimonial').click(function (event) {
        event.preventDefault();
        $('#addtestimonial').dialog('open');
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