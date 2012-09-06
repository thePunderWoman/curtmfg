$(function () {
    $('#addReplyForm').hide();
    $('#addReplyForm h4').hide();
    $('#addReplyForm #submitpost').hide();
    $("#addReplyForm").dialog({
        modal: true,
        autoOpen: false,
        title: "Reply",
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
                    $.post('/Forum/AddReplyAjax/', $('#replyForm').serialize(), function (data) {
                        if (data.error == undefined) {
                            $("#addReplyForm").dialog("close");
                            if (data.approved == false) {
                                $("#success-message").html("Your Reply was submitted for moderation.")
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
                                addPost(data);
                            }
                        } else {
                            hideLoadingBox()
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
            hideLoadingBox();
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

    $('#addReply').click(function (event) {
        event.preventDefault();
        $('#addtitle').val("Re: " + $('#threadTitle').html());
        $('#addReplyForm').dialog('open');
    });

    $('.flag').live('click', function (event) {
        event.preventDefault();
        var id = $(this).data("id");
        showLoadingBox()
        $.post("/Forum/Flag", { "postID": id }, function (data) {
            hideLoadingBox();
            if (data == "true") {
                $('#post_' + id).fadeOut();
                $("#success-message").html("Message was flagged as spam.")
                $("#success-message").dialog({
                    autoOpen: true,
                    modal: true,
                    title: "Flagged as Spam",
                    buttons: {
                        Ok: function () {
                            $(this).dialog("close");
                        }
                    }
                });
            } else {
                $("#success-message").html("There was a problem flagging the message as spam. Try again later.")
                $("#success-message").dialog({
                    autoOpen: true,
                    modal: true,
                    title: "A Problem Occurred",
                    buttons: {
                        Ok: function () {
                            $(this).dialog("close");
                        }
                    }
                });
            }
        }, "text");
    });

});

function addPost(data) {
    var email = data.email;
    var name = (data.name != "") ? data.name : "Anonymous";
    var post = '<div class="forumpost" id="post_' + data.postID + '" style="display: none;">' +
                    '<div class="datebox">' +
                    '<p><strong>Posted On</strong><br />' + data.date + '</p>' +
                    '<p><strong>By</strong><br />' + ((email != "") ? "<a href='mailto:" + email + "'>" + name + "</a>" : name) +
                    ((data.company != "") ? '<br />' + data.company : '') + '</p></div>' +
                    '<div class="postbox">' +
                    '<p class="title">' + data.title + '</p>' +
                    '<div class="controls"><a href="/Forum/FlagAsSpam/' + data.postID + '" data-id="' + data.postID + '" class="flag">Flag as Spam</a></div>' +
                    '<p>' + data.post + '</p></div><div class="clear"></div></div>';
    $('#forumposts').append(post);
    $('#post_' + data.postID).fadeIn();
}

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