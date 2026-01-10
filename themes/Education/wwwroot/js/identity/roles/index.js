// Wait for document ready
document.addEventListener('DOMContentLoaded', function () {
    console.log('!!! ROLES.JS EXECUTING via DOMContentLoaded !!!');

    if (typeof jQuery === 'undefined') {
        console.error('FATAL: jQuery is NOT defined!');
        return;
    }

    if (typeof abp === 'undefined') {
        console.error('FATAL: abp is NOT defined!');
        return;
    }

    (function ($) {
        var l = abp.localization.getResource('AbpIdentity');
        var _roleService = volo.abp.identity.identityRole;

        var _createModal = new abp.ModalManager(
            abp.appPath + 'Identity/Roles/CreateModal'
        );
        var _editModal = new abp.ModalManager(
            abp.appPath + 'Identity/Roles/EditModal'
        );
        var _permissionsModal = new abp.ModalManager(
            abp.appPath + 'Identity/Roles/PermissionsModal'
        );

        // Handle Create Button
        // ID: abp-identity-roles-create-button (standard) or from my Razer
        $('#abp-identity-roles-create-button').click(function (e) {
            e.preventDefault();
            _createModal.open();
        });

        _createModal.onResult(function () {
            window.location.reload(); // Reload page to see new role since we use server-side rendering
        });

        _editModal.onResult(function () {
            window.location.reload();
        });

        _permissionsModal.onResult(function () {
            window.location.reload();
        });


        // Handle Edit Button (Delegate)
        $(document).on('click', '.edit-role-btn', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            _editModal.open({
                id: id
            });
        });

        // Handle Permissions Button (Delegate)
        $(document).on('click', '.permissions-role-btn', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            _permissionsModal.open({
                providerName: 'R',
                providerKey: id // Role Name is mostly used for display, providerKey is strict
            });
        });

        // Handle Delete Button (Delegate)
        $(document).on('click', '.delete-role-btn', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            var name = $(this).data('name');

            abp.message.confirm(
                l('RoleDeletionConfirmationMessage', name),
                l('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _roleService.delete(id).then(function () {
                            window.location.reload();
                        });
                    }
                }
            );
        });

    })(jQuery);
});
