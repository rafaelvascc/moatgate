$(function () {
    applyTooltips();
    appyConfirmPopoverToDeleteButtons();
    applyTabs();

    $(".noEnterForm").on("keydown keypress keyup", function (e) {
        if (event.keyCode === 13) {
            event.preventDefault();
            return false;
        }
    });


    $("#deleteModal").on('shown.bs.modal', function (e) {
        var idToDelete = e.relatedTarget.attributes["data-id"].value;
        var deleteUrl = e.relatedTarget.attributes["data-url"].value;
        $("#deleteModal #btnConfirmDelete").off("click").on("click", function (event) {
            $.ajax({
                type: "POST",
                contentType: "application/json",
                url: deleteUrl,
                data: JSON.stringify({ Id: idToDelete }),
                success: function (response) {
                    $.notify({
                        icon: "fas fa-check",
                        message: "Record successfully deleted",
                    }, {
                            // settings
                            type: 'success',
                            delay: 5000,
                            animate: {
                                enter: 'animated fadeInDown',
                                exit: 'animated fadeOutUp'
                            }
                        });

                    var $table = $(e.relatedTarget).parents("table:first");

                    if ($.fn.DataTable.isDataTable($table))
                        $table.dataTable().api().draw();
                },
                error: function (response) {
                    $.notify({
                        icon: "fas fa-times",
                        message: "Failed to delete record",
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
                    $("#deleteModal").modal("hide");
                }
            });
        });
    });
});

function appyConfirmPopoverToDeleteButtons() {
    var options = {
        placement: "bottom",
        selector: ".deletePopOver",
        title: "Confirm Delete?",
        trigger: "click",
        //container: ".table-hover", //Fix for bootstrap table
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
    };

    $("body").popover(options);
}

function applyTabs() {
    $('.bootstrapTabs a').click(function (e) {
        e.preventDefault();
        $(this).tab('show');
    });
}

function applyTooltips() {
    $('[data-toggle="tooltip"]').tooltip();
}
