using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Mappings;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Implementations;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using IRasRag.Test.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace IRasRag.Test.UnitTests.Application
{
    public class FeedTypeServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<FeedTypeService>> _loggerMock;
        private readonly IMapper _mapper;
        private readonly Mock<IRepository<FeedType>> _repositoryMock;
        private readonly FeedTypeService _sut;

        public FeedTypeServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<FeedTypeService>>();
            _mapper = AutoMapperTestHelper.GetMapper(new FeedTypeProfile());
            _repositoryMock = new Mock<IRepository<FeedType>>();
            _unitOfWorkMock.Setup(x => x.GetRepository<FeedType>()).Returns(_repositoryMock.Object);

            _sut = new FeedTypeService(_unitOfWorkMock.Object, _loggerMock.Object, _mapper);
        }

        #region CreateFeedTypeAsync Tests

        [Fact]
        public async Task CreateFeedTypeAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var createDto = new CreateFeedTypeDto
            {
                Name = "Thức ăn viên",
                Description = "Thức ăn viên cho cá",
                WeightPerUnit = 25.5f,
                ProteinPercentage = 35.0f,
                Manufacturer = "Nhà máy ABC",
            };

            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<FeedType, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((FeedType?)null);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFeedTypeAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Tạo loại thức ăn thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be(createDto.Name);
            result.Data.Description.Should().Be(createDto.Description);
            result.Data.WeightPerUnit.Should().Be(createDto.WeightPerUnit);
            result.Data.ProteinPercentage.Should().Be(createDto.ProteinPercentage);
            result.Data.Manufacturer.Should().Be(createDto.Manufacturer);

            _repositoryMock.Verify(
                r =>
                    r.AddAsync(
                        It.Is<FeedType>(f =>
                            f.Name == createDto.Name
                            && f.Description == createDto.Description
                            && f.WeightPerUnit == createDto.WeightPerUnit
                            && f.ProteinPercentage == createDto.ProteinPercentage
                            && f.Manufacturer == createDto.Manufacturer
                        )
                    ),
                Times.Once
            );

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateFeedTypeAsync_ShouldTrimInputs_WhenCreating()
        {
            // Arrange
            var createDto = new CreateFeedTypeDto
            {
                Name = "  Thức ăn viên  ",
                Description = "  Thức ăn viên cho cá  ",
                WeightPerUnit = 25.5f,
                ProteinPercentage = 35.0f,
                Manufacturer = "  Nhà máy ABC  ",
            };

            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<FeedType, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((FeedType?)null);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFeedTypeAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data!.Name.Should().Be("Thức ăn viên");
            result.Data.Description.Should().Be("Thức ăn viên cho cá");
            result.Data.Manufacturer.Should().Be("Nhà máy ABC");

            _repositoryMock.Verify(
                r =>
                    r.AddAsync(
                        It.Is<FeedType>(f =>
                            f.Name == "Thức ăn viên"
                            && f.Description == "Thức ăn viên cho cá"
                            && f.Manufacturer == "Nhà máy ABC"
                        )
                    ),
                Times.Once
            );
        }

        [Theory]
        [InlineData("", "Tên loại thức ăn không được để trống.")]
        [InlineData(" ", "Tên loại thức ăn không được để trống.")]
        [InlineData("  ", "Tên loại thức ăn không được để trống.")]
        [InlineData("\t", "Tên loại thức ăn không được để trống.")]
        [InlineData("\n", "Tên loại thức ăn không được để trống.")]
        public async Task CreateFeedTypeAsync_ShouldReturnBadRequest_WhenNameIsInvalid(
            string name,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateFeedTypeDto
            {
                Name = name,
                WeightPerUnit = 25.5f,
                ProteinPercentage = 35.0f,
            };
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFeedTypeAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<FeedType>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Theory]
        [InlineData(0, "Trọng lượng mỗi đơn vị phải lớn hơn 0.")]
        [InlineData(-1, "Trọng lượng mỗi đơn vị phải lớn hơn 0.")]
        [InlineData(-25.5f, "Trọng lượng mỗi đơn vị phải lớn hơn 0.")]
        public async Task CreateFeedTypeAsync_ShouldReturnBadRequest_WhenWeightPerUnitIsInvalid(
            float weightPerUnit,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateFeedTypeDto
            {
                Name = "Thức ăn viên",
                WeightPerUnit = weightPerUnit,
                ProteinPercentage = 35.0f,
            };
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFeedTypeAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<FeedType>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Theory]
        [InlineData(-1, "Tỷ lệ protein phải từ 0 đến 100.")]
        [InlineData(101, "Tỷ lệ protein phải từ 0 đến 100.")]
        [InlineData(150, "Tỷ lệ protein phải từ 0 đến 100.")]
        public async Task CreateFeedTypeAsync_ShouldReturnBadRequest_WhenProteinPercentageIsInvalid(
            float proteinPercentage,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateFeedTypeDto
            {
                Name = "Thức ăn viên",
                WeightPerUnit = 25.5f,
                ProteinPercentage = proteinPercentage,
            };
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFeedTypeAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<FeedType>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task CreateFeedTypeAsync_ShouldReturnConflict_WhenDuplicateNameExists()
        {
            // Arrange
            var createDto = new CreateFeedTypeDto
            {
                Name = "Thức ăn viên",
                WeightPerUnit = 25.5f,
                ProteinPercentage = 35.0f,
            };
            var existingFeedType = new FeedType
            {
                Id = Guid.NewGuid(),
                Name = "Thức ăn viên",
                WeightPerUnit = 30.0f,
                ProteinPercentage = 40.0f,
            };

            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<FeedType, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(existingFeedType);
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFeedTypeAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Conflict);
            result.Message.Should().Be("Loại thức ăn với tên này đã tồn tại.");

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<FeedType>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task CreateFeedTypeAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var createDto = new CreateFeedTypeDto
            {
                Name = "Thức ăn viên",
                WeightPerUnit = 25.5f,
                ProteinPercentage = 35.0f,
            };

            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<FeedType, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ThrowsAsync(new Exception());
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateFeedTypeAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi tạo loại thức ăn.");
        }

        #endregion

        #region GetFeedTypeByIdAsync Tests

        [Fact]
        public async Task GetFeedTypeByIdAsync_ShouldReturnOk_WhenFeedTypeExists()
        {
            // Arrange
            var feedTypeId = Guid.NewGuid();
            var feedType = new FeedType
            {
                Id = feedTypeId,
                Name = "Thức ăn viên",
                Description = "Thức ăn viên cho cá",
                WeightPerUnit = 25.5f,
                ProteinPercentage = 35.0f,
                Manufacturer = "Nhà máy ABC",
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(feedType);

            // Act
            var result = await _sut.GetFeedTypeByIdAsync(feedTypeId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Lấy thông tin loại thức ăn thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(feedTypeId);
            result.Data.Name.Should().Be(feedType.Name);
            result.Data.Description.Should().Be(feedType.Description);
            result.Data.WeightPerUnit.Should().Be(feedType.WeightPerUnit);
            result.Data.ProteinPercentage.Should().Be(feedType.ProteinPercentage);
            result.Data.Manufacturer.Should().Be(feedType.Manufacturer);

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == feedTypeId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        [Fact]
        public async Task GetFeedTypeByIdAsync_ShouldReturnNotFound_WhenFeedTypeNotExists()
        {
            // Arrange
            var feedTypeId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((FeedType?)null);

            // Act
            var result = await _sut.GetFeedTypeByIdAsync(feedTypeId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Loại thức ăn không tồn tại.");
            result.Data.Should().BeNull();

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == feedTypeId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        [Fact]
        public async Task GetFeedTypeByIdAsync_ShouldReturnUnexpected_WhenThrownException()
        {
            // Arrange
            var feedTypeId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetFeedTypeByIdAsync(feedTypeId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi truy xuất thông tin loại thức ăn.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == feedTypeId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        #endregion

        #region GetAllFeedTypesAsync Tests

        [Fact]
        public async Task GetAllFeedTypesAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;

            var feedTypeList = new List<FeedType>
            {
                new FeedType
                {
                    Id = Guid.NewGuid(),
                    Name = "Thức ăn viên",
                    WeightPerUnit = 25.5f,
                    ProteinPercentage = 35.0f,
                },
                new FeedType
                {
                    Id = Guid.NewGuid(),
                    Name = "Thức ăn bột",
                    WeightPerUnit = 10.0f,
                    ProteinPercentage = 40.0f,
                },
            };

            _repositoryMock
                .Setup(r => r.GetPagedAsync(page, pageSize, QueryType.ActiveOnly))
                .ReturnsAsync(
                    new PagedResult<FeedType>
                    {
                        Items = feedTypeList,
                        TotalItems = feedTypeList.Count,
                    }
                );

            // Act
            var result = await _sut.GetAllFeedTypesAsync(page, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("Lấy danh sách loại thức ăn thành công.");
            result.Data.Should().NotBeNull();
            result.Data!.Count.Should().Be(2);

            result.Meta.Should().NotBeNull();
            result.Meta!.Page.Should().Be(page);
            result.Meta.PageSize.Should().Be(pageSize);
            result.Meta.TotalItems.Should().Be(2);

            result.Links.Should().NotBeNull();

            _repositoryMock.Verify(
                r => r.GetPagedAsync(page, pageSize, QueryType.ActiveOnly),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllFeedTypesAsync_ShouldReturnEmptyList_WhenNoRecordsExist()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;

            _repositoryMock
                .Setup(r => r.GetPagedAsync(page, pageSize, QueryType.ActiveOnly))
                .ReturnsAsync(
                    new PagedResult<FeedType> { Items = Array.Empty<FeedType>(), TotalItems = 0 }
                );

            // Act
            var result = await _sut.GetAllFeedTypesAsync(page, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("Không có loại thức ăn nào");
            result.Data.Should().NotBeNull();
            result.Data!.Should().BeEmpty();

            result.Meta.Should().NotBeNull();
            result.Meta!.TotalItems.Should().Be(0);

            result.Links.Should().NotBeNull();

            _repositoryMock.Verify(
                r => r.GetPagedAsync(page, pageSize, QueryType.ActiveOnly),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllFeedTypesAsync_ShouldReturnErrorMessage_WhenThrownException()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;

            _repositoryMock
                .Setup(r => r.GetPagedAsync(page, pageSize, QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetAllFeedTypesAsync(page, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("Lỗi khi truy xuất danh sách loại thức ăn.");

            result.Data.Should().NotBeNull();
            result.Data!.Should().BeEmpty();

            result.Meta.Should().BeNull();
            result.Links.Should().BeNull();

            _repositoryMock.Verify(
                r => r.GetPagedAsync(page, pageSize, QueryType.ActiveOnly),
                Times.Once
            );
        }

        #endregion

        #region DeleteFeedTypeAsync Tests

        [Fact]
        public async Task DeleteFeedTypeAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var feedTypeId = Guid.NewGuid();
            var feedType = new FeedType
            {
                Id = feedTypeId,
                Name = "Thức ăn viên",
                WeightPerUnit = 25.5f,
                ProteinPercentage = 35.0f,
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(feedType);
            _repositoryMock.Setup(r => r.Delete(It.IsAny<FeedType>())).Verifiable();
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.DeleteFeedTypeAsync(feedTypeId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Xóa loại thức ăn thành công.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == feedTypeId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(
                r => r.Delete(It.Is<FeedType>(f => f.Id == feedTypeId)),
                Times.Once
            );
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteFeedTypeAsync_ShouldReturnNotFound_WhenFeedTypeNotExists()
        {
            // Arrange
            var feedTypeId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((FeedType?)null);

            // Act
            var result = await _sut.DeleteFeedTypeAsync(feedTypeId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Loại thức ăn không tồn tại.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == feedTypeId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(r => r.Delete(It.IsAny<FeedType>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task DeleteFeedTypeAsync_ShouldReturnUnexpected_WhenThrownException()
        {
            // Arrange
            var feedTypeId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.DeleteFeedTypeAsync(feedTypeId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi xóa loại thức ăn.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == feedTypeId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        #endregion

        #region UpdateFeedTypeAsync Tests

        [Fact]
        public async Task UpdateFeedTypeAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var feedTypeId = Guid.NewGuid();
            var existingFeedType = new FeedType
            {
                Id = feedTypeId,
                Name = "Thức ăn viên",
                Description = "Mô tả cũ",
                WeightPerUnit = 25.5f,
                ProteinPercentage = 35.0f,
                Manufacturer = "Nhà máy ABC",
            };
            var updateDto = new UpdateFeedTypeDto
            {
                Name = "Thức ăn viên mới",
                Description = "Mô tả mới",
                WeightPerUnit = 30.0f,
                ProteinPercentage = 40.0f,
                Manufacturer = "Nhà máy XYZ",
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFeedType);
            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<FeedType, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((FeedType?)null);
            _repositoryMock.Setup(r => r.Update(It.IsAny<FeedType>())).Verifiable();
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateFeedTypeAsync(feedTypeId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Type.Should().Be(ResultType.Ok);
            result.Message.Should().Be("Cập nhật loại thức ăn thành công.");

            existingFeedType.Name.Should().Be(updateDto.Name);
            existingFeedType.Description.Should().Be(updateDto.Description);
            existingFeedType.WeightPerUnit.Should().Be(updateDto.WeightPerUnit!.Value);
            existingFeedType.ProteinPercentage.Should().Be(updateDto.ProteinPercentage!.Value);
            existingFeedType.Manufacturer.Should().Be(updateDto.Manufacturer);

            _repositoryMock.Verify(
                r => r.Update(It.Is<FeedType>(f => f.Id == feedTypeId)),
                Times.Once
            );
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Theory]
        [InlineData(null, "Thức ăn viên")]
        [InlineData("", "Thức ăn viên")]
        [InlineData(" ", "Thức ăn viên")]
        [InlineData("  ", "Thức ăn viên")]
        public async Task UpdateFeedTypeAsync_ShouldNotUpdateName_WhenNameIsNullOrWhitespace(
            string nameInput,
            string expectedName
        )
        {
            // Arrange
            var feedTypeId = Guid.NewGuid();
            var existingFeedType = new FeedType
            {
                Id = feedTypeId,
                Name = "Thức ăn viên",
                WeightPerUnit = 25.5f,
                ProteinPercentage = 35.0f,
            };
            var updateDto = new UpdateFeedTypeDto { Name = nameInput };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFeedType);
            _repositoryMock.Setup(r => r.Update(It.IsAny<FeedType>())).Verifiable();
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateFeedTypeAsync(feedTypeId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingFeedType.Name.Should().Be(expectedName);
        }

        [Fact]
        public async Task UpdateFeedTypeAsync_ShouldTrimInputs_WhenUpdating()
        {
            // Arrange
            var feedTypeId = Guid.NewGuid();
            var existingFeedType = new FeedType
            {
                Id = feedTypeId,
                Name = "Thức ăn viên",
                WeightPerUnit = 25.5f,
                ProteinPercentage = 35.0f,
            };
            var updateDto = new UpdateFeedTypeDto
            {
                Name = "  Thức ăn viên mới  ",
                Description = "  Mô tả mới  ",
                Manufacturer = "  Nhà máy XYZ  ",
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFeedType);
            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<FeedType, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((FeedType?)null);
            _repositoryMock.Setup(r => r.Update(It.IsAny<FeedType>())).Verifiable();
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateFeedTypeAsync(feedTypeId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingFeedType.Name.Should().Be("Thức ăn viên mới");
            existingFeedType.Description.Should().Be("Mô tả mới");
            existingFeedType.Manufacturer.Should().Be("Nhà máy XYZ");
        }

        [Fact]
        public async Task UpdateFeedTypeAsync_ShouldReturnNotFound_WhenFeedTypeNotExists()
        {
            // Arrange
            var feedTypeId = Guid.NewGuid();
            var updateDto = new UpdateFeedTypeDto { Name = "Thức ăn viên mới" };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync((FeedType?)null);

            // Act
            var result = await _sut.UpdateFeedTypeAsync(feedTypeId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            result.Message.Should().Be("Loại thức ăn không tồn tại.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == feedTypeId), QueryType.ActiveOnly),
                Times.Once
            );
            _repositoryMock.Verify(r => r.Update(It.IsAny<FeedType>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateFeedTypeAsync_ShouldReturnConflict_WhenDuplicateNameExists()
        {
            // Arrange
            var feedTypeId = Guid.NewGuid();
            var existingFeedType = new FeedType
            {
                Id = feedTypeId,
                Name = "Thức ăn viên",
                WeightPerUnit = 25.5f,
                ProteinPercentage = 35.0f,
            };
            var anotherFeedType = new FeedType
            {
                Id = Guid.NewGuid(),
                Name = "Thức ăn bột",
                WeightPerUnit = 10.0f,
                ProteinPercentage = 40.0f,
            };
            var updateDto = new UpdateFeedTypeDto { Name = "Thức ăn bột" };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFeedType);
            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<FeedType, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(anotherFeedType);

            // Act
            var result = await _sut.UpdateFeedTypeAsync(feedTypeId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Conflict);
            result.Message.Should().Be("Loại thức ăn với tên này đã tồn tại.");

            _repositoryMock.Verify(r => r.Update(It.IsAny<FeedType>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Theory]
        [InlineData(0, "Trọng lượng mỗi đơn vị phải lớn hơn 0.")]
        [InlineData(-1, "Trọng lượng mỗi đơn vị phải lớn hơn 0.")]
        [InlineData(-25.5f, "Trọng lượng mỗi đơn vị phải lớn hơn 0.")]
        public async Task UpdateFeedTypeAsync_ShouldReturnBadRequest_WhenWeightPerUnitIsInvalid(
            float weightPerUnit,
            string expectedMessage
        )
        {
            // Arrange
            var feedTypeId = Guid.NewGuid();
            var existingFeedType = new FeedType
            {
                Id = feedTypeId,
                Name = "Thức ăn viên",
                WeightPerUnit = 25.5f,
                ProteinPercentage = 35.0f,
            };
            var updateDto = new UpdateFeedTypeDto { WeightPerUnit = weightPerUnit };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFeedType);

            // Act
            var result = await _sut.UpdateFeedTypeAsync(feedTypeId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _repositoryMock.Verify(r => r.Update(It.IsAny<FeedType>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Theory]
        [InlineData(-1, "Tỷ lệ protein phải từ 0 đến 100.")]
        [InlineData(101, "Tỷ lệ protein phải từ 0 đến 100.")]
        [InlineData(150, "Tỷ lệ protein phải từ 0 đến 100.")]
        public async Task UpdateFeedTypeAsync_ShouldReturnBadRequest_WhenProteinPercentageIsInvalid(
            float proteinPercentage,
            string expectedMessage
        )
        {
            // Arrange
            var feedTypeId = Guid.NewGuid();
            var existingFeedType = new FeedType
            {
                Id = feedTypeId,
                Name = "Thức ăn viên",
                WeightPerUnit = 25.5f,
                ProteinPercentage = 35.0f,
            };
            var updateDto = new UpdateFeedTypeDto { ProteinPercentage = proteinPercentage };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ReturnsAsync(existingFeedType);

            // Act
            var result = await _sut.UpdateFeedTypeAsync(feedTypeId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);

            _repositoryMock.Verify(r => r.Update(It.IsAny<FeedType>()), Times.Never);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateFeedTypeAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var feedTypeId = Guid.NewGuid();
            var updateDto = new UpdateFeedTypeDto { Name = "Thức ăn viên mới" };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.UpdateFeedTypeAsync(feedTypeId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
            result.Message.Should().Be("Lỗi khi cập nhật loại thức ăn.");

            _repositoryMock.Verify(
                r => r.GetByIdAsync(It.Is<Guid>(id => id == feedTypeId), QueryType.ActiveOnly),
                Times.Once
            );
        }

        #endregion
    }
}
