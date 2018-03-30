$(function () {
    $("input[type='checkbox'].scopeCheckbox").off("change").on("change", function (event) {
        var $checkBox = $(event.target);
        var $hidden = $checkBox.siblings(".scopeCheckboxHiddentTextValue");
        if (!$checkBox.is(":checked")) {
            $checkBox.removeAttr("checked");
        }
        $hidden.attr("value", $checkBox.is(":checked"));
    });
});