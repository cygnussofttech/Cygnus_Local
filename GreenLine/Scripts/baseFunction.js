// Hard reload removed for performance — was doubling first-load time.
// (function () {
//     var storageKey = (typeof isFreshLogin !== 'undefined' && isFreshLogin) ? 'HardReloadPerformed_Login' : 'HardReloadPerformed_FirstLoad';
//     if (!sessionStorage.getItem(storageKey)) {
//         sessionStorage.setItem(storageKey, 'true');
//         if (window.location.reload) {
//             window.location.reload(true);
//             console.log("Hard Reload Performed");
//         }
//     }
// })();

// Cache-busting is now handled by version-based query strings on scripts.

//var _DomainName = '';
//var DomainName = '';

// Reusable GSAP Loader Component module
// Simple, Minimal AppLoader module with Nested Call Support
const AppLoader = (function () {
    let progressInterval;
    let callCounter = 0;

    function resetProgressBar() {
        $("#hyper-progress-bar").css("width", "0%");
    }

    function simulateProgress() {
        let width = parseFloat($("#hyper-progress-bar").css("width")) || 0;
        clearInterval(progressInterval);
        progressInterval = setInterval(() => {
            if (width < 90) {
                width += Math.random() * 2;
                $("#hyper-progress-bar").css("width", width + "%");
            }
        }, 200);
    }

    return {
        show: function (customText) {
            callCounter++;
            if (callCounter === 1) {
                resetProgressBar();
                $("#hyper-loader").addClass("active");
                simulateProgress();
            }
        },
        hide: function (force = false) {
            if (force) callCounter = 0;
            else callCounter = Math.max(0, callCounter - 1);

            if (callCounter === 0) {
                clearInterval(progressInterval);
                $("#hyper-progress-bar").css("width", "100%");

                setTimeout(() => {
                    // Only hide if a new request hasn't started in the meantime
                    if (callCounter === 0) {
                        $("#hyper-loader").removeClass("active");
                        setTimeout(resetProgressBar, 300);
                    }
                }, 300);
            }
        }
    };
})();

jQuery(document).ready(function () {
    // Fallback for FormComponents if not defined by another script
    if (typeof FormComponents === 'undefined') {
        window.FormComponents = {
            init: function () { }
        };
    }

    App.init(DomainName);

    // Override legacy Metronic blockUI methods to use our modern AppLoader component
    if (typeof App !== 'undefined') {
        App.blockUI = function (options) {
            // We use the global modern overlay instead of the legacy small "boxed" spinner
            AppLoader.show(null, false); // false = show immediately for non-ajax manual calls
        };
        App.unblockUI = function (target) {
            AppLoader.hide();
        };
    }

    FormComponents.init(DomainName);

    // Automatically trigger loader on all jQuery AJAX calls globally
    // $(document).ajaxStart(function () {
    //     AppLoader.show();
    // }).ajaxStop(function () {
    //     AppLoader.hide();
    // });

    // Remove fallback loader once document is fully downloaded/ready
    $(window).on('load', function () {
        AppLoader.hide();
    });

    // Provide instant feedback when clicking a link to change pages
    // This shows the truck immediately while the browser waits for the next page to load
    $(document).on('click', 'a', function (e) {
        var href = $(this).attr('href');
        var target = $(this).attr('target');
        var isDownload = $(this).attr('download') !== undefined;

        // Only trigger for valid internal links, not anchors, javascript:void, new tabs, or downloads
        if (href && href !== '#' && !href.startsWith('javascript') && !href.startsWith('#') && target !== '_blank' && !isDownload && !e.ctrlKey && !e.metaKey) {
            AppLoader.show("Navigating to " + $(this).text().trim() + "...", false);
        }
    });

    CapitalInput("clsCapital");
    //$("input:checkbox, input:radio").uniform();
    var text = $("#XMLPath").text();
    var xml = $.parseXML(text);
    //console.log($(xml).find('NavigationURL').text());
    var shortcutMap = {};
    $(window).keydown(function (event) {
        if (event.which == 113) { //F2
            $(".page-breadcrumb.breadcrumb").click();
            //return false;
        }
        if (event.which == 27) { //Esc
            $(".close").click();
            //return false;
        }
        let keyCombo = '';
        if (event.ctrlKey) keyCombo += 'ctrl+';
        if (event.altKey) keyCombo += 'alt+';
        if (event.shiftKey) keyCombo += 'shift+';
        if (event.key) {
            keyCombo += event.key.toLowerCase();
        }


        let shortcutUrl = shortcutMap[keyCombo];
        if (shortcutUrl) {
            $.ajax({
                url: DomainName + '/Master/ValidateShortcutAccess?shortcutKey=' + encodeURIComponent(keyCombo),
                type: 'GET',
                success: function (data) {
                    if (data.allowed) {
                        window.location.href = DomainName + data.url;
                    } else {
                        TosterNotification("error", data.message, "Error");


                    }
                }
            });
        }
    });
    $.ajaxPrefilter(function (options, originalOptions, jqXHR) {
        // Skip prefilter for FormData (file uploads)
        if (options.data instanceof FormData) {
            return;
        }

        // Apply settings only for POST requests
        if (options.type && options.type.toUpperCase() === 'POST') {
            // Set contentType for POST requests
            if (!options.contentType) {
                options.contentType = "application/json; charset=utf-8";
            }

            // Add an empty JSON object if no data is provided
            if (!options.data) {
                options.data = JSON.stringify({});
            }
        }
    });

    // Change Setting Popup Start
    $("#page-breadcrumb").click(function () {
        App.blockUI({
            target: '.scroller',
            cenrerY: true,
            boxed: true
        });
        var StrURL1 = DomainName + "/ChangeSetting/popUpIndex";
        $.ajax({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            url: StrURL1,
            data: {},
            success: function (data1) {
                $(".scroller").html(data1);
                // $('select').select2(); // Removed global select init to prevent conflicts
                $("#ChangeSetting select").select2();
                $("#ChangeSetting").keydown(function (e) {
                    if (e.keyCode == 13) { //Enter
                        $("#ChangeSetting #btnSubmit").click();
                    }
                });
                $("#ChangeSetting").on("click", "#btnSubmit", function (event) {
                    App.blockUI({
                        target: '.scroller',
                        cenrerY: true,
                        boxed: true
                    });
                    var btn = $(this);
                    btn.button('loading');
                    $.post($("#ChangeSetting").attr('action'), $("#ChangeSetting").serialize(), function (data, status) {
                        var finalStatus = data.split('-');
                        if (finalStatus[0] == "success") {
                            TosterNotification("success", "Record has been saved successfully..!!", "Success");
                            window.location = DomainName + "/Home/Index";
                        }
                        else {
                            TosterNotification("error", "Error !! There was an error druing process. Please check your internet connection or contact administrator.", "Error");
                        }
                    });
                });
            },
            error: function () {
                alert("error");
                App.unblockUI();
            }
        });
        $("#changeSettingPopUp").click();
    });
    // Change Setting Popup End
    $(".select2").each(function () {
        if (!$(this).data('select2') && $(this).is('select')) {
            $(this).select2({
                placeholder: "Select an option",
                allowClear: true
            });
        }
    });

    // Global Fix: Ensure Select2 search fields work correctly inside Bootstrap modals
    // This prevents the modal's focus-trap from blocking input into Select2 search boxes
    document.addEventListener('focusin', function (e) {
        if (e.target.closest('.select2-container') || e.target.closest('.select2-search__field') || e.target.closest('.select2-input')) {
            e.stopImmediatePropagation();
        }
    }, true);
});

var setDomainName = function (domainNameVal) {
    _DomainName = domainNameVal;
    DomainName = domainNameVal;
}
var postAndRedirect = function (url, postData) {
    var postFormStr = "<form method='POST' action='" + url + "'>\n";
    for (var key in postData) {
        if (postData.hasOwnProperty) {
            postFormStr += "<input type='hidden' name='" + key + "' value='" + postData[key] + "'></input>";
        }
    }
    postFormStr += "</form>";
    var formElement = $(postFormStr);
    $('.portlet-body').find('form').remove();
    $('.portlet-body').append(formElement);
    $(formElement).submit();
};

var HRMSFormValidate = function (DomainName, formId) {
    var form = $('#' + formId);
    var error = $('.alert-danger', form);
    var success = $('.alert-success', form);
    form.validate({
        doNotHideMessage: true,
        errorElement: 'span',
        errorClass: 'help-block',
        focusInvalid: false,
        ignore: "",
        rules: {},
        messages: {},
        invalidHandler: function (event, validator) {
            success.hide();
            error.show();
            App.scrollTo(error, -200);
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
            success.show();
            error.hide();
        }
    });
};

//  DropDown : Single-Value Search With Single Character ----------------------------------------------------------------------------------
var ddlSearch = function (ddlName, strUrl, param) {
    $("#" + ddlName + " ,." + ddlName).select2({
        placeholder: "Search Here",
        minimumInputLength: 1,
        allowClear: true,
        ajax: {
            url: strUrl,
            dataType: 'json',
            data: function (term) {
                return {
                    SearchText: term
                };
            },
            results: function (data) {
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = $(item).val();
            if (id !== "") {
                $.ajax(strUrl + '?' + param + '=' + id, {
                    dataType: "json"
                }).done(function (result) {
                    if (result.length > 0)
                        var data = {
                            id: result[0].id,
                            text: result[0].text
                        };
                    else {
                        var text = item.data('option');
                        var data = {
                            id: id,
                            text: text
                        };
                    }
                    callback(data);
                });
            }
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.id + " - " + item.text);
        },
        escapeMarkup: function (m) {
            return m;
        },
    });
}

//  DropDown : Multi-Value Selection ----------------------------------------------------------------------------------

var ddlMultiValue = function (ddlName, serachtext, strUrl) {
    var ddllist = $.ajax({
        url: strUrl,
        dataType: 'json',
        data: {
            str: serachtext
        },
        success: function (response) {
            $("#" + ddlName).select2({
                tags: response,
                placeholder: 'Search Here',
                multiple: true,
                allowClear: true
            });
        }
    });
};



function ddlFill(ddlName, StrURL, message) {
    $("#" + ddlName).get(0).options.length = 0;
    $("#" + ddlName).get(0).options[0] = new Option("Loading names", "-1");
    $("#" + ddlName).empty();
    $("#" + ddlName).find("option").remove();
    var select = $("#" + ddlName);
    select.empty();
    $.get(StrURL, function (data) {
        $("#" + ddlName).append('<option value="">Select ' + message + '</option>');
        for (i = 0; i < data.length; i++) {
            $("#" + ddlName).append('<option value="' + data[i].id + '">' + data[i].text + '</option>');
            $("#" + ddlName).trigger("liszt:updated");
        }
        App.unblockUI();
        if (data.length > 0) {
            $("#" + ddlName).select2({
                placeholderOption: 'first'
            });
        }
    }, "json");
    $("#" + ddlName).trigger("liszt:updated");
    $("#" + ddlName).val($("#target option:first").val());
    $("#" + ddlName).addClass("required");
}

function GetType(DDLName, str, url) {
    var tag_list = $.ajax({
        url: url,
        dataType: 'json',
        data: {
            str: str
        },
        success: function (response) {
            $("#" + DDLName).select2({
                tags: true,
                data: response,
                placeholder: "Search for a Type",
                multiple: true,
                allowClear: true
            });
        }
    });
}

$(".fa-star,.removeFavorite").click(function (e) {
    var IsFavorite = 0;
    if (this.className != "removeFavorite") {
        if ($(this).hasClass("checked")) {
            $(this).removeClass("checked");
            IsFavorite = 0;
        } else {
            $(this).addClass("checked");
            IsFavorite = 1;
        }
    }
    var Parameters = {};
    Parameters['MenuId'] = $(this).data('id');
    Parameters['UserId'] = UserId;
    Parameters['IsFavorite'] = IsFavorite;
    console.log($(this).data('id') + "  " + UserId);
    if ($(this).data('id') != undefined) {
        $.ajax({
            url: DomainName + "/Home/GetData",
            type: 'POST',
            dataType: "json",
            data: {
                Method: 'USP_AddRemoveMenuAsFavorite',
                parameters: Parameters,
            },
            success: function (result) {
                $.get(DomainName + "/Account/deleteAllFile?id=UserMenuRights_" + UserId + ".xml");
                window.location.href = DomainName + "/Home/Index";
            }
        });
    }
    return false;
});

//$(document).ajaxStart(function () {
//    App.blockUI({ boxed: true });
//});

//$(document).ajaxComplete(function () {
//    App.unblockUI();
//});

var handleDatePicker = function (startDate) {
    if (startDate == "" || startDate == "undefined" || startDate == undefined)
        startDate = CurrentFinYearStartDate
    $('.date-picker').datepicker({
        isRTL: App.isRTL(),
        format: "dd M yyyy",
        autoclose: true,
        pickerPosition: (App.isRTL() ? "bottom-right" : "bottom-left"),
        startDate: startDate,
        endDate: new Date(),
        todayBtn: true
    });
    $('body').removeClass("modal-open");
}

var BackhandleDatePicker = function () {
    $('.bkddate-picker').datepicker({
        isRTL: App.isRTL(),
        format: "dd M yyyy",
        autoclose: true,
        pickerPosition: (App.isRTL() ? "bottom-right" : "bottom-left"),
        todayBtn: true
    });
    $('body').removeClass("modal-open");
}

var FutureDatePicker = function () {
    $('.ftdate-picker').datepicker({
        isRTL: App.isRTL(),
        format: "dd-MM-yyyy",
        autoclose: true,
        pickerPosition: (App.isRTL() ? "bottom-right" : "bottom-left"),
        startDate: new Date(),
        todayBtn: true
    });
    $('.bkdate-picker').datepicker({
        isRTL: App.isRTL(),
        format: "dd-MM-yyyy",
        autoclose: true,
        pickerPosition: (App.isRTL() ? "bottom-right" : "bottom-left"),
        startDate: "01 Jun 2021",
        endDate: new Date(),
        todayBtn: true
    });
    $('body').removeClass("modal-open");
}

var PastDatePicker = function () {
    //if (jQuery().datepicker) {
    $('.ptdate-picker').datepicker({
        isRTL: App.isRTL(),
        format: "dd MM yyyy",
        autoclose: true,
        pickerPosition: (App.isRTL() ? "bottom-right" : "bottom-left"),
        startDate: "01 Jun 2021",
        endDate: new Date(),
    });
    $('body').removeClass("modal-open"); // fix bug when inline picker is used in modal
    //}
}

var jsGetReportFields = function (DDLName, StrURL1) {
    $.ajax({
        url: StrURL1,
        cache: false,
        data: {},
        type: 'POST',
        success: function (html) {
            $("." + DDLName).empty();
            $("." + DDLName).append(html);

            // Uniform removed - using native Bootstrap 5 styling
            //$("input:checkbox, input:radio").uniform();

            App.unblockUI();
        },
        error: function (req, status, error) {
            App.unblockUI();
            alert("error " + req + "   " + status + "   " + error);
        }
    });

}

var CapitalInput = function (ControlName) {
    $('input[type=text]').keyup(function (event) {
        // if (!$(this).hasClass('email') && !$(this).hasClass('multiemails') && !$(this).parent('div').hasClass("input-group date") && !$(this).hasClass("no-uppercase")) {
        if ($(this).hasClass('text-uppercase') || $(this).hasClass('auto-uppercase')) {
            var textBox = event.target;
            var start = textBox.selectionStart;
            var end = textBox.selectionEnd;
            textBox.value = textBox.value.toUpperCase();
            textBox.setSelectionRange(start, end);
        }
    });
}

var handleDateTimePicker_ddmmyyyy = function (DDLName, StartDate, EntDate) {
    if (jQuery().datepicker) {
        $("#" + DDLName + " ,." + DDLName).datepicker({
            rtl: App.isRTL(),
            format: "dd MM yyyy",
            showMeridian: true,
            autoclose: true,
            pickerPosition: (App.isRTL() ? "bottom-right" : "bottom-left"),
            startDate: new Date(StartDate),
            endDate: new Date(EntDate),
            todayBtn: true
        });
    }
}

var handleDateTimePicker_ddmmyyyyInvoice = function (DDLName, StartDate, EntDate) {
    if (jQuery().datepicker) {
        $("#" + DDLName + " ,." + DDLName).datepicker({
            rtl: App.isRTL(),
            format: "dd MM yyyy",
            showMeridian: true,
            autoclose: true,
            pickerPosition: (App.isRTL() ? "bottom-right" : "bottom-left"),
            //startDate: new Date(StartDate),
            //endDate: new Date(EntDate),
            //todayBtn: true
        });
    }
}

var handleDateTimePicker = function (StartDate, EntDate) {
    if (jQuery().datepicker) {
        $('.datetime-picker').datetimepicker({
            rtl: App.isRTL(),
            format: "dd MM yyyy  HH:ii P",
            showMeridian: true,
            autoclose: true,
            pickerPosition: (App.isRTL() ? "bottom-right" : "bottom-left"),
            startDate: new Date(StartDate),
            endDate: new Date(EntDate),
            todayBtn: true
        });
    }
}

var handleDateRangePickers = function (DatePickerId, FromDateId, ToDateId) {
    var YearEndDate = new Date(CurrentFinYearEndDate) > new Date() ? new Date() : new Date(CurrentFinYearEndDate);
    //console.log(YearEndDate);
    var CurrFinYearStartDate = new Date(CurrentFinYearStartDate);
    //console.log("CurrFinYearStartDate : " + CurrFinYearStartDate);
    var CurrFinYearEndDate = new Date(CurrentFinYearEndDate);
    //console.log("CurrFinYearEndDate : " + CurrFinYearEndDate);

    $('#' + DatePickerId).daterangepicker({
        opens: (App.isRTL() ? 'left' : 'right'),
        startDate: moment(YearEndDate).subtract('days', 29),
        endDate: moment(YearEndDate),
        maxDate: CurrFinYearEndDate,
        showDropdowns: true,
        showWeekNumbers: true,
        timePicker: false,
        timePickerIncrement: 1,
        timePicker12Hour: true,
        ranges: {
            'Today': [moment(YearEndDate), moment(YearEndDate)],
            'Yesterday': [moment(YearEndDate).subtract('days', 1), moment(YearEndDate).subtract('days', 1)],
            'Last 7 Days': [moment(YearEndDate).subtract('days', 6), moment(YearEndDate)],
            'Last 30 Days': [moment(YearEndDate).subtract('days', 29), moment(YearEndDate)],
            'This Month': [moment(YearEndDate).startOf('month'), moment(YearEndDate).endOf('month')],
            'Last Month': [moment(YearEndDate).subtract('month', 1).startOf('month'), moment(YearEndDate).subtract('month', 1).endOf('month')],
            'Last Financial Year': [moment().subtract(1, 'year').month(3).startOf('month'), moment().subtract(0, 'year').month(2).endOf('month')],
            'This Financial Year': [moment(CurrFinYearStartDate), moment(CurrFinYearEndDate)],
            //'This Financial Year': [moment().startOf('year').month(3).startOf('month'), moment(YearEndDate)],
            //'Last Financial Year': [moment(CurrFinYearStartDate).add('year', -1), moment(CurrFinYearEndDate).add('year', -1)],
        },
        buttonClasses: ['btn'],
        applyClass: 'green',
        cancelClass: 'default',
        format: 'MM/DD/YYYY',
        separator: ' to ',
        locale: {
            applyLabel: 'Apply',
            fromLabel: 'From',
            toLabel: 'To',
            customRangeLabel: 'Custom Range',
            daysOfWeek: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
            monthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
            firstDay: 1
        }
    }, function (start, end) {
        $('#' + FromDateId).val(start.format('DD MMM YYYY'));
        $('#' + ToDateId).val(end.format('DD MMM YYYY'));
        $('#' + DatePickerId + ' span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));

    });
    $('#' + DatePickerId + ' span').html(moment(YearEndDate).subtract('days', 29).format('MMMM D, YYYY') + ' - ' + moment(YearEndDate).format('MMMM D, YYYY'));
    $('#' + FromDateId).val(moment(YearEndDate).subtract('days', 29).format('DD MMM YYYY'));
    $('#' + ToDateId).val(moment(YearEndDate).format('DD MMM YYYY'));
}

var FormValidate = function (DomainName) {
    console.log('FormValidate')
    var form1 = $('#form_sample');
    var error1 = $('.alert-danger', form1);
    var success1 = $('.alert-success', form1);
    form1.validate({
        doNotHideMessage: true,
        errorElement: 'span',
        errorClass: 'help-block',
        focusInvalid: false,
        ignore: "",
        rules: {},
        messages: {},
        invalidHandler: function (event, validator) {
            success1.hide();
            error1.show();
            App.scrollTo(error1, -200);
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
            document.forms["form_sample"].submit()
        }
    });
}

var TosterNotification = function (Type, Message, Title) {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "positionClass": "toast-top-right",
        "onclick": null,
        "showDuration": "1000",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "show",
        "hideMethod": "hide"
    }
    if (Type == "success") {
        toastr.success(Message, Title);
    }
    if (Type == "error") {
        toastr.error(Message, Title);
    }
    if (Type == "warning") {
        toastr.warning(Message, Title);
    }
    if (Type == "info") {
        toastr.info(Message, Title);
    }
}

function validateFloatfor2point(el, evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    var number = el.value.split('.');
    if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    //just one dot
    if (number.length > 1 && charCode == 46) {
        return false;
    }
    //get the carat position
    var caratPos = getSelectionStart(el);
    var dotPos = el.value.indexOf(".");
    if (caratPos > dotPos && dotPos > -1 && (number[1].length > 1)) {
        return false;
    }
    return true;
}

function getSelectionStart(o) {
    if (o.createTextRange) {
        var r = document.selection.createRange().duplicate()
        r.moveEnd('character', o.value.length)
        if (r.text == '') return o.value.length
        return o.value.lastIndexOf(r.text)
    } else return o.selectionStart
}

function validFloat(e, txtid) {
    var code;
    var tb = (typeof txtid === 'string') ? document.getElementById(txtid) : txtid;
    if (!tb) return true;
    var txt = tb.value;
    if (!e) var e = window.event;
    if (e.keyCode)
        code = e.keyCode;
    else if (e.which)
        code = e.which;
    else
        return true;
    if (code == 13 || code == 8 || code == 9)
        return true;
    if (code == 46)
        if (txt.indexOf('.') != -1)
            code = 0;
    if ((code < 46 || code > 57) || code == 47) {
        code = 0;
        return false;
    }
}

function validDigits(e, txtid) {
    var code;
    var tb = (typeof txtid === 'string') ? document.getElementById(txtid) : txtid;
    if (!tb) return true;
    var txt = tb.value;
    if (!e) var e = window.event;
    if (e.keyCode)
        code = e.keyCode;
    else if (e.which)
        code = e.which;
    else
        return true;
    if (code == 13 || code == 8 || code == 9)
        return true;
    if (code == 46)
        if (txt.indexOf('.') != -1)
            code = 0;
    if ((code < 46 || code > 57) || code == 47 || code == 46) {
        code = 0;
        return false;
    }
}

function SkipSpecialCharacter(e) {
    var k;
    document.all ? k = e.keyCode : k = e.which;
    return ((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57));
}

function ChangeInJQDate(dateObject) {
    var arrmonths = new Array("", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December");
    var DateArr = dateObject.split('/');
    var day = DateArr[0];
    var month = DateArr[1];
    var year = DateArr[2];
    var date = day + " " + arrmonths[parseInt(month)] + " " + year;
    return date;
};

function myDateFormatter(dateObject) {
    var arrmonths = new Array("", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December");
    var d = new Date(dateObject);
    var day = d.getDate();
    var month = d.getMonth() + 1;
    var year = d.getFullYear();
    if (day < 10) {
        day = "0" + day;
    }
    var date = day + " " + arrmonths[month] + " " + year;
    return date;
};

function GetRadioButtonfromOthers(DDLName, StrURL, DllType, DefaultValue) {
    $.get(StrURL, function (data) {
        for (i = 0; i < data.length; i++) {
            $("." + DDLName).append('<label class="radio-inline"><input type="radio" name="' + DDLName.toUpperCase() + '" id="' + data[i].Value + '" value="' + data[i].Value + '" class="cls' + DDLName.toUpperCase() + '"> ' + data[i].Text + '</label>');
        }
        $(".cls" + DDLName).each(function () {
            if (DefaultValue == $(this).val()) {
                $(this).attr("checked", true);
                $(this).parents('span').addClass("checked");

            } else {
                $(this).attr("checked", false);
                $(this).parents('span').removeClass("checked");
            }
        });
        //$("input:checkbox, input:radio").uniform();
        // Uniform removed

    }, "json");
}

function GetCheckBoxButtonfromOthers(DDLName, StrURL, DllType, DefaultValue) {
    $.get(StrURL, function (data) {
        for (i = 0; i < data.length; i++) {
            $("." + DDLName).append('<label class="checkbox-inline"><input type="checkbox" name="' + DDLName.toUpperCase() + '" id="' + data[i].Value + '" value="' + data[i].Value + '" class="cls' + DDLName.toUpperCase() + '"> ' + data[i].Text + '</label>');
        }

        $(".cls" + DDLName).each(function () {
            if (DefaultValue == $(this).val()) {
                $(this).attr("checked", true);
                $(this).parents('span').addClass("checked");

            } else {
                $(this).attr("checked", false);
                $(this).parents('span').removeClass("checked");
            }
        });

        // $("input:checkbox, input:radio").uniform();

    }, "json");
}

function FillDropDownfromOther(DDLName, StrURL, DllType) {
    var target = $("select#" + DDLName).length > 0 ? "select#" + DDLName : "#" + DDLName;
    if ($(target).get(0) && $(target).get(0).options) {
        $(target).get(0).options.length = 0;
        $(target).get(0).options[0] = new Option("Loading names", "-1");
    }
    $(target).empty();
    $(target).find("option").remove();
    var select = $(target);
    select.empty();
    $.get(StrURL, function (data) {
        $(target).append('<option value="">Select ' + DllType + '</option>');
        for (i = 0; i < data.length; i++) {
            $(target).append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
            $(target).trigger("liszt:updated");
        }
        App.unblockUI();
        if (data.length > 0) {
            $(target).select2({
                placeholderOption: 'first'
            });
        }
    }, "json");
    $(target).trigger("liszt:updated");
    $(target).val($("#target option:first").val());
}

function FillDropDownfromOtherWithoutSelect(DDLName, StrURL, DllType) {
    $("#" + DDLName).get(0).options.length = 0;
    $("#" + DDLName).get(0).options[0] = new Option("Loading names", "-1");
    $("#" + DDLName).empty();
    $("#" + DDLName).find("option").remove();
    var select = $("#" + DDLName);
    select.empty();
    $.get(StrURL, function (data) {
        for (i = 0; i < data.length; i++) {
            $("#" + DDLName).append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
            $("#" + DDLName).trigger("liszt:updated");
        }
        App.unblockUI();
        if (data.length > 0) {
            $("#" + DDLName).select2({
                placeholderOption: 'first'
            });
        }
    }, "json");
    $("#" + DDLName).trigger("liszt:updated");
    $("#" + DDLName).addClass("required");
}

function formatBillOption(option) {
    console.log(option.text)
    var $option = '<div>' + option.text + '</div>';
    if (option.element[0].title != "false") {
        $option = $(
            '<div><strong>' + option.text + '</strong></div>' +
            '<div style="font-size: 10px;">State : <b>' + option.element[0].title.split(" || ")[0].trim() + '</b></div>' +
            '<div style="font-size: 10px;">Transit Mode : <b>' + option.element[0].title.split(" || ")[1].trim() + '</b></div>' +
            '<div style="font-size: 10px;">Docket Count : <b>' + option.element[0].title.split(" || ")[2].trim() + '</b></div>' +
            '<div style="font-size: 10px;">GST Type : <b>' + option.element[0].title.split(" || ")[3].trim() + '</b></div>' +
            '<div style="font-size: 10px;">GST % : <b>' + (option.element[0].title.split(" || ")[4].trim() == "0.00" ? '<del>' + option.element[0].title.split(" || ")[4].trim() + '</del>' : option.element[0].title.split(" || ")[4].trim()) + '</b></div>' +
            '<div style="font-size: 10px;">Sub Total : <b>' + option.element[0].title.split(" || ")[5].trim() + '</b></div>' +
            '<div style="font-size: 10px;">GST Amount : <b>' + option.element[0].title.split(" || ")[6].trim() + '</b></div>' +
            '<div style="font-size: 10px;">Billing Amount : <b>' + option.element[0].title.split(" || ")[7].trim() + '</b></div>'
            /*'<div style="font-size: 10px;">Billing Instance : <b>' + option.element[0].title.split(" || ")[8].trim() + '</b></div>'*/
        );
    }
    return $option;
};



function FillDropDownfromOtherWithJson(Method, Parameters, DDLName, Placeholder, IsAsync, DefaultValue, allowClear, SpecialDropdown) {
    if (Placeholder == "" || Placeholder == "undefined" || Placeholder == undefined)
        Placeholder = DDLName;

    if (IsAsync == undefined)
        IsAsync = true;
    if (allowClear == "" || allowClear == "undefined" || allowClear == undefined)
        allowClear = false;
    if (SpecialDropdown == "" || SpecialDropdown == "undefined" || SpecialDropdown == undefined)
        SpecialDropdown = "0";

    var strUrl = DomainName + "/Account/GetDataWithoutAlias";
    $.ajax({
        url: strUrl,
        type: 'POST',
        dataType: "json",
        async: IsAsync,
        data: {
            Method: Method,
            parameters: Parameters
        },
        success: function (data) {
            var Headers = GetHeaders(data[0]);

            $("#" + DDLName).get(0).options.length = 0;
            $("#" + DDLName).get(0).options[0] = new Option("Loading names", "-1");
            $("#" + DDLName).find("option").remove();
            $("#" + DDLName).empty();
            $("#" + DDLName).append('<option value="">Select ' + Placeholder + '</option>');
            for (i = 0; i < data.length; i++) {
                if (SpecialDropdown == "1") {
                    $("#" + DDLName).append('<option value="' + data[i][Headers[0].data] + '" title="' + data[i][Headers[2].data] + '"> ' + data[i][Headers[1].data] + '</option > ');
                }
                else {
                    if (DefaultValue == data[i][Headers[0].data]) {
                        $("#" + DDLName).append('<option selected value="' + data[i][Headers[0].data] + '" > ' + data[i][Headers[1].data] + '</option > ');
                    }
                    else {
                        $("#" + DDLName).append('<option value="' + data[i][Headers[0].data] + '" > ' + data[i][Headers[1].data] + '</option > ');
                    }
                }
                $("#" + DDLName).trigger("liszt:updated");
                if (i == 0 && DefaultValue == null) {
                    $("#" + DDLName).select2("data", {
                        id: data[i][Headers[0].data],
                        text: data[i][Headers[1].data]
                    });
                } else if (DefaultValue == data[i][Headers[0].data]) {
                    $("#" + DDLName).select2("data", {
                        id: data[i][Headers[0].data],
                        text: data[i][Headers[1].data]
                    });
                }
            }
            //App.unblockUI();
            if (SpecialDropdown == "1") {
                $("#" + DDLName).select2({
                    placeholderOption: 'first',
                    allowClear: allowClear,
                    formatResult: formatBillOption
                });
            }
            else {
                $("#" + DDLName).select2({
                    placeholderOption: 'first',
                    allowClear: allowClear
                });
            }
            $("#" + DDLName).trigger("liszt:updated");
            $("#" + DDLName).val($("#target option:first").val());
            $("#" + DDLName).addClass("required");
        },
        error: function (req, status, error) {
            TosterNotification("error", 'Operation fail..!! There is some issue while loading data. Please try again later..', "Oops..!!");
            //App.unblockUI();
        }
    });
}

function FillDropDownfromOtherDisabled(DDLName, StrURL, DllType) {
    $("#" + DDLName).get(0).options.length = 0;
    $("#" + DDLName).get(0).options[0] = new Option("Loading names", "-1");
    $("#" + DDLName).empty();
    $("#" + DDLName).find("option").remove();
    var select = $("#" + DDLName);
    select.empty();
    $.get(StrURL, function (data) {
        $("#" + DDLName).append('<option value="">Select ' + DllType + '</option>');
        for (i = 0; i < data.length; i++) {
            var finalText = '<option value="' + data[i].id + '">' + data[i].text + '</option>';
            //alert(data[i].text)
            if (data[i].text.indexOf("In Transit") > 0) {
                finalText = '<option value="' + data[i].id + '" disabled>' + data[i].text + '</option>';;
            }
            $("#" + DDLName).append(finalText);
            $("#" + DDLName).trigger("liszt:updated");
        }
        App.unblockUI();
        if (data.length > 0) {
            $("#" + DDLName).select2({
                placeholderOption: 'first'
            });
        }
    }, "json");
    $("#" + DDLName).trigger("liszt:updated");
    $("#" + DDLName).val($("#target option:first").val());
    $("#" + DDLName).addClass("required");
}

function FillDropDownfromOther(DDLName, StrURL, DllType) {
    var target = $("select#" + DDLName).length > 0 ? "select#" + DDLName : "#" + DDLName;
    if ($(target).get(0) && $(target).get(0).options) {
        $(target).get(0).options.length = 0;
        $(target).get(0).options[0] = new Option("Loading names", "-1");
    }
    $(target).empty();
    $(target).find("option").remove();
    var select = $(target);
    select.empty();
    $.get(StrURL, function (data) {
        $(target).append('<option value="">Select ' + DllType + '</option>');
        for (i = 0; i < data.length; i++) {
            $(target).append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
            $(target).trigger("liszt:updated");
        }
        App.unblockUI();
        if (data.length > 0) {
            $(target).select2({
                placeholderOption: 'first'
            });
        }
    }, "json");
    $(target).trigger("liszt:updated");
    $(target).val($("#target option:first").val());
}

function EmptyDDL(DDLName) {
    $("#" + DDLName).empty();
}

function FillDropDownfromOtherSelectFirst(DDLName, StrURL, DllType, DefaultValue) {
    $("#" + DDLName).get(0).options.length = 0;
    $("#" + DDLName).get(0).options[0] = new Option("Loading names", "-1");
    $("#" + DDLName).empty();
    $("#" + DDLName).find("option").remove();
    var select = $("#" + DDLName);
    select.empty();
    $.ajax({
        url: StrURL,
        type: 'POST',
        dataType: "json",
        success: function (data) {
            for (i = 0; i < data.length; i++) {
                $("#" + DDLName).append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
                $("#" + DDLName).trigger("liszt:updated");
                if (i == 0 && DefaultValue == null) {
                    FirstText = data[i].Text;
                    $("#" + DDLName).select2("data", {
                        id: data[i].Value,
                        text: data[i].Text
                    });
                } else if (DefaultValue == data[i].Value) {
                    $("#" + DDLName).select2("data", {
                        id: data[i].Value,
                        text: data[i].Text
                    });
                }
            }
        },
        error: function (req, status, error) {
            TosterNotification("error", 'Operation fail..!! There is some issue while loading data. Please try again later..', "Oops..!!");
            //App.unblockUI();
        }
    });
    $("#" + DDLName).trigger("liszt:updated");
    $("#" + DDLName).addClass("required");
}

function FillDropDownfromOtherSelectFirstReport(DDLName, StrURL, DllType, DefaultValue) {
    if (DDLName == "ReportCategory1") {
        // alert("DefaultValue " + DefaultValue);
    }
    $("#" + DDLName).html("");
    if (DefaultValue == null) {
        DefaultValue = "";
    }

    $("#" + DDLName).get(0).options.length = 0;
    $("#" + DDLName).get(0).options[0] = new Option("Loading names", "-1");
    $("#" + DDLName).empty();
    $("#" + DDLName).find("option").remove();
    var select = $("#" + DDLName);
    select.empty();
    $.get(StrURL, function (data) {
        $("#" + DDLName).html("");
        for (i = 0; i < data.length; i++) {

            $("#" + DDLName).append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
            $("#" + DDLName).trigger("liszt:updated");

            if (i == 0 && DefaultValue == "") {
                FirstText = data[i].Text;

                $("#" + DDLName).select2("data", { id: data[i].Value, text: data[i].Text });
            }
            else if (DefaultValue == data[i].Value) {

                $("#" + DDLName).select2("data", { id: data[i].Value, text: data[i].Text });
            }
        }
        App.unblockUI();
    }, "json");
    $("#" + DDLName).trigger("liszt:updated");
    // $("#" + DDLName).val($("#target option:first").val());

    $("#" + DDLName).addClass("required");

    setTimeout(
        function () {
            $("#" + DDLName).change();
        }, 1500);
}

function FillDropDownfromOther1(DDLName, StrURL, DllType) {
    $("#" + DDLName).get(0).options.length = 0;
    $("#" + DDLName).get(0).options[0] = new Option("Loading names", "-1");
    $("#" + DDLName).empty();
    $("#" + DDLName).find("option").remove();
    var select = $("#" + DDLName);
    select.empty();
    $.get(StrURL, function (data) {
        $("#" + DDLName).append('<option value="">Select ' + DllType + '</option>');
        for (i = 0; i < data.length; i++) {
            $("#" + DDLName).append('<option value="' + data[i].id + '">' + data[i].text + '</option>');
            $("#" + DDLName).trigger("liszt:updated");
        }
        App.unblockUI();
        if (data.length > 0) {
            $("#" + DDLName).select2({
                placeholderOption: 'first'
            });
        }
    }, "json");
    $("#" + DDLName).trigger("liszt:updated");
    $("#" + DDLName).val($("#target option:first").val());
}

function FillDropDownfromOtherSelectFirstValEvent(DDLName, StrURL, DllType, DefaultValue) {
    $("#" + DDLName).get(0).options.length = 0;
    $("#" + DDLName).get(0).options[0] = new Option("Loading names", "-1");
    $("#" + DDLName).empty();
    $("#" + DDLName).find("option").remove();
    var select = $("#" + DDLName);
    select.empty();
    $.ajax({
        url: StrURL,
        type: 'POST',
        dataType: "json",
        success: function (data) {
            for (i = 0; i < data.length; i++) {
                $("#" + DDLName).append('<option value="' + data[i].Value + '">' + data[i].Text + '</option>');
                $("#" + DDLName).trigger("liszt:updated");
                if (i == 0 && DefaultValue == null) {
                    FirstText = data[i].Text;
                    $("#" + DDLName).select2("data", {
                        id: data[i].Value,
                        text: data[i].Text
                    });
                } else if (DefaultValue == data[i].Value) {
                    $("#" + DDLName).select2("data", {
                        id: data[i].Value,
                        text: data[i].Text
                    });
                }
                $("#" + DDLName).val(data[0].Value).trigger('change');
            }
        },
        error: function (req, status, error) {
            TosterNotification("error", 'Operation fail..!! There is some issue while loading data. Please try again later..', "Oops..!!");
            App.unblockUI();
        }
    });
    $("#" + DDLName).trigger("liszt:updated");
    $("#" + DDLName).addClass("required");
}

function MultiSelectDDL(DDLName, strUrl, IsMultiple) {
    $("#" + DDLName + " ,." + DDLName).select2({
        placeholder: "Search for a Account Code",
        minimumInputLength: 1,
        allowClear: true,
        multiple: IsMultiple,
        tags: [],
        ajax: {
            url: strUrl,
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term
                };
            },
            results: function (data) {
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.id + " - " + item.text);
        },
        escapeMarkup: function (m) {
            return m;
        },
    });
}

function MultiSelectDDLCON(DDLName, strUrl, IsMultiple) {
    $("#" + DDLName + " ,." + DDLName).select2({
        placeholder: "Search for a Account Code",
        minimumInputLength: 0,
        allowClear: true,
        multiple: IsMultiple,
        tags: [],
        ajax: {
            url: strUrl,
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term
                };
            },
            results: function (data) {
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.id);
        },
        escapeMarkup: function (m) {
            return m;
        },
    });
}

function DLLTextSelected(DDLName, strUrl, IsMultiple, SerachLength, SerachCaption) {
    $("#" + DDLName + " ,." + DDLName).select2({
        minimumInputLength: SerachLength,
        placeholder: "Search " + SerachCaption,
        multiple: IsMultiple,
        allowClear: true,
        ajax: {
            url: strUrl,
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term
                };
            },
            results: function (data) {
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div> ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.id);
        },
        escapeMarkup: function (m) {
            return m;
        },
        dropdownCssClass: "bigdrop"
    });
}

function GetMultipleDropdownWithSearch(DDLName, StrURL1) {
    var data = [];
    $("#" + DDLName).select2({
        placeholder: "Search for a Location",
        multiple: true,
        tags: [],
        minimumInputLength: 1,
        allowClear: true,
        ajax: {
            url: StrURL1,
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term
                };
            },
            results: function (data) { // parse the results into the format expected by Select2.
                // since we are using custom formatting functions we do not need to alter remote JSON data
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var values = item.val().split(",");
            //var id = item.val().split(",");
            //var text = item.data('option');
            //var data = { id: id, text: id };
            var options = [];
            options = values.map(function (value) {
                var option = data.find(function (item) {
                    return item.id === value;
                });
                return { id: value, text: option ? option.text : value };
            });

            callback(options);
        },
        formatResult: function (item) { return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>'); },
        formatSelection: function (item) { return item.id + " - " + item.text; },
        escapeMarkup: function (m) { return m; },
        dropdownCssClass: "bigdrop" // apply css that makes the dropdown taller
    });
    App.unblockUI();
}

function JsonDDL(DDLName, strUrl) {
    $("#" + DDLName + " ,." + DDLName).select2({
        placeholder: "Search Here",
        minimumInputLength: 1,
        allowClear: true,
        ajax: {
            url: strUrl,
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term
                };
            },
            results: function (data) {
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.id + " - " + item.text);
        },
        escapeMarkup: function (m) {
            return m;
        },
    });
}

function JsonDDL_Disable(DDLName, strUrl) {
    $("#" + DDLName + " ,." + DDLName).select2({
        placeholder: "Search Here",
        minimumInputLength: 1,
        allowClear: true,
        ajax: {
            url: strUrl,
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term
                };
            },
            results: function (data) {
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.text + ' </b></div>');
        },
        formatSelection: function (item) {
            return (item.id);
        },
        escapeMarkup: function (m) {
            return m;
        },
    });
}

function GetPartyNameCode(DDLName, Location, Paybas) {
    var StrURL = DomainName + "/Operation/GetBillingParty?Location=" + Location + "&Paybas=" + Paybas;
    JsonDDL(DDLName, StrURL);
}

function GetPartyNameCode_Octroi(DDLName, Location, Paybas) {
    var StrURL = DomainName + "/Operation/GetBillingParty_OctroiBill?Location=" + Location + "&Paybas=" + Paybas;
    JsonDDL(DDLName, StrURL);
}

function GetPartyCode(DDLName, Location, Paybas, ServiceClass) {
    var StrURL = DomainName + "/Operation/GetBillingParty?Location=" + Location + "&Paybas=" + Paybas + "&ServiceClass=" + ServiceClass;
    JsonDDLSelectCode(DDLName, StrURL);
}

function GetVehicleCodeNotInTHC(DDLName, searchterm, Paybas) {
    var StrURL = DomainName + "/Operation/GetVehicleNoForOnTripButNotTHC?searchTerm=" + searchterm + "&Paybas=" + IndentNo;
    JsonDDLSelectCode(DDLName, StrURL);
}

function GetEmployeeCode(DDLName, Location) {
    var StrURL = DomainName + "/Master/GetEmployeeCodeList?Location=" + Location;
    JsonDDL(DDLName, StrURL);
}

function JsonDDLSelectCode(DDLName, strUrl) {
    $("#" + DDLName + " ,." + DDLName).select2({
        placeholder: "Search Here",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            url: strUrl,
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term
                };
            },
            results: function (data) {
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.id);
        },
        escapeMarkup: function (m) {
            return m;
        },
    });
}

function JsonDDLSelectCodeWithName(DDLName, strUrl) {
    //console.log(strUrl);
    $("#" + DDLName + " ,." + DDLName).select2({
        placeholder: "Search Here",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            url: strUrl,
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term
                };
            },
            results: function (data) {
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return ('<b>' + item.id + ' </b>- ' + item.text);
        },
        escapeMarkup: function (m) {
            return m;
        },
    });
}

function initTableById(tblId) {
    $('#' + tblId).dataTable({
        "aLengthMenu": [
            [5, 10, 15, 20, -1],
            [5, 10, 15, 20, "All"]
        ],
        "iDisplayLength": 10,
        "sPaginationType": "bootstrap",
        "bProcessing": true,
        "oLanguage": {
            "sLengthMenu": "_MENU_ records",
            "oPaginate": {
                "sPrevious": "Prev",
                "sNext": "Next"
            }
        },
        "bDestroy": true,
    });
    jQuery('#' + tblId + '_wrapper .dataTables_filter input').addClass("form-control input-medium input-inline");
    jQuery('#' + tblId + '_wrapper .dataTables_filter').addClass("pull-right");
    jQuery('#' + tblId + '_wrapper .dataTables_length select').addClass("form-control input-xsmall");
    jQuery('#' + tblId + '_wrapper .dataTables_length select').select2();
}
function initTableByIdWithSearch(tblId) {

    var $table = $('#' + tblId);

    if (!$table.length || !$table.find('thead').length) return;


    if ($.fn.DataTable.isDataTable($table)) {
        $table.DataTable().clear().destroy();
    }


    $table.find('thead tr.filters').remove();


    var $headerRow = $table.find('thead tr:first');
    var $filterRow = $('<tr class="filters"></tr>');

    $headerRow.children('th').each(function () {
        $filterRow.append('<th></th>');
    });

    $table.find('thead').append($filterRow);


    var table = $table.DataTable({
        dom: 'rtp',
        searching: true,
        ordering: true,
        paging: true,
        info: false,
        lengthChange: false,
        autoWidth: false
    });


    table.columns().every(function (index) {

        var headerText = $headerRow.children('th').eq(index).text().trim().toLowerCase();
        var cell = $table.find('thead tr.filters th').eq(index);


        if (headerText === 'action') {
            cell.html('');
            return;
        }

        $('<input>', {
            type: 'text',
            class: 'form-control input-sm',
            placeholder: 'Search'
        })
            .appendTo(cell)
            .on('keyup change clear', function () {
                table.column(index).search(this.value || '').draw();
            });
    });
}

function formateDataTable(tblId) {
    jQuery('#' + tblId + '_wrapper .dataTables_filter input').addClass("form-control input-medium input-inline");
    jQuery('#' + tblId + '_wrapper .dataTables_filter').addClass("pull-right");
    jQuery('#' + tblId + '_wrapper .dataTables_length select').addClass("form-control input-xsmall");
    jQuery('#' + tblId + '_wrapper .dataTables_length select').select2();
}

function ValidateAlpha(evt) {
    var keyCode = (evt.which) ? evt.which : evt.keyCode
    if ((keyCode < 65 || keyCode > 90) && (keyCode < 97 || keyCode > 123) && keyCode != 32)
        return false;
    return true;
}

function validInt(event) {
    if (event.keyCode == 13) {
        return true;
    }
    if (event.keyCode < 48 || event.keyCode > 57) {
        event.keyCode = 0;
        return false;
    }
}

var months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
var monthsShort = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

function GetHeaders(obj) {
    var cols = new Array();
    var count = false;
    for (var key in obj) {
        cols.push({
            data: key,
            title: key
        });
    }
    return cols;
}

function roundNumber(num, dec) {
    if (num == "." || num == ".0" || num == "0" || num == "0." || num == "" || !num) {
        if (dec == "0")
            return "0";
        else if (dec == "1")
            return "0.0"
        else if (dec == "2")
            return "0.00"
    }
    var result = Math.round(num * Math.pow(10, dec)) / Math.pow(10, dec);
    if (isNaN(result))
        result = "0";
    var len = 0;
    var number = result.toString();
    len = number.length;
    if (number == "." || number == ".0" || number == "0" || number == "0." || num == "" || !num) {
        if (dec == "0")
            return "0";
        else if (dec == "1")
            return "0.0"
        else if (dec == "2")
            return "0.00"
    }
    switch (dec) {
        case 2:
            if (number.indexOf('.') == parseInt(len - 2) && len >= 2) {
                number = number + "0";
            } else if (number.indexOf('.') == parseInt(len - 1)) {
                number = number + "00";
            } else if (number.indexOf('.') == -1) {
                number = number + ".00";
            }
            break;
        case 1:
            if (number.indexOf('.') == parseInt(len)) {
                number = number + "0";
            } else if (number.indexOf('.') == -1) {
                number = number + ".0";
            }
            break;
    }
    return number;
}

function GetType(DDLName, str, url) {
    var tag_list = $.ajax({
        url: url,
        dataType: 'json',
        data: {
            str: str
        },
        success: function (response) {
            $("#" + DDLName).select2({
                tags: true,
                data: response,
                placeholder: "Search for a Type",
                multiple: true,
                allowClear: true
            });
        }
    });
}
function GetTypeFirstSelect(DDLName, str, url) {
    var tag_list = $.ajax({
        url: url,
        dataType: 'json',
        data: {},
        success: function (response) {
            $("#" + DDLName).select2({
                data: response,
                placeholder: "Search for a Type",
                allowClear: true
            });
            if (response && response.length > 0 && response[0].id != null) {
                $("#" + DDLName).val(response[0].id).trigger('change');  // Select the first item if valid
            }
        }
    });
}
function GetTypeOne(DDLName, str, url) {
    var tag_list = $.ajax({
        url: url,
        dataType: 'json',
        data: {
            str: str
        },
        success: function (response) {
            $("#" + DDLName).select2({
                data: response,
                placeholder: "Search for a Type",
                allowClear: true
            });
        }
    });
}
function GetVendorCode(DDLName) {
    $("#" + DDLName).select2({
        placeholder: "Search for a Vendor",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            url: DomainName + '/Master/SearchVendorListJson',
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term
                };
            },
            results: function (data) {
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.id);
        },
        escapeMarkup: function (m) {
            return m;
        },
        dropdownCssClass: "bigdrop"
    });
}

function GetVendorName(DDLName) {
    $("#" + DDLName).select2({
        placeholder: "Search for a Vendor",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            url: DomainName + '/Master/SearchVendorListJson',
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term
                };
            },
            results: function (data) {
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.text);
        },
        escapeMarkup: function (m) {
            return m;
        },
        dropdownCssClass: "bigdrop"
    });
}

function GetPartyName(DDLName) {
    $("#" + DDLName).select2({
        placeholder: "Search for a customer",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            url: DomainName + '/Finance/GetCustomer',
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term
                };
            },
            results: function (data) {
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.text);
        },
        escapeMarkup: function (m) {
            return m;
        },
        dropdownCssClass: "bigdrop"
    });
}

function GetVehicleEmployeeUserName(DDLName, Type) {
    $("#" + DDLName).select2({
        placeholder: "Search for a customer",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            url: DomainName + '/Voucher/GetCustUserEmployeeVehicle',
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term,
                    Type: Type
                };
            },
            results: function (data) {
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.text);
        },
        escapeMarkup: function (m) {
            return m;
        },
        dropdownCssClass: "bigdrop"
    });
}

function GetAccountCode(DDLName) {
    $("#" + DDLName).select2({
        placeholder: "Search for account code",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            url: DomainName + '/Voucher/GetAccountCode',
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term
                };
            },
            results: function (data) {
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.text);
        },
        escapeMarkup: function (m) {
            return m;
        },
        dropdownCssClass: "bigdrop"
    });
}

function GetLocationNameCode(DomainName, DDLName) {
    var StrURL = DomainName + "/Operation/GetLocationSeachList";
    JsonDDL(DDLName, StrURL);
}

function rounditn(Num, decplaces) {
    Places = decplaces
    if (Places > 0) {
        if ((Num.toString().length - Num.toString().lastIndexOf('.')) > (Places + 1)) {
            if (Num.toString().lastIndexOf('.') < 0) {
                return Num.toString() + '.00';
            }
            var Rounder = Math.pow(10, Places);
            return Math.round(Num * Rounder) / Rounder;
        } else {
            if (Num.toString().lastIndexOf('.') < 0) {
                return Num.toString() + '.00';
            } else {
                if (Num.toString().lastIndexOf('.') + 1 == Num.toString().length - 1)
                    return Num.toString() + '0';
                else
                    return Num.toString();
            }
        }
    } else return Math.round(Num);
}

function GCView(GCNo) {
    var ReportURL = '@ViewBag.ReportURL';
    var strWinFeature = "menubar=no,toolbar=no,location=no,resizable=yes,scrollbars=yes,width=800 ,height=600,status=no,left=60,top=50"
    var strPopupURL = ReportURL + DomainName + "/ViewPrint/DocketViewPrint?Type=1&DocketNo=" + GCNo;
    winNew = window.open(strPopupURL, "_blank", strWinFeature)
    return false;
}

function JsonDDLWithoutInput(DDLName, strUrl) {
    $("#" + DDLName + " ,." + DDLName).select2({
        placeholder: "Search Here",
        allowClear: true,
        //multiple: IsMultiple,
        //tags: [],
        ajax: {
            url: strUrl,
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term
                };
            },
            results: function (data) { // parse the results into the format expected by Select2.
                // since we are using custom formatting functions we do not need to alter remote JSON data
                //  alert("Hello");
                // alert(data);
                //    alert(data.CMP);
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.id + " - " + item.text);
        },
        escapeMarkup: function (m) {
            return m;
        },
        //dropdownCssClass: "bigdrop" // apply css that makes the dropdown taller
    });
}

function GetPartyName_BranchWise(DDLName) {

    $("#" + DDLName).select2({
        placeholder: "Search for a customer",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            url: DomainName + '/Voucher/GetCustomer_BranchWise',
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term
                };
            },
            results: function (data) { // parse the results into the format expected by Select2.
                // since we are using custom formatting functions we do not need to alter remote JSON data
                //  alert("Hello");
                // alert(data);
                //    alert(data.CMP);
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.text);
        },
        escapeMarkup: function (m) {
            return m;
        },
        dropdownCssClass: "bigdrop" // apply css that makes the dropdown taller
    });
}

function GetVehicleEmployeeUserName_BranchWise(DDLName, Type) {

    $("#" + DDLName).select2({
        placeholder: "Search for a customer",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            url: DomainName + '/Voucher/GetCustUserEmployeeVehicle_BranchWise',
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term,
                    Type: Type
                };
            },
            results: function (data) { // parse the results into the format expected by Select2.
                // since we are using custom formatting functions we do not need to alter remote JSON data
                //  alert("Hello");
                // alert(data);
                //    alert(data.CMP);
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.text);
        },
        escapeMarkup: function (m) {
            return m;
        },
        dropdownCssClass: "bigdrop" // apply css that makes the dropdown taller
    });
}

function BackDateControl(StartDate, EndDate) {

    $(".clsBackDateControl").datepicker({
        rtl: App.isRTL(),
        format: "dd MM yyyy",
        showMeridian: true,
        autoclose: true,
        pickerPosition: (App.isRTL() ? "bottom-right" : "bottom-left"),
        startDate: new Date(StartDate),
        endDate: new Date(EndDate),
        todayBtn: true
    });
}

function GetVehicleEmployeeUserName_Type_Wise(DDLName, Type, SubType) {

    $("#" + DDLName).select2({
        placeholder: "Search for a customer",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            url: DomainName + '/BranchAccounting/GetVehicleEmployeeUserName_Type_Wise',
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term,
                    Type: Type,
                    SubType: SubType
                };
            },
            results: function (data) { // parse the results into the format expected by Select2.
                // since we are using custom formatting functions we do not need to alter remote JSON data
                //  alert("Hello");
                // alert(data);
                //    alert(data.CMP);
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.text);
        },
        escapeMarkup: function (m) {
            return m;
        },
        dropdownCssClass: "bigdrop" // apply css that makes the dropdown taller
    });
}

function GetDistace(source, destination, IsCity, LocationName, LocationCode, TotalKMID, GoogleHoursID) {

    App.blockUI({ boxed: true });

    var strurl = DomainName + "/BranchAccounting/GetDistanceFromTable?FromCity=" + source + "&ToCity=" + destination + "&IsCity=" + IsCity + "&LocationName=" + LocationName + "&LocationCode=" + LocationCode;

    if (source != destination) {
        $.ajax({
            url: strurl,
            cache: false,
            data: {},
            type: 'POST',
            success: function (data) {

                if (data.TotalKm == 0) {

                    var directionsDisplay;
                    var directionsService = new google.maps.DirectionsService();

                    google.maps.event.addDomListener(window, 'load', function () {

                        new google.maps.places.SearchBox(source);

                        new google.maps.places.SearchBox(destination);
                        directionsDisplay = new google.maps.DirectionsRenderer({ 'draggable': true });
                    });

                    var mumbai = new google.maps.LatLng(18.9750, 72.8258);
                    var mapOptions = {
                        zoom: 7,
                        center: mumbai
                    };

                    //*********DIRECTIONS AND ROUTE**********************//

                    //source = document.getElementById("CWE_FromCity").value;

                    //destination = document.getElementById("CWE_ToCity").value;

                    //destination = document.getElementById("CWE_ToCity").value;
                    if (source == "korba" || source == "KORBA") {
                        source = "korba(India)";
                    }
                    else {
                        source = source;
                    }

                    if (destination == "korba" || destination == "KORBA") {
                        destination = "korba(India)";
                    }
                    else {
                        destination = destination;
                    }

                    if (source == "SALTLAKE" || source == "saltlake") {
                        source = "SALTLAKE(India)";
                    }
                    else {
                        source = source;
                    }

                    if (destination == "SALTLAKE" || destination == "saltlake") {
                        destination = "SALTLAKE(India)";
                    }
                    else {
                        destination = destination;
                    }

                    if (source == "PATAN" || source == "patan") {
                        source = "PATAN(Gujarat)";
                    }
                    else {
                        source = source;
                    }
                    if (destination == "PATAN" || destination == "patan") {
                        destination = "PATAN(Gujarat)";
                    }
                    else {
                        destination = destination;
                    }

                    if (source == "DIU" || source == "diu") {
                        source = "DIU(Gujarat)";
                    }
                    else {
                        source = source;
                    }
                    if (destination == "DIU" || destination == "diu") {
                        destination = "DIU(Gujarat)";
                    }
                    else {
                        destination = destination;
                    }

                    if (source == "WELLINGTON" || source == "WELLINGTON") {
                        source = "WELLINGTON(tamilnadu)";
                    }
                    else {
                        source = source;
                    }
                    if (destination == "WELLINGTON" || destination == "WELLINGTON") {
                        destination = "WELLINGTON(tamilnadu)";
                    }
                    else {
                        destination = destination;
                    }

                    if (source == "REWA" || source == "REWA") {
                        source = "REWA(madhyapradesh)";
                    }
                    else {
                        source = source;
                    }
                    if (destination == "REWA" || destination == "REWA") {
                        destination = "REWA(madhyapradesh)";
                    }
                    else {
                        destination = destination;
                    }

                    if (source == "KASHMIR" || source == "KASHMIR") {
                        source = "KASHMIR(Himachal Pradesh)";
                    }
                    else {
                        source = source;
                    }
                    if (destination == "KASHMIR" || destination == "KASHMIR") {
                        destination = "KASHMIR(Himachal Pradesh)";
                    }
                    else {
                        destination = destination;
                    }
                    if (source == "MOHANA" || source == "MOHANA") {
                        source = "Mohana, Madhya Pradesh";
                    }
                    else {
                        source = source;
                    }
                    if (destination == "MOHANA" || destination == "MOHANA") {
                        destination = "Mohana, Madhya Pradesh";
                    }
                    else {
                        destination = destination;
                    }

                    if (source == "JAGRAWAN" || source == "JAGRAWAN") {
                        source = "Jagrawan, Punjab";
                    }
                    else {
                        source = source;
                    }
                    if (destination == "JAGRAWAN" || destination == "JAGRAWAN") {
                        destination = "Jagrawan, Punjab";
                    }
                    else {
                        destination = destination;
                    }
                    var request = {
                        origin: source,
                        destination: destination,
                        travelMode: google.maps.TravelMode.DRIVING
                    };

                    source = source + ',India';
                    destination = destination + ',India';

                    //*********DISTANCE AND DURATION**********************//
                    //if (source == destination) {
                    //    document.getElementById(""+TotalKMID).value = "30";
                    //}
                    //else {
                    var service = new google.maps.DistanceMatrixService();
                    service.getDistanceMatrix({
                        origins: [source],
                        destinations: [destination],
                        travelMode: google.maps.TravelMode.DRIVING,
                        unitSystem: google.maps.UnitSystem.METRIC,
                        avoidHighways: false,
                        avoidTolls: false
                    }, function (response, status) {
                        if (status == google.maps.DistanceMatrixStatus.OK && response.rows[0].elements[0].status != "ZERO_RESULTS") {
                            var distance = response.rows[0].elements[0].distance.text;
                            var duration = response.rows[0].elements[0].duration.text;

                            var TotalKM = distance.replace("km", "").replace(" ", "").replace(",", "").replace("m", "");
                            duration = duration.toUpperCase();
                            var TotalTime = duration.replace("DAY", "DAY ");

                            document.getElementById("" + TotalKMID).value = parseInt(TotalKM);
                            document.getElementById("" + GoogleHoursID).value = (TotalTime);

                            var strurl12 = DomainName + "/BranchAccounting/InsertDistanceFromTable?FromCity=" + source + "&ToCity=" + destination + "&TotalKM=" + TotalKM + "&Duration=" + TotalTime + "&IsCity=" + IsCity + "&LocationName=" + LocationName + "&LocationCode=" + LocationCode;

                            $.ajax({
                                url: strurl12,
                                cache: false,
                                data: {},
                                type: 'POST',
                                success: function (data) {
                                }
                            });

                            $("#" + TotalKMID).prop("readonly", true)
                            App.unblockUI();
                            if (parseInt(TotalKM) < 31) {
                                $("#" + TotalKMID).val("30");
                                $("#" + GoogleHoursID).val("1");
                                App.unblockUI();
                            }
                        }

                        else if (status == google.maps.DistanceMatrixStatus.OK && response.rows[0].elements[0].status == "ZERO_RESULTS") {
                            if (source != "" && destination != "") {
                                $("#" + TotalKMID).prop("readonly", false)
                                App.unblockUI();
                            }
                        }
                        else {
                            alert("Unable to find the distance via road.");
                            App.unblockUI();
                        }
                    });
                    //  }
                }
                else {
                    document.getElementById("" + TotalKMID).value = data.TotalKm;
                    document.getElementById("" + GoogleHoursID).value = data.TotalTime;

                    $("#" + TotalKMID).prop("readonly", true)
                    App.unblockUI();
                    if (parseInt(data.TotalKm) < 31) {
                        $("#" + TotalKMID).val("30");
                        $("#" + GoogleHoursID).val("1");
                        App.unblockUI();
                    }
                }
            },
            error: function (req, status, error) {
                alert("error--" + req + "---" + status + "---" + error);
                App.unblockUI();
            }
        });
    }
    else {
        document.getElementById("" + TotalKMID).value = "30";
        document.getElementById("" + GoogleHoursID).value = "1";
        App.unblockUI();
    }
}

function GetVendor_Wise(DDLName, Type) {

    $("#" + DDLName).select2({
        placeholder: "Search for a Driver",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            url: DomainName + '/BranchAccounting/GetVendor_Wise',
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term,
                    Type: Type
                };
            },
            results: function (data) { // parse the results into the format expected by Select2.
                // since we are using custom formatting functions we do not need to alter remote JSON data
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.text);
        },
        escapeMarkup: function (m) {
            return m;
        },
        dropdownCssClass: "bigdrop" // apply css that makes the dropdown taller
    });
}

function Get_Driver_BranchWise(DDLName, Type) {

    $("#" + DDLName).select2({
        placeholder: "Search for a Driver",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            url: DomainName + '/BranchAccounting/Get_Driver_BranchWise',
            dataType: 'json',
            data: function (term) {
                return {
                    searchTerm: term,
                    Type: Type
                };
            },
            results: function (data) { // parse the results into the format expected by Select2.
                // since we are using custom formatting functions we do not need to alter remote JSON data
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.id + ' </b>- ' + item.text + '</div>');
        },
        formatSelection: function (item) {
            return (item.text);
        },
        escapeMarkup: function (m) {
            return m;
        },
        dropdownCssClass: "bigdrop" // apply css that makes the dropdown taller
    });
}

//----------------------------Location Control
var LocationContraol = function () {
    $(document).on("change", ".clsLocation", function () {

        $("#Location").val($(this).val());
        App.blockUI({ boxed: true });
        var StrURL1 = DomainName + "/Master/GetLocationLevel/" + $(this).val();
        $.ajax({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            url: StrURL1,
            dataType: 'json',
            data: {},
            success: function (data1) {


                $("#LocationLevel").val(data1.LocationLevel);
                App.unblockUI();
            },
            error: function () {
                alert("error");
            }
        });

        var ContId = parseInt($(this).attr("id").replace("LOC", "")) + 1;

        var StrURL = DomainName + "/Master/GetLocationFromRegion/" + $(this).val();
        FillDropDownfromOtherSelectFirst("LOC" + ContId, StrURL, "Location");
    });

    $(document).on("change", ".clsLocationZone", function () {
        $("#Location").val($(this).val());
        App.blockUI({ boxed: true });
        var StrURL1 = DomainName + "/Master/GetLocationZoneLevel/" + $(this).val();
        $.ajax({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            url: StrURL1,
            dataType: 'json',
            data: {},
            success: function (data1) {
                $("#LocationLevel").val(data1.LocationLevel);
                App.unblockUI();
            },
            error: function () {
                alert("error");
            }
        });

        var ContId = parseInt($(this).attr("id").replace("LOC", "")) + 1;

        var StrURL = DomainName + "/Master/GetLocationZoneFromRegion/" + $(this).val();
        FillDropDownfromOtherSelectFirst("LOC" + ContId, StrURL, "Location");
    });
}

var Set_Remove_Default_Zero_Values = function (DomainName) {

    $(document).on("focus", ".cls_Remove_Zero", function () {
        var Total_Weight = parseInt($(this).val())
        if (Total_Weight == 0) {
            $(this).val("");
        }
    });

    $(document).on("focusout", ".cls_Set_Zero", function () {
        var Total_Weight = $(this).val()
        if (Total_Weight == "" || Total_Weight == null) {
            $(this).val("0.00");
        }
    });

    $(document).on("focus", ".cls_Set_Remove_Zero", function () {
        var Total_Weight = parseInt($(this).val())
        if (Total_Weight == 0) {
            $(this).val("");
        }
    });

    $(document).on("focusout", ".cls_Set_Remove_Zero", function () {
        var Total_Weight = $(this).val()
        if (Total_Weight == "" || Total_Weight == null) {
            $(this).val("0.00");
        }
    });
}

function generateRandomString(lenth) {
    var text = "";
    var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    for (var i = 0; i < lenth; i++)
        text += possible.charAt(Math.floor(Math.random() * possible.length));

    return text;
}

var MONTH_NAMES = new Array('January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December', 'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec');
var DAY_NAMES = new Array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat');
function LZ(x) { return (x < 0 || x > 9 ? "" : "0") + x }

function formatDate(date, format) {
    format = format + "";
    var result = "";
    var i_format = 0;
    var c = "";
    var token = "";
    var y = date.getYear() + "";
    var M = date.getMonth() + 1;
    var d = date.getDate();
    var E = date.getDay();
    var H = date.getHours();
    var m = date.getMinutes();
    var s = date.getSeconds();
    var yyyy, yy, MMM, MM, dd, hh, h, mm, ss, ampm, HH, H, KK, K, kk, k;
    // Convert real date parts into formatted versions
    var value = new Object();
    if (y.length < 4) { y = "" + (y - 0 + 1900); }
    value["y"] = "" + y;
    value["yyyy"] = y;
    value["yy"] = y.substring(2, 4);
    value["M"] = M;
    value["MM"] = LZ(M);
    value["MMM"] = MONTH_NAMES[M - 1];
    value["NNN"] = MONTH_NAMES[M + 11];
    value["d"] = d;
    value["dd"] = LZ(d);
    value["E"] = DAY_NAMES[E + 7];
    value["EE"] = DAY_NAMES[E];
    value["H"] = H;
    value["HH"] = LZ(H);
    if (H == 0) { value["h"] = 12; }
    else if (H > 12) { value["h"] = H - 12; }
    else { value["h"] = H; }
    value["hh"] = LZ(value["h"]);
    if (H > 11) { value["K"] = H - 12; } else { value["K"] = H; }
    value["k"] = H + 1;
    value["KK"] = LZ(value["K"]);
    value["kk"] = LZ(value["k"]);
    if (H > 11) { value["a"] = "PM"; }
    else { value["a"] = "AM"; }
    value["m"] = m;
    value["mm"] = LZ(m);
    value["s"] = s;
    value["ss"] = LZ(s);
    while (i_format < format.length) {
        c = format.charAt(i_format);
        token = "";
        while ((format.charAt(i_format) == c) && (i_format < format.length)) {
            token += format.charAt(i_format++);
        }
        if (value[token] != null) { result = result + value[token]; }
        else { result = result + token; }
    }
    return result;
}

function errorResult(jqXHR, exception) {
    var msg = '';
    if (jqXHR.status === 0) {
        msg = 'Not connect.\nVerify Network.';
    } else if (jqXHR.status == 404) {
        msg = 'Requested page not found. [404]';
    } else if (jqXHR.status == 500) {
        msg = 'Internal Server Error [500].';
    } else if (exception === 'parsererror') {
        msg = 'Requested JSON parse failed.';
    } else if (exception === 'timeout') {
        msg = 'Time out error.';
    } else if (exception === 'abort') {
        msg = 'Ajax request aborted.';
    } else {
        msg = 'Uncaught Error.\n' + jqXHR.responseText;
    }
    return msg;
}

/* Bind Menu for Quick Access*/
function formatOption(option) {
    var $option = $(
        '<div><strong>' + option.text + '</strong></div><div style="font-size: 10px;">' + option.element[0].title + '</div>'
    );
    return $option;
}

function bindDropDownMenu(DDLName) {
    var Parameters = {
        "UserId": BaseUserName
    }
    $("#" + DDLName).attr("disabled", true);
    var strUrl = DomainName + "/Account/GetDataWithoutAlias";
    $.ajax({
        url: strUrl,
        type: 'POST',
        dataType: "json",
        async: true,
        data: {
            Method: "USP_Searchlist",
            parameters: Parameters
        },
        success: function (data) {
            var Headers = GetHeaders(data[0]);
            $("#" + DDLName).get(0).options.length = 0;
            $("#" + DDLName).get(0).options[0] = new Option("Loading names", "-1");
            $("#" + DDLName).find("option").remove();
            $("#" + DDLName).select2("data", null);
            $("#" + DDLName).empty();
            $("#" + DDLName).append('<option value="">Select Menu</option>');
            for (i = 0; i < data.length; i++) {
                $("#" + DDLName).append('<option value="' + data[i][Headers[0].data] + '" title="' + data[i]["Hierarchy"] + '" > ' + data[i][Headers[1].data] + '</option > ');
                $("#" + DDLName).trigger("liszt:updated");
            }
            //App.unblockUI();
            $("#" + DDLName).select2({
                placeholderOption: 'first',
                formatResult: formatOption,
            });
            $("#" + DDLName).trigger("liszt:updated");
            $("#" + DDLName).val($("#target option:first").val());
            $("#" + DDLName).attr("disabled", false);
        },
        error: function (req, status, error) {
            TosterNotification("error", 'Operation fail..!! There is some issue while loading data. Please try again later..', "Oops..!!");
            //App.unblockUI();
        }
    });
}

function bindDropDownMenuJsonDDLSelectCodeWithName(DDLName, strUrl) {
    strUrl = DomainName + "/Account/GetDataWithoutAlias";
    $("#" + DDLName + " ,." + DDLName).select2({
        placeholder: "Search Here",
        minimumInputLength: 3,
        allowClear: true,
        ajax: {
            url: strUrl,
            dataType: 'json',
            data: function (term) {
                return {
                    //searchTerm: term
                    Method: "USP_Searchlist",
                    parameters: {
                        "UserId": BaseUserName,
                        "searchTerm": term
                    }
                };
            },
            results: function (data) {
                return {
                    results: data
                };
            }
        },
        initSelection: function (item, callback) {
            var id = item.val();
            var text = item.data('option');
            var data = {
                id: id,
                text: text
            };
            callback(data);
        },
        formatResult: function (item) {
            return ('<div><b>' + item.text + '</b></div>');
        },
        formatSelection: function (item) {
            return ('<b>' + item.text + '</b>');
        },
        escapeMarkup: function (m) {
            return m;
        },
    });
}

function validateExchangeRate(input) {
    var value = parseFloat(input.value);

    if (isNaN(value) || value === 0) {
        TosterNotification("error", "Exchange rate cannot be 0..!!", "Error");
        input.value = "1.00";
    }
}

$(document).ready(function () {
    // Dynamic Menu Active Route Highlighting (Prevents partial matches, handles AddEdit redirect pages)
    var url = window.location.pathname.toLowerCase();

    // Normalize AddEdit pages (e.g. /Master/AddEditUser -> /Master/User) to match the list page (e.g. /Master/UserList)
    var cleanedUrl = url.replace('_addedit', '').replace('addedit', '');

    var activeLink = $('#navbar-nav a').filter(function () {
        var linkUrl = $(this).attr('href');
        if (!linkUrl || linkUrl === '#') return false;
        linkUrl = linkUrl.toLowerCase();

        // 1. Exact URL match (Direct navigation)
        if (linkUrl === url) return true;

        // 2. AddEdit remapped match (e.g. AddEditUser redirects -> highlight UserList in sidebar)
        if (cleanedUrl !== url && linkUrl !== "" && linkUrl.indexOf(cleanedUrl) === 0) return true;

        // 3. Sub-route segment match (e.g. /Master/Card/Details -> highlight /Master/Card)
        if (url !== "/" && linkUrl !== "" && url.indexOf(linkUrl + "/") === 0) return true;

        return false;
    }).first();

    if (activeLink.length > 0) {
        activeLink.addClass("active");
        activeLink.parents('.collapse').addClass('show');
        activeLink.parents('.nav-item').children('a[data-bs-toggle="collapse"]')
            .removeClass('collapsed')
            .attr('aria-expanded', 'true')
            .addClass('active');
    }
});

/*
onblur = "ValidateField(this.id, 'email')"
onblur = "ValidateField(this.id, 'mobile')"
onblur = "ValidateField(this.id, 'pincode')"
onblur = "ValidateField(this.id, 'ifsc')"
onblur = "ValidateField(this.id, 'pan')"
onblur = "ValidateField(this.id, 'gst')"
*/
function ValidateField(id, type) {

    var el = document.getElementById(id);
    var value = el.value.trim().toUpperCase();

    var msg = "";

    // 👉 remove old error
    var oldError = el.parentElement.querySelector(".invalid-feedback");
    if (oldError) oldError.remove();

    el.classList.remove("is-invalid");

    if (value === "") return true;

    switch (type) {

        case "email":
            if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
                msg = "Please enter valid email address";
            }
            break;

        case "pincode":
            if (value.length !== 6) {
                msg = "Please enter 6 digit pin code";
            } else if (!/^\d+$/.test(value) || value.startsWith("0")) {
                msg = "Please enter valid pin code";
            }
            break;

        case "mobile":
            if (value.length !== 10) {
                msg = "Mobile No. should be 10 digits";
            } else if (!/^\d+$/.test(value) || value[0] < '6') {
                msg = "Invalid mobile number";
            }
            break;

        case "ifsc":
            if (!/^[A-Z]{4}0[A-Z0-9]{6}$/.test(value)) {
                msg = "Invalid IFSC code";
            }
            break;

        case "pan":
            if (!/^[A-Z]{5}[0-9]{4}[A-Z]{1}$/.test(value)) {
                msg = "Invalid PAN number";
            }
            break;

        case "gst":
            if (!/^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$/.test(value)) {
                msg = "Invalid GST number";
            }
            break;
    }

    if (msg !== "") {
        el.classList.add("is-invalid");

        var span = document.createElement("span");
        span.className = "invalid-feedback";
        span.innerText = msg;

        el.parentElement.appendChild(span);

        el.focus();
        return false;
    }

    // valid → set formatted value
    el.value = value;
    return true;
}

function DownloadMasterReport(DomainName, MethodName, fileName) {

    var strURL = DomainName + "/Master/ExportReport?MethodName=" + MethodName + "&FileName=" + fileName;
    App.blockUI({ boxed: true });
    setTimeout(function () {
        window.location.href = strURL;
        setTimeout(function () { App.unblockUI(); }, 1500);
    }, 200);
}

function validateImageFile(file, inputElement) {
    debugger
    if (!file) return true;

    var fileType = file.type || "";
    var isImage = fileType.startsWith("image/");
    var isPdf = fileType === "application/pdf";

    // Type Validation (Only images and PDFs)
    if (!isImage && !isPdf) {
        TosterNotification("warning", 'Operation fail..!! Please select a valid image or PDF.', "Oops..!!");
        if (inputElement) {
            $(inputElement).val("");
        }
        return false;
    }

    // Size Validation (Max 4 MB)
    var maxSizeBytes = 4 * 1024 * 1024;
    if (file.size > maxSizeBytes) {
        TosterNotification("warning", 'Operation fail..!! File size should not exceed 4 MB.', "Oops..!!");
        if (inputElement) {
            $(inputElement).val("");
        }
        return false;
    }

    return true;
}

