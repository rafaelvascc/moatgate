$(function () {
    $(".select-user-claims").select2({
        theme: "bootstrap"
    });

    function newApiResourceSecretModel(index) {
        this.index = index;
        this.id = 0;
        this.type = "";
        this.value = "";
        this.expiration = moment().add(30, "d").format("YYYY-MM-DD");
        this.description = "";
    }

    function newApiResouceScopeModel(index) {
        this.index = index;
        this.id = 0;
        this.name = "";
        this.displayName = "";
        this.description = "";
        this.required = false;
        this.emphasize = false;
        this.showInDiscoveryDocument = true;
        this.userScopes = [];
    }

    //function newApiScopeClaimModel(index, parentIndex) {
    //    this.index = index;
    //    this.parentIndex = parentIndex;
    //    this.id = 0;
    //    this.type = "";
    //}

    //Client secrets
    var $secretsScope = $("section[data-scope='partials-api-resource-secrets']");
    var secretCreateTemplate = $.templates("#api-resource-secret-template");
    var $tblApiResourceSecrets = $("#tbl-api-resource-secret", $secretsScope);
    var $tblApiResourceSecretsBody = $("tbody", $tblApiResourceSecrets);
    var $btnAddSecret = $("#btn-add-api-resource-secret", $secretsScope);

    $btnAddSecret.off("click").on("click", function (event) {
        var count = $("tr", $tblApiResourceSecretsBody).length;
        var newModel = new newApiResourceSecretModel(count);
        var newHtml = secretCreateTemplate.render(newModel);
        $tblApiResourceSecretsBody.append($(newHtml));
    });

    $tblApiResourceSecretsBody.off("click").on("click", ".btn-delete-api-resource-secret", function (event) {
        var $row = $(event.target).parents("tr:first");
        $row.remove();
        updateSecretIndexes();
    });

    function updateSecretIndexes() {
        var $rows = $tblApiResourceSecretsBody.find("tr");
        for (var i = 0; i < $rows.length; i++) {
            var $row = $($rows[i]);
            var $fields = $row.find("[name^='Client.ClientSecrets[']");
            for (var j = 0; j < $fields.length; j++) {
                var $field = $($fields[j]);
                $field.attr("name", $field.attr("name").replace(/\[([1-9]\d*)\]/, "[" + i + "]"));
            }
        }
    }

    //Allowed Scopes
    var $allowedScopeScope = $("section[data-scope='partials-api-resource-scopes']");
    var allowedScopeCreateTemplate = $.templates("#api-resource-scope-template");
    var $tblAllowedScopes = $("#tbl-api-resource-scope", $allowedScopeScope);
    var $tblAllowedScopesBody = $("tbody", $tblAllowedScopes);
    var $btnAddAllowedScope = $("#btn-add-api-resource-scope", $allowedScopeScope);

    $btnAddAllowedScope.off("click").on("click", function (event) {
        var count = $("tr.scopeValues", $tblAllowedScopesBody).length;
        var newModel = new newApiResouceScopeModel(count);
        var $newHtml = $(allowedScopeCreateTemplate.render(newModel));
        var $baseDdlClaims = $("#ddl-claims-base").clone().attr("id", "").addClass("select-user-claims").attr('name', 'ScopeUserClaims[' + (count + 1) * -1 + ']');
        $newHtml.find('.claims-cell').append($baseDdlClaims);
        $baseDdlClaims.select2({
            theme: "bootstrap"
        });
        $tblAllowedScopesBody.append($newHtml);
    });

    $tblAllowedScopesBody.off("click").on("click", ".btn-delete-api-resource-scope", function (event) {
        var $row = $(event.target).parents("tr.scopeValues:first");
        var parentIndex = $row.attr("data-index");
        var $children = $("tr[data-parent-index='" + parentIndex + "']", $tblAllowedScopesBody);
        $row.remove();
        $children.remove();
        updateAllowedScopesIndexes();
    });

    function updateAllowedScopesIndexes() {
        var $rows = $tblAllowedScopesBody.find("tr.scopeValues");
        for (var i = 0; i < $rows.length; i++) {
            var $row = $($rows[i]);
            var oldIndex = $row.attr("data-index");
            $row.attr("data-index", i);
            var $fields = $row.find("[name^='ApiResource.Scopes[']");
            for (var j = 0; j < $fields.length; j++) {
                var $field = $($fields[j]);
                $field.attr("name", $field.attr("name").replace(/Scopes\[([1-9]\d*)\]/, "Scopes[" + i + "]"));
            }

            var $children = $("tr[data-parent-index='" + oldIndex + "']", $tblAllowedScopesBody);
            $children.attr("data-parent-index", i);
            $fields = $children.find("[name^='ApiResource.Scopes[']");
            for (var k = 0; k < $fields.length; k++) {
                var $childField = $($fields[k]);
                $childField.attr("name", $childField.attr("name").replace(/Scopes\[([1-9]\d*)\]/, "Scopes[" + i + "]"));
            }
        }
    }

    //Scopes Claims
    //var allowedScopeClaimCreateTemplate = $.templates("#api-resource-scope-claim-template");

    //$tblAllowedScopesBody.on("click", "button.btn-add-api-resource-scope-claim", function (event) {
    //    var $row = $(event.target).parents("tr:first");
    //    var parentIndex = $row.attr("data-index");
    //    var $siblings = $("tr[data-parent-index='" + parentIndex + "']", $tblAllowedScopesBody);
    //    var count = $siblings.length;
    //    var newModel = new newApiScopeClaimModel(count, parentIndex);
    //    var newHtml = allowedScopeClaimCreateTemplate.render(newModel);
    //    if (count === 0) {
    //        $row.after($(newHtml));
    //    }
    //    else {
    //        $siblings.last().after($(newHtml));
    //    }
    //});

    //$tblAllowedScopesBody.on("click", "tr.scopeClaim button.btn-delete-api-resource-scope-claim", function (event) {
    //    var $row = $(event.target).parents("tr.scopeClaim:first");
    //    var parentIndex = $row.attr("data-parent-index");
    //    $row.remove();
    //    updateScopesClaimsIndexes(parentIndex);
    //});

    //function updateScopesClaimsIndexes(parentIndex) {
    //    var $rows = $tblAllowedScopesBody.find("tr.scopeClaim[data-parent-index='" + parentIndex + "']");
    //    for (var i = 0; i < $rows.length; i++) {
    //        var $row = $($rows[i]);
    //        $row.attr("data-index", i);
    //        var $fields = $row.find("[name^='ApiResource.Scopes[']");
    //        for (var j = 0; j < $fields.length; j++) {
    //            var $field = $($fields[j]);
    //            $field.attr("name", $field.attr("name").replace(/UserClaims\[([1-9]\d*)\]/, "UserClaims[" + i + "]"));
    //        }
    //    }
    //}

    //Scope checkbox binding fix
    $tblAllowedScopesBody.off("change").on("change", "input[type='checkbox'].scopeCheckbox", function (event) {
        var $checkBox = $(event.target);
        var $hidden = $checkBox.siblings(".scopeCheckboxHiddentTextValue");
        if (!$checkBox.is(":checked")) {
            $checkBox.removeAttr("checked");
        }
        $hidden.attr("value", $checkBox.is(":checked"));
    });
});