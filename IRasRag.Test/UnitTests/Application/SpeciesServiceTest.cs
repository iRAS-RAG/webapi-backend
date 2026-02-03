using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Mappings;
using IRasRag.Application.Common.Models;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Implementations;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using IRasRag.Test.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace IRasRag.Test.UnitTests.Application
{
    public class SpeciesServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<SpeciesService>> _loggerMock;
        private readonly IMapper _mapper;
        private readonly Mock<IRepository<Species>> _repositoryMock;
        private readonly SpeciesService _sut;

        public SpeciesServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<SpeciesService>>();
            _mapper = AutoMapperTestHelper.GetMapper(new SpeciesProfile());
            _repositoryMock = new Mock<IRepository<Species>>();
            _unitOfWorkMock.Setup(x => x.GetRepository<Species>()).Returns(_repositoryMock.Object);

            _sut = new SpeciesService(_unitOfWorkMock.Object, _loggerMock.Object, _mapper);
        }

        #region CreateSpeciesAsync Tests

        [Fact]
        public async Task CreateSpeciesAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var createDto = new CreateSpeciesDto { Name = "Cá Rô Phi" };

            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Species, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((Species?)null);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateSpeciesAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Tạo loài thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be(createDto.Name);

            _repositoryMock.Verify(
                r => r.AddAsync(It.Is<Species>(s => s.Name == createDto.Name)),
                Times.Once
            );

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateSpeciesAsync_ShouldTrimInputs_WhenCreating()
        {
            // Arrange
            var createDto = new CreateSpeciesDto { Name = "  Cá Rô Phi  " };

            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Species, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((Species?)null);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateSpeciesAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data!.Name.Should().Be("Cá Rô Phi");

            _repositoryMock.Verify(
                r => r.AddAsync(It.Is<Species>(s => s.Name == "Cá Rô Phi")),
                Times.Once
            );
        }

        [Theory]
        [InlineData("", "Tên loài không được để trống.")]
        [InlineData(" ", "Tên loài không được để trống.")]
        [InlineData("  ", "Tên loài không được để trống.")]
        [InlineData("\t", "Tên loài không được để trống.")]
        [InlineData("\n", "Tên loài không được để trống.")]
        public async Task CreateSpeciesAsync_ShouldReturnBadRequest_WhenNameIsInvalid(
            string name,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateSpeciesDto { Name = name };
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateSpeciesAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Species>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task CreateSpeciesAsync_ShouldReturnConflict_WhenDuplicateNameExists()
        {
            // Arrange
            var createDto = new CreateSpeciesDto { Name = "Cá Rô Phi" };
            var existingSpecies = new Species { Id = Guid.NewGuid(), Name = "Cá Rô Phi" };

            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Species, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(existingSpecies);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateSpeciesAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Conflict);
            result.Message.Should().Be("Loài với tên này đã tồn tại.");

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Species>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task CreateSpeciesAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var createDto = new CreateSpeciesDto { Name = "Cá Rô Phi" };

            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Species, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ThrowsAsync(new Exception());
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateSpeciesAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi tạo loài.");
        }

        #endregion

        #region GetSpeciesByIdAsync Tests

        [Fact]
        public async Task GetSpeciesByIdAsync_ShouldReturnOk_WhenSpeciesExists()
        {
            // Arrange
            var speciesId = Guid.NewGuid();
            var species = new Species { Id = speciesId, Name = "Cá Rô Phi" };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(species);

            // Act
            var result = await _sut.GetSpeciesByIdAsync(speciesId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Lấy thông tin loài thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(speciesId);
            result.Data.Name.Should().Be(species.Name);

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == speciesId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        [Fact]
        public async Task GetSpeciesByIdAsync_ShouldReturnNotFound_WhenSpeciesNotExists()
        {
            // Arrange
            var speciesId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((Species?)null);

            // Act
            var result = await _sut.GetSpeciesByIdAsync(speciesId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Loài không tồn tại.");
            result.Data.Should().BeNull();

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == speciesId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        [Fact]
        public async Task GetSpeciesByIdAsync_ShouldReturnUnexpected_WhenThrownException()
        {
            // Arrange
            var speciesId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetSpeciesByIdAsync(speciesId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi truy xuất thông tin loài.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == speciesId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        #endregion

        #region GetAllSpeciesAsync Tests

        [Fact]
        public async Task GetAllSpeciesAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var speciesList = new List<Species>
            {
                new Species { Id = Guid.NewGuid(), Name = "Cá Rô Phi" },
                new Species { Id = Guid.NewGuid(), Name = "Cá Chép" },
            };

            _repositoryMock
                .Setup(r => r.GetAllAsync(QueryType.ActiveOnly))
                .ReturnsAsync(speciesList);

            // Act
            var result = await _sut.GetAllSpeciesAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Lấy danh sách loài thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Count().Should().Be(speciesList.Count);
            _repositoryMock.Verify(r => r.GetAllAsync(QueryType.ActiveOnly), Times.Once);
        }

        [Fact]
        public async Task GetAllSpeciesAsync_ShouldReturnEmptyList_WhenNoRecordsExist()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllAsync(QueryType.ActiveOnly))
                .ReturnsAsync(new List<Species>());

            // Act
            var result = await _sut.GetAllSpeciesAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Lấy danh sách loài thành công.");
            result.Data.Should().BeEmpty();
            _repositoryMock.Verify(r => r.GetAllAsync(QueryType.ActiveOnly), Times.Once);
        }

        [Fact]
        public async Task GetAllSpeciesAsync_ShouldReturnUnexpected_WhenThrownException()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllAsync(QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetAllSpeciesAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi truy xuất danh sách loài.");
            _repositoryMock.Verify(r => r.GetAllAsync(QueryType.ActiveOnly), Times.Once);
        }

        #endregion

        #region DeleteSpeciesAsync Tests

        [Fact]
        public async Task DeleteSpeciesAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var speciesId = Guid.NewGuid();
            var species = new Species { Id = speciesId, Name = "Cá Rô Phi" };
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(species);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.DeleteSpeciesAsync(speciesId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Xóa loài thành công.");
            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == speciesId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(
                r => r.Delete(It.Is<Species>(s => s.Id == speciesId)),
                Times.Once
            );
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteSpeciesAsync_ShouldReturnNotFound_WhenSpeciesNotExists()
        {
            // Arrange
            var speciesId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((Species?)null);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.DeleteSpeciesAsync(speciesId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Loài không tồn tại.");
            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == speciesId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(r => r.Delete(It.IsAny<Species>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task DeleteSpeciesAsync_ShouldReturnUnexpected_WhenThrownException()
        {
            // Arrange
            var speciesId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.DeleteSpeciesAsync(speciesId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi xóa loài.");
            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == speciesId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(r => r.Delete(It.IsAny<Species>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        #endregion

        #region UpdateSpeciesAsync Tests

        [Fact]
        public async Task UpdateSpeciesAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var speciesId = Guid.NewGuid();
            var existingSpecies = new Species { Id = speciesId, Name = "Cá Rô Phi" };
            var updateDto = new UpdateSpeciesDto { Name = "Cá Rô Phi Đỏ" };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingSpecies);
            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Species, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((Species?)null);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateSpeciesAsync(speciesId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Cập nhật loài thành công.");

            existingSpecies.Name.Should().Be("Cá Rô Phi Đỏ");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == speciesId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(r => r.Update(It.IsAny<Species>()), Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Theory]
        [InlineData(null, "Cá Rô Phi")]
        [InlineData("", "Cá Rô Phi")]
        [InlineData(" ", "Cá Rô Phi")]
        [InlineData("  ", "Cá Rô Phi")]
        public async Task UpdateSpeciesAsync_ShouldNotUpdateName_WhenNameIsNullOrWhitespace(
            string nameInput,
            string expectedName
        )
        {
            // Arrange
            var speciesId = Guid.NewGuid();
            var existingSpecies = new Species { Id = speciesId, Name = "Cá Rô Phi" };
            var updateDto = new UpdateSpeciesDto { Name = nameInput };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingSpecies);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateSpeciesAsync(speciesId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Cập nhật loài thành công.");
            existingSpecies.Name.Should().Be(expectedName);

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == speciesId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(r => r.Update(It.IsAny<Species>()), Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateSpeciesAsync_ShouldTrimInputs_WhenUpdating()
        {
            // Arrange
            var speciesId = Guid.NewGuid();
            var existingSpecies = new Species { Id = speciesId, Name = "Cá Rô Phi" };
            var updateDto = new UpdateSpeciesDto { Name = "  Cá Rô Phi Đỏ  " };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingSpecies);
            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Species, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((Species?)null);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateSpeciesAsync(speciesId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingSpecies.Name.Should().Be("Cá Rô Phi Đỏ");
        }

        [Fact]
        public async Task UpdateSpeciesAsync_ShouldReturnNotFound_WhenSpeciesNotExists()
        {
            // Arrange
            var speciesId = Guid.NewGuid();
            var updateDto = new UpdateSpeciesDto { Name = "Cá Rô Phi Đỏ" };

            _repositoryMock
                .Setup(r =>
                    r.GetByIdAsync(It.Is<Guid>(id => id == speciesId), QueryType.ActiveOnly)
                )
                .ReturnsAsync((Species?)null);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateSpeciesAsync(speciesId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Loài không tồn tại.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == speciesId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(r => r.Update(It.IsAny<Species>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateSpeciesAsync_ShouldReturnConflict_WhenDuplicateNameExists()
        {
            // Arrange
            var speciesId = Guid.NewGuid();
            var existingSpecies = new Species { Id = speciesId, Name = "Cá Rô Phi" };
            var duplicateSpecies = new Species { Id = Guid.NewGuid(), Name = "Cá Chép" };
            var updateDto = new UpdateSpeciesDto { Name = "Cá Chép" };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingSpecies);
            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Species, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(duplicateSpecies);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateSpeciesAsync(speciesId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Conflict);
            result.Message.Should().Be("Loài với tên này đã tồn tại.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == speciesId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(r => r.Update(It.IsAny<Species>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateSpeciesAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var speciesId = Guid.NewGuid();
            var updateDto = new UpdateSpeciesDto { Name = "Cá Rô Phi Đỏ" };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateSpeciesAsync(speciesId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi cập nhật loài.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == speciesId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(r => r.Update(It.IsAny<Species>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        #endregion
    }
}
