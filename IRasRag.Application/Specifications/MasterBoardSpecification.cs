using Ardalis.Specification;
using IRasRag.Application.DTOs;
using IRasRag.Domain.Entities;

namespace IRasRag.Application.Specifications
{
    public class MasterBoardDtoListSpec : Specification<MasterBoard, MasterBoardDto>
    {
        public MasterBoardDtoListSpec()
        {
            Query
                .AsNoTracking()
                .Include(mb => mb.FishTank)
                .Select(mb => new MasterBoardDto
                {
                    Id = mb.Id,
                    Name = mb.Name,
                    MacAddress = mb.MacAddress,
                    FishTankName = mb.FishTank.Name,
                });
        }
    }

    public class MasterBoardDtoListByTankIdSpec : Specification<MasterBoard, MasterBoardDto>
    {
        public MasterBoardDtoListByTankIdSpec(Guid tankId)
        {
            Query
                .AsNoTracking()
                .Where(mb => mb.FishTankId == tankId)
                .Select(mb => new MasterBoardDto
                {
                    Id = mb.Id,
                    Name = mb.Name,
                    MacAddress = mb.MacAddress,
                    FishTankName = mb.FishTank.Name,
                });
        }
    }
}
