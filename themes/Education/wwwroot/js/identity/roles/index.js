
// Wait for document ready
document.addEventListener('DOMContentLoaded', function () {
    console.log('!!! ROLES INDEX.JS EXECUTING via DOMContentLoaded !!!');

    if (typeof jQuery === 'undefined') {
        console.error('FATAL: jQuery is NOT defined!');
        return;
    }

    if (typeof abp === 'undefined') {
        console.error('FATAL: abp is NOT defined!');
        return;
    }

    console.log('Dependencies verified. Initializing Roles...');

    (function ($) {

        var l = abp.localization.getResource('AbpIdentity');
        var _identityRoleService = volo.abp.identity.identityRole;

        var _editModal = new abp.ModalManager(
            abp.appPath + 'Identity/Roles/EditModal'
        );
        var _createModal = new abp.ModalManager(
            abp.appPath + 'Identity/Roles/CreateModal'
        );
        var _permissionsModal = new abp.ModalManager(
            abp.appPath + 'AbpPermissionManagement/PermissionManagementModal'
        );

        var _dataTable = null;

        // Function to update statistics cards
        function updateRoleStatistics() {
            _identityRoleService.getList({ maxResultCount: 1000 }).then(function (result) {
                var roles = result.items || [];
                var totalCount = roles.length;
                var publicCount = roles.filter(r => r.isPublic).length;
                var defaultCount = roles.filter(r => r.isDefault).length;
                var staticCount = roles.filter(r => r.isStatic).length;

                $('#totalRolesCount').text(totalCount);
                $('#publicRolesCount').text(publicCount);
                $('#defaultRolesCount').text(defaultCount);
                $('#staticRolesCount').text(staticCount);
            });
        }

        // Load statistics on page load
        updateRoleStatistics();

        $(function () {
            var _$wrapper = $('#RolesTable');

            if ($.fn.DataTable.isDataTable('#RolesTable')) {
                _dataTable = $('#RolesTable').DataTable();
                return;
            }

            // --- DataTables Initialization ---
            try {
                _dataTable = _$wrapper.DataTable({
                    order: [[1, "asc"]],
                    processing: true,
                    serverSide: true,
                    scrollX: false, // Match Users page
                    paging: true,
                    searching: true, // Enable default search for now, can implement custom input later
                    ajax: abp.libs.datatables.createAjax(_identityRoleService.getList),
                    columns: [
                        {
                            title: 'Thông tin vai trò',
                            data: 'name',
                            render: function (data, type, row) {
                                // Generate color based on role name
                                var colors = ['primary', 'success', 'info', 'warning', 'danger', 'secondary'];
                                var colorIndex = data.charCodeAt(0) % colors.length;
                                var color = colors[colorIndex];

                                var badges = '';
                                if (row.isDefault) {
                                    badges += '<span class="badge bg-label-success ms-2">Mặc định</span>';
                                }
                                if (row.isPublic) {
                                    badges += '<span class="badge bg-label-info ms-1">Công khai</span>';
                                }
                                if (row.isStatic) {
                                    badges += '<span class="badge bg-label-secondary ms-1">Hệ thống</span>';
                                }

                                return `
                                    <div class="d-flex justify-content-start align-items-center">
                                        <div class="avatar-wrapper">
                                            <div class="avatar avatar-sm me-3">
                                                <span class="avatar-initial rounded bg-label-${color}">
                                                    <i class="ti ti-shield-check"></i>
                                                </span>
                                            </div>
                                        </div>
                                        <div class="d-flex flex-column">
                                            <span class="fw-semibold">${data}</span>
                                            <small class="text-muted">${badges || 'Vai trò tùy chỉnh'}</small>
                                        </div>
                                    </div>`;
                            }
                        },
                        {
                            title: 'Trạng thái',
                            data: null,
                            orderable: false,
                            render: function (data, type, row) {
                                if (row.isStatic) {
                                    return '<span class="badge bg-secondary"><i class="ti ti-lock me-1"></i>Được bảo vệ</span>';
                                }
                                return '<span class="badge bg-success"><i class="ti ti-check me-1"></i>Hoạt động</span>';
                            }
                        },
                        {
                            title: 'Thao tác',
                            data: null,
                            orderable: false,
                            className: 'text-center',
                            render: function (data, type, row) {
                                var actions = '<div class="d-flex gap-2 justify-content-center">';

                                // Edit Action
                                if (row.isStatic) {
                                    actions += '<button class="btn btn-sm btn-label-secondary edit-role-button" data-id="' + row.id + '" title="Xem chi tiết"><i class="ti ti-eye me-1"></i>Xem</button>';
                                } else {
                                    actions += '<button class="btn btn-sm btn-label-primary edit-role-button" data-id="' + row.id + '" title="Chỉnh sửa"><i class="ti ti-edit me-1"></i>Sửa</button>';
                                }

                                // Permissions Action
                                actions += '<button class="btn btn-sm btn-label-warning permissions-role-button" data-name="' + row.name + '" data-id="' + row.id + '" title="Quản lý quyền"><i class="ti ti-lock-access me-1"></i>Quyền</button>';

                                // Delete Action - only for non-static roles
                                if (!row.isStatic) {
                                    actions += '<button class="btn btn-sm btn-label-danger delete-role-button" data-id="' + row.id + '" data-name="' + row.name + '" title="Xóa vai trò"><i class="ti ti-trash me-1"></i>Xóa</button>';
                                }



                                actions += '</div>';
                                return actions;
                            }
                        }
                    ],
                    dom: '<"table-responsive"t><"dataTable_footer"<"footer-left"l><"footer-right"p>>',
                    language: {
                        processing: "Đang tải dữ liệu...",
                        lengthMenu: "Hiển thị _MENU_ bản ghi",
                        info: "Hiển thị _START_ đến _END_ của _TOTAL_ bản ghi",
                        infoEmpty: "Hiển thị 0 đến 0 của 0 bản ghi",
                        infoFiltered: "(lọc từ _MAX_ bản ghi)",
                        infoPostFix: "",
                        loadingRecords: "Đang tải...",
                        zeroRecords: "Không tìm thấy kết quả nào",
                        emptyTable: "Bảng không có dữ liệu",
                        paginate: {
                            first: '<i class="ti ti-chevrons-left"></i>',
                            previous: '<i class="ti ti-chevron-left"></i>',
                            next: '<i class="ti ti-chevron-right"></i>',
                            last: '<i class="ti ti-chevrons-right"></i>'
                        },
                        search: "Tìm kiếm:",
                    }
                });
                console.log('[DATERP DEBUG] Roles DataTable initialized successfully.');
            } catch (e) {
                console.error('[DATERP ERROR] Roles DataTable initialization failed:', e);
            }

            _createModal.onResult(function () {
                _dataTable.ajax.reload();
                updateRoleStatistics();
            });

            _editModal.onResult(function () {
                _dataTable.ajax.reload();
                updateRoleStatistics();
            });

            _permissionsModal.onResult(function () {
                _dataTable.ajax.reload();
            });

            // Handle "New Role" button click
            $('button[name="CreateRole"]').click(function (e) {
                e.preventDefault();
                _createModal.open();
            });

            // Handle custom Edit button clicks
            $(document).on('click', '.edit-role-button', function (e) {
                e.preventDefault();
                var id = $(this).data('id');
                _editModal.open({ id: id });
            });

            // Handle Permissions button clicks
            $(document).on('click', '.permissions-role-button', function (e) {
                e.preventDefault();
                var name = $(this).data('name');
                _permissionsModal.open({
                    providerName: 'R',
                    providerKey: name,
                    providerKeyDisplayName: name
                });
            });

            // Handle Delete button clicks
            $(document).on('click', '.delete-role-button', function (e) {
                e.preventDefault();
                var id = $(this).data('id');
                var name = $(this).data('name');

                abp.message.confirm(
                    l('RoleDeletionConfirmationMessage', name),
                    l('AreYouSure'),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            _identityRoleService.delete(id).then(function () {
                                _dataTable.ajax.reload();
                                abp.notify.success(l('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            });
        });
    })(jQuery);
});
