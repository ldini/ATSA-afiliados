var timeOutTypeAhead;
var hijos = {};
var tblIndex = -1;
var bancos = {};

$(document).ready(function () {

    $("#Mes, #Anio, .tipoCta, .tipoDoc").chosen();

    var oTable = $('#tbl').dataTable({
        "iDisplayLength": -1,
        "bProcessing": false,
        "bServerSide": false,
        "bLengthChange": false,
        "scrollY": "300px",
        "bFilter": false,
        "bSort": false,
        "bInfo": false,
        "bPaginate": false,
        "aoColumns": [
            { "bSortable": false, "bVisible": false },
            { "bSortable": false, "bVisible": false },
            { "bSortable": false, "bVisible": false },
            { "bSortable": false, "bVisible": false },
            { "bSortable": false, "bVisible": false },
            { "bSortable": false },
            { "bSortable": false },
            { "bSortable": false },
            { "bSortable": false },
            { "bSortable": false },
            { "bSortable": false },
            { "bSortable": false },
            { "bSortable": false },
            { "bSortable": false },
            { "bSortable": false, "mRender": function () { return '<div class="btn btn-danger btn-sm quitar"><span class="glyphicon glyphicon-remove"></span></div>' } }
        ]
    });

    $('#tbl').on("click", ".quitar", function () {
        var tblIndex = $("#tbl").dataTable().fnGetPosition($(this).parents("tr")[0]);
        $("#tbl").dataTable().fnUpdate("", tblIndex, 0, false);
        $("#tbl").dataTable().fnUpdate("", tblIndex, 1, false);
        $("#tbl").dataTable().fnUpdate("", tblIndex, 2, false);
        $("#tbl").dataTable().fnUpdate("", tblIndex, 3, false);
        $("#tbl").dataTable().fnUpdate("", tblIndex, 4, false);
        $("#tbl tr[index='" + tblIndex + "'] input").val("");
        $("#tbl tr[index='" + tblIndex + "'] input.monto").val($("#MontoDefault").val());
        $("#tbl tr[index='" + tblIndex + "'] .tipoDoc").val("2").trigger("chosen:updated");
        $("#tbl tr[index='" + tblIndex + "'] .tipoCta").val("").trigger("chosen:updated");
        $("#tbl tr[index='" + tblIndex + "'] .vencimiento").val("");
        $("#tbl tr[index='" + tblIndex + "']").removeClass("proximoVencer");
    });

    $('.hijo').typeahead({
        minLength: 1,
        items: 20,
        matcher: function (item) { return true; },
        source: function (query, process) {

            if (timeOutTypeAhead) {
                clearTimeout(timeOutTypeAhead);
            }

            timeOutTypeAhead = setTimeout(function () {

                hijos = {};

                $.ajax({
                    type: "POST",
                    url: '/Hijo/BuscarHijos',
                    data: JSON.stringify({
                        hijo: query
                    }),
                    dataType: "json",
                    contentType: "application/json",
                    success: function (data) {
                        objects = TypeAhead.constructMap(data.datos, hijos);
                        process(objects);
                    }
                });

            }, 500);
        },
        updater: function (item) {
            $("#HijoId").val(hijos[item].id);
            $("#AfiliadoId").val(hijos[item].afiliadoid);
            $("#Hijo").val(hijos[item].hijo);
            $("#Afiliado").val(hijos[item].afiliado);

            tblIndex = parseInt($($(($($(this)[0])[0]).$element[0]).parents("tr")[0]).attr("index"));

            $("#tbl").dataTable().fnUpdate(hijos[item].id, tblIndex, 1, false);
            $("#tbl").dataTable().fnUpdate(hijos[item].afiliadoid, tblIndex, 0, false);
            $("#tbl").dataTable().fnUpdate(hijos[item].hijo, tblIndex, 4, false);
            $("#tbl").dataTable().fnUpdate(hijos[item].afiliado, tblIndex, 3, false);

            $("tr[index='" + tblIndex + "'] .cuil").val(hijos[item].cuil);
            $("tr[index='" + tblIndex + "'] .observaciones").val(hijos[item].obs);
            $("tr[index='" + tblIndex + "'] .vencimiento").val(hijos[item].vencimiento).trigger("blur");
            if (hijos[item].cbu == null) {
                $("tr[index='" + tblIndex + "'] .cbu").focus();
            }
            else {
                $("tr[index='" + tblIndex + "'] .cbu").val(hijos[item].cbu);
                $("tr[index='" + tblIndex + "'] .tipoCta").val(hijos[item].tipoCtaId).trigger("chosen:updated");
                $("tr[index='" + tblIndex + "'] .banco").val(hijos[item].banco);

                $("#tbl").dataTable().fnUpdate(hijos[item].bancoid, tblIndex, 2, false);
            }

            return item;
        }
    });

    $('.banco').typeahead({
        minLength: 1,
        items: 20,
        matcher: function (item) { return true; },
        source: function (query, process) {
            bancos = {};

            $.ajax({
                type: "POST",
                url: '/Hijo/BuscarBancos',
                data: JSON.stringify({
                    bancos: query
                }),
                dataType: "json",
                contentType: "application/json",
                success: function (data) {
                    objects = TypeAhead.constructMap(data.datos, bancos);
                    process(objects);
                }
            });
        },
        updater: function (item, dom) {
            tblIndex = parseInt($($(($($(this)[0])[0]).$element[0]).parents("tr")[0]).attr("index"));    
            $("#BancoId").val(bancos[item].id);
            $("#Banco").val(bancos[item].label);

            $("#tbl").dataTable().fnUpdate(bancos[item].id, tblIndex, 2, false);

            return item;
        }
    });

    

    //$("#Mes, #Anio").change(function () {
    //    var mes = parseInt($("#Mes").val());
    //    var anio = parseInt($("#Anio").val());
    //    if (!isNaN(mes) && !isNaN(anio)) {
    //        $.blockUI({ message: '<h4>Procesando...</h4>' });
    //        location.href = "Liquidacion/Create?mes=" + mes.toString() + "&anio=" + anio.toString();
    //    }
    //});

    $('[data-toggle="tooltip"]').tooltip(); 

    if (parseInt($("#LiquidacionPagoHijosDiscapacitadosId").val()) != 0) {
        Mensajes.MostrarNotificacionIzquierda("<h2>Período ya liquidado</h2>", "error");
    }

    $('.vencimiento').datetimepicker({
        format: 'DD/MM/YYYY',
        widgetPositioning: {
            horizontal: 'auto',
            vertical: 'auto'
        }
    });

    $(".vencimiento").blur(function () {
        var fecha = $(this).val();

        if (fecha.length != 10) {
            return;
        }

        fecha = ObtenerDiasCalendario(fecha);
        hoy = parseInt($("#Hoy").val());

        if ((fecha - hoy) <= 30) {
            $($(this).parents("tr")[0]).addClass("proximoVencer");
        }
        else {
            $($(this).parents("tr")[0]).removeClass("proximoVencer");
        }
    });

    jQuery(document).on('DOMNodeInserted', "body", function (e) {
        var t = e.target;

        if ($(t).is(".dropdown-menu")) {
            var parent = $($(t).parents("td")[0]);
            var d = $(parent).find("input.form-control")[0];
            var top = $(d).position().top + 25;
            var top = $($(".form-control.hijo")[0]).offset().top * -1 + $(d).position().top + 277;
            $(t).attr("newtop", top.toString());
        }
    });
});

setInterval(function () {
    $.each($("[newtop]"), function (i, t) {
        $(t).css("top", $(t).attr("newtop") + "px").css("min-height", "270px");
    });
}, 150);

(function () {
    var ev = new $.Event('style'),
        orig = $.fn.css;
    $.fn.css = function () {
        $(this).trigger(ev);
        return orig.apply(this, arguments);
    }
})();

function ObtenerDiasCalendario(fecha) {
    var d = fecha.split("/");
    var i = parseInt($("#Hoy").val());
    if (d.length == 3) {
        var dia = parseInt(d[0]);
        var mes = parseInt(d[1]);
        var anio = parseInt(d[2]);
        i = parseInt(anio / 4) + (anio * 365) + (mes * 30) + dia;
    }
    return i;
}

function Cancelar() {
    Mensajes.MostrarSiNo("¿Seguro que desea cancelar?", function () {
        location.href = "liquidacion";
    });
}

function Grabar() {

    var obj = {
        LiquidacionPagoHijosDiscapacitadosId: 0,
        Mes: parseInt($("#Mes").val()),
        Anio: parseInt($("#Anio").val()),

        Items: []
    };

    if (isNaN(obj.Mes)) {
        Mensajes.MostrarError("Debe seleccionar el Mes.");
        return;
    }
    else if(isNaN(obj.Anio)) {
        Mensajes.MostrarError("Debe seleccionar el Año.");
        return;
    }

    var conerrores = false;

    $.each($("#tbl").dataTable().fnGetData(), function (i, row) {
        if (!isNaN(parseInt(row[0]))) {
            var item = {
                Mes: obj.Mes,
                Anio: obj.Anio,
                Monto: parseFloat($("#tbl tr[index='" + i.toString() + "'] .monto").val()),
                CBU: $("#tbl tr[index='" + i.toString() + "'] .cbu").val(),
                BancoId: parseInt(row[2]),
                Afiliado: row[3],
                Hijo: row[4],
                AfiliadoId: parseInt(row[0]),
                HijoId: parseInt(row[1]),
                TipoCuentaBancariaId: parseInt($("#tbl tr[index='" + i.toString() + "'] .tipoCta").val()),
                TipoDoc: parseInt($("#tbl tr[index='" + i.toString() + "'] .tipoDoc").val()),
                CUIL: $("#tbl tr[index='" + i.toString() + "'] .cuil").val(),
                Observaciones: $("#tbl tr[index='" + i.toString() + "'] .observaciones").val(),
                Email: null,//$("#tbl tr[index='" + i.toString() + "'] .email").val(),
                Vencimiento: $("#tbl tr[index='" + i.toString() + "'] .vencimiento").val()
            };

            //if (isNaN(item.Mes)) {
            //    Mensajes.MostrarError("Debe seleccionar Mes.");
            //    conerrores = true;
            //    return;
            //}

            //if (isNaN(item.Anio)) {
            //    Mensajes.MostrarError("Debe seleccionar Año.");
            //    conerrores = true;
            //    return;
            //}

            //if (isNaN(item.AfiliadoId)) {
            //    Mensajes.MostrarError("Falta seleccionar el hijo del afiliado.");
            //    conerrores = true;
            //    return;
            //}

            //if (item.CUIL == "") {
            //    Mensajes.MostrarError("Falta ingresar CUIL.");
            //    conerrores = true;
            //    return;
            //}

            //if (item.CBU == "") {
            //    Mensajes.MostrarError("Falta ingresar CBU.");
            //    conerrores = true;
            //    return;
            //}

            //if (isNaN(item.TipoCuentaBancariaId)) {
            //    Mensajes.MostrarError("Falta seleccionar el tipo de cuenta.");
            //    conerrores = true;
            //    return;
            //}

            //if (isNaN(item.BancoId)) {
            //    Mensajes.MostrarError("Falta seleccionar el banco.");
            //    conerrores = true;
            //    return;
            //}

            //if (item.Vencimiento.length != 10) {
            //    Mensajes.MostrarError("Debe seleccionar fecha de vencimiento.");
            //    conerrores = true;
            //    return;
            //}

            //if (isNaN(item.Monto)) {
            //    Mensajes.MostrarError("Falta ingresar monto.");
            //    conerrores = true;
            //    return;
            //}

            obj.Items.push(item);

        }
    });
    
    if (conerrores) {
        return;
    }

    if (obj.Items.length == 0) {
        Mensajes.MostrarError("Debe ingresar al menos un hijo para liquidar.");
        return;
    }

    $.ajax({
        type: "POST",
        url: '/Liquidacion/Grabar',
        data: JSON.stringify({
            liquidacion: obj
        }),
        dataType: "json",
        contentType: "application/json",
        beforeSend: function (xhr) {
            $.blockUI({ message: '<h4>Procesando...</h4>' });
        },
        success: function (data, status) {
            if (status == "success") {
                if (data.success) {
                    window.open("Liquidacion/Imprimir?id=" + data.liquidacionId.toString(), "_blank");
                    location.href = "Liquidacion";
                }
                else {
                    Mensajes.MostrarError(data.error);
                    $.unblockUI();
                }
            }
            else {
                Mensajes.MostrarError("Se ha producido un error. Comuníquese con el administrador de ATSA.");
                $.unblockUI();
            }
        }
    });
}