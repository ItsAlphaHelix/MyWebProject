namespace MovieDG.Core.Services
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using MovieDG.Core.Contracts;
    using MovieDG.Core.ViewModels.Contact;
    using MovieDG.Data.Data.Models;
    using MoviesDG.Core.Messaging;
    using MoviesDG.Data.Data.Models;
    using MoviesDG.Data.Repositories;
    public class ContactService : IContactService
    {
        private readonly IRepository<Contact> contactsRepository;
        private readonly IEmailSender emailSender;

        public ContactService(
            IRepository<Contact> contactsRepository,
            IEmailSender emailSender)
        {
            this.contactsRepository = contactsRepository;
            this.emailSender = emailSender;
        }

        public async Task GetUserSubmisionAsync(ContactInputViewModel model)
        {
            var submision = new Contact()
            {
                Name = model.Name,
                Email = model.Email,
                Subject = model.Subject,
                Message = model.Message
            };

            await this.contactsRepository.AddAsync(submision);
            await this.contactsRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ContactViewModel>> GetSubmisionsAsync()
        {
            var submisions = await this.contactsRepository.
                AllAsNoTracking()
                .Select(x => new ContactViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    Subject = x.Subject,
                    Message = x.Message
                })
                .ToListAsync();

            return submisions;
        }

        public async Task<ContactViewModel> GetSubmisionByIdAsync(int id)
        {
            var submision = await this.contactsRepository
                 .AllAsNoTracking()
                 .Where(x => x.Id == id)
                 .Select(x => new ContactViewModel()
                 {
                     Id = x.Id,
                     Name = x.Name,
                     Email = x.Email,
                     Subject = x.Subject,
                     Message = x.Message
                 })
                 .FirstOrDefaultAsync();

            if (submision == null)
            {
                throw new NullReferenceException("The submision can not be null!");
            }

            return submision;
        }

        public async Task ReplyMessageToUserAsync(ReplyMessageViewModel replyModel)
        {
            await this.emailSender.SendEmailAsync
            (
                replyModel.AdminEmail,
                replyModel.Name,
                replyModel.ToUserEmail,
                replyModel.Subject,
                replyModel.Message
            );
        }

        public async Task DeleteQuestionAsync(int id)
        {
            var contact = await this.contactsRepository.AllAsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (contact == null)
            {
                throw new NullReferenceException("The contact can not be null");
            }

            this.contactsRepository.Delete(contact);

            await this.contactsRepository.SaveChangesAsync();
        }
    }
}
