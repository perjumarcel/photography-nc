$(function () {
    var _$albumCombobox = $('#AlbumId');
    var _imageService = abp.services.app.image;

    _$albumCombobox.change(function () {
        location.href = abp.adminPath + 'Images/index?albumId=' + _$albumCombobox.val();
    });

    $('.delete-image').click(function () {
        var imageId = $(this).attr("data-image-id");
        var imageTitle = $(this).attr('data-image-title');

        deleteImage(imageId, imageTitle);
    });

    $('#upload-images').click(function (e) {
        var albumId = _$albumCombobox.val();

        e.preventDefault();
        $.ajax({
            url: abp.appPath + 'Admin/Images/UploadImageModal?albumId=' + albumId,
            type: 'POST',
            contentType: 'application/html',
            success: function (content) {
                $('#UploadImageModal div.modal-content').html(content);
            },
            error: function (e) { }
        });
    });

    $('.cover-set-button').click(function () {
        var imageId = $(this).attr("data-image-id");
        var imageOriginalname = $(this).attr("data-image-originalname");
        var $images = $('#images');

        abp.message.confirm(
            abp.utils.formatString(abp.localization.localize('AreYouSureWantToUseAsCover', 'Photography'), ""),
            function (isConfirmed) {
                if (isConfirmed) {
                    abp.ui.setBusy($images);
                    _imageService.setImageAsCover(imageId)
                        .done(function () {
                            $('.box-item-base').removeClass("album-cover");
                            $('#' + imageId).addClass("album-cover");
                            abp.notify.success("Image " + imageOriginalname + " set as cover successfully!");
                            abp.ui.clearBusy($images);
                        })
                        .error(function () {
                            abp.notify.error("Failed to set as cover Image: " + imageOriginalname + "!");
                            abp.ui.clearBusy($images);
                        });
                }
            }
        );
    });
    
    function deleteImage(imageId, imageTitle) {
        abp.message.confirm(
            abp.utils.formatString(abp.localization.localize('AreYouSureWantToDelete', 'Photography'), imageTitle),
            function (isConfirmed) {
                if (isConfirmed) {
                    _imageService.delete({
                        id: imageId
                    }).done(function () {
                        $('#' + imageId).remove();
                        abp.notify.success("Image " + imageTitle + " removed successfully!");
                    }).error(function () {
                        abp.notify.error("Failed to remove Image: " + imageTitle + "!");
                    });
                }
            }
        );
    }
});
