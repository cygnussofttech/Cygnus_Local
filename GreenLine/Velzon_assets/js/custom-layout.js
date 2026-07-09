window.history.forward(0);

$(window).load(function () {
    $('#loading').fadeOut();
});

$(document).ready(function () {
    //StartWarningTimer();
});

$("body").on('click keypress mousemove', function () {
    if (typeof ResetTimeOutTimer === 'function') {
        ResetTimeOutTimer();
    }
});

var timoutWarning = 3540000; // Display warning in 59 Mins.
var timoutNow = 60000; // Warning has been shown, give the user 1 minute to interact

var warningTimer;
var timeoutTimer;

// Start warning timer.
function StartWarningTimer() {
    warningTimer = setTimeout("IdleWarning()", timoutWarning);
}

// Reset timers.
function ResetTimeOutTimer() {
    clearTimeout(timeoutTimer);
    StartWarningTimer();
    if ($("#timeout").length) {
        $("#timeout").dialog('close');
    }
}

// Show idle timeout warning dialog.
function IdleWarning() {
    clearTimeout(warningTimer);
    timeoutTimer = setTimeout("IdleTimeout()", timoutNow);
    if ($("#timeout").length) {
        $("#timeout").dialog({
            modal: true
        });
    }
}

// Logout the user.
function IdleTimeout() {
    if (typeof logoutUrl !== 'undefined') {
        window.location = logoutUrl;
    }
}

// Icon mapping for menu items based on common keywords
function getMenuIcon(titleText) {
    var t = titleText.toLowerCase();
    if (t.indexOf('dashboard') > -1 || t.indexOf('home') > -1) return 'ri-dashboard-2-line';
    if (t.indexOf('master') > -1) return 'ri-database-2-line';
    if (t.indexOf('purchase') > -1 || t.indexOf('buying') > -1) return 'ri-shopping-cart-2-line';
    if (t.indexOf('sale') > -1 || t.indexOf('selling') > -1) return 'ri-store-2-line';
    if (t.indexOf('stock') > -1 || t.indexOf('inventory') > -1 || t.indexOf('warehouse') > -1) return 'ri-stack-line';
    if (t.indexOf('account') > -1 || t.indexOf('finance') > -1 || t.indexOf('ledger') > -1) return 'ri-money-dollar-circle-line';
    if (t.indexOf('report') > -1 || t.indexOf('mis') > -1) return 'ri-bar-chart-box-line';
    if (t.indexOf('setting') > -1 || t.indexOf('config') > -1 || t.indexOf('admin') > -1) return 'ri-settings-3-line';
    if (t.indexOf('user') > -1 || t.indexOf('employee') > -1 || t.indexOf('hr') > -1) return 'ri-user-settings-line';
    if (t.indexOf('production') > -1 || t.indexOf('manufacturing') > -1) return 'ri-tools-line';
    if (t.indexOf('transport') > -1 || t.indexOf('vehicle') > -1 || t.indexOf('logistics') > -1) return 'ri-truck-line';
    if (t.indexOf('quality') > -1 || t.indexOf('qc') > -1) return 'ri-shield-check-line';
    if (t.indexOf('order') > -1) return 'ri-file-list-3-line';
    if (t.indexOf('dispatch') > -1 || t.indexOf('delivery') > -1) return 'ri-send-plane-line';
    if (t.indexOf('gate') > -1 || t.indexOf('security') > -1) return 'ri-door-lock-line';
    if (t.indexOf('payroll') > -1 || t.indexOf('salary') > -1) return 'ri-wallet-3-line';
    if (t.indexOf('crm') > -1 || t.indexOf('customer') > -1) return 'ri-contacts-book-line';
    if (t.indexOf('project') > -1 || t.indexOf('task') > -1) return 'ri-task-line';
    if (t.indexOf('mail') > -1 || t.indexOf('email') > -1) return 'ri-mail-line';
    if (t.indexOf('calendar') > -1) return 'ri-calendar-2-line';
    if (t.indexOf('utility') > -1 || t.indexOf('tool') > -1) return 'ri-hammer-line';
    // fallback icons cycle
    var fallbackIcons = [
        'ri-apps-2-line', 'ri-folder-2-line', 'ri-layout-3-line',
        'ri-pie-chart-line', 'ri-honour-line', 'ri-compass-3-line',
        'ri-bookmark-line', 'ri-pages-line'
    ];
    return fallbackIcons[Math.floor(Math.random() * fallbackIcons.length)];
}

// Build nested submenus recursively
function buildSubMenu(parentUl, depth) {
    parentUl.find('> li').each(function(subIndex) {
        var $subLi = $('<li>').addClass('nav-item');
        var $subAold = $(this).find('> a').first();
        var subText = $subAold.find('> span.title').text() || $subAold.text().trim();
        var subHref = $subAold.attr('href') || '#';
        var $innerSub = $(this).find('> ul.sub-menu');

        if ($innerSub.length > 0) {
            var innerTargetId = 'sidebarSub' + depth + '_' + subIndex + '_' + Math.floor(Math.random() * 10000);
            var $subA = $('<a>').addClass('nav-link').attr({
                'href': '#' + innerTargetId,
                'data-bs-toggle': 'collapse',
                'role': 'button',
                'aria-expanded': 'false',
                'aria-controls': innerTargetId
            }).text(subText);

            var $innerDiv = $('<div>').addClass('collapse menu-dropdown').attr('id', innerTargetId);
            var $innerUl = $('<ul>').addClass('nav nav-sm flex-column');
            buildSubMenu($innerSub, depth + 1).appendTo($innerDiv);
            $innerDiv.find('> ul').remove();
            $innerDiv.append($innerUl);
            buildSubMenuItems($innerSub, $innerUl);

            $subLi.append($subA).append($innerDiv);
        } else {
            var $subA = $('<a>').addClass('nav-link').attr('href', subHref).text(subText);
            $subLi.append($subA);
        }
        parentUl.parent().find('> ul.nav').append($subLi);
    });
}

function buildSubMenuItems($sourceUl, $targetUl) {
    $sourceUl.find('> li').each(function() {
        var $subLi = $('<li>').addClass('nav-item');
        var $subAold = $(this).find('> a').first();
        var subText = $subAold.find('> span.title').text() || $subAold.text().trim();
        var subHref = $subAold.attr('href') || '#';
        var $subA = $('<a>').addClass('nav-link').attr('href', subHref).text(subText);
        $subLi.append($subA);
        $targetUl.append($subLi);
    });
}

// Menu Transformer Script
$(document).ready(function() {
    var $oldMenu = $('#tempMenuContainer > ul.page-sidebar-menu');
    if ($oldMenu.length > 0) {
        var $newMenu = $('#navbar-nav');

        // Add "Menu" title header like Velzon
        $newMenu.append('<li class="menu-title"><span data-key="t-menu">Menu</span></li>');

        // Extract and transform each top-level menu item
        $oldMenu.find('> li').each(function(index) {
            // Skip separators or empty items
            var $origA = $(this).find('> a').first();
            if (!$origA.length) return;

            var $li = $('<li>').addClass('nav-item');

            // Get title text
            var $title = $origA.find('> span.title');
            var titleText = $title.text() || $origA.text().trim();
            if (!titleText || titleText === '') return;

            // Choose icon based on menu title
            var iconClass = getMenuIcon(titleText);

            var $submenu = $(this).find('> ul.sub-menu');
            if ($submenu.length > 0) {
                var targetId = 'sidebarMenu' + index;
                var $a = $('<a>').addClass('nav-link menu-link').attr({
                    'href': '#' + targetId,
                    'data-bs-toggle': 'collapse',
                    'role': 'button',
                    'aria-expanded': 'false',
                    'aria-controls': targetId
                }).html('<i class="' + iconClass + '"></i> <span data-key="t-menu-' + index + '">' + titleText + '</span>');

                var $subDiv = $('<div>').addClass('collapse menu-dropdown').attr('id', targetId);
                var $subUl = $('<ul>').addClass('nav nav-sm flex-column');

                $submenu.find('> li').each(function(subIndex) {
                    var $subLi = $('<li>').addClass('nav-item');
                    var $subAold = $(this).find('> a').first();
                    var subText = $subAold.find('> span.title').text() || $subAold.text().trim();
                    var subHref = $subAold.attr('href') || '#';

                    // Check for deeper nested sub-menu
                    var $deepSub = $(this).find('> ul.sub-menu');
                    if ($deepSub.length > 0) {
                        var deepTargetId = 'sidebarSub' + index + '_' + subIndex;
                        var $subA = $('<a>').addClass('nav-link').attr({
                            'href': '#' + deepTargetId,
                            'data-bs-toggle': 'collapse',
                            'role': 'button',
                            'aria-expanded': 'false',
                            'aria-controls': deepTargetId
                        }).text(subText);

                        var $deepDiv = $('<div>').addClass('collapse menu-dropdown').attr('id', deepTargetId);
                        var $deepUl = $('<ul>').addClass('nav nav-sm flex-column');

                        $deepSub.find('> li').each(function() {
                            var $deepLi = $('<li>').addClass('nav-item');
                            var $deepAold = $(this).find('> a').first();
                            var deepText = $deepAold.find('> span.title').text() || $deepAold.text().trim();
                            var deepHref = $deepAold.attr('href') || '#';
                            var $deepA = $('<a>').addClass('nav-link').attr('href', deepHref).text(deepText);
                            $deepLi.append($deepA);
                            $deepUl.append($deepLi);
                        });

                        $deepDiv.append($deepUl);
                        $subLi.append($subA).append($deepDiv);
                    } else {
                        var $subA = $('<a>').addClass('nav-link').attr('href', subHref).text(subText);
                        $subLi.append($subA);
                    }
                    $subUl.append($subLi);
                });

                $subDiv.append($subUl);
                $li.append($a).append($subDiv);
            } else {
                var linkHref = $origA.attr('href') || '#';
                var $a = $('<a>').addClass('nav-link menu-link').attr('href', linkHref)
                    .html('<i class="' + iconClass + '"></i> <span data-key="t-menu-' + index + '">' + titleText + '</span>');
                $li.append($a);
            }

            $newMenu.append($li);
        });

        // Remove the temp container
        $('#tempMenuContainer').remove();
    }

    // Velzon App JS (must run after menu is built)
    $.getScript(DomainName + "/Velzon_assets/js/app.js");
});
