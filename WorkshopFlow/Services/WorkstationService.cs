using AutoMapper;
using WorkshopFlow.DTO;
using WorkshopFlow.Exceptions;
using WorkshopFlow.Models;
using WorkshopFlow.Repositories;

namespace WorkshopFlow.Services
{
    public class WorkstationService : IWorkstationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<WorkstationService> _logger;

        public WorkstationService(IUnitOfWork unitOfWork, IMapper mapper,
            ILogger<WorkstationService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<WorkstationReadOnlyDTO>> GetAllWorkstationsAsync()
        {
            var workstations = await _unitOfWork.WorkstationRepository.GetAllWorkstationsAsync();
            _logger.LogInformation("Retrieved {Count} workstations", workstations.Count());
            return _mapper.Map<IEnumerable<WorkstationReadOnlyDTO>>(workstations);
        }

        public async Task<WorkstationReadOnlyDTO> GetWorkstationByIdAsync(int id)
        {
            var workstation = await _unitOfWork.WorkstationRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Workstation",
                    $"Workstation with id {id} not found");

            _logger.LogInformation("Workstation with id {Id} found", id);
            return _mapper.Map<WorkstationReadOnlyDTO>(workstation);
        }

        public async Task<WorkstationReadOnlyDTO> InsertWorkstationAsync(WorkstationInsertDTO dto)
        {
            // Επιχειρησιακός κανόνας: μοναδικός κωδικός
            var existing = await _unitOfWork.WorkstationRepository
                .GetWorkstationByCodeAsync(dto.Code!);
            if (existing != null)
            {
                throw new EntityAlreadyExistsException("Workstation",
                    $"Workstation with code {dto.Code} already exists");
            }

            var workstation = _mapper.Map<Workstation>(dto);
            await _unitOfWork.WorkstationRepository.AddAsync(workstation);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Workstation {Code} created successfully", workstation.Code);
            return _mapper.Map<WorkstationReadOnlyDTO>(workstation);
        }

        public async Task<WorkstationReadOnlyDTO> UpdateWorkstationAsync(int id, WorkstationUpdateDTO dto)
        {
            var workstation = await _unitOfWork.WorkstationRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Workstation",
                    $"Workstation with id {id} not found");

            _mapper.Map(dto, workstation);
            workstation.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.WorkstationRepository.UpdateAsync(workstation);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Workstation with id {Id} updated successfully", id);
            return _mapper.Map<WorkstationReadOnlyDTO>(workstation);
        }

        public async Task DeleteWorkstationAsync(int id)
        {
            var workstation = await _unitOfWork.WorkstationRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Workstation",
                    $"Workstation with id {id} not found");

            // Soft delete
            workstation.IsDeleted = true;
            workstation.DeletedAt = DateTime.UtcNow;
            workstation.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.WorkstationRepository.UpdateAsync(workstation);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Workstation with id {Id} soft deleted", id);
        }
    }
}

