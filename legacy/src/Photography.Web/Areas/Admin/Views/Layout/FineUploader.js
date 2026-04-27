var $fineuploader = $fineuploader || {};

$(function () {
    _fineuploaderOnCompleteAll = _fineuploaderOnCompleteAll || function (id, fileName, result) {
        if (result.success) {
            abp.notify.success(message, "Upload successful");
        } else {
            abp.notify.error(result.error, "Upload failed");
        }
    };

    $fineuploader = $(options.fineUploaderId).fineUploader({
        template: 'qq-template-manual-trigger',
        request: {
            endpoint: abp.appPath + 'Admin/Images/Upload',//options.url,
            customHeaders: {
                'X-XSRF-TOKEN': abp.security.antiForgery.getToken()
            },
            params: {
                albumId: options.albumId
            }
        },
        thumbnails: {
            placeholders: {
                waitingPath: '/css/admin/fineuploader/placeholders/waiting-generic.png',
                notAvailablePath: '/css/admin/fineuploader/placeholders/not_available-generic.png'
            }
        },

        autoUpload: false,
        multiple: options.multiple,
        validation: {
            allowedExtensions: ['jpeg', 'jpg'],
            sizeLimit: 200512000
        },
        showMessage: function (message) {
            abp.notify.warn(message, "Upload warn");
        },
        dragAndDrop: {
            extraDropzones: [$(".body-content")]
        },
        callbacks: {
            onSubmit: validateFileOrientation,
            onAllComplete: _fineuploaderOnCompleteAll,
            onComplete: function (id, fileName, result) {
                //debugger
                if (result.success) {
                    var message = 'Loaded file: ' + fileName + ". Successfully! File DB ID - " + result.fileid;
                    abp.notify.success(message, "Upload successful");
                } else {
                    abp.notify.error(result.error, "Upload failed");
                }
            }
        }
    });

    function validateFileOrientation(id) {
        var deferred = new $.Deferred(),
            file = $fineuploader.fineUploader('getFile', id),
            imageValidator = new qq.ImageValidation(file, function () { });

        imageValidator.getWidthHeight().done(function (dimensions) {
            if (options.useOrientationValidation && dimensions.width < dimensions.height) {
                abp.notify.warn(file.name + " has Invalid orientation", "Upload failed");
                deferred.reject();
            }
            else
                deferred.resolve();

        });

        return deferred.promise();
    }

    //$('#trigger-upload').click(function () {
    //    $('#fine-uploader-manual-trigger').fineUploader('uploadStoredFiles');
    //});

});
