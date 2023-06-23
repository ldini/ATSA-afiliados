using Natom.ATSA.Afiliados.Managers;
using Natom.ATSA.Afiliados.Models.Afiliados;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Web;

namespace Natom.ATSA.Afiliados.Tools
{
    public static class TareasSegundoPlano
    {
        private static Timer timer;

        public static void Inicializar()
        {
            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Interval = 1000 * 60 * 30;    //CADA 30 MINUTOS
            timer.Enabled = true;
            LiquidarSiCorresponde();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            LiquidarSiCorresponde();
        }

        private static void LiquidarSiCorresponde()
        {
            //Configuracion configuracion = new ConfiguracionManager().ObtenerConfiguracion();

            //if (configuracion.LiquidacionAutomaticaActivada && configuracion.DiaLiquidacionAutomatica == DateTime.Now.Day)
            //{
            //    DateTime ultimaDiaMesALiquidar = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);
            //    int mes = ultimaDiaMesALiquidar.Month;
            //    int anio = ultimaDiaMesALiquidar.Year;

            //    LiquidacionManager liquidacionMgr = new LiquidacionManager();
            //    if (!liquidacionMgr.PeriodoLiquidado(mes, anio))
            //    {
            //        LiquidacionPagoHijosDiscapacitados liquidacion = liquidacionMgr.GenerarLiquidacion(mes, anio, -1, true, true);
            //        liquidacionMgr.EnviarLiquidacionPorMes(liquidacion.LiquidacionPagoHijosDiscapacitadosId, null);
            //    }
            //}
        }
    }
}