$(function () {
    $('.downloadtooltip').tooltip();

    $('.toggle-event').change(function () {
        var credentialId = $(this).data("id");
        var showInOnlineDirectory = $(this).prop('checked');
        var requestUrl = $(this).data('request-url');
        updateCourses(credentialId, showInOnlineDirectory, requestUrl);
    });

    function updateCourses(credentialId, showInOnlineDirectory, requestUrl) {
        $.ajax({
            type: "POST",
            data: { credentialId: credentialId, showInOnlineDirectory: showInOnlineDirectory },
            cache: false,
            url: requestUrl,
            success: updateCoursesSuccess,
            error: updateCoursesError
        });
    }

    function updateCoursesSuccess(data) {
        if (data) {
            toastr.success("Saved successfully. Please note that this change may take a few minutes to update in the Online Directory.");
        } else {
            updateCoursesError();
        }

    }

    function updateCoursesError(error) {
        if (error.responseText.indexOf('User is not authorised to access this item.') !== -1) {
            toastr.error('User is not authorised to access this item.');
        } else {
            toastr.error("An error occurred while processing your request.");
        }
    }

    $('.downloadtooltip').click(function () {
        var credentialId = $(this).attr('data-credentialId');
        getCredentialAttachmentsById(credentialId);

        var $downloadFileModel = $('#downloadFileModel');
        $downloadFileModel.modal('show');
    });

    function getCredentialAttachmentsById(credentialId) {
        $.ajax({
            url: $("#credential-download-" + credentialId).data('request-url'),
            type: "GET",
            dataType: "json",
            data: { credentialId: credentialId },
            success: function (response) {
                //render the table
                $("#downloadFileModelTableBody").html('');

                $.each(response, function (index, value) {
                    var markup = "<tr>" +
                        "<td>" + moment(value.UploadedDateTime).format("DD/MM/YYYY") + "</td>" +
                        "<td class='smallwidth'>" + value.DocumentType + "</td>" +
                        "<td class='largewidth'>" + value.FileName + "</td>" +
                        "<td><span class='smallpadding'><a href=Credential/DownloadAttachmentByCredentialAttachmentId?storedFileId=" + value.StoredFileId + "><i class='glyphicon glyphicon-download-alt'></i></a></span></td>" +
                        "</tr>";

                    $("#downloadFileModelTableBody").append(markup);
                });
            },
            error: function (response) {
                toastr.error("An error occurred while processing your request.");
            }
        });
    }
});