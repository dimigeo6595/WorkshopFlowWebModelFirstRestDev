using AutoMapper;
using WorkshopFlow.DTO;
using WorkshopFlow.Exceptions;
using WorkshopFlow.Models;
using WorkshopFlow.Repositories;

namespace WorkshopFlow.Services
{
    public class MachineService : IMachineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MachineService> _logger;

        public MachineService(IUnitOfWork unitOfWork, IMapper mapper,
            ILogger<MachineService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<MachineReadOnlyDTO>> GetMachinesByWorkstationAsync(int workstationId)
        {
            // Επιχειρησιακός κανόνας: το workstation πρέπει να υπάρχει
            var workstation = await _unitOfWork.WorkstationRepository.GetByIdAsync(workstationId)
                ?? throw new EntityNotFoundException("Workstation",
                    $"Workstation with id {workstationId} not found");

            var machines = await _unitOfWork.MachineRepository
                .GetMachinesByWorkstationAsync(workstationId);

            _logger.LogInformation("Retrieved {Count} machines for workstation {Id}",
                machines.Count(), workstationId);
            return _mapper.Map<IEnumerable<MachineReadOnlyDTO>>(machines);
        }

        public async Task<MachineReadOnlyDTO> GetMachineByIdAsync(int id)
        {
            var machine = await _unitOfWork.MachineRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Machine",
                    $"Machine with id {id} not found");

            _logger.LogInformation("Machine with id {Id} found", id);
            return _mapper.Map<MachineReadOnlyDTO>(machine);
        }

        public async Task<MachineReadOnlyDTO> InsertMachineAsync(int workstationId, MachineInsertDTO dto)
        {
            // Επιχειρησιακός κανόνας: το workstation πρέπει να υπάρχει
            var workstation = await _unitOfWork.WorkstationRepository.GetByIdAsync(workstationId)
                ?? throw new EntityNotFoundException("Workstation",
                    $"Workstation with id {workstationId} not found");

            // Επιχειρησιακός κανόνας: μοναδικός κωδικός
            var existing = await _unitOfWork.MachineRepository.GetMachineByCodeAsync(dto.Code!);
            if (existing != null)
            {
                throw new EntityAlreadyExistsException("Machine",
                    $"Machine with code {dto.Code} already exists");
            }

            var machine = _mapper.Map<Machine>(dto);
            machine.WorkstationId = workstationId;

            await _unitOfWork.MachineRepository.AddAsync(machine);
            await _unitOfWork.SaveAsync();

            // Reload για navigation properties
            var createdMachine = await _unitOfWork.MachineRepository.GetByIdAsync(machine.Id);

            _logger.LogInformation("Machine {Code} created successfully", machine.Code);
            return _mapper.Map<MachineReadOnlyDTO>(createdMachine);
        }

        public async Task<MachineReadOnlyDTO> UpdateMachineAsync(int workstationId, int id, MachineUpdateDTO dto)
        {
            var machine = await _unitOfWork.MachineRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Machine",
                    $"Machine with id {id} not found");

            // Επιχειρησιακός κανόνας: η νέα workstation πρέπει να υπάρχει
            if (dto.WorkstationId!.Value != workstationId)
            {
                var newWorkstation = await _unitOfWork.WorkstationRepository
                    .GetByIdAsync(dto.WorkstationId!.Value)
                    ?? throw new EntityNotFoundException("Workstation",
                        $"Workstation with id {dto.WorkstationId} not found");
            }

            _mapper.Map(dto, machine);
            machine.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.MachineRepository.UpdateAsync(machine);
            await _unitOfWork.SaveAsync();

            var updatedMachine = await _unitOfWork.MachineRepository.GetByIdAsync(id);

            _logger.LogInformation("Machine with id {Id} updated successfully", id);
            return _mapper.Map<MachineReadOnlyDTO>(updatedMachine);
        }

        public async Task DeleteMachineAsync(int workstationId, int id)
        {
            var machine = await _unitOfWork.MachineRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("Machine",
                    $"Machine with id {id} not found");

            // Soft delete
            machine.IsDeleted = true;
            machine.DeletedAt = DateTime.UtcNow;
            machine.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.MachineRepository.UpdateAsync(machine);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Machine with id {Id} soft deleted", id);
        }
    }
}

