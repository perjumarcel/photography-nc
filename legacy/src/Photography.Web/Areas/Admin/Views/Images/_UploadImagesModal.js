var _fineuploaderOnCompleteAll = _fineuploaderOnCompleteAll || {};
(function ($) {

    //var _imageService = abp.services.app.image;
    var _$modal = $('#UploadImageModal');
    var _$form = $('form[name=UploadImageForm]');
    var shouldPreventFromSave = false;

    _fineuploaderOnCompleteAll = function (succeeded, failed) {
        if (failed.length === 0) {
            abp.notify.success("All files uploaded successfully", "Upload successful");
            $fineuploader.fineUploader('clearStoredFiles');
            _$modal.modal('hide');
            location.reload(true); //reload page to see edited role!
        } else {
            abp.notify.error("Failed to upload all files", "Upload Failed");
            _$form.closest('div.modal-content').find(".upload-button").hide();
        }

        abp.ui.clearBusy(_$modal);
    }

    function upload() {
        if (!_$form.valid()) {
            return;
        }
        abp.ui.setBusy(_$modal);

        $fineuploader.fineUploader('uploadStoredFiles');
    }

    //Handle upload button click
    _$form.closest('div.modal-content').find(".upload-button").click(function (e) {
        e.preventDefault();
        if (!shouldPreventFromSave) {
            upload();
        }
    });

    //Handle enter key
    _$form.find('input').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            if (!shouldPreventFromSave) {
                upload();
            }
        }
    });

    $.AdminBSB.input.activate(_$form);

    _$modal.on('shown.bs.modal', function () {
        _$form.find('input[type=text]:first').focus();
    });

})(jQuery);