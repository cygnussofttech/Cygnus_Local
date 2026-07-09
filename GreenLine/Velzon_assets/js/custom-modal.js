/* ═══════════════════════════════════════════════════════════════════════
   GreenLine Modal System — gl-modal.js
   Dynamic, reusable modal API for ASP.NET MVC / Web Forms.
   Dependencies: None (vanilla JS). jQuery optional — auto-detected.
   ═══════════════════════════════════════════════════════════════════════ */

var GLModal = (function () {
    'use strict';

    // ── Icon Map (Remix Icon classes used in Velzon) ──
    var ICON_MAP = {
        success: { icon: 'ri-checkbox-circle-line', css: 'success' },
        error:   { icon: 'ri-close-circle-line',    css: 'error' },
        warning: { icon: 'ri-alert-line',            css: 'warning' },
        info:    { icon: 'ri-information-line',       css: 'info' },
        confirm: { icon: 'ri-question-line',          css: 'confirm' },
        delete:  { icon: 'ri-delete-bin-line',        css: 'error' },
        save:    { icon: 'ri-save-line',              css: 'success' },
        submit:  { icon: 'ri-send-plane-line',        css: 'info' }
    };

    // ── Color map for dynamic button coloring ──
    var COLOR_MAP = {
        success: 'var(--gl-color-success)',
        error:   'var(--gl-color-error)',
        warning: 'var(--gl-color-warning)',
        info:    'var(--gl-color-info)',
        confirm: 'var(--gl-color-confirm)',
        delete:  'var(--gl-color-error)',
        save:    'var(--gl-color-success)',
        submit:  'var(--gl-color-info)'
    };

    var _overlay = null;
    var _activeCallback = null;
    var _cancelCallback = null;
    var _isOpen = false;

    // ── Initialize DOM (called once on first use) ──
    function _ensureDOM() {
        if (_overlay) return;

        _overlay = document.getElementById('glModalOverlay');
        
        // If not in DOM, create it (fallback)
        if (!_overlay) {
            var html = _buildHTML();
            var wrapper = document.createElement('div');
            wrapper.innerHTML = html;
            document.body.appendChild(wrapper.firstElementChild);
            _overlay = document.getElementById('glModalOverlay');
        }

        // Always bind events if it's the first time we've identified the overlay
        _bindEvents();
    }

    function _buildHTML() {
        return [
            '<div class="gl-modal-overlay" id="glModalOverlay">',
            '  <div class="gl-modal" role="dialog" aria-modal="true" aria-labelledby="glModalTitle" id="glModalDialog">',

            // Header (Universal for all modals)
            '    <div class="gl-modal__header" id="glModalHeader">',
            '      <div class="gl-modal__title-group">',
            '        <div class="gl-modal__header-icon" id="glModalHeaderIcon"><i id="glModalHeaderIconI"></i></div>',
            '        <h5 class="gl-modal__title" id="glModalTitle"></h5>',
            '      </div>',
            '      <button type="button" class="gl-modal__close" id="glModalCloseBtn" aria-label="Close">&times;</button>',
            '    </div>',


            // Body
            '    <div class="gl-modal__body">',
            '      <div id="glModalStatusIconContainer" style="display:none;">',
            '        <div class="gl-modal__status-wrapper" id="glModalStatusWrapper">',
            '          <div class="gl-modal__status-icon" id="glModalStatusIcon"><i id="glModalStatusIconI"></i></div>',
            '        </div>',
            '      </div>',
            '      <h4 class="gl-modal__body-title" id="glModalBodyTitle" style="display:none;"></h4>',
            '      <p class="gl-modal__body-message" id="glModalBodyMsg"></p>',
            '      <p class="gl-modal__body-sub" id="glModalBodySub" style="display:none;"></p>',
            '    </div>',

            // Footer
            '    <div class="gl-modal__footer" id="glModalFooter">',
            '      <button type="button" class="gl-modal__btn gl-modal__btn--cancel" id="glModalCancelBtn">Cancel</button>',
            '      <button type="button" class="gl-modal__btn gl-modal__btn--confirm" id="glModalConfirmBtn">Confirm</button>',
            '    </div>',

            '  </div>',
            '</div>'
        ].join('\n');
    }

    // ── Bind Close / Confirm Events ──
    function _bindEvents() {
        // Close button
        var closeBtn = document.getElementById('glModalCloseBtn');
        if (closeBtn) {
            closeBtn.addEventListener('click', function () { _close('cancel'); });
        }

        // Cancel button
        var cancelBtn = document.getElementById('glModalCancelBtn');
        if (cancelBtn) {
            cancelBtn.addEventListener('click', function () { _close('cancel'); });
        }

        // Confirm button
        var confirmBtn = document.getElementById('glModalConfirmBtn');
        if (confirmBtn) {
            confirmBtn.addEventListener('click', function () { _close('confirm'); });
        }

        // Overlay click to close
        _overlay.addEventListener('click', function (e) {
            if (e.target === _overlay) {
                _close('cancel');
            }
        });

        // Escape key
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape' && _isOpen) {
                _close('cancel');
            }
        });
    }

    // ── Open Modal ──
    function _open() {
        _overlay.classList.remove('gl-modal--closing');
        // Force reflow for animation restart
        void _overlay.offsetWidth;
        _overlay.classList.add('gl-modal--active');
        _isOpen = true;
        document.body.style.overflow = 'hidden';

        // Focus the confirm or OK button
        var confirmBtn = document.getElementById('glModalConfirmBtn');
        if (confirmBtn && confirmBtn.style.display !== 'none') {
            setTimeout(function () { confirmBtn.focus(); }, 350);
        }
    }

    // ── Close Modal ──
    function _close(action) {
        if (!_isOpen) return;

        _overlay.classList.add('gl-modal--closing');
        _overlay.classList.remove('gl-modal--active');
        _isOpen = false;
        document.body.style.overflow = '';

        // Callback after animation completes
        setTimeout(function () {
            _overlay.classList.remove('gl-modal--closing');

            if (action === 'confirm' && typeof _activeCallback === 'function') {
                _activeCallback();
            }
            if (action === 'cancel' && typeof _cancelCallback === 'function') {
                _cancelCallback();
            }

            _activeCallback = null;
            _cancelCallback = null;
        }, 280);
    }

    // ── Helper: Get icon data ──
    function _resolveIcon(type) {
        return ICON_MAP[type] || ICON_MAP['info'];
    }

    // ── Helper: Reset all sections ──
    function _reset() {
        var header    = document.getElementById('glModalHeader');
        var bodyTitle = document.getElementById('glModalBodyTitle');
        var bodySub   = document.getElementById('glModalBodySub');
        var cancelBtn = document.getElementById('glModalCancelBtn');
        var confirmBtn= document.getElementById('glModalConfirmBtn');
        var footer    = document.getElementById('glModalFooter');
        var statusIconContainer = document.getElementById('glModalStatusIconContainer');

        header.style.display    = ''; // Always show header now
        bodyTitle.style.display = 'none';
        bodySub.style.display   = 'none';
        statusIconContainer.style.display = 'none';
        cancelBtn.style.display = '';
        confirmBtn.style.display= '';
        footer.style.display    = '';

        // Reset footer alignment
        footer.classList.remove('gl-modal__footer--spread');

        // Reset confirm button color
        confirmBtn.className = 'gl-modal__btn gl-modal__btn--confirm';
        confirmBtn.style.background = '';
        confirmBtn.style.borderColor = '';
    }

    /* ═══════════════════════════════════════════════════════════
       PUBLIC API
    ═══════════════════════════════════════════════════════════ */

    /**
     * showConfirm — Confirmation modal with header bar (like the screenshot)
     *
     * @param {Object} options
     *   title        {string}   Header title (e.g. "Inactivate Customer")
     *   message      {string}   Body message (HTML supported)
     *   subtitle     {string}   Optional gray subtitle below message
     *   type         {string}   'confirm'|'delete'|'save'|'submit'|'warning'|'info'|'error'|'success'
     *   confirmText  {string}   Confirm button text (default: "Yes, Confirm")
     *   cancelText   {string}   Cancel button text (default: "Cancel")
     *   onConfirm    {function} Callback when confirmed
     *   onCancel     {function} Callback when cancelled
     */
    function showConfirm(options) {
        _ensureDOM();
        _reset();

        // Normalize: allow simple call  showConfirm("Title", "Message", callback)
        if (typeof options === 'string') {
            options = {
                title:     arguments[0],
                message:   arguments[1] || '',
                onConfirm: arguments[2] || null,
                type:      'confirm'
            };
        }

        var type = options.type || 'confirm';
        var iconData = _resolveIcon(type);

        // ── Header
        document.getElementById('glModalTitle').textContent = options.title || 'Confirm';

        // Header icon
        var headerIcon = document.getElementById('glModalHeaderIcon');
        headerIcon.className = 'gl-modal__header-icon gl-modal__header-icon--' + iconData.css;
        document.getElementById('glModalHeaderIconI').className = iconData.icon;


        // ── Body
        document.getElementById('glModalBodyMsg').innerHTML = options.message || '';

        if (options.subtitle) {
            var bodySub = document.getElementById('glModalBodySub');
            bodySub.style.display = '';
            bodySub.textContent = options.subtitle;
        }

        // ── Footer
        var footer = document.getElementById('glModalFooter');
        footer.classList.add('gl-modal__footer--spread');

        var cancelBtn = document.getElementById('glModalCancelBtn');
        var confirmBtn = document.getElementById('glModalConfirmBtn');

        cancelBtn.innerHTML = '<i class="ri-close-line align-middle"></i> ' + (options.cancelText || 'Cancel');
        confirmBtn.innerHTML = '<i class="' + iconData.icon + ' align-middle"></i> ' + (options.confirmText || 'Yes, Confirm');

        // Color confirm button based on type
        var color = COLOR_MAP[type] || COLOR_MAP['confirm'];
        confirmBtn.style.background = color;
        confirmBtn.style.borderColor = color;

        // ── Callbacks
        _activeCallback = options.onConfirm || null;
        _cancelCallback = options.onCancel || null;

        _open();
    }

    /**
     * showPopup — Image/Icon-based popup (Success / Error / Warning / Info)
     *
     * @param {Object} options
     *   title        {string}   Bold body title (e.g. "Success!")
     *   message      {string}   Main message text (HTML supported)
     *   subtitle     {string}   Optional gray sub-text
     *   type         {string}   'success'|'error'|'warning'|'info'
     *   image        {string}   Optional image URL (overrides icon)
     *   buttonText   {string}   Button text (default: "OK")
     *   onClose      {function} Callback when closed
     */
    function showPopup(options) {
        _ensureDOM();
        _reset();

        // Normalize: allow simple call  showPopup("Title", "Message", "success")
        if (typeof options === 'string') {
            options = {
                title:   arguments[0],
                message: arguments[1] || '',
                type:    arguments[2] || 'info',
                onClose: arguments[3] || null
            };
        }

        var type = options.type || 'info';
        var iconData = _resolveIcon(type);

        // ── Header (Title & Icon)
        document.getElementById('glModalTitle').textContent = options.title || 'Notification';
        
        var headerIcon = document.getElementById('glModalHeaderIcon');
        headerIcon.className = 'gl-modal__header-icon gl-modal__header-icon--' + iconData.css;
        document.getElementById('glModalHeaderIconI').className = iconData.icon;

        // ── Body
        // Show prominent status icon in body for popups
        var statusIconContainer = document.getElementById('glModalStatusIconContainer');
        var statusWrapper = document.getElementById('glModalStatusWrapper');
        var statusIconI = document.getElementById('glModalStatusIconI');
        
        statusIconContainer.style.display = 'block';
        statusWrapper.className = 'gl-modal__status-wrapper gl-modal__status--' + iconData.css;
        statusIconI.className = iconData.icon.replace('-line', '-fill'); // Use fill version for big icon

        document.getElementById('glModalBodyTitle').style.display = 'none';
        document.getElementById('glModalBodyMsg').innerHTML = options.message || '';


        if (options.subtitle) {
            var bodySub = document.getElementById('glModalBodySub');
            bodySub.style.display = '';
            bodySub.textContent = options.subtitle;
        }

        // ── Footer — Single OK button
        var cancelBtn  = document.getElementById('glModalCancelBtn');
        var confirmBtn = document.getElementById('glModalConfirmBtn');

        cancelBtn.style.display = 'none';
        confirmBtn.innerHTML = '<i class="' + iconData.icon + ' align-middle"></i> ' + (options.buttonText || 'OK');
        confirmBtn.classList.add('gl-modal__btn--ok', 'gl-modal__btn--' + iconData.css);

        var color = COLOR_MAP[type] || COLOR_MAP['info'];
        confirmBtn.style.background = color;
        confirmBtn.style.borderColor = color;

        // ── Callbacks
        _activeCallback = options.onClose || null;
        _cancelCallback = options.onClose || null;

        _open();
    }

    /**
     * showSuccess — Shorthand for success popup
     */
    function showSuccess(message, subtitle, onClose) {
        showPopup({
            title: 'Success!',
            message: message,
            subtitle: subtitle || '',
            type: 'success',
            onClose: onClose
        });
    }

    /**
     * showError — Shorthand for error popup
     */
    function showError(message, subtitle, onClose) {
        showPopup({
            title: 'Error!',
            message: message,
            subtitle: subtitle || '',
            type: 'error',
            onClose: onClose
        });
    }

    /**
     * showWarning — Shorthand for warning popup
     */
    function showWarning(message, subtitle, onClose) {
        showPopup({
            title: 'Warning!',
            message: message,
            subtitle: subtitle || '',
            type: 'warning',
            onClose: onClose
        });
    }

    /**
     * showInfo — Shorthand for info popup
     */
    function showInfo(message, subtitle, onClose) {
        showPopup({
            title: 'Information',
            message: message,
            subtitle: subtitle || '',
            type: 'info',
            onClose: onClose
        });
    }

    /**
     * showForm — Opens the dynamic form modal (_FormModal.cshtml)
     * 
     * @param {Object} options
     *   title    {string}  Modal header title
     *   url      {string}  URL to fetch form content from (AJAX GET)
     *   onLoaded {function} Callback after content is loaded
     *   onSave   {function} Callback when Save button is clicked. Should return a promise.
     */
    function showForm(options) {
        var modalEl = document.getElementById('glFormModal');
        if (!modalEl) {
            console.error('GLModal: _FormModal.cshtml not found in DOM.');
            return;
        }

        var bsModal = bootstrap.Modal.getOrCreateInstance(modalEl);
        var titleEl = document.getElementById('glFormModalLabel');
        var iconEl = document.getElementById('glFormModalIcon');
        var loader = document.getElementById('glFormLoader');
        var content = document.getElementById('glFormContent');
        var saveBtn = document.getElementById('glFormSaveBtn');

        // Reset
        titleEl.textContent = options.title || 'Form';
        if (iconEl && options.icon) {
            iconEl.className = options.icon;
        } else if (iconEl) {
            iconEl.className = 'ri-edit-2-line'; // Default
        }
        if (loader) {
            loader.classList.remove('hidden');
            loader.style.display = 'block';
        }
        content.innerHTML = '';
        saveBtn.disabled = true;

        bsModal.show();

        // Load Content
        $.ajax({
            url: options.url,
            type: 'GET',
            success: function (data) {
                if (loader) loader.style.display = 'none';
                 $(content).html(data);
               // content.innerHTML = data;
                saveBtn.disabled = false;

                if (typeof options.onLoaded === 'function') {
                    options.onLoaded();
                }
            },
            error: function () {
                if (loader) loader.style.display = 'none';
                $(content).html('<div class="alert alert-danger m-3">Failed to load form content. Please try again.</div>');
              //  content.innerHTML = '<div class="alert alert-danger m-3">Failed to load form content. Please try again.</div>';
            }
        });

        // Bind Save
        saveBtn.onclick = function () {
            if (typeof options.onSave === 'function') {
                var result = options.onSave();
                
                // If it's a promise (like $.ajax), handle loading state
                if (result && typeof result.then === 'function') {
                    var originalHtml = saveBtn.innerHTML;
                    saveBtn.disabled = true;
                    saveBtn.innerHTML = '<i class="ri-loader-4-line spin align-middle"></i> Saving...';
                    
                    result.done(function (response) {
                        // If response is exactly false, don't close (validation failed)
                        if (response !== false) {
                            bsModal.hide();
                        }
                    }).always(function () {
                        saveBtn.disabled = false;
                        saveBtn.innerHTML = originalHtml;
                    });
                }
            }
        };
    }

    /**
     * close — Programmatically close the modal
     */
    function close() {
        _close('cancel');
    }

    /**
     * isOpen — Check if modal is currently open
     */
    function isOpen() {
        return _isOpen;
    }

    // ── Public Interface ──
    return {
        showConfirm: showConfirm,
        showPopup:   showPopup,
        showForm:    showForm,
        showSuccess: showSuccess,
        showError:   showError,
        showWarning: showWarning,
        showInfo:    showInfo,
        close:       close,
        isOpen:      isOpen
    };

})();

/* ═══════════════════════════════════════════════════════════════════════
   GLOBAL CONVENIENCE FUNCTIONS
   These allow calling without the GLModal. prefix for quick usage.
   ═══════════════════════════════════════════════════════════════════════ */

/**
 * showConfirm("Delete Record?", "Are you sure?", callback)
 * showConfirm({ title, message, type, onConfirm, ... })
 */
function showConfirm(titleOrOptions, message, onConfirm) {
    if (typeof titleOrOptions === 'object') {
        GLModal.showConfirm(titleOrOptions);
    } else {
        GLModal.showConfirm({
            title:     titleOrOptions,
            message:   message || '',
            onConfirm: onConfirm || null,
            type:      'confirm'
        });
    }
}

/**
 * showPopup("Success", "Data saved successfully", "success")
 * showPopup({ title, message, type, image, onClose, ... })
 */
function showPopup(titleOrOptions, message, typeOrImage, onClose) {
    if (typeof titleOrOptions === 'object') {
        GLModal.showPopup(titleOrOptions);
    } else {
        // Detect if third arg is an image URL or a type keyword
        var type = 'info';
        var image = null;
        if (typeOrImage) {
            if (['success', 'error', 'warning', 'info'].indexOf(typeOrImage) > -1) {
                type = typeOrImage;
            } else {
                image = typeOrImage;
            }
        }
        GLModal.showPopup({
            title:   titleOrOptions,
            message: message || '',
            type:    type,
            image:   image,
            onClose: onClose || null
        });
    }
}
