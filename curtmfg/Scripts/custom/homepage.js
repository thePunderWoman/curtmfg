var loadTweets, loadYouTube, loadLatestParts, getBullets, getNotes, swapImage, loadVideo;
$(function () {
    readyMethods()
});

function readyMethods() {
    //When page loads...
    /*$(".video_content").hide(); //Hide all content
    $("ul.videos li:last a").addClass("selectedVideo"); //Activate first tab
    $(".video_content:first").show(); //Show first tab content*/

    $.getJSON('/Twitter/GetTweets', loadTweets);
    $.get('http://api.curtmfg.com/V2/getLatestParts?dataType=JSONP&callback=loadLatestParts', function () { }, 'jsonp');

    loadYouTube();

    //On Click Event
    $(document).on("click", "ul.videos li a", function () {
        $("ul.videos li a").removeClass("selectedVideo"); //Remove any "active" class
        $(this).addClass("selectedVideo"); //Add "active" class to selected tab
        $(".video_content").hide(); //Hide all tab content

        var activeTab = $(this).attr("href"); //Find the href attribute value to identify the active tab + content
        $(activeTab).fadeIn(); //Fade in the active ID content
        $(activeTab + " iframe").fadeIn();
        return false;
    });

    $(document).on('click', '#videos .videoIframe span.overlay', function (e) {
        e.preventDefault();
        loadVideo($(this));
    });

    var $imgs = $('img', '#fitYourLifestyle').get();
    var random = $.randomize($imgs);
    $('#fitYourLifestyle #lifestyleimages img').each(function () { $(this).remove() });
    $('#fitYourLifestyle #lifestyleimages').append(random);
    $('#fitYourLifestyle #lifestyleimages img:first').show();
    $('#fitYourLifestyle #lifestyleimages').cycle('fade');

    // handle the product swap
    $(document).on('click', '#part_container div', function () {
        $('#part_container div').show();
        $(this).hide();
        idstr = $(this).attr('id').split('_')[1]
        $('.product').hide();
        $('#part_' + idstr).show();
        adjustTabSize()
    });
}

loadTweets = function(data) {
    $('#twitterloader').hide();
    $('#twitterBox div.tweet').remove();
    $(data).each(function (i, obj) {
        var t = '<div class="tweet"><p class="date">' + obj.date + '</p><a class="tweet_text" target="_blank" href="http://twitter.com/CURTMFG/status/' + obj.twitterTweetID + '">' + obj.tweet1 + '</a><span class="clear"</span></div>';
        $('#tweetmore').before(t);
    });
};

loadYouTube = function () {
    $.getJSON("/Video/getHomePageVideos", function (data) {
        $('#videos').empty();
        $('#videoContainer ul.videos').empty();
        $(data).each(function (i, obj) {
            var vidtab = '<li><a href="#video' + i + '"></a></li>';
            if ($('#videoContainer ul.videos li').length == 0) {
                $('#videoContainer ul.videos').append(vidtab);
            } else {
                $('#videoContainer ul.videos li:first').before(vidtab);
            }
            var vid = '<div id="video' + i + '" class="video_content video-js-box">';
            vid += '<span class="video_title">' + obj.title + '</span>';
            vid += '<span id="videospan-' + obj.youtubeID + '" class="videoIframe"><img src="' + obj.screenshot + '" alt="' + obj.title + '" class="videoThumb" /><span class="overlay" data-videoID="' + obj.embed_link + '"><span class="playbutton"></span></span></span>';
            vid += '<div class="video_text"><span>' + obj.description + '</span></div></div>';
            $('#videos').append(vid);
        });
        $(".video_content").hide(); //Hide all content
        $("ul.videos li:last a").addClass("selectedVideo"); //Activate first tab
        $(".video_content:first").show(); //Show first tab content
    });
    $('#videos').append('<div class="clear"></div>');
}

loadVideo = function (obj) {
    var ytvideoID = $(obj).data("videoid");
    $('#videospan-' + ytvideoID).empty();
    $('#videospan-' + ytvideoID).append('<iframe allowtransparency="true" id="frame_' + ytvideoID + '" width="270" height="203" frameborder="0" allowfullscreen></iframe>');
    $('iframe#frame_' + ytvideoID).attr('src', 'http://www.youtube.com/embed/' + ytvideoID + '?autoplay=1&rel=0');
}

loadLatestParts = function (data) {
    $('#latestpartloader').hide();
    $('#partloadingmessage').remove();
    $(data).each(function (i, obj) {
        var imgsrc = "";
        var imgloader = "/Content/img/ajax-loader4.gif";
        var smallimgsrc = "";
        var largeimgobj = new Image();
        var smallimgobj = new Image();
        var tallimages = [];
        var grandeimages = [];
        if (obj.images.length > 0) {
            for (var im in obj.images) {
                if (obj.images[im].size == "Tall") tallimages.push(obj.images[im]);
                if (obj.images[im].size == "Grande") grandeimages.push(obj.images[im]);
                if (obj.images[im].size == "Grande" && obj.images[im].sort == "a") {
                    imgsrc = obj.images[im].path;
                }
                if (obj.images[im].size == "Tall" && obj.images[im].sort == "a") {
                    smallimgsrc = obj.images[im].path;
                }
            }
        }
        if (tallimages.length > 0) {
            tallimages.sort(imagesort);
            grandeimages.sort(imagesort);
        }
        if (imgsrc == "") {
            imgsrc = "/Content/img/noimage.png";
        }
        if (smallimgsrc == "") {
            smallimgsrc = "/Content/img/noimage.png";
        }

        largeimgobj.src = imgsrc;
        largeimgobj.setAttribute("id", obj.partID + '_large');
        largeimgobj.onload = function () { swapImage($(this)) };
        smallimgobj.src = smallimgsrc;
        smallimgobj.setAttribute("id", obj.partID + '_small');
        smallimgobj.onload = function () { swapImage($(this)) }; ;

        var thumb = '<div id="partimg_' + obj.partID + '"><img class="loader" id="' + obj.partID + '_small" src="' + imgloader + '" alt="' + obj.partID + ' thumb" /></div>'
        var part = '<div class="product" id="part_' + obj.partID + '"><div class="imgContainer"><a href="/part/' + obj.partID + '">';
        part += '<img class="loader" id="' + obj.partID + '_large" src="' + imgloader + '" alt="' + obj.partID + '" />';
        part += '</a></div><div class="productInfo">';
        part += '<a href="/part/' + obj.partID + '" class="shortDesc">' + obj.shortDesc + '</a>';
        part += '<ul class="desc">';
        var notes = getNotes(obj);
        var bullets = getBullets(obj);
        $(notes).each(function (x, note) {
            part += '<li>' + note + '</li>';
        });
        $(bullets).each(function (x, bullet) {
            part += '<li>' + bullet + '</li>';
        });

        part += '</ul><h5>Average Customer Review</h5>';
        part += '<div style="float: left;" class="starrating' + ((obj.reviews.length == 0) ? " norating" : "") + '">';
        if (obj.reviews.length > 0) {
            part += '<span class="stars" style="width: ' + ((Number(obj.averageReview) / 5) * 100) + '%"></span>';
        } else {
            part += '<span>No Reviews</span>';
        }
        part += '</div><a href="/part/' + obj.partID + '#tab=reviews" class="writeReview">Write a Review</a></div><div class="clear"></div></div>';
        $('#part_container').before(part);
        $('#part_container span.clear').before(thumb);
    });
    $('.product').hide();
    $('.product').first().show();
    $('#part_container div').first().hide();
    adjustTabSize()
};

swapImage = function (curimg) {
    var idstr = $(curimg).attr('id');
    $('#' + idstr).removeClass('loader');
    $('#' + idstr).hide()
    $('#' + idstr).attr('src', '');
    $('#' + idstr).attr('src', $(curimg).attr('src'));
    $('#' + idstr).fadeIn();
}

getBullets = function (part) {
    var bullets = new Array();
    if (part.content.length > 0) {
        for (var i in part.content) {
            if (part.content[i].key != undefined && part.content[i].key.length > 0 && part.content[i].key.toLowerCase() == "bullet") bullets.push(part.content[i].value);
        }
    }
    return bullets;
}

getNotes = function (part) {
    var notes = new Array();
    if (part.content.length > 0) {
        for (var i in part.content) {
            if (part.content[i].key != undefined && part.content[i].key.length > 0 && part.content[i].key.toLowerCase() == "note") notes.push(part.content[i].value);
        }
    }
    return notes;
}

function imagesort(a, b) {
    //Compare "a" and "b" in some fashion, and return -1, 0, or 1
    return a.sort.charCodeAt(0) - b.sort.charCodeAt(0);
}