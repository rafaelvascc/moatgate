$(function () {
    var $inputAvatar = $("#fileProfileavatar");

    $(".fa-info-circle[data-toggle='tooltip']").tooltip();

    //https://harvesthq.github.io/chosen/
    $(".applyChosen").chosen({ width: "100%" });

    $(".applyChosen").on('change', function (evt, params) {
        evt.currentTarget.value = params.selected;
    });

    $("#profile-img").off().on("click", function (e) {
        $inputAvatar.trigger("click");
    });

    $inputAvatar.off("change").on("change", function (e) {
        var formData = new FormData();
        formData.append('avatar', e.target.files[0]);

        $.ajax({
            url: "../api/internal/profile/avatar",
            data: formData,
            processData: false,
            contentType: false,
            type: "POST",
            success: function (r) {
                console.log(r);
                $("#profile-img").attr("src", r);
                $("#txtEncodedAvatar").val(r);
            },
            error: function (e) {
                console.log(e);
            }
        });
    });
});