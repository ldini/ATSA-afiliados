using Natom.ATSA.Afiliados.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;

namespace Natom.ATSA.Afiliados.Managers
{
    public class SesionManager
    {
        DbLocalAfiliadosContext db = new DbLocalAfiliadosContext();


        public bool ValidarLogin(string usuario, string clave, out int usuarioId)
        {
            usuarioId = 0;

            string userAdmin = ConfigurationManager.AppSettings["ATSA.Admin.Usuario"].ToLower();
            string userClave = ConfigurationManager.AppSettings["ATSA.Admin.Clave"];

            if (usuario.ToLower().Equals(userAdmin) && clave.Equals(userClave))
            {
                //ADMIN ATSA
                usuarioId = 0;
                return true;
            }
            else
            {
                List<Usuario> usuarios = this.db.Usuarios
                                                .Where(e => !e.FechaHoraBaja.HasValue && e.Email.ToLower().Equals(usuario.ToLower()))
                                                .ToList();

                if (usuarios.Count == 0)
                {
                    throw new Exception("Usuario inexistente");
                }
                else if (!usuarios.Any(e => !e.FechaHoraBaja.HasValue))
                {
                    throw new Exception("El usuario se encuentra deshabilitado para operar");
                }
                else if (usuarios.Any(e => !string.IsNullOrEmpty(e.Token)))
                {
                    throw new Exception("Debe primero confirmar el Email de registración y generar su clave para poder operar.");
                }
                else
                {
                    string claveMD5 = this.GenerarHashMD5(clave);
                    Usuario _usuario = usuarios.FirstOrDefault(e => !string.IsNullOrEmpty(e.Clave) && e.Clave.Equals(claveMD5));
                    //Establecimiento establecimiento = new EstablecimientoManager().ObtenerEstablecimiento(_usuario.EstablecimientoId);
                    if (_usuario == null)
                    {
                        throw new Exception("Clave incorrecta");
                    }
                    //else if (establecimiento.ACTIVO == 0)
                    //{
                    //    throw new Exception("El establecimiento se encuentra deshabilitado para operar");
                    //}
                    else
                    {
                        usuarioId = _usuario.UsuarioId;
                        return true;
                    }
                }
            }
        }

        private string GenerarHashMD5(string dato)
        {
            dato = dato ?? string.Empty;
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(dato);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }
    }
}