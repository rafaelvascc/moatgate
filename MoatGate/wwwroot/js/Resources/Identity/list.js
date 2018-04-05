$(function () {
    $("table").bootstrapTable({
        striped: true,
        pagination: true,
        searchOnEnterKey: true,
        searchTimeOut: 200,
        filterControl: true,
        hideUnusedSelectOptions: true,
        searchOnEnterKey: true,
        columns: [
            {
                field: "Name",
                title: "Name",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: true,
                searchable: true,
            },{
                field: "DisplayName",
                title: "Display Name",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: true,
                searchable: true,
            },{
                field: "Description",
                title: "Description",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: true,
                searchable: true,
            },{
                field: "Enabled",
                title: "Enabled",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: true,
                searchable: true,
            }, {
                field: "Required",
                title: "Required",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: true,
                searchable: true,
            }, {
                field: "ShowInDiscoveryDocument",
                title: "Discoverable",
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