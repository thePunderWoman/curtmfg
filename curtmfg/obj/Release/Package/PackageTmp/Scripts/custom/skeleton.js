/*
 * This file will house our javascript for the template of our page
 */

var currentTab;

$(function () {

    $("#dealerLogin").live('click', function () {
        $('#searchArea').slideUp(function () {
            $('#loginArea').slideDown();
        });
    });

    $("#search").live('click', function () {
        $('#loginArea').slideUp(function () {
            $('#searchArea').slideDown();
        });
    });

    $('#username').focus(function () {
        $(this).select();
    });

    $('#password').focus(function () {
        $(this).select();
    });

    $('#searchCriteria').focus(function () {
        $(this).select();
    });

    if (!Modernizr.input.placeholder) {
        $("input").each(
            function () {
                if ($(this).val() == "" && $(this).attr("placeholder") != "") {
                    $(this).val($(this).attr("placeholder"));
                    $(this).focus(function () {
                        if ($(this).val() == $(this).attr("placeholder")) $(this).val("");
                    });
                    $(this).blur(function () {
                        if ($(this).val() == "") $(this).val($(this).attr("placeholder"));
                    });
                }
            });
    }
});

