$(function () {
    $('#classTable').hide();
    $('#classSelector').show();
    $('.classimg img').fadeTo(0, .3);
    $('.vehicleClass p.vehicleName').fadeTo(0, .3);
    $('.carouselArrow').fadeTo(0, 0);
    $('.carouselArrow').hover(function () { $(this).fadeTo('fast', .6); }, function () { $(this).fadeTo('fast', 0); });
    $('.vehicleType').click(function (event) {
        event.preventDefault();
        if (!$(this).hasClass('active')) {
            $('.vehicleType.active').removeClass('active');
            $(this).addClass('active');
            var tw = Number($(this).data("tw"));
            $('.vehicleClass.exceeds').removeClass('exceeds');
            var exceeds = false;
            var scrollobj;
            var vclasses = $(".vehicleClass");
            for (var i = 0; i < vclasses.length; i++) {
                var objtw = Number($(vclasses[i]).data('tw'));
                if (tw >= objtw) {
                    $(vclasses[i]).find('p.vehicleName').fadeTo('fast', 1);
                    $(vclasses[i]).find('.classimg img').fadeTo('fast', 1);
                } else {
                    if (!exceeds) {
                        exceeds = true;
                        scrollobj = vclasses[i - 1];
                    }
                    $(vclasses[i]).addClass('exceeds');
                    $(vclasses[i]).find('p.vehicleName').fadeTo('fast', .3);
                    $(vclasses[i]).find('.classimg img').fadeTo('fast', .3);
                }
            }
            if (scrollobj == undefined) scrollobj = vclasses[i - 1];
            //if (!isVisible(scrollobj) || isPartiallyVisible(scrollobj)) {
            if (isRight(scrollobj)) {
                scrollCarousel(offsetRight(scrollobj));
                //scrollCarousel(offsetLeft(scrollobj));
            } else {
                scrollCarousel(optimizeLeft(scrollobj));
            }
            //}
        }
    });

    $('.vehicleClass').hoverIntent(function () {
        $(this).hasClass('exceeds') ? $(this).find('div.exceedsinfo').fadeIn('fast', function () { adjustHeight() }) : $(this).find('div.classinfo').fadeIn('fast', function () { adjustHeight() });

    }, function () {
        $(this).find('div.classinfo').fadeOut('fast', function () { adjustHeight() });
        $(this).find('div.exceedsinfo').fadeOut('fast', function () { adjustHeight() });
    });
    $('#carouselArrowLeft').click(function () { scrollCarousel(-1) });
    $('#carouselArrowRight').click(function () { scrollCarousel(1) });
});

$(window).load(function () {
    initScroll();
});

function initScroll() {
    var widthval = 0;
    var idcount = 0;
    $('.vehicleClass').each(function () {
        idcount++;
        $(this).attr('id', 'vehicle_' + idcount);
        $(this).find('div.classinfo').css('width', $(this).width() + 'px');
        $(this).find('div.exceedsinfo').css('width', $(this).width() + 'px');
    });
    $('.vehicleClass').each(function () {
        widthval += $(this).outerWidth();
    });
    $('#carousel').width(widthval);
    adjustHeight();
};

function getVisible() {
    var objlist = new Array();
    var visleft = $('#classCarousel').offset().left;
    var visright = visleft + $('#classCarousel').width();
    var heightval = 0;
    $('.vehicleClass').each(function (i, obj) {
        var objleft = $(this).offset().left;
        var objright = objleft + $(this).outerWidth();
        if ((objleft > visleft || objright > visleft) && (objleft < visright || objright < visright)) {
            //Object is visible on page
            objlist.push(obj);
        }
    });
    return objlist;
}

function adjustHeight() {
    var objlist = getVisible();
    var heightval = 0;
    for (var i = 0; i < objlist.length; i++) {
        heightval = ($(objlist[i]).outerHeight(true) > heightval) ? $(objlist[i]).outerHeight(true) : heightval;
    };
    $('#classCarousel').animate({ height: heightval + 'px' }, { queue: false, duration: 'fast', specialEasing: { height: 'easeOutQuart' }, complete: function () { adjustArrows(); } });
};

function adjustArrows() {
    var targetheight = $('#classCarousel').height();
    $('.carouselArrow').height(targetheight);
    var bordersize = (targetheight * .8) / 2;
    var marginsize = (targetheight - (targetheight * .8)) / 2;
    $('.carouselArrow span').css("border-top-width", bordersize + 'px');
    $('.carouselArrow span').css("border-bottom-width", bordersize + 'px');
    $('.carouselArrow span').css("marginTop", marginsize + 'px');
}

function getPosition(target) {
    var pos = 0;
    $('.vehicleClass').each(function (i, obj) {
        if ($(obj).attr('id') == $(target).attr('id')) pos = i;
    });
    return pos;
}

function getByPosition(pos) {
    return $('.vehicleClass')[pos];
}

function isVisible(obj) {
    var objlist = getVisible();
    var targetpos = getPosition($(obj));
    var smallpos = getPosition($(objlist[0]));
    var largepos = getPosition($(objlist[objlist.length - 1]));
    return (smallpos <= targetpos && largepos >= targetpos) ? true : false;
}

function isLeft(obj) {
    var objlist = getVisible();
    var targetpos = getPosition($(obj));
    var leftpos = getPosition($(objlist[0]));
    if (isPartiallyVisible(objlist[0])) {
        leftpos++;
    }
    return (leftpos > targetpos) ? true : false;
}

function isRight(obj) {
    var objlist = getVisible();
    var targetpos = getPosition($(obj));
    var rightpos = getPosition($(objlist[objlist.length - 1]));
    if (isPartiallyVisible(objlist[objlist.length - 1])) {
        rightpos--;
    }
    return (rightpos < targetpos) ? true : false;
}

function offsetRight(obj) {
    var objlist = getVisible();
    var targetpos = getPosition($(obj));
    var rightpos = getPosition($(objlist[objlist.length - 1]));
    if (isPartiallyVisible(objlist[objlist.length - 1])) {
        rightpos--;
    }
    return (targetpos - rightpos);
}

function offsetLeft(obj) {
    var objlist = getVisible();
    var targetpos = getPosition($(obj));
    var leftpos = getPosition($(objlist[0]));
    if (isPartiallyVisible(objlist[0])) {
        leftpos++;
    }
    return (targetpos - leftpos);
}

function optimizeLeft(obj) {
    var targetpos = getPosition($(obj)) + 1;
    var totalroom = $('#classCarousel').width();
    var targetroom = 0;
    while (targetroom < totalroom) {
        targetpos--;
        if (targetpos < 0) {
            targetpos = 0;
            break;
        }
        var target = getByPosition(targetpos);
        targetroom += $(target).outerWidth();
    }
    if (targetroom > totalroom) targetpos++;
    target = getByPosition(targetpos);
    return offsetLeft(target);
}

function isPartiallyVisible(obj) {
    var objlist = getVisible();
    var leftpos = Number($('#carousel').css('left').split('p')[0]);
    var visleft = $('#classCarousel').position().left;
    var visright = visleft + $('#classCarousel').width();
    if ((($(obj).position().left + $(obj).outerWidth()) + leftpos) > visright) {
        //partially off screen right
        return true;
    }
    if (($(obj).position().left + leftpos) < visleft) {
        //partially off screen left
        return true;
    }
    return false;
}

function scrollCarousel(scrolldir) {
    var objlist = getVisible();
    var visleft = $('#classCarousel').position().left;
    var visright = visleft + $('#classCarousel').width();
    var leftpos = Number($('#carousel').css('left').split('p')[0]);
    var pixamount = 0;
    var currentdiv = 0;
    if (scrolldir > 0) {
        //scroll right
        //check if last visible div is fully on screen or only partially
        // get last item in the visible list
        var obj = $(objlist[objlist.length - 1]);
        var objright = $(obj).position().left + $(obj).outerWidth();
        var currentpos = getPosition(obj);
        currentid = Number($(obj).attr('id').split('_')[1]);
        if ((objright + leftpos) > visright) {
            //partially off screen
            scrolldir--;
        }
        var targetpos = currentpos + scrolldir;
        if (targetpos > ($('.vehicleClass').length - 1)) {
            targetpos = ($('.vehicleClass').length - 1);
        }
        var targetobj = getByPosition(targetpos);
        pixamount = $(targetobj).position().left + $(targetobj).outerWidth();
        pixamount = -(pixamount - visright);
    } else {
        //scroll left
        var obj = $(objlist[0])
        var objleft = $(obj).position().left;
        var currentpos = getPosition((objlist[0]));
        currentid = Number($(objlist[0]).attr('id').split('_')[1]);
        if ((objleft + leftpos) < visleft) {
            //partially off screen
            scrolldir++;
        }
        var targetpos = currentpos + scrolldir;
        if (targetpos < 0) targetpos = 0;
        var targetobj = getByPosition(targetpos);
        pixamount = $(targetobj).position().left;
        pixamount = (visleft - pixamount);
    }
    $('#carousel').animate({ left: pixamount + 'px' }, { queue: false, duration: 'normal', specialEasing: { width: 'easeOutQuart' }, complete: function () { adjustHeight() } });
}