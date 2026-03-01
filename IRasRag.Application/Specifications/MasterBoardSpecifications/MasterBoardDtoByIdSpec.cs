using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications.MasterBoardSpecifications
{
    /// <summary>
    /// Specification chiếu một MasterBoard theo Id thành MasterBoardDto,
    /// bao gồm cả thông tin FishTank.
    /// </summary>
    public class MasterBoardDtoByIdSpec : Specification<MasterBoard, MasterBoardDto>
    {
        public MasterBoardDtoByIdSpec(Guid id)
        {
            Query
                .AsNoTracking()
                .Where(mb => mb.Id == id)
                .Select(mb => new MasterBoardDto
                {
                    Id = mb.Id,
                    Name = mb.Name,
                    MacAddress = mb.MacAddress,
                    FishTankId = mb.FishTankId,
                    FishTankName = mb.FishTank.Name,
                });
        }
    }
}
