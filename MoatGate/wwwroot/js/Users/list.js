$(function () {
    $("table").bootstrapTable({
        striped: true,
        pagination: true,
        search: true,
        searchOnEnterKey: true,
        searchTimeOut: 200,
        columns: [
            {
                field: "UserName",
                title: "UserName",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: true,
                searchable: true,
            },{
                field: "Email",
                title: "Email",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: true,
                searchable: true,
            },{
                field: "LocakoutEnd",
                title: "Lockout End",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: true,
                searchable: true,
            },{
                field: "LockoutEnabled",
                title: "LockoutEnabled",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: false,
                searchable: false,
            }, {
                field: "AccessFailedCount",
                title: "Access Failed Count",
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
                width: 85
            },
        ]
    });
});