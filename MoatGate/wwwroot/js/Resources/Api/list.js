﻿$(function () {
    $("table").bootstrapTable({
        striped: true,
        pagination: true,
        search: true,
        searchOnEnterKey: true,
        searchTimeOut: 200,
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