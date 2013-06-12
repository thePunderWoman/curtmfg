var partsperpage = 10;
var end = "";
var loadingpage = false;
var currentPage = 0;
var lastpage = 0;
var earliestPage = 0;
var APIURL = "http://api.curtmfg.com/v2";
var gridloadmore = false;
$(function () {
    $(window).on("tabcallback", function (e) {
        if (getCurrentTab() == 'grid' && $('table.gridview tbody tr').length == 0) {
            if (!gridloadmore) {
                gridloadmore = true;
                loadGrid(1);
            }
        }
    });
    $('#parts #pagination').hide();
    $('#loading').show().css('display', 'block');
    Shadowbox.init({
        height: '80%',
        width: '80%',
        enableKeys: false,
        displayCounter: false
    });
    currentPage = getCurrentPage();
    lastpage = currentPage;
    earliestPage = currentPage;

    if (earliestPage > 1) {
        $('#loadPrevious').show();
    }
    $('#loadPrevious').live('click', function () {
        getPreviousPage();
    });
    initScroll();
    $('.classProducts').css('display', 'none');


    $('.zoom').live('click', function () {
        $(this).parent().find('.bigImage').click();
        return false;
    });

    $('#showCategoryContent').click(function (event) {
        event.preventDefault();
        if ($(this).html() == "See More &gt;") {
            $(this).html("&lt; See Less");
            $('#categoryContent').slideDown();
        } else {
            $(this).html("See More &gt;");
            $('#categoryContent').slideUp('fast');
        }
    });

    $('#hideCategoryContent').click(function (event) {
        event.preventDefault();
        $('#showCategoryContent').html("See More &gt;");
        $('#categoryContent').slideUp('fast');
    });

    $('#fromName').live('blur', function () {
        if ($.trim($(this).val()) != '') {
            var text = $(this).val() + end;
            $('#messagePreview').html(text);
        }
    });

    $('.btnCompare').live('click', function () {
        var status = false;
        var selectedHitches = 0;
        $.each($('.checkCompare'), function () {
            if ($(this).is(':checked')) {
                selectedHitches++;
            }
        });
        if (selectedHitches < 2) {
            alert("You must select at least two hitches to compare.");
            return false;
        } else {
            return true;
        }
    });
    $('.mail').toggle();
    $('.mail img').toggle();

    $('#sub_nav ul li').not('.active').find('a img').fadeTo(0, 0);

    $('#sub_nav ul li').not('.active').find('a').hover(
        function () {
            $(this).find('img').fadeTo(600, 1);
        },
        function () {
            $(this).find('img').fadeTo(600, 0);
        });
    $('#sub_nav ul li.active .overlay').css('display', 'none');
});


function mailHitch(prodID) {
    var message = "[Your name] would like to share a CURT Part with you. http://www.curtmfg.com/part/" + prodID;
    end = " would like to share a CURT Part with you. http://www.curtmfg.com/part/" + prodID;
    $('#messagePreview').text(message);
    var html = $('#shadowbox_mail').html();
    Shadowbox.open({
        content: html,
        player: 'html',
        width: 600,
        height: 475
    });
}

$(window).load(function () {
    $('.subcategory span.subcatimage img').each(function () {
        var ratio = $(this).width() / $(this).height();
        var pratio = $(this).parent().width() / $(this).parent().height();
        if (ratio < pratio) {
            // image is taller than it is wide
            $(this).css('height', '100%');
            $(this).css('width', 'auto');
        }
    });
});

function updatePage() {
    if (getCurrentTab() == "parts") {
        if (detectStateSupport()) {
            window.history.replaceState({}, '', window.location.pathname + "?page=" + currentPage + getQueryStringMinusParameter("page"));
        } else {
            //console.log(getHashStringMinusParameter("page"));
            window.location.hash = "page=" + currentPage + getHashStringMinusParameter("page");
        }
    }
}

function triggerPage() {
    if (detectStateSupport()) {
        $(window).trigger("pagestate");
    } else {
        if (detectHashchangeSupport()) $(window).trigger("hashchange");
    } 
}

function getCurrentPage() {
    if ((getParameterByName("page") == null || getParameterByName("page") == "") && (getHashParameterByName("page") == null || getHashParameterByName("page") == "")) {
        return 1;
    } else {
        var hashpage = getHashParameterByName("page");
        if(hashpage != null) {
            window.location = window.location.protocol + "//" + window.location.host + window.location.pathname + '?page=' + hashpage;
        }
        return getParameterByName("page");
    }
}

function readyForNextPage() {
    if (!$('#loading').is(':visible')) return false;
    var threshold = 1500;
    var bottomPosition = $(window).scrollTop() + $(window).height();
    var distanceFromBottom = $(document).height() - bottomPosition;
    
    return distanceFromBottom <= threshold;
}

function observeScroll() {
    if (getCurrentTab() == 'parts') {
        if (readyForNextPage()) getNextPage();
        var inView = new Array();
        var percents = new Array();
        var visible = 1;
        $(".partPage").each(function () {
            if (isInView($(this))) {
                // check for which elements are in view and get the percentage of the viewport they take up
                inView.push(Number($(this).attr('id').split('_')[1]));
                percents.push(viewPercentage($(this)));
            }
        });
        if (inView.length > 1) {
            var visible = 0;
            var percent = 0;
            for (var x in inView) {
                if (percents[x] > percent) {
                    // this loops through the in view elements and makes the one that takes up the most space the "currentPage"
                    percent = percents[x];
                    visible = inView[x];
                }
            }
        } else if (inView.length == 1) {
            visible = inView[0];
        }
        if (currentPage != visible) {
            // Here we trigger the url update method if the page differs from what's already there
            currentPage = visible;
            triggerPage();
        }
    }
}

function getNextPage() {
    if (loadingpage) return;
    loadingpage = true;
    var jsonobj = $.getJSON(APIURL + "/GetCategoryParts?catID=" + $('#categoryid').val() + "&vehicleID=" + $('#vehicleID').val() + "&page=" + (Number(lastpage) + 1) + "&perpage=" + partsperpage + "&dataType=JSONP&callback=?", function (data) {
        if (data.length > 0) {
            lastpage++;
            if (data.length < partsperpage) {
                $('#loading').hide();
            }
            $("#loading").before('<div class="partPage" style="display: none;" id="page_' + lastpage + '"></div>');
            $('#page_' + lastpage).append('<span class="pageNumber">Page ' + lastpage + '</span>');
            $(data).each(function (i, part) {
                appendPart(part, lastpage);
            });
            $('#page_' + lastpage).fadeIn();
            adjustTabSize();
            loadingpage = false;
        } else {
            $('#loading').hide();
        }

    }).error(function (data) { });
}

function getPreviousPage() {
    var topdistance = $('#page_' + currentPage).offset().top - $(window).scrollTop();
    $('#loadPrevious').hide();
    $('#loadingprevious').show().css('display', 'block');
    var jsonobj = $.getJSON(APIURL + "/GetCategoryParts?catID=" + $('#categoryid').val() + "&vehicleID=" + $('#vehicleID').val() + "&page=" + (Number(earliestPage) - 1) + "&perpage=" + partsperpage + "&dataType=JSONP&callback=?", function (data) {
        if (data.length > 0) {
            earliestPage--;
            $("#page_" + currentPage).before('<div class="partPage" style="display: none;" id="page_' + earliestPage + '"></div>');
            $('#page_' + earliestPage).append('<span class="pageNumber">Page ' + earliestPage + '</span>');
            $(data).each(function (i, part) {
                appendPart(part, earliestPage);
            });
            $('#page_' + earliestPage).fadeIn();
            adjustTabSize();
            $('#loadingprevious').hide();
            if (earliestPage > 1) {
                $('#previouspage').html('Load Page ' + (earliestPage - 1));
                $('#loadPrevious').show();
            }
            var newtopdistance = $('#page_' + currentPage).offset().top;
            window.scrollTo($(window).scrollLeft(), newtopdistance - topdistance);
        } else {
            $('#loadPrevious').hide();
        }

    });
}

function getDescriptions(part) {
    var descs = new Array();
    if(part.content.length > 0) {
        for (var i in part.content) {
            if (part.content[i].key != undefined && part.content[i].key.length > 0 && part.content[i].key.toLowerCase() == "listdescription") descs.push(part.content[i].value);
        }
    }
    return descs;
}

function getShortDescription(part) {
    if (part.content.length > 0) {
        for (var i in part.content) {
            if (part.content[i].key != undefined && part.content[i].key.length > 0 && part.content[i].key.toLowerCase() == "listdescription") {
                return part.content[i].value;
            }
        }
    }
    return "";
}

function getAttributes(part) {
    var attribs = new Array();
    if (part.attributes.length > 0) {
        for (var i in part.attributes) {
            if (part.attributes[i].key != undefined && part.attributes[i].key.length > 0 && part.attributes[i].key.toLowerCase() != "upc") attribs.push(part.attributes[i]);
        }
    }
    return attribs;
}

function getUPC(part) {
    var upc = "";
    if (part.attributes.length > 0) {
        for (var i in part.attributes) {
            if (part.attributes[i].key != undefined && part.attributes[i].key.length > 0 && part.attributes[i].key.toLowerCase() == "upc") upc = part.attributes[i].value;
        }
    }
    return upc;
}

function getVehicleAttributes(part) {
    var attribs = new Array();
    if (part.vehicleAttributes != null && part.vehicleAttributes.length > 0) {
        for (var i in part.vehicleAttributes) {
            if (part.vehicleAttributes[i].key != undefined && part.vehicleAttributes[i].key.length > 0) attribs.push(part.vehicleAttributes[i]);
        }
    }
    return attribs;
}

$(window).scroll(observeScroll);

function initScroll() {
    if (detectStateSupport()) {
        $(window).on("pagestate", function (e) {
            updatePage();
        });
    } else if (detectHashchangeSupport()) {
        // No history support...IE8/9 FF3
        $(window).on("hashchange", function (e) {
            updatePage();
        });
    } else {
        // Old browser IE7 and earlier
        /*lastHash = window.location.hash;
        setInterval(function () {
            if (lastHash !== window.location.hash) {
                updatePage();
                lastHash = window.location.hash;
            }
        });
        updatePage()*/
    }
}

function addInstallSheets(partobj, part) {
    var instructionPath = "";
    if(part.content.length > 0) {
        for (var i in part.content) {
            if (part.content[i].key != undefined && part.content[i].key.length > 0 && part.content[i].key.toLowerCase() == "installationsheet") instructionPath = part.content[i].value;
        }
    }

    if (instructionPath != "") {
        $(partobj).find('p.msrp').after('<a class="instructions" target="_blank" href="' + instructionPath + '"><span>Instructions</span><img src="/Content/img/pdf.png" width="20" /></a>');
    }
}

function addAppGuides(partobj, part) {
    var guidePath = "";
    if (part.content.length > 0) {
        for (var i in part.content) {
            if (part.content[i].key != undefined && part.content[i].key.length > 0 && part.content[i].key.toLowerCase() == "appguide") guidePath = part.content[i].value;
        }
    }

    if (guidePath != "") {
        $(partobj).find('p.msrp').after('<a class="instructions" target="_blank" href="' + guidePath + '"><span>Fit Your Vehicle</span><img src="/Content/img/pdf.png" width="20" /></a>');
    }
}

function mailPart(prodID) {
    var message = "[Your name] would like to share a CURT Part with you. http://www.curtmfg.com/part/" + prodID;
    end = " would like to share a CURT Part with you. http://www.curtmfg.com/part/" + prodID;
    $('#messagePreview').text(message);
    var html = $('#shadowbox_mail').html();
    Shadowbox.open({
        content: html,
        player: 'html',
        width: 600,
        height: 475
    });
}

function isInView(elem) {
    var docViewTop = $(window).scrollTop();
    var docViewBottom = docViewTop + $(window).height();

    var elemTop = $(elem).offset().top;
    var elemBottom = elemTop + $(elem).height();

    return ((elemBottom >= docViewTop) && (elemTop <= docViewBottom));
}

function viewPercentage(elem) {
    var docViewTop = $(window).scrollTop();
    var docViewBottom = docViewTop + $(window).height();

    var elemTop = $(elem).offset().top;
    var elemBottom = elemTop + $(elem).height();
    if(elemTop < docViewTop) {
        //element top is above viewport
        return (docViewTop - elemBottom) / (docViewTop - docViewBottom);
    } else if (elemTop >= docViewTop && elemBottom <= docViewBottom ) {
        // element is completely in viewport
        return (elemTop - elemBottom) / (docViewTop - docViewBottom);
    } else {
        // just element top is in viewport
        return (elemTop - docViewBottom) / (docViewTop - docViewBottom);
    }
}

function detectStateSupport() {
    return !!(window.history && history.replaceState);
}

function loadGrid(gridpage) {
    var perpage = 50;
    var catID = $('#categoryid').val();
    var vehicleID = $('#vehicleID').val();
    var attributes = new Array();
    $('table.gridview thead tr th.attribute').each(function () { attributes.push($(this).html()); });
    $.getJSON('http://api.curtmfg.com/V2/GetCategoryParts?dataType=JSONP&callback=?', { 'catID': catID, "page": gridpage, "perpage": perpage }, function (data) {
        if (data.length == 0) {
            gridloadmore = false;
            $('#gridViewLoading').remove();
        }
        $(data).each(function (i, part) {
            var row = '<tr class="' + ((i % 2 == 0) ? "even" : "odd") + '">'
            try {
                row += '<td><img class="actualsize" src="' + part.images[2].path + '" alt="Part #' + part.partID + '" /></td>';
            } catch (err) { }
            row += "<td>" + part.partID + "</td>";
            $(attributes).each(function (x, attr) {
                var found = false;
                $(part.attributes).each(function (y, partattr) {
                    if (partattr.key.toLowerCase() == attr.toLowerCase()) {
                        row += '<td>' + partattr.value + ((partattr.key.toLowerCase() == "upc" && part.priceCode != 0) ? " PC: " + part.priceCode : "") + '</td>';
                        found = true;
                    }
                });
                if (!found) row += "<td></td>";
            });
            row += '<td><a class="button" href="/Part/' + part.partID + '">Details</a></td>';
            row += "</tr>";
            $('table.gridview tbody').append(row);
            if (i == (data.length - 1)) {
                adjustTabSize();
                if (gridloadmore) {
                    loadGrid(gridpage + 1);
                } else {
                    $('#gridViewLoading').remove();
                }
            }
        });
    });
}

function appendPart(part, page) {
    var newpart = $('#part-template').tmpl(part);
    $('#page_' + page).append(newpart);
    var thispart = $('#part_' + part.partID);
    var vehicleSpecific = $('#vehicleSpecific').val().toLowerCase() == "true";
    var imgsrc = "";
    var tallimages = [];
    var grandeimages = [];
    if (part.images.length > 0) {
        for (var im in part.images) {
            if (part.images[im].size == "Tall") tallimages.push(part.images[im]);
            if (part.images[im].size == "Grande") grandeimages.push(part.images[im]);
            if (part.images[im].size == "Grande" && part.images[im].sort == "a") {
                imgsrc = part.images[im].path;
            }
        }
    }
    if (tallimages.length > 0) {
        tallimages.sort(imagesort);
        grandeimages.sort(imagesort);
    }
    if (vehicleSpecific && Number(part.vehicleID) > 0 && $('#vehicleID').val() != 0) {
        $(thispart).addClass('fit');
        $(thispart).find('.fitinfo').append('<img src="/Content/img/fit.png" alt="Fits your vehicle" /><span>Fits your vehicle</span>');
    } else if (vehicleSpecific && part.vehicleID == 0 && $('#vehicleID').val() != 0) {
        $(thispart).addClass('nofit');
        $(thispart).find('.fitinfo').append('<img src="/Content/img/nofit.png" alt="Does NOT fit your vehicle" /><span>Does NOT fit your vehicle</span>');
    } else if (vehicleSpecific && $('#vehicleID').val() == 0) {
        $(thispart).addClass('novehicle');
        $(thispart).find('.fitinfo').append('<img src="/Content/img/warning.png" alt="Vehicle Specific Part" /><span><a href="/HitchLookup" class="fitYourVehicle">Vehicle Specific Part</a></span>');
    } else {
        for (var x = 0; x < tallimages.length; x++) {
            $(thispart).find('.fitinfo span.clear').before('<img class="thumb" data-partid="' + part.partID + '" data-grande="' + grandeimages[x].path + '" src="' + tallimages[x].path + '" alt="Thumbnail for Part ' + part.partID + ' ' + tallimages[x].sort + '"/>');
        }
    }
    if (part.reviews.length == 0) {
        $(thispart).find('div.starrating').addClass('norating');
        $(thispart).find('div.starrating').append('<span>No Reviews</span>');
    } else {
        $(thispart).find('div.starrating').append('<span class="stars" style="width: ' + ((part.averageReview / 5) * 100) + '%;"></span>');
    }
    if (imgsrc == "") {
        imgsrc = "/Content/img/noimage.png";
    }
    $(thispart).find('img.product_img').attr('src', imgsrc);
    var descriptions = getDescriptions(part);
    var shortDescription = getShortDescription(part);
    if (shortDescription.length > 0) {
        $(thispart).find('.shortDescription').html(shortDescription);
    }
    var attributes = getAttributes(part);
    var upc = getUPC(part);
    var vattributes = getVehicleAttributes(part);
    var classstr = (part.pClass != "") ? '<span>Class: <span>' + part.pClass + '</span></span>' : '';
    var attrtable = '';
    var tablecontents = '';
    if (attributes.length > 0 || vattributes.length > 0) {
        for (var x = 0; x < 4; x++) {
            if (attributes.length > x) {
                tablecontents += '<tr><td class="label">' + attributes[x].key + '</td><td>' + attributes[x].value + '</td></tr>';
            }
        }
        for (var x = 0; x < 2; x++) {
            if (vattributes.length > x) {
                tablecontents += '<tr><td class="label">' + vattributes[x].key + '</td><td>' + vattributes[x].value + '</td></tr>';
            }
        }
    }
    if (tablecontents != '') {
        attrtable = '<table class="partattributes"><thead><tr><th colspan="2">Product Details</th></tr></thead><tbody>' + tablecontents + '</tbody></table>';
    }
    if (part.priceCode > 0) {
        $(thispart).find('.msrp').after('<p class="priceCode">PC <span>' + part.priceCode + '</span></p>');
    }
    if (upc != "") {
        $(thispart).find('.msrp').after('<p class="upc">UPC <span>' + upc + '</span></p>');
    }
    $(thispart).find('.shortDescription').after(attrtable);
    addInstallSheets(thispart, part);
    addAppGuides(thispart, part)
    gapi.plusone.go("part_" + part.partID);
}

function imagesort(a, b) {
    //Compare "a" and "b" in some fashion, and return -1, 0, or 1
    return a.sort.charCodeAt(0) - b.sort.charCodeAt(0);
}