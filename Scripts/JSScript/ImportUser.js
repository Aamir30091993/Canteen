$('#clearAttachemnt').click(function () {
    var sessionkey = $('#AttachmentSessionKey').val();
    $.ajax({
        url: 'RemoveAttachment?SessionKey=' + sessionkey,
        type: 'POST',
        success: function (data) {
            $('#AttachmentSessionKey').val("");
            $('#hdfAttachmentStatus').val(false);
            $("#divAttachment").show();
            $("#Attachment").css("display", "block");
            $("#divAttachmentView").hide();
            $("#divClearAttachment").hide();
            $('#Attachment').val('');

        },
        error: function (error) {
            alert("Error while removing the attachment")
        }
    });
});
$(document).on("change", "#Attachment", function () {
    debugger;
    var formData = new FormData();
    var file = document.getElementById("Attachment").files[0];
    formData.append("AttachedFile", file);

    $.ajax({
        type: "POST",
        url: 'UploadAttachment',
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            $('#hdfAttachmentStatus').val(true);
            $("#divClearAttachment").show();
            $('#AttachmentSessionKey').val(response);
            if ($("#hdfUpload") != null)
                $("#hdfUpload").val("1");

        },
        error: function (error) {
            alert("error")
        }
    });
});
$("#btnClose").click(function (e) {
    $("#myModalSave").modal("hide");
    window.location.href = "/ImportUser/List";
});
$("#btnExport").click(function (e) {
    var sessionkey = $("#SessionIdentity").val();
    window.location.href = 'Export?SessionIdentity=' + sessionkey;
    //$.ajax({
    //    async: false,
    //    cache:false,
    //    url: 'Export',
    //    type: 'GET',
    //    success: function (data) {
    //        //parseXml(data);
    //    },
    //    error: function (request, status, error) {
    //        alert(request.responseText);
    //    }
    //});
});

function parseXml(xml) {
    var item = $(xml).find("item");

    $(item).each(function () {
        $("#results").append($("enclosure").attr("url").text() + "<br />");
    });

}