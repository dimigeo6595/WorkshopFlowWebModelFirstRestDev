using AutoMapper;
using WorkshopFlow.DTO;
using WorkshopFlow.Exceptions;
using WorkshopFlow.Models;
using WorkshopFlow.Repositories;

namespace WorkshopFlow.Services
{
    public class RoutingStepService : IRoutingStepService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoutingStepService> _logger;

        public RoutingStepService(IUnitOfWork unitOfWork, IMapper mapper,
            ILogger<RoutingStepService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<RoutingStepReadOnlyDTO>> GetRoutingByItemIdAsync(int producedItemId)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(producedItemId)
                ?? throw new EntityNotFoundException("Item",
                    $"Item with id {producedItemId} not found");

            // Επιχειρησιακός κανόνας: μόνο παραγόμενα items έχουν Routing
            if (!item.IsManufactured)
            {
                throw new InvalidArgumentException("Item",
                    $"Item with id {producedItemId} is not a manufactured item and cannot have a Routing");
            }

            var steps = await _unitOfWork.RoutingStepRepository
                .GetRoutingByProducedItemIdAsync(producedItemId);

            _logger.LogInformation("Retrieved routing for item {Id}", producedItemId);
            return _mapper.Map<IEnumerable<RoutingStepReadOnlyDTO>>(steps);
        }

        public async Task<RoutingStepReadOnlyDTO> InsertRoutingStepAsync(int producedItemId,
            RoutingStepInsertDTO dto)
        {
            // Επιχειρησιακός κανόνας: το item πρέπει να υπάρχει και να είναι παραγόμενο
            var item = await _unitOfWork.ItemRepository.GetByIdAsync(producedItemId)
                ?? throw new EntityNotFoundException("Item",
                    $"Item with id {producedItemId} not found");

            if (!item.IsManufactured)
            {
                throw new InvalidArgumentException("Item",
                    $"Item with id {producedItemId} is not a manufactured item and cannot have a Routing");
            }

            // Επιχειρησιακός κανόνας: το workstation πρέπει να υπάρχει
            var workstation = await _unitOfWork.WorkstationRepository
                .GetByIdAsync(dto.WorkstationId!.Value)
                ?? throw new EntityNotFoundException("Workstation",
                    $"Workstation with id {dto.WorkstationId} not found");

            // Επιχειρησιακός κανόνας: η machine πρέπει να ανήκει στο workstation
            if (dto.MachineId.HasValue)
            {
                var machine = await _unitOfWork.MachineRepository.GetByIdAsync(dto.MachineId.Value)
                    ?? throw new EntityNotFoundException("Machine",
                        $"Machine with id {dto.MachineId} not found");

                if (machine.WorkstationId != dto.WorkstationId!.Value)
                {
                    throw new InvalidArgumentException("Machine",
                        $"Machine with id {dto.MachineId} does not belong to workstation {dto.WorkstationId}");
                }
            }

            // Επιχειρησιακός κανόνας: μοναδική sequence ανά item
            var sequenceExists = await _unitOfWork.RoutingStepRepository
                .SequenceExistsAsync(producedItemId, dto.Sequence!.Value);
            if (sequenceExists)
            {
                throw new EntityAlreadyExistsException("RoutingStep",
                    $"A step with sequence {dto.Sequence} already exists for this item");
            }

            var step = _mapper.Map<RoutingStep>(dto);
            step.ProducedItemId = producedItemId;

            await _unitOfWork.RoutingStepRepository.AddAsync(step);
            await _unitOfWork.SaveAsync();

            var createdStep = await _unitOfWork.RoutingStepRepository
                .GetRoutingStepAsync(producedItemId, step.Id);

            _logger.LogInformation("RoutingStep {Sequence} added to item {Id}",
                step.Sequence, producedItemId);
            return _mapper.Map<RoutingStepReadOnlyDTO>(createdStep);
        }

        public async Task<RoutingStepReadOnlyDTO> UpdateRoutingStepAsync(int producedItemId,
            int stepId, RoutingStepUpdateDTO dto)
        {
            var step = await _unitOfWork.RoutingStepRepository
                .GetRoutingStepAsync(producedItemId, stepId)
                ?? throw new EntityNotFoundException("RoutingStep",
                    $"RoutingStep with id {stepId} not found for item {producedItemId}");

            // Επιχειρησιακός κανόνας: το workstation πρέπει να υπάρχει
            var workstation = await _unitOfWork.WorkstationRepository
                .GetByIdAsync(dto.WorkstationId!.Value)
                ?? throw new EntityNotFoundException("Workstation",
                    $"Workstation with id {dto.WorkstationId} not found");

            // Επιχειρησιακός κανόνας: η machine πρέπει να ανήκει στο workstation
            if (dto.MachineId.HasValue)
            {
                var machine = await _unitOfWork.MachineRepository.GetByIdAsync(dto.MachineId.Value)
                    ?? throw new EntityNotFoundException("Machine",
                        $"Machine with id {dto.MachineId} not found");

                if (machine.WorkstationId != dto.WorkstationId!.Value)
                {
                    throw new InvalidArgumentException("Machine",
                        $"Machine with id {dto.MachineId} does not belong to workstation {dto.WorkstationId}");
                }
            }

            // Επιχειρησιακός κανόνας: sequence conflict check
            var sequenceExists = await _unitOfWork.RoutingStepRepository
                .SequenceExistsAsync(producedItemId, dto.Sequence!.Value, excludeStepId: stepId);
            if (sequenceExists)
            {
                throw new EntityAlreadyExistsException("RoutingStep",
                    $"A step with sequence {dto.Sequence} already exists for this item");
            }

            _mapper.Map(dto, step);
            step.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.RoutingStepRepository.UpdateAsync(step);
            await _unitOfWork.SaveAsync();

            var updatedStep = await _unitOfWork.RoutingStepRepository
                .GetRoutingStepAsync(producedItemId, stepId);

            _logger.LogInformation("RoutingStep {StepId} updated for item {ItemId}",
                stepId, producedItemId);
            return _mapper.Map<RoutingStepReadOnlyDTO>(updatedStep);
        }

        public async Task DeleteRoutingStepAsync(int producedItemId, int stepId)
        {
            var step = await _unitOfWork.RoutingStepRepository
                .GetRoutingStepAsync(producedItemId, stepId)
                ?? throw new EntityNotFoundException("RoutingStep",
                    $"RoutingStep with id {stepId} not found for item {producedItemId}");

            // Soft delete
            step.IsDeleted = true;
            step.DeletedAt = DateTime.UtcNow;
            step.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.RoutingStepRepository.UpdateAsync(step);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("RoutingStep {StepId} deleted from item {ItemId}",
                stepId, producedItemId);
        }
    }
}

