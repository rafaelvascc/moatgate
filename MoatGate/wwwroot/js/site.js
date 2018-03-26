$(function () {
    appyConfirmPopoverToDeleteButtons();
});

function appyConfirmPopoverToDeleteButtons() {
    var options = {
        placement: "bottom",
        selector: ".deletePopOver",
        title: "Confirm Delete?",
        trigger: "click",
        html: true,
        content: function (event) {
            var $content = $(this).parents(".btn-group:first").siblings(".confirmDeletePopover").find("> div.btn-group").clone();
            $(this).off("shown.bs.popover").on("shown.bs.popover", function (event) {
                var $that = $(this);
                $content.find(".btnCancelDelete").on("click", function () {
                    $that.popover("hide");
                });
            });
            return $content;
        }
    }

    $("body").popover(options);
}
