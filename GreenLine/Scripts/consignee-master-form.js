    var initConsigneeMaster = function (DomainName, isPartial) {
        var formId = 'form_consignee_modal';
        var $form = $('#' + formId);
        if ($form.data('glInitialized')) return; $form.data('glInitialized', true);
        var saveBtnId = isPartial ? "#glFormSaveBtn" : "#btnSave";

        var Formvalidate = function () {
            var form1 = $form;
            var error1 = $('.alert-danger', form1);
            var success1 = $('.alert-success', form1);
            form1.validate({
                doNotHideMessage: true,
                errorElement: 'span',
                errorClass: 'help-block',
                focusInvalid: false,
                rules: {},
                messages: {},
                errorPlacement: function (error, element) {
                    error.insertAfter(element);
                },
                invalidHandler: function (event, validator) {
                    success1.hide();
                    error1.show();
                },
                highlight: function (element) {
                    $(element).closest('.form-group').addClass('has-error');
                },
                unhighlight: function (element) {
                    $(element).closest('.form-group').removeClass('has-error');
                },
                success: function (label) {
                    label.closest('.form-group').removeClass('has-error');
                },
                submitHandler: function (form) {
                    success1.show();
                    error1.hide();
                }
            });
        };

        var map;
        var directionsService;
        var directionsRenderer;
        var geocoder;
        var pinpointMarker = null;
        var polygon = null;
        var circle = null;
        var drawingManager = null;
        var autocomplete = null;

        function loadmapOnCreate() {
            var defaultLat = 21.2140032;
            var defaultLong = 72.843264;

            var myCenter = new google.maps.LatLng(defaultLat, defaultLong);
            var mapProp = {
                center: myCenter,
                zoom: 12
            };

            var mapDiv = $form.find("#googleMap")[0] || document.getElementById("googleMap");
            if (!mapDiv) return;

            map = new google.maps.Map(mapDiv, mapProp);
            directionsService = new google.maps.DirectionsService();
            directionsRenderer = new google.maps.DirectionsRenderer({
                map: map
            });
            geocoder = new google.maps.Geocoder();

            // Resize map trigger function to prevent map collapsing on hidden/fade-in containers
            function triggerMapResize() {
                if (map) {
                    google.maps.event.trigger(map, 'resize');
                    var latVal = $form.find("#Latitude").val();
                    var lngVal = $form.find("#Longitude").val();
                    if (latVal && lngVal && !isNaN(parseFloat(latVal)) && !isNaN(parseFloat(lngVal))) {
                        var markerLatLng = new google.maps.LatLng(parseFloat(latVal), parseFloat(lngVal));
                        map.setCenter(markerLatLng);
                    } else {
                        map.setCenter(myCenter);
                    }
                }
            }

            // 1. Trigger resize when the modal has finished fading in (.off ensures no duplicate bindings on re-open)
            $('#glFormModal').off('shown.bs.modal.consigneeMap').on('shown.bs.modal.consigneeMap', function () {
                triggerMapResize();
            });

            // 2. Trigger deferred resizes to handle fast AJAX rendering and Bootstrap transition timing
            setTimeout(triggerMapResize, 500);
            setTimeout(triggerMapResize, 1200);

            var latVal = $form.find("#Latitude").val();
            var lngVal = $form.find("#Longitude").val();
            if (latVal && lngVal && !isNaN(parseFloat(latVal)) && !isNaN(parseFloat(lngVal))) {
                var markerLatLng = new google.maps.LatLng(parseFloat(latVal), parseFloat(lngVal));
                pinpointMarker = new google.maps.Marker({
                    position: markerLatLng,
                    draggable: false,
                    map: map
                });
                map.setCenter(markerLatLng);
                map.setZoom(15);
            }

            drawingManager = new google.maps.drawing.DrawingManager({
                drawingMode: null,
                drawingControl: true,
                drawingControlOptions: {
                    position: google.maps.ControlPosition.TOP_CENTER,
                    drawingModes: ['polygon', 'circle']
                },
                polygonOptions: {
                    editable: true,
                    draggable: true,
                    fillColor: '#FF0000',
                    fillOpacity: 0.3,
                    strokeColor: '#FF0000',
                    strokeOpacity: 0.8,
                    strokeWeight: 2
                },
                circleOptions: {
                    editable: true,
                    draggable: true,
                    fillColor: '#FF0000',
                    fillOpacity: 0.3,
                    strokeColor: '#FF0000',
                    strokeOpacity: 0.8,
                    strokeWeight: 2
                }
            });
            drawingManager.setMap(map);

            google.maps.event.addListener(drawingManager, 'polygoncomplete', function (newPolygon) {
                clearExistingGeofence();
                polygon = newPolygon;
                $form.find("#Radius").val('');
                attachPolygonListeners(polygon);
                updatePolygonData();
                drawingManager.setDrawingMode(null);
                drawingManager.setOptions({ drawingControl: false });
            });

            google.maps.event.addListener(drawingManager, 'circlecomplete', function (newCircle) {
                clearExistingGeofence();
                circle = newCircle;
                attachCircleListeners(circle);
                enforceCircleRules(circle);
                updateCircleData();
                drawingManager.setDrawingMode(null);
                drawingManager.setOptions({ drawingControl: false });
            });

            var geoLatVal = $form.find("#GeoFenceLatitude").val();
            var geoLngVal = $form.find("#GeoFenceLongitude").val();
            var radiusVal = $form.find("#Radius").val();

            if (geoLatVal && geoLngVal) {
                if (geoLatVal.indexOf(',') > -1) {
                    var coordinates = parseCoordinates(geoLatVal, geoLngVal);
                    polygon = new google.maps.Polygon({
                        paths: coordinates,
                        editable: true,
                        draggable: true,
                        fillColor: '#FF0000',
                        fillOpacity: 0.3,
                        strokeColor: '#FF0000',
                        strokeOpacity: 0.8,
                        strokeWeight: 2,
                        map: map
                    });
                    attachPolygonListeners(polygon);
                    drawingManager.setOptions({ drawingControl: false });
                } else {
                    var radiusFloat = parseFloat(radiusVal);
                    if (!isNaN(radiusFloat) && radiusFloat > 0) {
                        circle = new google.maps.Circle({
                            strokeColor: '#FF0000',
                            strokeOpacity: 0.8,
                            strokeWeight: 2,
                            fillColor: '#FF0000',
                            fillOpacity: 0.3,
                            map: map,
                            center: new google.maps.LatLng(parseFloat(geoLatVal), parseFloat(geoLngVal)),
                            radius: radiusFloat,
                            editable: true,
                            draggable: true
                        });
                        attachCircleListeners(circle);
                        drawingManager.setOptions({ drawingControl: false });
                    }
                }
            }

            createRemoveGeofenceControl();
        }

        function parseCoordinates(latitudeString, longitudeString) {
            var latitudes = latitudeString.split(',');
            var longitudes = longitudeString.split(',');
            var coordinates = [];
            for (var i = 0; i < latitudes.length; i++) {
                coordinates.push(new google.maps.LatLng(parseFloat(latitudes[i]), parseFloat(longitudes[i])));
            }
            return coordinates;
        }

        function attachPolygonListeners(poly) {
            var path = poly.getPath();
            google.maps.event.addListener(path, 'set_at', function() { updatePolygonData(); });
            google.maps.event.addListener(path, 'insert_at', function() { updatePolygonData(); });
            google.maps.event.addListener(path, 'remove_at', function() { updatePolygonData(); });
            google.maps.event.addListener(poly, 'dragend', function() { updatePolygonData(); });
        }

        var lastToastTime = 0;
        function showGeofenceToast(type, message, title, force) {
            var now = Date.now();
            if (force || (now - lastToastTime > 2000)) {
                if (window.toastr) {
                    toastr.clear();
                }
                TosterNotification(type, message, title);
                lastToastTime = now;
            }
        }

        var isCorrectingCircle = false;
        function enforceCircleRules(circ) {
            if (!circ || isCorrectingCircle) return;

            var latVal = $form.find("#Latitude").val();
            var lngVal = $form.find("#Longitude").val();
            if (!latVal || !lngVal) {
                return;
            }

            var pLat = parseFloat(latVal);
            var pLng = parseFloat(lngVal);
            if (isNaN(pLat) || isNaN(pLng)) {
                return;
            }

            var center = circ.getCenter();
            var radius = circ.getRadius();
            var pinpoint = new google.maps.LatLng(pLat, pLng);

            var distance = 0;
            if (google.maps.geometry && google.maps.geometry.spherical) {
                distance = google.maps.geometry.spherical.computeDistanceBetween(center, pinpoint);
            } else {
                var R = 6371000;
                var dLat = (pLat - center.lat()) * Math.PI / 180;
                var dLon = (pLng - center.lng()) * Math.PI / 180;
                var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                        Math.cos(center.lat() * Math.PI / 180) * Math.cos(pLat * Math.PI / 180) *
                        Math.sin(dLon / 2) * Math.sin(dLon / 2);
                var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
                distance = R * c;
            }

            var isOutside = (distance > radius);
            var exceedsLimit = (radius > 5000);

            if (isOutside || exceedsLimit) {
                isCorrectingCircle = true;
                circ.setCenter(pinpoint);
                circ.setRadius(5000);
                isCorrectingCircle = false;

                showGeofenceToast("error", "Circle geofence has been reset to 5 km centered on the pinpoint location.", "Validation Warning", false);

                $form.find("#GeoFenceLatitude").val(pLat);
                $form.find("#GeoFenceLongitude").val(pLng);
                $form.find("#Radius").val("5000.00");

                checkAndClearGeofenceError();
            }
        }

        function attachCircleListeners(circ) {
            google.maps.event.addListener(circ, 'radius_changed', function() {
                if (isCorrectingCircle) return;
                enforceCircleRules(circ);
                updateCircleData();
            });
            google.maps.event.addListener(circ, 'center_changed', function() {
                if (isCorrectingCircle) return;
                enforceCircleRules(circ);
                updateCircleData();
            });
            google.maps.event.addListener(circ, 'dragend', function() {
                if (isCorrectingCircle) return;
                enforceCircleRules(circ);
                updateCircleData();
            });
        }

        function updatePolygonData() {
            if (!polygon) return;
            var path = polygon.getPath();
            var latitudes = [];
            var longitudes = [];
            path.forEach(function (vertex) {
                latitudes.push(vertex.lat());
                longitudes.push(vertex.lng());
            });
            $form.find("#GeoFenceLatitude").val(latitudes.join(','));
            $form.find("#GeoFenceLongitude").val(longitudes.join(','));
            $form.find("#Radius").val('');
            checkAndClearGeofenceError();
        }

        function updateCircleData() {
            if (!circle) return;
            var center = circle.getCenter();
            var radius = circle.getRadius();
            $form.find("#GeoFenceLatitude").val(center.lat());
            $form.find("#GeoFenceLongitude").val(center.lng());
            $form.find("#Radius").val(radius.toFixed(2));
            checkAndClearGeofenceError();
        }

        function clearExistingGeofence() {
            if (polygon) {
                polygon.setMap(null);
                polygon = null;
            }
            if (circle) {
                circle.setMap(null);
                circle = null;
            }
        }

        function createRemoveGeofenceControl() {
            var controlDiv = document.createElement('div');
            var controlUI = document.createElement('div');
            controlUI.style.backgroundColor = '#fff';
            controlUI.style.border = '1px solid #ccc';
            controlUI.style.borderRadius = '3px';
            controlUI.style.boxShadow = '0 2px 6px rgba(0,0,0,.3)';
            controlUI.style.cursor = 'pointer';
            controlUI.style.marginTop = '10px';
            controlUI.style.marginRight = '10px';
            controlUI.style.padding = '8px 12px';
            controlUI.title = 'Click to remove geofence';
            controlDiv.appendChild(controlUI);

            var controlText = document.createElement('div');
            controlText.style.color = '#333';
            controlText.style.fontFamily = 'Roboto,Arial,sans-serif';
            controlText.style.fontSize = '12px';
            controlText.style.fontWeight = 'bold';
            controlText.innerHTML = 'Remove Geofence';
            controlUI.appendChild(controlText);

            controlUI.addEventListener('click', function () {
                clearExistingGeofence();
                $form.find("#GeoFenceLatitude").val('');
                $form.find("#GeoFenceLongitude").val('');
                $form.find("#Radius").val('');
                if (drawingManager) {
                    drawingManager.setOptions({ drawingControl: true });
                }
                checkAndClearGeofenceError();
            });

            map.controls[google.maps.ControlPosition.TOP_RIGHT].push(controlDiv);
        }

        function isCoordinateInsideGeofence(latVal, lngVal, geoLatVal, geoLngVal, radiusVal) {
            if (typeof google === 'undefined' || typeof google.maps === 'undefined') {
                return true;
            }
            if (!latVal || !lngVal || !geoLatVal || !geoLngVal) {
                return true;
            }
            var pLat = parseFloat(latVal);
            var pLng = parseFloat(lngVal);
            if (isNaN(pLat) || isNaN(pLng)) {
                return true;
            }

            var pinpoint = new google.maps.LatLng(pLat, pLng);
            var isInside = true;

            if (geoLatVal.indexOf(',') > -1) {
                var lats = geoLatVal.split(',');
                var lngs = geoLngVal.split(',');
                var coords = [];
                for (var i = 0; i < lats.length; i++) {
                    var flat = parseFloat(lats[i]);
                    var flng = parseFloat(lngs[i]);
                    if (!isNaN(flat) && !isNaN(flng)) {
                        coords.push(new google.maps.LatLng(flat, flng));
                    }
                }

                if (coords.length > 0) {
                    var poly = new google.maps.Polygon({ paths: coords });
                    if (google.maps.geometry && google.maps.geometry.poly) {
                        isInside = google.maps.geometry.poly.containsLocation(pinpoint, poly);
                    } else {
                        var x = pLng, y = pLat;
                        isInside = false;
                        for (var i = 0, j = coords.length - 1; i < coords.length; j = i++) {
                            var xi = coords[i].lng(), yi = coords[i].lat();
                            var xj = coords[j].lng(), yj = coords[j].lat();
                            var intersect = ((yi > y) !== (yj > y))
                                && (x < (xj - xi) * (y - yi) / (yj - yi) + xi);
                            if (intersect) isInside = !isInside;
                        }
                    }
                }
            } else {
                var cLat = parseFloat(geoLatVal);
                var cLng = parseFloat(geoLngVal);
                var radius = parseFloat(radiusVal);

                if (!isNaN(cLat) && !isNaN(cLng) && !isNaN(radius)) {
                    var center = new google.maps.LatLng(cLat, cLng);
                    var distance = 0;
                    if (google.maps.geometry && google.maps.geometry.spherical) {
                        distance = google.maps.geometry.spherical.computeDistanceBetween(center, pinpoint);
                    } else {
                        var R = 6371000;
                        var dLat = (pLat - cLat) * Math.PI / 180;
                        var dLon = (pLng - cLng) * Math.PI / 180;
                        var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                                Math.cos(cLat * Math.PI / 180) * Math.cos(pLat * Math.PI / 180) *
                                Math.sin(dLon / 2) * Math.sin(dLon / 2);
                        var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
                        distance = R * c;
                    }
                    isInside = (distance <= radius);
                }
            }

            return isInside;
        }

        function checkAndClearGeofenceError() {
            var latVal = $form.find("#Latitude").val();
            var lngVal = $form.find("#Longitude").val();
            var geoLatVal = $form.find("#GeoFenceLatitude").val();
            var geoLngVal = $form.find("#GeoFenceLongitude").val();
            var radiusVal = $form.find("#Radius").val();

            if (!geoLatVal || !geoLngVal || isCoordinateInsideGeofence(latVal, lngVal, geoLatVal, geoLngVal, radiusVal)) {
                $form.find("#Consignee_Location").closest('.form-group').removeClass('has-error');
                $form.find("#Consignee_Location").nextAll('.validation-error-msg').remove();
                $form.find("#googleMap").parent().css("border", "1px solid #e9ebec");
            }
        }

        function checkDuplicateGST(gstVal, callback) {
            if (!gstVal) {
                $form.find("#Consignee_GST").removeClass('is-invalid').closest('.form-group').removeClass('has-error');
                $form.find("#Consignee_GST").nextAll('.validation-error-msg').remove();
                if (callback) callback(false);
                return;
            }
            var excludeId = $form.find("#Id").val() || 0;
            var excludeCode = $form.find("#Consignee_Code").val() || "";
            $.ajax({
                url: DomainName + '/Master/CheckDuplicateGST',
                type: 'POST',
                data: { GSTNo: gstVal, ExcludeId: excludeId, ExcludeCode: excludeCode },
                success: function (response) {
                    if (response.isDuplicate) {
                        if (window.toastr) {
                            toastr.clear();
                        }
                        TosterNotification("error", 'Consignee GST No. <b>"' + gstVal + '"</b> already exists!!!', "Duplicate GST Error");
                        $form.find("#Consignee_GST").addClass('is-invalid').closest('.form-group').addClass('has-error');
                        $form.find("#Consignee_GST").nextAll('.validation-error-msg').remove();
                        $form.find("#Consignee_GST").after('<span class="validation-error-msg text-danger small mt-1 d-block">Consignee GST No. already exists!</span>');

                        if (callback) callback(true);
                    } else {
                        $form.find("#Consignee_GST").removeClass('is-invalid').closest('.form-group').removeClass('has-error');
                        $form.find("#Consignee_GST").nextAll('.validation-error-msg').remove();
                        if (callback) callback(false);
                    }
                },
                error: function () {
                    if (callback) callback(false);
                }
            });
        }

        // Initialize Form validate
        Formvalidate();

        // Select2 v3.x initialization (dropdownParent is NOT supported in v3 — z-index is handled via CSS .select2-drop override)
        // Guard: retry if Select2 JS hasn't finished loading yet (can happen on live under load)
        function initSelect2Fields() {
            if (typeof $.fn.select2 === 'undefined') {
                // Select2 not available yet — retry after 100ms (max 30 attempts = 3s)
                if ((initSelect2Fields._attempts = (initSelect2Fields._attempts || 0) + 1) <= 30) {
                    setTimeout(initSelect2Fields, 100);
                }
                return;
            }
            $('#form_consignee_modal .select2').each(function() {
                var $el = $(this);
                // Destroy any previous instance first to avoid duplicate widgets on re-open
                if ($el.data('select2')) {
                    $el.select2('destroy');
                }
                $el.select2({
                    placeholder: "Select an Option",
                    allowClear: true,
                    width: '100%'
                });
            });
        }
        initSelect2Fields();

        // Deferred Map Initialization helper
        var initMapIfGoogleLoaded = function() {
            if (typeof google !== 'undefined' && typeof google.maps !== 'undefined') {
                loadmapOnCreate();

                // Autocomplete
                var dropLocationInput = $form.find('#Consignee_Location')[0] || document.getElementById('Consignee_Location');
                if (dropLocationInput) {
                    autocomplete = new google.maps.places.Autocomplete(dropLocationInput);
                    autocomplete.addListener('place_changed', function () {
                        var place = autocomplete.getPlace();
                        if (!place.geometry || !place.geometry.location) {
                            return;
                        }

                        var lat = place.geometry.location.lat();
                        var lng = place.geometry.location.lng();

                        $form.find("#Latitude").val(lat);
                        $form.find("#Longitude").val(lng);

                        if (pinpointMarker) {
                            pinpointMarker.setPosition(place.geometry.location);
                        } else {
                            pinpointMarker = new google.maps.Marker({
                                position: place.geometry.location,
                                draggable: false,
                                map: map
                            });
                        }

                        map.setCenter(place.geometry.location);
                        map.setZoom(18);

                        if (circle) {
                            enforceCircleRules(circle);
                        }

                        checkAndClearGeofenceError();
                    });

                    $form.find('#Consignee_Location').on('keydown', function (e) {
                        if (e.keyCode === 13) {
                            e.preventDefault();
                            return false;
                        }
                    });
                }
            }
        };

        // Expose to window so maps load callback can invoke it
        window.triggerConsigneeMapInit = initMapIfGoogleLoaded;
        
        // Try invoking it immediately (in case google is already loaded)
        initMapIfGoogleLoaded();

        $form.find("#Consignee_GST").on("input", function () {
            $(this).removeClass('is-invalid').closest('.form-group').removeClass('has-error');
            $(this).nextAll('.validation-error-msg').remove();
        });

        $form.find("#Consignee_GST").on("change", function () {
            var gstVal = $(this).val();
            if (!gstVal) {
                $form.find("#Consignee_GST").removeClass('is-invalid').closest('.form-group').removeClass('has-error');
                $form.find("#Consignee_GST").nextAll('.validation-error-msg').remove();
                return;
            }

            App.blockUI({ boxed: true });
            checkDuplicateGST(gstVal, function (isDuplicate) {
                if (isDuplicate) {
                    App.unblockUI();
                    $form.find("#Consignee_GST").val("");
                    $form.find("#Consignee_PANNo").val("");
                    $form.find("#Consignee_Name").val("");
                    $form.find("#Consignee_Address").val("");
                    $form.find("#Consignee_Pincode").val(null).trigger("change");
                } else {
                    $.ajax({
                        url: DomainName + '/Master/GetGSTDetails?GSTNO=' + gstVal,
                        type: 'POST',
                        success: function (response) {
                            App.unblockUI();
                            if (response && response.Data && response.Data.length > 0) {
                                $form.find("#Consignee_PANNo").val(response.Data[0].PanNo);
                                $form.find("#Consignee_Name").val(response.Data[0].TradeName);
                                $form.find("#Consignee_Address").val(response.Data[0].CSGEAddress);
                                $form.find("#Consignee_Pincode").select2("data", { id: response.Data[0].PinCode, text: response.Data[0].PinCode });
                                $form.find("#Consignee_Pincode").change();
                            }
                        },
                        error: function (req, status, error) {
                            TosterNotification("error", 'Operation fail..!! There is some issue please try again or Contact your administrator for more detail.', "Oops..!!");
                            App.unblockUI();
                        }
                    });
                }
            });
        });

        $form.find("#Consignee_Pincode").on("change", function () {
            var Consignee_Pincode = $(this).val();
            if (!Consignee_Pincode) {
                $form.find("#Consignee_State").val(null).trigger("change");
                $form.find("#Consignee_City").val(null).trigger("change");
                $form.find("#Consignee_Country").val(null).trigger("change");
                return;
            }
            var strUrl = (window.DomainName || "") + "/Account/GetData";
            var parameters = {};
            parameters['Consignee_Pincode'] = Consignee_Pincode;
            $.ajax({
                url: strUrl,
                type: 'POST',
                dataType: "json",
                data: {
                    Method: 'GetCountry_state_City_onPincode',
                    parameters: parameters
                },
                success: function (result) {
                    if (result && result.length > 0) {
                        $form.find("#Consignee_State").select2("data", { id: result[0].StateCode, text: result[0].stnm });
                        $form.find("#Consignee_City").select2("data", { id: result[0].CityCode, text: result[0].CityName });
                        $form.find("#Consignee_Country").select2("data", { id: result[0].countrycd, text: result[0].CountryName });
                    }
                    App.unblockUI();
                },
                error: function (req, status, error) {
                    TosterNotification("error", 'Operation fail..!! There is some issue while update candidate status... Please try again later..', "Oops..!!");
                    App.unblockUI();
                }
            });
        });

        $(saveBtnId).off('click').on('click', function (e) {
            if (window.toastr) {
                toastr.clear();
            }
            App.blockUI({ boxed: true });
            if (!$form.valid()) {
                App.unblockUI();
                return;
            }

            var gstVal = $form.find("#Consignee_GST").val();
            checkDuplicateGST(gstVal, function (isDuplicate) {
                if (isDuplicate) {
                    App.unblockUI();
                    return;
                }

                $form.find("#Consignee_Location").closest('.form-group').removeClass('has-error');
                $form.find("#Consignee_Location").nextAll('.validation-error-msg').remove();
                $form.find("#googleMap").parent().css("border", "1px solid #e9ebec");

                var latVal = $form.find("#Latitude").val();
                var lngVal = $form.find("#Longitude").val();
                var geoLatVal = $form.find("#GeoFenceLatitude").val();
                var geoLngVal = $form.find("#GeoFenceLongitude").val();
                var radiusVal = $form.find("#Radius").val();

                // Skip maps/geofence validations if Google Maps API failed to load (offline/blocked)
                if (typeof google !== 'undefined' && typeof google.maps !== 'undefined') {
                    if (!latVal || !lngVal) {
                        App.unblockUI();
                        $form.find("#Consignee_Location").closest('.form-group').addClass('has-error');
                        $form.find("#Consignee_Location").after('<span class="validation-error-msg text-danger small mt-1 d-block">Please search and select unloading location from Google search results to place the pinpoint marker.</span>');
                        showGeofenceToast("error", "Please select location from Google autocomplete to place pinpoint marker.", "Validation Error", true);
                        return;
                    }

                    if (!geoLatVal || !geoLngVal) {
                        App.unblockUI();
                        $form.find("#googleMap").parent().css("border", "2px solid #ea5f5f");
                        showGeofenceToast("error", "Please draw a geofence (polygon or circle) enclosing the pinpoint marker.", "Validation Error", true);
                        return;
                    }

                    if (circle) {
                        var radiusFloat = circle.getRadius();
                        var isInside = isCoordinateInsideGeofence(latVal, lngVal, geoLatVal, geoLngVal, radiusVal);
                        if (radiusFloat > 5000 || !isInside) {
                            showGeofenceToast("error", "Circle geofence has been reset to 5 km centered on the pinpoint location.", "Validation Warning", true);
                            enforceCircleRules(circle);
                            App.unblockUI();
                            return;
                        }
                    } else {
                        var isInside = isCoordinateInsideGeofence(latVal, lngVal, geoLatVal, geoLngVal, radiusVal);
                        if (!isInside) {
                            App.unblockUI();
                            $form.find("#Consignee_Location").closest('.form-group').addClass('has-error');
                            if ($form.find("#Consignee_Location").nextAll('.validation-error-msg').length === 0) {
                                $form.find("#Consignee_Location").after('<span class="validation-error-msg text-danger small mt-1 d-block">Unloading location pinpoint marker must be inside the geofence area.</span>');
                            }
                            $form.find("#googleMap").parent().css("border", "2px solid #ea5f5f");
                            showGeofenceToast("error", "The unloading location pinpoint marker must be inside the drawn geofence.", "Validation Error", true);
                            return;
                        }
                    }
                }

                $form.find(":disabled").prop("disabled", false);

                if (isPartial) {
                    var formData = $form.serialize();
                    $.ajax({
                        url: $form.attr('action'),
                        type: 'POST',
                        data: formData,
                        success: function (response) {
                            App.unblockUI();
                            if (response.Status) {
                                if (typeof window.onConsigneeAdded === 'function') {
                                    window.onConsigneeAdded(response);
                                } else {
                                    TosterNotification("success", "Consignee registered successfully!", "Success");
                                    var modalEl = document.getElementById('glFormModal');
                                    if (modalEl) {
                                        var bsModal = bootstrap.Modal.getOrCreateInstance(modalEl);
                                        bsModal.hide();
                                    }
                                }
                            } else {
                                TosterNotification("error", response.Message || "Failed to save consignee.", "Error");
                            }
                        },
                        error: function () {
                            App.unblockUI();
                            TosterNotification("error", "An error occurred while saving the consignee.", "Error");
                        }
                    });
                } else {
                    document.forms['form_consignee_modal'].submit();
                }
            });
        });
    };

    var isPartial = true;
    var DomainName = window.DomainName || "";

    function ensureGoogleAndInit() {
        // Initialize the form elements immediately (Select2, Validations, and event bindings)
        initConsigneeMaster(DomainName, isPartial);

        if (typeof google === 'undefined' || typeof google.maps === 'undefined') {
            var gScriptId = 'gmaps-api-script';
            if (!document.getElementById(gScriptId)) {
                var script = document.createElement('script');
                script.id = gScriptId;
                script.src = "https://maps.googleapis.com/maps/api/js?key=AIzaSyAfDCpK-8ZZ9ANpIZazwLqXlkJ8hAc6y3E&libraries=drawing,geometry,places&callback=onGoogleMapsApiLoaded";
                window.onGoogleMapsApiLoaded = function() {
                    if (typeof window.triggerConsigneeMapInit === 'function') {
                        window.triggerConsigneeMapInit();
                    }
                };
                document.head.appendChild(script);
            } else {
                var interval = setInterval(function() {
                    if (typeof google !== 'undefined' && typeof google.maps !== 'undefined') {
                        clearInterval(interval);
                        if (typeof window.triggerConsigneeMapInit === 'function') {
                            window.triggerConsigneeMapInit();
                        }
                    }
                }, 100);
                // Safety timeout to clear interval after 10 seconds
                setTimeout(function() { clearInterval(interval); }, 10000);
            }
        }
    }

