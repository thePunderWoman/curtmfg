/**
* popup is designed to scale up images to full size upon hover of images
* that were scaled down with css.
*
* popUp r1 // 2012.06.13 // jQuery 7.2+
*  
* popup is currently available for use in all personal or commercial 
* projects under both MIT and GPL licenses. This means that you can choose 
* the license that best suits your project, and use it accordingly.
* 
* // basic usage (just like .hover) receives onMouseOver and onMouseOut functions
* $("ul li").hoverIntent( showNav , hideNav );
* 
* // advanced usage receives configuration object only
* $("ul li").hoverIntent({
*	sensitivity: 7, // number = sensitivity threshold (must be 1 or higher)
*	interval: 100,   // number = milliseconds of polling interval
*	over: showNav,  // function = onMouseOver callback (required)
*	timeout: 0,   // number = milliseconds delay before onMouseOut function call
*	out: hideNav    // function = onMouseOut callback (required)
* });
* 
* @param  f  onMouseOver function || An object with configuration options
* @param  g  onMouseOut function  || Nothing (use configuration options object)
* @author    Jessica Janiuk
*/

(function ($) {
    $.fn.popUp = function () {
        // default configuration options
        var cfg = {
            duration: "fast",
            containerID: "container",
            interval: 200,
            easing: "easeOutQuart"
        };
        //easing: "easeOutQuart"

        // declare timer variable
        var hoverTimer;

        // A private function for comparing current and previous mouse position
        var popupImage = function (obj) {
            $(obj).css('z-index', '1');
            var scrollLeft = $(window).scrollLeft();
            var scrollTop = $(window).scrollTop();
            var windowHeight = $(window).height();
            var windowWidth = $(window).width();
            var origh = $(obj).height();
            var origw = $(obj).width();
            var img = new Image();
            img.src = $(obj).attr('src');
            var offset = $(obj).offset();
            var contoffset = $('#' + cfg.containerID).offset();
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

            $(newimg).animate({ height: img.height + 'px', width: img.width + 'px', top: topval + 'px', left: leftval + 'px' }, { queue: false, duration: cfg.duration, specialEasing: { height: cfg.easing, width: cfg.easing, top: cfg.easing, left: cfg.easing} });
            $(newimg).css('box-shadow', '0 0 25px #444');
            $(newimg).on({ click: function () { restoreActualSize($(this)); }, mouseleave: function () { restoreActualSize($(this)); } });
        };

        var restoreActualSize = function (obj) {
            var origh = $(obj).data("height");
            var origw = $(obj).data("width");
            var origt = $(obj).data("top");
            var origl = $(obj).data("left");
            $(obj).animate({ height: origh + 'px', width: origw + 'px', top: origt + 'px', left: origl + 'px' }, { queue: false, duration: cfg.duration, specialEasing: { height: cfg.easing, width: cfg.easing, top: cfg.easing, left: cfg.easing },
                complete: function () {
                    $(obj).remove();
                }
            });
        };

        var handleOver = function (e) {
            var ev = jQuery.extend({}, e);
            var obj = this;

            hoverTimer = setTimeout(function () { popupImage(obj) }, cfg.interval);
        };

        var handleOut = function () {
            hoverTimer = clearTimeout(hoverTimer);
        };

        // bind the function to the two event listeners
        return this.bind('mouseenter', handleOver).bind('mouseleave', handleOut);
    };
})(jQuery);