using repo_client.Data;
using repo_client.Models;

namespace api
{
    public class DataSeeder
    {
        private readonly DataContext dataContext;

        public DataSeeder(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Seed()
        {
            dataContext.Add(new Client("xosiosiosdhad", "John", "Smith", "john@gmail.com", "+18202820232"));

            dataContext.Add(new Client("vboikjsisdmbr", "David", "Sinclair", "david@gmail.com", "+18209871452"));

            dataContext.Add(new Client("bwhopgusbjurk", "Valter", "Longo", "valter@gmail.com", "+18206547458"));

            dataContext.Add(new Client("rldjfbsjdbsqc", "Leonid", "Gavrilov", "leonid@gmail.com", "+18203219856"));

            dataContext.Add(new Client("wdxateyufivak", "Sunder", "Levis", "sunder@gmail.com", "+18207416325"));

            dataContext.Add(new Client("cerauafiugsfjh", "Dan", "Lucas", "dan@gmail.com", "+18208521254"));

            dataContext.Add(new Client("vfcyiagfahngvb", "Elton", "John", "elton@gmail.com", "+18209516485"));


            dataContext.SaveChanges();
        }
    }
}

