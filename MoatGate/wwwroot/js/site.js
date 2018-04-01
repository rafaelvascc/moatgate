$(function () {
    appyConfirmPopoverToDeleteButtons();
    applyTabs();

    $(".noEnterForm").on("keydown keypress keyup", function (e) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });
});

function appyConfirmPopoverToDeleteButtons() {
    var options = {
        placement: "bottom",
        selector: ".deletePopOver",
        title: "Confirm Delete?",
        trigger: "click",
        container: ".table-hover", //Fix for bootstrap table
        html: true,
        content: function (event) {
            var $content = $(this).parents(".btn-group:first").siblings(".confirmDeletePopover").find("> form").clone();
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

function applyTabs() {
    $('.bootstrapTabs a').click(function (e) {
        e.preventDefault()
        $(this).tab('show')
    })
}
