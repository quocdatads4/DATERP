$(function () {
    // Password visibility toggle (ABP standard pattern)
    $("#PasswordVisibilityButton").click(function (e) {
        var button = $(this);
        var passwordInput = button.parent().find("input");
        if (!passwordInput.length) {
            return;
        }

        if (passwordInput.attr("type") === "password") {
            passwordInput.attr("type", "text");
        } else {
            passwordInput.attr("type", "password");
        }

        var icon = button.find("i");
        if (icon.length) {
            // Support both Font Awesome and Tabler icons
            icon.toggleClass("fa-eye-slash fa-eye ti-eye-off ti-eye");
        }
    });
});
