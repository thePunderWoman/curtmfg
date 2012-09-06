/**
* hoverIntent is similar to jQuery's built-in "hover" function except that
* instead of firing the onMouseOver event immediately, hoverIntent checks
* to see if the user's mouse has slowed down (beneath the sensitivity
* threshold) before firing the onMouseOver event.
* 
* hoverIntent r6 // 2011.02.26 // jQuery 1.5.1+
* <http://cherne.net/brian/resources/jquery.hoverIntent.html>
* 
* hoverIntent is currently available for use in all personal or commercial 
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
* @author    Brian Cherne brian(at)cherne(dot)net
*/
(function ($) {
    $.fn.tooltip = function () {
        // default configuration options
        var cfg = {
            verticalPosition: 'top',
            horizontalPosition: 'center',
            classname: 'tooltip'
        };
        // override configuration options with user supplied object
        cfg = $.extend(cfg);

        // instantiate variables
        // cX, cY = current X and Y position of mouse, updated by mousemove event
        // pX, pY = previous X and Y position of mouse, set by mouseover and polling interval
        var tooltip_next;

        // A private function for handling mouse 'hovering'
        var showTip = function (e) {
            // copy objects to be passed into t (required for event object to be passed in IE)
            var ev = jQuery.extend({}, e);
            var ob = this;

            // check if tooltip is already visible
            //if (ob.hoverIntent_t) { ob.hoverIntent_t = clearTimeout(ob.hoverIntent_t); }

            var getNextID = function () {
                var next = -1;
                $('.tooltip').each(function (i, obj) {
                    next = (Number($(obj).data('id')) > next) ? Number($(obj).data('id')) : next;
                });
                return next + 1;
            };

            // if e.type == "mouseenter"
            if (e.type == "mouseenter") {
                var nextid = getNextID();
                $('html').append('<div class="' + cfg.classname + '" id="tooltip_' + nextid + '" data-id="' + nextid + '">' + $(this).attr('title') + '</div>');
                $(this).data('tooltip', nextid);
                $('#tooltip_' + nextid).css({ 'position': 'absolute' });
                switch (cfg.verticalPosition) {
                    case 'top':
                        $('#tooltip_' + nextid).css({ "top": $(this).offset().top - $('#tooltip_' + nextid).outerHeight() + 'px' });
                        break;
                    case 'middle':
                        $('#tooltip_' + nextid).css({ "top": $(this).offset().top - (($('#tooltip_' + nextid).outerHeight() / 2) - ($(this).outerHeight() / 2)) + 'px' });
                        break;
                    case 'bottom':
                        $('#tooltip_' + nextid).css({ "top": $(this).offset().top + $(this).outerHeight() + 'px' });
                        break;
                }
                switch (cfg.horizontalPosition) {
                    case 'left':
                        $('#tooltip_' + nextid).css({ "left": $(this).offset().left - $('#tooltip_' + nextid).outerWidth() + "px" });
                        break;
                    case 'center':
                        $('#tooltip_' + nextid).css({ "left": $(this).offset().left - (($('#tooltip_' + nextid).outerWidth() / 2) - ($(this).outerWidth() / 2)) + "px" });
                        break;
                    case 'right':
                        $('#tooltip_' + nextid).css({ "left": $(this).offset().left + $(this).outerWidth() + "px" });
                        break;
                }
                $('#tooltip_' + nextid).fadeIn('fast');
                // else e.type == "mouseleave"
            } else {
                var tipid = $(this).data('tooltip');
                $('#tooltip_' + tipid).fadeOut('fast', function () { $('#tooltip_' + tipid).remove(); });
                $(this).data('tooltip', '');
            }
        };

        // bind the function to the two event listeners
        return this.on('mouseenter', showTip).on('mouseleave', showTip);
    };
})(jQuery);