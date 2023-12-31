﻿using Natom.ATSA.Afiliados.Managers;
using Natom.ATSA.Afiliados.Models;
using Natom.ATSA.Afiliados.Tools;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Natom.ATSA.Afiliados
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer<DbAfiliadosContext>(null);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(HttpContext.Current.Request.CurrentExecutionFilePathExtension))
            {
                HttpContextBase context = new HttpContextWrapper(HttpContext.Current);
                string controllerName = "";
                string actionName = "";
                RouteData rd = null;
                try
                {
                    rd = RouteTable.Routes.GetRouteData(context);
                    controllerName = rd.GetRequiredString("controller");
                    actionName = rd.GetRequiredString("action");
                }
                catch
                {
                }
                finally
                {
                    if (rd != null && !string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName))
                    {
                        if (!(controllerName.ToLower().Equals("home") && actionName.ToLower().Equals("login")) && !(controllerName.ToLower().Equals("usuarios") && actionName.ToLower().Equals("recuperodeclave")) && !(controllerName.ToLower().Equals("usuarios") && actionName.ToLower().Equals("enviarmailrecupero")))
                        {
                            HttpCookie cookie = Request.Cookies["ATSAConafWebApp"];
                            if (cookie == null)
                            {
                                //Response.Redirect("/afiliadostest/Home/Login");
                                Response.Redirect("/cupones/Home/Login");
                                Response.End();
                            }
                        }
                        
                    }
                }
            }
        }
    }
}