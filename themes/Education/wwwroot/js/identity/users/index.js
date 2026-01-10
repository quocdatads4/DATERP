
// Wait for document ready
document.addEventListener('DOMContentLoaded', function () {
    console.log('!!! SCRIPT.JS EXECUTING via DOMContentLoaded !!!');

    if (typeof jQuery === 'undefined') {
        console.error('FATAL: jQuery is NOT defined!');
        return;
    }

    if (typeof abp === 'undefined') {
        console.error('FATAL: abp is NOT defined!');
        return;
    }

    console.log('Dependencies verified. Initializing...');

    (function ($) {

        var l = abp.localization.getResource('AbpIdentity');
        var _identityUserService = volo.abp.identity.identityUser;

        var _editModal = new abp.ModalManager(
            abp.appPath + 'Identity/Users/EditModal'
        );
        var _createModal = new abp.ModalManager(
            abp.appPath + 'Identity/Users/CreateModal'
        );
        var _permissionsModal = new abp.ModalManager(
            abp.appPath + 'Identity/Users/PermissionsModal'
        );

        var _dataTable = null;

        abp.ui.extensions.entityActions.get('identity.user').addContributor(
            function (actionList) {
                return actionList.addManyTail(
                    [
                        {
                            text: l('Edit'),
                            visible: abp.auth.isGranted('AbpIdentity.Users.Update'),
                            action: function (data) {
                                _editModal.open({
                                    id: data.record.id,
                                });
                            },
                        },
                        {
                            text: l('Permissions'),
                            visible: abp.auth.isGranted('AbpIdentity.Users.ManagePermissions'),
                            action: function (data) {
                                _permissionsModal.open({
                                    providerName: 'U',
                                    providerKey: data.record.id,
                                });
                            },
                        },
                        {
                            text: l('Delete'),
                            visible: abp.auth.isGranted('AbpIdentity.Users.Delete'),
                            confirmMessage: function (data) {
                                return l('UserDeletionConfirmationMessage', data.record.userName);
                            },
                            action: function (data) {
                                _identityUserService
                                    .delete(data.record.id)
                                    .then(function () {
                                        _dataTable.ajax.reload();
                                    });
                            },
                        }
                    ]
                );
            }
        );

        // Columns are managed via Central ABP contributors, retrieved using columns.toArray()

        $(function () {
            var _$wrapper = $('#UsersTable');

            if ($.fn.DataTable.isDataTable('#UsersTable')) {
                _dataTable = $('#UsersTable').DataTable();
                return;
            }

            // --- Custom Search Implementation ---
            $('#SearchWrapper').html(`
                <div class="dt-search">
                    <i class="ti ti-search search-icon"></i>
                    <input type="search" class="form-control" placeholder="Tìm kiếm người dùng..." id="CustomSearchInput">
                </div>
            `);

            // --- DEBUG: Log column definitions ---
            var columnDefs = abp.ui.extensions.tableColumns.get('identity.user').columns.toArray();
            console.log('[DATERP DEBUG] ColumnDefs from ABP Extensions:', columnDefs);
            console.log('[DATERP DEBUG] ColumnDefs length:', columnDefs.length);

            // If no columns from ABP, define them manually
            if (!columnDefs || columnDefs.length === 0) {
                console.warn('[DATERP DEBUG] No columns from ABP! Using fallback columns.');
                columnDefs = [
                    { title: 'Tên đăng nhập', data: 'userName' },
                    { title: 'Email', data: 'email' },
                    { title: 'Số điện thoại', data: 'phoneNumber' },
                    { title: 'Ngày tạo', data: 'creationTime', render: function (data) { return data ? new Date(data).toLocaleDateString('vi-VN') : ''; } },
                    {
                        title: 'Thao tác',
                        data: null,
                        orderable: false,
                        defaultContent: '',
                        className: 'text-center',
                        render: function (data, type, row) {
                            return '<div class="d-flex gap-2 justify-content-center">' +
                                '<button class="btn btn-sm btn-label-primary edit-user-button" data-id="' + row.id + '"><i class="ti ti-edit me-1"></i>Sửa</button>' +
                                '<button class="btn btn-sm btn-label-danger delete-user-button" data-id="' + row.id + '" data-username="' + row.userName + '"><i class="ti ti-trash me-1"></i>Xóa</button>' +
                                '</div>';
                        }
                    }
                ];
            }

            // --- DataTables Initialization ---
            try {
                _dataTable = _$wrapper.DataTable({
                    order: [[0, "asc"]],
                    processing: true,
                    serverSide: true,
                    scrollX: false,
                    paging: true,
                    searching: true,
                    ajax: abp.libs.datatables.createAjax(_identityUserService.getList),
                    columns: columnDefs,
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
                        }
                    }
                });
                console.log('[DATERP DEBUG] DataTable initialized successfully.');
            } catch (e) {
                console.error('[DATERP ERROR] DataTable initialization failed:', e);
            }

            // Bind custom search input to DataTable
            $('#CustomSearchInput').on('keyup', function () {
                _dataTable.search(this.value).draw();
            });

            _createModal.onResult(function () {
                _dataTable.ajax.reload();
            });

            _editModal.onResult(function () {
                _dataTable.ajax.reload();
            });

            // Handle "New User" button click
            $('button[name="CreateUser"]').click(function (e) {
                e.preventDefault();
                _createModal.open();
            });

            // Handle custom Edit/Delete button clicks
            $(document).on('click', '.edit-user-button', function (e) {
                e.preventDefault();
                var id = $(this).data('id');
                _editModal.open({ id: id });
            });

            $(document).on('click', '.delete-user-button', function (e) {
                e.preventDefault();
                var id = $(this).data('id');
                var userName = $(this).data('username');

                abp.message.confirm(
                    l('UserDeletionConfirmationMessage', userName),
                    l('AreYouSure'),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            _identityUserService.delete(id).then(function () {
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
