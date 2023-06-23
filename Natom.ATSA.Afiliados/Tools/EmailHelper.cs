using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Natom.ATSA.Afiliados.Tools
{
    public static class EmailHelper
    {
        public static bool Enviar(List<string> destinatarios, string titulo, string detalle, string[] filePathAdjuntos)
        {
            bool enviado = false;
            var fromAddress = new MailAddress(ConfigurationManager.AppSettings["ATSA.Email.Emisor"], titulo);
            string fromPassword = ConfigurationManager.AppSettings["ATSA.Email.Clave"];

            var smtp = new SmtpClient
            {
                Host = "mail.w1362013.ferozo.com", //"smtp.gmail.com",
                Port = 25, //587,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            MailMessage message = new MailMessage();
            message.From = message.Sender = fromAddress;
            message.IsBodyHtml = true;
            message.Subject = titulo;
            message.Body = detalle;

            foreach (string destinatario in destinatarios)
            { 
                message.To.Add(destinatario);
            }

            foreach (string path in filePathAdjuntos)
            {
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(path);
                message.Attachments.Add(attachment);
            }


            try
            {
                smtp.Send(message);
                enviado = true;
            }
            catch (Exception ex) { }

            return enviado;
        }
    }
}