using Natom.ATSA.Afiliados.Models.Discapacitados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Natom.ATSA.Afiliados.Tools;
using Natom.ATSA.Afiliados.Models.ViewModels;
using System.Configuration;
using System.Text;
using System.IO;
using Microsoft.Reporting.WebForms;
using Natom.ATSA.Afiliados.Reporting;

namespace Natom.ATSA.Afiliados.Managers
{
    public class ReintegroManager
    {
        private DbDiscapacitadosContext db = new DbDiscapacitadosContext();
        private DbAfiliadosContext dbAfiliados = new DbAfiliadosContext();

        public ReintegroPagoHijosDiscapacitados Grabar(ReintegroPagoHijosDiscapacitados reintegro, int accionUsuarioId)
        {
            if (reintegro.ReintegroPagoHijosDiscapacitadosId == 0)
            {
                if (this.db.Reintegros.Any(l => l.Mes == reintegro.Mes && l.Anio == reintegro.Anio && !l.Anulado))
                {
                    throw new Exception("Ya existe un reintegro para el período indicado.");
                }

                reintegro.Total = reintegro.Items.Sum(p => (decimal?)p.Monto) ?? 0;

                reintegro.Items.ForEach(t => t.Observaciones = (string.IsNullOrEmpty(t.Observaciones) ? "" : t.Observaciones));

                this.db.Reintegros.Add(reintegro);
                this.db.SaveChanges();

                HistorialCambiosManager.RegistrarCambios(accionUsuarioId, "ReintegroPagoHijosDiscapacitados", reintegro.ReintegroPagoHijosDiscapacitadosId, "ALTA");


                return reintegro;
            }
            else
            {
                if (this.db.Reintegros.Any(l => l.Mes == reintegro.Mes && l.Anio == reintegro.Anio && !l.Anulado && l.ReintegroPagoHijosDiscapacitadosId != reintegro.ReintegroPagoHijosDiscapacitadosId))
                {
                    throw new Exception("Ya existe un reintegro para el período indicado.");
                }

                ReintegroPagoHijosDiscapacitados reintegroDB = this.db.Reintegros.Include(l => l.Items).Where(l => l.ReintegroPagoHijosDiscapacitadosId == reintegro.ReintegroPagoHijosDiscapacitadosId).First();
                this.db.Entry(reintegroDB).State = EntityState.Modified;

                reintegroDB.Total = reintegro.Items.Sum(p => (decimal?)p.Monto) ?? 0;

                reintegro.Items.ForEach(t => t.Observaciones = (string.IsNullOrEmpty(t.Observaciones) ? "" : t.Observaciones));

                while (reintegroDB.Items.Count() > 0)
                {
                    this.db.ItemsReintegros.Remove(reintegroDB.Items[0]);
                }

                foreach (ItemReintegroPagoHijosDiscapacitados i in reintegro.Items)
                {
                    reintegroDB.Items.Add(i);
                }

                reintegroDB.Mes = reintegro.Mes;
                reintegroDB.Anio = reintegro.Anio;

                this.db.SaveChanges();

                HistorialCambiosManager.RegistrarCambios(accionUsuarioId, "ReintegroPagoHijosDiscapacitados", reintegro.ReintegroPagoHijosDiscapacitadosId, "MODIFICACIÓN");

                return reintegro;
            }
        }

        public List<TipoCuentaBancaria> ObtenerTiposCuentasBancarias()
        {
            return this.db.TiposCuentasBancarias.ToList();
        }

        public void EnviarLiquidacionPorMes(int reintegroId, int? usuarioId)
        {
            Configuracion configuracion = new ConfiguracionManager().ObtenerConfiguracion();
            ReintegroPagoHijosDiscapacitados reintegro = this.db.Reintegros.Find(reintegroId);

            if (configuracion.Destinatarios.Count() > 0)
            {
                string filePDF = this.GenerarListadoPDF(reintegroId);
                string fileTXT = this.GenerarListadoTXT(reintegroId);

                List<string> destinatarios = configuracion.Destinatarios.Select(d => d.Email).ToList();
                string titulo = "ATSA - Sistema de registro de beneficiarios con hijos especiales ONLINE";
                string detalle = String.Format("<h4>Reintegros: Liquidación automática correspondiente al período {0}/{1}</h4><br/><b>Archivos de reintegro adjuntos</b>", reintegro.Mes.ToString().PadLeft(2, '0'), reintegro.Anio);

                EmailHelper.Enviar(destinatarios, titulo, detalle, new string[] { filePDF, fileTXT });

                this.AsentarEnvioPorEmail(reintegroId, usuarioId, destinatarios);
            }
        }

        public bool PeriodoLiquidado(int mes, int anio)
        {
            return this.db.Reintegros.Any(l => !l.Anulado && l.Mes == mes && l.Anio == anio);
        }

        public ReintegroPagoHijosDiscapacitados ObtenerLiquidacion(int id)
        {
            return this.db.Reintegros
                            .Include(l => l.Items)
                            .Include(l => l.Items.Select(i => i.Banco))
                            .Include(l => l.Items.Select(i => i.TipoCuentaBancaria))
                            .First(l => l.ReintegroPagoHijosDiscapacitadosId == id);
        }

        public void AsentarEnvioPorEmail(int reintegroId, int? usuarioId, List<string> destinatarios)
        {
            if (destinatarios.Count > 0)
            {
                foreach (string destinatario in destinatarios)
                {
                    this.db.EnviosMailsReintegros.Add(new EnvioMailReintegroPagoHijosDiscapacitados()
                    {
                        Destinatario = destinatario,
                        EnviadoPorUsuarioId = usuarioId,
                        FechaHora = DateTime.Now,
                        ReintegroPagoHijosDiscapacitadosId = reintegroId
                    });
                }
                this.db.SaveChanges();

                if (usuarioId.HasValue)
                {
                    HistorialCambiosManager.RegistrarCambios(usuarioId.Value, "ReintegroPagoHijosDiscapacitados", reintegroId, "ENVIO MANUAL POR MAIL");
                }
            }
        }

        public void ConfirmarTransferencias(int id, int usuarioId)
        {
            string usuario = new UsuarioManager().ObtenerUsuario(usuarioId).NombreApellido;
            ReintegroPagoHijosDiscapacitados l = this.db.Reintegros.Find(id);
            this.db.Entry(l).State = EntityState.Modified;
            l.FechaHoraConfirmaTransferencia = DateTime.Now;
            l.ConfirmaTransferencia = usuario;
            this.db.SaveChanges();

            HistorialCambiosManager.RegistrarCambios(usuarioId, "ReintegroPagoHijosDiscapacitados", id, "CONFIRMACIÓN DE TRANSFERENCIA");
        }

        public void RechazarTransferencias(int id, int usuarioId, string motivo)
        {
            string usuario = new UsuarioManager().ObtenerUsuario(usuarioId).NombreApellido;
            ReintegroPagoHijosDiscapacitados l = this.db.Reintegros.Find(id);
            this.db.Entry(l).State = EntityState.Modified;
            l.FechaHoraCerrado = DateTime.Now;
            l.CerradoPor = usuario;
            this.db.SaveChanges();

            HistorialCambiosManager.RegistrarCambios(usuarioId, "ReintegroPagoHijosDiscapacitados", id, "RECHAZO DE TRANSFERENCIA", motivo);
        }

        public List<LiquidacionReportResult> ObtenerReintegroParaReporte(int id)
        {
            return this.db.Database.SqlQuery<LiquidacionReportResult>("CALL ReintegroReport({0})", id).ToList();
        }

        public string GenerarListadoTXT(int id)
        {
            ReintegroPagoHijosDiscapacitados reintegro = this.ObtenerLiquidacion(id);
            //reintegro.Items.ForEach(i => i.Afiliado = this.dbAfiliados.Personas.Find(i.AfiliadoId));

            string CBU_ATSA = ConfigurationManager.AppSettings["ATSA.CBU"];
            if (string.IsNullOrEmpty(CBU_ATSA))
            {
                throw new Exception("Falta definir el CBU de ATSA en el Web.Config.");
            }

            reintegro.Items = reintegro.Items.Where(e => e.Banco.Codigo == 191).GroupBy(i => new {
                Afiliado = i.Afiliado,
                AfiliadoId = i.AfiliadoId,
                Banco = i.Banco,
                BancoId = i.BancoId,
                CBU = i.CBU,
                CUIL = i.CUIL
            }, (k, v) => new ItemReintegroPagoHijosDiscapacitados()
            {
                Afiliado = k.Afiliado,
                AfiliadoId = k.AfiliadoId,
                Banco = k.Banco,
                BancoId = k.BancoId,
                CBU = k.CBU,
                CUIL = k.CUIL,
                Monto = v.Sum(i => i.Monto)
            }).ToList();

            StringBuilder data = new StringBuilder();
            foreach (ItemReintegroPagoHijosDiscapacitados item in reintegro.Items)
            {
                string entidadSucursal = item.CBU.PadLeft(22, '0').Substring(0, 8); //CBU_ATSA.Substring(0, 8);
                string cuentaAAcreditar = item.CBU.PadLeft(22, '0').Substring(8, 14); //CBU_ATSA.Substring(8, 14);
                string importeAAcreditar = item.Monto.ToString("F").Replace(",", "").PadLeft(15, '0');
                string tipoYDoc = ("2" + item.CUIL).PadRight(22, ' ');
                string beneficiario = item.Afiliado.PadLeft(40, ' ').ToUpper();
                string observaciones = "REINTEGROS ATSA".PadLeft(60, ' ');
                string concepto1 = "VAR";
                string concepto2 = "Varios".PadLeft(12, ' ');
                string correo = (string.IsNullOrEmpty(item.Email) ? "" : item.Email).PadLeft(50, ' ').ToUpper();
                if (beneficiario.Length > 40)
                {
                    beneficiario = beneficiario.Substring(0, 40);
                }

                if (correo.Length > 50)
                {
                    correo = correo.Substring(0, 50);
                }

                //data.AppendLine(String.Concat(
                //        entidadSucursal,
                //        cuentaAAcreditar,
                //        importeAAcreditar,
                //        tipoYDoc,
                //        beneficiario,
                //        observaciones,
                //        concepto1,
                //        concepto2,
                //        correo
                //    ));
                data.Append(String.Concat(
                        "1",
                        "191",
                        item.CBU,
                        item.CBU.Last(),
                        importeAAcreditar,
                        beneficiario,
                        item.CUIL.PadLeft(11, '0'),
                        observaciones,
                        correo,
                        '\n'
                    ));
            }

            string filePath = String.Concat(System.IO.Path.GetTempPath(), String.Format("CredicoopReintegros{0}-{1}-{2}.txt", reintegro.Mes.ToString().PadLeft(2, '0'), reintegro.Anio, DateTime.Now.Ticks));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            System.IO.File.WriteAllText(filePath, data.ToString());

            return filePath;
        }

        public string GenerarListadoTXT3ros(int id)
        {
            ReintegroPagoHijosDiscapacitados reintegro = this.ObtenerLiquidacion(id);
            //reintegro.Items.ForEach(i => i.Afiliado = this.dbAfiliados.Personas.Find(i.AfiliadoId));

            string CBU_ATSA = ConfigurationManager.AppSettings["ATSA.CBU"];
            if (string.IsNullOrEmpty(CBU_ATSA))
            {
                throw new Exception("Falta definir el CBU de ATSA en el Web.Config.");
            }

            reintegro.Items = reintegro.Items.Where(e => e.Banco.Codigo != 191).GroupBy(i => new {
                Afiliado = i.Afiliado,
                AfiliadoId = i.AfiliadoId,
                Banco = i.Banco,
                BancoId = i.BancoId,
                CBU = i.CBU,
                CUIL = i.CUIL,
                TipoDoc = i.TipoDoc,
                TipoCuentaBancaria = i.TipoCuentaBancaria
            }, (k, v) => new ItemReintegroPagoHijosDiscapacitados()
            {
                Afiliado = k.Afiliado,
                AfiliadoId = k.AfiliadoId,
                Banco = k.Banco,
                BancoId = k.BancoId,
                CBU = k.CBU,
                CUIL = k.CUIL,
                Monto = v.Sum(i => i.Monto),
                TipoDoc = k.TipoDoc,
                TipoCuentaBancaria = k.TipoCuentaBancaria
            }).ToList();

            StringBuilder data = new StringBuilder();
            foreach (ItemReintegroPagoHijosDiscapacitados item in reintegro.Items)
            {
                string importeAAcreditar = item.Monto.ToString("F").Replace(",", "").PadRight(10, '0');
                string afiliado = item.Afiliado.PadRight(40, ' ').ToUpper();
                if (afiliado.Length > 40)
                {
                    afiliado = afiliado.Substring(0, 40);
                }

                string observaciones = "AYUDA ECONOMICA PARA AFILIADOS ATSA".PadRight(60, ' ');
                string correo = (string.IsNullOrEmpty(item.Email) ? "" : item.Email).PadRight(50, ' ').ToUpper();
                if (correo.Length > 50)
                {
                    correo = correo.Substring(0, 50);
                }
                //data.AppendLine(String.Concat(
                //        item.TipoCuentaBancaria.Codigo,
                //        item.Banco.Codigo.ToString().PadLeft(3, '0'),
                //        item.CBU.PadLeft(22, '0').Substring(3, 4),
                //        item.CBU.PadLeft(22, '0').Substring(8, 13),
                //        item.CBU.PadLeft(22, '0').Substring(21, 1),
                //        importeAAcreditar,
                //        afiliado,
                //        item.CUIL.ToString().PadLeft(11, '0'),
                //        observaciones,
                //        correo
                //    ));
                data.Append(String.Concat(
                        item.CBU,
                        importeAAcreditar,
                        "1",
                        item.CUIL.ToString().PadLeft(11, '0'),
                        "          ",
                        afiliado,
                        observaciones,
                        "VAR",
                        "Varios".PadLeft(12, ' '),
                        correo,
                        '\n'
                    ));
            }

            string filePath = String.Concat(System.IO.Path.GetTempPath(), String.Format("CredicoopReintegrosOtrosBancos{0}-{1}-{2}.txt", reintegro.Mes.ToString().PadLeft(2, '0'), reintegro.Anio, DateTime.Now.Ticks));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            System.IO.File.WriteAllText(filePath, data.ToString());

            return filePath;
        }

        public string GenerarListadoPDF(int id)
        {
            ReintegroPagoHijosDiscapacitados reintegro = this.db.Reintegros.Find(id);
            List<LiquidacionReportResult> data = this.ObtenerReintegroParaReporte(id);

            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Local;
            viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reporting/ReintegroReport.rdlc");
            viewer.LocalReport.EnableExternalImages = true;
            viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));

            string filePath = String.Concat(System.IO.Path.GetTempPath(), String.Format("Reintegro{0}-{1}-{2}.pdf", reintegro.Mes.ToString().PadLeft(2, '0'), reintegro.Anio, DateTime.Now.Ticks));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            ReportHelper.ExportToPDF(viewer, filePath);

            viewer.Dispose();

            return filePath;
        }

        public byte[] GenerarListadoPDFEnBytes(int id)
        {
            List<LiquidacionReportResult> data = this.ObtenerReintegroParaReporte(id);

            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Local;
            viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reporting/ReintegroReport.rdlc");
            viewer.LocalReport.EnableExternalImages = true;
            viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));

            byte[] b = ReportHelper.ExportToPDF(viewer);

            viewer.Dispose();

            return b;
        }

        //public ReintegroPagoHijosDiscapacitados GenerarLiquidacion(int? mes, int? anio, int? usuarioId, bool grabar = false, bool automatica = false)
        //{
        //    ReintegroPagoHijosDiscapacitados l = new ReintegroPagoHijosDiscapacitados();

        //    if (mes != null && anio != null)
        //    {
        //        l = this.db.Reintegros
        //                        .Include(i => i.Items)
        //                        .Include(i => i.Items.Select(t => t.Banco))
        //                        .Include(i => i.Items.Select(t => t.Hijo))
        //                        .Where(i => i.Mes == mes && i.Anio == anio && !i.Anulado)
        //                        .FirstOrDefault();

        //        if (l == null)
        //        {
        //            l = new ReintegroPagoHijosDiscapacitados();
        //            l.Mes = mes.Value;
        //            l.Anio = anio.Value;
        //            l.Automatica = automatica;

        //            List<long> afiliadosId = this.dbAfiliados.Personas.Where(p => p.ACTIVO == true).Select(p => p.ID).ToList();
        //            DateTime desde = new DateTime(anio.Value, mes.Value, 1);
        //            DateTime hasta = new DateTime(anio.Value, mes.Value, 1).AddMonths(1).AddDays(-1);

        //            List<Hijo> hijosParaLiquidar = (from c in this.db.CertificadosDiscapacidad
        //                                            join h in this.db.Hijos on c.HijoId equals h.HijoId
        //                                            where (
        //                                                        (c.FechaEmision <= desde && c.FechaVencimiento >= hasta)
        //                                                        || (c.FechaEmision >= desde && c.FechaEmision <= hasta)
        //                                                        || (c.FechaVencimiento >= desde && c.FechaVencimiento <= hasta)
        //                                                  )
        //                                                    && !c.Anulado && !h.Anulado
        //                                            select h)
        //                                            .Include(k => k.Banco)
        //                                            .Include(k => k.Certificados)
        //                                            .ToList();

        //            hijosParaLiquidar = hijosParaLiquidar.Where(h => afiliadosId.Contains(h.AfiliadoId)).ToList();

        //            l.Items = new List<ItemReintegroPagoHijosDiscapacitados>();
        //            foreach (Hijo hijo in hijosParaLiquidar)
        //            {
        //                if (!l.Items.Any(n => n.HijoId == hijo.HijoId))
        //                {
        //                    l.Items.Add(new ItemReintegroPagoHijosDiscapacitados()
        //                    {
        //                        AfiliadoId = hijo.AfiliadoId,
        //                        Anio = anio ?? 0,
        //                        BancoId = hijo.BancoId ?? 0,
        //                        Banco = hijo.BancoId.HasValue ? hijo.Banco : new Banco(),
        //                        CBU = hijo.CBU,
        //                        Hijo = hijo.Hijo,
        //                        HijoId = hijo.HijoId,
        //                        Mes = mes ?? 0,
        //                        Monto = hijo.MontoATransferir,
        //                        Observaciones = ""
        //                    });

        //                    l.Total += hijo.MontoATransferir;
        //                }
        //            }

        //            if (grabar)
        //            {
        //                this.db.Reintegros.Add(l);
        //                this.db.SaveChanges();

        //                HistorialCambiosManager.RegistrarCambios(usuarioId ?? 0, "ReintegroPagoHijosDiscapacitados", l.ReintegroPagoHijosDiscapacitadosId, "ALTA");
        //            }

        //        }
        //        else
        //        {
        //            this.db.Entry(l).State = EntityState.Modified;
        //        }
        //    }

        //    if (l.Items == null)
        //    {
        //        l.Items = new List<ItemReintegroPagoHijosDiscapacitados>();
        //    }

        //    return l;
        //}

        public IEnumerable<ListarReintegrosResult> ObtenerReintegrosConFiltros(string search, int? mes = null, int? anio = null, bool incluirAnulados = false)
        {
            if (search != null)
            {
                search = search.ToLower();
            }

            int iSearch = 0;
            int.TryParse(search, out iSearch);

            IEnumerable<ListarReintegrosResult> reintegroes = this.db.Database.SqlQuery<ListarReintegrosResult>("CALL ListarReintegros({0}, {1})", mes, anio)
                            .Where(h => (incluirAnulados ? true : !h.Anulado) &&
                                (iSearch == 0 || (iSearch != 0 && (h.Mes.Equals(iSearch) || (h.Anio.Equals(iSearch)))))
                                );


            return reintegroes;
        }

        public void Eliminar(int id, int accionUsuarioId, string motivo)
        {
            ReintegroPagoHijosDiscapacitados reintegro = this.db.Reintegros.Find(id);
            this.db.Entry(reintegro).State = EntityState.Modified;
            reintegro.Anulado = true;
            this.db.SaveChanges();

            HistorialCambiosManager.RegistrarCambios(accionUsuarioId, "ReintegroPagoHijosDiscapacitados", reintegro.ReintegroPagoHijosDiscapacitadosId, "BAJA", motivo);
        }

        public int ObtenerCantidadReintegros()
        {
            return this.db.Reintegros.Count();
        }
    }
}