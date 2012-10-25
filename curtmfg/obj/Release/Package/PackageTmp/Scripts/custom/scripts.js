var zoom = document.documentElement.clientWidth / window.innerWidth;
var count = 0;
/*var hoverTimer;*/
$(function () {
    fitMenu();
    $(document).on("click", "#wheretobuyfind", function (event) {
        event.preventDefault();
        var zip = $.trim($('#wheretobuyzipcode').val());
        var partid = $(this).data('partid');
        var analyticstr = 'Searched for zip code ' + zip + ' on part ' + partid;
        if (zip != "") {
            if (typeof (_gaq) != 'undefined') {
                _gaq.push(['_trackEvent', 'WhereToBuy', 'Searched Local', analyticstr]);
            }
            window.location.href = "http://" + window.location.host + "/WhereToBuy?search=" + zip + "#tab=local";
        }
    });
    $('img.actualsize').popUp();

    /*$(document).on('mouseover', 'img.actualsize', function () {
    var obj = $(this);
    hoverTimer = setTimeout(function () { popupImage(obj) }, 200);
    });
    $(document).on('mouseout', 'img.actualsize', function () { hoverTimer = clearTimeout(hoverTimer); });*/

    $(document).on('mouseover', '.fitinfo img.thumb', function () {
        var partid = $(this).data('partid');
        var grandepath = $(this).data('grande');
        var tpart = $('#part_' + partid);
        $(tpart).find('img.product_img').attr('src', grandepath);
    });

    $(document).on('click', '.clearvehicle', function (event) {
        event.preventDefault();
        $(this).hide();
        var gohome = $(this).data('home') == 'yes';
        $('.clearloading').show();
        $.post('/HitchLookup/Clear', function () {
            if (gohome) {
                location.href = '/'
            } else {
                location.reload(true);
            }
        });
    });

    $(document).on('click', '.fitYourVehicle', function (event) {
        event.preventDefault();
        $('body').append('<div id="site_overlay"></div>');
        $('#site_overlay').fadeTo('fast', .75);
        $('#hitchlookup').appendTo('body');
        var stylestr = "";
        stylestr = "left:" + (($(window).width() - $('#hitchlookup').width()) / 2) + "px;"
            + "top:" + (($(window).height() - $('#hitchlookup').height()) / 2) + "px;"
            + "position: fixed; background: #666;";
        $('#hitchlookup').attr('style', stylestr);
        $('#closeLookup').show();
        $('#hitchlookup').hide();
        $('#hitchlookup').fadeIn('fast');
        $('#select_year').focus();
        $(document).keyup(function (e) {
            if (e.keyCode == 27) { cancelLookup() }   // esc
        });
    });

    $('#closeLookup').on({ click: function () {
        cancelLookup()
    }
    });

    $(document).on('click', 'li.onlinedealer a', function (event) {
        var name = $(this).data('name');
        var partid = $(this).data('partid');
        var pos = $(this).data('position');
        var analyticstr = 'Clicked dealer ' + name + ' (' + pos + ') for part ' + partid;
        if (typeof (_gaq) != 'undefined') {
            _gaq.push(['_trackEvent', 'WhereToBuy', 'Clicked Dealer', analyticstr]);
        }
    });

    $(document).on('click', '#seemoredealers', function (event) {
        var partid = $(this).data('partid');
        var analyticstr = 'Clicked See More for part ' + partid;
        if (typeof (_gaq) != 'undefined') {
            _gaq.push(['_trackEvent', 'WhereToBuy', 'Clicked See More', analyticstr]);
        }
    });

    $(document).on('click', '#becomedealer', function (event) {
        var partid = $(this).data('partid');
        var analyticstr = 'Clicked Become a Dealer - part ' + partid;
        if (typeof (_gaq) != 'undefined') {
            _gaq.push(['_trackEvent', 'WhereToBuy', 'Clicked Become Dealer', analyticstr]);
        }
    });

    $('#sub_nav ul li').not(".active").hoverIntent(
        function () {
            $(this).animate({ top: "-2px", boxShadow: '0 4px 10px #000' }, { queue: false, duration: 'fast' });
            $(this).find('span').animate({ top: "110px" }, { queue: false, duration: 'fast' });
            $(this).find('div.overlay').animate({ top: "106px" }, { queue: false, duration: 'fast' });
        },
        function () {
            $(this).animate({ top: "0", boxShadow: '0 3px 2px #444' }, { queue: false, duration: 'fast' });
            $(this).find('span').animate({ top: "104px" }, { queue: false, duration: 'fast' });
            $(this).find('div.overlay').animate({ top: "104px" }, { queue: false, duration: 'fast' });
        }
    );

    $(document).on("click", ".buyOnline", function (event) {
        event.preventDefault();
        var partID = $(this).attr('id');
        var analyticstr = 'Opened Where to buy for part ' + partID;
        if (typeof (_gaq) != 'undefined') {
            _gaq.push(['_trackEvent', 'WhereToBuy', 'Initial Click', analyticstr]);
        }
        $('body').append('<div id="site_overlay"></div>');
        $('body').append('<div id="site_overlay_container"></div>');
        $('#site_overlay_container').append('<div id="wheretobuydealers"></div>');
        $('#wheretobuydealers').append('<span id="wheretobuyclose" title="Close Where To Buy Box">&times;</span>');
        $('#wheretobuydealers').append('<h2>Where To Buy Online</h2>');
        $('#wheretobuydealers').append('<ul id="wheretobuyul"></ul>');
        $('#wheretobuydealers').append('<img id="dealerloading" src="/Content/img/ajax-loader.gif" alt="loading..." />');
        $('#wheretobuydealers').append('<div class="clear"></div>');
        $('#wheretobuydealers').append('<h2>Where To Buy Locally</h2>');
        $('#wheretobuydealers').append('<div id="wheretobuyform"><label for="wheretobuyzipcode">Zip Code <input type="text" id="wheretobuyzipcode" value="" /></label><input type="button" data-partid="' + partID + '" id="wheretobuyfind" class="button" value="Find" /></div>');
        $('#wheretobuydealers').css({ 'margin': 'auto', 'top': (($(window).height() - $('#wheretobuydealers').outerHeight()) / 2) + "px", 'display': 'none' });
        $('#wheretobuydealers').append('<a class="button" id="becomedealer" data-partid="' + partID + '" href="/Dealer/BecomeADealer" target="_blank">Become a Dealer</a>');
        $('#site_overlay').fadeTo('fast', .75);
        $('#site_overlay_container').fadeTo('fast', 1);
        $('#wheretobuydealers').fadeTo('fast', 1);
        $(window).on({ resize: adjustTop });
        $('#wheretobuyclose').click(function (event) { removeWhereToBuy(); });
        $('body').on({ keyup: function (event) {
            if (event.keyCode == 27) {
                removeWhereToBuy();
            } else {
                event.preventDefault();
            }
        }
        });
        $.getJSON('/Dealer/GetWhereToBuyDealers', function (data) {
            $('#dealerloading').remove();
            var count = 4;
            var dealers = new Array();
            $(data).each(function (x, obj) {
                dealers.push(obj);
            });
            for (var i = count; i > 0; i--) {
                var rand = Math.round(Math.random() * (dealers.length - 1));
                $('#wheretobuyul').append('<li class="onlinedealer" style="display: none;"><a target="_blank" data-position="' + i + '" data-partid="' + partID + '" data-name="' + dealers[rand].name + '" href="' + dealers[rand].searchURL + partID + '">' + ((dealers[rand].logo != null && dealers[rand].logo != "") ? '<img src="' + dealers[rand].logo + '" alt="' + dealers[rand].name + '" />' : dealers[rand].name) + '</a></li>');
                dealers.splice(rand, 1);
            }
            $('#wheretobuyul').append('<li class="clear"></li>');
            $('#wheretobuyul li.onlinedealer').fadeIn();
            $('#wheretobuyul').after('<a href="/WhereToBuy/#tab=online" data-partid="' + partID + '" id="seemoredealers" target="_blank">See More Dealers...</a>');
        });
    });
});

/*function restoreActualSize(obj) {
    var origh = $(obj).data("height");
    var origw = $(obj).data("width");
    var origt = $(obj).data("top");
    var origl = $(obj).data("left");
    $(obj).animate({ height: origh + 'px', width: origw + 'px', top: origt + 'px', left: origl + 'px' }, { queue: false, duration: 'fast', specialEasing: { height: 'easeOutQuart', width: 'easeOutQuart', top: 'easeOutQuart', left: 'easeOutQuart' },
        complete: function () {
            $(obj).remove();
        }
    });
}*/

$(window).resize(function () {
    var zoomNew = document.documentElement.clientWidth / window.innerWidth;
    if (zoom != zoomNew) {
        // zoom has changed
        // adjust your fixed element
        zoom = zoomNew
        fitMenu();
    }
});

function fitMenu() {
    var totalwidth = $('#nav_container').width() - 1;
    var count = $('#nav_container ul li a').length;
    var awidth = 0;
    var newwidth = 0;
    $('#nav_container ul li').each(function () {
        $(this).attr('style', '');
        $(this).find('a').attr('style', '');
        awidth += $(this).find('a').outerWidth();
    });
    var padding = Math.floor((totalwidth - awidth) / count);
    $('#nav_container ul li a').each(function () {
        $(this).css('padding-left', Math.floor(padding / 2));
        $(this).css('padding-right', Math.floor(padding / 2));
        newwidth += $(this).outerWidth();
    });
}

function getImageDimensions(imgsrc) {
    var img = new Image();
    img.src = imgsrc;

    var height = img.height;
    var width = img.width;
    return [width, height];
}

function removeWhereToBuy() {
    $('#site_overlay').remove();
    $('#site_overlay_container').remove();
    $('#wheretobuydealers').remove();
    $('body').off('keyup');
    $(window).off('resize');
}

function adjustTop(event) {
    var topstr = (($(window).height() - $('#wheretobuydealers').outerHeight()) / 2) + "px";
    $('#wheretobuydealers').css('top', topstr);
}

/*function popupImage(obj) {
    var scrollLeft = $(window).scrollLeft();
    var scrollTop = $(window).scrollTop();
    var windowHeight = $(window).height();
    var windowWidth = $(window).width();
    var origh = $(obj).height();
    var origw = $(obj).width();
    var img = new Image();
    img.src = $(obj).attr('src');
    var offset = $(obj).offset();
    var contoffset = $('#container').offset();
    var offleft = offset.left - contoffset.left;
    var idstr = "actualsizeimg-" + count++;
    var topval = (origh - img.height) / 2 + offset.top;
    var leftval = (origw - img.width) / 2 + offleft;
    if (leftval + img.width > scrollLeft + windowWidth) {
        leftval = scrollLeft + windowWidth - img.width;
    }
    if (leftval < 0 + scrollLeft) {
        leftval = 0 + scrollLeft;
    }
    if ((topval + img.height) > (scrollTop + windowHeight)) {
        topval = scrollTop + windowHeight - img.height;
    }
    if (scrollTop > topval) {
        topval = scrollTop;
    }
    var imgstr = '<img class="actualsizeimg" id="' + idstr + '" data-top="' + offset.top + '" data-left="' + offleft + '" data-height="' + origh + '" data-width="' + origw + '" src="' + $(obj).attr('src') + '" alt="Actual Size Image" style="width: ' + origw + 'px; height: ' + origh + 'px; top: ' + offset.top + 'px; left: ' + offleft + 'px;"/>'
    $('body').append(imgstr);
    var newimg = $('#' + idstr);

    $(newimg).animate({ height: img.height + 'px', width: img.width + 'px', top: topval + 'px', left: leftval + 'px' }, { queue: false, duration: 'fast', specialEasing: { height: 'easeOutQuart', width: 'easeOutQuart', top: 'easeOutQuart', left: 'easeOutQuart'} });
    $(newimg).on({ click: function () { restoreActualSize($(this)); }, mouseleave: function () { restoreActualSize($(this)); } });
}*/

function cancelLookup() {
    $('#hitchlookup').removeAttr("style");
    $('#closeLookup').hide();
    $('#hitchlookup').insertAfter('#nav_container');
    $('#site_overlay').remove();
}