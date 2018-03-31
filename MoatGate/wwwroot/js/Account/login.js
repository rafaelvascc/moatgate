$(function () {
    var $carousel = $("#loginCarousel");

    $carousel.carousel({
        interval: false,
        keyboard: false
    });

    $("#lnkToResetPassword").off("click").on("click", function (e) {
        e.preventDefault();
        $carousel.carousel("next");
    });

    $("#lnkToLogin").off("click").on("click", function (e) {
        e.preventDefault();
        $carousel.carousel("prev");
    });

    if (document.location.search.indexOf("resetPasswordEmailSent=True")) {
        $.notify({
            icon: "fas fa-check",
            message: "Password reset email sent"
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
});