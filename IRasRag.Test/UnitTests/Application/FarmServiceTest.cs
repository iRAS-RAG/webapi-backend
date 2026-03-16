using System.Linq.Expressions;
using Ardalis.Specification;
using AutoMapper;
using FluentAssertions;
using IRasRag.Application.Common.Interfaces.Persistence;
using IRasRag.Application.Common.Interfaces.Persistence.Repositories;
using IRasRag.Application.Common.Mappings;
using IRasRag.Application.Common.Models;
using IRasRag.Application.Common.Models.Pagination;
using IRasRag.Application.DTOs;
using IRasRag.Application.Services.Implementations;
using IRasRag.Application.Specifications;
using IRasRag.Application.Specifications.FarmSpecifications;
using IRasRag.Domain.Entities;
using IRasRag.Domain.Enums;
using IRasRag.Test.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace IRasRag.Test.UnitTests.Application
{
    public class FarmServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<FarmService>> _loggerMock;
        private readonly IMapper _mapper;
        private readonly Mock<IRepository<Farm>> _repositoryMock;
        private readonly FarmService _sut;

        public FarmServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<FarmService>>();
            _mapper = AutoMapperTestHelper.GetMapper(new FarmProfile());
            _repositoryMock = new Mock<IRepository<Farm>>();
            _unitOfWorkMock.Setup(x => x.GetRepository<Farm>()).Returns(_repositoryMock.Object);

            _sut = new FarmService(_unitOfWorkMock.Object, _loggerMock.Object, _mapper);
        }

        #region CreateFarmAsync Tests

        [Fact]
        public async Task CreateFarmAsync_ShouldReturnSuccess_WhenValidInput()
        {
            // Arrange
            var createDto = new CreateFarmDto
            {
                Name = "Trang trại ABC",
                Address = "123 Đường ABC",
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
            };

            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Farm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((Farm?)null);

            // Act
            var result = await _sut.CreateFarmAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be(createDto.Name);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Farm>()), Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateFarmAsync_ShouldTrimInputs()
        {
            // Arrange
            var createDto = new CreateFarmDto
            {
                Name = "  Trang trại ABC  ",
                Address = "  123 Đường ABC  ",
                PhoneNumber = "  0123456789  ",
                Email = "  abc@farm.com  ",
            };

            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Farm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((Farm?)null);

            // Act
            var result = await _sut.CreateFarmAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(
                r =>
                    r.AddAsync(
                        It.Is<Farm>(f =>
                            f.Name == "Trang trại ABC"
                            && f.Address == "123 Đường ABC"
                            && f.PhoneNumber == "0123456789"
                            && f.Email == "abc@farm.com"
                        )
                    ),
                Times.Once
            );
        }

        [Theory]
        [InlineData(null, "Tên trang trại không được để trống.")]
        [InlineData("", "Tên trang trại không được để trống.")]
        [InlineData(" ", "Tên trang trại không được để trống.")]
        public async Task CreateFarmAsync_ShouldReturnBadRequest_WhenNameIsInvalid(
            string name,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateFarmDto
            {
                Name = name,
                Address = "123 Đường ABC",
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
            };

            // Act
            var result = await _sut.CreateFarmAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            result.Message.Should().Be(expectedMessage);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Farm>()), Times.Never);
        }

        [Theory]
        [InlineData(null, "Địa chỉ không được để trống.")]
        [InlineData("", "Địa chỉ không được để trống.")]
        [InlineData(" ", "Địa chỉ không được để trống.")]
        public async Task CreateFarmAsync_ShouldReturnBadRequest_WhenAddressIsInvalid(
            string address,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateFarmDto
            {
                Name = "Trang trại ABC",
                Address = address,
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
            };

            // Act
            var result = await _sut.CreateFarmAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Farm>()), Times.Never);
        }

        [Theory]
        [InlineData(null, "Số điện thoại không được để trống.")]
        [InlineData("", "Số điện thoại không được để trống.")]
        [InlineData(" ", "Số điện thoại không được để trống.")]
        public async Task CreateFarmAsync_ShouldReturnBadRequest_WhenPhoneNumberIsInvalid(
            string phoneNumber,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateFarmDto
            {
                Name = "Trang trại ABC",
                Address = "123 Đường ABC",
                PhoneNumber = phoneNumber,
                Email = "abc@farm.com",
            };

            // Act
            var result = await _sut.CreateFarmAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Farm>()), Times.Never);
        }

        [Theory]
        [InlineData(null, "Email không được để trống.")]
        [InlineData("", "Email không được để trống.")]
        [InlineData(" ", "Email không được để trống.")]
        public async Task CreateFarmAsync_ShouldReturnBadRequest_WhenEmailIsInvalid(
            string email,
            string expectedMessage
        )
        {
            // Arrange
            var createDto = new CreateFarmDto
            {
                Name = "Trang trại ABC",
                Address = "123 Đường ABC",
                PhoneNumber = "0123456789",
                Email = email,
            };

            // Act
            var result = await _sut.CreateFarmAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.BadRequest);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Farm>()), Times.Never);
        }

        [Fact]
        public async Task CreateFarmAsync_ShouldReturnConflict_WhenDuplicateEmailExists()
        {
            // Arrange
            var createDto = new CreateFarmDto
            {
                Name = "Trang trại ABC",
                Address = "123 Đường ABC",
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
            };
            var existingFarm = new Farm { Email = "abc@farm.com", IsDeleted = false };

            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Farm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(existingFarm);

            // Act
            var result = await _sut.CreateFarmAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Conflict);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Farm>()), Times.Never);
        }

        [Fact]
        public async Task CreateFarmAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var createDto = new CreateFarmDto
            {
                Name = "Trang trại ABC",
                Address = "123 Đường ABC",
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
            };

            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Farm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.CreateFarmAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
        }

        #endregion

        #region GetFarmByIdAsync Tests

        [Fact]
        public async Task GetFarmByIdAsync_ShouldReturnSuccess_WhenFarmExists()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var farm = new Farm
            {
                Id = farmId,
                Name = "Trang trại ABC",
                Address = "123 Đường ABC",
                PhoneNumber = "0123456789",
                Email = "abc@farm.com",
                IsDeleted = false,
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(farmId, QueryType.ActiveOnly))
                .ReturnsAsync(farm);

            // Act
            var result = await _sut.GetFarmByIdAsync(farmId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(farmId);
        }

        [Fact]
        public async Task GetFarmByIdAsync_ShouldReturnNotFound_WhenFarmDoesNotExist()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(farmId, QueryType.ActiveOnly))
                .ReturnsAsync((Farm?)null);

            // Act
            var result = await _sut.GetFarmByIdAsync(farmId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
        }

        [Fact]
        public async Task GetFarmByIdAsync_ShouldReturnNotFound_WhenFarmIsDeleted()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var deletedFarm = new Farm { Id = farmId, IsDeleted = true };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(farmId, QueryType.ActiveOnly))
                .ReturnsAsync(deletedFarm);

            // Act
            var result = await _sut.GetFarmByIdAsync(farmId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
        }

        [Fact]
        public async Task GetFarmByIdAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(farmId, QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetFarmByIdAsync(farmId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
        }

        #endregion

        #region GetAllFarmsAsync Tests

        [Fact]
        public async Task GetAllFarmsAsync_ShouldApplySearchAndSort_FromSpecification()
        {
            // Arrange
            var request = new FarmListRequest
            {
                Page = 1,
                PageSize = 10,
                SearchTerm = "farm",
                SortBy = "email",
                SortDir = "desc",
            };
            var farms = new List<Farm>
            {
                new Farm
                {
                    Id = Guid.NewGuid(),
                    Name = "Farm 1",
                    Address = "Address 1",
                    PhoneNumber = "0123",
                    Email = "farm1@test.com",
                },
                new Farm
                {
                    Id = Guid.NewGuid(),
                    Name = "Farm 2",
                    Address = "Address 2",
                    PhoneNumber = "0456",
                    Email = "farm2@test.com",
                },
                new Farm
                {
                    Id = Guid.NewGuid(),
                    Name = "Aquaculture",
                    Address = "Address 3",
                    PhoneNumber = "0789",
                    Email = "aqua@test.com",
                },
            };

            ISpecification<Farm, FarmDto>? capturedSpec = null;

            _repositoryMock
                .Setup(r =>
                    r.GetPagedAsync(
                        It.IsAny<ISpecification<Farm, FarmDto>>(),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    )
                )
                .Callback(
                    (ISpecification<Farm, FarmDto> spec, int _, int _, QueryType _) =>
                        capturedSpec = spec
                )
                .ReturnsAsync(
                    (ISpecification<Farm, FarmDto> spec, int page, int pageSize, QueryType _) =>
                        SpecificationTestHelper.ApplySpecificationWithPaging(
                            farms,
                            spec,
                            page,
                            pageSize
                        )
                );

            // Act
            var result = await _sut.GetAllFarmsAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result
                .Data.Select(x => x.Email)
                .Should()
                .ContainInOrder("farm2@test.com", "farm1@test.com");
            result.Meta.Should().NotBeNull();
            result.Meta!.TotalItems.Should().Be(2);
            capturedSpec.Should().NotBeNull();
            capturedSpec.Should().BeOfType<FarmListDtoSpec>();

            _repositoryMock.Verify(
                r =>
                    r.GetPagedAsync(
                        It.Is<ISpecification<Farm, FarmDto>>(s => s is FarmListDtoSpec),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllFarmsAsync_ShouldApplyDefaultSortByName_WhenSortByIsNull()
        {
            // Arrange
            var request = new FarmListRequest { Page = 1, PageSize = 10 };
            var farms = new List<Farm>
            {
                new Farm
                {
                    Id = Guid.NewGuid(),
                    Name = "Zulu",
                    Address = "A1",
                    PhoneNumber = "1",
                    Email = "z@test.com",
                },
                new Farm
                {
                    Id = Guid.NewGuid(),
                    Name = "Alpha",
                    Address = "A2",
                    PhoneNumber = "2",
                    Email = "a@test.com",
                },
                new Farm
                {
                    Id = Guid.NewGuid(),
                    Name = "Beta",
                    Address = "A3",
                    PhoneNumber = "3",
                    Email = "b@test.com",
                },
            };

            _repositoryMock
                .Setup(r =>
                    r.GetPagedAsync(
                        It.IsAny<ISpecification<Farm, FarmDto>>(),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    (ISpecification<Farm, FarmDto> spec, int page, int pageSize, QueryType _) =>
                        SpecificationTestHelper.ApplySpecificationWithPaging(
                            farms,
                            spec,
                            page,
                            pageSize
                        )
                );

            // Act
            var result = await _sut.GetAllFarmsAsync(request);

            // Assert
            result.Data.Should().NotBeNull();
            result.Data!.Select(x => x.Name).Should().ContainInOrder("Alpha", "Beta", "Zulu");

            _repositoryMock.Verify(
                r =>
                    r.GetPagedAsync(
                        It.Is<ISpecification<Farm, FarmDto>>(s => s is FarmListDtoSpec),
                        request.Page,
                        request.PageSize,
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllFarmsAsync_ShouldReturnEmptyList_WhenNoFarmsExist()
        {
            // Arrange
            var request = new FarmListRequest { Page = 1, PageSize = 10 };
            _repositoryMock
                .Setup(r =>
                    r.GetPagedAsync(
                        It.IsAny<ISpecification<Farm, FarmDto>>(),
                        1,
                        10,
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(
                    new PagedResult<FarmDto> { Items = new List<FarmDto>(), TotalItems = 0 }
                );

            // Act
            var result = await _sut.GetAllFarmsAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().BeEmpty();
            result.Meta!.TotalItems.Should().Be(0);

            _repositoryMock.Verify(
                r =>
                    r.GetPagedAsync(
                        It.Is<ISpecification<Farm, FarmDto>>(s => s != null),
                        1,
                        10,
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllFarmsAsync_ShouldReturnError_WhenExceptionThrown()
        {
            // Arrange
            var request = new FarmListRequest { Page = 1, PageSize = 10 };
            _repositoryMock
                .Setup(r =>
                    r.GetPagedAsync(
                        It.IsAny<ISpecification<Farm, FarmDto>>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetAllFarmsAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().BeEmpty();
            result.Meta.Should().BeNull();

            _repositoryMock.Verify(
                r =>
                    r.GetPagedAsync(
                        It.Is<ISpecification<Farm, FarmDto>>(s => s != null),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<QueryType>()
                    ),
                Times.Once
            );
        }

        #endregion

        #region DeleteFarmAsync Tests

        [Fact]
        public async Task DeleteFarmAsync_ShouldReturnSuccess_WhenFarmExists()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var farm = new Farm
            {
                Id = farmId,
                Name = "Farm ABC",
                IsDeleted = false,
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(farmId, QueryType.ActiveOnly))
                .ReturnsAsync(farm);

            // Act
            var result = await _sut.DeleteFarmAsync(farmId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            farm.IsDeleted.Should().BeTrue();
            farm.DeletedAt.Should().NotBeNull();
            _repositoryMock.Verify(r => r.Update(farm), Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteFarmAsync_ShouldReturnNotFound_WhenFarmDoesNotExist()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(farmId, QueryType.ActiveOnly))
                .ReturnsAsync((Farm?)null);

            // Act
            var result = await _sut.DeleteFarmAsync(farmId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            _repositoryMock.Verify(r => r.Update(It.IsAny<Farm>()), Times.Never);
        }

        [Fact]
        public async Task DeleteFarmAsync_ShouldReturnNotFound_WhenFarmAlreadyDeleted()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var deletedFarm = new Farm { Id = farmId, IsDeleted = true };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(farmId, QueryType.ActiveOnly))
                .ReturnsAsync(deletedFarm);

            // Act
            var result = await _sut.DeleteFarmAsync(farmId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            _repositoryMock.Verify(r => r.Update(It.IsAny<Farm>()), Times.Never);
        }

        [Fact]
        public async Task DeleteFarmAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetByIdAsync(farmId, QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.DeleteFarmAsync(farmId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
        }

        #endregion

        #region UpdateFarmAsync Tests

        [Fact]
        public async Task UpdateFarmAsync_ShouldReturnSuccess_WhenValidInput()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var existingFarm = new Farm
            {
                Id = farmId,
                Name = "Old Name",
                Address = "Old Address",
                PhoneNumber = "0123456789",
                Email = "old@farm.com",
                IsDeleted = false,
            };
            var updateDto = new UpdateFarmDto
            {
                Name = "New Name",
                Address = "New Address",
                PhoneNumber = "0987654321",
                Email = "new@farm.com",
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(farmId, QueryType.ActiveOnly))
                .ReturnsAsync(existingFarm);
            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Farm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((Farm?)null); // No duplicate email

            // Act
            var result = await _sut.UpdateFarmAsync(farmId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingFarm.Name.Should().Be("New Name");
            existingFarm.Address.Should().Be("New Address");
            existingFarm.Email.Should().Be("new@farm.com");
            _repositoryMock.Verify(r => r.Update(existingFarm), Times.Once);
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateFarmAsync_ShouldTrimInputs()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var existingFarm = new Farm { Id = farmId, IsDeleted = false };
            var updateDto = new UpdateFarmDto
            {
                Name = "  New Name  ",
                Address = "  New Address  ",
                Email = "  new@farm.com  ",
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(farmId, QueryType.ActiveOnly))
                .ReturnsAsync(existingFarm);
            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Farm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync((Farm?)null);

            // Act
            var result = await _sut.UpdateFarmAsync(farmId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingFarm.Name.Should().Be("New Name");
            existingFarm.Address.Should().Be("New Address");
            existingFarm.Email.Should().Be("new@farm.com");
        }

        [Fact]
        public async Task UpdateFarmAsync_ShouldNotUpdateField_WhenNullOrWhitespace()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var existingFarm = new Farm
            {
                Id = farmId,
                Name = "Original Name",
                Address = "Original Address",
                IsDeleted = false,
            };
            var updateDto = new UpdateFarmDto { Name = null, Address = "" };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(farmId, QueryType.ActiveOnly))
                .ReturnsAsync(existingFarm);

            // Act
            var result = await _sut.UpdateFarmAsync(farmId, updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingFarm.Name.Should().Be("Original Name");
            existingFarm.Address.Should().Be("Original Address");
        }

        [Fact]
        public async Task UpdateFarmAsync_ShouldReturnNotFound_WhenFarmDoesNotExist()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var updateDto = new UpdateFarmDto { Name = "New Name" };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(farmId, QueryType.ActiveOnly))
                .ReturnsAsync((Farm?)null);

            // Act
            var result = await _sut.UpdateFarmAsync(farmId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.NotFound);
            _repositoryMock.Verify(r => r.Update(It.IsAny<Farm>()), Times.Never);
        }

        [Fact]
        public async Task UpdateFarmAsync_ShouldReturnConflict_WhenDuplicateEmailExists()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var existingFarm = new Farm
            {
                Id = farmId,
                Email = "old@farm.com",
                IsDeleted = false,
            };
            var anotherFarm = new Farm
            {
                Id = Guid.NewGuid(),
                Email = "taken@farm.com",
                IsDeleted = false,
            };
            var updateDto = new UpdateFarmDto { Email = "taken@farm.com" };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(farmId, QueryType.ActiveOnly))
                .ReturnsAsync(existingFarm);
            _repositoryMock
                .Setup(r =>
                    r.FirstOrDefaultAsync(
                        It.IsAny<Expression<Func<Farm, bool>>>(),
                        It.IsAny<QueryType>()
                    )
                )
                .ReturnsAsync(anotherFarm);

            // Act
            var result = await _sut.UpdateFarmAsync(farmId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Conflict);
            _repositoryMock.Verify(r => r.Update(It.IsAny<Farm>()), Times.Never);
        }

        [Fact]
        public async Task UpdateFarmAsync_ShouldReturnUnexpected_WhenExceptionThrown()
        {
            // Arrange
            var farmId = Guid.NewGuid();
            var updateDto = new UpdateFarmDto { Name = "New Name" };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(farmId, QueryType.ActiveOnly))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _sut.UpdateFarmAsync(farmId, updateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Type.Should().Be(ResultType.Unexpected);
        }

        #endregion
    }
}
