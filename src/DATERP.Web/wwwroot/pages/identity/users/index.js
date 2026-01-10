(function ($) {
    // Vietnamese localization fallback
    var viLabels = {
        'Actions': 'Thao tác',
        'User': 'Người dùng',
        'Roles': 'Vai trò',
        'PhoneNumber': 'Số điện thoại',
        'Phone': 'Số điện thoại',
        'Status': 'Trạng thái',
        'CreationTime': 'Ngày tạo',
        'Created': 'Ngày tạo',
        'Edit': 'Chỉnh sửa',
        'Permissions': 'Phân quyền',
        'Delete': 'Xóa',
        'Active': 'Hoạt động',
        'Inactive': 'Ngừng hoạt động',
        'NoRoles': 'Chưa có vai trò',
        'DeleteConfirm': 'Bạn có chắc muốn xóa người dùng'
    };

    function t(key) {
        var abpL = abp.localization.getResource('AbpIdentity');
        var result = abpL(key);
        // If ABP returns the key itself (not found), use Vietnamese fallback
        if (result === key || !result) {
            return viLabels[key] || key;
        }
        return result;
    }

    var _identityUserAppService = volo.abp.identity.identityUser;

    // Helper functions
    function getInitials(name) {
        if (!name) return '?';
        var parts = name.split(/[\s@._-]+/);
        if (parts.length >= 2) {
            return (parts[0][0] + parts[1][0]).toUpperCase();
        }
        return name.substring(0, 2).toUpperCase();
    }

    function getAvatarColor(name) {
        var colors = [
            { bg: 'rgba(105, 108, 255, 0.12)', text: '#696cff' },
            { bg: 'rgba(113, 221, 55, 0.12)', text: '#71dd37' },
            { bg: 'rgba(3, 195, 236, 0.12)', text: '#03c3ec' },
            { bg: 'rgba(255, 171, 0, 0.12)', text: '#ffab00' },
            { bg: 'rgba(133, 146, 163, 0.12)', text: '#8592a3' },
        ];
        var hash = 0;
        for (var i = 0; i < (name || '').length; i++) {
            hash = name.charCodeAt(i) + ((hash << 5) - hash);
        }
        return colors[Math.abs(hash) % colors.length];
    }

    function formatDate(dateString) {
        if (!dateString) return '—';
        var date = new Date(dateString);
        return date.toLocaleDateString('vi-VN', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
        });
    }

    var togglePasswordVisibility = function () {
        $("#PasswordVisibilityButton").click(function (e) {
            var button = $(this);
            var passwordInput = button.parent().find("input");
            if (!passwordInput) return;

            if (passwordInput.attr("type") === "password") {
                passwordInput.attr("type", "text");
            } else {
                passwordInput.attr("type", "password");
            }

            var icon = button.find("i");
            if (icon) {
                icon.toggleClass("fa-eye-slash").toggleClass("fa-eye");
            }
        });
    }

    abp.modals.createUser = function () {
        return {
            initModal: function (publicApi, args) {
                togglePasswordVisibility();
            }
        };
    }

    abp.modals.editUser = function () {
        return {
            initModal: function (publicApi, args) {
                togglePasswordVisibility();
            }
        };
    }

    var _editModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'Identity/Users/EditModal',
        modalClass: "editUser"
    });
    var _createModal = new abp.ModalManager({
        viewUrl: abp.appPath + 'Identity/Users/CreateModal',
        modalClass: "createUser"
    });
    var _permissionsModal = new abp.ModalManager(
        abp.appPath + 'AbpPermissionManagement/PermissionManagementModal'
    );

    var _dataTable = null;

    // Entity Actions
    abp.ui.extensions.entityActions.get('identity.user').addContributor(
        function (actionList) {
            return actionList.addManyTail([
                {
                    text: t('Edit'),
                    icon: 'fa fa-edit',
                    visible: abp.auth.isGranted('AbpIdentity.Users.Update'),
                    action: function (data) {
                        _editModal.open({ id: data.record.id });
                    }
                },
                {
                    text: t('Permissions'),
                    icon: 'fa fa-shield-alt',
                    visible: abp.auth.isGranted('AbpIdentity.Users.ManagePermissions'),
                    action: function (data) {
                        _permissionsModal.open({
                            providerName: 'U',
                            providerKey: data.record.id,
                            providerKeyDisplayName: data.record.userName
                        });
                    }
                },
                {
                    text: t('Delete'),
                    icon: 'fa fa-trash',
                    visible: function (data) {
                        return abp.auth.isGranted('AbpIdentity.Users.Delete') && abp.currentUser.id !== data.id;
                    },
                    confirmMessage: function (data) {
                        return viLabels['DeleteConfirm'] + ' "' + data.record.userName + '"?';
                    },
                    action: function (data) {
                        _identityUserAppService
                            .delete(data.record.id)
                            .then(function () {
                                _dataTable.ajax.reloadEx();
                                abp.notify.success('Đã xóa thành công!');
                            });
                    }
                }
            ]);
        }
    );

    // Table Columns - Vietnamese Labels
    abp.ui.extensions.tableColumns.get('identity.user').addContributor(
        function (columnList) {
            columnList.addManyTail([
                {
                    title: t('User'),
                    data: 'userName',
                    render: function (data, type, row) {
                        var userName = $.fn.dataTable.render.text().display(row.userName || '');
                        var email = $.fn.dataTable.render.text().display(row.email || '');
                        var color = getAvatarColor(userName);
                        var initials = getInitials(userName);
                        var isActive = row.isActive !== false;

                        var statusDot = isActive
                            ? '<span class="user-status-dot online" title="Đang hoạt động"></span>'
                            : '<span class="user-status-dot offline" title="Ngừng hoạt động"></span>';

                        return '<div class="user-info-cell">' +
                            '<div class="user-avatar-wrapper">' +
                            '<div class="user-avatar" style="background:' + color.bg + ';color:' + color.text + '">' +
                            initials +
                            '</div>' +
                            statusDot +
                            '</div>' +
                            '<div class="user-details">' +
                            '<span class="user-name' + (!isActive ? ' inactive' : '') + '">' + userName + '</span>' +
                            '<span class="user-email">' + email + '</span>' +
                            '</div>' +
                            '</div>';
                    }
                },
                {
                    title: t('Roles'),
                    data: 'roleNames',
                    orderable: false,
                    render: function (data, type, row) {
                        var roles = row.roleNames || [];
                        if (roles.length === 0) {
                            return '<span class="no-roles">' + viLabels['NoRoles'] + '</span>';
                        }

                        var roleColors = {
                            'admin': 'role-admin',
                            'teacher': 'role-teacher',
                            'student': 'role-student',
                            'moderator': 'role-moderator'
                        };

                        var html = '<div class="roles-container">';
                        roles.forEach(function (role) {
                            var roleClass = roleColors[role.toLowerCase()] || 'role-default';
                            html += '<span class="role-badge ' + roleClass + '">' +
                                $.fn.dataTable.render.text().display(role) +
                                '</span>';
                        });
                        html += '</div>';
                        return html;
                    }
                },
                {
                    title: t('Phone'),
                    data: 'phoneNumber',
                    render: function (data, type, row) {
                        var phone = row.phoneNumber;
                        if (!phone) {
                            return '<span class="no-data">—</span>';
                        }
                        return '<span class="phone-number">' +
                            '<i class="fa fa-phone-alt phone-icon"></i>' +
                            $.fn.dataTable.render.text().display(phone) +
                            '</span>';
                    }
                },
                {
                    title: t('Status'),
                    data: 'isActive',
                    render: function (data, type, row) {
                        if (row.isActive !== false) {
                            return '<span class="status-badge status-active">' +
                                '<i class="fa fa-check-circle"></i> ' + viLabels['Active'] +
                                '</span>';
                        }
                        return '<span class="status-badge status-inactive">' +
                            '<i class="fa fa-times-circle"></i> ' + viLabels['Inactive'] +
                            '</span>';
                    }
                },
                {
                    title: t('Created'),
                    data: 'creationTime',
                    render: function (data, type, row) {
                        return '<span class="date-cell">' +
                            '<i class="fa fa-calendar-alt date-icon"></i>' +
                            formatDate(row.creationTime) +
                            '</span>';
                    }
                },
                {
                    title: t('Actions'),
                    data: null,
                    orderable: false,
                    className: 'text-end',
                    render: function (data, type, row) {
                        var html = '<div class="action-buttons">';
                        var currentUserId = abp.currentUser && abp.currentUser.id;

                        // Edit button with Tabler icon and text
                        html += '<button type="button" class="btn-action btn-action-edit" ' +
                            'data-id="' + row.id + '">' +
                            '<i class="ti ti-edit"></i> Chỉnh sửa' +
                            '</button>';

                        // Delete button (hide for current user)
                        if (currentUserId !== row.id) {
                            html += '<button type="button" class="btn-action btn-action-delete" ' +
                                'data-id="' + row.id + '" ' +
                                'data-name="' + (row.userName || '').replace(/"/g, '&quot;') + '">' +
                                '<i class="ti ti-trash"></i> Xóa' +
                                '</button>';
                        }

                        html += '</div>';
                        return html;
                    }
                }
            ]);
        },
        0
    );

    $(function () {
        var _$wrapper = $('#IdentityUsersWrapper');
        var _$table = _$wrapper.find('table');

        var dataTableConfig = abp.libs.datatables.normalizeConfiguration({
            order: [[5, 'desc']],
            processing: true,
            serverSide: true,
            scrollX: false,
            paging: true,
            pageLength: 10,
            responsive: true,
            // Custom Layout:
            // t: Table
            // r: Processing
            // Footer Row:
            //   Left: Length (l) - auto width
            //   Right: Info (i) + Pagination (p) - auto width, aligned right
            dom: 'tr<"row dataTable_footer align-items-center"<"col-auto me-auto"l><"col-auto d-flex align-items-center gap-3"ip>>',
            ajax: abp.libs.datatables.createAjax(
                _identityUserAppService.getList
            ),
            columnDefs: abp.ui.extensions.tableColumns.get('identity.user').columns.toArray(),
            drawCallback: function () {
                $('[data-toggle="tooltip"]').tooltip();
            }
        });

        // Enforce Vietnamese language settings
        $.extend(true, dataTableConfig, {
            language: {
                processing: "Đang xử lý...",
                search: "Tìm kiếm:",
                lengthMenu: "Hiển thị _MENU_ mục",
                info: "Hiển thị _START_ đến _END_ trong tổng số _TOTAL_ mục",
                infoEmpty: "Không có dữ liệu",
                infoFiltered: "(lọc từ _MAX_ mục)",
                loadingRecords: "Đang tải...",
                zeroRecords: "Không tìm thấy kết quả",
                emptyTable: "Không có dữ liệu trong bảng",
                paginate: {
                    first: '<i class="ti ti-chevrons-left"></i>',
                    previous: '<i class="ti ti-chevron-left"></i>',
                    next: '<i class="ti ti-chevron-right"></i>',
                    last: '<i class="ti ti-chevrons-right"></i>'
                },
                aria: {
                    sortAscending: ": sắp xếp tăng dần",
                    sortDescending: ": sắp xếp giảm dần"
                }
            }
        });

        _dataTable = _$table.DataTable(dataTableConfig);

        // Bind Custom Search Input
        $('#CustomSearchInput').on('keyup', function () {
            _dataTable.search(this.value).draw();
        });

        _createModal.onResult(function () {
            _dataTable.ajax.reloadEx();
        });

        _editModal.onResult(function () {
            _dataTable.ajax.reloadEx();
        });

        $('#AbpContentToolbar button[name=CreateUser]').click(function (e) {
            e.preventDefault();
            _createModal.open();
        });

        $('#CreateUserButton').click(function (e) {
            e.preventDefault();
            _createModal.open();
        });

        // Edit button click handler
        _$wrapper.on('click', '.btn-action-edit', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            _editModal.open({ id: id });
        });

        // Delete button click handler
        _$wrapper.on('click', '.btn-action-delete', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            var name = $(this).data('name');

            abp.message.confirm(
                viLabels['DeleteConfirm'] + ' "' + name + '"?',
                'Xác nhận xóa',
                function (isConfirmed) {
                    if (isConfirmed) {
                        _identityUserAppService
                            .delete(id)
                            .then(function () {
                                _dataTable.ajax.reloadEx();
                                abp.notify.success('Đã xóa thành công!');
                            });
                    }
                }
            );
        });
    });
})(jQuery);
