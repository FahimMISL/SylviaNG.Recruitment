using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Departments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
        {
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(DepartmentCreateRequest request)
        {
            var exists = await _departmentRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Department", "Name", request.Name);

            var entity = request.ToEntity();
            await _departmentRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.DepartmentId;
        }

        public async Task UpdateAsync(long departmentId, DepartmentUpdateRequest request)
        {
            var entity = await _departmentRepository.GetByIdAsync(departmentId)
                ?? throw new NotFoundException("Department", departmentId);

            var nameTaken = await _departmentRepository.ExistsByNameAsync(request.Name, departmentId);
            if (nameTaken)
                throw new DuplicateException("Department", "Name", request.Name);

            entity.Name = request.Name;
            _departmentRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long departmentId)
        {
            var entity = await _departmentRepository.GetByIdAsync(departmentId)
                ?? throw new NotFoundException("Department", departmentId);

            var usageCount = await _departmentRepository.CountUsageAsync(departmentId);
            if (usageCount > 0)
                throw new ResourceInUseException("Department", departmentId, usageCount);

            _departmentRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<DepartmentResponse>> GetAllAsync()
        {
            var entities = await _departmentRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
