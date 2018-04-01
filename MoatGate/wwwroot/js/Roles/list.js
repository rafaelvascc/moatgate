$(function () {
    $("table").bootstrapTable({
        striped: true,
        pagination: true,
        search: true,
        searchOnEnterKey: true,
        searchTimeOut: 200,
        columns: [
            {
                field: "RoleName",
                title: "Role Name",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: true,
                searchable: true,
            }, {
                field: "",
                title: "Actions",
                align: "right",
                halign: "center",
                valign: "middle",
                searchable: false,
                width: 120
            },
        ]
    });
});