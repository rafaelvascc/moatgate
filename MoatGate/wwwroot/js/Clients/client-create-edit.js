﻿$(function () {
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
        this.id = 0;
        this.value = "";
    }

    function newClientAllowedScopeModel(index) {
        this.index = index;
        this.id = 0;
        this.value = "";
    }

    function newClientPropertyModel(index) {
        this.index = index;
        this.id = 0;
        this.key = "";
        this.value = "";
    }

    function newClientRedirectLogoutUriModel(index) {
        this.index = index;
        this.id = 0;
        this.value = "";
    }

    function newClientIdpRestrictionModel(index) {
        this.index = index;
        this.id = 0;
        this.value = "";
    }

    //Client secrets
    var $secretsScope = $("section[data-scope='client-partials-secret-table']");
    var secretCreateTemplate = $.templates("#client-secret-template");
    var $tblClientSecrets = $("#tbl-client-secret", $secretsScope);
    var $tblClientSecretsBody = $("tbody", $tblClientSecrets);
    var $btnAddSecret = $("#btn-add-client-secret", $secretsScope);

    $btnAddSecret.off("click").on("click", function (event) {
        var count = $("tr", $tblClientSecretsBody).length;
        var newModel = new newClientSecretModel(count);
        var newHtml = secretCreateTemplate.render(newModel);
        $tblClientSecretsBody.append($(newHtml));
    });

    $tblClientSecretsBody.off("click").on("click", ".btn-delete-client-secret", function (event) {
        var $row = $(event.target).parents("tr:first");
        $row.remove();
        updateSecretIndexes();
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

    //Redirect Uri
    var $redirectUriScope = $("section[data-scope='client-partials-redirect-uri']");
    var redirectUriCreateTemplate = $.templates("#redirect-uri-template");
    var $tblRedirectUris = $("#tbl-redirect-uri", $redirectUriScope);
    var $tblRedirectUrisBody = $("tbody", $tblRedirectUris);
    var $btnAddRedirectUri = $("#btn-add-redirect-uri", $redirectUriScope);

    $btnAddRedirectUri.off("click").on("click", function (event) {
        var count = $("tr", $tblRedirectUrisBody).length;
        var newModel = new newClientRedirectUriModel(count);
        var newHtml = redirectUriCreateTemplate.render(newModel);
        $tblRedirectUrisBody.append($(newHtml));
    });

    $tblRedirectUrisBody.off("click").on("click", ".btn-delete-redirect-uri", function (event) {
        var $row = $(event.target).parents("tr:first");
        $row.remove();
        updateRedirectUriIndexes();
    });

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

    //Allowed Scopes
    var $allowedScopeScope = $("section[data-scope='client-partials-allowed-scopes']");
    var allowedScopeCreateTemplate = $.templates("#allowed-scope-template");
    var $tblAllowedScopes = $("#tbl-allowed-scope", $allowedScopeScope);
    var $tblAllowedScopesBody = $("tbody", $tblAllowedScopes);
    var $btnAddAllowedScope = $("#btn-add-allowed-scope", $allowedScopeScope);

    $btnAddAllowedScope.off("click").on("click", function (event) {
        var count = $("tr", $tblAllowedScopesBody).length;
        var newModel = new newClientAllowedScopeModel(count);
        var newHtml = allowedScopeCreateTemplate.render(newModel);
        $tblAllowedScopesBody.append($(newHtml));
    });

    $tblAllowedScopesBody.off("click").on("click", ".btn-delete-allowed-scope", function (event) {
        var $row = $(event.target).parents("tr:first");
        $row.remove();
        updateAllowedScopesIndexes();
    });

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

    //Client Properties
    var $clientPropertiesScope = $("section[data-scope='client-partials-client-property']");
    var clientPropertiesCreateTemplate = $.templates("#client-property-template");
    var $tblClientProperties = $("#tbl-client-property", $clientPropertiesScope);
    var $tblClientPropertiesBody = $("tbody", $tblClientProperties);
    var $btnAddClientProperty = $("#btn-add-client-property", $clientPropertiesScope);

    $btnAddClientProperty.off("click").on("click", function (event) {
        var count = $("tr", $tblClientPropertiesBody).length;
        var newModel = new newClientPropertyModel(count);
        var newHtml = clientPropertiesCreateTemplate.render(newModel);
        $tblClientPropertiesBody.append($(newHtml));
    });

    $tblClientPropertiesBody.off("click").on("click", ".btn-delete-client-property", function (event) {
        var $row = $(event.target).parents("tr:first");
        $row.remove();
        updateClientPropertiesIndexes();
    });

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

    //Redirect Logout Uris
    var $redirectLogoutUriScope = $("section[data-scope='client-partials-logout-redirect-uri']");
    var redirectLogoutUriCreateTemplate = $.templates("#logout-redirect-uri-template");
    var $tblRedirectLogoutUris = $("#tbl-logout-redirect-uri", $redirectLogoutUriScope);
    var $tblRedirectLogoutUrisBody = $("tbody", $tblRedirectLogoutUris);
    var $btnAddRedirectLogoutUri = $("#btn-add-logout-redirect-uri", $redirectLogoutUriScope);

    $btnAddRedirectLogoutUri.off("click").on("click", function (event) {
        var count = $("tr", $tblRedirectLogoutUrisBody).length;
        var newModel = new newClientRedirectLogoutUriModel(count);
        var newHtml = redirectLogoutUriCreateTemplate.render(newModel);
        $tblRedirectLogoutUrisBody.append($(newHtml));
    });

    $tblRedirectLogoutUrisBody.off("click").on("click", ".btn-delete-logout-redirect-uri", function (event) {
        var $row = $(event.target).parents("tr:first");
        $row.remove();
        updateRedirectLogoutUriIndexes();
    });

    function updateRedirectLogoutUriIndexes() {
        var $rows = $tblClientSecretsBody.find("tr");
        for (var i = 0; i < $rows.length; i++) {
            var $row = $($rows[i]);
            var $fields = $row.find("[name^='Client.PostLogoutRedirectUris[']");
            for (var j = 0; j < $fields.length; j++) {
                var $field = $($fields[j]);
                $field.attr("name", $field.attr("name").replace(/\[([1-9]\d*)\]/, "[" + i + "]"));
            }
        }
    }

    //Client identity provider restrictions
    var $idpRestrictionScope = $("section[data-scope='client-partials-idp-restrictions']");
    var idpRestrictionCreateTemplate = $.templates("#idp-restriction-template");
    var $tblClientIdpRestrictions = $("#tbl-idp-restriction", $idpRestrictionScope);
    var $tblClientIdpRestrictionsBody = $("tbody", $tblClientIdpRestrictions);
    var $btnAddIdpRestriction = $("#btn-add-idp-restriction", $idpRestrictionScope);

    $btnAddIdpRestriction.off("click").on("click", function (event) {
        var count = $("tr", $tblClientIdpRestrictionsBody).length;
        var newModel = new newClientSecretModel(count);
        var newHtml = idpRestrictionCreateTemplate.render(newModel);
        $tblClientIdpRestrictionsBody.append($(newHtml));
    });

    $tblClientIdpRestrictionsBody.off("click").on("click", ".btn-delete-idp-restriction", function (event) {
        var $row = $(event.target).parents("tr:first");
        $row.remove();
        updateProviderRestrictionsIndexes();
    });

    function updateProviderRestrictionsIndexes() {
        var $rows = $tblClientIdpRestrictionsBody.find("tr");
        for (var i = 0; i < $rows.length; i++) {
            var $row = $($rows[i]);
            var $fields = $row.find("[name^='Client.IdentityProviderRestrictions[']");
            for (var j = 0; j < $fields.length; j++) {
                var $field = $($fields[j]);
                $field.attr("name", $field.attr("name").replace(/\[([1-9]\d*)\]/, "[" + i + "]"));
            }
        }
    }
});