  /**
 * GreenLine DataTable Helper (glDataTable.js)
 * -------------------------------------------
 * Common wrapper around DataTables 2.x with Bootstrap5 defaults.
 *
 * Usage on any page:
 *   GlDataTable.init('myTableId', {
 *       data: result,
 *       columns: [...],
 *       columnDefs: [...]
 *       // any DataTables option can be overridden here
 *   });
 *
 * Or with AJAX:
 *   GlDataTable.init('myTableId', {
 *       ajax: { url: '/Controller/Action', type: 'POST' },
 *       columns: [...]
 *   });
 *
 * Status Filter (Active / Inactive):
 *   Add `statusFilter: columnIndex` or `statusFilter: [colIndex1, colIndex2]`
 *   to enable the All/Active/Inactive filter.
 */

/* ================================================================
   Global Status-Filter Extension (registered once per page load)
   Works by checking whether the rendered status-column cell HTML
   contains a checked checkbox (toggle switch) or not.
   ================================================================ */
(function () {
    if (window._glStatusFilterExtRegistered) return;
    window._glStatusFilterExtRegistered = true;
    window._glStatusFilters = {};   // { tableId: { colIdx: 'all'|'active'|'inactive', ... } }

    $.fn.dataTable.ext.search.push(function (settings, searchData, index) {
        var tblId = $(settings.nTable).attr('id');
        var filters = window._glStatusFilters[tblId];
        if (!filters) return true;

        for (var colIdx in filters) {
            if (!filters.hasOwnProperty(colIdx)) continue;
            var filterVal = filters[colIdx];
            if (filterVal === 'all') continue;

            // 1. Get the cell HTML if cells are rendered
            var cellHtml = '';
            if (settings.aoData && settings.aoData[index] && settings.aoData[index].anCells) {
                var cellNode = settings.aoData[index].anCells[colIdx];
                if (cellNode) {
                    cellHtml = cellNode.innerHTML || '';
                }
            }
            
            // 2. Also fallback to searchData plain text
            var searchText = (searchData[colIdx] || '').trim();
            
            // 3. Also fallback to raw row data value
            var rawVal = '';
            if (settings.aoData && settings.aoData[index] && settings.aoData[index]._aData) {
                var rowObj = settings.aoData[index]._aData;
                var mData = settings.aoColumns[colIdx].mData;
                if (typeof mData === 'function') {
                    rawVal = mData(rowObj, 'filter');
                } else if (mData !== undefined && mData !== null) {
                    rawVal = rowObj[mData];
                }
            }

            // Determine if it is Active
            var isActive = false;
            if (cellHtml.indexOf('type="checkbox"') > -1 || cellHtml.indexOf('class="slider') > -1) {
                isActive = cellHtml.indexOf('checked') > -1;
            } else {
                var valToCheck = String(rawVal !== undefined && rawVal !== null ? rawVal : searchText).trim().toUpperCase();
                isActive = (valToCheck === 'Y' || valToCheck === '1' || valToCheck === 'ACTIVE' || valToCheck === 'YES' || valToCheck === 'TRUE');
            }

            var matches = (filterVal === 'active') ? isActive : !isActive;
            if (!matches) return false;
        }

        return true;
    });

    // Register statusSearch content plugin in ColumnControl
    if ($.fn.dataTable.ColumnControl && !$.fn.dataTable.ColumnControl.content.statusSearch) {
        $.fn.dataTable.ColumnControl.content.statusSearch = {
            defaults: {},
            init: function (config) {
                var tableApi = this.dt();
                var colIdx = this.idx();
                var tableId = $(tableApi.table().container()).find('table').attr('id') || $(tableApi.table().node()).attr('id');

                // Initialize or retrieve the status filter state for this column
                if (!window._glStatusFilters[tableId]) {
                    window._glStatusFilters[tableId] = {};
                }
                if (window._glStatusFilters[tableId][colIdx] === undefined) {
                    window._glStatusFilters[tableId][colIdx] = 'all';
                }
                var currentVal = window._glStatusFilters[tableId][colIdx];

                // Create wrapper container
                var container = document.createElement('div');
                container.className = 'dtcc-content dtcc-search dtcc-status-search';

                var wrapper = document.createElement('div');

                // Left icon (toggle switch SVG)
                var iconDiv = document.createElement('div');
                iconDiv.className = 'dtcc-search-type-icon';
                iconDiv.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-toggle-left"><rect x="1" y="5" width="22" height="14" rx="7" ry="7"></rect><circle cx="8" cy="12" r="3"></circle></svg>';

                // Select dropdown
                var select = document.createElement('select');
                select.className = 'form-select form-select-sm status-select';

                // Options
                var optAll = new Option('All', 'all');
                var optActive = new Option('Active', 'active');
                var optInactive = new Option('Inactive', 'inactive');
                select.add(optAll);
                select.add(optActive);
                select.add(optInactive);

                // Set current selected value
                select.value = currentVal;

                // Prevent closing the ColumnControl popup or triggering sort when interacting with dropdown
                $(select).on('click mousedown keydown', function (e) {
                    e.stopPropagation();
                });

                // Bind change event to update filter and redraw
                $(select).on('change', function () {
                    var val = $(this).val();
                    window._glStatusFilters[tableId][colIdx] = val;
                    
                    if (config._parents) {
                        config._parents.forEach(function (parent) {
                            parent.activeList(colIdx + '_status', val !== 'all');
                        });
                    }
                    tableApi.draw();
                });

                // Set initial highlight state if active
                if (config._parents && currentVal !== 'all') {
                    config._parents.forEach(function (parent) {
                        parent.activeList(colIdx + '_status', true);
                    });
                }

                wrapper.appendChild(iconDiv);
                wrapper.appendChild(select);
                container.appendChild(wrapper);

                return container;
            }
        };
    }
}());

var GlDataTable = (function () {

    var _defaults = {
        responsive: true,
        columnControl: [['search', 'orderAsc', 'orderDesc', 'orderClear']],
        pageLength: 10,
        lengthMenu: [
            [10, 25, 50, 100, -1],
            [10, 25, 50, 100, 'All']
        ],
        language: {
            paginate: {
                first:    '&laquo;',
                last:     '&raquo;',
                previous: '&lsaquo;',
                next:     '&rsaquo;'
            },
            lengthMenu:  '_MENU_ records per page',
            info:        'Showing _START_ to _END_ of _TOTAL_ entries',
            infoEmpty:   'No entries to show',
            search:      'Search:',
            zeroRecords: 'No matching records found',
            emptyTable:  'No data available'
        },
        dom:
            '<"row align-items-center py-2 px-3 mb-1"' +
                '<"col-sm-6 d-flex align-items-center gap-2"l>' +
                '<"col-sm-6 d-flex justify-content-end"f>' +
            '>' +
            '<"table-responsive"t>' +
            '<"row align-items-center py-2 px-3 mt-1"' +
                '<"col-sm-6"i>' +
                '<"col-sm-6 d-flex justify-content-end"p>' +
            '>',
        drawCallback: function () {
            // Re-apply Select2 to length menu select on every redraw
            var wrapper = this.api().table().container();
            $(wrapper).find('.dataTables_length select').select2({
                minimumResultsForSearch: Infinity,
                width: 'auto'
            });
        }
    };

    /**
     * Initialize a DataTable.
     * @param {string} tableId  - The HTML table element id (without #)
     * @param {object} options  - DataTables options (merged with defaults)
     * @returns {DataTables.Api} DataTables instance
     */
    function init(tableId, options) {
        // --- Extract custom options before passing to DataTables ---
        var statusFilterCols = [];
        if (options.statusFilter !== undefined) {
            if (Array.isArray(options.statusFilter)) {
                statusFilterCols = options.statusFilter.map(function(c) { return parseInt(c, 10); });
            } else {
                statusFilterCols = [parseInt(options.statusFilter, 10)];
            }
        }
        delete options.statusFilter;  // DataTables doesn't know this option

        statusFilterCols.forEach(function (colIdx) {
            // 1. If options.columns exists, override columnControl directly
            if (options.columns && options.columns[colIdx]) {
                options.columns[colIdx].columnControl = [['statusSearch', 'orderAsc', 'orderDesc', 'orderClear']];
            }
            
            // 2. Also put it in columnDefs as a fallback
            if (!options.columnDefs) {
                options.columnDefs = [];
            }
            options.columnDefs.forEach(function (def) {
                if (def.targets && (def.targets === colIdx || (Array.isArray(def.targets) && def.targets.indexOf(colIdx) > -1))) {
                    if (def.columnControl !== undefined) {
                        delete def.columnControl;
                    }
                }
            });
            options.columnDefs.push({
                targets: [colIdx],
                columnControl: [['statusSearch', 'orderAsc', 'orderDesc', 'orderClear']]
            });
        });

        // Force all columns to treat data as string by default
        if (options.columns) {
            options.columns.forEach(function (col) {
                if (col.type === undefined) {
                    col.type = 'string';
                }
            });
        }

        // Also apply globally via columnDefs as a fallback
        var localDefaults = $.extend(true, {}, _defaults);
        if (!localDefaults.columnDefs) localDefaults.columnDefs = [];
        localDefaults.columnDefs.unshift({ type: 'string', targets: '_all' });

        var config = $.extend(true, {}, localDefaults, options);
        var table  = $('#' + tableId).DataTable(config);

        // --- Handle Status Filter initialization if requested ---
        statusFilterCols.forEach(function (colIdx) {
            if (!window._glStatusFilters[tableId]) {
                window._glStatusFilters[tableId] = {};
            }
            if (window._glStatusFilters[tableId][colIdx] === undefined) {
                window._glStatusFilters[tableId][colIdx] = 'all';
            }
            
            // Remove 'no-cc' class from the status column header so the ColumnControl icon is visible!
            var headerNode = table.column(colIdx).header();
            if (headerNode) {
                $(headerNode).removeClass('no-cc');
            }
        });

        return table;
    }

    /**
     * Destroy and re-initialize a DataTable.
     * Useful when reloading data after a save.
     * @param {string} tableId
     * @param {object} options
     * @returns {DataTables.Api}
     */
    function refresh(tableId, options) {
        if ($.fn.DataTable.isDataTable('#' + tableId)) {
            $('#' + tableId).DataTable().destroy();
        }
        return init(tableId, options);
    }

    /**
     * Get an existing DataTable instance.
     * @param {string} tableId
     * @returns {DataTables.Api|null}
     */
    function get(tableId) {
        if ($.fn.DataTable.isDataTable('#' + tableId)) {
            return $('#' + tableId).DataTable();
        }
        return null;
    }

    return {
        init:    init,
        refresh: refresh,
        get:     get
    };

})();
