/**
 * admin-table.js - Shared JavaScript for Admin Table Component
 * Provides a reusable initialization function for DataTables across admin modules
 *
 * Usage:
 * initAdminTable({
 *     tableId: 'ExamSubjectsTable',
 *     ajaxUrl: '/Examination/ExamSubjects?handler=GetList',
 *     columns: [
 *         { data: 'name', title: 'Tên' },
 *         { data: 'code', title: 'Mã' }
 *     ],
 *     createModal: { path: '/Examination/ExamSubjects/CreateModal' },
 *     editModal: { path: '/Examination/ExamSubjects/EditModal' },
 *     deleteService: dATERP.examination.examSubject.delete
 * });
 */

(function () {
    'use strict';

    // Default configuration
    const defaultConfig = {
        tableId: 'DataTable',
        ajaxUrl: '',
        columns: [],
        showActions: true,
        actionColumnTitle: 'Thao tác',
        createModal: null,
        editModal: null,
        deleteService: null,
        deleteConfirmMessage: 'Bạn có chắc chắn muốn xóa mục này?',
        pageLength: 10, // Default back to 10
        order: [[1, 'asc']],
        language: {
            emptyTable: 'Không có dữ liệu',
            info: 'Hiển thị _START_ đến _END_ trong tổng số _TOTAL_ mục',
            infoEmpty: 'Không có mục nào',
            infoFiltered: '(lọc từ _MAX_ mục)',
            lengthMenu: 'Hiển thị _MENU_',
            loadingRecords: 'Đang tải...',
            processing: '', // Hide processing text
            search: 'Tìm kiếm:',
            zeroRecords: 'Không tìm thấy kết quả',
            paginate: {
                first: '<i class="ti ti-chevrons-left"></i>',
                last: '<i class="ti ti-chevrons-right"></i>',
                next: '<i class="ti ti-chevron-right"></i>',
                previous: '<i class="ti ti-chevron-left"></i>'
            }
        },
        onRowCreated: null,
        onDataLoaded: null
    };

    /**
     * Initialize an admin table with the given configuration
     * @param {Object} config - Configuration options
     * @returns {DataTable} The initialized DataTable instance
     */
    window.initAdminTable = function (config) {
        config = Object.assign({}, defaultConfig, config);

        const tableSelector = '#' + config.tableId;
        const $table = $(tableSelector);

        if ($table.length === 0) {
            console.error('Admin Table: Element not found:', tableSelector);
            return null;
        }

        // Build columns array - user columns first, then actions at end
        let columns = config.columns.slice();

        // Actions column (last)
        if (config.showActions) {
            columns.push({
                data: null,
                orderable: false,
                className: 'actions-column text-center',
                title: config.actionColumnTitle,
                render: function (data, type, row) {
                    return buildActionButtons(row, config);
                }
            });
        }

        // Initialize DataTable
        const dataTable = $table.DataTable({
            processing: true,
            serverSide: true,
            paging: true,
            searching: false, // We use custom search
            ordering: true,
            info: false, // Hide info explicitly
            autoWidth: false,
            scrollX: false,
            pageLength: config.pageLength,
            order: config.order,
            ajax: {
                url: config.ajaxUrl,
                type: 'GET',
                data: function (d) {
                    // Custom search from toolbar
                    const searchInput = $('#' + config.tableId + 'SearchInput');
                    if (searchInput.length) {
                        d.search = { value: searchInput.val() };
                    }

                    // Custom filters
                    $('.admin-table-filter').each(function () {
                        const filterKey = $(this).data('filter');
                        const filterValue = $(this).val();
                        if (filterKey && filterValue) {
                            d[filterKey] = filterValue;
                        }
                    });

                    return d;
                },
                dataSrc: function (json) {
                    if (config.onDataLoaded) {
                        config.onDataLoaded(json);
                    }
                    return json.data || [];
                },
                error: function (xhr, error, thrown) {
                    console.error('Admin Table AJAX Error:', error, thrown);
                    abp.message.error('Có lỗi xảy ra khi tải dữ liệu.');
                }
            },
            columns: columns,
            language: config.language,
            dom: 'rtlp', // Table, length, pagination (Removed 'i')
            drawCallback: function (settings) {
                // custom footer ID
                const footerId = '#' + config.tableId + 'Footer';
                const $footer = $(footerId);

                if ($footer.length) {
                    // Create wrapper divs ONLY if they don't exist
                    if ($footer.find('.footer-left').length === 0) {
                        const $left = $('<div class="footer-left d-flex align-items-center gap-3"></div>');
                        const $right = $('<div class="footer-right"></div>');
                        $footer.append($left).append($right);
                    }

                    const $left = $footer.find('.footer-left');
                    const $right = $footer.find('.footer-right');
                    const $wrapper = $table.closest('.dt-container, .dataTables_wrapper');

                    // ==========================================
                    // 1. LENGTH MENU
                    // ==========================================
                    let $length = $wrapper.find('.dataTables_length, .dt-length');
                    // Fallback selectors if not found in wrapper (might be already moved or detached)
                    if (!$length.length) $length = $left.find('.dataTables_length, .dt-length'); // Check inside footer first
                    if (!$length.length) $length = $('#' + config.tableId + '_length');

                    if (!$length.length) {
                        const $select = $wrapper.find('select[aria-controls="' + config.tableId + '"]');
                        if ($select.length) $length = $select.closest('.dt-length, .dataTables_length, div');
                    }

                    // Move ONLY if not already in the correct place
                    if ($length.length && !$length.parent().is($left)) {
                        $length.detach().appendTo($left);
                    }

                    // ==========================================
                    // 2. PAGINATION
                    // ==========================================
                    // Pagination is usually re-created on redraw, so we often need to move it again
                    let $paging = $wrapper.find('.dataTables_paginate, .dt-paging');

                    if (!$paging.length) $paging = $('#' + config.tableId + '_paginate');
                    if (!$paging.length) {
                        const $navBtn = $wrapper.find('button[aria-controls="' + config.tableId + '"], a[aria-controls="' + config.tableId + '"]');
                        if ($navBtn.length) {
                            $paging = $navBtn.closest('.dt-paging, .dataTables_paginate, nav');
                            if ($paging.parent().hasClass('dt-paging')) $paging = $paging.parent();
                        }
                    }

                    if ($paging.length && !$paging.parent().is($right)) {
                        $paging.detach().appendTo($right);
                    }
                }
            },
            createdRow: function (row, data, dataIndex) {
                if (config.onRowCreated) {
                    config.onRowCreated(row, data, dataIndex);
                }
            }
        });

        // Bind search input
        const $searchInput = $('#' + config.tableId + 'SearchInput');
        if ($searchInput.length) {
            let searchTimeout;
            $searchInput.on('input', function () {
                clearTimeout(searchTimeout);
                searchTimeout = setTimeout(function () {
                    dataTable.ajax.reload();
                }, 300);
            });
        }

        // Bind filter dropdowns
        $('.admin-table-filter').on('change', function () {
            dataTable.ajax.reload();
        });

        // Bind Create button
        if (config.createModal) {
            const createModal = new abp.ModalManager(config.createModal.path);
            $('#' + config.createButtonId || 'CreateButton').on('click', function () {
                createModal.open();
            });
            createModal.onResult(function () {
                dataTable.ajax.reload();
                abp.notify.success('Thêm mới thành công!');
            });
        }

        // Bind Edit button (delegated)
        if (config.editModal) {
            const editModal = new abp.ModalManager(config.editModal.path);
            $table.on('click', '.btn-edit', function () {
                const id = $(this).data('id');
                editModal.open({ id: id });
            });
            editModal.onResult(function () {
                dataTable.ajax.reload();
                abp.notify.success('Cập nhật thành công!');
            });
        }

        // Bind Delete button (delegated)
        if (config.deleteService) {
            $table.on('click', '.btn-delete', function () {
                const id = $(this).data('id');
                abp.message.confirm(config.deleteConfirmMessage).then(function (confirmed) {
                    if (confirmed) {
                        config.deleteService(id).then(function () {
                            dataTable.ajax.reload();
                            abp.notify.success('Xóa thành công!');
                        });
                    }
                });
            });
        }

        return dataTable;
    };

    /**
     * Build action buttons HTML for a row
     */
    function buildActionButtons(row, config) {
        let html = '<div class="d-flex justify-content-center gap-2">';

        if (config.editModal) {
            html += '<button type="button" class="btn btn-sm btn-label-primary btn-edit" data-id="' + row.id + '">';
            html += '<i class="ti ti-edit me-1"></i> Sửa';
            html += '</button>';
        }

        if (config.deleteService) {
            html += '<button type="button" class="btn btn-sm btn-label-danger btn-delete" data-id="' + row.id + '">';
            html += '<i class="ti ti-trash me-1"></i> Xóa';
            html += '</button>';
        }

        html += '</div>';
        return html;
    }

    /**
     * Refresh a specific admin table
     * @param {string} tableId - The table ID to refresh
     */
    window.refreshAdminTable = function (tableId) {
        const $table = $('#' + tableId);
        if ($table.length && $.fn.DataTable.isDataTable($table)) {
            $table.DataTable().ajax.reload();
        }
    };

})();
