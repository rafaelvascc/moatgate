$(function () {
    //https://datatables.net/
    var $table = $('table').DataTable({
        rowId: 'Id',
        processing: true,
        serverSide: true,
        searching: true,
        ajax: {
            url: "../../internal-api/resources/api/search",
            type: "POST"
        },
        columns: [
            {
                data: "name",
                name: "Name",
                title: "Name",
                searchable: true,
                searching: true
            },
            {
                data: "displayName",
                name: "DisplayName",
                title: "Display Name",
                searchable: true,
                searching: true
            },
            {
                data: "description",
                name: "Description",
                title: "Description",
                searchable: true,
                searching: true
            },
            {
                data: "enabled",
                name: "Enabled",
                title: "Enabled",
                searchable: true,
                searching: true,
                orderable: false
            },
            {
                class: "actions-control",
                title: "Actions",
                orderable: false,
                data: null,
                searchable: false,
                searching: false,
                width: "52px",
                render: function (c) {
                    return renderResourceActionButtons(c);
                }
            }
        ]
    });

    $("#btnSearch").on("click", function (e) {
        $table
            .columns([0]).search($("#txtName").val())
            .columns([1]).search($("#txtDisplayName").val())
            .columns([2]).search($("#txtDescription").val())
            .columns([3]).search($("#ddlEnabled").val());
        $table.draw();
    });

    var resourceActionsTemplate = $.templates("#resource-actions-template");

    function renderResourceActionButtons(client) {
        return resourceActionsTemplate.render(client);
    }
});