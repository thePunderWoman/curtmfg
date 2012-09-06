$(function () {
    $('#addDiscussionForm').hide();
    $('#addDiscussionForm h4').hide();
    $('#addDiscussionForm #submitpost').hide();
    $('#addDiscussionForm #testimonialrecaptcha').hide();
    $("#addDiscussionForm").dialog({
        modal: true,
        autoOpen: false,
        title: "Start a Discussion",
        height: 470,
        width: 470,
        buttons: {
            "Submit": function () {
                var title = $.trim($('#addtitle').val());
                var post = $.trim($('#post').val());
                var email = $.trim($('#email').val());
                var notify = ($('#notify').is(':checked')) ? true : false;
                errors = false;
                if (notify && email == "") {
                    $('#email').addClass('ui-state-error');
                    errors = true;
                }
                if (title == "") {
                    $('#addtitle').addClass('ui-state-error');
                    errors = true;
                }
                if (post == "") {
                    $('#post').addClass('ui-state-error');
                    errors = true;
                }
                if (!errors) {
                    showLoadingBox()
                    $.post('/Forum/AddDiscussionAjax/', $('#discussionForm').serialize(), function (data) {
                        if (data.error == undefined) {
                            $("#addDiscussionForm").dialog("close");
                            if (data.approved) {
                                var newthread = '<div class="thread"><div class="threadinfo"><p class="replies">Replies<span>0</span></p><span class="clear"></span></div><div class="threaddetails"><a class="title" href="/Forum/Discussion/' + data.threadID + '/' + data.slug + '">' + data.title + '</a><p class="latest"><strong>Latest Post:</strong> ' + data.date + '</p></div><div class="clear"></div></div>';
                                $('#threads').children(':first').before(newthread);
                            } else {
                                hideLoadingBox();
                                $("#success-message").html("Your Discussion was submitted for approval.")
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

                            }
                        } else {
                            hideLoadingBox();
                            Recaptcha.reload();
                            $('#recaptcha_response_field').addClass('ui-state-error');
                            $('#recaptcha_response_field').val();
                            alert(data.error);
                        }
                    }, "json");
                }
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        },
        close: function () {
            hideLoadingBox()
            $('#addtitle').val("");
            $('#post').val("");
            $('#email').val("");
            $('#name').val("");
            $('#company').val("");
            $('#notify').attr("checked", false);
            $('#discussionForm input').removeClass('ui-state-error');
            $('#discussionForm textarea').removeClass('ui-state-error');
            Recaptcha.reload();
            $('#recaptcha_response_field').removeClass('ui-state-error');
        }
    });

    $('#addDiscussion').click(function (event) {
        event.preventDefault();
        $('#addDiscussionForm').dialog('open');
    });

});

function showLoadingBox() {
    $('html').append('<div id="loadingBox"><img id="loading" alt="loading..." src="/Content/img/ajax-loader2.gif" /></div>');
    var wheight = $(window).height();
    var imgheight = $('#loading').height();
    var margintop = (wheight / 2) - (imgheight / 2);
    $('#loading').css('margin-top', margintop);
}

function hideLoadingBox() {
    $('#loadingBox').remove();
}