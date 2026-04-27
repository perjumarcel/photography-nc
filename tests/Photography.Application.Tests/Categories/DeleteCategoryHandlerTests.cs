using Moq;
using Photography.Application.Categories.Commands;
using Photography.Core.Albums;
using Photography.Core.Categories;
using Photography.SharedKernel;
using Xunit;

namespace Photography.Application.Tests.Categories;

public class DeleteCategoryHandlerTests
{
    [Fact]
    public async Task Returns_NotFound_When_Missing()
    {
        var categories = new Mock<ICategoryRepository>();
        var albums = new Mock<IAlbumQueryRepository>();
        categories.Setup(c => c.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Category?)null);

        var handler = new DeleteCategoryHandler(categories.Object, albums.Object);
        var result = await handler.Handle(new DeleteCategoryCommand(1), CancellationToken.None);

        Assert.Equal(ResultErrorKind.NotFound, result.ErrorKind);
    }

    [Fact]
    public async Task Returns_Conflict_When_In_Use()
    {
        var categories = new Mock<ICategoryRepository>();
        var albums = new Mock<IAlbumQueryRepository>();
        categories.Setup(c => c.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(Category.Create("Wedding", "wedding"));
        albums.Setup(a => a.AnyInCategoryAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new DeleteCategoryHandler(categories.Object, albums.Object);
        var result = await handler.Handle(new DeleteCategoryCommand(1), CancellationToken.None);

        Assert.Equal(ResultErrorKind.Conflict, result.ErrorKind);
        categories.Verify(c => c.Remove(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task Removes_When_Unused()
    {
        var categories = new Mock<ICategoryRepository>();
        var albums = new Mock<IAlbumQueryRepository>();
        var category = Category.Create("Wedding", "wedding");
        categories.Setup(c => c.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(category);
        albums.Setup(a => a.AnyInCategoryAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new DeleteCategoryHandler(categories.Object, albums.Object);
        var result = await handler.Handle(new DeleteCategoryCommand(1), CancellationToken.None);

        Assert.True(result.Success);
        categories.Verify(c => c.Remove(category), Times.Once);
        categories.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
