$(function () {
    //https://datatables.net/
    var $table = $('table').DataTable({
        rowId: 'Id',
        processing: true,
        serverSide: true,
        searching: true,
        searchDelay: 350,
        ajax: {
            url: "../api/internal/roles/search",
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
                class: "actions-control",
                title: "Actions",
                orderable: false,
                data: null,
                searchable: false,
                searching: false,
                width: "52px",
                render: function (c) {
                    return renderRoleActionButtons(c);
                }
            }
        ]
    });

    var roleActionsTemplate = $.templates("#role-actions-template");

    function renderRoleActionButtons(role) {
        return roleActionsTemplate.render(role);
    }
});