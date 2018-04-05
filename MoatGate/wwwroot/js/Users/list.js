$(function () {
    $("table").bootstrapTable({
        striped: true,
        pagination: true,
        columns: [
            {
                field: "UserName",
                title: "UserName",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: true,
            },{
                field: "Email",
                title: "Email",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: true,
            }, {
                field: "PhoneNumber",
                title: "Phone Number",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: true,
            },{
                field: "LocakoutEnd",
                title: "Lockout End",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: true,
            },{
                field: "LockoutEnabled",
                title: "Lockout Enabled",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: false,
            }, {
                field: "AccessFailedCount",
                title: "Access Failed Count",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: true,
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