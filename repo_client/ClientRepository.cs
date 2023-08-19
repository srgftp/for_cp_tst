using repo_client.Models;
using Microsoft.EntityFrameworkCore;
using repo_client.Data;
using repo_document;
using repo_email;

namespace repo_client
{
    public interface IClientRepository
    {
        Task<Client[]> Get();
        Task<Client> Get(string id);
        Task<bool> Create(Client client);
        Task<bool> Update(Client client);

        Task<Client[]> Search(string name);
    }

    public class ClientRepository : IClientRepository
    {
        private readonly DataContext dataContext;
        private readonly IEmailRepository emailRepository;
        private readonly IDocumentRepository documentRepository;

        public ClientRepository(DataContext dataContext, IEmailRepository emailRepository, IDocumentRepository documentRepository)
        {
            this.dataContext = dataContext;
            this.emailRepository = emailRepository;
            this.documentRepository = documentRepository;
        }

        public async Task<bool> Create(Client client)
        {
            try
            {
                await dataContext.AddAsync(client);

                // Assuming a sub-process is NOT a workflow breaker / show stopper / critical
                // process must go on
                // errors can be handled by a separate/independent process
                await emailRepository.Send(client.Email, "Hi there - welcome to my Carepatron portal.");

                // Assuming a sub-process is a workflow breaker / show stopper / critical
                // only commit changes for happy path
                // errors can be handled at different layer
                if (await documentRepository.SyncDocumentsFromExternalSource(client.Email))
                    await dataContext.SaveChangesAsync();
                else
                    return false;

            }
            catch (Exception)
            {
                // Todo: Send error detail logs (ie. GRAYLOG)
                return false;
            }
            return true;
        }

        public Task<Client[]> Get()
        {
            return dataContext.Clients.ToArrayAsync();
        }

        public Task<Client> Get(string id)
        {
            return dataContext.Clients.SingleAsync(x => x.Id == id);
        }


        public Task<Client[]> Search(string Name)
        {
            return dataContext.Clients
                .Where( x => x.FirstName.ToUpper().Contains(Name.ToUpper()) 
                          || x.LastName.ToUpper().Contains(Name.ToUpper())
                          ).ToArrayAsync();
        }



        public async Task<bool> Update(Client client)
        {
            try
            {
                var existingClient = await dataContext.Clients.FirstOrDefaultAsync(x => x.Id == client.Id);

                if (existingClient == null)
                {
                    // Todo: Send null search logs (ie. GRAYLOG)
                    return false;
                }

                existingClient.FirstName = client.FirstName;
                existingClient.LastName = client.LastName;
                existingClient.Email = client.Email;
                existingClient.PhoneNumber = client.PhoneNumber;

                await dataContext.SaveChangesAsync();

                if (existingClient.Email != client.Email)
                {
                    await emailRepository.Send(client.Email, "Hi there - welcome to my Carepatron portal.");
                    await documentRepository.SyncDocumentsFromExternalSource(client.Email);
                }
            }
            catch (Exception)
            {
                // Todo: Send error detail logs (ie. GRAYLOG)
                return false;
            }
            return true;
        }
    }
}