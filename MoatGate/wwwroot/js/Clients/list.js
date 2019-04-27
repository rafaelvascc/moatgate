$(function () {
    //https://datatables.net/
    var $table = $('table').DataTable({
        rowId: 'Id',
        processing: true,
        serverSide: true,
        searching: true,
        ajax: {
            url: "../internal-api/clients/search",
            type: "POST"
        },
        columns: [
            {
                data: "clientName",
                name: "ClientName",
                title: "Client Name",
                searchable: true,
                searching: true
            },
            {
                data: "clientId",
                name: "ClientId",
                title: "Client Id",
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
                searching: true
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
                    return renderClientActionButtons(c);
                }
            },
        ]
    });

    $("#btnSearch").on("click", function (e) {
        $table
            .columns([0]).search($("#txtClientName").val())
            .columns([1]).search($("#txtClientId").val())
            .columns([2]).search($("#txtDescription").val())
            .columns([3]).search($("#ddlEnabled").val())
        $table.draw();
    });

    var clientActionsTemplate = $.templates("#client-actions-template");

    function renderClientActionButtons(client) {
        return clientActionsTemplate.render(client);
    }
});