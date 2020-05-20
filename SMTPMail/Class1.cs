using System;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMTPMail
{
    public class SendEmail
    {

        /*
        -- enable CLR on the SQL Server
        EXEC sp_configure 'clr enabled', 1;  RECONFIGURE WITH OVERRIDE;

        -- add netstandard.dll to the sql server assemblies
        CREATE ASSEMBLY netstandard 
        AUTHORIZATION dbo 
        FROM 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\netstandard.dll' 
        WITH Permission_set = UNSAFE

        -- create an assembly for this dll
        CREATE ASSEMBLY SMTPMail from 'C:\Kwekel\SMTPMail.dll' WITH PERMISSION_SET = UNSAFE;

        --create a stored procedure to call the assembly
        //-----------------------------------------------------------------------------
        USE [msdb]
        GO

        SET ANSI_NULLS OFF
        GO

        SET QUOTED_IDENTIFIER OFF
        GO

        CREATE PROCEDURE[dbo].[spSendMail]
            @server[nvarchar] (256),
	        @port int,
	        @username[nvarchar] (256),
	        @password[nvarchar] (256),
	        @usessl int,
	        @from[nvarchar] (4000),
	        @recipients[nvarchar] (4000),
	        @subject[nvarchar] (4000),
	        @body[nvarchar] (4000),
	        @attachments[nvarchar] (4000)

        WITH EXECUTE AS CALLER
        AS
        EXTERNAL NAME[SMTPMail].[SMTPMail.SendEmail].[Send]
                GO
        //-----------------------------------------------------------------------------

        -- call the assembly using the above stored procedure
        exec[dbo].[spSendMail] @server='smtp.gmail.com', @port=587, @username='dewhead@gmail.com', @password='4QuajnCZ0SeeHW', @usessl=1, @from='dewhead@gmail.com', 
        @recipients='ledew2@gmail.com', @subject='Subject 5', @body='This is a test email. Please do not reply.', @attachments=''
 */

        public static Int32 Send(string server, int port, string username, string password, int usessl, string from, string recipients, string subject, string body, string attachments)
        {
            Int32 status = 0;
            SmtpClient client = new SmtpClient(server, port);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            if (String.IsNullOrEmpty(username))
            {
                //no authentication provided
                client.UseDefaultCredentials = true;
            }
            else
            {
                client.UseDefaultCredentials = false;
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(username, password);
                client.Credentials = credentials;
            }

            if (usessl==1)
            {
                client.EnableSsl = true;
            }
            else
            {
                client.EnableSsl = false;
            }

            try
            {
                var mail = new MailMessage();

                foreach (string addr in recipients.Split(','))
                {
                    mail.To.Add(addr);
                }

                if (!String.IsNullOrEmpty(attachments))
                {
                    foreach (string file in attachments.Split(','))
                    {
                        mail.Attachments.Add(new Attachment(file));
                    }
                }

                mail.From = new MailAddress(from);
                mail.Subject = subject;
                mail.Body = body;
                client.Send(mail);
                status = 0;            }
            catch (Exception ex)
            {
                status = -1;
            }

            return status;
        }
    }
}
