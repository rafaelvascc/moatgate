$(function () {
    $("table").bootstrapTable({
        striped: true,
        pagination: true,
        search: true,
        searchOnEnterKey: true,
        searchTimeOut: 200,
        columns: [
            {
                field: "ClientName",
                title: "Client Name",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: true,
                searchable: true,
            },{
                field: "ClientId",
                title: "Client Id",
                align: "left",
                halign: "left",
                valign: "middle",
                sortable: true,
                searchable: true,
            },{
                field: "ClientUri",
                title: "Client Uri",
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
                searchable: false,
            },{
                field: "Description",
                title: "Description",
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