﻿using Natom.ATSA.Afiliados.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Natom.ATSA.Afiliados.Managers
{
    public static class EmailManager
    {
        public static void EnviarMailSetearClave(Usuario e)
        {
            var fromAddress = new MailAddress(ConfigurationManager.AppSettings["ATSA.Email.Emisor"], "ATSA - Sistema de consulta de afiliados");
            var toAddress = new MailAddress(e.Email, e.Nombre);
            string fromPassword = ConfigurationManager.AppSettings["ATSA.Email.Clave"];
            const string subject = "ATSA - Generación de clave";

            string link = ConfigurationManager.AppSettings["ATSA.SistemaURL"];
            if (link.Last() != '/')
            {
                link = String.Concat(link, "/");
            }
            link = String.Concat(link, "usuarios/recuperodeclave?u1du_22m2dl=", e.Token);

            string body = String.Format("<h2>ATSA - Sistema Generador de bonos y cupos </h2><br/><br/>Por favor, para <b>generar la clave de acceso al sistema</b> haga clic en el siguiente link: {0}", link);

            var smtp = new SmtpClient
            {
                Host = ConfigurationManager.AppSettings["SMTP.Host"],
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTP.Port"]),
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                IsBodyHtml = true,
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}