var currentTab;
var lastHash;
var hashtime = 250;
var hashinterval;
var contwidth = 0;
var cpad = 0;
var count = 0;
$(function () {
    contwidth = $('#tab_container').parent().width();
    cpad = Number($('#tab_container .tab_content').css('paddingLeft').split('p')[0]) + Number($('#tab_container .tab_content').css('paddingRight').split('p')[0]);
    currentTab = getCurrentTab();
    initTabs();
    if (!detectHistorySupport()) {
        triggerHandlers();
    }
    //On Click Event
    $("ul.tabs li a").click(function (event) {
        event.preventDefault();
        currentTab = $(this).attr('id').split('_')[0];
        pushLocation(currentTab);
    });

    if (isTouchSupported() && $('.tab_content').length > 1) {
        $('.tab_content').on({ touchstart: function (event) { dragTab(event); } });
    }
    $('#tab_container img').imagesLoaded( adjustTabSize );
});

function dragTab(event) {
    var cursor = event.originalEvent.touches[0].pageX;
    var cursory = event.originalEvent.touches[0].pageY;
    var allowTouchEvents = true;
    $('.notouch').each(function () {
        if ((cursor > $(this).offset().left && cursor < ($(this).offset().left + $(this).width())) && (cursory > $(this).offset().top && cursory < ($(this).offset().top + $(this).height()))) {
            allowTouchEvents = false;
        }
    });
    if (allowTouchEvents) {
        var tableft = Number($('#tab_scroll').css('left').split('p')[0]);
        var touchStartTime = new Date();
        var lastTouch = cursor;
        $('.tab_content').on({ touchmove: function (moveEvent) {
            lastTouch = moveEvent.originalEvent.touches[0].pageX;
            if (Math.abs(lastTouch - cursor) > 50) {
                moveEvent.preventDefault();
                $('.tab_content').disableSelection();
                observeMove(moveEvent, cursor, lastTouch, tableft);
            }
        }
        });
        $('.tab_content').on({ touchend: function (upEvent) {
            $('.tab_content').off("touchmove");
            $('.tab_content').off("touchend");
            $('.tab_content').enableSelection();
            var touchEndTime = new Date();
            var tabwidth = $('.tab_content.active').outerWidth();
            var tabx = getTabIndex(currentTab);
            var movement = lastTouch - cursor;
            var velocity = Math.abs(movement) / (touchEndTime.getTime() - touchStartTime.getTime());
            if (velocity > .5 || (Math.abs(movement) > tabwidth / 2)) {
                // move tab
                if (movement < 0) {
                    //moving to the tab to the right
                    if (tabx != $('.tab_content').length - 1) {
                        var newtab = $('.tab_content').get(tabx + 1);
                        currentTab = $(newtab).attr('id');
                        pushLocation(currentTab);
                    } else {
                        switchTab()
                    }
                } else {
                    //moving to the tab to the left
                    if (tabx != 0) {
                        var newtab = $('.tab_content').get(tabx - 1);
                        currentTab = $(newtab).attr('id');
                        pushLocation(currentTab);
                    } else {
                        switchTab()
                    }
                }
            } else {
                switchTab()
            }
        }
        });
    }
}

function getTabIndex(tabid) {
    var tabindex = -1;
    $('.tab_content').each(function (x, obj) {
        if ($(obj).attr('id') == tabid) {
            tabindex = x;
        }
    });
    return tabindex;
}

function getTabByIndex(tabx) {
    var tabindex = null;
    $('.tab_content').each(function (x, obj) {
        if ($(obj).attr('id') == tabid) {
            tabindex = $('.tab_content')[x + tabx];
        }
    });
    return tabindex;
}

function observeMove(event, cursor, cursorPosition, tableft) {
    $('#tab_scroll').css('left', (tableft + (cursorPosition - cursor)) + "px");
}

function getCurrentTab() {
    if (getParameterByName("tab") == null && getHashParameterByName("tab") == null) {
        return $(".tab_content:first").attr('id');
    } else if (getParameterByName("tab") == null) {
        return getHashParameterByName("tab");
    } else {
        return getParameterByName("tab");
    }
}

function pushLocation(tab) {
    if (detectHistorySupport()) {
        window.history.pushState({}, '', window.location.pathname + "?tab=" + currentTab + getQueryStringMinusParameter("tab"));
    } else {
        window.location.hash = "tab=" + currentTab + getHashStringMinusParameter("tab");
    }
}

function adjustTabSize() {
    $('#tab_container').animate({ height: $('#tab_scroll #' + getCurrentTab()).outerHeight() + 'px' }, { queue: false, duration: 'normal' }, 'easeOutQuart');
    $('#tab_scroll').animate({ height: $('#tab_scroll #' + getCurrentTab()).outerHeight() + 'px' }, { queue: false, duration: 'normal' }, 'easeOutQuart');
}

function initTabs() {
    $('#tab_container .tab_content').each(function() {$(this).width(contwidth - cpad);});
    $('#tab_container #tab_scroll').width(contwidth * $('#tab_container .tab_content').length);
    if (detectHistorySupport()) {
        $(window).on("popstate", function (e) {
            switchTab();
        });
    } else if (detectHashchangeSupport()) {
        // No history support...IE8/9 FF3
        $(window).on("hashchange", function (e) {
            switchTab();
        });
    } else {
        // Old browser IE7 and earlier
        lastHash = window.location.hash;
        hashinterval = setInterval(pollURL, hashtime);
        switchTab();
    }
}

function pollURL() {
    adjustTabSize()
    if (lastHash != window.location.hash) {
        switchTab();
        lastHash = window.location.hash;
    }
}

function triggerHandlers() {
    if (detectHistorySupport()) {
        $(window).trigger("popstate");
    } else if (detectHashchangeSupport()) {
        // No history support...IE8/9 FF3
        if (getParameterByName("tab") != null) {
            window.location = window.location.protocol + "//" + window.location.host + window.location.pathname + '#tab=' + currentTab;
        }
        $(window).trigger("hashchange");
    } else {
        // Old browser IE7 and earlier
        if (getParameterByName("tab") != null) {
            window.location = window.location.protocol + "//" + window.location.host + window.location.pathname + '#tab=' + currentTab;
        }
        switchTab()
    }
}

function detectHistorySupport() {
    //return !!(window.history && history.pushState);
    return false;
}

function detectHashchangeSupport() {
    var isSupported = "onhashchange" in window;
    if (!isSupported && window.setAttribute) {
        window.setAttribute("onhashchange", "return;");
        isSupported = typeof window.onhashchange === "function";
    }
    return isSupported;
}

function getParameterByName(name) {
    var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
    return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
}

function getQueryStringMinusParameter(param) {
    var qrystr = window.location.search.replace(param + "=" + getParameterByName(param), "");
    qrystr = qrystr.replace('?', '');
    if (qrystr != "") {
        var qrysplit = qrystr.split("&");
        qrystr = "";
        $.each(qrysplit, function (i, str) {
            if(str != "") qrystr += "&" + str;
        });
    }
    return qrystr;
}

function getHashStringMinusParameter(param) {
    var qrystr = window.location.hash.toString().replace(param + "=" + getHashParameterByName(param), "");
    
    qrystr = qrystr.replace('#', '');
    if (qrystr != "") {
        var qrysplit = qrystr.split("&");
        qrystr = "";
        $.each(qrysplit, function (i, str) {
            if (str != "") qrystr += "&" + str;
        });
    }
    return qrystr;
}
 function getHashParameterByName(name) {
    var match = RegExp(name + '=([^&]*)').exec(window.location.hash);
    return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
}

function switchTab() {
    $("ul.tabs li a").removeClass("active"); //Remove any "active" class
    $('#' + getCurrentTab() + '_tab').addClass("active"); //Add "active" class to selected tab
    var curtab = $('#tab_scroll #' + getCurrentTab());
    var tabindex = $('#tab_scroll div.tab_content').index(curtab);
    var leftpx = contwidth * tabindex;
    adjustTabSize();
    $("#tab_scroll").animate({ left: -leftpx + 'px' }, { queue: false, duration: 'slow', specialEasing: {left: 'easeOutQuart'}, complete: function () {
        $('.tab_content').removeClass('active');
        $('#tab_scroll #' + getCurrentTab()).addClass("active");
        $(window).trigger("tabcallback");
    } 
    });
}

function isTouchSupported() {
    return 'ontouchmove' in document.documentElement;
}