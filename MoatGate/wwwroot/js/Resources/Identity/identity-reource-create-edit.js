$(function () {
    function newIdentityResourceClaimModel(index) {
        this.index = index;
        this.id = 0;
        this.type = "";
    }

    //Identity Resource Claims
    var $identityResourceClaimScope = $("section[data-scope='partials-identity-resource-claim']");
    var identityResourceClaimCreateTemplate = $.templates("#identity-resource-claim-template");
    var $tblIdentityResourceClaims = $("#tbl-identity-resource-claim", $identityResourceClaimScope);
    var $tblIdentityResourceClaimsBody = $("tbody", $tblIdentityResourceClaims);
    var $btnAddIdentityResourceClaim = $("#btn-add-identity-resource-claim", $identityResourceClaimScope);

    $btnAddIdentityResourceClaim.off("click").on("click", function (event) {
        var count = $("tr", $tblIdentityResourceClaimsBody).length;
        var newModel = new newIdentityResourceClaimModel(count);
        var newHtml = identityResourceClaimCreateTemplate.render(newModel);
        $tblIdentityResourceClaimsBody.append($(newHtml));
    });

    $tblIdentityResourceClaimsBody.off("click").on("click", ".btn-delete-identity-resource-claim", function (event) {
        var $row = $(event.target).parents("tr:first");
        $row.remove();
        updateClientClaimIndexes();
    });

    function updateClientClaimIndexes() {
        var $rows = $tblIdentityResourceClaimsBody.find("tr");
        for (var i = 0; i < $rows.length; i++) {
            var $row = $($rows[i]);
            var $fields = $row.find("[name^='IdentityResource.UserClaims[']");
            for (var j = 0; j < $fields.length; j++) {
                var $field = $($fields[j]);
                $field.attr("name", $field.attr("name").replace(/\[([1-9]\d*)\]/, "[" + i + "]"));
            }
        }
    }
});