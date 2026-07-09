/**
 * Session Timeout Manager for GreenLine
 * Handles user inactivity tracking and automatic logout across multiple tabs.
 * Optimized for Velzon / Bootstrap 5.
 */
const SessionManager = (function () {
    // Configuration
    const config = {
        sessionTimeoutMinutes: 20,        // Default session timeout (20 minutes)
        warningMinutes: 1,                // Show warning at 19 minutes (1 min remaining)
        logoutUrl: DomainName + '/Account/Logout',
        keepAliveUrl: DomainName + '/Account/KeepAlive',
        storageKey: 'gl_last_activity',   // LocalStorage key for cross-tab sync
        checkIntervalMs: 5000             // Check interval (5 seconds)
    };

    let checkInterval;
    let isWarningShowing = false;

    // Convert minutes to milliseconds
    const getTimeoutMs = () => config.sessionTimeoutMinutes * 60 * 1000;
    const getWarningThresholdMs = () => (config.sessionTimeoutMinutes - config.warningMinutes) * 60 * 1000;

    // Throttle function to prevent performance issues with rapid events
    const throttle = (func, limit) => {
        let inThrottle;
        return function() {
            const args = arguments;
            const context = this;
            if (!inThrottle) {
                func.apply(context, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        }
    };

    const updateActivityTime = () => {
        localStorage.setItem(config.storageKey, Date.now().toString());
        if (isWarningShowing) {
            hideWarning();
            // Ping server to keep server session alive when user returns from inactivity
            pingServer();
        }
    };

    const pingServer = () => {
        fetch(config.keepAliveUrl, { 
            method: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        }).catch(err => console.error('Keep-alive failed', err));
    };

    const checkInactivity = () => {
        const lastActivityStr = localStorage.getItem(config.storageKey);
        if (!lastActivityStr) return;

        const lastActivity = parseInt(lastActivityStr, 10);
        const currentTime = Date.now();
        const inactiveDuration = currentTime - lastActivity;

        if (inactiveDuration >= getTimeoutMs()) {
            executeLogout();
        } else if (inactiveDuration >= getWarningThresholdMs()) {
            showWarning(getTimeoutMs() - inactiveDuration);
        } else {
            hideWarning();
        }
    };

    const showWarning = (timeRemainingMs) => {
        if (isWarningShowing) return;
        
        isWarningShowing = true;

        if (typeof GLModal !== 'undefined') {
            GLModal.showConfirm({
                title: 'Session Expiring',
                message: '<div class="text-center"><div class="mb-3 text-warning"><i class="ri-error-warning-fill" style="font-size: 80px;"></i></div><h4 class="mt-4">Are you still there?</h4><p class="text-muted">Your session will expire in less than a minute due to inactivity.</p></div>',
                confirmText: 'Stay Logged In',
                cancelText: 'Logout Now',
                type: 'warning',
                onConfirm: function() {
                    updateActivityTime();
                },
                onCancel: function() {
                    executeLogout();
                }
            });
        } else {
            // Fallback to simple confirm if GLModal is missing
            if (confirm("Your session is about to expire. Stay logged in?")) {
                updateActivityTime();
            } else {
                executeLogout();
            }
        }
    };

    const hideWarning = () => {
        if (!isWarningShowing) return;
        isWarningShowing = false;
        if (typeof GLModal !== 'undefined' && GLModal.isOpen()) {
            GLModal.close();
        }
    };

    const executeLogout = () => {
        clearInterval(checkInterval);
        window.location.href = config.logoutUrl;
    };

    const init = () => {
        // Only run on pages where user is likely logged in
        // We check if the logout link exists in the header (common in Velzon)
        const logoutLink = document.querySelector('a[href*="LogOff"]');
        if (!logoutLink) return;

        updateActivityTime();

        const activityHandler = throttle(updateActivityTime, 2000);
        
        window.addEventListener('mousemove', activityHandler);
        window.addEventListener('keydown', activityHandler);
        window.addEventListener('scroll', activityHandler, true);
        window.addEventListener('click', activityHandler);
        window.addEventListener('touchstart', activityHandler);

        checkInterval = setInterval(checkInactivity, config.checkIntervalMs);
        
        console.log('Session Manager initialized (Timeout: ' + config.sessionTimeoutMinutes + 'm)');
    };

    return { init: init };
})();

// Start tracker
document.addEventListener('DOMContentLoaded', () => {
    SessionManager.init();
});
