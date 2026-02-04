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
                    FishTankId = mb.FishTankId,
                    FishTankName = mb.FishTank.Name,
                });
        }
    }
}
