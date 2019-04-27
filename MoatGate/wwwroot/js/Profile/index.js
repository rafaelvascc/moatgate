$(function () {
    $("[data-toggle='tooltip']").tooltip();

    if (document.location.search.indexOf("PasswordChanged=True") > -1) {
        $.notify({
            icon: "fas fa-check",
            message: "Password successfully updated"
        }, {
                // settings
                type: 'success',
                delay: 5000,
                animate: {
                    enter: 'animated fadeInDown',
                    exit: 'animated fadeOutUp'
                }
            });
    }

    if (document.location.search.indexOf("profileUpdated=True") > -1) {
        $.notify({
            icon: "fas fa-check",
            message: "Profile successfully updated"
        }, {
                // settings
                type: 'success',
                delay: 5000,
                animate: {
                    enter: 'animated fadeInDown',
                    exit: 'animated fadeOutUp'
                }
            });
    }

    if (document.location.search.indexOf("confirmationEmailSent=True") > -1) {
        $.notify({
            icon: "fas fa-check",
            message: "Email change confirmation email successfully sent"
        }, {
                // settings
                type: 'success',
                delay: 5000,
                animate: {
                    enter: 'animated fadeInDown',
                    exit: 'animated fadeOutUp'
                }
            });
    }

    if (document.location.search.indexOf("confirmationSmsSent=True") > -1) {
        $.notify({
            icon: "fas fa-check",
            message: "Phone number change token SMS successfully sent"
        }, {
                // settings
                type: 'success',
                delay: 5000,
                animate: {
                    enter: 'animated fadeInDown',
                    exit: 'animated fadeOutUp'
                }
            });
    }

    $("#btnVerityEmail").off("click").on("click", function (event) {
        $.ajax({
            type: "GET",
            url: "../internal-api/profile/sendemailconfirm",
            success: function (response) {
                $.notify({
                    icon: "fas fa-check",
                    message: "Confirmation email sent",
                }, {
                        // settings
                        type: 'success',
                        delay: 5000,
                        animate: {
                            enter: 'animated fadeInDown',
                            exit: 'animated fadeOutUp'
                        }
                    });
            },
            error: function (response) {
                $.notify({
                    icon: "fas fa-times",
                    message: "Failed send confirmation email",
                }, {
                        // settings
                        type: 'danger',
                        delay: 5000,
                        animate: {
                            enter: 'animated fadeInDown',
                            exit: 'animated fadeOutUp'
                        }
                    });
            },
            complete: function (response) {
            }
        });
    });

    $("#btnVerityPhone").off("click").on("click", function (event) {
        $.ajax({
            type: "GET",
            url: "../internal-api/profile/sendphoneconfirm",
            success: function (response) {
                $.notify({
                    icon: "fas fa-check",
                    message: "Confirmation SMS sent",
                }, {
                        // settings
                        type: 'success',
                        delay: 5000,
                        animate: {
                            enter: 'animated fadeInDown',
                            exit: 'animated fadeOutUp'
                        }
                    });
            },
            error: function (response) {
                $.notify({
                    icon: "fas fa-times",
                    message: "Failed send confirmation SMS",
                }, {
                        // settings
                        type: 'danger',
                        delay: 5000,
                        animate: {
                            enter: 'animated fadeInDown',
                            exit: 'animated fadeOutUp'
                        }
                    });
            },
            complete: function (response) {
            }
        });
    });

    $("#btnConfirmPhoneVerification").off("click").on("click", function (event) {
        var token = $('#txtPhoneVerificationCode').val();
        $.ajax({
            type: "POST",
            url: `../internal-api/profile/confirmphone/${token}`,
            success: function (response) {
                $.notify({
                    icon: "fas fa-check",
                    message: "Phone number confirmed",
                }, {
                        // settings
                        type: 'success',
                        delay: 5000,
                        animate: {
                            enter: 'animated fadeInDown',
                            exit: 'animated fadeOutUp'
                        }
                    });

                $(".phoneVerificationControl").remove();
                $("#iconWarningPhoneConfirm").replaceWith($('<i class="fas fa-check pull-right text-success" data-toggle="tooltip" data-placement="bottom" data-html="true" title="Phone number confirmed"></i>'));
                $("[data-toggle='tooltip']").tooltip();
            },
            error: function (response) {
                $.notify({
                    icon: "fas fa-times",
                    message: "Failed to confirm phone number",
                }, {
                        // settings
                        type: 'danger',
                        delay: 5000,
                        animate: {
                            enter: 'animated fadeInDown',
                            exit: 'animated fadeOutUp'
                        }
                    });
            },
            complete: function (response) {
            }
        });
    });
});