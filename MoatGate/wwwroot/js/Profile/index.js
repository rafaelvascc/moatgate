$(function () {
    if (document.location.search.indexOf("?PasswordChanged=True") > -1) {
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
});