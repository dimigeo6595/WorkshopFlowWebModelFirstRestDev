using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using WorkshopFlow.Core;
using WorkshopFlow.Core.Filters;
using WorkshopFlow.DTO;
using WorkshopFlow.Exceptions;
using WorkshopFlow.Repositories;
using WorkshopFlow.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using WorkshopFlow.Models;

namespace WorkshopFlow.Services
{  
    public class UserService : IUserService
    {
        private readonly IEncryptionUtil _encryptionUtil;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _configuration;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, 
            ILogger<UserService> logger, IEncryptionUtil encryptionUtil, IConfiguration configuration)
        {
            _encryptionUtil = encryptionUtil;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<UserReadOnlyDTO> GetUserByUsernameAsync(string username)
        {
            
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            if (user == null)
            {
                throw new EntityNotFoundException("User", $"User with username: {username} not found");
            }

            _logger.LogInformation("User found: {Username}", username);
            return _mapper.Map<UserReadOnlyDTO>(user);
        }

        public async Task<UserReadOnlyDTO> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new EntityNotFoundException("User", $"User with id {id} not found");
            }

            _logger.LogInformation("User with id {Id} found", id);
            return _mapper.Map<UserReadOnlyDTO>(user);
        }

        public async Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(
            int pageNumber, int pageSize, UserFiltersDTO userFiltersDTO)
        {
            //List<User> users = [];
            List<Expression<Func<User, bool>>> predicates = [];

            if (!string.IsNullOrEmpty(userFiltersDTO.Username))
            {
                predicates.Add(u => u.Username == userFiltersDTO.Username);
            }
            if (!string.IsNullOrEmpty(userFiltersDTO.Email))
            {
                predicates.Add(u => u.Email == userFiltersDTO.Email);
            }
            if (!string.IsNullOrEmpty(userFiltersDTO.UserRole))
            {
                predicates.Add(u => u.Role.Name == userFiltersDTO.UserRole);
            }

            var result = await _unitOfWork.UserRepository.GetUsersAsync(pageNumber, pageSize, 
                predicates);

            var dtoResult = new PaginatedResult<UserReadOnlyDTO>()
            {
                Data = _mapper.Map<List<UserReadOnlyDTO>>(result.Data),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };

            _logger.LogInformation("Retrieved {Count} users", dtoResult.Data.Count);
            return dtoResult;
        }

        public async Task<User> VerifyAndGetUserAsync(UserLoginDTO credentials)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(credentials.Username);

            if (user == null || !_encryptionUtil.IsValidPassword(credentials.Password, user.Password))
            {
                //throw new EntityNotAuthorizedException("User", Resources.ErrorMessages.BadCredentials);
                throw new EntityNotAuthorizedException("User", "Bad Credentials");
            }

            _logger.LogInformation("User with username {Username} verified for login", credentials.Username);
            return user; 
        }

        public async Task<UserReadOnlyDTO> InsertUserAsync(UserInsertDTO dto)
        {
            var existingUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(dto.Username);
            if (existingUser != null)
            {
                throw new EntityAlreadyExistsException("User", $"User with username {dto.Username} already exists");
            }

            var user = _mapper.Map<User>(dto);
            user.Password = _encryptionUtil.Encrypt(dto.Password);

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("User {Username} created successfully", user.Username);
            return _mapper.Map<UserReadOnlyDTO>(user);
        }

        public async Task<UserReadOnlyDTO> UpdateUserAsync(int id, UserUpdateDTO dto)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("User", $"User with id {id} not found");

            _mapper.Map(dto, user);
            user.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("User with id {Id} updated successfully", id);
            return _mapper.Map<UserReadOnlyDTO>(user);
        }

        public async Task PatchUserAsync(int id, UserPatchDTO dto)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("User", $"User with id {id} not found");

            if (!string.IsNullOrEmpty(dto.Email))
            {
                user.Email = dto.Email;
            }

            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                if (!_encryptionUtil.IsValidPassword(dto.CurrentPassword!, user.Password))
                {
                    throw new EntityNotAuthorizedException("User", "Current password is incorrect");
                }
                user.Password = _encryptionUtil.Encrypt(dto.NewPassword);
            }

            user.ModifiedAt = DateTime.UtcNow;
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("User with id {Id} patched successfully", id);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException("User", $"User with id {id} not found");

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("User with id {Id} soft deleted", id);
        }

        public string CreateUserToken(User user)
        {
            var secretKey = _configuration["Jwt:Secret"]!;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsInfo = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            // Προσθέτουμε τα capabilities του role ως claims
            // Έτσι το API ξέρει τι επιτρέπεται σε κάθε request χωρίς να χρειάζεται νέο DB call
            foreach (var capability in user.Role.Capabilities)
            {
                claimsInfo.Add(new Claim("capability", capability.Name));
            }

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claimsInfo,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }
}
