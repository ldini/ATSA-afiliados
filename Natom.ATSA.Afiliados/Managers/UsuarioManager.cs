using Natom.ATSA.Afiliados.Models;
using Natom.ATSA.Afiliados.Models.Afiliados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;

namespace Natom.ATSA.Afiliados.Managers
{
    public class UsuarioManager
    {
        private DbLocalAfiliadosContext db = new DbLocalAfiliadosContext();

        public void GrabarPassword(long establecimientoId, string clave)
        {
            Usuario e = this.db.Usuarios.Find(establecimientoId);
            this.db.Entry(e).State = System.Data.Entity.EntityState.Modified;
            e.Clave = this.GenerarHashMD5(clave);
            e.Token = null;
            this.db.SaveChanges();
        }

        public Usuario ObtenerUsuario(int id)
        {
            if (id == 0)
            {
                return new Usuario()
                {
                    Nombre = "Admin",
                    EsAdmin = true
                };
            }
            return this.db.Usuarios
                            .FirstOrDefault(u => u.UsuarioId == id);
        }

        public Usuario ObtenerUsuarioPorToken(string u1du_22m2dl)
        {
            return this.db.Usuarios.FirstOrDefault(u => u.Token.Equals(u1du_22m2dl));
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

        public int ObtenerCantidadUsuarios()
        {
            return this.db.Usuarios.Count();
        }

        public IEnumerable<Usuario> ObtenerUsuariosConFiltros(string search)
        {
            search = search.ToLower();

            return this.db.Usuarios
                            .Where(u => !u.FechaHoraBaja.HasValue  && (u.Nombre.ToLower().Contains(search)
                                        || u.Apellido.ToLower().Contains(search)
                                        || u.Email.ToLower().Contains(search)));
        }

        public void Eliminar(int id, int accionUsuarioId)
        {
            Usuario e = this.db.Usuarios.Find(id);
            this.db.Entry(e).State = System.Data.Entity.EntityState.Modified;
            e.FechaHoraBaja = DateTime.Now;
            this.db.SaveChanges();

        }

        public void Grabar(Usuario usuario, int accionUsuarioId)
        {            
            if (usuario.UsuarioId == 0)
            {
                if (this.db.Usuarios.Any(u => !u.FechaHoraBaja.HasValue && u.Email.ToLower().Equals(usuario.Email.ToLower())))
                {
                    throw new Exception("Ya existe un usuario con mismo Email.");
                }

                usuario.FechaHoraAlta = DateTime.Now;
                usuario.Token = Guid.NewGuid().ToString().Replace("-", "");
                usuario.Clave = "";

                this.db.Usuarios.Add(usuario);
                this.db.SaveChanges();

                EmailManager.EnviarMailSetearClave(usuario);

            }
            else
            {
                if (this.db.Usuarios.Any(us => !us.FechaHoraBaja.HasValue && us.Email.ToLower().Equals(usuario.Email.ToLower()) && us.UsuarioId != usuario.UsuarioId))
                {
                    throw new Exception("Ya existe un usuario con mismo Email.");
                }

                Usuario u = this.db.Usuarios
                                    .First(s => s.UsuarioId == usuario.UsuarioId);
                this.db.Entry(u).State = System.Data.Entity.EntityState.Modified;

                u.Nombre = usuario.Nombre;
                u.Apellido = usuario.Apellido;
                u.Email = usuario.Email;



                this.db.SaveChanges();

            }
        }

        public void EnviarMailRecuperoDeClave(string email)
        {
            if (!this.db.Usuarios.Any(e => !e.FechaHoraBaja.HasValue && e.Email.ToLower().Equals(email.ToLower())))
            {
                throw new Exception("El Email ingresado no se encuentra registrado como Usuario.");
            }

            Usuario usuario = this.db.Usuarios.First(e => !e.FechaHoraBaja.HasValue && e.Email.ToLower().Equals(email.ToLower()));
            this.db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;

            usuario.Clave = null;

            usuario.Token = Guid.NewGuid().ToString().Replace("-", "");

            EmailManager.EnviarMailSetearClave(usuario);

            this.db.SaveChanges();
        }
    }
}