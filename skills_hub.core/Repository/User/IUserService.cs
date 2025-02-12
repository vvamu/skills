﻿namespace skills_hub.core.Repository.User;

public interface IUserService
{
    public Task<ApplicationUser?> GetCurrentUserAsync();
    public Task<IQueryable<NotificationMessage>> GetCurrentUserNotifications();
    public Task<DTO.UserCreateDTO> GetCreateDTOByIdAsync(Guid id);
    public Task<ApplicationUser> GetByIdAsync(Guid id);
    public Task<IQueryable<ApplicationUser>> GetItems();

    public Task<bool> IsInRole(ApplicationUser user, string role);
    public Task<ApplicationUser> SignInAsync(DTO.UserLoginDTO item);
    public Task<ApplicationUser> SignInAsync(ApplicationUser item);
    public Task SignOutAsync();
    public Task<ApplicationUser> CreateAsync(DTO.UserCreateDTO user);
    public Task<ApplicationUser> UpdateAsync(DTO.UserCreateDTO item);

    public Task<ApplicationUser> HardDeleteAsync(ApplicationUser item);
    public Task<ApplicationUser> SoftDeleteAsync(ApplicationUser item);
    public Task<ApplicationUser> Restore(ApplicationUser item);
}




