$(function () {
    function newClientSecretModel(index) {
        this.index = index;
        this.id = 0;
        this.type = "";
        this.value = "";
        this.expiration = moment().add(30, "d").format("YYYY-MM-DD");
        this.description = "";
    }

    function newClientRedirectUriModel(index) {
        this.index = index;
        this.value = "";
    }

    function newClientAllowedScopeModel(index) {
        this.index = index;
        this.value = "";
    }

    function newClientPropertyModel(index) {
        this.index = index;
        this.key = "";
        this.value = "";
    }

    var $secretsScope = $("section[data-scope='client-partials-secret-table']");
    var $redirectUriScope = $("section[data-scope='client-partials-redirect-uri']");
    var $allowedScopeScope = $("section[data-scope='client-partials-allowed-scopes']");
    var $clientPropertiesScope = $("section[data-scope='client-partials-client-property']");

    var secretCreateTemplate = $.templates("#client-secret-template");
    var redirectUriCreateTemplate = $.templates("#redirect-uri-template");
    var allowedScopeCreateTemplate = $.templates("#allowed-scope-template");
    var clientPropertiesCreateTemplate = $.templates("#client-property-template");

    var $tblClientSecrets = $("#tbl-client-secret", $secretsScope);
    var $tblRedirectUris = $("#tbl-redirect-uri", $redirectUriScope);
    var $tblAllowedScopes = $("#tbl-allowed-scope", $allowedScopeScope);
    var $tblClientProperties = $("#tbl-client-property", $clientPropertiesScope);

    var $tblClientSecretsBody = $("tbody", $tblClientSecrets);
    var $tblRedirectUrisBody = $("tbody", $tblRedirectUris);
    var $tblAllowedScopesBody = $("tbody", $tblAllowedScopes);
    var $tblClientPropertiesBody = $("tbody", $tblClientProperties);

    var $btnAddSecret = $("#btn-add-client-secret", $secretsScope);
    var $btnAddRedirectUri = $("#btn-add-redirect-uri", $redirectUriScope);
    var $btnAddAllowedScope = $("#btn-add-allowed-scope", $allowedScopeScope);
    var $btnAddClientProperty = $("#btn-add-client-property", $clientPropertiesScope);

    $btnAddSecret.off("click").on("click", function (event) {
        var count = $("tr", $tblClientSecretsBody).length;
        var newModel = new newClientSecretModel(count);
        var newHtml = secretCreateTemplate.render(newModel);
        $tblClientSecretsBody.append($(newHtml));
    });

    $btnAddRedirectUri.off("click").on("click", function (event) {
        var count = $("tr", $tblRedirectUrisBody).length;
        var newModel = new newClientRedirectUriModel(count);
        var newHtml = redirectUriCreateTemplate.render(newModel);
        $tblRedirectUrisBody.append($(newHtml));
    });

    $btnAddAllowedScope.off("click").on("click", function (event) {
        var count = $("tr", $tblAllowedScopesBody).length;
        var newModel = new newClientAllowedScopeModel(count);
        var newHtml = allowedScopeCreateTemplate.render(newModel);
        $tblAllowedScopesBody.append($(newHtml));
    });

    $btnAddClientProperty.off("click").on("click", function (event) {
        var count = $("tr", $tblClientPropertiesBody).length;
        var newModel = new newClientPropertyModel(count);
        var newHtml = clientPropertiesCreateTemplate.render(newModel);
        $tblClientPropertiesBody.append($(newHtml));
    });

    $tblClientSecretsBody.off("click").on("click", ".btn-delete-client-secret", function (event) {
        var $row = $(event.target).parents("tr:first");
        $row.remove();
        updateSecretIndexes();
    });

    $tblRedirectUrisBody.off("click").on("click", ".btn-delete-redirect-uri", function (event) {
        var $row = $(event.target).parents("tr:first");
        $row.remove();
        updateRedirectUriIndexes();
    });

    $tblAllowedScopesBody.off("click").on("click", ".btn-delete-allowed-scope", function (event) {
        var $row = $(event.target).parents("tr:first");
        $row.remove();
        updateAllowedScopesIndexes();
    });

    $tblClientPropertiesBody.off("click").on("click", ".btn-delete-client-property", function (event) {
        var $row = $(event.target).parents("tr:first");
        $row.remove();
        updateClientPropertiesIndexes();
    });

    function updateSecretIndexes() {
        var $rows = $tblClientSecretsBody.find("tr");
        for (var i = 0; i < $rows.length; i++) {
            var $row = $($rows[i]);
            var $fields = $row.find("[name^='Client.ClientSecrets[']");
            for (var j = 0; j < $fields.length; j++) {
                var $field = $($fields[j]);
                $field.attr("name", $field.attr("name").replace(/\[([1-9]\d*)\]/, "[" + i + "]"));
            }
        }
    }

    function updateRedirectUriIndexes() {
        var $rows = $tblClientSecretsBody.find("tr");
        for (var i = 0; i < $rows.length; i++) {
            var $row = $($rows[i]);
            var $fields = $row.find("[name^='Client.RedirectUris[']");
            for (var j = 0; j < $fields.length; j++) {
                var $field = $($fields[j]);
                $field.attr("name", $field.attr("name").replace(/\[([1-9]\d*)\]/, "[" + i + "]"));
            }
        }
    }

    function updateAllowedScopesIndexes() {
        var $rows = $tblAllowedScopesBody.find("tr");
        for (var i = 0; i < $rows.length; i++) {
            var $row = $($rows[i]);
            var $fields = $row.find("[name^='Client.AllowedScopes[']");
            for (var j = 0; j < $fields.length; j++) {
                var $field = $($fields[j]);
                $field.attr("name", $field.attr("name").replace(/\[([1-9]\d*)\]/, "[" + i + "]"));
            }
        }
    }
    
    function updateClientPropertiesIndexes() {
        var $rows = $tblClientPropertiesBody.find("tr");
        for (var i = 0; i < $rows.length; i++) {
            var $row = $($rows[i]);
            var $fields = $row.find("[name^='Client.Properties[']");
            for (var j = 0; j < $fields.length; j++) {
                var $field = $($fields[j]);
                $field.attr("name", $field.attr("name").replace(/\[([1-9]\d*)\]/, "[" + i + "]"));
            }
        }
    }
});