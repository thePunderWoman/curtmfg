var map, geocoder, platinumjson, preferredjson, directionsmap, directionsmarker, initialize, fitRegion, setCenter, removeDirections, loadDirections, showDirections, loadRegions, clearInfoBoxes, clearMarkers, adjustMap, setNoGeoCenter, setGeoCenter, loadData, findLocation, mpos, loadMarkers, haversine, getMapIcons, getViewPortWidth;
var MapMarkers = new Array();
var MapRegions = new Array();
var locationids = new Array();
var radius = 80467.2;  /* 50 miles */
var centerMarker = new google.maps.Marker();
var locationcount = 9;
var curtlocation = new google.maps.LatLng(44.7946, -91.4107);
var regional = false;
var regionsLoaded = false;

initialize = function () {
    MapMarkers = new Array();
    locationids = new Array();
    var options = {
        center: curtlocation,
        mapTypeId: google.maps.MapTypeId.TERRAIN,
        fillOpacity: 0,
        strokeOpacity: 0,
        zoom: 8
    };
    map = new google.maps.Map(document.getElementById('map_canvas'), options);
    if ($.trim($('#dealer-location').val()) != "" && $.trim($('#dealer-location').val()) != "Search For Locations") {
        findLocation();
    } else {
        setCenter();
    };
    google.maps.event.addListener(map, 'idle', function () {
        loadData(map.getBounds());
    });
}

setCenter = function () {
    if (Modernizr.geolocation) {
        navigator.geolocation.getCurrentPosition(setGeoCenter, setNoGeoCenter);
    } else {
        // No native support, falling back
        setNoGeoCenter();
    }
}

setNoGeoCenter = function () {
    map.setCenter(curtlocation);
    map.setZoom(8);
/*    var circleOptions = {
        center: initialLocation,
        fillOpacity: 0,
        strokeOpacity: 0,
        map: map,
        zoom
    };
    var myCircle = new google.maps.Circle(circleOptions);
    map.fitBounds(myCircle.getBounds());*/
}

setGeoCenter = function (position) {
    var initialLocation = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
    map.setCenter(initialLocation);
    var circleOptions = {
        center: initialLocation,
        fillOpacity: 0,
        strokeOpacity: 0,
        map: map,
        radius: radius
    };
    var myCircle = new google.maps.Circle(circleOptions);
    map.fitBounds(myCircle.getBounds());
}

loadData = function (bounds) {
    var bndsurl = bounds.toUrlValue();
    if (centerMarker.position == undefined) {
        var center = map.getCenter().toUrlValue();
    } else {
        var center = centerMarker.position.toUrlValue();
    }
    if (getViewPortWidth(bndsurl) < 1000) {
        hideRegions();
        showMarkers();
        if (regional) {
            // region view is showing, toggle to local view
            regional = false;
        }
        $.getJSON('/WhereToBuy/getLocalDealersJSON?latlng=' + bndsurl + '&center=' + center, function (data) {
            loadMarkers(data);
        });
    } else {
        clearMarkers();
        if (!regional) {
            regional = true;
        }
        if (!regionsLoaded) {
            $.getJSON('/WhereToBuy/getLocalRegionsJSON', function (data) {
                loadRegions(data);
                regionsLoaded = true;
            });
        } else {
            showRegions();
        }
    }
}

mpos = function (key, value) {
    this.key = key;
    this.value = value;
}

getMapIcons = function (location) {
    var icons;
    $(location.dealerType.MapIcons).each(function (i, icon) {
        if (location.dealerTier.ID == icon.tier) {
            icons = icon;
        }
    });
    return icons;
}

getTierIndex = function(id) {
    var index = -1;
    $(MapMarkers).each(function (i, obj) {
        if (obj.tier == id) {
            index = i;
        }
    });
    return index;
}

function dealerGroup(tier, icon, iconshadow, name, markers, status) {
    this.tier = tier;
    this.icon = icon;
    this.iconshadow = iconshadow;
    this.name = name;
    this.markers = markers;
    this.status = status;
    this.hide = function () {
        this.status = 'hidden';
        $('#toggle_' + this.tier).attr('checked', false);
        $('#togglebox_' + this.tier + ' span').removeClass('active');
        $(this.markers).each(function (i, marker) {
            marker.marker.setMap(null);
        })
    };
    this.show = function () {
        this.status = 'visible';
        $('#toggle_' + this.tier).attr('checked', true);
        $('#togglebox_' + this.tier + ' span').addClass('active');
        $(this.markers).each(function (i, marker) {
            marker.marker.setMap(map);
        })
    };
    this.toggle = function () {
        if (this.status == 'visible') {
            this.hide();
        } else {
            this.show();
        }
    }
    this.getIndex = function (locationid) {
        var index = -1;
        $(this.markers).each(function (i, marker) {
            if (locationid == marker.locationid) {
                index = i;
            }
        });
        return index;
    }
}

function marker (locationid, marker, infobox, dealerbox, name, address, city, state, postalCode, phone, distance) {
    this.locationid = locationid;
    this.marker = marker;
    this.infobox = infobox;
    this.dealerbox = dealerbox;
    this.name = name;
    this.address = address;
    this.city = city;
    this.state = state;
    this.postalCode = postalCode;
    this.phone = phone;
    this.distance = distance;
}

function region(stateID, name, abbr, polygons, marker, bounds) {
    this.stateID = stateID;
    this.name = name;
    this.abbr = abbr;
    this.polygons = polygons;
    this.marker = marker;
    this.bounds = bounds;
}



loadMarkers = function (data) {
    $('#local .dealer').remove();
    $('#local .morecount').remove();
    $('#local .nodealers').remove();
    var animType = google.maps.Animation.DROP;
    var loccount = new Object();
    $(data).each(function (i, location) {
        var locid = Number(location.locationID);
        var latitude = location.latitude;
        var longitude = location.longitude;
        var tierindex = getTierIndex(location.dealerTier.ID);
        if (latitude != "" && longitude != "" && $.inArray(locid, locationids) == -1) {
            locationids.push(locid);
            var name = location.name;
            var loc_latlng = new google.maps.LatLng(latitude, longitude);
            var contentString = '<div id="content"><div id="siteNotice"></div><p style="font-size: 12px;"><strong>' + name + '</strong><br>';
            contentString += location.address + '<br>' + location.city + ', ' + ((location.State == null) ? "" : location.State.abbr) + ' ' + location.postalCode + '<br>';
            contentString += location.phone;
            contentString += '<br /><a class="getDirections" data-name="' + name + '" data-id="' + location.locationID + '" href="/WhereToBuy/Directions/' + location.locid + '">Get Directions</a></p></div>';
            var infoobj = new google.maps.InfoWindow({
                content: contentString
            });
            var newdealer = document.createElement('div');
            $(newdealer).attr('id', 'location_' + location.locationID);
            $(newdealer).attr('class', 'dealer');
            $(newdealer).attr('style', 'display: none;');
            $(newdealer).attr('data-tier', location.dealerTier.ID);
            if (i >= 50) animType = "";
            var mapicon = "";
            var shadow = "";
            $(location.dealerType.MapIcons).each(function (x, icons) {
                if (icons.tier == location.dealerTier.ID) {
                    mapicon = icons.mapicon1;
                    shadow = icons.mapiconshadow;
                }
            });
            var dealerTier = MapMarkers[tierindex];
            if (tierindex == -1) {
                var icons = getMapIcons(location);
                var dealerTier = new dealerGroup(
                    location.dealerTier.ID,
                    icons.mapicon1,
                    icons.mapiconshadow,
                    location.dealerTier.tier + ' ' + location.dealerType.label,
                    [],
                    'visible'
                );
                MapMarkers.push(dealerTier);
                tierindex = getTierIndex(location.dealerTier.ID);
            }

            var tempmarker = new marker(
                locid,
                new google.maps.Marker({
                    position: loc_latlng,
                    map: (dealerTier.status == 'visible') ? map : null,
                    title: name,
                    icon: mapicon,
                    shadow: shadow,
                    animation: animType
                }),
                infoobj,
                newdealer,
                location.name,
                location.address,
                location.city,
                ((location.State == null) ? "" : location.State.abbr),
                location.postalCode,
                location.phone,
                location.distance.toFixed(2)
            );
            var markerindex = MapMarkers[tierindex].markers.push(tempmarker);
            markerindex--;
            var markerobj = MapMarkers[tierindex].markers[markerindex].marker;
            var locationnumber = locid;
            google.maps.event.addListener(markerobj, 'click', function () {
                clearInfoBoxes();
                infoobj.open(map, markerobj);
                $('#location_' + locationnumber).addClass('active');
            });
        }
        if (location.dealerTier.ID in loccount) {
            loccount[location.dealerTier.ID]++;
        } else {
            loccount[location.dealerTier.ID] = 1;
        }
        var clearspan = $('#dealertype_' + location.dealerTier.ID + ' #clearspan_' + location.dealerTier.ID);
        $(clearspan).before(MapMarkers[tierindex].markers[MapMarkers[tierindex].getIndex(locid)].dealerbox);
        //markerarray[MarkerPositions[location.locationID]].distance = location.distance.toFixed(2);
        $('#location_' + location.locationID).html('<p><strong>' + location.name + '</strong><br>' + location.address + '<br>' + location.city + ', ' + ((location.State == null) ? "" : location.State.abbr) + ' ' + location.postalCode + '<br>' + location.phone + '<br>Distance: <span id="distance_' + location.locationID + '">' + location.distance.toFixed(2) + '</span> Miles<br><a class="getDirections" data-name="' + location.name + '" data-id="' + location.locationID + '" href="/WhereToBuy/Directions/' + location.locationID + '">Get Directions</a></p>');
        //$('#distance_' + location.locationID).html(markerarray[MarkerPositions[location.locationID]].distance);
    });
    $('#local .dealers').each(function () {
        var typeid = $(this).attr('id').split('_')[1];
        var dealers = $(this).find('.dealer');
        if ($(dealers).length == 0) {
            $(this).find('span.clear').before('<p class="nodealers">No Dealers were found in this area.  Zoom out or search again for more dealers.</p>');
        } else {
            if ($(dealers).length > locationcount) {
                $.each(dealers, function (i, dealer) {
                    if (i < locationcount) {
                        $(dealer).fadeIn();
                    } else {
                        $(dealer).hide();
                    }
                });
                var clearspan = $('#clearspan_' + typeid);
                $(clearspan).before('<p class="morecount">And ' + ($(dealers).length - locationcount) + ' More...</p>')

            } else {
                $.each(dealers, function (i, dealer) {
                    $(dealer).fadeIn();
                });
            }
        }
    });
    $('#tab_container').height($('#local').outerHeight());
    $('#tab_scroll').height($('#local').outerHeight());
}

loadRegions = function (data) {
    $(data).each(function (i, stateobj) {
        var polygons = new Array();
        var tempbounds = new google.maps.LatLngBounds();
        var markerobj = new google.maps.Marker();
        $(stateobj.polygons).each(function (x, polygonobj) {
            var coords = new Array();
            $(polygonobj.MapPolygonCoordinates).each(function (z, coord) {
                coords.push(new google.maps.LatLng(coord.latitude, coord.longitude));
                tempbounds.extend(new google.maps.LatLng(coord.latitude, coord.longitude));
            });
            var poly = new google.maps.Polygon({
                fillColor: '#ec5526',
                fillOpacity: 0.3,
                strokeColor: '#ec5526',
                strokeOpacity: 0.6,
                strokeWeight: 1,
                geodesic: false,
                paths: coords
            });
            poly.setMap(map);
            google.maps.event.addListener(poly, 'click', function () {
                fitRegion(poly);
            });
            polygons.push(poly);
        });
        if (polygons.length > 0) {
            var markerobj = new google.maps.Marker({
                position: tempbounds.getCenter(),
                map: map,
                icon: 'http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=' + stateobj.count + '|ec5526|000000',
                animation: google.maps.Animation.DROP
            });
        }
        var regionobj = new region(stateobj.stateID, stateobj.name, stateobj.abbr, polygons, markerobj, tempbounds);
        MapRegions.push(regionobj);
    });
};

fitRegion = function (poly) {
    var tempbounds = new google.maps.LatLngBounds();
    var paths = poly.getPaths();
    for (var p = 0; p < paths.getLength(); p++) {
        path = paths.getAt(p);
        for (var i = 0; i < path.getLength(); i++) {
            tempbounds.extend(path.getAt(i));
        }
    }
    map.setCenter(tempbounds.getCenter());
    map.setZoom(7);
}

haversine = function (lat1,lon1) {
    if (centerMarker.position == undefined) {
        var center = map.getCenter();
    } else {
        var center = centerMarker.position;
    }
    var lat2 = center.lat();
    var lon2 = center.lng();
    var R = 3963.1676;
    var dlat = (lat2 - lat1) * (Math.PI / 180);
    var dlon = (lon2 - lon1) * (Math.PI / 180);
    var lat1 = lat1 * (Math.PI / 180);
    var lat2 = lat2 * (Math.PI / 180);

    var a = (Math.sin(dlat / 2) * Math.sin(dlat / 2)) + ((Math.sin(dlon / 2)) * (Math.sin(dlon / 2))) * Math.cos(lat1) * Math.cos(lat2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    return (R * c).toFixed(2);
}

findLocation = function () {
    var loc = $('#dealer-location').val();
    $.getJSON('/WhereToBuy/searchLocations?search=' + loc, function (data) {
        if (data.length == 0) {
            geocoder = new google.maps.Geocoder();
            geocoder.geocode({ 'address': loc }, function (results, status) { adjustMap(results, status); });
        } else {
            var newbounds = new google.maps.LatLngBounds();
            $.each(data, function (i, location) {
                var loc_latlng = new google.maps.LatLng(location.latitude, location.longitude);
                newbounds.extend(loc_latlng);
            });
            map.fitBounds(newbounds);
        }
    });
};

adjustMap = function (results, status) {
    centerMarker.setMap(null);
    if (status == google.maps.GeocoderStatus.OK) {
        centerMarker = new google.maps.Marker({
            position: results[0].geometry.location,
            map: map,
            title: results[0].formatted_address,
            animation: google.maps.Animation.DROP
        })
        map.setCenter(results[0].geometry.location);
        map.fitBounds(results[0].geometry.viewport);
    } else {
        alert('We were unable to find a location that matches that name');
    }
}

clearMarkers = function () {
    $(MapMarkers).each(function (i, dealertier) {
        $(dealertier.markers).each(function (x , marker) {
            marker.marker.setMap(null);
        });
    });
}

showMarkers = function () {
    $(MapMarkers).each(function (i, dealertier) {
        $(dealertier.markers).each(function (x, marker) {
            marker.marker.setMap(map);
        });
    });
}

showRegions = function () {
    $(MapRegions).each(function (i, regionobj) {
        $(regionobj.polygons).each(function (x, poly) {
            poly.setMap(map);
        });
        regionobj.marker.setMap(map);
    });
}

hideRegions = function () {
    $(MapRegions).each(function (i, regionobj) {
        $(regionobj.polygons).each(function (x, poly) {
            poly.setMap(null);
        });
        regionobj.marker.setMap(null);
    });
}

clearInfoBoxes = function () {
    $(MapMarkers).each(function (i, dealertier) {
        $(dealertier.markers).each(function (x, marker) {
            marker.infobox.close();
            $(marker.dealerbox).removeClass('active');
        });
    });
}

showDirections = function (event) {
    event.preventDefault();
    $('body').append('<div id="site_overlay"></div>');
    $('body').append('<div id="site_overlay_container"></div>');
    $('#site_overlay_container').append('<div id="directions"></div>');
    $('#directions').css({ 'height': ($(window).height() - 200) + 'px', 'width': ($(window).width() - 200) + 'px' });
    $('#directions').append('<span id="directionsclose" title="Close Directions Box">&times;</span>');
    $('#directions').append('<h2>Get Directions</h2>');
    $('#directions').append('<div id="directionsContent"></div>');
    $('#directionsContent').append('<div id="directions_mapcont"><p class="loading">Loading Map...<img src="/Content/img/ajax-loader.gif" alt="loading..." /></p><div id="directions_map"></div></div>');
    var formstr = '<div id="directionsformcont"><p>Enter your Address</p><form id="directionsform" action="/WhereToBuy/GetDirections" method="post">';
    formstr += '<label for="directionsstreet"><span>Street</span> <input type="text" id="directionsstreet" value="" placeholder="Enter Street" /></label>';
    formstr += '<label for="directionscitystzip"><span>City, State Zip</span> <input type="text" id="directionscitystzip" value="" placeholder="Enter City, State Zip" /></label>';
    formstr += '<input type="submit" id="directionssubmit" class="button" value="Get Directions" /></form></div>';
    $('#directionsContent').append(formstr);
    $('#directionsContent').append('<input type="button" id="changeDirections" class="button" value="Change Address" style="display: none;" />');
    $('#directionsContent').append('<div id="directionsPanel"></div>');
    $('#directions').css({ 'margin': 'auto', 'top': (($(window).height() - $('#directions').outerHeight()) / 2) + "px", 'display': 'none' });
    $('#site_overlay').fadeTo('fast', .75);
    $('#site_overlay_container').fadeTo('fast', 1);
    $('#directions').fadeTo('fast', 1);
    var locname = $(this).data('name');
    var analyticstr = 'Get Directions for ' + locname;
    if (typeof (_gaq) != 'undefined') {
        _gaq.push(['_trackEvent', 'WhereToBuy', 'Clicked Get Directions', analyticstr]);
    }
    $.getJSON('/WhereToBuy/Directions', { 'id': $(this).data('id') }, function (data) {
        $('#directions_mapcont p.loading').hide();
        $('#directions h2').html('Get Directions to ' + data.name);
        $('#directions_map').css('height', $('#directions_map').width());
        $('#directions_map').data('latitude', data.latitude);
        $('#directions_map').data('longitude', data.longitude);
        $('#directions_map').data('address', data.address + ', ' + data.city + ', ' + ((data.state == null) ? "" : data.state.abbr) + ' ' + data.postalCode);
        var endlocation = new google.maps.LatLng(data.latitude, data.longitude);
        var directionsoptions = {
            center: endlocation,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            zoom: 12
        };
        directionsmap = new google.maps.Map(document.getElementById('directions_map'), directionsoptions);
        var contentString = '<p style="font-size: 12px; text-align: left;"><strong>' + data.name + '</strong><br>';
        contentString += data.address + '<br>' + data.city + ', ' + ((data.state == null) ? "" : data.state.abbr) + ' ' + data.postalCode + '<br>';
        contentString += data.phone + '</p>';
        directionsInfo = new google.maps.InfoWindow({
            content: contentString
        });
        directionsmarker = new google.maps.Marker({
            position: endlocation,
            map: directionsmap,
            title: data.name,
            animation: google.maps.Animation.DROP
        })
        directionsInfo.open(directionsmap, directionsmarker);
    });
    $('#directionsclose').click(function (event) { removeDirections(); });
    $('body').on({ keyup: function (event) {
        if (event.keyCode == 27) {
            removeDirections();
        } else {
            event.preventDefault();
        }
    }
    });
}

loadDirections = function (event) {
    event.preventDefault();
    var directionsRenderer = new google.maps.DirectionsRenderer();
    directionsRenderer.setMap(directionsmap);
    directionsRenderer.setPanel(document.getElementById('directionsPanel'));
    var targetlat = $('#directions_map').data('latitude');
    var targetlong = $('#directions_map').data('longitude');
    var targetloc = new google.maps.LatLng(targetlat, targetlong);
    var directionsService = new google.maps.DirectionsService();
    var request = {
        origin: $('#directionsstreet').val() + ' ' + $('#directionscitystzip').val(),
        destination: $('#directions_map').data('address'),
        travelMode: google.maps.DirectionsTravelMode.DRIVING,
        unitSystem: google.maps.DirectionsUnitSystem.IMPERIAL,
        provideRouteAlternatives: true
    };
    directionsService.route(request, function (response, status) {
        if (status == google.maps.DirectionsStatus.OK) {
            directionsRenderer.setMap(null);
            directionsRenderer.setMap(directionsmap);
            directionsmarker.setMap(null);
            $('#directionsPanel').empty();
            directionsRenderer.setDirections(response);
            $('#directionsformcont').slideUp();
            $('#changeDirections').fadeIn();
        } else {
            alert('Error: ' + status);
        }
    });
};

removeDirections = function () {
    $('#site_overlay').remove();
    $('#site_overlay_container').remove();
    $('#directions').remove();
    $('body').off('keyup');
    $(window).off('resize');
}

getViewPortWidth = function (bndsstring) {
    var bndsplit = bndsstring.split(',');
    var lat1, lon1, lat2, lon2;
    lat1 = Number(bndsplit[0]);
    lon1 = Number(bndsplit[1]);
    lat2 = Number(bndsplit[2]);
    lon2 = Number(bndsplit[3]);
    var earth = 3963.1676; // radius of Earth in miles
    var dlat = (lat2 - lat1) * (Math.PI / 180);
    var dlon = (lon2 - lon1) * (Math.PI / 180);
    lat1 = lat1 * (Math.PI / 180);
    lat2 = lat2 * (Math.PI / 180);

    var a = (Math.sin(dlat / 2) * Math.sin(dlat / 2)) + ((Math.sin(dlon / 2)) * (Math.sin(dlon / 2))) * Math.cos(lat1) * Math.cos(lat2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var distance = earth * c;
    return distance;
}


$(window).load(function () { adjustTabSize() });

$(function () {
    $(window).on("tabcallback", function (e) {
        if (getCurrentTab() == 'local' && map == undefined) {
            initialize();
        }
    });
    $(document).on('click', 'a.getDirections', showDirections);
    $(document).on('click', '#changeDirections', function (event) { $('#directionsformcont').slideDown(); $('#changeDirections').hide(); });
    $('#map_canvas').css('height', '500px');
    $("#searchmap").submit(function (event) {
        event.preventDefault();
        if (getCurrentTab() != 'local') {
            currentTab = 'local';
            pushLocation(currentTab);
        }
        findLocation();
    });

    $(document).on('submit', '#directionsform', loadDirections);
    $('#toggles .toggle input').hide();

    $("#dealer-clear").click(function (event) {
        $('#dealer-location').val('');
        centerMarker.setMap(null);
        centerMarker.setPosition(null);
        setCenter();
        loadData(map.getBounds());
    });

    $(document).on('click', "#local .dealer", function () {
        var locid = $(this).attr('id').split('_')[1];
        var tier = $(this).data('tier');
        var tierindex = getTierIndex(tier);
        var markerindex = MapMarkers[tierindex].getIndex(locid);
        if (MapMarkers[tierindex].status == 'visible') {
            google.maps.event.trigger(MapMarkers[tierindex].markers[markerindex].marker, 'click');
        }
    });

    $(document).on('click','.dealer a.onlinedealer', function () {
        var locname = $(this).data('name');
        var analyticstr = 'Clicked Online Dealer ' + locname;
        if (typeof (_gaq) != 'undefined') {
            _gaq.push(['_trackEvent', 'WhereToBuy', 'Clicked Online Dealer', analyticstr]);
        }
    });

    $(document).on('click', '#toggles .toggle input', function (e) {
        var dealerid = $(this).val();
        MapMarkers[getTierIndex(dealerid)].toggle();
    });

    $(document).on('click', '#toggles .toggle .togglebox', function (e) {
        var dealerid = $(this).data("id");
        MapMarkers[getTierIndex(dealerid)].toggle();
    });
});