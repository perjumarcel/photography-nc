(function () {
    $(function () {

        var _categoryService = abp.services.app.category;
        var _$modal = $('#CategoryCreateModal');
        var _$form = _$modal.find('form');

        _$form.validate();

        $('#RefreshButton').click(function () {
            refreshCategoryList();
        });

        $('.delete-category').click(function () {
            var categoryId = $(this).attr("data-category-id");
            var categoryTitle = $(this).attr('data-category-title');

            deleteCategory(categoryId, categoryTitle);
        });

        $('.edit-category').click(function (e) {
            var categoryId = $(this).attr("data-category-id");

            e.preventDefault();
            $.ajax({
                url: abp.appPath + 'Admin/Categories/EditCategoryModal?id=' + categoryId,
                type: 'POST',
                contentType: 'application/html',
                success: function (content) {
                    $('#CategoryEditModal div.modal-content').html(content);
                },
                error: function (e) { }
            });
        });

        _$form.find('button[type="submit"]').click(function (e) {
            e.preventDefault();

            if (!_$form.valid()) {
                return;
            }

            var category = _$form.serializeFormToObject(); //serializeFormToObject is defined in main.js

            abp.ui.setBusy(_$modal);
            _categoryService.create(category).done(function () {
                _$modal.modal('hide');
                location.reload(true); //reload page to see new category!
            }).always(function () {
                abp.ui.clearBusy(_$modal);
            });
        });

        _$modal.on('shown.bs.modal', function () {
            _$modal.find('input:not([type=hidden]):first').focus();
        });

        function refreshCategoryList() {
            location.reload(true); //reload page to see new category!
        }

        function deleteCategory(categoryId, categoryTitle) {
            abp.message.confirm(
                abp.utils.formatString(abp.localization.localize('AreYouSureWantToDelete', 'Photography'), categoryTitle),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _categoryService.delete({
                            id: categoryId
                        }).done(function () {
                            refreshCategoryList();
                        });
                    }
                }
            );
        }
    });
})();