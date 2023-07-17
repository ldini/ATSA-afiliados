using Natom.ATSA.Afiliados.Models;
using Natom.ATSA.Afiliados.Models.ViewModels;
using Natom.ATSA.Afiliados.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Reporting.WebForms;
using System.Data.Entity;
using System.Web;
using System.Globalization;

namespace Natom.ATSA.Afiliados.Managers
{
    public class AfiliadoManager
    {
        private DbAfiliadosContext db;

        public AfiliadoManager()
        {
            this.db = new DbAfiliadosContext();
        }

        public Persona ObtenerAfiliado(string dni, string numero, int usuarioId)
        {
            var persona = this.db.Personas.FirstOrDefault(p => (p.NUMERO_AFILIADO.Equals(numero) || p.DOCUMENTO.Equals(dni)) && (p.ESTADO_ID == 2 || p.ESTADO_ID == 4)); //2: AFILIADO || 4: JUBILADO
            if (persona != null)
            {
                var dbLocal = new DbLocalAfiliadosContext();
                dbLocal.HistoricosConsultas.Add(new HistoricoConsultas()
                {
                    NumeroAfiliadoConsultado = persona.NUMERO_AFILIADO,
                    UsuarioId = usuarioId,
                    FechaHora = DateTime.Now
                });
                dbLocal.SaveChanges();
            }
            return persona;
        }

        public Establecimiento ObtenerEstablecimiento(long id)
        {
            return db.Establecimientos.Find(id);
        }

        public Cupones ObtenerCupones(int id)
        {
            return this.db.Cupones.Find(id);
            //return this.db.Cupones.Where(x => x.CuponAfiliadoNro == id.ToString()).FirstOrDefault();
        }

        public byte[] GenerarImprimirReciboPDFEnBytes(int id, string tipo)
        {
            string tipoCupon = "";
            switch (tipo)
            {
                case "1":
                    tipoCupon = "Prosamco";
                    break;
                case "2":
                    tipoCupon = "Ceproco";
                    break;
                case "3A":
                case "3B":
                case "3C":
                    tipoCupon = "Médicos de cabecera";
                    break;
            }       
            try
            {
                var data = this.ObtenerCupones(id);
                var reportData = new List<CuponesResult>();
                var persona = this.db.Personas.FirstOrDefault(p => (p.NUMERO_AFILIADO.Equals(id.ToString()))); // /*|| p.DOCUMENTO.Equals(dni)*/) && (p.ESTADO_ID == 2 || p.ESTADO_ID == 4)); //2: AFILIADO || 4: JUBILADO

                CultureInfo culture = new CultureInfo("es-ES");
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;

                reportData.Add(new CuponesResult
                {
                
                CuponId = 1,
                    CuponAfiliadoNombre = persona.NOMBRES, //data.CuponAfiliadoNombre,
                    CuponAfiliadoApellido = persona.APELLIDOS, //data.CuponAfiliadoApellido,
                    CuponAfiliadoNro = persona.NUMERO_AFILIADO, //data.CuponAfiliadoNro,
                    CuponOrdenNro = "1",
                    CuponFechaGeneracion = DateTime.Now.ToLongDateString(),
                    CuponCodigoPrestacion = "",
                    CuponCodigoPrestador = tipoCupon,
                    /*
                    CuponOrdenNro = data.CuponOrdenNro,
                    CuponFechaGeneracion = data.CuponFechaGeneracion,
                    CuponCodigoPrestacion = data.CuponCodigoPrestacion,
                    CuponCodigoPrestador  = data.CuponCodigoPrestador,
                     */
                    /*
                    CuponId = 1,
                    CuponAfiliadoNombre = persona.NOMBRES,
                    CuponAfiliadoApellido = persona.APELLIDOS,
                    CuponAfiliadoNro = persona.NUMERO_AFILIADO,
                    CuponOrdenNro = "1",
                    CuponFechaGeneracion = DateTime.Now.ToShortDateString(),
                    CuponCodigoPrestacion = "1",
                    CuponCodigoPrestador = "1",              
                    */
                });

                ReportViewer viewer = new ReportViewer();
                viewer.ProcessingMode = ProcessingMode.Local;
                //viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("/Reporting/Report" + tipo + ".rdlc");   //test
                viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("/cupones/Reporting/Report" + tipo + ".rdlc");   //produccion
                viewer.LocalReport.EnableExternalImages = true;
                viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", reportData));

                byte[] b = ReportHelper.ExportToPDF(viewer);

                viewer.Dispose();

                return b;
            }
            catch
            {
                return null;
            }
            
        }

        public List<Persona> ObtenerFamiliares(long AfiliadoId)
        {
            return this.db.Database.SqlQuery<Persona>("SELECT F.* FROM persona P INNER JOIN Familiar R ON R.AFILIADO_ID = P.ID INNER JOIN persona F ON F.ID = R.FAMILIAR_ID WHERE P.ESTADO_ID = 2 AND P.ID = {0}", AfiliadoId).ToList();
        }
    }
}