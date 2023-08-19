namespace repo_document
{
    public interface IDocumentRepository
    {
        Task<bool> SyncDocumentsFromExternalSource(string email);
    }

    public class DocumentRepository : IDocumentRepository
    {
        // environment parameters upon service creation
        int failTreshold = 3;

        public async Task<bool> SyncDocumentsFromExternalSource(string _)
        {
            int failCounter = 0;
            bool docuProcessing = true;

            while (docuProcessing)
            {
                try
                {
                    RollTheDice();
                    await Task.Delay(2000);
                }
                catch (Exception ex)
                {
                    failCounter++;

                    if (failCounter == failTreshold)
                    {
                        // FAIL DOCU HANDLING ROUTINES 
                        // - SEND LOGS (ie. GRAYLOG)
                        // - SEND ALERT NOTIFICATION TO SUPPORT TEAM (ie. AWS SQS)
                        // - IF AVAILABLE, USE ALTERNATIVE DOCU PROCESSING SERVICE

                        break;
                    }

                    continue;
                }

                docuProcessing = false;
            }

            return failCounter != failTreshold;
        }

        public static void RollTheDice()
        {
            // Generate a random number  
            var random = new Random();

            // A random number 0 or 1
            var failureDice = random.Next(2);

            if (failureDice < 1) throw new Exception("Chaos created - sorry");
        }

    }



  
}