(function () {
    $(function () {

        var _albumService = abp.services.app.album;
        var _$modal = $('#AlbumCreateModal');
        var _$form = _$modal.find('form');

        _$form.validate();

        $('#RefreshButton').click(function () {
            refreshAlbumList();
        });

        $('#eventDateCreate').bootstrapMaterialDatePicker({
            format: 'DD/MM/YYYY',
            time: false
        });

        $('.delete-album').click(function () {
            var albumId = $(this).attr("data-album-id");
            var albumTitle = $(this).attr('data-album-title');

            deleteAlbum(albumId, albumTitle);
        });

        $('.edit-album').click(function (e) {
            var albumId = $(this).attr("data-album-id");

            e.preventDefault();
            $.ajax({
                url: abp.appPath + 'Admin/Albums/EditAlbumModal?id=' + albumId,
                type: 'POST',
                contentType: 'application/html',
                success: function (content) {
                    $('#AlbumEditModal div.modal-content').html(content);
                },
                error: function (e) { }
            });
        });

        $('.upload-images').click(function (e) {
            var albumId = $(this).attr("data-album-id");

            location.href = abp.appPath + 'Admin/Images/Index?albumId=' + albumId;
        });

        _$form.find('button[type="submit"]').click(function (e) {
            e.preventDefault();

            if (!_$form.valid()) {
                return;
            }

            var album = _$form.serializeFormToObject(); //serializeFormToObject is defined in main.js

            abp.ui.setBusy(_$modal);
            _albumService.create(album).done(function () {
                _$modal.modal('hide');
                location.reload(true); //reload page to see new album!
            }).always(function () {
                abp.ui.clearBusy(_$modal);
            });
        });

        _$modal.on('shown.bs.modal', function () {
            _$modal.find('input:not([type=hidden]):first').focus();
        });

        function refreshAlbumList() {
            location.reload(true); //reload page to see new album!
        }

        function deleteAlbum(albumId, albumTitle) {
            abp.message.confirm(
                abp.utils.formatString(abp.localization.localize('AreYouSureWantToDelete', 'Photography'), albumTitle),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _albumService.delete({
                            id: albumId
                        }).done(function () {
                            refreshAlbumList();
                        });
                    }
                }
            );
        }
    });
})();