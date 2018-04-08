$(function () {
    $(".fa-info-circle[data-toggle='tooltip']").tooltip();

    //https://harvesthq.github.io/chosen/
    $(".applyChosen").chosen({ width: "100%" });

    $(".applyChosen").on('change', function (evt, params) {
        evt.currentTarget.value = params.selected;
    });
});