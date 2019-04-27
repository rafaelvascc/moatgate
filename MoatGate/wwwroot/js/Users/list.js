$(function () {
    //https://harvesthq.github.io/chosen/
    $("#ddlRoles").chosen({ width: "100%" });

    //https://datatables.net/
    var $table = $('table').DataTable({
        rowId: 'Id',
        processing: true,
        serverSide: true,
        searching: true,
        ajax: {
            url: "../internal-api/users/search",
            type: "POST",
            data: function (d) {
                var additionalSearchParams = [{
                    data: "lockoutEndFrom",
                    name: "LockoutEndFrom",
                    orderable: true,
                    search: {
                        regex: false,
                        value: $("#txtFromLockoutEnd").val()
                    },
                    searchable: true
                }, {
                    data: "lockoutEndTo",
                    name: "LockoutEndTo",
                    orderable: true,
                    search: {
                        regex: false,
                        value: $("#txtToLockoutEnd").val()
                    },
                    searchable: true
                }, {
                    data: "lockoutEnabled",
                    name: "LockoutEnabled",
                    orderable: true,
                    search: {
                        regex: false,
                        value: $("#ddlLockoutEnabled").val()
                    },
                    searchable: true
                },{
                    data: "accessFailedCount",
                    name: "AccessFailedCount",
                    orderable: true,
                    search: {
                        regex: false,
                        value: $("#numbFailedCount").val()
                    },
                    searchable: true
                },];

                d.columns.concat(additionalSearchParams);
                
                d.Id = $("#txtId").val();

                d.roles = $("#ddlRoles").val();

                return d;
            }
        },
        columns: [
            {
                class: "details-control text-center",
                orderable: false,
                data: null,
                defaultContent: "<i class='fas fa-plus'></i>",
                searchable: false,
                searching: false
            },
            {
                data: "name",
                name: "Name",
                title: "Name",
                searchable: true,
                searching: true
            },
            {
                data: "user.userName",
                name: "UserName",
                title: "User Name",
                searchable: true,
                searching: true
            },
            {
                data: "user.email",
                name: "Email",
                title: "Email",
                searchable: true,
                searching: true
            },
            {
                data: "user.phoneNumber",
                name: "PhoneNumber",
                title: "Phone Number",
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
                render: function (o) {
                    return renderUseractionButtons(o.user);
                }
            },
        ]
    });

    $("#btnSearch").on("click", function (e) {
        $table
            .columns([1]).search($("#txtName").val())
            .columns([2]).search($("#txtUserName").val())
            .columns([3]).search($("#txtEmail").val())
            .columns([4]).search($("#txtPhoneNumber").val())
        $table.draw();
    });

    //https://datatables.net/examples/server_side/row_details.html
    var detailRows = [];

    $table.on( 'click', 'tbody tr td.details-control', function () {
        var $tr = $(this).closest('tr');
        var row = $table.row($tr);
        var idx = $.inArray($tr.attr('id'), detailRows);
 
        if (row.child.isShown()) {
            $tr.find('i:first').addClass('fa-plus');
            $tr.find('i:first').removeClass('fa-minus');
            row.child.hide();
 
            // Remove from the 'open' array
            detailRows.splice(idx, 1);
        }
        else {
            $tr.find('i:first').removeClass('fa-plus');
            $tr.find('i:first').addClass('fa-minus');
            row.child(formatDetails(row.data())).show();
 
            // Add to the 'open' array
            if ( idx === -1 ) {
                detailRows.push( $tr.attr('id') );
            }
        }
    });

    // On each draw, loop over the `detailRows` array and show any child rows
    $table.on('draw', function () {
        $.each(detailRows, function (i, id) {
            $('#' + id + ' td.details-control').trigger( 'click' );
        });
    });
    
    var userDetailsTemplate = $.templates("#user-details-template");

    function formatDetails (d) {
        return userDetailsTemplate.render(d);
    }
    
    var userActionsTemplate = $.templates("#user-actions-template");

    function renderUseractionButtons(user) {
        return userActionsTemplate.render(user);
    }
});