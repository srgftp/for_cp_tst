using System.Diagnostics;

namespace repo_email
{
    public interface IEmailRepository
    {
        Task Send(string email, string message);
    }

    public class EmailRepository : IEmailRepository
    {
        // environment parameters upon service creation
        int failTreshold = 3;

        

        public async Task Send(string _, string __)
        {
            int failCounter = 0;
            bool emailProcessing = true;

            while (emailProcessing)
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

                        // FAIL EMAIL HANDLING ROUTINES 
                        // - SEND LOGS (ie. GRAYLOG)
                        // - SEND ALERT NOTIFICATION TO SUPPORT TEAM (ie. AWS SQS)
                        // - IF AVAILABLE, USE ALTERNATIVE EMAIL SERVICE

                        break;
                    }

                    continue;
                }

                emailProcessing = false;
            }
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