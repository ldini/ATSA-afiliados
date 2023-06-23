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
        $("#tbl tr[index='" + tblIndex + "'] input").val("");
        $("#tbl tr[index='" + tblIndex + "'] input.monto").val($("#MontoDefault").val());
        $("#tbl tr[index='" + tblIndex + "'] .tipoDoc").val("2").trigger("chosen:updated");
        $("#tbl tr[index='" + tblIndex + "'] .tipoCta").val("").trigger("chosen:updated");
    });

    $('.hijo').typeahead({
        minLength: 1,
        items: 20,
        matcher: function (item) { return true; },
        source: function (query, process) {
            hijos = {};

            $.ajax({
                type: "POST",
                url: '/Hijo/BuscarAfiliados',
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
        },
        updater: function (item) {
            $("#AfiliadoId").val(hijos[item].afiliadoid);
            $("#Afiliado").val(hijos[item].afiliado);

            tblIndex = parseInt($($(($($(this)[0])[0]).$element[0]).parents("tr")[0]).attr("index"));

            $("#tbl").dataTable().fnUpdate(hijos[item].afiliadoid, tblIndex, 0, false);
            $("#tbl").dataTable().fnUpdate(hijos[item].afiliado, tblIndex, 2, false);

            $("tr[index='" + tblIndex + "'] .cuil").val(hijos[item].cuil);
            $("tr[index='" + tblIndex + "'] .cbu").focus();

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
    //        location.href = "Reintegro/Create?mes=" + mes.toString() + "&anio=" + anio.toString();
    //    }
    //});

    $('[data-toggle="tooltip"]').tooltip(); 

    //if (parseInt($("#ReintegroPagoHijosDiscapacitadosId").val()) != 0) {
    //    Mensajes.MostrarNotificacionIzquierda("<h2>Período ya liquidado</h2>", "error");
    //}
});

function Cancelar() {
    Mensajes.MostrarSiNo("¿Seguro que desea cancelar?", function () {
        location.href = "reintegro";
    });
}

function Grabar() {

    var obj = {
        ReintegroPagoHijosDiscapacitadosId: parseInt($("#ReintegroPagoHijosDiscapacitadosId").val()),
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
                BancoId: parseInt(row[1]),
                Afiliado: row[2],
                AfiliadoId: parseInt(row[0]),
                TipoCuentaBancariaId: parseInt($("#tbl tr[index='" + i.toString() + "'] .tipoCta").val()),
                TipoDoc: parseInt($("#tbl tr[index='" + i.toString() + "'] .tipoDoc").val()),
                CUIL: $("#tbl tr[index='" + i.toString() + "'] .cuil").val(),
                Observaciones: $("#tbl tr[index='" + i.toString() + "'] .observaciones").val(),
                Email: $("#tbl tr[index='" + i.toString() + "'] .email").val()
            };

            if (isNaN(item.Mes)) {
                Mensajes.MostrarError("Debe seleccionar Mes.");
                conerrores = true;
                return;
            }

            if (isNaN(item.Anio)) {
                Mensajes.MostrarError("Debe seleccionar Año.");
                conerrores = true;
                return;
            }

            if (isNaN(item.AfiliadoId)) {
                Mensajes.MostrarError("Falta seleccionar el hijo del afiliado.");
                conerrores = true;
                return;
            }

            if (item.CUIL == "") {
                Mensajes.MostrarError("Falta ingresar CUIL.");
                conerrores = true;
                return;
            }

            if (item.CBU == "") {
                Mensajes.MostrarError("Falta ingresar CBU.");
                conerrores = true;
                return;
            }

            if (item.CBU.length != 22 && item.CBU.length != 10) {
                Mensajes.MostrarError("El CBU debe poseer un largo de 22 u 10 caracteres.");
                conerrores = true;
                return;
            }

            if (isNaN(item.TipoCuentaBancariaId)) {
                Mensajes.MostrarError("Falta seleccionar el tipo de cuenta.");
                conerrores = true;
                return;
            }

            if (isNaN(item.BancoId)) {
                Mensajes.MostrarError("Falta seleccionar el banco.");
                conerrores = true;
                return;
            }

            if (isNaN(item.Monto)) {
                Mensajes.MostrarError("Falta ingresar monto.");
                conerrores = true;
                return;
            }

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
        url: '/Reintegro/Grabar',
        data: JSON.stringify({
            reintegro: obj
        }),
        dataType: "json",
        contentType: "application/json",
        beforeSend: function (xhr) {
            $.blockUI({ message: '<h4>Procesando...</h4>' });
        },
        success: function (data, status) {
            if (status == "success") {
                if (data.success) {
                    //window.open("/Reintegro/Imprimir?id=" + data.reintegroId.toString(), "_blank");
                    location.href = "/Reintegro";
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